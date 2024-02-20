﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PositionToolsLib.窗体.Pages
{
    /// <summary>
    /// RectanglePage.xaml 的交互逻辑
    /// </summary>
    public partial class RectanglePage : Page
    {
        public RectanglePage()
        {
            InitializeComponent();
            This = this;
        }
        static public RectanglePage This { get; private set; }
        private TrajectoryTypeRectangleTool tool = null;
        public delegate void SaveToolHandle(TrajectoryTypeBaseTool tool);
        public SaveToolHandle OnSaveTool = null;
        public Action<string> OnDrawRegion = null;
        public Action OnRectangleIdentify = null;
        public Action PageLoad = null;
        public int LineIndex = 0;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageLoad?.Invoke();

        }
        public void SetTool(TrajectoryTypeBaseTool m_tool)
        {
            tool = (TrajectoryTypeRectangleTool)m_tool;
            ShowData();
        }

        /// <summary>
        /// 显示数据
        /// </summary>
        void ShowData()
        {
            this.Dispatcher.Invoke(() =>
            {
                LineIndex = cobxLineSelectIndex.SelectedIndex;
                if (LineIndex == -1)
                    LineIndex = cobxLineSelectIndex.SelectedIndex = 0;
                TrajectoryTypeRectangleParam RectangleParam = (TrajectoryTypeRectangleParam)tool.param;
                TrajectoryTypeLineParam par = RectangleParam.GetParamFormIndex(LineIndex);
                numThd.NumericValue = par.EdgeContrast;
                numCaliperNum.NumericValue = par.CaliperNum;
                numCaliperWidth.NumericValue = par.CaliperWidth;
                numCaliperHeight.NumericValue = par.CaliperHeight;
                cobxTransition.SelectedIndex = (int)par.Transition;
                cobxSelect.SelectedIndex = (int)par.Select;
            });
        }

        /// <summary>
        /// 绘制检测区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrawRegion_Click(object sender, RoutedEventArgs e)
        {
            OnDrawRegion?.Invoke(((Button)sender).Name);
        }
        /// <summary>
        /// 矩形识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRectangleIdentify_Click(object sender, RoutedEventArgs e)
        {
            OnRectangleIdentify?.Invoke();
        }
        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click(object sender, RoutedEventArgs e)
        {
            TrajectoryTypeRectangleParam RectangleParam
                        = (TrajectoryTypeRectangleParam)tool.param;
            int LineIndex = cobxLineSelectIndex.SelectedIndex;
            if (LineIndex == -1) return;
            TrajectoryTypeLineParam par = RectangleParam.GetParamFormIndex(LineIndex);
            par.EdgeContrast = (byte)numThd.NumericValue;
            par.CaliperNum = (int)numCaliperNum.NumericValue;
            par.CaliperWidth = (int)numCaliperWidth.NumericValue;
            par.CaliperHeight = (int)numCaliperHeight.NumericValue;
            if (cobxTransition.SelectedIndex != -1)
                par.Transition = (EumTransition)cobxTransition.SelectedIndex;
            if (cobxSelect.SelectedIndex != -1)
                par.Select = (EumSelect)cobxSelect.SelectedIndex;
            RectangleParam.SetParamFormIndex(par, LineIndex);
            tool.param = RectangleParam;
            OnSaveTool?.Invoke(tool);
        }

        private void cobxLineSelectIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowData();
        }

    }

  
}
