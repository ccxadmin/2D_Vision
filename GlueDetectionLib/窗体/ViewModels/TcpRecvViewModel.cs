using CommunicationTools;
using CommunicationTools.Models;
using ControlShareResources.Common;
using FunctionLib.TCP;
using GlueDetectionLib.参数;
using GlueDetectionLib.工具;
using GlueDetectionLib.窗体.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GlueDetectionLib.窗体.ViewModels
{
    public class TcpRecvViewModel : BaseViewModel
    {
        public Action Window_LoadedAction { get; set; }
        public Action Window_ClosingAction { get; set; }
        public static TcpRecvViewModel This { get; set; }
        public TcpRecvToolModel Model { get; set; }
      
        string CommDevName = "";
        public CommandBase SaveButClickCommand { get; set; }
        public  TcpRecvViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new TcpRecvToolModel();
            Model.TitleName = baseTool.GetToolName();//工具名称
            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            Window_LoadedAction = Window_Loaded;
            Window_ClosingAction = Window_Closing;

        }

        void Window_Loaded()
        {
            foreach (var item in CommDeviceController.g_CommDeviceList)
                Model.DevList.Add(item.m_Name);        
          
            BaseParam par = baseTool.GetParam();
            ShowData((ParamsOfTcpRecv)par);
        }
        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData(ParamsOfTcpRecv parDat)
        {
            string head = "", spilt = "", tail = "";
            ((TcpRecvTool)baseTool).getProtoolFormat(ref head, ref spilt, ref tail);
            Model.TxbHead = head;
            Model.TxbSpilt = spilt;
            Model.TxbTail = tail;
            if (tail == "\r")
                Model.TxbTail = @"\r";
            else if (tail == "\r\n")
                Model.TxbTail = @"\r\n";
            else if (tail == "\n")
                Model.TxbTail = @"\n";

            CommDevName = Model.DevSelectName = ((TcpRecvTool)baseTool).CommDevName;
            Model.DevSelectIndex = Model.DevList.IndexOf(Model.DevSelectName);

            if (Model.DevSelectIndex == -1) return;
           
            //如果存在先注销事件
            if (CommDeviceController.g_CommDeviceList.Exists(t => t.m_Name.Equals(Model.DevSelectName)))
            {
                int index = CommDeviceController.g_CommDeviceList.
                               FindIndex(t => t.m_Name.Equals(Model.DevSelectName));
                //事件订阅
                if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                    ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData += TcpServer_ReceiveData;
                else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                    ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData += TcpClient_ReceiveData;
            }

        }
        void Window_Closing()
        {
           
            int index = CommDeviceController.g_CommDeviceList.FindIndex(it => it.m_Name == Model.DevSelectName);
            if (index == -1)
                return;
            if (CommDeviceController.g_CommDeviceList[index].obj != null)
            {

                if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpServer)
                {
                    ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj)
                        .ReceiveData -= TcpServer_ReceiveData;


                }
                else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                {
                    ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj)
                        .ReceiveData -= TcpClient_ReceiveData;

                }

            }
        }

        /// <summary>
        /// 服务端数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void TcpServer_ReceiveData(IPEndPoint remote, byte[] buffer, int length)
        {

            if (buffer == null || length <= 0 || buffer.Length < length)
                return;
           
            string datarv = System.Text.Encoding.Default.GetString(buffer);

            Application.Current.Dispatcher.Invoke(() =>
            {
               Model.TxbRecieveData = datarv;
            });
           
        }

        /// <summary>
        /// 客户端数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void TcpClient_ReceiveData(IPEndPoint remote, byte[] buffer, int length)
        {
            if (buffer == null || length <= 0 || buffer.Length < length)
                return;
           
            string datarv = System.Text.Encoding.Default.GetString(buffer, 0, length);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Model.TxbRecieveData = datarv;
            });
        }

        /// <summary>
        /// 数据格式整合
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        private string protoolBuild(List<string> strings)
        {
            string data = "";
            data += Model.TxbHead;

            for (int i = 0; i < strings.Count; i++)
            {
                data += strings[i];
                if (i == strings.Count - 1)
                    break;
                data += Model.TxbTail;
            }

            data += Model.TxbTail;

            return data;
        }

        /// <summary>
        /// 参数保存
        /// </summary>
        public void btnSaveParam_Click()
        {
            string escapeCharacter = Model.TxbTail;
            if (Model.TxbTail == @"\n")
                escapeCharacter = "\n";
            else if (Model.TxbTail == @"\r")
                escapeCharacter = "\r";
            else if (Model.TxbTail == @"\r\n")
                escapeCharacter = "\r\n";
            ((TcpRecvTool)baseTool).setProtoolFormat
                (Model.TxbHead, Model.TxbSpilt, escapeCharacter);
          
            if (CommDevName == Model.DevSelectName) return;
            int index = CommDeviceController.g_CommDeviceList.FindIndex(it => it.m_Name 
                      ==Model.DevSelectName);
            if (index == -1)
                return ;

          
            //如果存在先注销事件
            if (CommDeviceController.g_CommDeviceList.Exists(t => t.m_Name.Equals(CommDevName)))
            {
                int n_index = CommDeviceController.g_CommDeviceList.
                               FindIndex(t => t.m_Name.Equals(CommDevName));
                //先清除旧的事件订阅
                if (CommDeviceController.g_CommDeviceList[n_index].m_CommDevType == (CommDevType.TcpServer))
                    ((TcpSocketServer)CommDeviceController.g_CommDeviceList[n_index].obj).ReceiveData -= TcpServer_ReceiveData;
                else if (CommDeviceController.g_CommDeviceList[n_index].m_CommDevType == CommDevType.TcpClient)
                    ((TcpSocketClient)CommDeviceController.g_CommDeviceList[n_index].obj).ReceiveData -= TcpClient_ReceiveData;
            }
            BaseParam par = baseTool.GetParam();
            CommDevName = Model.DevSelectName;
            ((TcpRecvTool)baseTool).CommDevName = CommDevName;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
            //重新订阅事件          
            int e_index = CommDeviceController.g_CommDeviceList.
                               FindIndex(t => t.m_Name.Equals(CommDevName));
            if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == (CommDevType.TcpServer))
                ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).ReceiveData += TcpServer_ReceiveData;
            else if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == CommDevType.TcpClient)
                ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).ReceiveData += TcpClient_ReceiveData;
            return;
        }
    }
}
