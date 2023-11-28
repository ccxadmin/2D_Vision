using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlShareResources.Attach
{
    public class RadioButtonAttach
    {


        public static string GetSize(DependencyObject obj)
        {
            return (string)obj.GetValue(SizeProperty);
        }

        public static void SetSize(DependencyObject obj, string value)
        {
            obj.SetValue(SizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.RegisterAttached("Size", typeof(string), typeof(RadioButtonAttach), new PropertyMetadata(new PropertyChangedCallback((o,e)=> {
                string size = e.NewValue.ToString();
                if (!string.IsNullOrEmpty(size))
                {
                    switch (size)
                    {
                        case "medium":
                            (o as RadioButton).Width = 70;
                            (o as RadioButton).Height = 36;
                            (o as RadioButton).Padding = new Thickness(0, 9, 0, 0);
                            break;
                        case "small":
                            (o as RadioButton).Width = 56;
                            (o as RadioButton).Height = 32;
                            (o as RadioButton).Padding = new Thickness(0, 7, 0, 0);
                            break;
                        case "mini":
                            (o as RadioButton).Width = 56;
                            (o as RadioButton).Height = 28;
                            (o as RadioButton).Padding = new Thickness(0, 5, 0, 0);
                            break;
                    }
                }
            })));



        public static string GetFrameSize(DependencyObject obj)
        {
            return (string)obj.GetValue(FrameSizeProperty);
        }

        public static void SetFrameSize(DependencyObject obj, string value)
        {
            obj.SetValue(FrameSizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for FrameSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameSizeProperty =
            DependencyProperty.RegisterAttached("FrameSize", typeof(string), typeof(RadioButtonAttach), new PropertyMetadata(new PropertyChangedCallback((o,e)=> {
                string size = e.NewValue.ToString();
                if (!string.IsNullOrEmpty(size))
                {
                    switch (size)
                    {
                        case "medium":
                            (o as RadioButton).Width = 106;
                            (o as RadioButton).Height = 36;
                            //(o as CheckBox).Padding = new Thickness(0, 9, 0, 0);
                            break;
                        case "small":
                            (o as RadioButton).Width = 91;
                            (o as RadioButton).Height = 32;
                            //(o as CheckBox).Padding = new Thickness(0, 7, 0, 0);
                            break;
                        case "mini":
                            (o as RadioButton).Width = 91;
                            (o as RadioButton).Height = 28;
                            //(o as CheckBox).Padding = new Thickness(0, 5, 0, 0);
                            break;
                    }
                }
            })));


    }
}
