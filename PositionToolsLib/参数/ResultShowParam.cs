using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using PositionToolsLib.窗体.Models;
using PositionToolsLib.窗体.Pages;

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

        EumOutputType outputType = EumOutputType.Location;
        /// <summary>
        /// 输出类型
        /// </summary>
        [Description("输出类型"), DefaultValue(typeof(EumOutputType), "Location")]
        public EumOutputType OutputType
        {
            get => this.outputType;
            set
            {
                this.outputType = value;
            }
        }
        List<OutputTypeOfTrajectory> trajectoryNameList 
                             = new List<OutputTypeOfTrajectory>();
        /// <summary>
        /// 轨迹工具名称集合
        /// </summary>
        [Description("轨迹工具名称集合"), DefaultValue(null)]
        public List<OutputTypeOfTrajectory> TrajectoryNameList
        {
            get => this.trajectoryNameList;
            set
            {
                this.trajectoryNameList = value;
            }
        }
        List<OutputTypeOfSize> sizeNameList
                            = new List<OutputTypeOfSize>();
        /// <summary>
        /// 尺寸工具名称集合
        /// </summary>
        [Description("尺寸工具名称集合"), DefaultValue(null)]
        public List<OutputTypeOfSize> SizeNameList
        {
            get => this.sizeNameList;
            set
            {
                this.sizeNameList = value;
            }
        }

        List<DataOfResultShow> resultShowDataList = new List<DataOfResultShow>();
        /// <summary>
        /// 表格数据集合
        /// </summary>                                                       
        [Description("表格数据集合"), DefaultValue(null)]
        public List<DataOfResultShow> ResultShowDataList
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
        /// 定位坐标：输出类型Location
        /// </summary>
        [Description("定位坐标"), DefaultValue(null)]
        public StuCoordinateData CoordinateData
        {
            get => coordinateData;
            set
            {
                coordinateData = value;
            }
        }

        List<DgTrajectoryData> trajectoryDataList=new List<DgTrajectoryData>();
        /// <summary>
        /// 轨迹坐标：输出类型Trajectory
        /// </summary>
        [Description("轨迹坐标"), DefaultValue(null)]
        public List<DgTrajectoryData> TrajectoryDataList
        {
            get => trajectoryDataList;
            set
            {
                trajectoryDataList = value;
            }
        }

        private List<double> distances = new List<double>();
        /// <summary>
        /// 尺寸：输出类型Size
        /// </summary>
        [Description("尺寸"), DefaultValue(null)]
        public List<double> Distances
        {
            get => distances;
            set
            {
                distances = value;
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
