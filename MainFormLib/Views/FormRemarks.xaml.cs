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

namespace MainFormLib.Views
{
    /// <summary>
    /// FormRemarks.xaml 的交互逻辑
    /// </summary>
    public partial class FormRemarks : Window
    {
        public FormRemarks()
        {
            InitializeComponent();
            txbMarks.Focus();
        }


        public string remarks = "";
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            remarks = txbMarks.Text.Trim();
            this.DialogResult =true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult =false;
        }

        private void txbMarks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                remarks = txbMarks.Text.Trim();
                this.DialogResult = true;
                //this.Close();
            }
        }
    }
}
