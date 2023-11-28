using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MvCamCtrl.NET;
using HalconDotNet;
using OSLog;

namespace FunctionLib.Cam
{
    /// <summary>
    /// 相机父类
    /// </summary>
    public abstract class CamBase
    {
        public CamBase(string userName)
        {
            camLog = new Log(userName);
            HOperatorSet.GenEmptyObj(out image);
            CamUserName = userName;
            DeviceListAcq();

        }
        public void Dispose()
        {
            CloseCam();
            if (image != null)
            {
                image.Dispose();
            }
        }
        /*//////////////////////////////*/
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList; //相机列表
        protected MyCamera m_pMyCamera;   //相机句柄
        MyCamera.cbOutputdelegate ImageCallback;//相机码流回调
        IntPtr pTemp = IntPtr.Zero;
        IntPtr pImageBuf = IntPtr.Zero;
        protected Log camLog;
        private HObject image = null;
        /*//////////////////////////////*/
        public event ImgGetHandle setImgGetHandle = null;//采集图像传递
        public event EventHandler CamConnectHnadle = null;//相机连接
        /*//////////////////////////////*/
        protected const bool CO_FAIL = false;
        protected const bool CO_OK = true;
        protected string CamUserName = "HKCam";
        /*//////////////////////////////*/
        int cam_index = -1; //相机索引编号
        bool isalive = false;
        int camNum = 0;
        long imageWidth = 0;
        long imageHeight = 0;
        bool m_bGrabbing;  //是否开启采集，启用码流
        bool m_bContinue;//是否连续采集中
        /******************    相机属性     ******************/
        /// <summary>
        /// 图像宽度
        /// </summary>
        public long ImageWidth
        {
            get
            { return this.imageWidth; }
        }
        /// <summary>
        /// 图像高度
        /// </summary>
        public long ImageHeight
        {
            get
            { return this.imageHeight; }
        }
        /// <summary>
        /// 相机数量
        /// </summary>
        public int CamNum
        {
            get

            { return this.camNum; }
        }
        /// <summary>
        /// 相机是否在线
        /// </summary>
        public bool IsAlive
        {
            get
            { return this.isalive; }
        }
        /// <summary>
        /// 相机索引
        /// </summary>
        public int CamIndex
        {
            get
            { return this.cam_index; }
        }
        /// <summary>
        /// 相机类型
        /// </summary>
        public CamType currCamType { get; set; }
        /// <summary>
        /// 相机是否采集中
        /// </summary>
        public bool IsGrabing
        {
            get
            { return this.m_bGrabbing; }
            set
            {
                this.m_bGrabbing = value;
            }
        }
        /// <summary>
        /// 是否连续采集中
        /// </summary>
        public bool IsContinueGrab
        {
            get
            { return this.m_bContinue; }
            set
            {
                this.m_bContinue = value;
            }
        }
        /******************    相机操作     ******************/
        /// <summary>
        /// 查找相机
        /// </summary>
        protected bool DeviceListAcq()
        {
            int nRet = -1;
            // ch:创建设备列表 || en: Create device list
            try
            {
                //string path =  AppDomain.CurrentDomain.BaseDirectory+ "MvCameraControl.Net.dll";
                //Assembly assem = Assembly.LoadFile(path);

                m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST(); //相机列表
                nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE| MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
                m_pMyCamera = new MyCamera();   //相机句柄
            }
            catch (Exception er)
            {

                return false;
            }

            if (0 != nRet)
            {

                return false;
            }
            camNum = (int)m_pDeviceList.nDeviceNum;
            return true;
        }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <param name="camIndex"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool OpenCam(int camIndex, ref string msg)
        {
            bool getcamfalg = DeviceListAcq();
            if (!getcamfalg)
            {
                msg += "相机查找失败";

                return false;//相机查找失败
            }
            if (m_pDeviceList.nDeviceNum == 0 || m_pDeviceList.nDeviceNum <= camIndex)
            {
                msg += "No device,please select";

                return false;
            }
            int nRet = -1;

            //ch:获取选择的设备信息 | en:Get selected device information
            MyCamera.MV_CC_DEVICE_INFO device =
                (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[camIndex],
                                                              typeof(MyCamera.MV_CC_DEVICE_INFO));
            //创建相机
            nRet = m_pMyCamera.MV_CC_CreateDevice_NET(ref device);
            if (MyCamera.MV_OK != nRet)
            {

                return false;
            }

            // ch:打开设备 | en:Open device
            nRet = m_pMyCamera.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {

                msg += "Open Device Fail";
                return false;
            }

            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();

            // ch:获取高 || en: Get Height
            nRet = m_pMyCamera.MV_CC_GetIntValue_NET("Height", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                msg += "Get Height Fail";
                return false;
            }
            imageHeight = (int)stParam.nCurValue;

            // ch:获取宽 || en: Get Width
            nRet = m_pMyCamera.MV_CC_GetIntValue_NET("Width", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                msg += "Get Width Fail";
                return false;
            }
            imageWidth = (int)stParam.nCurValue;

            //// ch:获取帧率 || en: Get AcquisitionFrameRate
            //MyCamera.MVCC_FLOATVALUE stParam2 = new MyCamera.MVCC_FLOATVALUE();
            //// nRet = m_pMyCamera.MV_CC_GetIntValue_NET("AcquisitionFrameRate", ref stParam);
            //nRet = m_pMyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam2);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    msg += "Get ResultingFrameRate Fail";
            //    return false;
            //}
            //d_stucamInfo.imgFPS = (int)FPS;

            // ch:设置触发模式为off || en:set trigger mode as off
            m_pMyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", 2);     //设置采集连续模式
            m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", 0);

            /**********************************************************************************************************/
            // ch:注册回调函数 | en:Register image callback
            ImageCallback = new MyCamera.cbOutputdelegate(GrabImage);
            // ImageCallback = new MyCamera.cbOutputExdelegate(GrabImage);
            nRet = m_pMyCamera.MV_CC_RegisterImageCallBack_NET(ImageCallback, IntPtr.Zero);
            // nRet = m_pMyCamera.MV_CC_RegisterImageCallBackForRGB_NET(ImageCallback, IntPtr.Zero);
            if (MyCamera.MV_OK != nRet)
            {
                msg += "Register Image CallBack Fail";
                return false;
            }
            /**********************************************************************************************************/
            isalive = true;
            cam_index = camIndex;
            if (CamConnectHnadle != null)
                CamConnectHnadle(isalive, null);
            return true;
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        /// <param name="msg"></param>
        public void StopGrab()
        {
            m_bContinue = false;
            if (!m_bGrabbing)
                return;
            int nRet = -1;
            // ch:停止抓图 || en:Stop grab image
            nRet = m_pMyCamera.MV_CC_StopGrabbing_NET();
            //if (nRet != MyCamera.MV_OK)
            //{
            //    msg += "Stop Grabbing Fail";
            //    return;
            //}
            m_bGrabbing = false;


        }
        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <returns></returns>
        public void CloseCam()
        {
            try
            {
                m_bContinue = false;
                if (!isalive)
                    return;
                if (m_bGrabbing)
                {
                    m_bGrabbing = false;
                    // ch:停止抓图 || en:Stop grab image
                    m_pMyCamera.MV_CC_StopGrabbing_NET();
                }

                //关闭相机
                m_pMyCamera.MV_CC_CloseDevice_NET();
                //销毁相机
                m_pMyCamera.MV_CC_DestroyDevice_NET();

                isalive = false;
                cam_index = -1;
                if (CamConnectHnadle != null)
                    CamConnectHnadle(isalive, null);

                if (pImageBuf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pImageBuf);
                    pImageBuf = IntPtr.Zero;
                }
            }
            catch(Exception er)
            { 
            
            }
        }
        /// <summary>
        /// 连续采集
        /// </summary>
        abstract public bool ContinueGrab();
        /// <summary>
        /// 单次采集
        /// </summary>
        abstract public bool OneShot();


