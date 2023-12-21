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
    /// FormMatch.xaml 的交互逻辑
    /// </summary>
    public partial class FormMatch : Window
    {
        public FormMatch(BaseTool tool)
        {
           
            InitializeComponent();
            var model = new MatchViewModel(tool);
            DataContext = model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");

            var tool = MatchViewModel.This.ShowTool;
            //tool.SetColorOfTopBottomTitle(System.Drawing.Color.FromArgb(255, 109, 60));
            tool.Dock = System.Windows.Forms.DockStyle.Fill;
            tool.Padding = new System.Windows.Forms.Padding(2);

            tool.SetBackgroundColor(EumControlBackColor.white);
            tool.setDraw(EumDrawModel.margin);
            host.Child = tool;
            //RoiEditer.SetRoiController(tool.RoiController);
            //tool.ViewController.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);
        }
    }
}
