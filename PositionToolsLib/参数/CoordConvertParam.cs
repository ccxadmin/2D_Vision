using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.参数
{
    /// <summary>
    /// 坐标换算
    /// </summary>
    [Serializable]
    public class CoordConvertParam : BaseParam
    {
        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --3


        string coordXName = "";
        /// <summary>
        ///X坐标名称
        /// </summary>
        [Description("X坐标名称"), DefaultValue("")]
        public string CoordXName
        {
            get => this.coordXName;
            set
            {
                this.coordXName = value;
            }
        }

        double x = 0;
        /// <summary>
        /// X坐标
        /// </summary>
        [Description("X坐标"), DefaultValue("0")]
        public double X
        {
            get => this.x;
            set
            {
                this.x = value;
            }
        }

        string coordYName = "";
        /// <summary>
        ///Y坐标名称
        /// </summary>
        [Description("Y坐标名称"), DefaultValue("")]
        public string CoordYName
        {
            get => this.coordYName;
            set
            {
                this.coordYName = value;
            }
        }

        double y = 0;
        /// <summary>
        /// Y坐标
        /// </summary>
        [Description("Y坐标"), DefaultValue("0")]
        public double Y
        {
            get => this.y;
            set
            {
                this.y = value;
            }
        }

        /// <summary>
        /// 转换类型
        /// </summary>
        private EumConvertWay convertWay = EumConvertWay.ToPhysical;
        public EumConvertWay ConvertWay
        {
            get => this.convertWay;
            set
            {
                this.convertWay = value;
            }
        }
        //Input parmas value define --2
        double convertedX = 0;
        /// <summary>
        /// 转换后X坐标
        /// </summary>
        [Description("转换后X坐标"), DefaultValue(0)]
        public double ConvertedX
        {
            get => this.convertedX;
            set
            {
                this.convertedX = value;
            }
        }


        double convertedY = 0;
        /// <summary>
        /// 转换后Y坐标
        /// </summary>
        [Description("转换后Y坐标"), DefaultValue(0)]
        public double ConvertedY
        {
            get => this.convertedY;
            set
            {
                this.convertedY = value;
            }
        }

        //Run status value define----1
        bool coordConvertRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool CoordConvertRunStatus
        {
            get => coordConvertRunStatus;
            set
            {
                coordConvertRunStatus = value;
            }
        }
        #endregion
    }

    public enum EumConvertWay
    {
        ToPixel,
        ToPhysical

    }

}
