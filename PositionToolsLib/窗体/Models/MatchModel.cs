using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Models
{
    public class MatchModel : NotifyBase
    {
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

        private int startAngle = -90;
        public int StartAngle
        {
            get { return this.startAngle; }
            set
            {
                startAngle = value;
                DoNotify();
            }
        }
        private int rangeAngle = 180;
        public int RangeAngle
        {
            get { return this.rangeAngle; }
            set
            {
                rangeAngle = value;
                DoNotify();
            }
        }

        private byte contrast = 20;
        public byte Contrast
        {
            get { return this.contrast; }
            set
            {
                contrast = value;
                DoNotify();
            }
        }

        private double matchScore = 0.5;
        public double MatchScore
        {
            get { return this.matchScore; }
            set
            {
                matchScore = value;
                DoNotify();
            }
        }

        private double scaleDownLimit = 1.0;
        public double ScaleDownLimit
        {
            get { return this.scaleDownLimit; }
            set
            {
                scaleDownLimit = value;
                DoNotify();
            }
        }

        private double scaleUpLimit = 1.0;
        public double ScaleUpLimit
        {
            get { return this.scaleUpLimit; }
            set
            {
                scaleUpLimit = value;
                DoNotify();
            }
        }

        private string searchModelROISelectName = "全图搜索";
        public string SearchModelROISelectName
        {
            get { return this.searchModelROISelectName; }
            set
            {
                searchModelROISelectName = value;
                DoNotify();
            }
        }

        private int searchModelROISelectIndex;
        public int SearchModelROISelectIndex
        {
            get { return this.searchModelROISelectIndex; }
            set
            {
                searchModelROISelectIndex = value;
                DoNotify();
            }
        }

        private bool btnDrawModelSearchRegionEnable = false;
        public bool BtnDrawModelSearchRegionEnable
        {
            get { return this.btnDrawModelSearchRegionEnable; }
            set
            {
                btnDrawModelSearchRegionEnable = value;
                DoNotify();
            }
        }

        private string baseXText = "0.000";
        public string BaseXText
        {
            get { return this.baseXText; }
            set
            {
                baseXText = value;
                DoNotify();
            }
        }
        private string baseYText = "0.000";
        public string BaseYText
        {
            get { return this.baseYText; }
            set
            {
                baseYText = value;
                DoNotify();
            }
        }
        private string baseAngleText = "0.000";
        public string BaseAngleText
        {
            get { return this.baseAngleText; }
            set
            {
                baseAngleText = value;
                DoNotify();
            }
        }
      

        private ObservableCollection<DgResultOfMatch> dgResultOfMatchList =
           new ObservableCollection<DgResultOfMatch>();
        public ObservableCollection<DgResultOfMatch> DgResultOfMatchList
        {
            get { return this.dgResultOfMatchList; }
            set
            {
                dgResultOfMatchList = value;
                DoNotify();

            }
        }

    }

    [Serializable]
    public class DgResultOfMatch
    {
        public DgResultOfMatch( int id,double score, double x,
                         double y, double angle)
        {
            ID = id;
            Score = score;
            X = x;
            Y = y;
            Angle = angle;
        }
        public int  ID { get; set; }
        public double Score { get; set; }
        public double  X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }

    }

}
