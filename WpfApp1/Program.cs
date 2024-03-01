using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
   

    public partial class Program : Application
    {
        public Program()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

        }

        private static System.Threading.Mutex m;

        [System.STAThreadAttribute()]
        public static void Main()
        {
            bool run;

            m = new System.Threading.Mutex(true, "OnlyRunOneInstance", out run);
            if (run)
            {
                Application app = new Application();    // 定义Application对象作为整个应用程序入口  
                MainWindow win = new MainWindow();  // 窗口实例化
                app.Run(win);   // 调用Run方法

                //Application currApp = Application.Current;
                //currApp.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);

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
