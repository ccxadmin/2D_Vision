using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace FunctionLib.GlueWidth
{
    [Serializable]
    /// <summary>
    ///胶水宽度测量设置参数
    /// </summary>
    public  class GlueWidthParam
    {
        public GlueWidthParam()
        {

        }
        public GlueWidthParam(byte thd,float Sigma,int DetectHeight, EumTransition Transition, EumSelect Select)
        {
            hv_Threshold = thd;
            hv_Sigma = Sigma;
            hv_DetectHeight = DetectHeight;
            hv_Transition = Transition;
            hv_Select = Select;
        }

        public void updateParam(byte thd, float Sigma, int DetectHeight, EumTransition Transition, EumSelect Select)
        {
            hv_Threshold = thd;
            hv_Sigma = Sigma;
            hv_DetectHeight = DetectHeight;
            hv_Transition = Transition;
            hv_Select = Select;
        }

        public int hv_DetectHeight=100;
        public float hv_Sigma=1;
        public int hv_Threshold=20;
        public EumTransition hv_Transition= EumTransition.all;
        public EumSelect hv_Select= EumSelect.all;

    }
    /// <summary>
    /// 极性
    /// </summary>
    public enum EumTransition
    {
        all,
        all_strongest,
        negative, 
        negative_strongest,
        positive,
        positive_strongest
    }
    /// <summary>
    /// 选择
    /// </summary>
    public enum EumSelect
    { 
        all,
        first, 
        last
    
    }

    /// <summary>
    /// 胶水宽度运行结果
    /// </summary>
    public struct StuGlueWidthResult
    {
        public StuGlueWidthResult(int i=0)
        {
            HOperatorSet.GenEmptyObj(out drawPoints);
            HOperatorSet.GenEmptyObj(out searchRegion);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            hv_ResultRows = 0;
            hv_ResultColumns = 0;
            hv_Distance = 0;
            runFlag = false;
            errInfo = string.Empty;
        }
        public HObject drawPoints;
        public HObject searchRegion;
        public HObject ho_Regions;
        public HTuple hv_ResultRows;
        public HTuple hv_ResultColumns;
        public HTuple hv_Distance;
        public bool runFlag;
        public string errInfo;
    }
    /// <summary>
    /// 类型设置
    /// </summary>
    public enum EumSetting
    {
        create,
        fit
    }
}
