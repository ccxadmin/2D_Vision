using FunctionLib.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSLog;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FilesRAW.Common;

namespace CommunicationTools
{
    public class CommDeviceController
    {
        /// <summary>
        /// 通讯设备列表
        /// </summary>
        public static List<CommDevInfo> g_CommDeviceList = new List<CommDevInfo>();

        static Log log = new Log(" 通讯设备");

        #region tcp server

        /// <summary>
        /// 远程连接
        /// </summary>
        static public void TcpServer_RemoteConnect(string key)
        {
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            log.Info(funName,string.Format("客户端[{0}]上线", key));

        }

        /// <summary>
        /// 远程关闭
        /// </summary>
        /// <param name="key"></param>
        static public void TcpServer_RemoteClose(string key)
        {
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            log.Info(funName,string.Format("客户端[{0}]下线", key));

        }
        #endregion
        #region tcp client

        /// <summary>
        /// 远程关闭
        /// </summary>
        /// <param name="key"></param>
        static public void TcpClient_RemoteClose(string key)
        {
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            log.Info(funName,string.Format("服务端[{0}]下线", key));
            int i = 0;
            foreach (var item in CommDeviceController.g_CommDeviceList)
            {
                if (item.m_CommDevType.Equals(CommDevType.TcpClient))
                {
                    CommDevInfo tem = item;
                    if (((TcpSocketClient)item.obj).Remote.ToString() == key)
                    {
                        tem.status = EumStatus.未连接;
                        CommDeviceController.g_CommDeviceList[i] = tem;
                        break;
                    }
                }
                i++;
            }

        }
        #endregion

        /// <summary>
        /// 加载
        /// </summary>
        static public void LoadCommDev(string path)
        {
            List<CommDevInfo> tem = GeneralUse.ReadSerializationFile<List<CommDevInfo>>(path);
            //g_CommDeviceList = GeneralUse.ReadSerializationFile<List<CommDevInfo>>(path);
            if (tem != null)
                foreach (var s in tem)
                    if (!g_CommDeviceList.Exists(t => t.m_Name == s.m_Name))
                        g_CommDeviceList.Add(s);
        }
        /// <summary>
        /// 保存
        /// </summary>
        static public void SaveCommDev(string path)
        {
            GeneralUse.WriteSerializationFile<List<CommDevInfo>>(path, g_CommDeviceList);
        }
        /// <summary>
        ///  初始化连接
        /// </summary>
        /// <returns></returns>
        public static bool InitialConnect()
        {
            try
            {              
                   
                //通讯设备启动
                int i = 0;
                List<CommDevInfo> datas = DeepCopy2<List<CommDevInfo>>(CommDeviceController.g_CommDeviceList);
                foreach (var item in CommDeviceController.g_CommDeviceList)
                {
                    if (item.m_CommDevType.Equals(CommDevType.TcpServer))
                    {
                        ((TcpSocketServer)item.obj).RemoteConnect += TcpServer_RemoteConnect;
                        ((TcpSocketServer)item.obj).RemoteClose += TcpServer_RemoteClose;
                        if (!((TcpSocketServer)item.obj).State)
                        {
                            CommDevInfo tem = item;
                            if (!((TcpSocketServer)item.obj).Start(((TcpSocketServer)item.obj).IP,
                                ((TcpSocketServer)item.obj).Port))
                            {
                                tem.status = EumStatus.未连接;
                                log.Error("err","CommDeviceController.InitialConnect:" + "服务器监听失败！");
                              
                            }
                            else
                            {
                                tem.status = EumStatus.连接;
                            }

                            datas[i] = tem;
                        }

                    }
                    else if (item.m_CommDevType.Equals(CommDevType.TcpClient))
                    {
                        ((TcpSocketClient)item.obj).RemoteClose += TcpClient_RemoteClose;
                        if (!((TcpSocketClient)item.obj).State)
                        {
                            CommDevInfo tem = item;
                            if (!((TcpSocketClient)item.obj).Connect(((TcpSocketClient)item.obj).IP,
                                  ((TcpSocketClient)item.obj).Port))
                            {
                                tem.status = EumStatus.未连接;
                                log.Error("err","CommDeviceController.InitialConnect:" + "服务器链接失败！");
                               
                            }
                            else
                            {
                                ((TcpSocketClient)item.obj).StartReceive();
                                tem.status = EumStatus.连接;
                            }
                            datas[i] = tem;

                        }
                    }
                    i++;
                }
                CommDeviceController.g_CommDeviceList = datas;

                return true;
            }
            catch (Exception ex)
            {           
                log.Error("err", "CommDeviceController.InitialConnect:" + ex.ToString());

                return false;
            }
        }
        /// <summary>
        /// 释放连接
        /// </summary>
        public static void DisposeConnect()
        {
         
            //通讯设备由于可能之前已经打开了  此处要先将tcp服务器关闭
            foreach (var item in CommDeviceController.g_CommDeviceList)
            {
                if (item.m_CommDevType.Equals(CommDevType.TcpServer))
                {
                    if (((TcpSocketServer)item.obj).State)
                        ((TcpSocketServer)item.obj).Close();
                }
                else if (item.m_CommDevType.Equals(CommDevType.TcpClient))
                {
                    if (((TcpSocketClient)item.obj).State)
                        ((TcpSocketClient)item.obj).Close();
                }

            }       
        }
        /// <summary>
        /// 释放外部通讯设备
        /// </summary>
        public static void ReleaseDev()
        {
            foreach (var item in CommDeviceController.g_CommDeviceList)
                if (item.m_CommDevType.Equals(CommDevType.TcpServer))
                {
                    ((TcpSocketServer)item.obj).RemoteConnect -= TcpServer_RemoteConnect;
                    ((TcpSocketServer)item.obj).RemoteClose -= TcpServer_RemoteClose;
                    if (((TcpSocketServer)item.obj).State)
                    {
                        ((TcpSocketServer)item.obj).Close();
                    }

                }
                else if (item.m_CommDevType.Equals(CommDevType.TcpClient))
                {
                    ((TcpSocketClient)item.obj).RemoteClose -= TcpClient_RemoteClose;
                    if (((TcpSocketClient)item.obj).State)
                    {
                        ((TcpSocketClient)item.obj).Close();
                    }
                }
        }
        /// <summary>
        /// 深度复制（二进制）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        static public T DeepCopy2<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var t = (T)binaryFormatter.Deserialize(memoryStream);
                return t;
            }
        }
    } 
    
   
    /// <summary>
    /// 通讯设备信息
    /// </summary>
    [Serializable]
    public struct CommDevInfo
    {

        public string m_Name;
        public CommDevType m_CommDevType;
        public int m_Index;
        public object obj;
        public EumStatus status;
    } 
    /// <summary>
      /// 通讯设备类型
      /// </summary>
    [Serializable]
    public enum CommDevType
    {
        TcpServer,
        TcpClient,
        SerialPort,
        UdpServer,
        UdpClient,
        ModbusTcpMaster,
        ModbusTcpSlave,
        ModbusSerialPortMaster,
        ModbusSerialPortSlave,
    }
}
