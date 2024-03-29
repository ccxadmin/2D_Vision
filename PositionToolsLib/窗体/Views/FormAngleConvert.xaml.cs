﻿using HalconDotNet;
using PositionToolsLib.工具;
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

namespace PositionToolsLib.窗体.Views
{
    /// <summary>
    /// FormAngleConvert.xaml 的交互逻辑
    /// </summary>
    public partial class FormAngleConvert : Window
    {
        public FormAngleConvert(BaseTool tool)
        {
            InitializeComponent();
            var model = new AngleConvertViewModel(tool);
            DataContext = model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 装载winform控件，主动释放方式内存泄漏
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            //host.Child.Dispose();
            //host.Child = null;
            //base.OnClosed(e);
        }
    }
}
