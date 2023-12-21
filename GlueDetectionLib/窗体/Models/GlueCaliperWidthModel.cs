using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlueDetectionLib.窗体.Models
{
    public class GlueCaliperWidthModel : NotifyBase
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

     
        private ObservableCollection<DgDataOfGlueCaliperWidth> dgDataOfGlueCaliperWidthList =
            new ObservableCollection<DgDataOfGlueCaliperWidth>();
        public ObservableCollection<DgDataOfGlueCaliperWidth> DgDataOfGlueCaliperWidthList
        {
            get { return this.dgDataOfGlueCaliperWidthList; }
            set
            {
                dgDataOfGlueCaliperWidthList = value;
                DoNotify();
            
            }
        }
        /// <summary>
        ///选择表格索引编号
        /// </summary>
        private int dgDataSelectIndex;
        public int DgDataSelectIndex
        {
            get { return this.dgDataSelectIndex; }
            set
            {
                dgDataSelectIndex = value;
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
        private string selectImageName = "";
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
        private string selectMatrixName="";
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
        /// <summary>
        /// 像素转换比
        /// </summary>
        private string pixelRatio;
        public string PixelRatio
        {
            get { return this.pixelRatio; }
            set
            {
                pixelRatio = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 卡尺高度
        /// </summary>
        private int caliperHeight;
        public int CaliperHeight
        {
            get { return this.caliperHeight; }
            set
            {
                caliperHeight = value;
                DoNotify();
            }
        } 
        /// <summary>
          /// 卡尺阈值
          /// </summary>
        private byte caliperEdgeThd;
        public byte CaliperEdgeThd
        {
            get { return this.caliperEdgeThd; }
            set
            {
                caliperEdgeThd = value;
                DoNotify();
            }
        }

        /// <summary>
        /// 边距下限
        /// </summary>
        private double distanceMin;
        public double DistanceMin
        {
            get { return this.distanceMin; }
            set
            {
                distanceMin = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 边距上限
        /// </summary>
        private double distanceMax;
        public double DistanceMax
        {
            get { return this.distanceMax; }
            set
            {
                distanceMax = value;
                DoNotify();
            }
        }

        private bool usePosiCorrect;
        public bool UsePosiCorrect
        {
            get { return this.usePosiCorrect; }
            set
            {
                usePosiCorrect = value;
                DoNotify();
            }
        }

        private bool cobxMatrixListEnable;
        public bool CobxMatrixListEnable
        {
            get { return this.cobxMatrixListEnable; }
            set
            {
                cobxMatrixListEnable = value;
                DoNotify();
            }
        }

    }

   
    public class DgDataOfGlueCaliperWidth
    {
        public DgDataOfGlueCaliperWidth(bool use,string caliperName,string toolStatus)
        {
            Use = use;
            CaliperName = caliperName;
            ToolStatus = toolStatus;
        }
        public bool Use { get; set; }
        public string CaliperName { get; set; }
        public string ToolStatus { get; set; }

    }


}
