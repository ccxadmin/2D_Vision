using HalconDotNet;
using OSLog;
using PositionToolsLib.参数;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.工具
{
    public class ImageCorrectTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号
        public string CalibFilePath = "";//标定文件路径
        public ImageCorrectTool()
        {
            toolParam = new ImageCorrectParam();
            toolName = "畸变校正" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("畸变校正");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("畸变校正", ""));
            if (number > inum)
                inum = number;
    
            inum++;
        }

        /// <summary>
        /// 获取相机内参
        /// </summary>
        /// <param name="homMat2D"></param>
        override public void GetMatrix(HTuple homMat2D)
        {
            (toolParam as ImageCorrectParam).Hv_CamParam = homMat2D;
        }
        /// <summary>
        /// 拟合直线工具运行
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

                (toolParam as ImageCorrectParam).InputImg = dm.imageBufDic[(toolParam as ImageCorrectParam).InputImageName];
                if (!ObjectValided((toolParam as ImageCorrectParam).InputImg))
                {
                    (toolParam as ImageCorrectParam).ImageCorrectRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                HTuple CamParam = (toolParam as ImageCorrectParam).Hv_CamParam;
                if (CamParam == null || CamParam.Length <= 0)
                {
                    (toolParam as ImageCorrectParam).ImageCorrectRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入相机内参无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }

                HObject ho_Map, ho_ImageMapped;
                HOperatorSet.GenEmptyObj(out ho_Map);
                HOperatorSet.GenEmptyObj(out ho_ImageMapped);
                HTuple hv_CamParamOut;

                //Change the radial distortion: mode 'adaptive'
                HOperatorSet.ChangeRadialDistortionCamPar("adaptive", CamParam, 0, out hv_CamParamOut);
                //get_domain (Image, Domain)
                //* change_radial_distortion_image (Image, Domain, ImageRectifiedAdaptive, CamParam, CamParamOut)
                //* dev_display (ImageRectifiedAdaptive)
                ////图像校正2
                ho_Map.Dispose();
                HOperatorSet.GenRadialDistortionMap(out ho_Map, CamParam, hv_CamParamOut,
                    "bilinear");
                ho_ImageMapped.Dispose();
                HOperatorSet.MapImage((toolParam as ImageCorrectParam).InputImg, ho_Map, out ho_ImageMapped);

                string info = toolName + "检测完成";
                //测试信息
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;
                //测试结果
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;

                //+输入图像

                ho_Map.Dispose();
                (toolParam as ImageCorrectParam).OutputImg = ho_ImageMapped;
                result.runFlag = true;
                (toolParam as ImageCorrectParam).ImageCorrectRunStatus = true;
                //添加图像到缓存集合
                if (!dm.imageBufDic.ContainsKey(toolName))
                    dm.imageBufDic.Add(toolName, (toolParam as ImageCorrectParam).OutputImg);
                else
                    dm.imageBufDic[toolName] = (toolParam as ImageCorrectParam).OutputImg;
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
                (toolParam as ImageCorrectParam).ImageCorrectRunStatus = false;
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
