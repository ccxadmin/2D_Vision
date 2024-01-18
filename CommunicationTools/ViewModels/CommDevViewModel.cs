using CommunicationTools.Models;
using ControlShareResources.Common;
using FunctionLib.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OSLog;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace CommunicationTools.ViewModels
{
    public class CommDevViewModel
    {
        Log log = new Log("通讯设备");
        /// <summary>
      /// 当前选中的设备对象
      /// </summary>
        CommDevInfo stCurDev = new CommDevInfo();
        public Action<System.Net.IPEndPoint, byte[], int> AppenTxtAction = null;
        public Action ClearTxtAction = null;
        public static CommDevViewModel This { get; set; }
        public CommDevModel Model { get; set; }
        public CommandBase AddCommDevClickCommand { get; set; }
        public CommandBase RemoveCommDevClickCommand { get; set; }
        public CommandBase DevListViewItemMouseDoubleClickCommand { get; set; }
        public CommandBase btnRefreshClickCommand { get; set; }
        public CommandBase btnOPenClickCommand { get; set; }
        public CommandBase btnCloseClickCommand { get; set; }
        public CommandBase Window_LoadedCommand { get; set; }
        public CommandBase Window_ClosingCommand { get; set; }
      
        public CommandBase SendDataClickCommand { get; set; }

        public CommDevViewModel(Action<System.Net.IPEndPoint , byte[] , int > appenTxtAction,
            Action clearTxtAction)
        {
            AppenTxtAction = appenTxtAction;
            ClearTxtAction = clearTxtAction;
            This = this;
            Model = new CommDevModel();
            
            AddCommDevClickCommand = new CommandBase();
            AddCommDevClickCommand.DoExecute = new Action<object>((o) => AddCommDevClick());
            AddCommDevClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            RemoveCommDevClickCommand = new CommandBase();
            RemoveCommDevClickCommand.DoExecute = new Action<object>((o) => RemoveCommDevClick());
            RemoveCommDevClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DevListViewItemMouseDoubleClickCommand = new CommandBase();
            DevListViewItemMouseDoubleClickCommand.DoExecute = new Action<object>((o) => DevListViewItem_MouseDoubleClick(o));
            DevListViewItemMouseDoubleClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            btnRefreshClickCommand = new CommandBase();
            btnRefreshClickCommand.DoExecute = new Action<object>((o) => btnRefresh_Click());
            btnRefreshClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            
            btnOPenClickCommand = new CommandBase();
            btnOPenClickCommand.DoExecute = new Action<object>((o) => btnOPen_Click());
            btnOPenClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            btnCloseClickCommand = new CommandBase();
            btnCloseClickCommand.DoExecute = new Action<object>((o) => btnClose_Click());
            btnCloseClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            Window_LoadedCommand = new CommandBase();
            Window_LoadedCommand.DoExecute = new Action<object>((o) => Window_Loaded(o));
            Window_LoadedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            Window_ClosingCommand = new CommandBase();
            Window_ClosingCommand.DoExecute = new Action<object>((o) => Window_Closing(o));
            Window_ClosingCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });



            SendDataClickCommand = new CommandBase();
            SendDataClickCommand.DoExecute = new Action<object>((o) => SendData_Click());
            SendDataClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

        }

        void Window_Loaded(object o)
        {
            updateListView(CommDeviceController.g_CommDeviceList);
            Model.DevInfoSelectIndex = -1;
            Model.FrameParamsVisib = Visibility.Hidden;
            foreach (var item in CommDeviceController.g_CommDeviceList)
            {
                if (item.m_CommDevType == CommDevType.TcpServer)
                {
                    ((TcpSocketServer)item.obj).RemoteConnect += TcpServer_RemoteConnect;
                    ((TcpSocketServer)item.obj).ReceiveData += TcpServer_ReceiveData;
                    ((TcpSocketServer)item.obj).RemoteClose += TcpServer_RemoteClose;

                }
                else if (item.m_CommDevType == CommDevType.TcpClient)
                {
                    ((TcpSocketClient)item.obj).ReceiveData += Form_TcpClient_ReceiveData;
                    ((TcpSocketClient)item.obj).RemoteClose += TcpClient_RemoteClose;

                }
            }
        }

        void Window_Closing(object o)
        {
            foreach (var item in CommDeviceController.g_CommDeviceList)
            {
                if (item.m_CommDevType == CommDevType.TcpServer)
                {
                    ((TcpSocketServer)item.obj).ReceiveData -= TcpServer_ReceiveData;
                    ((TcpSocketServer)item.obj).RemoteConnect -= TcpServer_RemoteConnect;
                    ((TcpSocketServer)item.obj).RemoteClose -= TcpServer_RemoteClose;
                }
                else if (item.m_CommDevType == CommDevType.TcpClient)
                {
                    ((TcpSocketClient)item.obj).ReceiveData -= Form_TcpClient_ReceiveData;
                    ((TcpSocketClient)item.obj).RemoteClose -= TcpClient_RemoteClose;
                }
            }
 
        }
        /// <summary>
        /// 添加通讯设备
        /// </summary>
        void AddCommDevClick()
        {
            //确定新模块的不重名名称
            bool flag = false;
            int encode = 0;
            do
            {
                flag = true;
                foreach (var comm in CommDeviceController.g_CommDeviceList)
                {
                    if (comm.m_CommDevType == (CommDevType)Model.CommDevSelectIndex
                        && comm.m_Index == encode)
                    {
                        encode++;
                        flag = false;
                        break;
                    }
                }

                if (flag == true)
                {
                    break;
                }
            } while (true);

            object m_CommDevObj = null;
            if (Model.CommDevSelectIndex == (int)CommDevType.TcpServer)
            {
                m_CommDevObj = new TcpSocketServer();
                ((TcpSocketServer)m_CommDevObj).RemoteConnect += TcpServer_RemoteConnect;
                ((TcpSocketServer)m_CommDevObj).RemoteConnect += CommDeviceController.TcpServer_RemoteConnect;
                ((TcpSocketServer)m_CommDevObj).ReceiveData += TcpServer_ReceiveData;
                ((TcpSocketServer)m_CommDevObj).RemoteClose += TcpServer_RemoteClose;
                ((TcpSocketServer)m_CommDevObj).RemoteClose += CommDeviceController.TcpServer_RemoteClose;
                CommDevInfo commDevInfo;
                commDevInfo.m_Name =
                    Enum.GetName(typeof(CommDevType), (CommDevType)Model.CommDevSelectIndex)
                    + encode.ToString(); 

                commDevInfo.m_CommDevType = CommDevType.TcpServer;
                commDevInfo.obj = m_CommDevObj;
                commDevInfo.m_Index = encode;
                commDevInfo.status = EumStatus.未连接;

                CommDeviceController.g_CommDeviceList.Add(commDevInfo);
            }
            else if (Model.CommDevSelectIndex == (int)CommDevType.TcpClient)
            {
                m_CommDevObj = new TcpSocketClient();

                ((TcpSocketClient)m_CommDevObj).ReceiveData += Form_TcpClient_ReceiveData;
                ((TcpSocketClient)m_CommDevObj).RemoteClose += TcpClient_RemoteClose;
                ((TcpSocketClient)m_CommDevObj).RemoteClose += CommDeviceController.TcpClient_RemoteClose;
                CommDevInfo commDevInfo;
                commDevInfo.m_Name =
                      Enum.GetName(typeof(CommDevType), (CommDevType)Model.CommDevSelectIndex)
                    + encode.ToString();
                commDevInfo.m_CommDevType = CommDevType.TcpClient;
                commDevInfo.obj = m_CommDevObj;
                commDevInfo.m_Index = encode;
                commDevInfo.status = EumStatus.未连接;
                CommDeviceController.g_CommDeviceList.Add(commDevInfo);
            }
            updateListView(CommDeviceController.g_CommDeviceList);
        }
        /// <summary>
        /// 刷新设备列表
        /// </summary>
        /// <param name="commDevInfos"></param>
        private void updateListView(List<CommDevInfo> commDevInfos)
        {
            Model.FrameParamsVisib = Visibility.Hidden;
            Model.DevInfoList.Clear();

            if (commDevInfos.Count <= 0)
                return;

            for (int i = 0; i < commDevInfos.Count; i++)
                Model.DevInfoList.Add(new DevInfo(i, commDevInfos[i].m_Name,
                       commDevInfos[i].status.ToString()));
            if (!string.IsNullOrEmpty(stCurDev.m_Name))
            {
                int index = CommDeviceController.g_CommDeviceList.FindIndex(cd =>
                   cd.m_Name == stCurDev.m_Name);
                Model.DevInfoSelectIndex = index;
                Model.FrameParamsVisib = Visibility.Visible;
            }

        }
        /// <summary>
        /// 移除通讯设备
        /// </summary>
        void RemoveCommDevClick()
        {
            int selectIndex = Model.DevInfoSelectIndex;
            int index = CommDeviceController.g_CommDeviceList.FindIndex(com =>
            com.m_Name == CommDeviceController.g_CommDeviceList[selectIndex].m_Name);

            if (index >= 0)
            {
                CommDevInfo commDevInfo = CommDeviceController.g_CommDeviceList[index];
                if (commDevInfo.m_CommDevType == CommDevType.TcpServer)
                {
                    ((TcpSocketServer)commDevInfo.obj).RemoteConnect -= TcpServer_RemoteConnect;
                    ((TcpSocketServer)commDevInfo.obj).RemoteConnect -= CommDeviceController.TcpServer_RemoteConnect;
                    ((TcpSocketServer)commDevInfo.obj).ReceiveData -= TcpServer_ReceiveData;
                    ((TcpSocketServer)commDevInfo.obj).RemoteClose -= TcpServer_RemoteClose;
                    ((TcpSocketServer)commDevInfo.obj).RemoteClose -= CommDeviceController.TcpServer_RemoteClose;
                }
                if (commDevInfo.m_CommDevType == CommDevType.TcpClient)
                {
                    ((TcpSocketClient)commDevInfo.obj).ReceiveData -= Form_TcpClient_ReceiveData;
                    ((TcpSocketClient)commDevInfo.obj).RemoteClose -= TcpClient_RemoteClose;
                    ((TcpSocketClient)commDevInfo.obj).RemoteClose -= CommDeviceController.TcpClient_RemoteClose;
                }
                CommDeviceController.g_CommDeviceList.Remove(CommDeviceController.g_CommDeviceList[index]);
            }
            updateListView(CommDeviceController.g_CommDeviceList);
        }
        //双击
        void DevListViewItem_MouseDoubleClick(object o)
        {
            int selectIndex = Model.DevInfoSelectIndex;
            string sCurSelDevName = Model.DevInfoList[selectIndex].DevDescrip;

            int index = CommDeviceController.g_CommDeviceList.FindIndex(cd =>
                cd.m_Name == sCurSelDevName);
            if (index == -1)
                return;
            Model.FrameParamsVisib = Visibility.Visible ;
            Model.SendText = "";
             ClearTxtAction?.Invoke();


            stCurDev = CommDeviceController.g_CommDeviceList[index];

            if (sCurSelDevName.Contains("Tcp"))
            {
             
                if (sCurSelDevName.Contains("Server"))
                {
                    Model.BtnContent= "监听";
                    Model.CbxCilentsVisib = Visibility.Visible;
                    Model.LblClientsVisib = Visibility.Visible;
                    Model.BtnRefreshVisib =Visibility.Visible ;
                    LoadConnect();
                }
                else
                {
                    Model.BtnContent = "打开";
                    Model.CbxCilentsVisib = Visibility.Hidden;
                    Model.LblClientsVisib = Visibility.Hidden;
                    Model.BtnRefreshVisib = Visibility.Hidden;
                }

                if (stCurDev.m_CommDevType
                    == CommDevType.TcpServer)
                {
                    Model.IPtext = ((TcpSocketServer)stCurDev.obj).IP;
                    Model.PortValue = ((TcpSocketServer)stCurDev.obj).Port;           
                    Model.IsOpen = ((TcpSocketServer)stCurDev.obj).State;
                }
                else if (stCurDev.m_CommDevType
                    == CommDevType.TcpClient)
                {
                    Model.IPtext = ((TcpSocketClient)stCurDev.obj).IP;
                    Model.PortValue = ((TcpSocketClient)stCurDev.obj).Port;         
                    Model.IsOpen = ((TcpSocketClient)stCurDev.obj).State;
                }
            }
          
        }
        /// <summary>
        /// 刷新
        /// </summary>
        void btnRefresh_Click()
        {
            Model.ClientsList.Clear();
            refreshClient();

        }
        /// <summary>
        /// 刷新客户端连接状态
        /// </summary>
        void refreshClient()
        {
            foreach (var item in ((TcpSocketServer)stCurDev.obj).Sessions.Keys)
                Model.ClientsList.Add(item);
            if (Model.ClientsList.Count > 0)
                Model.ClientsSelectIndex = 0;
            else
                Model.ClientsSelectIndex=-1;
        }
        /// <summary>
        /// 打开
        /// </summary>
        void btnOPen_Click()
        {
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (stCurDev.obj == null)
                return;

            if (stCurDev.m_CommDevType == CommDevType.TcpServer)
            {
                //参数设置
                Model.IsOpen = false;
                if (!isIp(Model.IPtext))
                    return;
                ((TcpSocketServer)stCurDev.obj).IP = Model.IPtext;
                ((TcpSocketServer)stCurDev.obj).Port = Model.PortValue;
                if (!((TcpSocketServer)stCurDev.obj).Start(Model.IPtext, Model.PortValue))
                {
                    log.Info(funName,string.Format("[{0}:{1}]启动监听失败", Model.IPtext, Model.PortValue));
                    stCurDev.status = EumStatus.未连接;
                }
                else
                {
                    log.Info(funName,string.Format("服务器[{0}:{1}]已启动监听", Model.IPtext, Model.PortValue));
                    stCurDev.status = EumStatus.连接;
                }
         
                Model.IsOpen = ((TcpSocketServer)stCurDev.obj).State;
            }
            else if (stCurDev.m_CommDevType == CommDevType.TcpClient)
            {

                //参数设置
                Model.IsOpen = false;
      
                if (!isIp(Model.IPtext))
                    return;
                ((TcpSocketClient)stCurDev.obj).IP = Model.IPtext;
                ((TcpSocketClient)stCurDev.obj).Port = Model.PortValue;
                if (!((TcpSocketClient)stCurDev.obj).Connect(Model.IPtext, Model.PortValue))
                {
                    log.Info(funName,string.Format("[{0}:{1}]连接服务器失败", Model.IPtext, Model.PortValue));
                    stCurDev.status = EumStatus.未连接;
                }
                else
                {
                    ((TcpSocketClient)stCurDev.obj).StartReceive();
                    stCurDev.status = EumStatus.连接;
                    log.Info(funName,string.Format("连接到服务器[{0}:{1}]", Model.IPtext, Model.PortValue));
                }
     
                Model.IsOpen = ((TcpSocketClient)stCurDev.obj).State;
            }
         

            int index = CommDeviceController.g_CommDeviceList.FindIndex(cd =>
                  cd.m_Name == stCurDev.m_Name);
            if (index == -1)
                return;
            CommDeviceController.g_CommDeviceList[index] = stCurDev;
            updateListView(CommDeviceController.g_CommDeviceList);
        }
        /// <summary>
        /// 关闭
        /// </summary>
        void btnClose_Click()
        {
            if (stCurDev.obj == null)
                return;

            if (stCurDev.m_CommDevType == CommDevType.TcpServer)
            {
                ((TcpSocketServer)stCurDev.obj).Close();    
                Model.IsOpen=((TcpSocketServer)stCurDev.obj).State;
            }
            else if (stCurDev.m_CommDevType == CommDevType.TcpClient)
            {
                ((TcpSocketClient)stCurDev.obj).Close();
                Model.IsOpen = ((TcpSocketClient)stCurDev.obj).State;
     
            }
            stCurDev.status = EumStatus.连接;
            int index = CommDeviceController.g_CommDeviceList.FindIndex(cd =>
                 cd.m_Name == stCurDev.m_Name);
            if (index == -1)
                return;
            CommDeviceController.g_CommDeviceList[index] = stCurDev;
            updateListView(CommDeviceController.g_CommDeviceList);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        void SendData_Click()
        {
            byte[] bytes = new byte[0];
            bytes = System.Text.Encoding.Default.GetBytes(Model.SendText);
            if (stCurDev.m_CommDevType == CommDevType.TcpServer)
            { 
                if(Model.ClientsSelectIndex>=0)
                {
                    string key = Model.ClientsList[Model.ClientsSelectIndex];
                    ((TcpSocketServer)stCurDev.obj).SendTo(key,bytes);
                }
               else
                    ((TcpSocketServer)stCurDev.obj).SendTo( bytes);
            }
            else if (stCurDev.m_CommDevType == CommDevType.TcpClient)
            {
                ((TcpSocketClient)stCurDev.obj).Send(bytes);
            }
        }
        /// <summary>
        /// 验证IP地址是否合法
        /// </summary>
        /// <param name="ip">要验证的IP地址</param>
        /// <returns></returns>
        private bool isIp(string ip)
        {
            //如果为空，认为验证不合格
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            //清除要验证字符传中的空格
            ip = ip.Trim();

            //模式字符串，正则表达式
            string patten = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

            //验证
            return Regex.IsMatch(ip, patten);
        }

        #region tcp server
        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void TcpServer_ReceiveData(IPEndPoint remote, byte[] buffer, int count)
        {
            AppenTxtAction?.Invoke(remote, buffer, count);
        }

        /// <summary>
        /// 远程连接
        /// </summary>
        private void TcpServer_RemoteConnect(string key)
        {
            if ((TcpSocketServer)stCurDev.obj == null)
                return;
            //Log.Info(string.Format("客户端[{0}]上线",key));
           LoadConnect();
        }
        private void LoadConnect()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {

                Model.ClientsList.Clear();

                foreach (var item in ((TcpSocketServer)stCurDev.obj).Sessions.Keys)
                {
                    Model.ClientsList.Add(item);
                }
                if (Model.ClientsList.Count > 0)
                    Model.ClientsSelectIndex = 0;
                else
                    Model.ClientsSelectIndex = -1;
            }));
        }

        /// <summary>
        /// 远程关闭
        /// </summary>
        /// <param name="key"></param>
        private void TcpServer_RemoteClose(string key)
        {
            if ((TcpSocketServer)stCurDev.obj == null)
                return;
            //Log.Info(string.Format("客户端下线"));
            
            LoadConnect();
        }
        #endregion

        #region tcp client
        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void Form_TcpClient_ReceiveData(IPEndPoint remote, byte[] buffer, int count)
        {
            AppenTxtAction?.Invoke(remote, buffer, count);
        }
        /// <summary>
        /// 远程关闭
        /// </summary>
        /// <param name="key"></param>
        private void TcpClient_RemoteClose(string key)
        {
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            log.Info(funName,string.Format("服务端[{0}]断开连接", key));    
            btnClose_Click();

            stCurDev.status = EumStatus.未连接;
            int index = CommDeviceController.g_CommDeviceList.FindIndex(cd =>
                 cd.m_Name == stCurDev.m_Name);
            if (index == -1)
                return;
            CommDeviceController.g_CommDeviceList[index] = stCurDev;
            updateListView(CommDeviceController.g_CommDeviceList);

        }
        #endregion
    }
}
