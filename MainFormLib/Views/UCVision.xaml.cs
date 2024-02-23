using HalconDotNet;
using CommunicationTools.ViewModels;
using MainFormLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionShowLib.UserControls;

namespace MainFormLib.Views
{
    /// <summary>
    /// UCVision.xaml 的交互逻辑
    /// </summary>
    public partial class UCVision : UserControl
    {
        static UCVision uCVision = null;
        public VisionViewModel viewModel { get; set; } = null;
        private UCVision(string camStationName = "camStationName_1")
        {
            InitializeComponent();          
            viewModel = new VisionViewModel(
                AppenTxt,
                ClearTxt,
                camStationName);
            DataContext = viewModel;

            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");
            HOperatorSet.SetLineWidth(viewModel.ShowTool.HWindowsHandle, 2);
            var tool = viewModel.ShowTool;
            tool.Dock = System.Windows.Forms.DockStyle.Fill;
            tool.Padding = new System.Windows.Forms.Padding(2);

            tool.SetBackgroundColor(EumControlBackColor.white);
            tool.setDraw(EumDrawModel.margin);
            host.Child = tool;
        }
        ~UCVision()
        {
            host.Child.Dispose();
            host.Child = null;
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="camStationName"></param>
        /// <returns></returns>
        public static UCVision CreateInstance(string camStationName = "camStationName_1")
        {
            return new UCVision(camStationName);
        }

        /// <summary>
        /// 创建单例
        /// </summary>
        /// <param name="camStationName"></param>
        /// <returns></returns>
        public static UCVision CreateSingleton(string camStationName = "camStationName_1")
        {
            if (uCVision == null)
                uCVision = new UCVision(camStationName);
            return uCVision;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                    
            //HOperatorSet.SetSystem("temporary_mem_cache", "false");
            //HOperatorSet.SetSystem("clip_region", "false");
            //HOperatorSet.SetLineWidth(viewModel.ShowTool.HWindowsHandle, 2);
            //var tool = viewModel.ShowTool;  
            //tool.Dock = System.Windows.Forms.DockStyle.Fill;
            //tool.Padding = new System.Windows.Forms.Padding(2);

            //tool.SetBackgroundColor(EumControlBackColor.white);
            //tool.setDraw(EumDrawModel.margin);
            //host.Child = tool;

            viewModel.WindowsLoadedCommand.DoExecute?.Invoke(sender);
        }
        
        private void PosListViewItem__MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.ToolsOfPosDoubleClickCommand.DoExecute?.Invoke(sender);
        }

        private void PosListViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {

            viewModel.ToolsOfPosMouseUpCommand.DoExecute?.Invoke(sender);
        }
      

      
        private void PosMenuItem_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ToolsOfPosition_ContextMenuCommand.DoExecute?.Invoke(sender);
        }
    

        void AppenTxt(string objName,string info)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if(objName== "richTxtInfo")
                {
                    TextPointer tpstart = richTxtInfo.Document.ContentStart;
                    int length = Math.Abs(richTxtInfo.Document.ContentEnd.GetOffsetToPosition(tpstart));
                    if (length > 5000)
                        richTxtInfo.Document.Blocks.Clear();
                    richTxtInfo.AppendText(info);
                    richTxtInfo.AppendText("\r");
                    richTxtInfo.ScrollToEnd();
                }
               else if(objName == "scanRichTxtInfo")
                {
                    TextPointer tpstart = scanRichTxtInfo.Document.ContentStart;
                    int length = Math.Abs(scanRichTxtInfo.Document.ContentEnd.GetOffsetToPosition(tpstart));
                    if (length > 5000)
                        scanRichTxtInfo.Document.Blocks.Clear();
                    scanRichTxtInfo.AppendText(info);
                    scanRichTxtInfo.AppendText("\r");
                    scanRichTxtInfo.ScrollToEnd();
                }
            }));
        }

        void ClearTxt(string objName)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (objName == "richTxtInfo")
                    richTxtInfo.Document.Blocks.Clear();
                else if (objName == "scanRichTxtInfo")
                    scanRichTxtInfo.Document.Blocks.Clear();
            }));
        }

        private void expouseSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            viewModel.ExpouseValueChangedCommand.DoExecute?.Invoke(sender);
        }
        private void gainSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            viewModel.GainValueChangedCommand.DoExecute?.Invoke(sender);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
