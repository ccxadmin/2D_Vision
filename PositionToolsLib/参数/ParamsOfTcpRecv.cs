
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
    /// Tcp接收设置参数
    /// </summary>  
    [Serializable]
    public class ParamsOfTcpRecv : BaseParam
    {

        /*************************Property*******************************/
        #region---Property---
        //Input parmas value define --0


        //Output parmas value define ---1

        string recieveData = string.Empty;
        /// <summary>
        /// 接收数据
        /// </summary>     
        [Description("接收数据"), DefaultValue("")]
        public string RecieveData
        {
            get { return this.recieveData; }
            set
            {
                recieveData = value;

            }
        }

        //Run status value define----1
        bool tcpRecvRunStatus = false;
        /// <summary>
        /// TCP数据接收工具运行结果状态
        /// </summary>
        [Description("TCP数据接收工具运行结果状态"), DefaultValue(false)]
        public bool TcpRecvRunStatus
        {
            get => tcpRecvRunStatus;
            set
            {
                tcpRecvRunStatus = value;
   
            }
        }
        #endregion

       
    }
}
