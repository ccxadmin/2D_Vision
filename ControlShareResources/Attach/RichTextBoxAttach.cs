using ControlShareResources.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlShareResources.Attach
{
    public class RichTextBoxAttach : DependencyObject
    {
        public static string GetRichText(DependencyObject obj)
        {
            return (string)obj.GetValue(RichTextProperty);
        }
        public static void SetRichText(DependencyObject obj, string value)
        {
            obj.SetValue(RichTextProperty, value);
        }
        public static readonly DependencyProperty RichTextProperty =
            DependencyProperty.RegisterAttached("RichText", typeof(string), typeof(RichTextBoxAttach), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                PropertyChangedCallback = (obj, e) =>
                {
                    var richTextBox = (RichTextBox)obj;
                    var text = GetRichText(richTextBox);
                    richTextBox.AppendText(text);
                    richTextBox.AppendText("\r");
                    richTextBox.ScrollToEnd();
                }

            });



        ////属性包装器
        //public bool ClearRichText
        //{
        //    get { return (bool)GetValue(ClearRichTextProperty); }
        //    set { SetValue(ClearRichTextProperty, value); }

        //}
        public static bool GetClearRichText(DependencyObject obj)
        {
            return (bool)obj.GetValue(ClearRichTextProperty);
        }
        public static void SetClearRichText(DependencyObject obj, string value)
        {
            obj.SetValue(ClearRichTextProperty, value);
        }

        public static readonly DependencyProperty ClearRichTextProperty =
            DependencyProperty.RegisterAttached("ClearRichText", typeof(bool), typeof(RichTextBoxAttach), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                PropertyChangedCallback = (obj, e) =>
                {
                    var richTextBox = (RichTextBox)obj;
                    var flag = GetClearRichText(richTextBox);
                    if(flag)
                      richTextBox.Document.Blocks.Clear();                  
                }

            });
    }
}
