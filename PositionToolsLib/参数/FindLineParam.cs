using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.参数
{
    /// <summary>
    /// 查找直线检测参数
    /// </summary>
    [Serializable]
    public class FindLineParam : BaseParam
    {
        public FindLineParam()
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

        HObject inspectXLD = null;
        /// <summary>
        /// 检测XLD
        /// </summary>
        [Description("检测XLD"), DefaultValue(null)]
        public HObject InspectXLD
        {
            get => this.inspectXLD;
            set
            {
                this.inspectXLD = value;
            }
        }

        bool usePosiCorrect = false;
        /// <summary>
        /// 是否位置补正
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

        int caliperNum = 30;
        /// <summary>
        ///卡尺数量
        /// </summary>
        [Description("卡尺数量"), DefaultValue(30)]
        public int CaliperNum
        {
            get => this.caliperNum;
            set
            {
                this.caliperNum = value;
            }
        }

        int caliperWidth = 15;
        /// <summary>
        ///卡尺宽度
        /// </summary>
        [Description("卡尺宽度"), DefaultValue(15)]
        public int CaliperWidth
        {
            get => this.caliperWidth;
            set
            {
                this.caliperWidth = value;
            }
        }

        int caliperHeight = 60;
        /// <summary>
        ///卡尺高度
        /// </summary>
        [Description("卡尺高度"), DefaultValue(60)]
        public int CaliperHeight
        {
            get => this.caliperHeight;
            set
            {
                this.caliperHeight = value;
            }
        }

        byte edgeThd = 20;
        /// <summary>
        ///边缘阈值
        /// </summary>
        [Description("边缘阈值"), DefaultValue(20)]
        public byte EdgeThd
        {
            get => this.edgeThd;
            set
            {
                this.edgeThd = value;
            }
        }

        EumTransition transition = EumTransition.all;
        /// <summary>
        ///边缘极性
        /// </summary>
        [Description("边缘极性"), DefaultValue(typeof(EumTransition),"all")]
        public EumTransition Transition
        {
            get => this.transition;
            set
            {
                this.transition = value;
            }
        }

        EumSelect select = EumSelect.max;
        /// <summary>
        ///边缘选择
        /// </summary>
        [Description("边缘选择"), DefaultValue(typeof(EumSelect), "max")]
        public EumSelect Select
        {
            get => this.select;
            set
            {
                this.select = value;
            }
        }

        //Output parmas value define ---2
      
        double startPointRow = 0;
        /// <summary>
        ///起点行坐标
        /// </summary>
        [Description("起点行坐标"), DefaultValue(0)]
        public double StartPointRow
        {
            get => this.startPointRow;
            set
            {
                this.startPointRow = value;
            }
        }

        double startPointColumn = 0;
        /// <summary>
        ///起点列坐标
        /// </summary>
        [Description("起点列坐标"), DefaultValue(0)]
        public double StartPointColumn
        {
            get => this.startPointColumn;
            set
            {
                this.startPointColumn = value;
            }
        }

        double endPointRow = 0;
        /// <summary>
        ///终点行坐标
        /// </summary>
        [Description("终点行坐标"), DefaultValue(0)]
        public double EndPointRow
        {
            get => this.endPointRow;
            set
            {
                this.endPointRow = value;
            }
        }

        double endPointColumn = 0;
        /// <summary>
        ///终点列坐标
        /// </summary>
        [Description("终点列坐标"), DefaultValue(0)]
        public double EndPointColumn
        {
            get => this.endPointColumn;
            set
            {
                this.endPointColumn = value;
            }
        }

        double lineAngle = 0;
        /// <summary>
        ///直线角度
        /// </summary>
        [Description("直线角度"), DefaultValue(0)]
        public double LineAngle
        {
            get => this.lineAngle;
            set
            {
                this.lineAngle = value;
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
        bool findLineRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool FindLineRunStatus
        {
            get => findLineRunStatus;
            set
            {
                findLineRunStatus = value;
            }
        }
        #endregion
    }
}
