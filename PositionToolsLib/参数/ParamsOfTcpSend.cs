
using HalconDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PositionToolsLib.参数
{
    /// <summary>
    /// Tcp发送设置参数
    /// </summary>
    [Serializable]
    public class ParamsOfTcpSend : BaseParam
    {
                  
     
        /*************************Property*******************************/
        #region---Property---
        //Input parmas value define --1
       

        //Output parmas value define ---1

        string sendData = string.Empty;
        /// <summary>
        /// 发送数据
        /// </summary>   
        [Description("发送数据"), DefaultValue("")]
        public string SendData
        {
            get { return this.sendData; }
            set
            {
                sendData = value;
            
            }
        }

        //Run status value define----1
        bool tcpSendRunStatus = false;
        /// <summary>
        ///  TCP数据发送工具运行结果状态
        /// </summary>
        [Description("TCP数据发送工具运行结果状态"), DefaultValue(false)]
        public bool TcpSendRunStatus
        {
            get => tcpSendRunStatus;
            set
            {
                tcpSendRunStatus = value;
            
            }
        }
        #endregion

      
    }
}
