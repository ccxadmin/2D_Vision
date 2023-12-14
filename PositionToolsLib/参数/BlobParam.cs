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
    /// Blob检测参数
    /// </summary>
    [Serializable]
   public  class BlobParam : BaseParam
    {
        public BlobParam()
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

        //double pixleRatio = 1;
        ///// <summary>
        ///// 像素比例
        ///// </summary>
        //[Description("像素比例"), DefaultValue(1)]
        //public double PixleRatio
        //{
        //    get => this.pixleRatio;
        //    set
        //    {
        //        this.pixleRatio = value;
        //    }
        //}

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

        int grayMax = 128;
        /// <summary>
        /// 最大灰度值
        /// </summary>
        [Description("最大灰度值"), DefaultValue(128)]
        public int GrayMax
        {
            get => this.grayMax;
            set
            {
                this.grayMax = value;
            }
        }

        List<ParticleFeaturesData> particleFeaturesDataList = new List<ParticleFeaturesData>();
        /// <summary>
        /// 表格数据集合
        /// </summary>                                                       
        [Description("表格数据集合"), DefaultValue(null)]
        public List<ParticleFeaturesData> ParticleFeaturesDataList
        {
            get => this.particleFeaturesDataList;
            set
            {
                this.particleFeaturesDataList = value;
            }
        }

        //Output parmas value define ---2
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

        List<StuBlobFeaturesResult> blobFeaturesResult=new List<StuBlobFeaturesResult> ();
        /// <summary>
        /// Blob粒子特性结果
        /// </summary>
        [Description("Blob粒子特性结果"), DefaultValue(null)]
        public List<StuBlobFeaturesResult> BlobFeaturesResult
        {
            get => this.blobFeaturesResult;
            set
            {
                this.blobFeaturesResult = value;
            }
        }
        //Run status value define----1
        bool blobRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool BlobRunStatus
        {
            get => blobRunStatus;
            set
            {
                blobRunStatus = value;
            }
        }
        #endregion


    }
}
