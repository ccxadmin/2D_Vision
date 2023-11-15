using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionShowLib;
using HalconDotNet;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        /// <summary>
        /// 装载winform控件，主动释放方式内存泄漏
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            winHost.Child.Dispose();
            winHost.Child = null;
            base.OnClosed(e);
        }

        VisionShowTool tool; HObject grabImg = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");

            tool = new VisionShowTool();
            tool.SetColorOfTopBottomTitle(System.Drawing.Color.FromArgb(255, 109, 60));
            tool.Dock = System.Windows.Forms.DockStyle.Fill;
            tool.Padding = new System.Windows.Forms.Padding(2);
            tool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            tool.SetBackgroundColor(EumControlBackColor.white);
            tool.setDraw(EumDrawModel.margin);
            winHost.Child = tool;
        }

        
        void LoadedImageNoticeEvent(object sender, EventArgs e)
        {
            HOperatorSet.GenEmptyObj(out grabImg);
            grabImg.Dispose();
            grabImg = tool.D_HImage;
          
        }

    }
}
