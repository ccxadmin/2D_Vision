﻿using FilesRAW.Common;
using MainFormLib.Views;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace VisionStartup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FormGifShow f_GifShow;//进度显示
        UCVision newf = null;
        double width, height;
        int suffix = 1;
        Dictionary<string, string> VisionDicName = new Dictionary<string, string>();
        Dictionary<string, UCVision> VisionDic = new Dictionary<string, UCVision>();

        public MainWindow()
        {
            InitializeComponent();
            width = this.panel0.Width;
            height = this.panel0.Height;
            tbc.Items.Clear();
            Load();
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
            bool wholeShow = bool.Parse(GeneralUse.ReadValue("窗体", "完整显示", "config", "true", "Config"));
            if (!wholeShow)
            {

                win.WindowStyle = WindowStyle.None;

                rd1.Height = new GridLength(0);
                tb.Visibility = Visibility.Collapsed;
                tbar.Visibility = Visibility.Collapsed;
                rd3.Height = new GridLength(0);
                GeneralUse.WriteValue("窗体", "完整显示", "false", "config", "Config");
            }
            else
            {
                win.WindowStyle = WindowStyle.SingleBorderWindow;
                tb.Visibility = Visibility.Visible;
                tbar.Visibility = Visibility.Visible;
                GeneralUse.WriteValue("窗体", "完整显示", "true", "config", "Config");
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var v in VisionDic.Values)
            {
                v.viewModel.Release();
            }
        }
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }
        private static Object ExitFrame(Object state)
        {
            ((DispatcherFrame)state).Continue = false;
            return null;
        }
        /// <summary>
        /// 加载
        /// </summary>
        void Load()
        {
            try
            {
                string workPath = AppDomain.CurrentDomain.BaseDirectory + "workflow.flw";
                VisionDicName = GeneralUse.ReadSerializationFile<Dictionary<string, string>>(workPath);
                if (VisionDicName == null) VisionDicName = new Dictionary<string, string>();

                if (VisionDicName.Count < 1)
                {
                    newf = UCVision.CreateInstance("camStationName_1");
                    newf.Margin = new Thickness(1);
                    newf.Width = width;
                    newf.Height = height;
                    newf.HorizontalAlignment = HorizontalAlignment.Stretch;
                    newf.VerticalAlignment = VerticalAlignment.Stretch;

                    TabItem item = new TabItem();
                    Uri uri = new Uri("pack://application:,,,/ControlShareResources;component/Themes/TabControl2.xaml", UriKind.RelativeOrAbsolute);
                    ResourceDictionary rd = new ResourceDictionary();
                    rd.Source = uri;
                    item.Style = rd["TabItemStyle"] as Style;
                    item.Header = "Main";
                    Grid grid = new Grid();
                    if (!VisionDicName.ContainsKey(item.Header.ToString()))
                        VisionDicName.Add(item.Header.ToString(), newf.viewModel.CurrCamStationName);
                    if (!VisionDic.ContainsKey(item.Header.ToString()))
                        VisionDic.Add(item.Header.ToString(), newf);
                    grid.Children.Add(newf);
                    item.Content = grid;
                    tbc.Items.Add(item);
                }
                else
                {
                    foreach (var s in VisionDicName)
                    {
                        newf = UCVision.CreateInstance(s.Value);
                        newf.Margin = new Thickness(1);
                        newf.Width = this.panel0.Width;
                        newf.Height = this.panel0.Height;
                        newf.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newf.VerticalAlignment = VerticalAlignment.Stretch;

                        TabItem item = new TabItem();
                        Uri uri = new Uri("pack://application:,,,/ControlShareResources;component/Themes/TabControl2.xaml", UriKind.RelativeOrAbsolute);
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = uri;
                        item.Style = rd["TabItemStyle"] as Style;

                        item.Header = s.Key;
                        Grid grid = new Grid();
                        if (!VisionDicName.ContainsKey(item.Header.ToString()))
                            VisionDicName.Add(item.Header.ToString(), newf.viewModel.CurrCamStationName);
                        if (!VisionDic.ContainsKey(item.Header.ToString()))
                            VisionDic.Add(item.Header.ToString(), newf);
                        grid.Children.Add(newf);
                        item.Content = grid;
                        tbc.Items.Add(item);
                    }
                }
                initStatus.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                initStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 109, 60));
                initStatus.Content = "工作流程初始化成功";
            }
            catch (Exception er)
            {
                MessageBox.Show("工作流程初始化失败：" + er.Message);
                initStatus.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                initStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                initStatus.Content = "工作流程初始化失败：" + er.Message;
            }
            // 在UI线程上执行更新操作
            // 更新绑定数据的代码
            if (f_GifShow != null && f_GifShow.IsLoaded)
                f_GifShow.Close();
        }
        /// <summary>
        /// 保存
        /// </summary>
        void Save()
        {
            try
            {
                string workPath = AppDomain.CurrentDomain.BaseDirectory + "workflow.flw";
                GeneralUse.WriteSerializationFile<Dictionary<string, string>>(workPath, VisionDicName);
            }
            catch (Exception er)
            {
                MessageBox.Show("工作流程保存失败：" + er.Message);
                initStatus.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                initStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                initStatus.Content = "工作流程保存失败：" + er.Message;
            }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewClick(object sender, RoutedEventArgs e)
        {
            do
            { suffix = suffix + 1; }
            while (VisionDicName.ContainsKey("Page" + suffix));

            newf = UCVision.CreateInstance("camStationName_" + suffix);
            newf.Margin = new Thickness(1);
            newf.Width = this.panel0.Width;
            newf.Height = this.panel0.Height;
            newf.HorizontalAlignment = HorizontalAlignment.Stretch;
            newf.VerticalAlignment = VerticalAlignment.Stretch;

            TabItem item = new TabItem();
            Uri uri = new Uri("pack://application:,,,/ControlShareResources;component/Themes/TabControl2.xaml", UriKind.RelativeOrAbsolute);
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = uri;
            item.Style = rd["TabItemStyle"] as Style;
            item.Header = "Page" + suffix;
            Grid grid = new Grid();
            if (VisionDicName.ContainsKey(item.Header.ToString())) return;
            VisionDicName.Add(item.Header.ToString(), newf.viewModel.CurrCamStationName);
            VisionDic.Add(item.Header.ToString(), newf);
            //CopyFolder();
            grid.Children.Add(newf);
            item.Content = grid;
            tbc.Items.Add(item);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteClick(object sender, RoutedEventArgs e)
        {
            int index = tbc.SelectedIndex;
            if (index < 1)
            {
                MessageBox.Show("Main工位不可被删除！");
                return;
            }
            string header = ((TabItem)(tbc.Items[index])).Header.ToString();
            if (MessageBox.Show("确认删除工位:" + header + "?", "Information", MessageBoxButton.OKCancel,
                MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                tbc.Items.RemoveAt(index);
                if (!VisionDicName.ContainsKey(header)) return;
                //VisionDic[header].Dispose();
                string preDeletePath = AppDomain.CurrentDomain.BaseDirectory + VisionDicName[header];
                if (Directory.Exists(preDeletePath))
                    Directory.Delete(preDeletePath, true);
                VisionDic.Remove(header);
                VisionDicName.Remove(header);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            Save();
            initStatus.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            initStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 109, 60));
            initStatus.Content = "工作流程保存成功";
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExitClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认退出应用？", "Warning", MessageBoxButton.OKCancel,
             MessageBoxImage.Warning) == MessageBoxResult.OK)
                foreach (var v in VisionDic.Values)
                {
                    v.viewModel.Release();
                }
        }
        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        private bool CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                //如果目标路径不存在,则创建目标路径
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                //得到原文件根目录下的所有文件
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest, true);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);//构建目标路径,递归复制文件
                }
                return true;
            }
            catch (Exception er)
            {
                return false;
            }
        }
    }
}