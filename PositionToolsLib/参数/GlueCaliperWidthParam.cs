using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;


namespace PositionToolsLib.参数
{
    /// <summary>
    /// 胶宽检测参数
    /// </summary>
    [Serializable]
   public class GlueCaliperWidthParam : BaseParam
    {
        public GlueCaliperWidthParam()
        {

        }
        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --2

        string matrixName = "";
        /// <summary>
        /// 位置补正
        /// </summary>
        [Description("位置补正"), DefaultValue("")]
        public string MatrixName
        {
            get => this.matrixName;
            set
            {
                this.matrixName = value;
            }
        }

        List<StuRegionBuf> regionBufList = new List<StuRegionBuf>();//绘制卡尺缓存     
        /// <summary>
        /// 绘制卡尺缓存
        /// </summary>
        [Description("绘制卡尺缓存"), DefaultValue(null)]
        public List<StuRegionBuf> RegionBufList
        {
            get => this.regionBufList;
            set
            {
                this.regionBufList = value;
            }
        }


        bool usePosiCorrect = false;
        /// <summary>
        /// 位置补正
        /// </summary>
        [Description("位置补正"), DefaultValue(false)]
        public bool UsePosiCorrect
        {
            get => this.usePosiCorrect;
            set
            {
                this.usePosiCorrect = value;
            }
        }

        double pixleRatio = 1;
        /// <summary>
        /// 像素比例
        /// </summary>
        [Description("像素比例"), DefaultValue(1)]
        public double PixleRatio
        {
            get => this.pixleRatio;
            set
            {
                this.pixleRatio = value;
            }
        }
 
       // int caliperWidth = 5;
        /// <summary>
        ///卡尺宽度
        /// </summary>
        //[Description("卡尺宽度"), DefaultValue(5)]
        //public int CaliperWidth
        //{
        //    get => this.caliperWidth;
        //    set
        //    {
        //        this.caliperWidth = value;
        //    }
        //}

        int caliperHeight = 50;
        /// <summary>
        ///卡尺高度
        /// </summary>
        [Description("卡尺高度"), DefaultValue(50)]
        public int CaliperHeight
        {
            get => this.caliperHeight;
            set
            {
                this.caliperHeight = value;
            }
        }

        byte caliperEdgeThd = 30;
        /// <summary>
        /// 卡尺边缘阈值
        /// </summary>
        [Description("卡尺边缘阈值"), DefaultValue(30)]
        public byte CaliperEdgeThd
        {
            get => this.caliperEdgeThd;
            set
            {
                this.caliperEdgeThd = value;
            }
        }

        double distanceMin = 0;
        /// <summary>
        /// 边距下限
        /// </summary>
        [Description("边距下限"), DefaultValue(0)]
        public double DistanceMin
        {
            get => this.distanceMin;
            set
            {
                this.distanceMin = value;
            }
        }

        double distanceMax = 999999;
        /// <summary>
        /// 边距上限
        /// </summary>
        [Description("边距上限"), DefaultValue(999999)]
        public double DistanceMax
        {
            get => this.distanceMax;
            set
            {
                this.distanceMax = value;
            }
        }

        //Output parmas value define ---2

        List<double> distanceList = new List<double>();//边距缓存     
        /// <summary>
        /// 边距缓存
        /// </summary>
        [Description("边距缓存"), DefaultValue(null)]
        public List<double> DistanceList
        {
            get => this.distanceList;
            set
            {
                this.distanceList = value;
            }
        }
 
     
        HObject resultInspectRegions= null;
        /// <summary>
        /// 结果检测卡尺区域集合
        /// </summary>
        [Description("结果检测卡尺区域集合"), DefaultValue(null)]
        public HObject ResultInspectRegions
        {
            get => this.resultInspectRegions;
            set
            {
                this.resultInspectRegions = value;
            }
        }

      

        //Run status value define----1
        bool glueWidthRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool GlueWidthRunStatus
        {
            get => glueWidthRunStatus;
            set
            {
                glueWidthRunStatus = value;
            }
        }
        #endregion
    }
}
