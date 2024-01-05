using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PositionToolsLib.窗体.Models
{
    public class FindLineModel : NotifyBase
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

        private byte numEdgeThd = 20;
        public byte NumEdgeThd
        {
            get { return this.numEdgeThd; }
            set
            {
                numEdgeThd = value;
                DoNotify();
            }
        }

        private int numCaliperCount = 10;
        public int NumCaliperCount
        {
            get { return this.numCaliperCount; }
            set
            {
                numCaliperCount = value;
                DoNotify();
            }
        }

        private int numCaliperWidth = 15;
        public int NumCaliperWidth
        {
            get { return this.numCaliperWidth; }
            set
            {
                numCaliperWidth = value;
                DoNotify();
            }
        }

        private int numCaliperHeight = 30;
        public int NumCaliperHeight
        {
            get { return this.numCaliperHeight; }
            set
            {
                numCaliperHeight = value;
                DoNotify();
            }
        }
        private string selectTransitionName;
        public string SelectTransitionName
        {
            get { return this.selectTransitionName; }
            set
            {
                selectTransitionName = value;
                DoNotify();
            }
        }

        private int selectTransitionIndex;
        public int SelectTransitionIndex
        {
            get { return this.selectTransitionIndex; }
            set
            {
                selectTransitionIndex = value;
                DoNotify();
            }
        }


        private string selectEdgeName;
        public string SelectEdgeName
        {
            get { return this.selectEdgeName; }
            set
            {
                selectEdgeName = value;
                DoNotify();
            }
        }

        private int selectEdgeIndex;
        public int SelectEdgeIndex
        {
            get { return this.selectEdgeIndex; }
            set
            {
                selectEdgeIndex = value;
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
        private bool btnDrawRegionEnable = true;
        public bool BtnDrawRegionEnable
        {
            get { return this.btnDrawRegionEnable; }
            set
            {
                btnDrawRegionEnable = value;
                DoNotify();
            }
        }

        private ObservableCollection<LineResultData> dgResultOfFindLineList =
    new ObservableCollection<LineResultData>();
        public ObservableCollection<LineResultData> DgResultOfFindLineList
        {
            get { return this.dgResultOfFindLineList; }
            set
            {
                dgResultOfFindLineList = value;
                DoNotify();

            }
        }
    }

    [Serializable]
    public class LineResultData
    {
        public LineResultData(int id,  double x1, double y1, double x2, 
            double y2, double angle)

        {
            ID = id;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Angle = angle;
           
        }
        public int ID { get; set; }    
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double Angle { get; set; }

    

    }
}
