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
    /// 断胶检测参数
    /// </summary>
    [Serializable]
    public class GlueGapParam : BaseParam
    {
        public GlueGapParam()
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

        HObject inspectROI = null;
        /// <summary>
        /// 检测ROI
        /// </summary>
        [Description("检测ROI"), DefaultValue(null)]
        public HObject InspectROI
        {
            get => this.inspectROI;
            set
            {
                this.inspectROI = value;
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

        byte grayMin = 0;
        /// <summary>
        ///最小灰度值
        /// </summary>
        [Description("最小灰度值"), DefaultValue(0)]
        public byte GrayMin
        {
            get => this.grayMin;
            set
            {
                this.grayMin = value;
            }
        }

        byte grayMax = 128;
        /// <summary>
        /// 最大灰度值
        /// </summary>
        [Description("最大灰度值"), DefaultValue(128)]
        public byte GrayMax
        {
            get => this.grayMax;
            set
            {
                this.grayMax = value;
            }
        }

        double areaMin = 0;
        /// <summary>
        /// 最小面积
        /// </summary>
        [Description("最小面积"), DefaultValue(0)]
        public double AreaMin
        {
            get => this.areaMin;
            set
            {
                this.areaMin = value;
            }
        }

        double areaMax = 999999;
        /// <summary>
        /// 最大面积
        /// </summary>
        [Description("最大面积"), DefaultValue(999999)]
        public double AreaMax
        {
            get => this.areaMax;
            set
            {
                this.areaMax = value;
            }
        }
        //Output parmas value define ---2

        List<StuRegionBuf> regionBufList = new List<StuRegionBuf>();//绘制区域缓存     
        /// <summary>
        /// 绘制区域缓存
        /// </summary>
        [Description("绘制区域缓存"), DefaultValue(null)]
        public List<StuRegionBuf> RegionBufList
        {
            get => this.regionBufList;
            set
            {
                this.regionBufList = value;
            }
        }


        EumGenRegionWay genRegionWay = EumGenRegionWay.auto;//检测区域生成方式

        /// <summary>
        /// 检测区域生成方式
        /// </summary>
        [Description("检测区域生成方式"), DefaultValue(typeof(EumGenRegionWay), "auto")]
        public EumGenRegionWay GenRegionWay
        {
            get => this.genRegionWay;
            set
            {
                this.genRegionWay = value;
            }
        }


        int resultNum = 0;
        /// <summary>
        /// 结果数量
        /// </summary>
        [Description("结果数量"), DefaultValue(0)]
        public int ResultNum
        {
            get => this.resultNum;
            set
            {
                this.resultNum = value;
            }
        }


        HObject resultBaseRegion = null;
        /// <summary>
        /// 结果基准区域
        /// </summary>
        [Description("结果基准区域"), DefaultValue(null)]
        public HObject ResultBaseRegion
        {
            get => this.resultBaseRegion;
            set
            {
                this.resultBaseRegion = value;
            }
        }


        HObject resultInspectROI = null;
        /// <summary>
        /// 结果检测ROI
        /// </summary>
        [Description("结果检测ROI"), DefaultValue(null)]
        public HObject ResultInspectROI
        {
            get => this.resultInspectROI;
            set
            {
                this.resultInspectROI = value;
            }
        }

        //Run status value define----1
        bool glueLossRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool GlueLossRunStatus
        {
            get => glueLossRunStatus;
            set
            {
                glueLossRunStatus = value;
            }
        }
        #endregion
    }
}
