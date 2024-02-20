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
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class AnyCurvePage : Page
    {
        public AnyCurvePage()
        {
            InitializeComponent();
            This = this;
        }

        static public AnyCurvePage This { get; private set; }
        private TrajectoryTypeAnyCurveTool tool = null;
        public delegate void SaveToolHandle(TrajectoryTypeBaseTool tool);
        public SaveToolHandle OnSaveTool = null;
        public Action<string> OnDrawRegion = null;
        public Action OnAnyCurveIdentify = null;
        public Action OnDrawMaskRegion = null;
        public Action PageLoad = null;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageLoad?.Invoke();

        }
        public void SetTool(TrajectoryTypeBaseTool m_tool)
        {
            tool = (TrajectoryTypeAnyCurveTool)m_tool;
            ShowData();
        }
        /// <summary>
        /// 显示数据
        /// </summary>
        void ShowData()
        {
            this.Dispatcher.Invoke(() =>
            {
                TrajectoryTypeAnyCurveParam AnyCurveParam = (TrajectoryTypeAnyCurveParam)tool.param;
                cobxRegionType.SelectedIndex = (int)AnyCurveParam.RegionType;
                numThdMin.NumericValue = AnyCurveParam.EdgeThdMin;
                numThdMax.NumericValue = AnyCurveParam.EdgeThdMax;
                chxbXldClosed.IsChecked = AnyCurveParam.IsXldClosed;
                numXldLengthMin.NumericValue= AnyCurveParam.XldLengthMin;
                numXldLengthMax.NumericValue = AnyCurveParam.XldLengthMax;
                numSamplingPoints.NumericValue = AnyCurveParam.SamplingPointNums;
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
        /// 任意曲线识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnyCurveIdentify_Click(object sender, RoutedEventArgs e)
        {
            OnAnyCurveIdentify?.Invoke();
        }


        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click(object sender, RoutedEventArgs e)
        {
            TrajectoryTypeAnyCurveParam AnyCurveParam = (TrajectoryTypeAnyCurveParam)tool.param;
            AnyCurveParam.RegionType= (EumRegionTypeOfGJ)(cobxRegionType.SelectedIndex);
            AnyCurveParam.EdgeThdMin= (byte)numThdMin.NumericValue ;
            AnyCurveParam.EdgeThdMax= (byte)numThdMax.NumericValue ;
            AnyCurveParam.IsXldClosed=chxbXldClosed.IsChecked.Value ;
            AnyCurveParam.XldLengthMin= (int)numXldLengthMin.NumericValue ;
            AnyCurveParam.XldLengthMax=(int)numXldLengthMax.NumericValue ;
            AnyCurveParam.SamplingPointNums= (int)numSamplingPoints.NumericValue ;
            tool.param = AnyCurveParam;
            OnSaveTool?.Invoke(tool);
        }
        /// <summary>
        /// 绘制掩膜区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrawMaskRegion_Click(object sender, RoutedEventArgs e)
        {
            OnDrawMaskRegion?.Invoke();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tool == null) return;
            TrajectoryTypeAnyCurveParam AnyCurveParam = (TrajectoryTypeAnyCurveParam)tool.param;
            AnyCurveParam.RegionType = (EumRegionTypeOfGJ)(cobxRegionType.SelectedIndex);          
            tool.param = AnyCurveParam;
            OnSaveTool?.Invoke(tool);
        }
    }
}
