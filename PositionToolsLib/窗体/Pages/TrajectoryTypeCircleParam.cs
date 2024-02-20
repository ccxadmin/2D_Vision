using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Pages
{
    [Serializable]
    public class TrajectoryTypeCircleParam : TrajectoryTypeBaseParam
    {
        [NonSerialized]
        public HObject InspectXLD = new HObject();
        public byte EdgeContrast { get; set; } = 20;
        public int CaliperNum { get; set; } = 30;
        public int CaliperWidth { get; set; } = 15;
        public int CaliperHeight { get; set; } = 60;
        public EumTransition Transition { get; set; } = EumTransition.all;
        public EumSelect Select { get; set; } = EumSelect.max;      
        public EumDirection Direction { get; set; } = EumDirection.outer;
       
    }
}
