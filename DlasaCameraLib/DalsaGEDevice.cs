﻿using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALSA.SaperaLT.SapClassBasic;

namespace DalsaCamera
{
    /// <summary>
    /// gige相机
    /// </summary>
    public class DalsaGEDevice : IDalsaCam
    {

        public DalsaGEDevice()
        {
            _exposure = 50;
            _gain = 20;
            HOperatorSet.GenEmptyObj(out _image);
            HOperatorSet.GenEmptyObj(out _objDisp);
        }
        ~DalsaGEDevice()
        {
            _image.Dispose();
            _objDisp.Dispose();
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
        private string _deviceName = null; //设备名(gige相机名/cameralink 采集卡名)
        private string _configFileName = null;//ccf路径

        private SapAcqDevice m_AcqDevice;
        private SapLocation m_SapLocation;    //设备连接地址
        private SapBuffer m_Buffers;           //缓存对象
        private SapAcqDeviceToBuf m_Xfer;   //传输对象

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
        /// 曝光设置
        /// </summary>
        /// <param name="value"></param>
        public void SetExposure(double value)
        {
            try
            {
                m_AcqDevice.SetFeatureValue("ExposureTime", value);

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        /// <summary>
        /// 增益设置
        /// </summary>
        /// <param name="value"></param>
        public void SetGain(double value)
        {
            try
            {


                m_AcqDevice.SetFeatureValue("Gain", value);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public bool Freeze()
        {
            if (null != m_Xfer)
                return m_Xfer.Freeze();
            else
                return false;
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
        /// 图像数据流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="argsNotify"></param>
        private void m_Xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
        {

            //判断此帧是否废弃帧
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
            //buffer 转halcon Image
            HOperatorSet.GenImage1(out _image, "byte", m_Buffers.Width, m_Buffers.Height, addr);
            showAction?.Invoke(_image);
            procAction?.Invoke(_image);
            m_Buffers.Clear();


        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="serverName"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        private bool GetCameraInfo(string deviceName, out string serverName, out int nIndex)
        {
            serverName = "";
            nIndex = 0;

            int serverCount = SapManager.GetServerCount();
            int GenieIndex = 0;
            System.Collections.ArrayList listServerNames = new System.Collections.ArrayList();

            bool bFind = false;
            string _serverName = "";
            for (int serverIndex = 0; serverIndex < serverCount; serverIndex++)
            {
                if (SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) != 0)
                {
                    _serverName = SapManager.GetServerName(serverIndex);
                    listServerNames.Add(_serverName);
                    GenieIndex++;
                    bFind = true;
                }
            }

            int count = 0;
            string _deviceName = "";
            if (listServerNames.Count == 0)
                throw new Exception("未找到任何采集设备");
            foreach (string sName in listServerNames)
            {
                _deviceName = SapManager.GetResourceName(sName, SapManager.ResourceType.AcqDevice, 0);
                if (_deviceName == deviceName)
                {
                    serverName = sName;
                    nIndex = count;
                }
                if (count > listServerNames.Count)
                {
                    throw new Exception("未找到设备名为" + deviceName + "的相机");
                }
                count++;
            }

            //sCameraName = serverName;
            //nIndex = GenieIndex;

            return bFind;
        }

        /// <summary>
        /// 创建采集设备
        /// </summary>
        /// <param name="configFileName">相机的配置文件</param>
        /// <returns></returns>
        private bool CreateNewObjects(string DeviceName, string configFileName)
        {
            string Name = "";
            int Index;
            bool RTemp = GetCameraInfo(DeviceName, out Name, out Index);
            if (!RTemp)
            {
                throw new Exception("Get camera info false!");
            }

            m_SapLocation = new SapLocation(Name, 0);

            // 创建采集设备，new SapAcqDevice()的括号中第二个参数既可以写配置文件路径
            // 也可使用默认 默认情况下相机不加载ccf 配置文件 直接读取硬件保持的参数(网口相机timeout 掉电不能保持)
            // m_AcqDevice = new SapAcqDevice(m_ServerLocation, false);
            m_AcqDevice = new SapAcqDevice(m_SapLocation, configFileName);
            // 创建缓存对象
            if (SapBuffer.IsBufferTypeSupported(m_SapLocation, SapBuffer.MemoryType.ScatterGather))
            {
                m_Buffers = new SapBufferWithTrash(1, m_AcqDevice, SapBuffer.MemoryType.ScatterGather);
            }
            else
            {
                m_Buffers = new SapBufferWithTrash(1, m_AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical);
            }


            m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);
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
            if (m_AcqDevice != null && !m_AcqDevice.Initialized)
            {
                if (false == m_AcqDevice.Create())
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
            IntPtr TempBuffer;
            HObject ho_Image = null;
            try
            {
                m_Buffers.GetAddress(out TempBuffer);
                HOperatorSet.GenImage1(out ho_Image, "byte", m_Buffers.Width, m_Buffers.Height, TempBuffer);
                return ho_Image;
            }
            catch (Exception ex)
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
            if (m_AcqDevice != null && m_AcqDevice.Initialized)
                m_AcqDevice.Destroy();
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
            if (m_AcqDevice != null)
            { m_AcqDevice.Dispose(); m_AcqDevice = null; }
        }

    }
}
