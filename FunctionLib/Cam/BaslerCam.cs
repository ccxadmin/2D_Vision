using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvCamCtrl.NET;
using System.Runtime.InteropServices;
using HalconDotNet;
using System.Drawing;
using System.Drawing.Imaging;
using FilesRAW.Common;
using OSLog;
using System.Reflection;
using MvCamCtrl.NET;


namespace FunctionLib.Cam
{
    public class BaslerCam : CamBase, Icam
    {

        public BaslerCam(string userName) : base(userName)
        {
            //camLog = new Log(CamUserName);
            //string path = AppDomain.CurrentDomain.BaseDirectory + "MvCameraControl.Net.dll";
            //bool loadFlag = LoadDLL(path);
            //if (!loadFlag)
            //    driveInfo = "无法加载 DLL";

            currCamType = CamType.巴斯勒;
        }


        /******************    相机操作     ******************/

        /// <summary>
        /// 连续采集
        /// </summary>
        override public bool ContinueGrab()
        {
            string strfucName = System.Reflection.MethodBase.GetCurrentMethod().ToString();
            StopGrab();
            m_pMyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", 2);     //设置采集连续模式
            m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", 0);
            int nRet = 0;
            if (!IsGrabing)
                nRet = m_pMyCamera.MV_CC_StartGrabbing_NET(); // ch:开启抓图 | en:start grab                    
            if (MyCamera.MV_OK != nRet)
            {
                IsContinueGrab = false;
                camLog.Error(strfucName, "Start Grabbing Fail");
                return CO_FAIL;
            }
            else
            {
                IsContinueGrab = true;
                IsGrabing = true;
                return CO_OK;
            }

        }

        /// <summary>
        /// 单次采集
        /// </summary>
        /// <param name="msg"></param>
        override public bool OneShot()
        {
            string strfucName = System.Reflection.MethodBase.GetCurrentMethod().ToString();
            StopGrab();
            m_pMyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", 0);     //设置采集单帧模式
            m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", 0);
            // ch: 触发源设为软触发 || en: set trigger mode as Software
            //m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerSource", 7);
            int nRet = 0;
            // ch:开启抓图 | en:start grab
            if (!IsGrabing)
                nRet = m_pMyCamera.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                camLog.Error(strfucName, "Start Grabbing Fail");
                return false;
            }
            IsGrabing = true;
            // ch: 触发命令 || en: Trigger command
            nRet = m_pMyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");

            if (MyCamera.MV_OK != nRet)
            {
                camLog.Error(strfucName, "Trigger Fail");
                return CO_FAIL;
            }
            else
                return CO_OK;
        }

        /******************    相机参数设置   ********************/
        //设曝光
        override public bool SetExposureTime(long dValue)
        {

            if (!IsAlive)
                return false;
        
            m_pMyCamera.MV_CC_SetEnumValue_NET("ExposureAuto", 0);
            int nRet = m_pMyCamera.MV_CC_SetFloatValue_NET("ExposureTimeAbs", (float)dValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;        
        }

        //设置增益
        override public bool SetGain(long dValue)
        {
            if (!IsAlive)
                return false;
           
            m_pMyCamera.MV_CC_SetEnumValue_NET("GainAuto", 0);
            int nRet = m_pMyCamera.MV_CC_SetIntValueEx_NET("GainRaw", (int)dValue);

            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        override public  bool GetExposureRangeValue(out long min, out long max)
        {
            min = 1000;
            max = 200000;
            if (!IsAlive)
                return false;

            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pMyCamera.MV_CC_GetFloatValue_NET("ExposureTimeAbs", ref stParam);
            if (MyCamera.MV_OK != nRet)
                return false;
            //加减一防止越界
            max = (long)stParam.fMax - 1;
            min = (long)stParam.fMin + 1;

            return true;
        }

        override public  bool GetGainRangeValue(out long min, out long max)
        {
            min = 0;
            max = 10;
            if (!IsAlive)
                return false;
            MyCamera.MVCC_INTVALUE_EX stParam = new MyCamera.MVCC_INTVALUE_EX();
            int nRet = m_pMyCamera.MV_CC_GetIntValueEx_NET("GainRaw", ref stParam);
            if (MyCamera.MV_OK != nRet)
                return false;
            //加减一防止越界
            max = (long)stParam.nMax - 1;
            min = (long)stParam.nMin + 1;

            return true;
        }

        //查询曝光：dValue曝光值
        override public bool GetExposureTime(out long dValue)
        {

            dValue = -999;
            if (!IsAlive)
                return false;
            float fExposure = 0;
            if (!GetFloatValue("ExposureTime", ref fExposure))
                return false;
            dValue = (long)fExposure;
            return CO_OK;
        }

        //查询增益：dValue增益值
        override public bool GetGain(out long dValue)
        {
            dValue = -999;
            if (!IsAlive)
                return false;
            float fGain = 0;
            if (!GetFloatValue("Gain", ref fGain))
                return false;
            dValue = (long)fGain;
            return CO_OK;
        }
           
    }

}
