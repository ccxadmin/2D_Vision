﻿using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Models
{
    public class DistanceLLModel : NotifyBase
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
        ///选择直线2名称
        /// </summary>
        private string selectLine2Name;
        public string SelectLine2Name
        {
            get { return this.selectLine2Name; }
            set
            {
                selectLine2Name = value;
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
        /// <summary>
        ///选择直线2索引编号
        /// </summary>
        private int selectLine2Index;
        public int SelectLine2Index
        {
            get { return this.selectLine2Index; }
            set
            {
                selectLine2Index = value;
                DoNotify();
            }
        }

        private ObservableCollection<DistanceLLResultData> dgResultOfDistanceLLList =
new ObservableCollection<DistanceLLResultData>();
        public ObservableCollection<DistanceLLResultData> DgResultOfDistanceLLList
        {
            get { return this.dgResultOfDistanceLLList; }
            set
            {
                dgResultOfDistanceLLList = value;
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
    public class DistanceLLResultData
    {
        public DistanceLLResultData(int id, double distance)

        {
            ID = id;
            Distance = Math.Round(distance, 3);
        }
        public int ID { get; set; }

        public double Distance { get; set; }



    }
}
