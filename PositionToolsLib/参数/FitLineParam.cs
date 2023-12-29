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
    /// 拟合直线检测参数
    /// </summary>
    [Serializable]
    public class FitLineParam : BaseParam
    {
        public FitLineParam()
        {

        }

        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --2

        string startXName = "";
        /// <summary>
        /// 起点行坐标名称
        /// </summary>
        [Description("起点X坐标名称"), DefaultValue("")]
        public string StartXName
        {
            get => this.startXName;
            set
            {
                this.startXName = value;
            }
        }

        double x1 = 0;
        /// <summary>
        /// 起点X坐标
        /// </summary>
        [Description("起点行坐标"), DefaultValue("0")]
        public double X1
        {
            get => this.x1;
            set
            {
                this.x1 = value;
            }
        }


        string startYName = "";
        /// <summary>
        /// 起点Y坐标名称
        /// </summary>
        [Description("起点Y坐标名称"), DefaultValue("")]
        public string StartYName
        {
            get => this.startYName;
            set
            {
                this.startYName = value;
            }
        }

        double y1 = 0;
        /// <summary>
        /// 起点Y坐标
        /// </summary>
        [Description("起点Y坐标"), DefaultValue("0")]
        public double Y1
        {
            get => this.y1;
            set
            {
                this.y1 = value;
            }
        }

        string endXName = "";
        /// <summary>
        /// 终点X坐标名称
        /// </summary>
        [Description("终点X坐标名称"), DefaultValue("")]
        public string EndXName
        {
            get => this.endXName;
            set
            {
                this.endXName = value;
            }
        }

        double x2 = 0;
        /// <summary>
        /// 终点X坐标
        /// </summary>
        [Description("终点X坐标"), DefaultValue("0")]
        public double X2
        {
            get => this.x2;
            set
            {
                this.x2 = value;
            }
        }


        string endYName = "";
        /// <summary>
        /// 终点Y标名称
        /// </summary>
        [Description("终点Y坐标名称"), DefaultValue("")]
        public string EndYName
        {
            get => this.endYName;
            set
            {
                this.endYName = value;
            }
        }

        double y2 = 0;
        /// <summary>
        /// 终点Y坐标
        /// </summary>
        [Description("终点Y坐标"), DefaultValue("0")]
        public double Y2
        {
            get => this.y2;
            set
            {
                this.y2 = value;
            }
        }


        //Output parmas value define ---1
    
        HObject resultline1 = null;
        /// <summary>
        ///直线1
        /// </summary>
        [Description("直线1"), DefaultValue(null)]
        public HObject Resultline1
        {
            get => this.resultline1;
            set
            {
                this.resultline1 = value;
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

        //Run status value define----1
        bool fitLineRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool FitLineRunStatus
        {
            get => fitLineRunStatus;
            set
            {
                fitLineRunStatus = value;
            }
        }
        #endregion
    }
}
