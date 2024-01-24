using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using GlueDetectionLib.参数;
using OSLog;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace GlueDetectionLib.工具
{
    /// <summary>
    /// 模板匹配工具
    /// </summary>
    [Serializable]
    public  class MatchTool: BaseTool,IDisposable
    {
        public static int inum = 0;//工具编号

        public MatchTool()
        {
            toolParam = new MatchParam();
            toolName ="模板匹配"+ inum;     
            inum++;
         
        }

        public void Dispose()
        {
            if (hv_ModelID != null && hv_ModelID.Length > 0)
                HOperatorSet.ClearShapeModel(hv_ModelID);
            hv_ModelID = null;

        }
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (!ObjectValided((toolParam as MatchParam).InspectROI))
            {
                HOperatorSet.GenEmptyObj(out HObject emptyObj);
                (toolParam as MatchParam).InspectROI = emptyObj;
            }

        }
        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("模板匹配");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("模板匹配", ""));
            if (number > inum)
                inum = number;
            //toolName = "模板匹配" + number;
            inum++;
        }

        //模板句柄
        [NonSerialized]
         HTuple hv_ModelID = null;
        //模板轮廓
        HObject ModelContours = null;
        /// <summary>
        /// 模板训练
        /// </summary>
        /// <param name="modelTrans">模板轮廓特征</param>
        /// <returns></returns>
        public bool TemplateTraining(out HObject modelTrans)
        {

            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            HObject modelRegion = (toolParam as MatchParam).ModelROI;
            if (!ObjectValided(modelRegion))
            {
                log.Error(funName, "模板训练失败：模板区域无效");
                modelTrans = null;
                return false;
            }
            HObject ho_ModelTrans = null;
            HOperatorSet.GenEmptyObj(out ho_ModelTrans);
            HObject ho_ImageReduced2 = null;
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);

            //模板区域减去掩膜区域
            HObject subROI = null;
            HOperatorSet.GenEmptyObj(out subROI);
            subROI.Dispose();
            if (ObjectValided((toolParam as MatchParam).MaskROI))
            {
                HOperatorSet.Difference(modelRegion, (toolParam as MatchParam).MaskROI,
                    out HObject regionDifference);
                HOperatorSet.CopyObj(regionDifference, out subROI, 1, -1);
            }
            else
                HOperatorSet.CopyObj(modelRegion, out subROI, 1, -1);

            if (!ObjectValided((toolParam as MatchParam).InputImg))
            {
                log.Error(funName, "模板训练失败：输入图像为空");
                modelTrans = null;
                ho_ModelTrans.Dispose();
                ho_ImageReduced2.Dispose();
                subROI.Dispose();
                return false;
            }

            HOperatorSet.ReduceDomain((toolParam as MatchParam).InputImg, subROI, out HObject ho_ImageReduced);

            if (hv_ModelID != null && hv_ModelID.Length > 0)
                HOperatorSet.ClearShapeModel(hv_ModelID);
            hv_ModelID = null;
            try
            {
                //创建模板
                HOperatorSet.CreateScaledShapeModel(ho_ImageReduced, "auto", (toolParam as MatchParam).StartAngle / 180.0 * Math.PI
                 , (toolParam as MatchParam).RangeAngle / 180.0 * Math.PI, "auto",
                 (toolParam as MatchParam).MatchDownScale, (toolParam as MatchParam).MatchUpScale,
                 "auto", "auto", "use_polarity",
                 (toolParam as MatchParam).ContrastValue, "auto", out hv_ModelID);

                ho_ImageReduced2.Dispose();
                HOperatorSet.ReduceDomain((toolParam as MatchParam).InputImg, modelRegion,
                         out ho_ImageReduced2);

                //识别模板
                HOperatorSet.FindScaledShapeModel(ho_ImageReduced2, hv_ModelID, (toolParam as MatchParam).StartAngle / 180.0 * Math.PI
                   , (toolParam as MatchParam).RangeAngle / 180.0 * Math.PI, (toolParam as MatchParam).MatchDownScale,
                   (toolParam as MatchParam).MatchUpScale,
                     (toolParam as MatchParam).MatchScore, 1, (toolParam as MatchParam).MaxOverlap,
                     "least_squares", 0, (toolParam as MatchParam).GreedValue,
                       out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Angle, out HTuple hv_Scale, out HTuple hv_Score);
                //创建失败
                if (hv_Score.TupleLength() <= 0)
                {
                    log.Error(funName, "模板训练失败");
                    modelTrans = null;
                    ho_ModelTrans.Dispose();
                    ho_ImageReduced.Dispose();
                    ho_ImageReduced2.Dispose();
                    subROI.Dispose();
                    return false;
                }

                //设置原点
                HOperatorSet.SetShapeModelOrigin(hv_ModelID, (toolParam as MatchParam).ModelBaseRow - hv_Row,
                               (toolParam as MatchParam).ModelBaseCol - hv_Column);
                //再识别模板
                HOperatorSet.FindScaledShapeModel(ho_ImageReduced2, hv_ModelID, (toolParam as MatchParam).StartAngle / 180.0 * Math.PI
                   , (toolParam as MatchParam).RangeAngle / 180.0 * Math.PI, (toolParam as MatchParam).MatchDownScale,
                   (toolParam as MatchParam).MatchUpScale,
                     (toolParam as MatchParam).MatchScore, 1, (toolParam as MatchParam).MaxOverlap,
                     "least_squares", 0, (toolParam as MatchParam).GreedValue,
                       out hv_Row, out hv_Column, out hv_Angle, out hv_Scale, out hv_Score);
                //创建失败
                if (hv_Score.TupleLength() <= 0)
                {
                    log.Error(funName, "模板训练失败");
                    modelTrans = null;
                    ho_ModelTrans.Dispose();
                    ho_ImageReduced.Dispose();
                    ho_ImageReduced2.Dispose();
                    subROI.Dispose();
                    return false;
                }

                //创建成功
                HOperatorSet.HomMat2dIdentity(out HTuple hv_HomMat2DIdentity);
                HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_Row.TupleSelect(0),
                    hv_Column.TupleSelect(0), out HTuple hv_HomMat2DTranslate);
                HOperatorSet.HomMat2dRotate(hv_HomMat2DTranslate, hv_Angle.TupleSelect(0),
                    hv_Row.TupleSelect(0), hv_Column.TupleSelect(0), out HTuple hv_HomMat2DRotate);
                HOperatorSet.HomMat2dScale(hv_HomMat2DRotate, hv_Scale.TupleSelect(0), hv_Scale.TupleSelect(
                    0), hv_Row.TupleSelect(0), hv_Column.TupleSelect(0), out HTuple hv_HomMat2DScale);
                ho_ModelTrans.Dispose();
                //模板轮廓
                HOperatorSet.GetShapeModelContours(out ModelContours, hv_ModelID, 1);
                HOperatorSet.AffineTransContourXld(ModelContours, out ho_ModelTrans, hv_HomMat2DScale);
                HOperatorSet.CopyObj(ho_ModelTrans, out modelTrans, 1, -1);
                HOperatorSet.AffineTransPixel(hv_HomMat2DScale, 0, 0,
                           out HTuple rowTrans, out HTuple colTrans);
                (toolParam as MatchParam).ModelBaseRow = rowTrans.TupleSelect(0).D;
                (toolParam as MatchParam).ModelBaseCol = colTrans.TupleSelect(0).D;
                (toolParam as MatchParam).ModelBaseRadian = hv_Angle.TupleSelect(0).D;
                (toolParam as MatchParam).ModelContour = ho_ModelTrans.Clone();
                (toolParam as MatchParam).ModelImgOfPart = ho_ImageReduced2.Clone();
                (toolParam as MatchParam).ModelImgOfWhole = (toolParam as MatchParam).InputImg.Clone();

                HOperatorSet.WriteShapeModel(hv_ModelID, (toolParam as MatchParam).RootFolder + "\\" + toolName + ".shm");
                if (ObjectValided(ho_ImageReduced))
                {
                    HOperatorSet.CropDomain(ho_ImageReduced, out HObject imagePart);
                    HOperatorSet.WriteImage(imagePart, "png", 0, (toolParam as MatchParam).RootFolder + "\\" + toolName + ".png");
                    imagePart.Dispose();
                }

                ho_ModelTrans.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageReduced2.Dispose();
                subROI.Dispose();
               
            }
            catch
            {
                log.Error(funName, "模板训练失败");
                modelTrans = null;
                ho_ModelTrans.Dispose();
                ho_ImageReduced2.Dispose();
                subROI.Dispose();
                return false;
            }

            return true;
        }
        /// <summary>
        /// 模板匹配工具运行
        /// </summary>
        /// <returns></returns>
        override public RunResult Run()
        {
            DataManage dm = GetManage();
            if (!dm.enumerableTooDic.Contains(toolName))
                dm.enumerableTooDic.Add(toolName);
            HObject objectsConcat = null;
            HOperatorSet.GenEmptyObj(out objectsConcat);
            HObject ModelTransConcated = null;
            HOperatorSet.GenEmptyObj(out ModelTransConcated);
            HObject ho_ModelTrans = null;
            HOperatorSet.GenEmptyObj(out ho_ModelTrans);
            HObject imageReduced = null;
            HOperatorSet.GenEmptyObj(out imageReduced);
            RunResult result = new RunResult();
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (sw == null) sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
            try
            {
                //*先释放
                if (hv_ModelID != null && hv_ModelID.Length > 0)
                    HOperatorSet.ClearShapeModel(hv_ModelID);
                hv_ModelID = null;
                if (!System.IO.Directory.Exists((toolParam as MatchParam).RootFolder))
                    System.IO.Directory.CreateDirectory((toolParam as MatchParam).RootFolder);
                if (System.IO.File.Exists((toolParam as MatchParam).RootFolder + "\\" + toolName + ".shm"))
                   HOperatorSet.ReadShapeModel((toolParam as MatchParam).RootFolder + "\\" + toolName + ".shm", out hv_ModelID);

                (toolParam as MatchParam).InputImg = dm.imageBufDic[(toolParam as MatchParam).InputImageName];
                if (!ObjectValided((toolParam as MatchParam).InputImg))
                {
                    (toolParam as MatchParam).MatchRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+ "输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;

                    if (!dm.matrixBufDic.ContainsKey(toolName))
                        dm.matrixBufDic.Add(toolName, null);
                    else
                        dm.matrixBufDic[toolName] = null;
                    return result;
                }
              
                if (hv_ModelID == null || hv_ModelID.TupleLength() <= 0)
                {
                    (toolParam as MatchParam).MatchRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+ "模板句柄无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;

                    if (!dm.matrixBufDic.ContainsKey(toolName))
                        dm.matrixBufDic.Add(toolName, null);
                    else
                        dm.matrixBufDic[toolName] = null;

                    return result;
                }

                imageReduced.Dispose();

                HObject InspectROI = (toolParam as MatchParam).InspectROI;
                //输入检测区域有效
                if ((toolParam as MatchParam).ModelSearch == EumModelSearch.局部搜索
                    && ObjectValided(InspectROI))
                    HOperatorSet.ReduceDomain((toolParam as MatchParam).InputImg,
                        InspectROI, out imageReduced);
                //输入检测区域无效则为全图检测
                else
                    HOperatorSet.CopyObj((toolParam as MatchParam).InputImg, out imageReduced, 1, 1);

                //识别模板
                HOperatorSet.FindScaledShapeModel(imageReduced, hv_ModelID, (toolParam as MatchParam).StartAngle / 180.0 * Math.PI
                   , (toolParam as MatchParam).RangeAngle / 180.0 * Math.PI, (toolParam as MatchParam).MatchDownScale,
                   (toolParam as MatchParam).MatchUpScale,
                     (toolParam as MatchParam).MatchScore, (toolParam as MatchParam).MatchNumber, (toolParam as MatchParam).MaxOverlap,
                     "least_squares", 0, (toolParam as MatchParam).GreedValue,
                       out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Angle, out HTuple hv_Scale, out HTuple hv_Score);
                //匹配失败
                if (hv_Score.TupleLength() <= 0)
                {                  
                    if (!dm.resultInfoDic.ContainsKey(toolName))
                        dm.resultInfoDic.Add(toolName, toolName+"模板匹配失败");
                    else
                        dm.resultInfoDic[toolName] = toolName+"模板匹配失败";

                    log.Error(funName, toolName+"模板匹配失败");
                    (toolParam as MatchParam).MatchRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"模板匹配失败";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;

                    if (!dm.matrixBufDic.ContainsKey(toolName))
                        dm.matrixBufDic.Add(toolName, null);
                    else
                        dm.matrixBufDic[toolName] = null;

                    return result;
                }
                //+匹配轮廓
                //ModelTransConcated.Dispose();

                for (int i = 0; i < hv_Score.TupleLength() ; i++)
                {
                    //匹配成功
                    HOperatorSet.HomMat2dIdentity(out HTuple hv_HomMat2DIdentity);
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_Row.TupleSelect(i),
                        hv_Column.TupleSelect(i), out HTuple hv_HomMat2DTranslate);
                    HOperatorSet.HomMat2dRotate(hv_HomMat2DTranslate, hv_Angle.TupleSelect(i),
                        hv_Row.TupleSelect(i), hv_Column.TupleSelect(i), out HTuple hv_HomMat2DRotate);
                    HOperatorSet.HomMat2dScale(hv_HomMat2DRotate, hv_Scale.TupleSelect(i), hv_Scale.TupleSelect(
                        i), hv_Row.TupleSelect(i), hv_Column.TupleSelect(i), out HTuple hv_HomMat2DScale);
                    ho_ModelTrans.Dispose();
                    HOperatorSet.GetShapeModelContours(out ModelContours, hv_ModelID, 1);
                    HOperatorSet.AffineTransContourXld(ModelContours, out ho_ModelTrans, hv_HomMat2DScale);
                    HOperatorSet.ConcatObj(ModelTransConcated, ho_ModelTrans, out ModelTransConcated);

                }

                objectsConcat.Dispose();
                //输出仿射变换矩阵
                double baseRow = (toolParam as MatchParam).ModelBaseRow;
                double baseCol = (toolParam as MatchParam).ModelBaseCol;
                double baseRadian = (toolParam as MatchParam).ModelBaseRadian;
                double currRow = hv_Row.TupleSelect(0).D;
                double currCol= hv_Column.TupleSelect(0).D;
                double currRadian= hv_Angle.TupleSelect(0).D;
                HOperatorSet.VectorAngleToRigid(baseRow, baseCol, baseRadian, currRow, currCol, currRadian, out HTuple homMat2D);
                (toolParam as MatchParam).AffinneTranMatix = homMat2D; //变换矩阵

                if (!dm.matrixBufDic.ContainsKey(toolName))
                    dm.matrixBufDic.Add(toolName, homMat2D);
                else
                    dm.matrixBufDic[toolName] = homMat2D;
                //输出图像,检测区域及匹配轮廓做结果显示


                    //+检测区域
                    //if (ObjectValided(InspectROI))
                    //    HOperatorSet.ConcatObj(InspectROI, ModelTransConcated, out objectsConcat);
                    //else
                    //    HOperatorSet.CopyObj(ModelTransConcated, out objectsConcat, 1, -1);
                    //+匹配特征点
                HOperatorSet.GenCrossContourXld(out HObject cross, hv_Row, hv_Column, 20, hv_Angle);
                HOperatorSet.ConcatObj(cross, ModelTransConcated, out objectsConcat);
                (toolParam as MatchParam).ResultContour = objectsConcat.Clone();

                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, objectsConcat.Clone());
                else
                    dm.resultBufDic[toolName] = objectsConcat.Clone();

                string info = string.Format("模板匹配OK,坐标x:{0:f3},y:{1:f3},r:{2:f3}", hv_Column.D, hv_Row.D, hv_Angle.D) ;
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;
             
                //+输入图像
                //HOperatorSet.ConcatObj((toolParam as MatchParam).InputImg, objectsConcat, out HObject objectsConcat2);

                (toolParam as MatchParam).OutputImg = (toolParam as MatchParam).InputImg;
                (toolParam as MatchParam).MatchResultNumber = hv_Score.TupleLength();
                (toolParam as MatchParam).MatchResultScales = hv_Scale;
                (toolParam as MatchParam).MatchResultScores = hv_Score;
                (toolParam as MatchParam).MatchResultRows = hv_Row;
                (toolParam as MatchParam).MatchResultColumns = hv_Column;
                (toolParam as MatchParam).MatchResultRadians = hv_Angle;
                result.runFlag = true;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;
                (toolParam as MatchParam).MatchRunStatus = true;
                objectsConcat.Dispose();
                ModelTransConcated.Dispose();
                ho_ModelTrans.Dispose();
                imageReduced.Dispose();
            }
            catch (Exception er)
            {
                if (!dm.matrixBufDic.ContainsKey(toolName))
                    dm.matrixBufDic.Add(toolName, null);
                else
                    dm.matrixBufDic[toolName] = null;

                string info = string.Format(toolName+"模板匹配异常：{0}",er.Message);
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;

                objectsConcat.Dispose();
                ModelTransConcated.Dispose();
                ho_ModelTrans.Dispose();
                imageReduced.Dispose();
                log.Error(funName, er.Message);
                result.runFlag = false;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as MatchParam).MatchRunStatus = false;
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
