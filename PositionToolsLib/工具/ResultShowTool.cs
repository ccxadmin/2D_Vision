﻿using PositionToolsLib.参数;
using HalconDotNet;
using OSLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using PositionToolsLib.窗体.Models;
using PositionToolsLib.窗体.Pages;

namespace PositionToolsLib.工具
{
    /// <summary>
    /// 图像结果显示工具
    /// </summary>
    [Serializable]
   public class ResultShowTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public ResultShowTool()
        {
            toolParam = new ResultShowParam();
            toolName = "图像结果显示" + inum;
            inum++;
        }

        public void Dispose()
        {

        }
        //是否显示检测区域
        public bool isShowInspectRegion = true;
        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("图像结果显示");
        //文本信息起始坐标
        public int inforCoorX = 10;
        public int inforCoorY = 300;

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("图像结果显示", ""));
            if (number > inum)
                inum = number;
            //toolName = "图像结果显示" + number;
            inum++;
        }
        /// <summary>
        ///图像结果显示工具运行
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
             
                if (BaseTool.ObjectValided((toolParam as ResultShowParam).ResultRegion))
                      (toolParam as ResultShowParam).ResultRegion.Dispose();
                (toolParam as ResultShowParam).ResultInfo.Clear();

                (toolParam as ResultShowParam).InputImg = dm.imageBufDic[(toolParam as ResultShowParam).InputImageName];
                if (!ObjectValided((toolParam as ResultShowParam).InputImg))
                {
                    (toolParam as ResultShowParam).ResultShowStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                (toolParam as ResultShowParam).OutputImg = (toolParam as ResultShowParam).InputImg;//输出图像 

                EumOutputType outputType = (toolParam as ResultShowParam).OutputType;
                if (outputType == EumOutputType.Location)
                {                 
                    double X = 0, Y = 0, angle = 0;
                    if (dm.resultFlagDic[(toolParam as ResultShowParam).InputXCoorName.Replace("起点", "").Replace("终点", "")])
                    {
                        StuCoordinateData xDat = dm.PositionDataDic[(toolParam as ResultShowParam).InputXCoorName];
                        X = Math.Round(xDat.x, 3);
                    }
                    if (dm.resultFlagDic[(toolParam as ResultShowParam).InputYCoorName.Replace("起点", "").Replace("终点", "")])
                    {
                        StuCoordinateData yDat = dm.PositionDataDic[(toolParam as ResultShowParam).InputYCoorName];
                        Y = Math.Round(yDat.y, 3);
                    }
                    if (dm.resultFlagDic[(toolParam as ResultShowParam).InputAngleCoorName.Replace("起点", "").Replace("终点", "")])
                    {
                        StuCoordinateData angleDat = dm.PositionDataDic[(toolParam as ResultShowParam).InputAngleCoorName];
                        angle = Math.Round(angleDat.angle, 3);
                    }

                    (toolParam as ResultShowParam).CoordinateData = new StuCoordinateData(X, Y, angle);
                }
                else if (outputType == EumOutputType.Trajectory)
                {
                    List<OutputTypeOfTrajectory> names = (toolParam as ResultShowParam).TrajectoryNameList;
                    List<DgTrajectoryData> TrajectoryDatas = new List<DgTrajectoryData> ();
                    foreach (var s in names)
                    {
                        if (s.IsUse)
                            if (dm.TrajectoryDataDic.ContainsKey(s.ToolName))
                                TrajectoryDatas.AddRange(dm.TrajectoryDataDic[s.ToolName]);
                    }
                   (toolParam as ResultShowParam).TrajectoryDataList = TrajectoryDatas;
                }
                else if (outputType == EumOutputType.Size)
                {
                    List<OutputTypeOfSize> names = (toolParam as ResultShowParam).SizeNameList;
                    List<double> SizeDatas = new List<double>();
                    foreach (var s in names)
                    {
                        if (s.IsUse)
                            if (dm.SizeDataDic.ContainsKey(s.ToolName))
                                SizeDatas.Add(dm.SizeDataDic[s.ToolName]);
                    }
                   (toolParam as ResultShowParam).Distances = SizeDatas;
                }
                else if(outputType == EumOutputType.AOI)
                {
                    List<OutputTypeOfSize> names = (toolParam as ResultShowParam).SizeNameList;
                    bool flag = true;
                    foreach (var s in names)
                    {
                        if (s.IsUse)
                            if (dm.SizeDataDic.ContainsKey(s.ToolName))
                                flag &= dm.AoiDataDic[s.ToolName];
                     
                    }
                    (toolParam as ResultShowParam).AoiResultFlag = flag;
                }
                HObject emptyObj = null;
                HOperatorSet.GenEmptyObj(out emptyObj);
                List<StuFlagInfo> flagInfoList = new List<StuFlagInfo>();    
                foreach (var s in (toolParam as ResultShowParam).ResultShowDataList)
                {
                    if (s.Use)
                    {
                        //if (DataManage.resultFlagDic.ContainsKey(s.toolName))
                        if (dm.resultFlagDic[s.ToolName])//结果标志为true
                            if (dm.resultBufDic.ContainsKey(s.ToolName))
                                if (BaseTool.ObjectValided(dm.resultBufDic[s.ToolName]))
                                    HOperatorSet.ConcatObj(emptyObj, dm.resultBufDic[s.ToolName], out emptyObj);
                        if (dm.resultInfoDic.ContainsKey(s.ToolName))
                            flagInfoList.Add(new StuFlagInfo(dm.resultInfoDic[s.ToolName],
                                      dm.resultFlagDic[s.ToolName]));
                    }
                       
                }
                 (toolParam as ResultShowParam).ResultRegion = emptyObj.Clone();
                (toolParam as ResultShowParam).ResultInfo = flagInfoList;           
                result.runFlag = true;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;
                (toolParam as ResultShowParam).ResultShowStatus = true;
                emptyObj.Dispose();
            }
            catch (Exception er)
            {
                log.Error(funName, er.Message);
                result.runFlag = false;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as ResultShowParam).ResultShowStatus = false;
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
