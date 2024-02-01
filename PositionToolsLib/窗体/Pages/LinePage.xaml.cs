
using HalconDotNet;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PositionToolsLib.窗体.Pages
{
    /// <summary>
    /// LinePage.xaml 的交互逻辑
    /// </summary>
    public partial class LinePage : Page
    {
        public LinePage()
        {
            InitializeComponent();
            This = this;
         
        }
        static public LinePage This { get; private set; }
        private TrajectoryTypeLineTool tool = null;
        public delegate void SaveToolHandle(TrajectoryTypeBaseTool tool);
        public SaveToolHandle OnSaveTool = null;
        public Action<string> OnDrawRegion = null;
        public Action OnLineIdentify = null;
        public Action PageLoad = null;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageLoad?.Invoke();
            
        }
       public void SetTool(TrajectoryTypeBaseTool m_tool)
        {
            tool = (TrajectoryTypeLineTool)m_tool;
            ShowData();
        }
        /// <summary>
        /// 显示数据
        /// </summary>
        void ShowData()
        {
            this.Dispatcher.Invoke(() =>
           {
            TrajectoryTypeLineParam lineParam = (TrajectoryTypeLineParam)tool.param;
            numThd.NumericValue = lineParam.EdgeContrast;
            numCaliperNum.NumericValue = lineParam.CaliperNum;
            numCaliperWidth.NumericValue = lineParam.CaliperWidth;
            numCaliperHeight.NumericValue = lineParam.CaliperHeight;
            cobxTransition.SelectedIndex = (int)lineParam.Transition;
            cobxSelect.SelectedIndex = (int)lineParam.Select;
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
        /// 直线识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLineIdentify_Click(object sender, RoutedEventArgs e)
        {
            OnLineIdentify?.Invoke();
        }
        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click(object sender, RoutedEventArgs e)
        {
            TrajectoryTypeLineParam lineParam = (TrajectoryTypeLineParam)tool.param;
            lineParam.EdgeContrast = (byte)numThd.NumericValue;
            lineParam.CaliperNum = (int)numCaliperNum.NumericValue;
            lineParam.CaliperWidth = (int)numCaliperWidth.NumericValue;
            lineParam.CaliperHeight = (int)numCaliperHeight.NumericValue;
            if (cobxTransition.SelectedIndex != -1)
                lineParam.Transition = (EumTransition)cobxTransition.SelectedIndex;
            if (cobxSelect.SelectedIndex != -1)
                lineParam.Select = (EumSelect)cobxSelect.SelectedIndex;

            tool.param = lineParam;
            OnSaveTool?.Invoke(tool);
        }

    }
}
