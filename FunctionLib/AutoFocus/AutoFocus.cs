using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using OSLog;
namespace FunctionLib.AutoFocus
{
    /// <summary>
    /// 自动对焦
    /// </summary>
    public class AutoFocus
    {
        /// <summary>
        ///  自动对焦
        /// </summary>
        static AutoFocus()
        {
            stuProcessFocusData = new StuProcessFocusResultData(-1);
            stuProcessFocusData.resetData();

        }
        static Log log = new Log("自动对焦");
        static StuProcessFocusResultData stuProcessFocusData ;


        /// <summary>
        /// 数据复位
        /// </summary>
        static public void resetData()
        {
            stuProcessFocusData.resetData();
        }

        /// <summary>
        /// 自动调整   
        /// </summary>
        /// <returns>需要调整的数据结构</returns>
        public static StuProcessFocusSendData sendCmdOfAutoAdjust()
        {
            StuProcessFocusSendData stuProcessFocusSendData=new StuProcessFocusSendData(-1);
           
            if(stuProcessFocusData.focusSuccessFlag)
            {
                stuProcessFocusSendData.noticeInfo = "对焦完成";
                stuProcessFocusSendData.eumtendency = Eumtendency.ending;
                stuProcessFocusSendData.index = stuProcessFocusData.dataIndex;
                stuProcessFocusSendData.deviation = stuProcessFocusData.currDeviation;
            }
            else
            {
                if(stuProcessFocusData.focusExceptionFlag)
                {
                    stuProcessFocusSendData.deviation = stuProcessFocusData.currDeviation;
                    stuProcessFocusSendData.noticeInfo = "对焦异常";
                    stuProcessFocusSendData.eumtendency = Eumtendency.exception;
                }             
                else
                {
                    stuProcessFocusSendData.deviation = stuProcessFocusData.currDeviation;
                    stuProcessFocusSendData.noticeInfo = "继续调整";
                    stuProcessFocusSendData.eumtendency = stuProcessFocusData.currEumtendency;
                }           
            }         
            return stuProcessFocusSendData;
        }

        /// <summary>
        /// 自动对焦运算结果判定
        /// </summary>
        /// <param name="ho_inputImage"></param>
        /// <param name="ho_focusRegion"></param>
        /// <returns></returns>
        public static bool calculateState(HObject ho_inputImage, HObject ho_focusRegion,
                            int compareValue = 10, int NormalStep = 20)
        {
            HTuple FocusRefrenceValue;

            bool flag = evaluate_definition(ho_inputImage, ho_focusRegion, "Brenner", out FocusRefrenceValue);

            if (flag)
            {
                //添加当前偏差值
                stuProcessFocusData.addDeviationValue(FocusRefrenceValue.D);
                //趋势判断
                stuProcessFocusData.tendencyJudgment(compareValue);
                //结果判断          
                stuProcessFocusData.resultJudgment(NormalStep);
                return true;
            }
            else
            {
                string strfunName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(strfunName,"对焦错误");
                return false;
            }
           // return stuProcessFocusData;
        }

        /// <summary>
        /// 计算偏差，对焦评估；
        /// ho_inputImage：输入图像；
        /// ho_focusRegion：对焦区域；
        /// hv_methodName：对焦方法（ 'laplace', 'energy', 'Brenner'）
        /// </summary>
        /// <param name="ho_inputImage">输入图像</param>
        /// <param name="ho_focusRegion">对焦区域</param>
        /// <param name="hv_methodName">对焦方法</param>
        /// <param name="hv_FocusRefrenceValue">对焦参考值</param>
        /// <returns>运行标志</returns>
      static  bool evaluate_definition(HObject ho_inputImage, HObject ho_focusRegion,
                                            HTuple hv_methodName, out HTuple hv_FocusRefrenceValue)
        {


            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImageScaled, ho_ImageLaplace4 = null;
            HObject ho_ImageLaplace8 = null, ho_ImageResult1 = null, ho_ImageResult = null;
            HObject ho_ImagePart00 = null, ho_ImagePart01 = null, ho_ImagePart10 = null;
            HObject ho_ImageSub1 = null, ho_ImageSub2 = null, ho_ImageResult2 = null;
            HObject ho_ImagePart20 = null, ho_ImageSub = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_Value = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageLaplace4);
            HOperatorSet.GenEmptyObj(out ho_ImageLaplace8);
            HOperatorSet.GenEmptyObj(out ho_ImageResult1);
            HOperatorSet.GenEmptyObj(out ho_ImageResult);
            HOperatorSet.GenEmptyObj(out ho_ImagePart00);
            HOperatorSet.GenEmptyObj(out ho_ImagePart01);
            HOperatorSet.GenEmptyObj(out ho_ImagePart10);
            HOperatorSet.GenEmptyObj(out ho_ImageSub1);
            HOperatorSet.GenEmptyObj(out ho_ImageSub2);
            HOperatorSet.GenEmptyObj(out ho_ImageResult2);
            HOperatorSet.GenEmptyObj(out ho_ImagePart20);
            HOperatorSet.GenEmptyObj(out ho_ImageSub);
            hv_FocusRefrenceValue = new HTuple();
            ho_ImageScaled.Dispose();

