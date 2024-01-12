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
    /// 偏位检测工具
    /// </summary>
    [Serializable]
   public class GlueOffsetTool : BaseTool, IDisposable
    {
        public int autoRegionTypeSelectIndex = 0;
        public int selectPolarityIndex = 0;
        public int morphProcessSelectIndex = 0;
        public int numRadius = 1;
        public int convertUnitsSelectIndex = 0;
        public int manulRegionTypeSelectIndex = 0;
        public static int inum = 0;//工具编号

        public GlueOffsetTool()
        {
            toolParam = new GlueOffsetParam();
            toolName = "偏位检测" + inum;
            inum++;
           
        }

        public void Dispose()
        {

        }
        public GlueInfo glueInfo = new GlueInfo();//胶水信息
      
        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("偏位检测");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("偏位检测", ""));
            if (number > inum)
                inum = number;
            //toolName = "偏位检测" + number;
            inum++;
        }
        /// <summary>
        ///偏位检测工具运行
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
                (toolParam as GlueOffsetParam).InputImg = dm.imageBufDic[(toolParam as GlueOffsetParam).InputImageName];
                if (!ObjectValided((toolParam as GlueOffsetParam).InputImg))
                {
                    (toolParam as GlueOffsetParam).GlueOffsetRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                if (!ObjectValided((toolParam as GlueOffsetParam).InspectROI))
                {
                    (toolParam as GlueOffsetParam).GlueOffsetRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"检测区域无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                //仿射变换矩阵
                HObject inspectROI = (toolParam as GlueOffsetParam).InspectROI;
                if ((toolParam as GlueOffsetParam).UsePosiCorrect)
                {
                    HTuple matrix2D = dm.matrixBufDic[(toolParam as GlueOffsetParam).MatrixName];
                    if (matrix2D != null)
                        HOperatorSet.AffineTransRegion((toolParam as GlueOffsetParam).InspectROI,
                          out inspectROI, matrix2D, "nearest_neighbor");
                    else
                    {
                        if (!dm.resultFlagDic.ContainsKey(toolName))
                            dm.resultFlagDic.Add(toolName, false);
                        else
                            dm.resultFlagDic[toolName] = false;
                        (toolParam as GlueOffsetParam).GlueOffsetRunStatus = false;
                        result.runFlag = false;
                        result.errInfo = toolName + "检测区域位置补正异常";
                        return result;
                    }
                }
                (toolParam as GlueOffsetParam).ResultInspectROI = inspectROI.Clone();
                //开始检测
                HOperatorSet.ReduceDomain((toolParam as GlueOffsetParam).InputImg,
                    inspectROI, out HObject imageReduced);
                HOperatorSet.Threshold(imageReduced, out HObject region,
                    (toolParam as GlueOffsetParam).GrayMin, (toolParam as GlueOffsetParam).GrayMax);
                HOperatorSet.Connection(region, out HObject connectedRegions);
                HOperatorSet.CountObj(connectedRegions, out HTuple number);
                int length = number;
                double pixleRatio = (toolParam as GlueOffsetParam).PixleRatio;
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
                    if (areaValue >= (toolParam as GlueOffsetParam).AreaMax
                       /* && areaValue <= (toolParam as GlueOffsetParam).AreaMax*/)
                    {
                        count++;
                        //HOperatorSet.ConcatObj(emptyRegionBuf, objectSelected, out emptyRegionBuf);
                       
                    }
                }

                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, emptyRegionBuf);
                else
                    dm.resultBufDic[toolName] = emptyRegionBuf.Clone();

                string info = toolName+"胶水偏位检测完成";
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;
          
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, count<=0);
                else
                    dm.resultFlagDic[toolName] = count<=0;

                //+输入图像
                HOperatorSet.ConcatObj((toolParam as GlueOffsetParam).InputImg, emptyRegionBuf, out HObject objectsConcat2);
                (toolParam as GlueOffsetParam).OutputImg = objectsConcat2;
                (toolParam as GlueOffsetParam).ResultNum = count;
                result.runFlag = true;
                (toolParam as GlueOffsetParam).GlueOffsetRunStatus = true;
                emptyRegionBuf.Dispose();
            }
            catch (Exception er)
            {
                string info = string.Format(toolName+"胶水偏位检测异常,{0}", er.Message);
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
                (toolParam as GlueOffsetParam).GlueOffsetRunStatus = false;
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
