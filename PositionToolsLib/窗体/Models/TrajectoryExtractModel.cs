using ControlShareResources.Common;
using FunctionLib.Cam;
using PositionToolsLib.窗体.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PositionToolsLib.窗体.Models
{
    public class TrajectoryExtractModel : NotifyBase
    {
        public TrajectoryExtractModel()
        {
            foreach (EumTrackType s in Enum.GetValues(typeof(EumTrackType)))
            { TrackTypeList.Add(s); }
          
        }

        private ObservableCollection<EumTrackType> trackTypeList = new ObservableCollection<EumTrackType>();
        public ObservableCollection<EumTrackType> TrackTypeList
        {
            get { return this.trackTypeList; }
            set
            {
                trackTypeList = value;
                DoNotify();
            }
        }
      
        private string selectTrajectoryName;
        public string SelectTrajectoryName
        {
            get { return this.selectTrajectoryName; }
            set
            {
                selectTrajectoryName = value;
                DoNotify();
            }
        }
       
        private int selectTrajectoryIndex;
        public int SelectTrajectoryIndex
        {
            get { return this.selectTrajectoryIndex; }
            set
            {
                selectTrajectoryIndex = value;
                DoNotify();
            }
        }


        /// <summary>
        /// 窗体标题
        /// </summary>
        private string titleName = "参数设置窗体";
        public string TitleName
        {
            get { return this.titleName; }
            set
            {
                titleName = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 图像源集合
        /// </summary>
        private ObservableCollection<string> imageList = new ObservableCollection<string>();
        public ObservableCollection<string> ImageList
        {
            get { return this.imageList; }
            set
            {
                imageList = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择图像名称
        /// </summary>
        private string selectImageName;
        public string SelectImageName
        {
            get { return this.selectImageName; }
            set
            {
                selectImageName = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择图像索引编号
        /// </summary>
        private int selectImageIndex;
        public int SelectImageIndex
        {
            get { return this.selectImageIndex; }
            set
            {
                selectImageIndex = value;
                DoNotify();
            }
        }

        private bool usePosiCorrectChecked = false;
        public bool UsePosiCorrectChecked
        {
            get { return this.usePosiCorrectChecked; }
            set
            {
                usePosiCorrectChecked = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 矩阵源集合
        /// </summary>
        private ObservableCollection<string> matrixList = new ObservableCollection<string>();
        public ObservableCollection<string> MatrixList
        {
            get { return this.matrixList; }
            set
            {
                matrixList = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择矩阵名称
        /// </summary>
        private string selectMatrixName = "";
        public string SelectMatrixName
        {
            get { return this.selectMatrixName; }
            set
            {
                selectMatrixName = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择矩阵索引编号
        /// </summary>
        private int selectMatrixIndex;
        public int SelectMatrixIndex
        {
            get { return this.selectMatrixIndex; }
            set
            {
                selectMatrixIndex = value;
                DoNotify();
            }
        }

        private bool matrixEnable = false;
        public bool MatrixEnable
        {
            get { return this.matrixEnable; }
            set
            {
                matrixEnable = value;
                DoNotify();
            }
        }
        private Uri framePath = new Uri("../pages/AnyCurvePage.xaml", UriKind.RelativeOrAbsolute);
        public Uri FramePath
        {
            get { return this.framePath; }
            set
            {
                framePath = value;
                DoNotify();
            }
        }

        private int dgTrajectorySelectIndex=-1;
        public int DgTrajectorySelectIndex
        {
            get { return this.dgTrajectorySelectIndex; }
            set
            {
                dgTrajectorySelectIndex = value;
                DoNotify();
            }
        }

        private ObservableCollection<DgTrajectoryData> dgTrajectoryDataList
             =new ObservableCollection<DgTrajectoryData> ();
        public ObservableCollection<DgTrajectoryData> DgTrajectoryDataList
        {
            get { return this.dgTrajectoryDataList; }
            set
            {
                dgTrajectoryDataList = value;
                DoNotify();
            }
        }

    }
  

    /// <summary>
    /// 轨迹类型枚举
    /// </summary>
    public enum EumTrackType
    {
        [Description("任意曲线")]
        /// <summary>
        /// 任意曲线
        /// </summary>
        AnyCurve,

        [Description("直线")]
        /// <summary>
        /// 直线
        /// </summary>
        Line,

        [Description("圆弧")]
        /// <summary>
        /// 圆弧
        /// </summary>
        Circle,

       [Description("矩形")]
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle,

        [Description("旋转矩形")]
        /// <summary>
        ///旋转矩形
        /// </summary>
        RRectangle
    }
}
