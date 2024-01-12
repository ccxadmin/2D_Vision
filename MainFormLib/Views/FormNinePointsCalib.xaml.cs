using HalconDotNet;
using MainFormLib.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MainFormLib.Views
{
    /// <summary>
    /// FormNinePointsCalib.xaml 的交互逻辑
    /// </summary>
    public partial class FormNinePointsCalib : Window
    {
        public bool isClosedFlag = false;

        public FormNinePointsCalib(string path, string _calibName = "default")
        {
            InitializeComponent();
            var model = new NinePointsCalibViewModel(path, _calibName);
            DataContext = model;
            isClosedFlag = false;
        }
     
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");

            var tool = NinePointsCalibViewModel.This.ShowTool;
            //tool.SetColorOfTopBottomTitle(System.Drawing.Color.FromArgb(255, 109, 60));
            tool.Dock = System.Windows.Forms.DockStyle.Fill;
            tool.Padding = new System.Windows.Forms.Padding(2);

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
            isClosedFlag = true;
            
        }
    }
}
