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
    internal class BlobModel : NotifyBase
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

        private byte numGrayMin;
        public byte NumGrayMin
        {
            get { return this.numGrayMin; }
            set
            {
                numGrayMin = value;
                DoNotify();
            }
        }

        private byte numGrayMax;
        public byte NumGrayMax
        {
            get { return this.numGrayMax; }
            set
            {
                numGrayMax = value;
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

        private ObservableCollection<ParticleFeaturesData> dgDataOfBlobList =
          new ObservableCollection<ParticleFeaturesData>();
        public ObservableCollection<ParticleFeaturesData> DgDataOfBlobList
        {
            get { return this.dgDataOfBlobList; }
            set
            {
                dgDataOfBlobList = value;
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


        private ObservableCollection<string> itemList = new ObservableCollection<string>();
        public ObservableCollection<string> ItemList
        {
            get { return this.itemList; }
            set
            {
                itemList = value;
                DoNotify();
            }
        }

        private bool btnDrawRegionEnable=true;
        public bool BtnDrawRegionEnable
        {
            get { return this.btnDrawRegionEnable; }
            set
            {
                btnDrawRegionEnable = value;
                DoNotify();
            }
        }
        private ObservableCollection<BlobFeaturesResultData> dgResultOfBlobList =
        new ObservableCollection<BlobFeaturesResultData>();
        public ObservableCollection<BlobFeaturesResultData> DgResultOfBlobList
        {
            get { return this.dgResultOfBlobList; }
            set
            {
                dgResultOfBlobList = value;
                DoNotify();

            }
        }
        

    }

    [Serializable]
    public class ParticleFeaturesData
    {
        public ParticleFeaturesData(bool use, string item, int limitDown, int limitUp)
        {
            Use = use;
            Item = item;
            LimitDown = limitDown;
            LimitUp = limitUp;
        }
        public bool Use { get; set; }
        public string Item { get; set; }
        public int  LimitDown { get; set; }
        public int LimitUp { get; set; }


    }
    [Serializable]
    public class BlobFeaturesResultData
    {
        public BlobFeaturesResultData(int id,double x, double y,
            double area)
        {
            ID = id;
            X = x;
            Y = y;
            Area = area;
               
        }
     
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Area { get; set; }

   
    }
}
