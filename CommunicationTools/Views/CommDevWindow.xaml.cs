using CommunicationTools.ViewModels;
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

namespace CommunicationTools.Views
{
    /// <summary>
    /// CommDevWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CommDevWindow : Window
    {
        static CommDevWindow win = null;
        private CommDevWindow()
        {
            InitializeComponent();
            var model = new CommDevViewModel(AppenTxt, ClearTxt);
            DataContext = model;
        }

       static  public CommDevWindow CreateInstance()
        {
            if (win == null)
                win = new CommDevWindow();
            return win;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CommDevViewModel.This.Window_LoadedCommand.DoExecute?.Invoke(sender);
        }

        private void DevListViewItem__MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CommDevViewModel.This.DevListViewItemMouseDoubleClickCommand?.DoExecute(sender);
        }

        void AppenTxt(System.Net.IPEndPoint remote, byte[] buffer, int length)
        {
            if (buffer == null || length <= 0 || buffer.Length < length)
                return;

            this.Dispatcher.Invoke(new Action(() =>
            {
                string data = string.Empty;
                data = System.Text.Encoding.Default.GetString(buffer, 0, length);

                this.recieveTxb.AppendText("[" + remote.ToString() + "]");
                this.recieveTxb.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "]");
                this.recieveTxb.AppendText(data);
                this.recieveTxb.AppendText("\r\n");
                this.recieveTxb.ScrollToEnd();
            }));
          
        }

        void ClearTxt()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.recieveTxb.Clear();
            }));
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            win = null;
            CommDevViewModel.This.Window_ClosingCommand.DoExecute?.Invoke(sender);
            
        }
    }
}
