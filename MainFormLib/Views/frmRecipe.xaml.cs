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
using visionForm.ViewModels;

namespace visionForm.Views
{
    /// <summary>
    /// frmRecipe.xaml 的交互逻辑
    /// </summary>
    public partial class frmRecipe : Window
    {
        public frmRecipe()
        {
            InitializeComponent();
            var model = new CalibrationViewModel();
            DataContext = model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
