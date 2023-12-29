using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using PositionToolsLib.窗体.Models;

namespace PositionToolsLib.参数
{
    /// <summary>
    /// 结果显示参数
    /// </summary>
    [Serializable]
    public class ResultShowParam : BaseParam
    {
        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --2

        string inputXCoorName = "";
        /// <summary>
        /// 输出X坐标名称
        /// </summary>
        [Description("输出X坐标名称"), DefaultValue("")]
        public string InputXCoorName
        {
            get => this.inputXCoorName;
            set
            {
                this.inputXCoorName = value;
            }
        }

        string inputYCoorName = "";
        /// <summary>
        /// 输出Y坐标名称
        /// </summary>
        [Description("输入Y坐标名称"), DefaultValue("")]
        public string InputYCoorName
        {
            get => this.inputYCoorName;
            set
            {
                this.inputYCoorName = value;
            }
        }

        string inputAngleCoorName = "";
        /// <summary>
        /// 输出角度坐标名称
        /// </summary>
        [Description("输入角度坐标名称"), DefaultValue("")]
        public string InputAngleCoorName
        {
            get => this.inputAngleCoorName;
            set
            {
                this.inputAngleCoorName = value;
            }
        }


        List<DgDataOfResultShow> resultShowDataList = new List<DgDataOfResultShow>();
        /// <summary>
        /// 表格数据集合
        /// </summary>                                                       
        [Description("表格数据集合"), DefaultValue(null)]
        public List<DgDataOfResultShow> ResultShowDataList
        {
            get => this.resultShowDataList;
            set
            {
                this.resultShowDataList = value;
            }
        }


        //Output parmas value define --2

        List<StuFlagInfo>  resultInfo = new List<StuFlagInfo>() ;
        /// <summary>
        /// 结果信息
        /// </summary>
        [Description("结果信息"), DefaultValue(null)]
        public List<StuFlagInfo>  ResultInfo
        {
            get => resultInfo;
            set
            {
                resultInfo = value;
            }
        }

        HObject resultRegion = null;
        /// <summary>
        /// 结果区域
        /// </summary>
        [Description("结果区域"), DefaultValue(null)]
        public HObject ResultRegion
        {
            get => resultRegion;
            set
            {
                resultRegion = value;
            }
        }

        StuCoordinateData coordinateData;
        /// <summary>
        /// 结果坐标
        /// </summary>
        [Description("结果坐标"), DefaultValue(null)]
        public StuCoordinateData CoordinateData
        {
            get => coordinateData;
            set
            {
                coordinateData = value;
            }
        }



        //Run status value define----1
        bool resultShowStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool ResultShowStatus
        {
            get => resultShowStatus;
            set
            {
                resultShowStatus = value;
            }
        }
        #endregion
    }
}
