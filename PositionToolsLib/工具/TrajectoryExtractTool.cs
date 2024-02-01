using HalconDotNet;
using OSLog;
using PositionToolsLib.参数;
using PositionToolsLib.窗体.Models;
using PositionToolsLib.窗体.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.工具
{

    /// <summary>
    ///轨迹提取工具
    /// </summary>
    [Serializable]
    public class TrajectoryExtractTool : BaseTool, IDisposable
    {

        public static int inum = 0;//工具编号

        public TrajectoryExtractTool()
        {
            toolParam = new TrajectoryExtractParam();
            toolName = "轨迹提取" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

     
        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("轨迹提取");

        public override RunResult Run()
        {
            RunResult result = new RunResult();
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (sw == null) sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
            DataManage dm = GetManage();
            if (!dm.enumerableTooDic.Contains(toolName))
                dm.enumerableTooDic.Add(toolName);
            if (!dm.trajectoryTooDic.Contains(toolName))
                dm.trajectoryTooDic.Add(toolName);
            
            try
            {
                if (!dm.TrajectoryDataDic.ContainsKey(toolName))
                    dm.TrajectoryDataDic.Add(toolName, null);
                else
                    dm.TrajectoryDataDic[toolName] = null;

                (toolParam as TrajectoryExtractParam).InputImg = dm.imageBufDic[(toolParam as TrajectoryExtractParam).InputImageName];
                if (!ObjectValided((toolParam as TrajectoryExtractParam).InputImg))
                {
                    (toolParam as TrajectoryExtractParam).TrajectoryExtractRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入图像无效";

                    return result;
                }
                /*----加入图像格式判断防止多通道----*/
                #region---图像格式判断---
                HOperatorSet.CountChannels((toolParam as TrajectoryExtractParam).InputImg, out HTuple channels);
                HObject grayImage = null;
                if (channels[0].I > 1)
                    HOperatorSet.Rgb1ToGray((toolParam as TrajectoryExtractParam).InputImg, out grayImage);
                else
                    grayImage = (toolParam as TrajectoryExtractParam).InputImg.Clone();
                #endregion
                //检测工具
                TrajectoryTypeBaseTool TypeTool = (toolParam as TrajectoryExtractParam).TrajectoryTool;
                //输入图像       
                TypeTool.param.InputImage = grayImage;

                EumTrackType trackType = (toolParam as TrajectoryExtractParam).TrackType;

                Dictionary<string, HObject> trajectoryInspectObjDic =
                          (toolParam as TrajectoryExtractParam).TrajectoryInspectObjDic;
                //HObject xld = new HObject();
                HOperatorSet.GenEmptyObj(out  HObject xld);

                switch (trackType)
                {
                    case EumTrackType.Line:
                        //检测区域                      
                        if (!trajectoryInspectObjDic.ContainsKey("Line:直线1"))
                        {
                            (toolParam as TrajectoryExtractParam).TrajectoryExtractRunStatus = false;
                            result.runFlag = false;
                            result.errInfo = toolName + "检测区域无效";
                            if (!dm.resultFlagDic.ContainsKey(toolName))
                                dm.resultFlagDic.Add(toolName, false);
                            else
                                dm.resultFlagDic[toolName] = false;
                            return result;
                        }
                        xld = trajectoryInspectObjDic["Line:直线1"];
                        break;
                    case EumTrackType.RRectangle:
                        //检测区域
                        for (int i = 1; i < 5; i ++)
                        {
                            if (!trajectoryInspectObjDic.ContainsKey("RRectangle:直线"+ i))
                            {
                                (toolParam as TrajectoryExtractParam).TrajectoryExtractRunStatus = false;
                                result.runFlag = false;
                                result.errInfo = toolName + "检测区域无效";
                                if (!dm.resultFlagDic.ContainsKey(toolName))
                                    dm.resultFlagDic.Add(toolName, false);
                                else
                                    dm.resultFlagDic[toolName] = false;
                                return result;
                            }
                            HOperatorSet.ConcatObj(xld, trajectoryInspectObjDic["RRectangle:直线" + i],
                                out xld);                      
                        }
                       
                        break;
                }
                HObject newXld = xld;
                //仿射变换矩阵            
                if ((toolParam as TrajectoryExtractParam).UsePosiCorrect)
                {
                    HTuple matrix2D = dm.matrixBufDic[(toolParam as TrajectoryExtractParam).MatrixName];
                    if (matrix2D != null)
                        HOperatorSet.AffineTransContourXld(xld,
                          out newXld, matrix2D);
                    else
                    {
                        if (!dm.resultFlagDic.ContainsKey(toolName))
                            dm.resultFlagDic.Add(toolName, false);
                        else
                            dm.resultFlagDic[toolName] = false;
                        (toolParam as TrajectoryExtractParam).TrajectoryExtractRunStatus = false;
                        result.runFlag = false;
                        result.errInfo = toolName + "检测区域位置补正异常";
                        return result;
                    }
                }
                if (trackType == EumTrackType.Line)
                    (TypeTool.param as TrajectoryTypeLineParam).InspectXLD = newXld.Clone();
                else if (trackType == EumTrackType.RRectangle)
                {
                    //HOperatorSet.CountObj(xld,out HTuple num);
                    for (int i = 1; i < 5; i++)
                    {
                        HOperatorSet.SelectObj(xld, out HObject objectSelect, i);
                        (TypeTool.param as TrajectoryTypeRRectangleParam).GetParamFormIndex(i-1).InspectXLD
                            = objectSelect.Clone();
                        objectSelect.Dispose();
                    }
                }
                //子页面工具运行
                TemRunResult temrlt = TypeTool.Run();
                (toolParam as TrajectoryExtractParam).TrajectoryDataPoints =
                      temrlt.trajectoryDataPoints;
                //轨迹结果缓存
                if (!dm.TrajectoryDataDic.ContainsKey(toolName))
                    dm.TrajectoryDataDic.Add(toolName, temrlt.trajectoryDataPoints);
                else
                    dm.TrajectoryDataDic[toolName] = temrlt.trajectoryDataPoints;

                //测试信息
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, temrlt.info);
                else
                    dm.resultInfoDic[toolName] = temrlt.info;
                //测试结果
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, temrlt.runFlag);
                else
                    dm.resultFlagDic[toolName] = temrlt.runFlag;

                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, temrlt.resultContour.Clone());
                else
                    dm.resultBufDic[toolName] = temrlt.resultContour.Clone();

                result.runFlag = true;
                (toolParam as TrajectoryExtractParam).TrajectoryExtractRunStatus = true;
                grayImage.Dispose();
                //+输入图像
                HOperatorSet.ConcatObj((toolParam as TrajectoryExtractParam).InputImg, 
                    temrlt.resultContour, out HObject objectsConcat2);
                (toolParam as TrajectoryExtractParam).OutputImg = objectsConcat2;
             
            }
            catch (Exception er)
            {
                string info = string.Format(toolName + "检测异常:{0}", er.Message);
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;

                log.Error(funName, er.Message);
                result.runFlag = false;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as TrajectoryExtractParam).TrajectoryExtractRunStatus = false;
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
