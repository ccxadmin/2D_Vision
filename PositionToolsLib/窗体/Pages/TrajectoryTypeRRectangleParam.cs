using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Pages
{
    [Serializable]
    public class TrajectoryTypeRRectangleParam : TrajectoryTypeBaseParam
    {
        public TrajectoryTypeRRectangleParam()
        {
            TrajectoryTypeLineParam.Add(new Pages.TrajectoryTypeLineParam());
            TrajectoryTypeLineParam.Add(new Pages.TrajectoryTypeLineParam());
            TrajectoryTypeLineParam.Add(new Pages.TrajectoryTypeLineParam());
            TrajectoryTypeLineParam.Add(new Pages.TrajectoryTypeLineParam());
        }
        public List<TrajectoryTypeLineParam> TrajectoryTypeLineParam =
             new List<TrajectoryTypeLineParam>();
        public int paramIndex { get; set; } = 0;
        public TrajectoryTypeLineParam GetParamFormIndex()
        {
           return TrajectoryTypeLineParam[paramIndex];
        }
        public TrajectoryTypeLineParam GetParamFormIndex(int ind)
        {
            return TrajectoryTypeLineParam[ind];
        }
        public TrajectoryTypeLineParam SetParamFormIndex(TrajectoryTypeLineParam par,int ind)
        {
            return TrajectoryTypeLineParam[ind]= par;
        }
    }
}
