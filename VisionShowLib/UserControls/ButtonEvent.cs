using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisionShowLib.UserControls
{
     public partial class ButtonEvent : ResourceDictionary
    {
       static   public  RoutedEventHandler Button_Click { get; set; }

        private void BtnClick(object sender,RoutedEventArgs e)
        {
            Button_Click?.Invoke(sender,e);
        }
              
    }
}
