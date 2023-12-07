using System;
using System.Collections.Generic;
using System.Data;
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
using ControlShareResources;
using LightSourceController.Models;
using LightSourceController.ViewModels;

namespace LightSourceController.Views
{
    /// <summary>
    /// LightSourceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LightSourceWindow :Window
    {
        public LightSourceWindow(LightSource _lightSource)
        {
            InitializeComponent();
            var model =  LightSourceViewModel.CreateSingleInstance(_lightSource);         
            DataContext = model;
        }

      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
          
        
        }

        private void mySlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            LightSourceViewModel.This.LightSliderValueChangedCommand.DoExecute?.Invoke(sender);
        }
    }
}
