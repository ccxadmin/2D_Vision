using HalconDotNet;
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
        VisionViewModel model = null;
        public UCVision(string camStationName = "camStationName_1")
        {
            InitializeComponent();
             model = new VisionViewModel(camStationName);
            DataContext = model;

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
                    
            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");
            HOperatorSet.SetLineWidth(model.ShowTool.HWindowsHandle, 2);
            var tool = model.ShowTool;  
            tool.Dock = System.Windows.Forms.DockStyle.Fill;
            tool.Padding = new System.Windows.Forms.Padding(2);

            tool.SetBackgroundColor(EumControlBackColor.white);
            tool.setDraw(EumDrawModel.margin);
            host.Child = tool;

            model.WindowsLoadedCommand.DoExecute?.Invoke(sender);
        }
        
        private void ListViewItem__MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            model.ToolsOfPosDoubleClickCommand.DoExecute?.Invoke(sender);
        }

        private void ListViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {

            model.ToolsOfPosMouseUpCommand.DoExecute?.Invoke(sender);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            model.ToolsOfPosition_ContextMenuCommand.DoExecute?.Invoke(sender);
        }     
    }
}
