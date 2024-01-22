using GlueDetectionLib.工具;
using GlueDetectionLib.窗体.ViewModels;
using HalconDotNet;
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
using System.Windows.Shapes;
using VisionShowLib.UserControls;

namespace GlueDetectionLib.窗体.Views
{
    /// <summary>
    /// FormGlueOffset.xaml 的交互逻辑
    /// </summary>
    public partial class FormGlueOffset : Window
    {
        public FormGlueOffset(BaseTool tool)
        {
            InitializeComponent();
            var model = new GlueOffsetViewModel(tool);
            DataContext = model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");

            var tool = GlueOffsetViewModel.This.ShowTool;
            //tool.SetColorOfTopBottomTitle(System.Drawing.Color.FromArgb(255, 109, 60));
            tool.Dock = System.Windows.Forms.DockStyle.Fill;
            tool.Padding = new System.Windows.Forms.Padding(2);
            tool.SetEnable(false);
            tool.SetBackgroundColor(EumControlBackColor.white);
            tool.setDraw(EumDrawModel.margin);
            host.Child = tool;
        }
        /// <summary>
        /// 装载winform控件，主动释放方式内存泄漏
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            host.Child.Dispose();
            host.Child = null;
            base.OnClosed(e);
        }
    }
}
