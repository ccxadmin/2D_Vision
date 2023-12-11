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
    /// 图像开运算工具
    /// </summary>
    [Serializable]
   public  class OpeningTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public OpeningTool()
        {
            toolParam = new OpeningParam();
            toolName = "图像开运算" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("图像开运算");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("图像开运算", ""));
            if (number > inum)
                inum = number;
            //toolName = "图像开运算" + number;
            inum++;
        }
        /// <summary>
        /// 图像开运算工具运行
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
                (toolParam as OpeningParam).InputImg = dm.imageBufDic[(toolParam as OpeningParam).InputImageName];
                if (!ObjectValided((toolParam as OpeningParam).InputImg))
                {
                    (toolParam as OpeningParam).OpeningRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+ "输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                HOperatorSet.GrayOpeningRect((toolParam as OpeningParam).InputImg, out HObject imageMax,
                    (toolParam as OpeningParam).MaskWidth, (toolParam as OpeningParam).MaskHeight);
                (toolParam as OpeningParam).OutputImg = imageMax.Clone();//输出图像
                imageMax.Dispose();
                result.runFlag = true;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;

                (toolParam as OpeningParam).OpeningRunStatus = true;
                //添加图像到缓存集合
                if (!dm.imageBufDic.ContainsKey(toolName))
                    dm.imageBufDic.Add(toolName, (toolParam as OpeningParam).OutputImg);
                else
                    dm.imageBufDic[toolName] = (toolParam as OpeningParam).OutputImg;
            }
            catch (Exception er)
            {
                log.Error(funName, er.Message);
                result.runFlag = false;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as OpeningParam).OpeningRunStatus = false;
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
