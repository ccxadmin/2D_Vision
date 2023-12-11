using OSLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlueDetectionLib.参数;
using HalconDotNet;
using System.Runtime.Serialization;

namespace GlueDetectionLib.工具
{
    /// <summary>
    /// 图像腐蚀工具
    /// </summary>
    [Serializable]
    public  class ErosionTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public ErosionTool()
        {
            toolParam = new ErosionParam();
            toolName = "图像腐蚀" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("图像腐蚀");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("图像腐蚀", ""));
            if (number > inum)
                inum = number;
            //toolName = "图像腐蚀" + number;
            inum++;
        }
        /// <summary>
        /// 图像腐蚀工具运行
        /// </summary>
        /// <returns></returns>
        override public RunResult Run()
        {
            RunResult result = new RunResult();
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (sw == null) sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
            DataManage dm = GetManage();
            try
            {
                (toolParam as ErosionParam).InputImg = dm.imageBufDic[(toolParam as ErosionParam).InputImageName];
                if (!ObjectValided((toolParam as ErosionParam).InputImg))
                {
                    (toolParam as ErosionParam).ErosionRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                HOperatorSet.GrayErosionRect((toolParam as ErosionParam).InputImg, out HObject imageMax,
                    (toolParam as ErosionParam).MaskWidth, (toolParam as ErosionParam).MaskHeight);
                (toolParam as ErosionParam).OutputImg = imageMax.Clone();//输出图像
                imageMax.Dispose();
                result.runFlag = true;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;
                (toolParam as ErosionParam).ErosionRunStatus = true;
                //添加图像到缓存集合
                if (!dm.imageBufDic.ContainsKey(toolName))
                    dm.imageBufDic.Add(toolName, (toolParam as ErosionParam).OutputImg);
                else
                    dm.imageBufDic[toolName] = (toolParam as ErosionParam).OutputImg;
            }
            catch (Exception er)
            {
                log.Error(funName, er.Message);
                result.runFlag = false;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as ErosionParam).ErosionRunStatus = false;
                result.errInfo = er.Message;
                sw.Stop();
                result.runTime = sw.ElapsedMilliseconds;
                return result;
            }
            sw.Stop();
            result.runTime = sw.ElapsedMilliseconds;
            return result;
        }
    }
}