        /****************  图像响应事件函数  ****************/
        /// <summary>
        /// 码流函数
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pFrameInfo"></param>
        /// <param name="pUser"></param>
        private void GrabImage(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser)
        {
            string strfucName = System.Reflection.MethodBase.GetCurrentMethod().ToString();
            // HObject Hobj = new HObject();

            int nImageBufSize = 0;
            int nRet = MyCamera.MV_OK;


            if (pData != null)
            {
                if (IsColorPixelFormat(pFrameInfo.enPixelType))
                {
                    HObject imgBuffer = null;
                    HOperatorSet.GenEmptyObj(out imgBuffer);
                    //////
                    if (pFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed)
                    {
                        pTemp = pData;
                    }
                    else
                    {
                        if (IntPtr.Zero == pImageBuf || nImageBufSize < (pFrameInfo.nWidth * pFrameInfo.nHeight * 3))
                        {
                            if (pImageBuf != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(pImageBuf);
                                pImageBuf = IntPtr.Zero;
                            }

                            pImageBuf = Marshal.AllocHGlobal((int)pFrameInfo.nWidth * pFrameInfo.nHeight * 3);
                            if (IntPtr.Zero == pImageBuf)
                            {
                                camLog.Error(strfucName, "图像源数据为空");
                                return;
                            }
                            nImageBufSize = pFrameInfo.nWidth * pFrameInfo.nHeight * 3;
                        }

                        MyCamera.MV_CC_PIXEL_CONVERT_PARAM stPixelConvertParam = new MyCamera.MV_CC_PIXEL_CONVERT_PARAM();

                        stPixelConvertParam.pSrcData = pData;//源数据
                        stPixelConvertParam.nWidth = pFrameInfo.nWidth;//图像宽度
                        stPixelConvertParam.nHeight = pFrameInfo.nHeight;//图像高度
                        stPixelConvertParam.enSrcPixelType = pFrameInfo.enPixelType;//源数据的格式
                        stPixelConvertParam.nSrcDataLen = pFrameInfo.nFrameLen;

                        stPixelConvertParam.nDstBufferSize = (uint)nImageBufSize;
                        stPixelConvertParam.pDstBuffer = pImageBuf;//转换后的数据
                        stPixelConvertParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
                        nRet = m_pMyCamera.MV_CC_ConvertPixelType_NET(ref stPixelConvertParam);//格式转换
                        if (MyCamera.MV_OK != nRet)
                        {
                            camLog.Error(strfucName, "图像格式转换错误");
                            return;
                        }
                        pTemp = pImageBuf;
                    }

                    try
                    {
                        HOperatorSet.GenEmptyObj(out image);
                        image.Dispose();
                        HOperatorSet.GenImageInterleaved(out image, (HTuple)pTemp, (HTuple)"rgb",
                            (HTuple)pFrameInfo.nWidth, (HTuple)pFrameInfo.nHeight, -1, "byte", 0, 0, 0, 0, -1, 0);
                        imgBuffer.Dispose();
                        HOperatorSet.CopyObj(image, out imgBuffer, 1, 1);
                        setImgGetHandle?.Invoke(imgBuffer);
                    }
                    catch (System.Exception ex)
                    {
                        camLog.Error(strfucName, ex.Message);
                    }

                }
                else if (IsMonoData(pFrameInfo.enPixelType))
                {
                    HObject imgBuffer = null;
                    HOperatorSet.GenEmptyObj(out imgBuffer);
                    //
                    if (pFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                    {
                        pTemp = pData;
                    }
                    else
                    {
                        if (IntPtr.Zero == pImageBuf || nImageBufSize < (pFrameInfo.nWidth * pFrameInfo.nHeight))
                        {
                            if (pImageBuf != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(pImageBuf);
                                pImageBuf = IntPtr.Zero;
                            }

                            pImageBuf = Marshal.AllocHGlobal((int)pFrameInfo.nWidth * pFrameInfo.nHeight);
                            if (IntPtr.Zero == pImageBuf)
                            {
                                camLog.Error(strfucName, "图像源数据为空");
                                return;
                            }
                            nImageBufSize = pFrameInfo.nWidth * pFrameInfo.nHeight;
                        }

                        MyCamera.MV_CC_PIXEL_CONVERT_PARAM stPixelConvertParam = new MyCamera.MV_CC_PIXEL_CONVERT_PARAM();

                        stPixelConvertParam.pSrcData = pData;//源数据
                        stPixelConvertParam.nWidth = pFrameInfo.nWidth;//图像宽度
                        stPixelConvertParam.nHeight = pFrameInfo.nHeight;//图像高度
                        stPixelConvertParam.enSrcPixelType = pFrameInfo.enPixelType;//源数据的格式
                        stPixelConvertParam.nSrcDataLen = pFrameInfo.nFrameLen;

                        stPixelConvertParam.nDstBufferSize = (uint)nImageBufSize;
                        stPixelConvertParam.pDstBuffer = pImageBuf;//转换后的数据
                        stPixelConvertParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                        nRet = m_pMyCamera.MV_CC_ConvertPixelType_NET(ref stPixelConvertParam);//格式转换
                        if (MyCamera.MV_OK != nRet)
                        {
                            camLog.Error(strfucName, "图像格式转换错误");
                            return;
                        }
                        pTemp = pImageBuf;
                    }
                    try
                    {
                        HOperatorSet.GenEmptyObj(out image);
                        image.Dispose();
                        //  HOperatorSet.GenImage1Extern(out image, "byte", pFrameInfo.nWidth, pFrameInfo.nHeight, pTemp, IntPtr.Zero);

                        HOperatorSet.GenImage1(out image, "byte", pFrameInfo.nWidth, pFrameInfo.nHeight, pTemp);



                        imgBuffer.Dispose();
                        HOperatorSet.CopyObj(image, out imgBuffer, 1, 1);
                        setImgGetHandle?.Invoke(imgBuffer);

                    }
                    catch (System.Exception ex)
                    {
                        camLog.Error(strfucName, ex.Message);
                    }

                }
                else
                {
                    camLog.Error(strfucName, "图像格式异常!");
                }

                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }

                GC.Collect();
                //   GC.GetTotalMemory(true);//需要实时将图像的内存回收掉，防止占用过多CPU资源控制系统垃圾回收器，专制内存不足   
            }
            //System.Threading.Thread.Sleep(5);
            return;
        }

