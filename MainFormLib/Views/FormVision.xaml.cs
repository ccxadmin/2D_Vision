﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using HalconDotNet;
using MainFormLib.ViewModels;
using VisionShowLib.UserControls;

namespace MainFormLib.Views
{
    /// <summary>
    /// frmCalibration.xaml 的交互逻辑
    /// </summary>
    public partial class FormVision : Window
    {
        static FormVision  formVision = null;
        VisionViewModel model = null;
        private FormVision(string camStationName = "camStationName_1")
        {
            InitializeComponent();
            model = new VisionViewModel(camStationName);
            DataContext = model;
            
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="camStationName"></param>
        /// <returns></returns>
        public static FormVision CreateInstance(string camStationName = "camStationName_1")            
        {
            return new FormVision(camStationName);
        }

        /// <summary>
        /// 创建单例
        /// </summary>
        /// <param name="camStationName"></param>
        /// <returns></returns>
        public static FormVision CreateSingleton(string camStationName = "camStationName_1")
        {
            if (formVision == null)
                formVision = new FormVision(camStationName);
            return formVision;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");

            var tool = model.ShowTool;
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
