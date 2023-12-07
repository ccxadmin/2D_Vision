using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlShareResources.UserControls
{
    /// <summary>
    /// MyNumUpDown.xaml 的交互逻辑
    /// </summary>
    public partial class MyNumUpDown : UserControl
    {
        public MyNumUpDown()
        {
            InitializeComponent();
        }

        /// <summary>
		/// 当前值
		/// </summary>
		private decimal AvaliableVal = 0;
        /// <summary>
		/// Sync Resource
		/// </summary>
		private static object lockObj = new object();

        /// <summary>
        /// 最小值
        /// </summary>
        public decimal MinValue
        {
            get { return Convert.ToDecimal(GetValue(MinValueProperty)); }
            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        /// 最大值
        /// </summary>
        public decimal MaxValue
        {
            get { return Convert.ToDecimal(GetValue(MaxValueProperty)); }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        public decimal DefaultValue
        {
            get { return Convert.ToDecimal(GetValue(DefaultValueProperty)); }
            set
            {
                SetValue(DefaultValueProperty, value);
                NumericValue = DefaultValue;
            }
        }
        /// <summary>
        /// 是否整数
        /// </summary>
        public bool IsInteger
        {
            get { return Convert.ToBoolean(GetValue(IsIntegerProperty)); }
            set { SetValue(IsIntegerProperty, value); }
        }

        /// <summary>
        /// 步长
        /// </summary>
        public decimal OffsetValue
        {
            get { return Convert.ToDecimal(GetValue(OffsetValueProperty)); }
            set { SetValue(OffsetValueProperty, value); }
        }

        /// <summary>
        /// 输入框值
        /// </summary>
        public decimal NumericValue
        {
            get { return Convert.ToDecimal(GetValue(NumericValueProperty)); }
            set
            {
                SetValue(NumericValueProperty, value);
            }
        }


        //属性包装器
        public Action ValueChangeAction
        {
            get { return (Action)GetValue(ValueChangeActionProperty); }
            set { SetValue(ValueChangeActionProperty, value); }

        }

        public static readonly DependencyProperty ValueChangeActionProperty =
                DependencyProperty.RegisterAttached("ValueChangeAction", typeof(Action), typeof(MyNumUpDown), new PropertyMetadata(null));


        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue",
                typeof(decimal),
                typeof(MyNumUpDown),
                new PropertyMetadata(-1m, null)
         );

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue",
                typeof(decimal),
                typeof(MyNumUpDown),
                new PropertyMetadata(-1m, new PropertyChangedCallback(MaxValueChangedCallback))
         );

        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register("DefaultValue",
                typeof(decimal),
                typeof(MyNumUpDown)
         );

        public static readonly DependencyProperty IsIntegerProperty =
            DependencyProperty.Register("IsInteger",
                typeof(bool),
                typeof(MyNumUpDown), new PropertyMetadata(false)
         );

        public static readonly DependencyProperty OffsetValueProperty =
             DependencyProperty.Register("OffsetValue",
                typeof(decimal),
                typeof(MyNumUpDown), new PropertyMetadata(1m)
         );

        public static readonly DependencyProperty NumericValueProperty =
            DependencyProperty.Register("NumericValue",
               typeof(decimal),
               typeof(MyNumUpDown),
               new PropertyMetadata(new PropertyChangedCallback(NumericValueChangedCallback))
        );


        private static void MaxValueChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MyNumUpDown control = obj as MyNumUpDown;

            control.tbContent.MaxLength = e.NewValue.ToString().Length;
        }

        private static void NumericValueChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MyNumUpDown control = obj as MyNumUpDown;
            control.AvaliableVal = Convert.ToDecimal(e.NewValue);
            control.tbContent.Text = e.NewValue.ToString();
            control.tbContent.SelectionStart = control.tbContent.Text.Length;
        }
      

        /// <summary>
        /// 增加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddClick(object sender, RoutedEventArgs e)
        {
            decimal offsetValue = OffsetValue;
            AvaliableVal = Convert.ToDecimal(tbContent.Text);
            decimal newValue;

            if (AvaliableVal > MaxValue)
            {
                NumericValue = MaxValue;
                return;
            }
            if (AvaliableVal + offsetValue > MaxValue)
            {
                NumericValue = MaxValue;
                return;
            }
            newValue = AvaliableVal + offsetValue;
            if (newValue < MinValue)
            {
                NumericValue = MinValue;
                return;
            }
            if (IsInteger)
                newValue = decimal.Parse(newValue.ToString("0"));
            else
                newValue = decimal.Parse(newValue.ToString("0.######"));
            tbContent.Text = newValue.ToString();
            NumericValue = newValue;
            ValueChangeAction?.Invoke();
    }

        /// <summary>
        /// 减少按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubClick(object sender, RoutedEventArgs e)
        {
            AvaliableVal = Convert.ToDecimal(tbContent.Text);
            if (AvaliableVal < MinValue)
            {
                NumericValue = MinValue;
                return;
            }
            if (AvaliableVal - OffsetValue < MinValue)
            {
                NumericValue = MinValue;
                return;
            }
            if (AvaliableVal > MaxValue)
            {
                NumericValue = MaxValue;
                return;
            }
            decimal newValue;
            newValue = AvaliableVal - OffsetValue;
            if (IsInteger)
                newValue = decimal.Parse(newValue.ToString("0"));
            else
                newValue = decimal.Parse(newValue.ToString("0.######"));
            tbContent.Text = newValue.ToString();
            NumericValue = newValue;
            ValueChangeAction?.Invoke();
        }

        /// <summary>
        /// 获取焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbContentGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            this.AvaliableVal = Convert.ToDecimal(tb.Text);
        }

        /// <summary>
        /// 是否数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsNumber(string input)
        {
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 是否整数
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        private bool IsWholeNumber(string strNumber)
        {
            Regex g = new Regex(@"^[0-9]\d*$");
            return g.IsMatch(strNumber);
        }

        /// <summary>
        /// 数字Text变动Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbContentTextChanged(object sender, TextChangedEventArgs e)
        {
            lock (lockObj)
            {
                TextBox tb = sender as TextBox;
                if (tb != null)
                {
                    bool isNumeric = IsNumber(tb.Text);
                    if (isNumeric)
                    {
                        var vv = Convert.ToDecimal(tb.Text);
                        if (vv > MaxValue && MaxValue != -1)
                        {
                            //此句为了解决实际应用时数据刷新的问题
                            NumericValue = MaxValue - 1;
                            try
                            {
                                System.Threading.Thread.Sleep(0);
                            }
                            catch { }
                            NumericValue = MaxValue;
                            return;
                        }
                        NumericValue = vv;
                    }
                    else
                    {
                        //NumericValue = null;
                    }
                }
            }
        }

        /// <summary>
        /// Lost焦点事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbCntLostFocus(object sender, RoutedEventArgs e)
        {
            lock (lockObj)
            {
                TextBox tb = sender as TextBox;
                bool isNumeric = IsNumber(tb.Text);
                if (isNumeric && IsInteger)
                {
                    isNumeric = IsWholeNumber(tb.Text);
                }
                decimal oldValue = this.AvaliableVal;
                if (isNumeric)
                {
                    decimal ctrlValue = Convert.ToDecimal(tb.Text);
                    if (ctrlValue < MinValue)
                    {
                        if (oldValue <= MaxValue && oldValue >= MinValue)
                            NumericValue = oldValue;
                        else
                            NumericValue = MinValue;
                        AvaliableVal = NumericValue;
                    }
                    else if (ctrlValue > MaxValue)
                    {
                        if (oldValue <= MaxValue && oldValue >= MinValue)
                            NumericValue = oldValue;
                        else
                            NumericValue = MaxValue;
                        AvaliableVal = NumericValue;
                    }
                    else
                    {
                        AvaliableVal = ctrlValue;
                        NumericValue = AvaliableVal;
                    }
                }
                else
                {
                    if (oldValue > MaxValue || oldValue < MinValue)
                        oldValue = DefaultValue;
                    tb.Text = oldValue.ToString();
                    NumericValue = oldValue;
                }
            }
        }
    }
}
