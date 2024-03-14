using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALSA.SaperaLT.SapClassBasic;

namespace DalsaCamera
{
    /// <summary>
    /// cameralink 相机
    /// </summary>
    public class DalsaCLDevice : IDalsaCam
    {

        public DalsaCLDevice()
        {

            _exposure = 50;
            _gain = 20;
            HOperatorSet.GenEmptyObj(out _image);
            HOperatorSet.GenEmptyObj(out _objDisp);
        }

        ~DalsaCLDevice()
        {
            _image?.Dispose();
            _objDisp?.Dispose();
            FreeDalsaCam();
        }
        public string DeviceName { get => _deviceName; set => _deviceName = value; }
        public string ConfigFileName { get => _configFileName; set => _configFileName = value; }
        public HObject Imgae { get => _image; set => _image = value; }
        public HObject ObjDisp { get => _objDisp; set => _objDisp = value; }
        public double Exposure { get => _exposure; set => _exposure = value; }
        public double Gain { get => _gain; set => _gain = value; }
        public bool CamIsOK { get => _bCamIsOK; set => _bCamIsOK = value; }

        private Action<HObject> procAction = null;
        /// <summary>
        /// 图像返回
        /// </summary>
        public event Action<HObject> OnProcImage
        {
            add { procAction += value; }
            remove { procAction -= value; }
        }

        private Action<HObject> showAction = null;
        /// <summary>
        /// 图像返回
        /// </summary>
        public event Action<HObject> OnShowImage
        {
            add { showAction += value; }
            remove { showAction -= value; }
        }

        private string _deviceName = null;//设备名(gige相机名/cameralink 采集卡名)
        private string _configFileName = null;//ccf路径

        private SapAcquisition m_Acquisition;
        private SapLocation m_SapLocation;    //设备连接地址
        private SapBuffer m_Buffers;           //缓存对象
        private SapAcqToBuf m_Xfer;   //传输对象

        private HObject _image;
        private HObject _objDisp;
        private bool _bCamIsOK;
        private double _exposure;
        private double _gain;
        /// <summary>
        /// 相机资源释放
        /// </summary>
        public void FreeDalsaCam()
        {
            DestroyObjects();
            DisposeObjects();
        }

        /// <summary>
        /// 相机初始化
        /// </summary>
        /// <param name="configpath"></param>
        /// <returns></returns>
        public bool InitCamera(string configpath)
        {
            _configFileName = configpath;
            m_Acquisition = null;
            m_Buffers = null;
            m_Xfer = null;
            if (CreateNewObjects(_deviceName, _configFileName))
            {
                _bCamIsOK = true;
            }
            else
            {
                _bCamIsOK = false;
            }
            return _bCamIsOK;
        }

        /// <summary>
        ///  曝光设置
        /// </summary>
        /// <param name="value"></param>
        public void SetExposure(double value)
        {
           
        }

        /// <summary>
        /// 增益设置
        /// </summary>
        /// <param name="value"></param>
        public void SetGain(double value)
        {
           
        }

        public bool Freeze()
        {
            return m_Xfer.Freeze();
        }
        public bool Grab()
        {
            return m_Xfer.Grab();
        }
        public bool Snap()
        {
            return m_Xfer.Snap();
        }

        /// <summary>
        ///  获取设备信息
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="cameraName"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        private bool GetCameraInfo(string deviceName, out string cameraName, out int nIndex)
        {
            cameraName = "";
            nIndex = 0;

            int serverCount = SapManager.GetServerCount();
            int GenieIndex = 0;
            System.Collections.ArrayList listServerNames = new System.Collections.ArrayList();

            bool bFind = false;
            string serverName = "";
            for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
            {
                if (SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) != 0)
                {
                    serverName = SapManager.GetServerName(serverIndex);
                    listServerNames.Add(serverName);
                    GenieIndex++;
                    bFind = true;
                }
            }

            int count = 0;
            string _deviceName = "";
            if (listServerNames.Count == 0)
                throw new Exception("未找到任何采集设备");
            foreach (string name in listServerNames)
            {
                _deviceName = SapManager.GetResourceName(name, SapManager.ResourceType.AcqDevice, 0);
                if (_deviceName == DeviceName)
                {
                    cameraName = name;
                    nIndex = count;
                }
                if (count > listServerNames.Count)
                {
                    throw new Exception("未找到设备名为" + DeviceName + "的相机");
                }
                count++;
            }

