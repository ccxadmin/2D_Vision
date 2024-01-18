using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using HalconDotNet;
using FunctionLib.TCP;
using OSLog;
using CommunicationTools.Models;
using GlueDetectionLib.参数;
using CommunicationTools;

namespace GlueDetectionLib.工具
{
    [Category("通讯工具")]
    [DisplayName("Tcp接收")]
    [Description("工具开发用例，说明工具的作用！")]
    [Serializable]
    public class TcpRecvTool : BaseTool, IDisposable
    {
        /// <summary>
        /// 工具构造函数
        /// </summary>
        public TcpRecvTool()
        {
            toolParam = new ParamsOfTcpRecv();
            toolName = "Tcp接收" + inum;
            inum++;                   
        }
        public void Dispose()
        {

        }
        /***************************Field*****************************/
        public static int inum = 0;//工具编号
      
        //通讯设备对象
        //private CommDevInfo commDevInfo = new  CommDevInfo();
        public string CommDevName = "";//通讯设备名称
        //协议格式
        private string sHead = "";
        private string sSplit = "";
        private string sTail = "";

        public EventHandler WaitForRevDataHandle = null;

        //public delegate void EventOutRecvDataUpdate(List<string> datas);

        //[NonSerialized]
        //public EventOutRecvDataUpdate outRecvUpdateHandle = null;

        /****************************Method****************************/
       
