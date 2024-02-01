using FunctionLib.Cam;
using HalconDotNet;
using PositionToolsLib.窗体.Models;
using PositionToolsLib.窗体.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.参数
{ 
    /// <summary>
  /// 轨迹提取工具运行参数
  /// </summary>
    [Serializable]
    public class TrajectoryExtractParam : BaseParam
    {
      

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

        EumTrackType trackType =  EumTrackType.AnyCurve;
        /// <summary>
        /// 轨迹类型
        /// </summary>
        [Description("轨迹类型"), DefaultValue(typeof(EumTrackType), "AnyCurve")]
        public EumTrackType TrackType
        {
            get => this.trackType;
            set
            {
                this.trackType = value;
            }
        }

        Dictionary<string, HObject> trajectoryInspectObjDic 
            = new Dictionary<string, HObject>();
        /// <summary>
        /// 轨迹类型检测区域
        /// </summary>
        [Description("轨迹类型检测区域"), DefaultValue(null)]
        public Dictionary<string, HObject> TrajectoryInspectObjDic
        {
            get => this.trajectoryInspectObjDic;
            set
            {
                this.trajectoryInspectObjDic = value;
            }
        }


        TrajectoryTypeBaseTool trajectoryTool = null ;
        /// <summary>
        /// 轨迹类型基础工具
        /// </summary>
        [Description("轨迹类型基础工具"), DefaultValue(null)]
        public TrajectoryTypeBaseTool TrajectoryTool
        {
            get => this.trajectoryTool;
            set
            {
                this.trajectoryTool = value;
            }
        }
        //HObject resultInspectROI = null;
        ///// <summary>
        ///// 结果检测ROI
        ///// </summary>
        //[Description("结果检测ROI"), DefaultValue(null)]
        //public HObject ResultInspectROI
        //{
        //    get => this.resultInspectROI;
        //    set
        //    {
        //        this.resultInspectROI = value;
        //    }
        //}
        List<DgTrajectoryData> trajectoryDataPoints = new List<DgTrajectoryData>();
        /// <summary>
        /// 轨迹点集合
        /// </summary>
        [Description("轨迹点集合"), DefaultValue(null)]
        public List<DgTrajectoryData> TrajectoryDataPoints
        {
            get => this.trajectoryDataPoints;
            set
            {
                this.trajectoryDataPoints = value;
            }
        }

        //Run status value define----1
        bool trajectoryExtractRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool TrajectoryExtractRunStatus
        {
            get => trajectoryExtractRunStatus;
            set
            {
                trajectoryExtractRunStatus = value;
            }
        }
    }
}
