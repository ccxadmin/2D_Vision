using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionShowLib;
using HalconDotNet;
using VisionShowLib.UserControls;
using ROIGenerateLib;
using System.Collections.ObjectModel;
using CommunicationTools.Views;
using CommunicationTools.Models;
using FilesRAW.Common;

using PositionToolsLib.窗体.Views;
using MainFormLib.Views;

using System.Windows.Interop;
using MainFormLib.ViewModels;
using MainFormLib.Models;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainFormLib.Views.UCVision f =null;
        MainFormLib.Views.UCVision f2 =null;
        public MainWindow()
        {
            InitializeComponent();
            f = MainFormLib.Views.UCVision.CreateInstance("camStationName_1");
            f.Margin = new Thickness(1);
            f.Width = this.panel0.Width;
            f.Height = this.panel0.Height;
            f.HorizontalAlignment = HorizontalAlignment.Stretch;
            f.VerticalAlignment = VerticalAlignment.Stretch;     
            panel0.Children.Add(f);

       
        }
        /// <summary>
        /// 装载winform控件，主动释放方式内存泄漏
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
          
            base.OnClosed(e);
        }

      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string txt = ((Button)sender).Content.ToString();
            switch(txt)
            {
                case "打开相机":
                    f.viewModel.OpenCam();
                    break;
                case "关闭相机":
                    f.viewModel.CloseCam();
                    break;
                case "单帧采集":
                    f.viewModel.OneShot();
                    break;
                case "连续采集":
                    f.viewModel.ContinueGrab();
                    break;
                case "停止采集":
                    f.viewModel.StopGrab();
                    break;
                case "切换标定":
                    f.viewModel.SwithCalib("123");
                    break;
                case "加载GIF":
                    FormGifShow f_GifShow=new FormGifShow();
                    f_GifShow.Topmost = true;
                    f_GifShow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    f_GifShow.Show();
                    break;
                case "折叠参数":
                    f.viewModel.SetParamMode( EumParamOperate.fold);
                    break;
                case "展开参数":
                    f.viewModel.SetParamMode( EumParamOperate.expand);
                    break;
            }
          
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            f.viewModel.Release();
        }
    }
}
   
