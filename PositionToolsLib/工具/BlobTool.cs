using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PositionToolsLib.参数;
using OSLog;
using HalconDotNet;
using System.Runtime.Serialization;

namespace PositionToolsLib.工具
{
    /// <summary>
    /// Blob检测工具
    /// </summary>
    [Serializable]
    public class BlobTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public BlobTool()
        {
            toolParam = new BlobParam();
            toolName = "Blob中心" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("Blob中心");


        /// <summary>
        /// Blob检测工具运行
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

            try
            {
                if (!dm.PositionDataDic.ContainsKey(toolName))
                    dm.PositionDataDic.Add(toolName, new StuCoordinateData(0, 0, 0));
                else
                    dm.PositionDataDic[toolName] = new StuCoordinateData(0, 0, 0);

                (toolParam as BlobParam).InputImg = dm.imageBufDic[(toolParam as BlobParam).InputImageName];
                if (!ObjectValided((toolParam as BlobParam).InputImg))
                {
                    (toolParam as BlobParam).BlobRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                /*----加入图像格式判断防止多通道----*/
                #region---图像格式判断---
                HOperatorSet.CountChannels((toolParam as BlobParam).InputImg, out HTuple channels);
                HObject grayImage = null;
                if (channels[0].I > 1)
                    HOperatorSet.Rgb1ToGray((toolParam as BlobParam).InputImg, out grayImage);
                else
                    grayImage = (toolParam as BlobParam).InputImg.Clone();
                #endregion

                if (!ObjectValided((toolParam as BlobParam).InspectROI))
                {
                    (toolParam as BlobParam).BlobRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"检测区域无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                //仿射变换矩阵
                HObject inspectROI = (toolParam as BlobParam).InspectROI;
                if ((toolParam as BlobParam).UsePosiCorrect)
                {
                    HTuple matrix2D = dm.matrixBufDic[(toolParam as BlobParam).MatrixName];
                    if (matrix2D != null)
                        HOperatorSet.AffineTransRegion((toolParam as BlobParam).InspectROI,
                          out inspectROI, matrix2D, "nearest_neighbor");
                    else
                    {
                        if (!dm.resultFlagDic.ContainsKey(toolName))
                            dm.resultFlagDic.Add(toolName, false);
                        else
                            dm.resultFlagDic[toolName] = false;
                        (toolParam as BlobParam).BlobRunStatus = false;
                        result.runFlag = false;
                        result.errInfo = toolName + "检测区域位置补正异常";
                        return result;
                    }
                }
                (toolParam as BlobParam).ResultInspectROI = inspectROI.Clone();

                //开始检测
                HOperatorSet.ReduceDomain(grayImage,
                  inspectROI, out HObject imageReduced);
                HOperatorSet.Threshold(imageReduced, out HObject region,
                    (toolParam as BlobParam).GrayMin, (toolParam as BlobParam).GrayMax);
                HOperatorSet.Connection(region, out HObject connectedRegions);
                HObject emptyRegionBuf = null;
                HOperatorSet.GenEmptyObj(out emptyRegionBuf);
                emptyRegionBuf.Dispose();
                HOperatorSet.CopyObj(connectedRegions, out emptyRegionBuf, 1, -1);
                //粒子筛选
                foreach (var s in (toolParam as BlobParam).ParticleFeaturesDataList)
                {
                    if (!s.Use) continue;
                    HOperatorSet.SelectShape(emptyRegionBuf, out HObject selectRegions,
                           s.Item,"and", s.LimitDown, s.LimitUp);
                    HOperatorSet.CopyObj(selectRegions,out emptyRegionBuf,1,-1);
                    selectRegions.Dispose();

                }
                //粒子数量
                HOperatorSet.CountObj(emptyRegionBuf, out HTuple number);
                int length = number.I;
                //double pixleRatio = GetPixelRatio();//暂不使用像素转换关系
                double pixleRatio = 1.0;
                (toolParam as BlobParam).BlobFeaturesResult.Clear();

                //粒子筛选
                for (int i=1;i <= length; i++)
                {
                    HOperatorSet.SelectObj(emptyRegionBuf,out HObject objectSelected,i);
                    StuBlobFeaturesResult blobFeaturesResult = new StuBlobFeaturesResult();
                    //面积
                    //if ((toolParam as BlobParam).ParticleFeaturesDataList.Exists(t=>t.features.Equals
                    //   (Enum.GetName(typeof(EumParticleFeatures), EumParticleFeatures.area))))
                    {
                        HOperatorSet.AreaCenter(objectSelected,out HTuple area,out HTuple row,out HTuple column);
                        double areaValue = area.D * pixleRatio * pixleRatio;
                        double rowValue = row.D ;
                        double columnValue = column.D ;
                        blobFeaturesResult.area = areaValue;
                        blobFeaturesResult.row = rowValue;
                        blobFeaturesResult.column = columnValue;
                    }
                    #region------暂不使用-------------
                    ////宽度
                    //if ((toolParam as BlobParam).ParticleFeaturesDataList.Exists(t => t.features.Equals
                    //   (Enum.GetName(typeof(EumParticleFeatures), EumParticleFeatures.width)) &&
                    //   t.isUse))
                    //{
                    //    HOperatorSet.SmallestRectangle2(objectSelected,out HTuple row,out HTuple column,
                    //        out HTuple phi, out HTuple length1, out HTuple length2);

                    //    double widthValue = length1.D*2;
                    //    blobFeaturesResult.width = widthValue;
                    //}
                    ////高度
                    //if ((toolParam as BlobParam).ParticleFeaturesDataList.Exists(t => t.features.Equals
                    //   (Enum.GetName(typeof(EumParticleFeatures), EumParticleFeatures.height)) &&
                    //   t.isUse))
                    //{
                    //    HOperatorSet.SmallestRectangle2(objectSelected, out HTuple row, out HTuple column,
                    //        out HTuple phi, out HTuple length1, out HTuple length2);

                    //    double heightValue = length2.D * 2;
                    //    blobFeaturesResult.height = heightValue;
                    //}
                    ////矩形度
                    //if ((toolParam as BlobParam).ParticleFeaturesDataList.Exists(t => t.features.Equals
                    //   (Enum.GetName(typeof(EumParticleFeatures), EumParticleFeatures.rectangularity)) &&
                    //   t.isUse))
                    //{
                    //    HOperatorSet.Rectangularity(objectSelected,out HTuple rectangularity);
                    //    double rectangularityValue = rectangularity.D;
                    //    blobFeaturesResult.rectangularity = rectangularityValue;
                    //}
                    ////圆度
                    //if ((toolParam as BlobParam).ParticleFeaturesDataList.Exists(t => t.features.Equals
                    //   (Enum.GetName(typeof(EumParticleFeatures), EumParticleFeatures.circularity)) &&
                    //   t.isUse))
                    //{
                    //    HOperatorSet.Circularity(objectSelected, out HTuple circularity);
                    //    double circularityValue = circularity.D;
                    //    blobFeaturesResult.circularity = circularityValue;
                    //}
                    #endregion

                    (toolParam as BlobParam).BlobFeaturesResult.Add(blobFeaturesResult);
                }


                if ((toolParam as BlobParam).BlobFeaturesResult.Count > 0)
                {
                    StuBlobFeaturesResult dat = (toolParam as BlobParam).BlobFeaturesResult[0];
                    HOperatorSet.GenCrossContourXld(out HObject cross, dat.row, dat.column, 20, 0);
                    if (ObjectValided(cross))
                        HOperatorSet.ConcatObj(emptyRegionBuf, cross, out emptyRegionBuf);
                }

                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, emptyRegionBuf.Clone());
                else
                    dm.resultBufDic[toolName] = emptyRegionBuf.Clone();

                string info = toolName+"检测完成";
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;

                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, number.I > 0);
                else
                    dm.resultFlagDic[toolName] = number.I > 0;

                if ((toolParam as BlobParam).BlobFeaturesResult.Count>0)
                {
                    StuBlobFeaturesResult dat = (toolParam as BlobParam).BlobFeaturesResult[0];
                    if (!dm.PositionDataDic.ContainsKey(toolName))
                        dm.PositionDataDic.Add(toolName, new StuCoordinateData(dat.column, dat.row, 0));
                    else
                        dm.PositionDataDic[toolName] = new StuCoordinateData(dat.column, dat.row, 0);

                }
              
                grayImage.Dispose();
                //+输入图像
                HOperatorSet.ConcatObj((toolParam as BlobParam).InputImg, emptyRegionBuf, out HObject objectsConcat2);
                (toolParam as BlobParam).OutputImg = objectsConcat2;
                (toolParam as BlobParam).ResultNum = number.I;
                result.runFlag = true;
                (toolParam as BlobParam).BlobRunStatus = true;
                emptyRegionBuf.Dispose();
            }
            catch (Exception er)
            {
                string info = string.Format(toolName+"检测异常:{0}", er.Message);
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
                (toolParam as BlobParam).BlobRunStatus = false;
                result.errInfo = er.Message;
                sw.Stop();
                result.runTime = sw.ElapsedMilliseconds;
                return result;
            }
            sw.Stop();
            result.runTime = sw.ElapsedMilliseconds;
            return result;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            //inum++;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("Blob中心", ""));
            if (number > inum)
                inum = number;
            //toolName = "Blob" + number;
            inum++;
        }
    }
}
