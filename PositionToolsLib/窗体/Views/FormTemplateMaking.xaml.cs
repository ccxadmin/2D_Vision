using HalconDotNet;
using PositionToolsLib.窗体.Models;
using PositionToolsLib.窗体.ViewModels;
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

namespace PositionToolsLib.窗体.Views
{
    /// <summary>
    /// frmTemplateMaking.xaml 的交互逻辑
    /// </summary>
    public partial class FormTemplateMaking : Window
    {
        public FormTemplateMaking(HObject img, HObject MaskROI,
                                    string _rootPath, EumMakeType
            type = EumMakeType.Model, HObject modelcontour = null)
        {
            InitializeComponent();
            var model = new TemplateMakingViewModel(img, MaskROI, _rootPath, type, modelcontour);
            DataContext = model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");

            var tool = TemplateMakingViewModel.This.ShowTool;
            //tool.SetColorOfTopBottomTitle(System.Drawing.Color.FromArgb(255, 109, 60));
            tool.Dock = System.Windows.Forms.DockStyle.Fill;
            tool.Padding = new System.Windows.Forms.Padding(2);

            tool.SetBackgroundColor(EumControlBackColor.white);
            tool.setDraw(EumDrawModel.fill);
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
        private void mySlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            TemplateMakingViewModel.This.BarPanSizeValueChangedCommand.DoExecute?.Invoke(sender);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TemplateMakingViewModel.This.OnClosingCommand.DoExecute?.Invoke(sender);
        }
    }
}
