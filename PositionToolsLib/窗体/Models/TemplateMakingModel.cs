using ControlShareResources.Common;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Models
{
    public class TemplateMakingModel : NotifyBase
    {

        private string cobxPanTypeSelectName = "圆";
        public string CobxPanTypeSelectName
        {
            get { return this.cobxPanTypeSelectName; }
            set
            {
                cobxPanTypeSelectName = value;
                DoNotify();
            }
        }
        private int cobxPanTypeSelectIndex;
        public int CobxPanTypeSelectIndex
        {
            get { return this.cobxPanTypeSelectIndex; }
            set
            {
                cobxPanTypeSelectIndex = value;
                DoNotify();
            }
        }

        private int panSize = 10;
        public int PanSize
        {
            get { return this.panSize; }
            set
            {
                panSize = value;
                DoNotify();
            }
        }

        private string panValue = "10";
        public string PanValue
        {
            get { return this.panValue; }
            set
            {
                panValue = value;
                DoNotify();
            }
        }

        /// <summary>
        /// 区域生成方式
        /// </summary>
        private EumworkType workType = EumworkType.擦拭;
        public EumworkType WorkType
        {
            get { return this.workType; }
            set
            {
                workType = value;
                DoNotify();
            }
        }
    }
    public enum EumworkType
    {

        擦拭,
        清除
        //,
        //放大,
        //缩小,
        //自适应,
        //平移
    }
    public enum EumPanType
    {
        圆,
        矩形
    }
    /// <summary>
    /// 掩膜绘制类型
    /// </summary>
    public enum EumMakeType
    {
        /// <summary>
        /// 模板
        /// </summary>
        Model,
        /// <summary>
        /// 轨迹
        /// </summary>
        Trajectory
    }
    /// <summary>
    /// 返回数据
    /// </summary>
    public class ReturnData : EventArgs
    {
        public ReturnData(HObject hObject, EumMakeType makeType)
        {
            region = hObject.Clone();
            type = makeType;
        }
        /// <summary>
        /// 区域
        /// </summary>
        public HObject region { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public EumMakeType type { get; set; }

    }
}
