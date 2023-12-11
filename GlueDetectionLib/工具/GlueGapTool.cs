using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GlueDetectionLib.参数;
using HalconDotNet;
using OSLog;

namespace GlueDetectionLib.工具
{
    /// <summary>
    /// 断胶检测工具
    /// </summary>
    [Serializable]
    public class GlueGapTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public GlueGapTool()
        {
            toolParam = new GlueGapParam();
            toolName = "断胶检测" + inum;
            inum++;
          
        }

        public void Dispose()
        {

        }
        public GlueInfo glueInfo = new GlueInfo();//胶水信息
        //public List<double> areaList = new List<double>();
        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("断胶检测");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("断胶检测", ""));
            if (number > inum)
                inum = number;
            //toolName = "断胶检测" + number;
            inum++;
        }
        /// <summary>
        ///断胶检测工具运行
        /// </summary>
        /// <returns></returns>
        override public RunResult Run()
        {
            DataManage dm = GetManage();
            if (!dm.enumerableTooDic.Contains(toolName))
                dm.enumerableTooDic.Add(toolName);
            RunResult result = new RunResult();
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (sw == null) sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
            if (glueInfo == null)
                glueInfo = new GlueInfo();
            try
            {
                (toolParam as GlueGapParam).InputImg = dm.imageBufDic[(toolParam as GlueGapParam).InputImageName];
                if (!ObjectValided((toolParam as GlueGapParam).InputImg))
                {
                    (toolParam as GlueGapParam).GlueLossRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                if (!ObjectValided((toolParam as GlueGapParam).InspectROI))
                {
                    (toolParam as GlueGapParam).GlueLossRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"检测区域无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                if (!ObjectValided((toolParam as GlueGapParam).ResultBaseRegion))
                {
                    (toolParam as GlueGapParam).GlueLossRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"基准区域无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                //仿射变换矩阵
                HObject inspectROI = (toolParam as GlueGapParam).InspectROI;
                if ((toolParam as GlueGapParam).UsePosiCorrect)
                {
                    HTuple matrix2D = dm.matrixBufDic[(toolParam as GlueGapParam).MatrixName];
                    if (matrix2D != null)
                        HOperatorSet.AffineTransRegion((toolParam as GlueGapParam).InspectROI,
                          out inspectROI, matrix2D, "nearest_neighbor");
                    else
                    {
                        if (!dm.resultFlagDic.ContainsKey(toolName))
                            dm.resultFlagDic.Add(toolName, false);
                        else
                            dm.resultFlagDic[toolName] = false;
                        (toolParam as GlueGapParam).GlueLossRunStatus = false;
                        result.runFlag = false;
                        result.errInfo = toolName + "检测区域位置补正异常";
                        return result;
                    }
                }
                (toolParam as GlueGapParam).ResultInspectROI = inspectROI.Clone();
                //开始检测
                HOperatorSet.ReduceDomain((toolParam as GlueGapParam).InputImg,
                    inspectROI, out HObject imageReduced);
                HOperatorSet.Threshold(imageReduced, out HObject region,
                    (toolParam as GlueGapParam).GrayMin, (toolParam as GlueGapParam).GrayMax);

                //区域差分
                HObject baseRegion = (toolParam as GlueGapParam).ResultBaseRegion;
                if ((toolParam as GlueGapParam).UsePosiCorrect)
                {
                    HTuple matrix2D = dm.matrixBufDic[(toolParam as GlueGapParam).MatrixName];
                    if (matrix2D != null & matrix2D.Length > 0)
                        HOperatorSet.AffineTransRegion((toolParam as GlueGapParam).ResultBaseRegion,
                          out baseRegion, matrix2D, "nearest_neighbor");
                }
                HOperatorSet.Difference(baseRegion, region,out HObject regionDifference);

                //粒子分析
                HOperatorSet.Connection(regionDifference, out HObject connectedRegions);
                HOperatorSet.CountObj(connectedRegions, out HTuple number);
                int length = number;
                double pixleRatio = (toolParam as GlueGapParam).PixleRatio;
                int count = 0;
                HObject emptyRegionBuf = null;
                HOperatorSet.GenEmptyObj(out emptyRegionBuf);
                glueInfo.toolName = toolName;
                glueInfo.areaList.Clear();
                glueInfo.coorditions.Clear();
                for (int i = 0; i < length; i++)
                {
                    HOperatorSet.SelectObj(connectedRegions, out HObject objectSelected, i + 1);
                    HOperatorSet.AreaCenter(objectSelected, out HTuple area, out HTuple row, out HTuple column);
                    double areaValue = area * pixleRatio * pixleRatio;
                    glueInfo.areaList.Add(areaValue);
                    glueInfo.coorditions.Add(new CoorditionDat(row.D, column.D));
                    HOperatorSet.ConcatObj(emptyRegionBuf, objectSelected, out emptyRegionBuf);
                    if (areaValue >= (toolParam as GlueGapParam).AreaMin &&
                        areaValue <= (toolParam as GlueGapParam).AreaMax)
                    {
                        count++;
                        //HOperatorSet.ConcatObj(emptyRegionBuf, objectSelected, out emptyRegionBuf);
                     
                    }
                }
                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, emptyRegionBuf);
                else
                    dm.resultBufDic[toolName] = emptyRegionBuf.Clone();

                string info = toolName+"断胶检测完成";
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;
            
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, count<=0);
                else
                    dm.resultFlagDic[toolName] = count <= 0;

                //+输入图像
                HOperatorSet.ConcatObj((toolParam as GlueGapParam).InputImg, emptyRegionBuf, out HObject objectsConcat2);
                (toolParam as GlueGapParam).OutputImg = objectsConcat2;
             
                (toolParam as GlueGapParam).ResultNum = count;
                result.runFlag = true;
                (toolParam as GlueGapParam).GlueLossRunStatus = true;
                emptyRegionBuf.Dispose();
            }
            catch (Exception er)
            {

                string info = string.Format(toolName+"断胶检测异常:{0}", er.Message);
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
                (toolParam as GlueGapParam).GlueLossRunStatus = false;
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
