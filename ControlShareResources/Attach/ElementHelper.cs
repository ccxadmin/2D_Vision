﻿using System.Windows;

namespace ControlShareResources.Attach
{
    public class ElementHelper 
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ElementHelper),
               new PropertyMetadata(null));
        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }



        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(ElementHelper),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsStripeProperty =
            DependencyProperty.RegisterAttached("IsStripe", typeof(bool), typeof(ElementHelper),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsRoundProperty =
           DependencyProperty.RegisterAttached("IsRound", typeof(bool), typeof(ElementHelper),
               new PropertyMetadata(false));

      
        public static string GetWatermark(DependencyObject obj)
        {
            return (string) obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        public static bool GetIsStripe(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsStripeProperty);
        }

        public static void SetIsStripe(DependencyObject obj, bool value)
        {
            obj.SetValue(IsStripeProperty, value);
        }
        public static bool GetIsRound(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRoundProperty);
        }

        public static void SetIsRound(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRoundProperty, value);
        }
    }
}