        //判断是否为彩色图像
        private bool IsColorPixelFormat(MyCamera.MvGvspPixelType enType)
        {
            switch (enType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGBA8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGRA8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                    return true;
                default:
                    return false;
            }
        }
        //判断是否为黑白图像
        private Boolean IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;
                default:
                    return false;
            }
        }

        /******************    相机参数设置   ********************/
        //设曝光
        abstract public bool SetExposureTime(long dValue);
        //设置增益
        abstract public bool SetGain(long dValue);
        //查询曝光：dValue曝光值
        abstract public bool GetExposureTime(out long dValue);
        //查询增益：dValue增益值
        abstract public bool GetGain(out long dValue);
        //参数保存
        public bool SaveCamParmas()
        {
            string strfucName = System.Reflection.MethodBase.GetCurrentMethod().ToString();
            if (!isalive)
                return false;
            int nRet = m_pMyCamera.MV_CC_SetCommandValue_NET("UserSetSave");

            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /****************************************************************************
       * @fn           GetFloatValue
       * @brief        获取Float型参数值
       * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
       * @param        pValue                OUT       返回值
       * @return       成功：0；错误：-1
       ****************************************************************************/
        public bool GetFloatValue(string strKey, ref float pfValue)
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pMyCamera.MV_CC_GetFloatValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pfValue = stParam.fCurValue;

            return CO_OK;
        }
        public bool GetIntValue(string strKey, ref long pfValue)
        {
            MyCamera.MVCC_INTVALUE_EX stParam = new MyCamera.MVCC_INTVALUE_EX();
            int nRet = m_pMyCamera.MV_CC_GetIntValueEx_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pfValue = stParam.nCurValue;

            return CO_OK;
        }       

        public virtual bool GetExposureRangeValue(out long min, out long max)
        {
            min = 1000;
            max = 200000;
            if (!IsAlive)
                return false;
         
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pMyCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
            if (MyCamera.MV_OK != nRet)
                return false;
            //加减一防止越界
            max = (long)stParam.fMax-1;
            min = (long)stParam.fMin+1;

            return true;
        }

        public virtual bool GetGainRangeValue(out long min, out long max)
        {
            min = 0;
            max = 10;
            if (!IsAlive)
                return false;
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pMyCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);
            if (MyCamera.MV_OK != nRet)
                return false;
            //加减一防止越界
            max = (long)stParam.fMax-1;
            min = (long)stParam.fMin+1;

            return true;
        }


    }
    public struct stucamInfo
    {
        public int imgWidth;
        public int imgHeight;
        public int imgFPS;
    }
}
