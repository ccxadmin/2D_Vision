using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Models
{
    internal class LineOffsetModel : NotifyBase
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

        private int numOffsetDistance;
        public int NumOffsetDistance
        {
            get { return this.numOffsetDistance; }
            set
            {
                numOffsetDistance = value;
                DoNotify();
            }
        }

        private string convertUnits= "像素";
        public string ConvertUnits
        {
            get { return this.convertUnits; }
            set
            {
                convertUnits = value;
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

        private double pixelRatio = 1;
        public double PixelRatio
        {
            get { return this.pixelRatio; }
            set
            {
                pixelRatio = value;
                DoNotify();
            }
        }

        private ObservableCollection<LineOffsetData> lineOffsetDataList = new ObservableCollection<LineOffsetData>();
        public ObservableCollection<LineOffsetData> LineOffsetDataList
        {
            get { return this.lineOffsetDataList; }
            set
            {
                lineOffsetDataList = value;
                DoNotify();
            }
        }

    }

    [Serializable]
    public class LineOffsetData
    {
        public LineOffsetData(int id, double x1, double y1, double x2,
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
