using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Xml.Serialization;


namespace GlueDetectionLib.参数
{
    /// <summary>
    ///模板匹配参数
    /// </summary>
    [Serializable]
    public class MatchParam : BaseParam
    {
        public MatchParam()
        {

        }
        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --13 
        
        string rootFolder =AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 根路径
        /// </summary>
        [Description("根路径"), DefaultValue("")]
        public string RootFolder
        {
            get => this.rootFolder;
            set
            {
                this.rootFolder = value;
            }
        }

        HObject modelROI = null;
        /// <summary>
        /// 模板ROI
        /// </summary>
        [Description("模板ROI"), DefaultValue(null)]
        public HObject ModelROI
        {
            get => this.modelROI;
            set
            {
                this.modelROI = value; 
            }
        }

        HObject maskROI = null;
        /// <summary>
        /// 掩膜ROI
        /// </summary>
        [Description("掩膜ROI"), DefaultValue(null)]
        public HObject MaskROI
        {
            get => this.maskROI;
            set
            {
                this.maskROI = value;    
            }
        }


        HObject inspectROI = null;
        /// <summary>
        /// 检测ROI
        /// </summary>
        [Description("检测ROI"), DefaultValue(null)]
        public HObject InspectROI
        {
            get => this.inspectROI;
            set
            {
                this.inspectROI = value;  
            }
        }
        EumModelSearch modelSearch = EumModelSearch.全图搜索;
        /// <summary>
        /// 模板搜索方式
        /// </summary>
        [Description("模板搜索方式"), DefaultValue(null)]
        public EumModelSearch ModelSearch
        {
            get => this.modelSearch;
            set
            {
                this.modelSearch = value;
            }
        }
        float greedValue = 0.9f;
        /// <summary>
        /// 模板匹配贪婪系数
        /// </summary>
        [Description("模板匹配贪婪系数"), DefaultValue(0.9)]
        public float GreedValue
        {
            get => this.greedValue;
            set
            {
                this.greedValue = value;      
            }
        }

        byte contrastValue = 30;
        /// <summary>
        /// 模板匹配对比度
        /// </summary>
        [Description("模板匹配对比度"), DefaultValue(30)]
        public byte ContrastValue
        {
            get => this.contrastValue;
            set
            {
                this.contrastValue = value;      
            }
        }

        int startAngle = -90;
        /// <summary>
        /// 模板匹配起始角度
        /// </summary>
        [Description("模板匹配起始角度"), DefaultValue(-90)]
        public int StartAngle
        {
            get => this.startAngle;
            set
            {
                this.startAngle = value;        
            }
        }

        int rangeAngle = 180;
        /// <summary>
        /// 模板匹配角度范围
        /// </summary>
        [Description("模板匹配角度范围"), DefaultValue(180)]
        public int RangeAngle
        {
            get => this.rangeAngle;
            set
            {
                this.rangeAngle = value;         
            }
        }

        float matchDownScale = 1.0f;
        /// <summary>
        /// 模板匹配比例下限
        /// </summary>
        [Description("模板匹配比例下限"), DefaultValue(1.0)]
        public float MatchDownScale
        {
            get => this.matchDownScale;
            set
            {
                this.matchDownScale = value;         

            }
        }

        float matchUpScale = 1.0f;
        /// <summary>
        /// 模板匹配比例上限
        /// </summary>
        [Description("模板匹配比例上限"), DefaultValue(1.0)]
        public float MatchUpScale
        {
            get => this.matchUpScale;
            set
            {
                this.matchUpScale = value;           
            }
        }


        float maxOverlap = 0.5f;
        /// <summary>
        /// 模板重叠比例
        /// </summary>
        [Description("模板重叠比例"), DefaultValue(0.5)]
        public float MaxOverlap
        {
            get => this.maxOverlap;
            set
            {
                this.maxOverlap = value;
            }
        }


        int matchNumber = 1;
        /// <summary>
        /// 模板匹配个数
        /// </summary>
        [Description("模板匹配数量"), DefaultValue(1)]
        public int MatchNumber
        {
            get => this.matchNumber;
            set
            {
                this.matchNumber = value;  
            }
        }

        float matchScore = 0.4f;
        /// <summary>
        /// 模板匹配得分
        /// </summary>
        [Description("模板匹配得分"), DefaultValue(0.4)]
        public float MatchScore
        {
            get => this.matchScore;
            set
            {
                this.matchScore = value;       
            }
        }

