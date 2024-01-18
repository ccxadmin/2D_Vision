using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using CommunicationTools;
using CommunicationTools.Models;
using FunctionLib.TCP;
using OSLog;
using GlueDetectionLib.参数;

namespace GlueDetectionLib.工具
{
    [Category("通讯工具")]
    [DisplayName("Tcp发送")]
    [Description("工具开发用例，说明工具的作用！")]
    [Serializable]
    public class TcpSendTool : BaseTool, IDisposable
    {
        /// <summary>
        /// 工具构造函数
        /// </summary>
        public TcpSendTool()
        {
            toolParam = new ParamsOfTcpSend();
            toolName = "Tcp发送" + inum;
            inum++;
       
        }
        public void Dispose()
        {

        }
        /***************************Field*****************************/
        
        public string CommDevName = "";//通讯设备名称
        public static int inum = 0;//工具编号
        //协议格式
        private string sHead = "";
        private string sSplit = "";
        private string sTail = "";

        private List<string> dataList = new List<string> ();

        /****************************Method****************************/

    
        /// <summary>
        /// 传递数据接收工具给作业流程，以接收外部数据信号
        /// </summary>
        /// <param name="revDevName">通讯工具名称</param> 
        /// <returns></returns>
        private bool SetSendTool(string sendDevName)
        {
            if (string.IsNullOrEmpty(sendDevName)
                           )
                //设备同名
                return false;

            CommDevName = sendDevName;
            return CommDeviceController.g_CommDeviceList.
                     Exists(t => t.m_Name.Equals(CommDevName));

        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public bool SendData(string dat)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(dat);
            int e_index = CommDeviceController.g_CommDeviceList.
                              FindIndex(t => t.m_Name.Equals(CommDevName));
            if (e_index < 0) return false;
            if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == (CommDevType.TcpServer))
                ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).SendTo(bytes);
            else if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == CommDevType.TcpClient)
                ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).Send(bytes);
            return true;
        }

        public void setProtoolFormat(string head,string split,string tail)
        {
            sHead = head;
            sSplit = split;
            sTail = tail;
        }

        public void getProtoolFormat(ref string head, ref string split, ref string tail)
        {
            head =  sHead  ;
            split = sSplit ;
            tail  = sTail  ;
        }

        public void setDataList(List<string> datalist)
        {
            dataList = datalist;
        }

        public List<string> getDataList()
        {           
            return dataList ;
        }

        private string sendDataBuild(List<string> strings)
        {
            string data = "";
            data += sHead;

            for (int i = 0; i < strings.Count; i++)
            {
                data += strings[i];
                if(i == strings.Count - 1)
                    break;
                data += sSplit;
            }

            data += sTail;  

            return data;
        }
    
        /// <summary>
        /// 工具运行
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        override public RunResult Run()
        {
            //string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            RunResult rlt = new RunResult ();       
            Stopwatch m_ToolStopwatch = Stopwatch.StartNew();
            m_ToolStopwatch.Restart();

            DataManage dm = GetManage();         
            if (!dm.resultFlagDic.ContainsKey(toolName))
                dm.resultFlagDic.Add(toolName, true);
            else
                dm.resultFlagDic[toolName] = true;
            (toolParam as ParamsOfTcpSend).TcpSendRunStatus = true;

            m_ToolStopwatch.Stop();
            rlt.runTime = m_ToolStopwatch.ElapsedMilliseconds;
            rlt.runFlag = true;

            return rlt; 
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("Tcp发送", ""));
            if (number > inum)
                inum = number;

            inum++;
        }
    }
}