        /// <summary>
        /// 传递数据接收工具给作业流程，以接收外部数据信号
        /// </summary>
        /// <param name="revDevName">通讯工具名称</param> 
        /// <returns></returns>
         bool SetRevTool(string revDevName)
        {
            if (string.IsNullOrEmpty(revDevName)
                           )
                //设备同名
                return false;
            
          
            //如果存在先注销事件
            if (CommDeviceController.g_CommDeviceList.Exists(t => t.m_Name.Equals(CommDevName)))
            {
                int index = CommDeviceController.g_CommDeviceList.
                               FindIndex(t => t.m_Name.Equals(CommDevName));


                //先清除旧的事件订阅
                if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                    ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= Tool_TcpServer_ReceiveData;
                else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                    ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= Tool_TcpClient_ReceiveData;
            }
            //重新订阅事件
            CommDevName = revDevName;
            int e_index = CommDeviceController.g_CommDeviceList.
                               FindIndex(t => t.m_Name.Equals(CommDevName));
            if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == (CommDevType.TcpServer))
                ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).ReceiveData += Tool_TcpServer_ReceiveData;
            else if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == CommDevType.TcpClient)
                ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).ReceiveData += Tool_TcpClient_ReceiveData;


            return true;
        }
        /// <summary>
        /// 移除外部通讯设备
        /// </summary>
        /// <param name="toolName"></param>
         void RemoveRevTool(string revDevName)
        {
            if (CommDevName != revDevName)
                return;
            //如果存在注销事件
            if (CommDeviceController.g_CommDeviceList.Exists(t => t.m_Name.Equals(CommDevName)))
            {
                int index = CommDeviceController.g_CommDeviceList.
                               FindIndex(t => t.m_Name.Equals(CommDevName));
                //先清除旧的事件订阅
                if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                    ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= Tool_TcpServer_ReceiveData;
                else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                    ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= Tool_TcpClient_ReceiveData;
            }
            //CommDevName = "";
        }
        /// <summary>
        /// 移除外部通讯设备
        /// </summary>
         void RemoveRevTool()
        {
            //如果存在注销事件
            if (CommDeviceController.g_CommDeviceList.Exists(t => t.m_Name.Equals(CommDevName)))
            {
                int index = CommDeviceController.g_CommDeviceList.
                               FindIndex(t => t.m_Name.Equals(CommDevName));
                //先清除旧的事件订阅
                if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                    ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= Tool_TcpServer_ReceiveData;
                else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                    ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= Tool_TcpClient_ReceiveData;
            }
            //CommDevName = "";
        }
        /// <summary>
        /// 服务端数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void Tool_TcpServer_ReceiveData(IPEndPoint remote, byte[] buffer, int length)
        {

            if (buffer == null || length <= 0 || buffer.Length < length)
                return;

            string datarv = System.Text.Encoding.Default.GetString(buffer);
            //bool flag = recvDataParse(buffer.Take(length).ToArray(), out List<string> datas);

            ((ParamsOfTcpRecv)toolParam).RecieveData = datarv;
        }
        /// <summary>
        /// 客户端数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void Tool_TcpClient_ReceiveData(IPEndPoint remote, byte[] buffer, int length)
        {
            if (buffer == null || length <= 0 || buffer.Length < length)
                return;

            string datarv = System.Text.Encoding.Default.GetString(buffer, 0, length);
            //bool flag = recvDataParse(buffer.Take(length).ToArray(), out List<string> datas);
            ((ParamsOfTcpRecv)toolParam).RecieveData = datarv;


        }
        /// <summary>
        /// 接收数据解析
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool recvDataParse(byte[] buffer, out List<string> datas)
        {
            datas = new List<string>();
            datas.Add(Encoding.Default.GetString(buffer, 0, buffer.Length));
            byte[] header = Encoding.Default.GetBytes(sHead);
            byte[] separator = Encoding.Default.GetBytes(sSplit);
            byte[] footer = Encoding.Default.GetBytes(sTail);
            List<byte> messageBytes = buffer.ToList();
            // 判断是否收到消息头
            if (header != null && messageBytes.Count >= header.Length
                    && messageBytes.Take(header.Length).SequenceEqual(header))
            {
                messageBytes.RemoveRange(0, header.Length);
            }

            // 判断是否收到消息尾
            if (footer != null && messageBytes.Count >= footer.Length
                && messageBytes.Skip(messageBytes.Count - footer.Length).SequenceEqual(footer))
            {
                messageBytes.RemoveRange(messageBytes.Count - footer.Length, footer.Length);
            }
            string temdat = Encoding.Default.GetString(messageBytes.ToArray(), 0, messageBytes.Count);
            datas.Clear();
            datas.Add(temdat);
            // 判断是否收到分隔符
            if (separator != null && messageBytes.Count >= separator.Length)
            {
                string separatorstring = Encoding.Default.GetString(separator, 0, separator.Length);
                datas = temdat.Split(separatorstring.ToCharArray()).ToList();
            }

            return true;
        }
        /// <summary>
        /// 获取数据格式
        /// </summary>
        /// <param name="head"></param>
        /// <param name="split"></param>
        /// <param name="tail"></param>
        public void setProtoolFormat(string head, string split, string tail)
        {
            sHead = head;
            sSplit = split;
            sTail = tail;
        }
        /// <summary>
        /// 设置数据格式
        /// </summary>
        /// <param name="head"></param>
        /// <param name="split"></param>
        /// <param name="tail"></param>
        public void getProtoolFormat(ref string head, ref string split, ref string tail)
        {
            head = sHead;
            split = sSplit;
            tail = sTail;
        }

        /// <summary>
        /// 工具运行
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        override public RunResult Run()
        {
            //string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            RunResult rlt = new RunResult();        
            Stopwatch m_ToolStopwatch = Stopwatch.StartNew();
            m_ToolStopwatch.Restart(); 
            DataManage dm = GetManage();
            WaitForRevDataHandle?.Invoke(null, null);
            if (!dm.resultFlagDic.ContainsKey(toolName))
                dm.resultFlagDic.Add(toolName, true);
            else
                dm.resultFlagDic[toolName] = true;
            (toolParam as ParamsOfTcpRecv).TcpRecvRunStatus = true;
            m_ToolStopwatch.Stop();
            rlt.runTime = m_ToolStopwatch.ElapsedMilliseconds;
            rlt.runFlag = true;

            return rlt;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            int number = int.Parse(toolName.Replace("Tcp接收", ""));
            if (number > inum)
                inum = number;
         
            inum++;
            //SetRevTool(CommDevName);
        }
       
    }
}