            //sCameraName = serverName;
            //nIndex = GenieIndex;

            return bFind;
        }

        /// <summary>
        /// 图像数据流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="argsNotify"></param>
        private void m_Xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
        {

            //首先需判断此帧是否是废弃帧，若是则立即返回，等待下一帧
            if (argsNotify.Trash) return;

            //获取m_Buffers的地址（指针）
            IntPtr addr;
            m_Buffers.GetAddress(out addr);

            //观察buffer中的图片的一些属性值，语句后注释里面的值是可能的值
            int count = m_Buffers.Count;  //2
            SapFormat format = m_Buffers.Format;  //Uint8
            double rate = m_Buffers.FrameRate;  //30.0，连续采集时，这个值会动态变化
            int height = m_Buffers.Height;  //
            int weight = m_Buffers.Width;  //
            int pixd = m_Buffers.PixelDepth;  //8


            _image.Dispose();
            //利用halcon从内存中采集图片
            HOperatorSet.GenImage1(out _image, "byte", m_Buffers.Width, m_Buffers.Height, addr);//取内存数据，生成图像，halcon实现
            showAction?.Invoke(_image);
            procAction?.Invoke(_image);
            m_Buffers.Clear();


        }

        /// <summary>
        /// 创建采集设备
        /// </summary>
        /// <param name="configFileName">相机的配置文件</param>
        /// <returns></returns>
        public bool CreateNewObjects(string deviceName, string configFileName)
        {
            //string Name = "";
            //int Index;
            //bool RTemp = GetCameraInfo(deviceName, out Name, out Index);
            //if (!RTemp)
            //{
            //    throw new Exception("Get camera info false!");
            //}

            m_SapLocation = new SapLocation(deviceName, 0);



            // 创建采集设备，new SapAcqDevice()的括号中第二个参数既可以写配置文件路径，
            // 也可以写false,false是用相机当前的设置,timeout参数无法保持

            //m_AcqDevice = new SapAcqDevice(m_ServerLocation, false);
            m_Acquisition = new SapAcquisition(m_SapLocation, configFileName);

            // 创建缓存对象
            if (SapBuffer.IsBufferTypeSupported(m_SapLocation, SapBuffer.MemoryType.ScatterGather))
            {
                m_Buffers = new SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGather);
            }
            else
            {
                m_Buffers = new SapBufferWithTrash(2, m_Acquisition, SapBuffer.MemoryType.ScatterGatherPhysical);
            }

            m_Xfer = new SapAcqToBuf(m_Acquisition, m_Buffers);
            // 创建传输对象

            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.XferNotify += new SapXferNotifyHandler(m_Xfer_XferNotify);
            m_Xfer.XferNotifyContext = this;
            if (!CreateObjects())
            {
                DisposeObjects();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        private bool CreateObjects()
        {
            //create acquisition object
            if (m_Acquisition != null && !m_Acquisition.Initialized)
            {
                if (false == m_Acquisition.Create())
                {
                    DestroyObjects();
                    return false;
                }

            }
            //create Buffer
            if (m_Buffers != null && !m_Buffers.Initialized)
            {
                if (m_Buffers.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }
            // Create Xfer object
            if (m_Xfer != null && !m_Xfer.Initialized)
            {
                if (m_Xfer.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 数据流缓存转halcon图像
        /// </summary>
        /// <returns></returns>
        public HObject BufToHobjectImage()
        {
            IntPtr tempBuffer;
            HObject ho_Image = null;
            try
            {
                m_Buffers.GetAddress(out tempBuffer);
                HOperatorSet.GenImage1(out ho_Image, "byte", m_Buffers.Width, m_Buffers.Height, tempBuffer);
                return ho_Image;
            }
            catch (Exception)
            {
                return ho_Image;
            }
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        private void DestroyObjects()
        {
            if (m_Xfer != null && m_Xfer.Initialized)
                m_Xfer.Destroy();
            if (m_Buffers != null && m_Buffers.Initialized)
                m_Buffers.Destroy();
            if (m_Acquisition != null && m_Acquisition.Initialized)
                m_Acquisition.Destroy();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private void DisposeObjects()
        {
            if (m_Xfer != null)
            { m_Xfer.Dispose(); m_Xfer = null; }
            if (m_Buffers != null)
            { m_Buffers.Dispose(); m_Buffers = null; }
            if (m_Acquisition != null)
            { m_Acquisition.Dispose(); m_Acquisition = null; }
        }
    }
}
