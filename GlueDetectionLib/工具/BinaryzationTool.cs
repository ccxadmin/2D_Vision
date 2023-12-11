using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlueDetectionLib.参数;
using OSLog;
using HalconDotNet;
using System.Runtime.Serialization;

namespace GlueDetectionLib.工具
{
    /// <summary>
    /// 图像二值化工具
    /// </summary>
    [Serializable]
    public class BinaryzationTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public BinaryzationTool()
        {
            toolParam = new BinaryzationParam();
            toolName = "图像二值化" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("图像二值化");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("图像二值化", ""));
            if (number > inum)
                inum = number;
            //toolName = "图像二值化" + number;
            inum++;
        }
        /// <summary>
        /// 图像二值化工具运行
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

                (toolParam as BinaryzationParam).InputImg = dm.imageBufDic[(toolParam as BinaryzationParam).InputImageName];
                if (!ObjectValided((toolParam as BinaryzationParam).InputImg))
                {
                    (toolParam as BinaryzationParam).BinaryzationRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
              
                HOperatorSet.Threshold((toolParam as BinaryzationParam).InputImg, out HObject region,
                    (toolParam as BinaryzationParam).GrayMin, (toolParam as BinaryzationParam).GrayMax);
                HOperatorSet.GetImageSize((toolParam as BinaryzationParam).InputImg, out HTuple width,out HTuple height);
                HOperatorSet.GenImageConst(out HObject image1,"byte", width, height);
                HOperatorSet.PaintRegion(region, image1,out HObject imageResult,255,"fill");
                //图像是否反转
                if ((toolParam as BinaryzationParam).IsInvert)
                {
                    HOperatorSet.InvertImage(imageResult, out HObject imageInvert);
                    (toolParam as BinaryzationParam).OutputImg = imageInvert.Clone();//输出图像
                    imageInvert.Dispose();

                }
                else
                    (toolParam as BinaryzationParam).OutputImg = imageResult.Clone();//输出图像
               
            
                image1.Dispose();
                imageResult.Dispose();
                result.runFlag = true;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;
                (toolParam as BinaryzationParam).BinaryzationRunStatus = true;
                //添加图像到缓存集合
                if (!dm.imageBufDic.ContainsKey(toolName))
                    dm.imageBufDic.Add(toolName, (toolParam as BinaryzationParam).OutputImg);
                else
                    dm.imageBufDic[toolName] = (toolParam as BinaryzationParam).OutputImg;
            }
            catch (Exception er)
            {
                log.Error(funName, er.Message);
                result.runFlag = false;    
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as BinaryzationParam).BinaryzationRunStatus = false;
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
