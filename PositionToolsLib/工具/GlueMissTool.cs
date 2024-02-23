using OSLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PositionToolsLib.参数;
using HalconDotNet;
using System.Runtime.Serialization;

namespace PositionToolsLib.工具
{
    /// <summary>
    /// 漏胶检测工具
    /// </summary>
    [Serializable]
   public class GlueMissTool : BaseTool, IDisposable
    {
        public  int autoRegionTypeSelectIndex = 0;
        public int selectPolarityIndex = 0;
        public int morphProcessSelectIndex = 0;
        public int numRadius = 1;
        public int convertUnitsSelectIndex = 0;
        public int manulRegionTypeSelectIndex = 0;
        public static int inum = 0;//工具编号

        public GlueMissTool()
        {
            toolParam = new GlueMissParam();
            toolName = "漏胶检测" + inum;
            inum++;
         
        }

        public void Dispose()
        {

        }

        public GlueInfo glueInfo = new GlueInfo();//胶水信息
    
        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("漏胶检测");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("漏胶检测", ""));
            if (number > inum)
                inum = number;
            //toolName = "漏胶检测" + number;
            inum++;
        }
        /// <summary>
        ///漏胶检测工具运行
        /// </summary>
        /// <returns></returns>
        override public RunResult Run()
        {
            DataManage dm = GetManage();
            if (!dm.enumerableTooDic.Contains(toolName))
                dm.enumerableTooDic.Add(toolName);
            if (!dm.aoiTooDic.Contains(toolName))
                dm.aoiTooDic.Add(toolName);
            RunResult result = new RunResult();
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (sw == null) sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
            if (glueInfo == null)
                glueInfo = new GlueInfo();
            try
            {
                (toolParam as GlueMissParam).InputImg = dm.imageBufDic[(toolParam as GlueMissParam).InputImageName];
                if (!ObjectValided((toolParam as GlueMissParam).InputImg))
                {
                    (toolParam as GlueMissParam).GlueMissRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                if (!ObjectValided((toolParam as GlueMissParam).InspectROI))
                {
                    (toolParam as GlueMissParam).GlueMissRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"检测区域无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                //仿射变换矩阵
                HObject inspectROI = (toolParam as GlueMissParam).InspectROI;
                if ((toolParam as GlueMissParam).UsePosiCorrect)
                {
                    HTuple matrix2D = dm.matrixBufDic[(toolParam as GlueMissParam).MatrixName];
                    if (matrix2D != null )
                        HOperatorSet.AffineTransRegion((toolParam as GlueMissParam).InspectROI,
                          out  inspectROI, matrix2D, "nearest_neighbor");
                    else
                    {
                        if (!dm.resultFlagDic.ContainsKey(toolName))
                            dm.resultFlagDic.Add(toolName, false);
                        else
                            dm.resultFlagDic[toolName] = false;
                        (toolParam as GlueMissParam).GlueMissRunStatus = false;
                        result.runFlag = false;
                        result.errInfo = toolName + "检测区域位置补正异常";
                        return result;
                    }
                }
                (toolParam as GlueMissParam).ResultInspectROI = inspectROI.Clone();
                //开始检测
                HOperatorSet.ReduceDomain((toolParam as GlueMissParam).InputImg,
                    inspectROI, out HObject imageReduced);
                HOperatorSet.Threshold(imageReduced,out HObject region,
                    (toolParam as GlueMissParam).GrayMin, (toolParam as GlueMissParam).GrayMax);
                HOperatorSet.Connection(region,out HObject connectedRegions);
                //HOperatorSet.FillUp(connectedRegions, out HObject regionFillUp);

                HOperatorSet.SelectShapeStd(connectedRegions,out HObject selectRegion, "max_area",70);
                HOperatorSet.CountObj(selectRegion, out HTuple number);
                int length = number.I;
                double pixleRatio = (toolParam as GlueMissParam).PixleRatio;
                int count = 0;
                HObject emptyRegionBuf = null;
                HOperatorSet.GenEmptyObj(out emptyRegionBuf);
                glueInfo.toolName = toolName;
                glueInfo.areaList.Clear();
                glueInfo.coorditions.Clear();
                glueInfo.rect_r1.Clear();
                glueInfo.rect_c1.Clear();
                glueInfo.rect_r2.Clear();
                glueInfo.rect_c2.Clear();
                for (int i = 0;i < length;i++)
                {
                    HOperatorSet.SelectObj(selectRegion, out HObject objectSelected,i+1);
                    HOperatorSet.AreaCenter(objectSelected, out HTuple area, out HTuple row, out HTuple column);
                    double areaValue = area.D * pixleRatio * pixleRatio;
                    glueInfo.areaList.Add(areaValue);
                    glueInfo.coorditions.Add(new CoorditionDat(row.D, column.D));

                    HOperatorSet.SmallestRectangle1(objectSelected,out HTuple row1,
                         out HTuple column1, out HTuple row2, out HTuple column2);
                    glueInfo.rect_r1.Add(row1.D);
                    glueInfo.rect_c1. Add(column1.D);
                    glueInfo.rect_r2. Add( row2.D);
                    glueInfo.rect_c2 .Add( column2.D);
                    HOperatorSet.ConcatObj(emptyRegionBuf, objectSelected, out emptyRegionBuf);
                    if (areaValue>= (toolParam as GlueMissParam).AreaMin &&
                        areaValue<= (toolParam as GlueMissParam).AreaMax)
                    {
                        count++;
                        //HOperatorSet.ConcatObj(emptyRegionBuf, objectSelected, out emptyRegionBuf);
                   
                    }
                }

                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, emptyRegionBuf.Clone());
                else
                    dm.resultBufDic[toolName] = emptyRegionBuf.Clone();

                string info = toolName+"漏胶检测完成";
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;

                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, count>0);
                else
                    dm.resultFlagDic[toolName] = count>0;

                //+输入图像
                HOperatorSet.ConcatObj((toolParam as GlueMissParam).InputImg, emptyRegionBuf, out HObject objectsConcat2);
                (toolParam as GlueMissParam).OutputImg = objectsConcat2;
                (toolParam as GlueMissParam).ResultNum = count;
                result.runFlag = true;
                (toolParam as GlueMissParam).GlueMissRunStatus = true;
                emptyRegionBuf.Dispose();

            }
            catch (Exception er)
            {
                string info = string.Format(toolName+"漏胶检测异常:{0}", er.Message);
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
                (toolParam as GlueMissParam).GlueMissRunStatus = false;
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
