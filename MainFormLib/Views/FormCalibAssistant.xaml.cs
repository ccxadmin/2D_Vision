using HalconDotNet;
using MainFormLib.ViewModels;
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

namespace MainFormLib.Views
{
    /// <summary>
    /// FormImageCorrect.xaml 的交互逻辑
    /// </summary>
    public partial class FormCalibAssistant : Window
    {
        public FormCalibAssistant(string path)
        {
            InitializeComponent();
            var model = new CalibAssistantViewModel(path);
            DataContext = model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");

            var tool = CalibAssistantViewModel.This.ShowTool;
            //tool.SetColorOfTopBottomTitle(System.Drawing.Color.FromArgb(255, 109, 60));
            tool.Dock = System.Windows.Forms.DockStyle.Fill;
            tool.Padding = new System.Windows.Forms.Padding(2);

            tool.SetBackgroundColor(EumControlBackColor.white);
            tool.setDraw(EumDrawModel.margin);
            host.Child = tool;
        }

        private void dgCalibImageInfo_Drop(object sender, DragEventArgs e)
        {
            CalibAssistantViewModel.This.DgDragDropCommand.DoExecute.Invoke(e);
        }

      
        private void dgCalibImageInfo_DragEnter(object sender, DragEventArgs e)
        {
            CalibAssistantViewModel.This.DgDragEnterCommand.DoExecute.Invoke(e);
        }
    }
}
