using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Models
{
    public class AngleConvertModel : NotifyBase
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

        private int selectConvertWayIndex;
        public int SelectConvertWayIndex
        {
            get { return this.selectConvertWayIndex; }
            set
            {
                selectConvertWayIndex = value;
                DoNotify();
            }
        }


        private string calibFilePath;
        public string CalibFilePath
        {
            get { return this.calibFilePath; }
            set
            {
                calibFilePath = value;
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

        private string selectEndXName;
        public string SelectEndXName
        {
            get { return this.selectEndXName; }
            set
            {
                selectEndXName = value;
                DoNotify();
            }
        }
        private int selectEndXIndex;
        public int SelectEndXIndex
        {
            get { return this.selectEndXIndex; }
            set
            {
                selectEndXIndex = value;
                DoNotify();
            }
        }

        private string selectEndYName;
        public string SelectEndYName
        {
            get { return this.selectEndYName; }
            set
            {
                selectEndYName = value;
                DoNotify();
            }
        }
        private int selectEndYIndex;
        public int SelectEndYIndex
        {
            get { return this.selectEndYIndex; }
            set
            {
                selectEndYIndex = value;
                DoNotify();
            }
        }

        private ObservableCollection<AngleConvertData> dgResultOfAngleConvertList =
  new ObservableCollection<AngleConvertData>();
        public ObservableCollection<AngleConvertData> DgResultOfAngleConvertList
        {
            get { return this.dgResultOfAngleConvertList; }
            set
            {
                dgResultOfAngleConvertList = value;
                DoNotify();

            }
        }
    }

    [Serializable]
    public class AngleConvertData
    {
        public AngleConvertData(int id, double angle)

        {
            ID = id;
            Angle = angle;
        }
        public int ID { get; set; }

        public double Angle { get; set; }



    }
}
