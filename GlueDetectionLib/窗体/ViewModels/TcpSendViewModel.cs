using CommunicationTools;
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
    public class TcpSendViewModel : BaseViewModel
    {
        public Action Window_LoadedAction { get; set; }
        public Action Window_ClosingAction { get; set; }
        public static TcpSendViewModel This { get; set; }
        public TcpSendToolModel Model { get; set; }
        string CommDevName = "";
        public CommandBase SaveButClickCommand { get; set; }
   
        public TcpSendViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new TcpSendToolModel();
            Model.TitleName = baseTool.GetToolName();//工具名称
            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            Window_LoadedAction = Window_Loaded;
            Window_ClosingAction = Window_Closing;


        }
        /// <summary>
        /// 窗体加载
        /// </summary>
        void Window_Loaded()
        {
            foreach (var item in CommDeviceController.g_CommDeviceList)
                Model.DevList.Add(item.m_Name);

            BaseParam par = baseTool.GetParam();
            ShowData((ParamsOfTcpSend)par);
        }
        /// <summary>
        /// 窗体关闭
        /// </summary>
        void Window_Closing()
        {

        }


        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData(ParamsOfTcpSend parDat)
        {
            string head = "", spilt = "", tail = "";
            ((TcpSendTool)baseTool).getProtoolFormat(ref head, ref spilt, ref tail);
            Model.TxbHead = head;
            Model.TxbSpilt = spilt;
            Model.TxbTail = tail;
            if (tail == "\r")
                Model.TxbTail = @"\r";
            else if (tail == "\r\n")
                Model.TxbTail = @"\r\n";
            else if (tail == "\n")
                Model.TxbTail = @"\n";

            CommDevName = Model.DevSelectName = ((TcpSendTool)baseTool).CommDevName;
            Model.DevSelectIndex = Model.DevList.IndexOf(Model.DevSelectName);

            if (Model.DevSelectIndex == -1) return;
         
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
            ((TcpSendTool)baseTool).setProtoolFormat
                (Model.TxbHead, Model.TxbSpilt, escapeCharacter);

            if (CommDevName == Model.DevSelectName) return;
          
            BaseParam par = baseTool.GetParam();
            CommDevName = Model.DevSelectName;
            ((TcpSendTool)baseTool).CommDevName = CommDevName;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
           
           
        }
    }
}
