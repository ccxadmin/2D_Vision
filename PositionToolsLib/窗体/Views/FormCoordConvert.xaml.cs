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
using VisionShowLib.UserControls;

namespace PositionToolsLib.窗体.Views
{
    /// <summary>
    /// FormCoordConvert.xaml 的交互逻辑
    /// </summary>
    public partial class FormCoordConvert : Window
    {
        public FormCoordConvert(BaseTool tool)
        {
            InitializeComponent();

            var model = new CoordConvertViewModel(tool);
            DataContext = model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
    }
}
