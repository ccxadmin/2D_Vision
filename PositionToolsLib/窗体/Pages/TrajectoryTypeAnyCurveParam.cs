using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Pages
{
    [Serializable]
    public class TrajectoryTypeAnyCurveParam : TrajectoryTypeBaseParam
    {
        [NonSerialized]
        public HObject InspectRegion = new HObject();
        public HObject MaskRegion = new HObject();
        public EumRegionTypeOfGJ RegionType { get; set; } = EumRegionTypeOfGJ.any;
        public byte EdgeThdMin { get; set; } = 20;
        public byte EdgeThdMax { get; set; } = 40;
        public bool IsXldClosed { get; set; } = false;
        public int  XldLengthMin { get; set; } = 5;
        public int XldLengthMax { get; set; } = 99999;
        public int SamplingPointNums { get; set; } = 10;

    }

    /// <summary>
    /// 检测区域类型
    /// </summary>
    public enum EumRegionTypeOfGJ
    {
        /// <summary>
        /// 任意
        /// </summary>
        any,
        /// <summary>
        /// 矩形
        /// </summary>
        rectangle,
        /// <summary>
        /// 圆
        /// </summary>
        cirle

    }
    public enum EumStartP
    {
        上,
        下,
        左,
        右
    }
}
