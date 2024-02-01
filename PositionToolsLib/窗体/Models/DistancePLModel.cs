using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Models
{
    public class DistancePLModel : NotifyBase
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
        private string camParamFilePath;
        public string CamParamFilePath
        {
            get { return this.camParamFilePath; }
            set
            {
                camParamFilePath = value;
                DoNotify();
            }
        }
        private string camPoseFilePath;
        public string CamPoseFilePath
        {
            get { return this.camPoseFilePath; }
            set
            {
                camPoseFilePath = value;
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
        private ObservableCollection<string> positionDataList = new ObservableCollection<string>();
        public ObservableCollection<string> PositionDataList
        {
            get { return this.positionDataList; }
            set
            {
                positionDataList = value;
                DoNotify();
            }
        }

        private string selectStartXName;
        public string SelectStartXName
        {
            get { return this.selectStartXName; }
            set
            {
                selectStartXName = value;
                DoNotify();
            }
        }
        private int selectStartXIndex;
        public int SelectStartXIndex
        {
            get { return this.selectStartXIndex; }
            set
            {
                selectStartXIndex = value;
                DoNotify();
            }
        }

        private string selectStartYName;
        public string SelectStartYName
        {
            get { return this.selectStartYName; }
            set
            {
                selectStartYName = value;
                DoNotify();
            }
        }
        private int selectStartYIndex;
        public int SelectStartYIndex
        {
            get { return this.selectStartYIndex; }
            set
            {
                selectStartYIndex = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 直线集合
        /// </summary>
        private ObservableCollection<string> lineList = new ObservableCollection<string>();
        public ObservableCollection<string> LineList
        {
            get { return this.lineList; }
            set
            {
                lineList = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择直线1名称
        /// </summary>
        private string selectLine1Name;
        public string SelectLine1Name
        {
            get { return this.selectLine1Name; }
            set
            {
                selectLine1Name = value;
                DoNotify();
            }
        }

        /// <summary>
        ///选择直线1索引编号
        /// </summary>
        private int selectLine1Index;
        public int SelectLine1Index
        {
            get { return this.selectLine1Index; }
            set
            {
                selectLine1Index = value;
                DoNotify();
            }
        }

        private ObservableCollection<DistancePLResultData> dgResultOfDistancePLList =
new ObservableCollection<DistancePLResultData>();
        public ObservableCollection<DistancePLResultData> DgResultOfDistancePLList
        {
            get { return this.dgResultOfDistancePLList; }
            set
            {
                dgResultOfDistancePLList = value;
                DoNotify();

            }
        }
        private double txbPixelRatio = 1;
        public double TxbPixelRatio
        {
            get { return this.txbPixelRatio; }
            set
            {
                txbPixelRatio = value;
                DoNotify();
            }
        }

        private bool usePixelRatio;
        public bool UsePixelRatio
        {
            get { return this.usePixelRatio; }
            set
            {
                usePixelRatio = value;
                DoNotify();
            }
        }
    }


    [Serializable]
    public class DistancePLResultData
    {
        public DistancePLResultData(int id, double distance)

        {
            ID = id;
            Distance = Math.Round(distance, 3);
        }
        public int ID { get; set; }

        public double Distance { get; set; }



    }
}
