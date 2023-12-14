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
    /// 查找圆检测参数
    /// </summary>
    [Serializable]
    public class FindCircleParam : BaseParam
    {

        public FindCircleParam()
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


        EumDirection direction = EumDirection.outer;
        /// <summary>
        /// 圆查找方向
        /// </summary>
        [Description("圆查找方向"), DefaultValue(typeof(EumDirection), "outer")]
        public EumDirection Direction
        {
            get => this.direction;
            set
            {
                this.direction = value;
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
        [Description("边缘极性"), DefaultValue(typeof(EumTransition), "all")]
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

        double row = 0;
        /// <summary>
        ///圆心行坐标
        /// </summary>
        [Description("圆心行坐标"), DefaultValue(0)]
        public double Row
        {
            get => this.row;
            set
            {
                this.row = value;
            }
        }

        double column = 0;
        /// <summary>
        ///圆心列坐标
        /// </summary>
        [Description("圆心列坐标"), DefaultValue(0)]
        public double Column
        {
            get => this.column;
            set
            {
                this.column = value;
            }
        }
      
        double radius = 0;
        /// <summary>
        ///半径
        /// </summary>
        [Description("直线角度"), DefaultValue(0)]
        public double Radius
        {
            get => this.radius;
            set
            {
                this.radius = value;
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
        bool findCircleRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool FindCircleRunStatus
        {
            get => findCircleRunStatus;
            set
            {
                findCircleRunStatus = value;
            }
        }
        #endregion

    }
}
