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
    public class DVPCam : CamBase, Icam
    {    
        public DVPCam(string userName) : base(userName)
        {
            //camLog = new Log(CamUserName);
            //string path = AppDomain.CurrentDomain.BaseDirectory + "MvCameraControl.Net.dll";
            //bool loadFlag = LoadDLL(path);
            //if (!loadFlag)
            //    driveInfo = "无法加载 DLL";

            currCamType = CamType.度申;
         
        }

        /******************    相机操作     ******************/
        /// <summary>
        /// 连续采集
        /// </summary>
        override public bool ContinueGrab()
        {
            string strfucName = System.Reflection.MethodBase.GetCurrentMethod().ToString();
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
                IsGrabing = true;
                IsContinueGrab = true;
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
            m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", 1);
            // ch: 触发源设为软触发 || en: set trigger mode as Software
            m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerSource", 7);
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
            int nRet = m_pMyCamera.MV_CC_SetFloatValue_NET("ExposureTime", (float)dValue);
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
            int nRet = m_pMyCamera.MV_CC_SetFloatValue_NET("Gain", (float)dValue);

            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
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
