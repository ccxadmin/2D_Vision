using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Pages
{
    [Serializable]
    public  abstract class TrajectoryTypeBaseTool
    {
        //public abstract TrajectoryTypeLineParam GetParam();
        public TrajectoryTypeBaseParam param { get; set; }
        public abstract TemRunResult Run();

        /// 判断图像或区域是否存在
        /// </summary>
        /// <param name = "obj" > 区域 </ param >
        /// < returns ></ returns >
        static public bool ObjectValided(HObject obj)
        {
            try
            {
                if (obj == null)
                    return false;
                if (!obj.IsInitialized())
                {
                    return false;
                }
                if (obj.CountObj() < 1)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
