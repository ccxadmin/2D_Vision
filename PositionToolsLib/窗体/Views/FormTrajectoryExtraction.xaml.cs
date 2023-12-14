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
    /// FormTrajectoryExtraction.xaml 的交互逻辑
    /// </summary>
    public partial class FormTrajectoryExtraction : Window
    {
        public FormTrajectoryExtraction()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fra.Source=new Uri("LinePage.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
