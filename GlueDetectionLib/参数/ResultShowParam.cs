using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;


namespace GlueDetectionLib.参数
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
        List<ResultShowData> resultShowDataList = new List<ResultShowData>();
        /// <summary>
        /// 表格数据集合
        /// </summary>                                                       
        [Description("表格数据集合"), DefaultValue(null)]
        public List<ResultShowData> ResultShowDataList
        {
            get => this.resultShowDataList;
            set
            {
                this.resultShowDataList = value;
            }
        }



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