            try
            {
                HOperatorSet.ScaleImageMax(ho_inputImage, out ho_ImageScaled);
                HOperatorSet.GetImageSize(ho_inputImage, out hv_Width, out hv_Height);

                if ((int)(new HTuple(hv_methodName.TupleEqual("laplace"))) != 0)
                {
                    //拉普拉斯能量函数
                    ho_ImageLaplace4.Dispose();
                    HOperatorSet.Laplace(ho_ImageScaled, out ho_ImageLaplace4, "signed", 3, "n_4");
                    ho_ImageLaplace8.Dispose();
                    HOperatorSet.Laplace(ho_ImageScaled, out ho_ImageLaplace8, "signed", 3, "n_8");
                    ho_ImageResult1.Dispose();
                    HOperatorSet.AddImage(ho_ImageLaplace4, ho_ImageLaplace4, out ho_ImageResult1,
                        1, 0);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.AddImage(ho_ImageLaplace4, ho_ImageResult1, out ExpTmpOutVar_0,
                            1, 0);
                        ho_ImageResult1.Dispose();
                        ho_ImageResult1 = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.AddImage(ho_ImageLaplace8, ho_ImageResult1, out ExpTmpOutVar_0,
                            1, 0);
                        ho_ImageResult1.Dispose();
                        ho_ImageResult1 = ExpTmpOutVar_0;
                    }
                    ho_ImageResult.Dispose();
                    HOperatorSet.MultImage(ho_ImageResult1, ho_ImageResult1, out ho_ImageResult,
                        1, 0);
                    HOperatorSet.Intensity(ho_focusRegion, ho_ImageResult, out hv_Value, out hv_FocusRefrenceValue);

                }
                else if ((int)(new HTuple(hv_methodName.TupleEqual("energy"))) != 0)
                {
                    //能量梯度函数
                    ho_ImagePart00.Dispose();
                    HOperatorSet.CropPart(ho_ImageScaled, out ho_ImagePart00, 0, 0, hv_Width - 1,
                        hv_Height - 1);
                    ho_ImagePart01.Dispose();
                    HOperatorSet.CropPart(ho_ImageScaled, out ho_ImagePart01, 0, 1, hv_Width - 1,
                        hv_Height - 1);
                    ho_ImagePart10.Dispose();
                    HOperatorSet.CropPart(ho_ImageScaled, out ho_ImagePart10, 1, 0, hv_Width - 1,
                        hv_Height - 1);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImagePart00, out ExpTmpOutVar_0, "real");
                        ho_ImagePart00.Dispose();
                        ho_ImagePart00 = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImagePart10, out ExpTmpOutVar_0, "real");
                        ho_ImagePart10.Dispose();
                        ho_ImagePart10 = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImagePart01, out ExpTmpOutVar_0, "real");
                        ho_ImagePart01.Dispose();
                        ho_ImagePart01 = ExpTmpOutVar_0;
                    }
                    ho_ImageSub1.Dispose();
                    HOperatorSet.SubImage(ho_ImagePart10, ho_ImagePart00, out ho_ImageSub1, 1,
                        0);
                    ho_ImageResult1.Dispose();
                    HOperatorSet.MultImage(ho_ImageSub1, ho_ImageSub1, out ho_ImageResult1, 1,
                        0);
                    ho_ImageSub2.Dispose();
                    HOperatorSet.SubImage(ho_ImagePart01, ho_ImagePart00, out ho_ImageSub2, 1,
                        0);
                    ho_ImageResult2.Dispose();
                    HOperatorSet.MultImage(ho_ImageSub2, ho_ImageSub2, out ho_ImageResult2, 1,
                        0);
                    ho_ImageResult.Dispose();
                    HOperatorSet.AddImage(ho_ImageResult1, ho_ImageResult2, out ho_ImageResult,
                        1, 0);
                    HOperatorSet.Intensity(ho_focusRegion, ho_ImageResult, out hv_Value, out hv_FocusRefrenceValue);

                }
                else if ((int)(new HTuple(hv_methodName.TupleEqual("Brenner"))) != 0)
                {
                    //Brenner函数法
                    ho_ImagePart00.Dispose();
                    HOperatorSet.CropPart(ho_ImageScaled, out ho_ImagePart00, 0, 0, hv_Width, hv_Height - 2);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImagePart00, out ExpTmpOutVar_0, "real");
                        ho_ImagePart00.Dispose();
                        ho_ImagePart00 = ExpTmpOutVar_0;
                    }
                    ho_ImagePart20.Dispose();
                    HOperatorSet.CropPart(ho_ImageScaled, out ho_ImagePart20, 2, 0, hv_Width, hv_Height - 2);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImagePart20, out ExpTmpOutVar_0, "real");
                        ho_ImagePart20.Dispose();
                        ho_ImagePart20 = ExpTmpOutVar_0;
                    }
                    ho_ImageSub.Dispose();
                    HOperatorSet.SubImage(ho_ImagePart20, ho_ImagePart00, out ho_ImageSub, 1, 0);
                    ho_ImageResult.Dispose();
                    HOperatorSet.MultImage(ho_ImageSub, ho_ImageSub, out ho_ImageResult, 1, 0);
                    HOperatorSet.Intensity(ho_focusRegion, ho_ImageResult, out hv_Value, out hv_FocusRefrenceValue);

                }
            }
            catch (Exception er)
            {
                string strfunName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(strfunName, er.Message);
                ho_ImageScaled.Dispose();
                ho_ImageLaplace4.Dispose();
                ho_ImageLaplace8.Dispose();
                ho_ImageResult1.Dispose();
                ho_ImageResult.Dispose();
                ho_ImagePart00.Dispose();
                ho_ImagePart01.Dispose();
                ho_ImagePart10.Dispose();
                ho_ImageSub1.Dispose();
                ho_ImageSub2.Dispose();
                ho_ImageResult2.Dispose();
                ho_ImagePart20.Dispose();
                ho_ImageSub.Dispose();
                return false;

            }

            ho_ImageScaled.Dispose();
            ho_ImageLaplace4.Dispose();
            ho_ImageLaplace8.Dispose();
            ho_ImageResult1.Dispose();
            ho_ImageResult.Dispose();
            ho_ImagePart00.Dispose();
            ho_ImagePart01.Dispose();
            ho_ImagePart10.Dispose();
            ho_ImageSub1.Dispose();
            ho_ImageSub2.Dispose();
            ho_ImageResult2.Dispose();
            ho_ImagePart20.Dispose();
            ho_ImageSub.Dispose();
            return true;

        }

    }

    /// <summary>
    /// 运动趋势
    /// </summary>
    public enum Eumtendency
    {
        /// <summary>
        /// 结束：对焦流程正常结束
        /// </summary>
        ending,
        /// <summary>
        /// 正向：自动调整确运动趋势为正向
        /// </summary>
        positive,
        /// <summary>
        /// 反向：自动调整确运动趋势为反向
        /// </summary>
        negative,
        /// <summary>
        /// 异常：自动调整确运动趋势为反向
        /// </summary>
        exception,
        /// <summary>
        /// 错误：终止对焦，检查参数或流程是否正确？
        /// </summary>
       error,

    }

    /// <summary>
    /// 自动对焦发送数据结构
    /// </summary>
    public struct StuProcessFocusSendData
    {
        public StuProcessFocusSendData(int _indexNum = -1)
        {
            eumtendency = Eumtendency.positive;
            index = -1;
            noticeInfo = string.Empty;
            deviation = 0;
        }
        /// <summary>
        /// 运动趋势
        /// </summary>
        public Eumtendency eumtendency;
        /// <summary>
        /// 通知信息
        /// </summary>
        public string noticeInfo;
        /// <summary>
        /// 对焦位置索引编号
        /// </summary>
        public int index;
        /// <summary>
        /// 偏差值
        /// </summary>
        public double deviation;
        /// <summary>
        /// 数据复位
        /// </summary>
        public void resetData()
        {
            eumtendency = Eumtendency.positive;
            index = -1;
            noticeInfo = string.Empty;           
        }

        public override string ToString()
        {
            return string.Format("运动趋势：{0},偏差值：{1},通知信息：{2},数据索引：{3}",eumtendency.ToString(),
                deviation,noticeInfo, index);
        }
    }
    /// <summary>
    /// 自动对焦结果数据结构
    /// </summary>
    public struct StuProcessFocusResultData
    {
        public StuProcessFocusResultData(int _indexNum=-1)
        {
            dataIndex = -1;
            DeviationList = new List<double>();
            eumtendencyList = new List<Eumtendency>();
            focusSuccessFlag = false;
            focusExceptionFlag = false;
          
        }
        public int dataIndex;
        /// <summary>
        /// 偏差值集合
        /// </summary>
         List<double> DeviationList;
        /// <summary>
        /// 对焦趋势集合
        /// </summary>
         List<Eumtendency> eumtendencyList;  
        /// <summary>
        /// 对焦成功标志
        /// </summary>
        public bool focusSuccessFlag;

        /// <summary>
        /// 对焦异常标志
        /// </summary>
        public bool focusExceptionFlag;

        
        /// <summary>
        /// 当前偏差值
        /// </summary>
        public double currDeviation
        {
            get
            {
                if (DeviationList.Count > 0)
                    return DeviationList[DeviationList.Count - 1];
                else
                    return 0;
            }
        }

        /// <summary>
        /// 当前趋势
        /// </summary>
        public Eumtendency currEumtendency
        {
            get
            {
                if (eumtendencyList.Count > 0)
                    return eumtendencyList[eumtendencyList.Count - 1];
                else
                    return Eumtendency.positive;
            }
        }
        /// <summary>
        /// 数据复位
        /// </summary>
        public void resetData()
        {
            dataIndex = -1;
            DeviationList.Clear();
            eumtendencyList.Clear();
            focusSuccessFlag = false;
            focusExceptionFlag = false;
         
        }

        public void addDeviationValue(double DeviationValue)
        {
            DeviationList.Add(DeviationValue);
        }
        /// <summary>
        /// 趋势判定
        /// </summary>
        /// <param name="compareValue">偏差比较值</param>
        public void tendencyJudgment(int compareValue=10)
        {
            int count = DeviationList.Count;
            if(count>1)
            {
                //当前偏差值与上一次的偏差值做比较，偏差值越大，对焦趋势为正，否则为负
                if (DeviationList[count - 1] - DeviationList[count - 2]>= compareValue * -1)
                    eumtendencyList.Add(Eumtendency.positive);
                else 
                    eumtendencyList.Add(Eumtendency.negative);              
            }
            else //默认为正趋势
            {
                eumtendencyList.Add(Eumtendency.positive);
            }         
        }
        /// <summary>
        /// 结果判定
        /// </summary>
        public void resultJudgment(int NormalStep=20)
        {
            int count = eumtendencyList.FindAll(s => s == Eumtendency.negative).Count;
            if (count >= 2)
            {
                double maxvalue = DeviationList.Max();
                dataIndex = DeviationList.FindIndex(s => s == maxvalue);
                focusSuccessFlag = true;
                focusExceptionFlag = false;
            }
            else
            {    
                if(DeviationList.Count>= NormalStep)
                {
                    if (count == 0)
                        focusExceptionFlag = true;
                }                  
                focusSuccessFlag = false;
                dataIndex = -1;
               
            }             
        }
    }
}
