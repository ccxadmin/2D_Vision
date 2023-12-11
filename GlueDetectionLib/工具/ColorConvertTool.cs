using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GlueDetectionLib.参数;
using GlueDetectionLib.窗体.Models;
using HalconDotNet;
using OSLog;


namespace GlueDetectionLib.工具
{
    /// <summary>
    /// 图像颜色转换工具
    /// </summary>
    [Serializable]
    public  class ColorConvertTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public ColorConvertTool()
        {
            toolParam = new ColorConvertParam();
            toolName = "图像颜色转换" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("图像颜色转换");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("图像颜色转换", ""));
            if (number > inum)
                inum = number;
            //toolName = "图像颜色转换" + number;
            inum++;
        }
        /// <summary>
        ///图像颜色转换工具运行
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
                (toolParam as ColorConvertParam).InputImg = dm.imageBufDic[(toolParam as ColorConvertParam).InputImageName];
                if (!ObjectValided((toolParam as ColorConvertParam).InputImg))
                {
                    (toolParam as ColorConvertParam).ColorConvertRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }

                HOperatorSet.CountChannels((toolParam as ColorConvertParam).InputImg,out HTuple channels);
                if(channels!=3)
                {
                    (toolParam as ColorConvertParam).OutputImg = (toolParam as ColorConvertParam).InputImg.Clone();//输出图像            
                    result.runFlag = true;
                    (toolParam as ColorConvertParam).ColorConvertRunStatus = true;
                }
                else
                {
                    HOperatorSet.Rgb1ToGray((toolParam as ColorConvertParam).InputImg,out HObject grayImage);
                    HOperatorSet.Decompose3((toolParam as ColorConvertParam).InputImg,
                              out HObject redImg,out HObject greenImg,out HObject blueImg);
                    HOperatorSet.TransFromRgb(redImg, greenImg, blueImg,
                         out HObject ImageResultH, out HObject ImageResultS, out HObject ImageResultV, "hsv");

                    EumImgFormat imgFormat = (toolParam as ColorConvertParam).ImgFormat;
                    switch (imgFormat)
                    {
                        case EumImgFormat.gray:
                            (toolParam as ColorConvertParam).OutputImg = grayImage.Clone();//输出图像
                            break;
                        case EumImgFormat.red:
                            (toolParam as ColorConvertParam).OutputImg = redImg.Clone();//输出图像
                            break;
                        case EumImgFormat.green:
                            (toolParam as ColorConvertParam).OutputImg = greenImg.Clone();//输出图像
                            break;
                        case EumImgFormat.blue:
                            (toolParam as ColorConvertParam).OutputImg = blueImg.Clone();//输出图像
                            break;
                        case EumImgFormat.h:
                            (toolParam as ColorConvertParam).OutputImg = ImageResultH.Clone();//输出图像
                            break;
                        case EumImgFormat.s:
                            (toolParam as ColorConvertParam).OutputImg = ImageResultS.Clone();//输出图像
                            break;
                        case EumImgFormat.v:
                            (toolParam as ColorConvertParam).OutputImg = ImageResultV.Clone();//输出图像
                            break;
                        default:
                            (toolParam as ColorConvertParam).OutputImg = grayImage.Clone();//输出图像
                            break;
                    }
                    grayImage.Dispose();
                    redImg.Dispose();
                    greenImg.Dispose();
                    blueImg.Dispose();
                    ImageResultH.Dispose();
                    ImageResultS.Dispose();
                    ImageResultV.Dispose();
                    result.runFlag = true;
                  
                    
                }
                //添加图像到缓存集合
                if (!dm.imageBufDic.ContainsKey(toolName))
                    dm.imageBufDic.Add(toolName, (toolParam as ColorConvertParam).OutputImg);
                else
                    dm.imageBufDic[toolName] = (toolParam as ColorConvertParam).OutputImg;

                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;
                (toolParam as ColorConvertParam).ColorConvertRunStatus = true;
            }
            catch (Exception er)
            {
                log.Error(funName, er.Message);
                result.runFlag = false;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as ColorConvertParam).ColorConvertRunStatus = false;
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
