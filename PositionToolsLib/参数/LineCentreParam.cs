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
    public class LineCentreParam : BaseParam
    {
        public LineCentreParam()
        {

        }

        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --2

        string startRowName = "";
        /// <summary>
        /// 起点行坐标名称
        /// </summary>
        [Description("起点行坐标名称"), DefaultValue("")]
        public string StartRowName
        {
            get => this.startRowName;
            set
            {
                this.startRowName = value;
            }
        }

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


        string startColName = "";
        /// <summary>
        /// 起点列坐标名称
        /// </summary>
        [Description("起点列坐标名称"), DefaultValue("")]
        public string StartColName
        {
            get => this.startColName;
            set
            {
                this.startColName = value;
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

        string endRowName = "";
        /// <summary>
        /// 终点行坐标名称
        /// </summary>
        [Description("终点行坐标名称"), DefaultValue("")]
        public string EndRowName
        {
            get => this.endRowName;
            set
            {
                this.endRowName = value;
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


        string endColName = "";
        /// <summary>
        /// 终点列坐标名称
        /// </summary>
        [Description("终点列坐标名称"), DefaultValue("")]
        public string EndColName
        {
            get => this.endColName;
            set
            {
                this.endColName = value;
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


        //Output parmas value define ---1

        double centreRow = 0;
        /// <summary>
        ///中心行坐标
        /// </summary>
        [Description("中心行坐标"), DefaultValue("0")]
        public double CentreRow
        {
            get => this.centreRow;
            set
            {
                this.centreRow = value;
            }
        }


        double centreCol = 0;
        /// <summary>
        ///中心列坐标
        /// </summary>
        [Description("中心列坐标"), DefaultValue("0")]
        public double CentreCol
        {
            get => this.centreCol;
            set
            {
                this.centreCol = value;
            }
        }

        //Run status value define----1
        bool lineCentreRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool LineCentreRunStatus
        {
            get => lineCentreRunStatus;
            set
            {
                lineCentreRunStatus = value;
            }
        }
        #endregion
    }
}
