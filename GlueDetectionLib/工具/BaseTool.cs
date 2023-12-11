using GlueDetectionLib.参数;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSLog;
using HalconDotNet;
using System.Diagnostics;

namespace GlueDetectionLib.工具
{
    /// <summary>
    /// 胶水检测工具基类
    /// </summary>
    [Serializable]
    abstract  public class BaseTool
    {
        protected BaseParam toolParam = null;//工具运行检测参数

        protected string toolName = string.Empty;//工具名称

        public string remark = "";//备注
        public delegate DataManage GetManageHandle();
        [NonSerialized]
        public GetManageHandle OnGetManageHandle;

        public DataManage GetManage()
        {
            if(OnGetManageHandle==null)
                return  new DataManage();
            else
              return OnGetManageHandle.Invoke();
        }
        /// <summary>
        /// 获取工具运行检测参数
        /// </summary>
        /// <returns></returns>
        public BaseParam GetParam()
        {
            return toolParam;
        }
        /// <summary>
        ///  设置工具运行检测参数
        /// </summary>
        /// <param name="param"></param>
        public void  SetParam(BaseParam param)
        {
            toolParam = param;
        }
        /// <summary>
        /// 获取工具名称
        /// </summary>
        /// <returns></returns>
        public string GetToolName()
        {
            return toolName;
        }
        /// <summary>
        /// 检测工具运行
        /// </summary>
        /// <returns></returns>
        abstract public RunResult Run();

        /// 判断图像或区域是否存在
        /// </summary>
        /// <param name = "obj" > 区域 </ param >
        /// < returns ></ returns >
       static   public  bool ObjectValided(HObject obj)
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

        //计时器
        [NonSerialized]
        public Stopwatch sw = new Stopwatch();
    }

    /// <summary>
    /// 运行结果基类
    /// </summary>
    public class RunResult
    {
        /// <summary>
        /// 运行时间
        /// </summary>
        public long runTime ;
        /// <summary>
        /// 运行状态
        /// </summary>
        public bool runFlag;
  
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errInfo;
    
    }

    
}