        //Output parmas value define ---10
        double modelBaseRow = 0;
        /// <summary>
        /// 模板基准点行坐标
        /// </summary>
        [Description("模板基准行坐标"), DefaultValue(0)]
        public double ModelBaseRow
        {
            get => modelBaseRow;
            set
            {
                modelBaseRow = value;  
            }
        }


        double modelBaseCol = 0;
        /// <summary>
        /// 模板基准列坐标
        /// </summary>
        [Description("模板基准列坐标"), DefaultValue(0)]
        public double ModelBaseCol
        {
            get => modelBaseCol;
            set
            {
                modelBaseCol = value;    
            }
        }

        double modelBaseRadian = 0;
        /// <summary>
        /// 模板基准弧度
        /// </summary>
        [Description("模板基准弧度"), DefaultValue(0)]
        public double ModelBaseRadian
        {
            get => modelBaseRadian;
            set
            {
                modelBaseRadian = value;        
            }
        }

        HObject modelImgOfPart = null;
        /// <summary>
        /// 局部模板图像
        /// </summary>     
        [Description("局部模板图像"), DefaultValue(null)]
        public HObject ModelImgOfPart
        {
            get => this.modelImgOfPart;
            set
            {
                this.modelImgOfPart = value;            
            }
        }

        HObject modelImgOfWhole = null;
        /// <summary>
        /// 完整模板图像
        /// </summary>     
        [Description("完整模板图像"), DefaultValue(null)]
        public HObject ModelImgOfWhole
        {
            get => this.modelImgOfWhole;
            set
            {
                this.modelImgOfWhole = value;        
            }
        }


        HObject modelContour = null;
        /// <summary>
        /// 模板轮廓
        /// </summary>     
        [Description("模板轮廓"), DefaultValue(null)]
        public HObject ModelContour
        {
            get => this.modelContour;
            set
            {
                this.modelContour = value;   
            }
        }


        int matchResultNumber = 0;
        /// <summary>
        /// 模板匹配结果个数
        /// </summary>
        [Description("模板匹配结果数量"), DefaultValue(0)]
        public int MatchResultNumber
        {
            get { return this.matchResultNumber; }
            set
            {
                matchResultNumber = value;  
            }
        }

        HTuple matchResultRows = 0;
        /// <summary>
        /// 模板匹配行坐标
        /// </summary>
        [Description("模板匹配行坐标"), DefaultValue(0)]
        public HTuple MatchResultRows
        {
            get => matchResultRows;
            set
            {
                matchResultRows = value;  
            }
        }

        HTuple matchResultColumns = 0;
        /// <summary>
        /// 模板匹配列坐标
        /// </summary>
        [Description("模板匹配列坐标"), DefaultValue(0)]
        public HTuple MatchResultColumns
        {
            get => matchResultColumns;

            set
            {
                matchResultColumns = value;    
            }
        }

        HTuple matchResultRadians = 0;
        /// <summary>
        /// 模板匹配结果弧度
        /// </summary>
        [Description("模板匹配结果弧度"), DefaultValue(0)]
        public HTuple MatchResultRadians
        {
            get => matchResultRadians;
            set
            {
                matchResultRadians = value;     
            }

        }

        HTuple matchResultScales = 0;
        /// <summary>
        /// 模板匹配结果比例大小
        /// </summary>
        [Description("模板匹配结果比例大小"), DefaultValue(0)]
        public HTuple MatchResultScales
        {
            get => matchResultScales;
            set
            {
                matchResultScales = value;   
            }
        }

        HTuple matchResultScores = 0;
        /// <summary>
        /// 模板匹配结果得分
        /// </summary>
        [Description("模板匹配结果得分"), DefaultValue(0)]
        public HTuple MatchResultScores
        {
            get => matchResultScores;
            set
            {
                matchResultScores = value;    
            }
        }


        HTuple affinneTranMatix = null;
        /// <summary>
        /// 仿射变换矩阵
        /// </summary>
        [Description("仿射变换矩阵"), DefaultValue(null)]
        public HTuple AffinneTranMatix
        {
            get => affinneTranMatix;
            set
            {
                affinneTranMatix = value;
            }
        }
        //Run status value define----1
        bool matchRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool MatchRunStatus
        {
            get => matchRunStatus;
            set
            {
                matchRunStatus = value;   
            }
        }
        #endregion

    }
    /// <summary>
    ///模板搜索区域
    /// </summary>
    public enum EumModelSearch
    {
        全图搜索,
        局部搜索,
    }
}
