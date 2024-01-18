using CommunicationTools.ViewModels;
using HalconDotNet;
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
    /// TcpRecvToolWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FormTcpRecv : Window
    {
        public FormTcpRecv(BaseTool tool)
        {
            InitializeComponent();
            var model = new TcpRecvViewModel(tool);
            DataContext = model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TcpRecvViewModel.This.Window_LoadedAction?.Invoke();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TcpRecvViewModel.This.Window_ClosingAction?.Invoke();
        }
    }
}
