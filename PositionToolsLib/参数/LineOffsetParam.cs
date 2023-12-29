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
    /// 直线偏移检测参数
    /// </summary>
    [Serializable]
    public class LineOffsetParam : BaseParam
    {
        public LineOffsetParam()
        {

        }

        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --2

        string inputLineName = "";
        /// <summary>
        /// 直线1名称
        /// </summary>
        [Description("直线1名称"), DefaultValue("")]
        public string InputLineName
        {
            get => this.inputLineName;
            set
            {
                this.inputLineName = value;
            }
        }

        StuLineData lineData = new StuLineData();
        /// <summary>
        /// 直线1
        /// </summary>
        [Description("直线1"), DefaultValue("")]
        public StuLineData LineData
        {
            get => this.lineData;
            set
            {
                this.lineData = value;
            }
        }

        int offsetDistance = 0;
        /// <summary>
        ///偏移距离
        /// </summary>
        [Description("偏移距离"), DefaultValue(0)]
        public int OffsetDistance
        {
            get => this.offsetDistance;
            set
            {
                this.offsetDistance = value;
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

        //Output parmas value define ---6
        double row1 = 0;
        /// <summary>
        /// 起点行坐标
        /// </summary>
        [Description("起点行坐标"), DefaultValue("0")]
        public double Row1
        {
            get => this.row1;
            set
            {
                this.row1 = value;
            }
        }

        double col1 = 0;
        /// <summary>
        /// 起点列坐标
        /// </summary>
        [Description("起点列坐标"), DefaultValue("0")]
        public double Col1
        {
            get => this.col1;
            set
            {
                this.col1 = value;
            }
        }

        double row2 = 0;
        /// <summary>
        /// 终点行坐标
        /// </summary>
        [Description("终点行坐标"), DefaultValue("0")]
        public double Row2
        {
            get => this.row2;
            set
            {
                this.row2 = value;
            }
        }

        double col2 = 0;
        /// <summary>
        /// 终点列坐标
        /// </summary>
        [Description("终点列坐标"), DefaultValue("0")]
        public double Col2
        {
            get => this.col2;
            set
            {
                this.col2 = value;
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

        HObject resultline = null;
        /// <summary>
        ///直线1
        /// </summary>
        [Description("直线1"), DefaultValue(null)]
        public HObject Resultline
        {
            get => this.resultline;
            set
            {
                this.resultline = value;
            }
        }

        //Run status value define----1
        bool lineOffsetRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool LineOffsetRunStatus
        {
            get => lineOffsetRunStatus;
            set
            {
                lineOffsetRunStatus = value;
            }
        }
        #endregion

    }
}
