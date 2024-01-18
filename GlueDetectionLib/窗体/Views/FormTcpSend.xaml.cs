using GlueDetectionLib.工具;
using GlueDetectionLib.窗体.ViewModels;
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

namespace GlueDetectionLib.窗体.Views
{
    /// <summary>
    /// TcpSendToolWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FormTcpSend : Window
    {
        public FormTcpSend(BaseTool tool)
        {
            InitializeComponent();
            var model = new TcpSendViewModel(tool);
            DataContext = model;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TcpSendViewModel.This.Window_LoadedAction?.Invoke();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TcpSendViewModel.This.Window_ClosingAction?.Invoke();
        }
    }
}
