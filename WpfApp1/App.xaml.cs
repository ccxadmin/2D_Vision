using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

        }

        private static System.Threading.Mutex m;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool run;

            m = new System.Threading.Mutex(true, "OnlyRunOneInstance", out run);
            if (run)
            {

                Application currApp = Application.Current;
                currApp.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);

            }
            else
            {
                MessageBox.Show("程序已启动!", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //MessageBox.Show("我们很抱歉，当前应用程序遇到一些问题，该操作已经终止，请进行重试，如果问题继续存在，请联系管理员.", "意外的操作", MessageBoxButton.OK, MessageBoxImage.Information);//这里通常需要给用户一些较为友好的提示，并且后续可能的操作
            MessageBox.Show(e.Exception.Message + "\r\n" + e.Exception.StackTrace, "系统信息");
            e.Handled = true;//使用这一行代码告诉运行时，该异常被处理了，不再作为UnhandledException抛出了。
        }

    }
}
