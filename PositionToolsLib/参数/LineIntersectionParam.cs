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
    /// 直线交点检测参数
    /// </summary>
    [Serializable]
    public class LineIntersectionParam : BaseParam
    {
        public LineIntersectionParam()
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

        StuLineData  lineData =new StuLineData ();
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


        string inputLine2Name = "";
        /// <summary>
        /// 直线2名称
        /// </summary>
        [Description("直线2名称"), DefaultValue("")]
        public string InputLine2Name
        {
            get => this.inputLine2Name;
            set
            {
                this.inputLine2Name = value;
            }
        }


        StuLineData lineData2 = new StuLineData();
        /// <summary>
        /// 直线2
        /// </summary>
        [Description("直线2"), DefaultValue("")]
        public StuLineData LineData2
        {
            get => this.lineData2;
            set
            {
                this.lineData2 = value;
            }
        }     
     
        //Output parmas value define ---2

        double intersectionRow = 0;
        /// <summary>
        ///交点行坐标
        /// </summary>
        [Description("交点行坐标"), DefaultValue(0)]
        public double IntersectionRow
        {
            get => this.intersectionRow;
            set
            {
                this.intersectionRow = value;
            }
        }

        double intersectionColumn = 0;
        /// <summary>
        ///交点列坐标
        /// </summary>
        [Description("交点列坐标"), DefaultValue(0)]
        public double IntersectionColumn
        {
            get => this.intersectionColumn;
            set
            {
                this.intersectionColumn = value;
            }
        }

       

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
        HObject resultline2 = null;
        /// <summary>
        ///直线2
        /// </summary>
        [Description("直线2"), DefaultValue(null)]
        public HObject Resultline2
        {
            get => this.resultline2;
            set
            {
                this.resultline2 = value;
            }
        }

        //Run status value define----1
        bool lineIntersectionRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool LineIntersectionRunStatus
        {
            get => lineIntersectionRunStatus;
            set
            {
                lineIntersectionRunStatus = value;
            }
        }
        #endregion

    }
}
