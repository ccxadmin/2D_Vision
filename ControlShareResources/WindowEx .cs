using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace ControlShareResources
{
    /// <summary>
    /// 窗体封装
    /// </summary>
    public class WindowEx : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _WindowBtnCommand;
        /// <summary>
        /// 窗体按钮命令
        /// </summary>
        public ICommand WindowBtnCommand
        {
            get
            {
                return _WindowBtnCommand;
            }
            set
            {
                _WindowBtnCommand = value;
                OnPropertyChanged("WindowBtnCommand");
            }
        }

     

        private Thickness _BorderMargin = new Thickness(0, 0, 0, 0);
        public Thickness BorderMargin
        {
            get
            {
                return _BorderMargin;
            }
            set
            {
                _BorderMargin = value;
                OnPropertyChanged("BorderMargin");
            }
        }

        private HorizontalAlignment _BtnPanelHorizontalAlignment = HorizontalAlignment.Right;
        /// <summary>
        /// 窗体按钮的Panel位置
        /// </summary>
        public HorizontalAlignment BtnPanelHorizontalAlignment
        {
            get
            {
                return _BtnPanelHorizontalAlignment;
            }
            set
            {
                _BtnPanelHorizontalAlignment = value;
                OnPropertyChanged("BtnPanelHorizontalAlignment");
            }
        }

        private Visibility _BtnMinimizeVisibility = Visibility.Visible;
        /// <summary>
        /// 窗体最小化按钮的显示状态
        /// </summary>
        public Visibility BtnMinimizeVisibility
        {
            get
            {
                return _BtnMinimizeVisibility;
            }
            set
            {
                _BtnMinimizeVisibility = value;
                OnPropertyChanged("BtnMinimizeVisibility");
            }
        }

        private Visibility _BtnMaximizeVisibility = Visibility.Visible;
        /// <summary>
        /// 窗体最大化按钮的显示状态
        /// </summary>
        public Visibility BtnMaximizeVisibility
        {
            get
            {
                return _BtnMaximizeVisibility;
            }
            set
            {
                _BtnMaximizeVisibility = value;
                OnPropertyChanged("BtnMaximizeVisibility");
            }
        }

        /// <summary>
        /// 窗体 构造函数
        /// </summary>
        public WindowEx()
        {
            this.DataContext = this;
            this.ShowInTaskbar = true;

            #region 窗体样式设置
            //Uri uri = new Uri("/SunCreate.Common.Controls;Component/WindowEx/WindowExResource.xaml", UriKind.Relative);
            //ResourceDictionary rd = new ResourceDictionary();
            //rd.Source = uri;
            //this.Style = rd["stlWindowEx"] as Style;
            #endregion

            #region 窗体按钮事件
            WindowBtnCommand windowBtnCommand = new WindowBtnCommand();
            windowBtnCommand.DoAction = (parameter) =>
            {
                if (parameter == 1) //最小化
                {
                    this.BorderMargin = new Thickness(1, 0, 0, 0);
                    BtnPanelHorizontalAlignment = HorizontalAlignment.Left;
                    BtnMinimizeVisibility = Visibility.Collapsed;
                    this.WindowState = WindowState.Minimized;
                }
                if (parameter == 2) //窗口还原、最大化
                {
                    if (this.WindowState == WindowState.Normal)
                    {
                        double taskBarHeight = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height;
                        double taskBarWidth = SystemParameters.PrimaryScreenWidth - SystemParameters.WorkArea.Width;
                        if (taskBarWidth > 0)
                        {
                            this.BorderMargin = new Thickness(0, 0, taskBarWidth, 0);
                        }
                        if (taskBarHeight > 0)
                        {
                            this.BorderMargin = new Thickness(0, 0, 0, taskBarHeight);
                        }
                        BtnPanelHorizontalAlignment = HorizontalAlignment.Right;
                        BtnMinimizeVisibility = Visibility.Visible;
                        this.WindowState = WindowState.Maximized;
                    }
                    else if (this.WindowState == WindowState.Maximized)
                    {
                        this.BorderMargin = new Thickness(0, 0, 0, 0);
                        BtnPanelHorizontalAlignment = HorizontalAlignment.Right;
                        BtnMinimizeVisibility = Visibility.Visible;
                        this.WindowState = WindowState.Normal;
                    }
                    else if (this.WindowState == WindowState.Minimized)
                    {
                        this.BorderMargin = new Thickness(0, 0, 0, 0);
                        BtnPanelHorizontalAlignment = HorizontalAlignment.Right;
                        BtnMinimizeVisibility = Visibility.Visible;
                        this.WindowState = WindowState.Normal;
                    }
                }
                if (parameter == 3) //关闭窗口
                {
                    this.Close();
                }
            };
            this.WindowBtnCommand = windowBtnCommand;
            this.StateChanged += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    BtnPanelHorizontalAlignment = HorizontalAlignment.Right;
                    BtnMinimizeVisibility = Visibility.Visible;
                    double taskBarHeight = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height;
                    double taskBarWidth = SystemParameters.PrimaryScreenWidth - SystemParameters.WorkArea.Width;
                    if (taskBarWidth > 0)
                    {
                        this.BorderMargin = new Thickness(0, 0, taskBarWidth, 0);
                    }
                    if (taskBarHeight > 0)
                    {
                        this.BorderMargin = new Thickness(0, 0, 0, taskBarHeight);
                    }
                }
                if (this.WindowState == WindowState.Normal)
                {
                    this.BorderMargin = new Thickness(0, 0, 0, 0);
                    BtnPanelHorizontalAlignment = HorizontalAlignment.Right;
                    BtnMinimizeVisibility = Visibility.Visible;
                }
            };
            #endregion

        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class WindowBtnCommand : ICommand
    {
        public Action<int> DoAction { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (DoAction != null)
            {
                DoAction(Convert.ToInt32(parameter));
            }
        }
    }

}
