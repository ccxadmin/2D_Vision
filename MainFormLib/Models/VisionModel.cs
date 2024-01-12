﻿using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using GlueBaseTool = GlueDetectionLib.工具.BaseTool;
using GlueDataManage = GlueDetectionLib.DataManage;
using GlueaseParam =GlueDetectionLib.参数.BaseParam;
using GlueRunResult = GlueDetectionLib.工具.RunResult;

using PosBaseTool = PositionToolsLib.工具.BaseTool;
using PosDataManage = PositionToolsLib.DataManage;
using PosBaseParam = PositionToolsLib.参数.BaseParam;
using PosRunResult = PositionToolsLib.工具.RunResult;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using HalconDotNet;
using GlueDetectionLib;
using FunctionLib.Cam;

namespace MainFormLib.Models
{
    public class VisionModel : NotifyBase
    {

       public VisionModel()
        {
            foreach (EumModelType s  in Enum.GetValues(typeof(EumModelType))) 
            { modelTypeList.Add(s); }
            foreach (CamType s in Enum.GetValues(typeof(CamType)))
            { CamTypeList.Add(s); }
        }
        
        /// <summary>
        /// 相机类型源集合
        /// </summary>
        private ObservableCollection<CamType> camTypeList = new ObservableCollection<CamType>();
        public ObservableCollection<CamType> CamTypeList
        {
            get { return this.camTypeList; }
            set
            {
                camTypeList = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择相机类型名称
        /// </summary>
        private string selectCamTypeName;
        public string SelectCamTypeName
        {
            get { return this.selectCamTypeName; }
            set
            {
                selectCamTypeName = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择相机类型索引编号
        /// </summary>
        private int selectCamTypeIndex;
        public int SelectCamTypeIndex
        {
            get { return this.selectCamTypeIndex; }
            set
            {
                selectCamTypeIndex = value;
                DoNotify();
            }
        }

      
        private ObservableCollection<int> camIndexerList = new ObservableCollection<int>();
        public ObservableCollection<int> CamIndexerList
        {
            get { return this.camIndexerList; }
            set
            {
                camIndexerList = value;
                DoNotify();
            }
        }
      
        private string selectCamIndexerName;
        public string SelectCamIndexerName
        {
            get { return this.selectCamIndexerName; }
            set
            {
                selectCamIndexerName = value;
                DoNotify();
            }
        }
      
        private int selectCamIndexerIndex;
        public int SelectCamIndexerIndex
        {
            get { return this.selectCamIndexerIndex; }
            set
            {
                selectCamIndexerIndex = value;
                DoNotify();
            }
        }

        public long expouseMaxValue;
        public long ExpouseMaxValue
        {
            get { return this.expouseMaxValue; }
            set
            {
                expouseMaxValue = value;
                DoNotify();
            }
        }

        public long expouseMinValue;
        public long ExpouseMinValue
        {
            get { return this.expouseMinValue; }
            set
            {
                expouseMinValue = value;
                DoNotify();
            }
        }

        public long expouseSliderValue=1000;
        public long ExpouseSliderValue
        {
            get { return this.expouseSliderValue; }
            set
            {
                expouseSliderValue = value;
                DoNotify();
            }
        }

        public long expouseNumricValue = 1000;
        public long ExpouseNumricValue
        {
            get { return this.expouseNumricValue; }
            set
            {
                expouseNumricValue = value;
                DoNotify();
            }
        }
        private Action expouseNumericCommand;
        public Action ExpouseNumericCommand
        {
            get { return expouseNumericCommand; }
            set
            {
                expouseNumericCommand = value;
                DoNotify();
            }
        }

        private bool isCamAlive;
        public bool IsCamAlive
        {
            get { return this.isCamAlive; }
            set
            {
                isCamAlive = value;
                DoNotify();
            }
        }
        private bool btnOpenCamEnable;
        public bool BtnOpenCamEnable
        {
            get { return this.btnOpenCamEnable; }
            set
            {
                btnOpenCamEnable = value;
                DoNotify();
            }
        }
        private bool btnCloseCamEnable;
        public bool BtnCloseCamEnable
        {
            get { return this.btnCloseCamEnable; }
            set
            {
                btnCloseCamEnable = value;
                DoNotify();
            }
        }
        private bool btnOneShotEnable;
        public bool BtnOneShotEnable
        {
            get { return this.btnOneShotEnable; }
            set
            {
                btnOneShotEnable = value;
                DoNotify();
            }
        }
        private bool btnContinueGrabEnable;
        public bool BtnContinueGrabEnable
        {
            get { return this.btnContinueGrabEnable; }
            set
            {
                btnContinueGrabEnable = value;
                DoNotify();
            }
        }
        private bool btnStopGrabEnable;
        public bool BtnStopGrabEnable
        {
            get { return this.btnStopGrabEnable; }
            set
            {
                btnStopGrabEnable = value;
                DoNotify();
            }
        }

        public int gainMaxValue;
        public int GainMaxValue
        {
            get { return this.gainMaxValue; }
            set
            {
                gainMaxValue = value;
                DoNotify();
            }
        }

        public int gainMinValue;
        public int GainMinValue
        {
            get { return this.gainMinValue; }
            set
            {
                gainMinValue = value;
                DoNotify();
            }
        }

        public int gainSliderValue;
        public int GainSliderValue
        {
            get { return this.gainSliderValue; }
            set
            {
                gainSliderValue = value;
                DoNotify();
            }
        }

        public int gainNumricValue ;
        public int GainNumricValue
        {
            get { return this.gainNumricValue; }
            set
            {
                gainNumricValue = value;
                DoNotify();
            }
        }

        private int numDeviationThd;
        public int NumDeviationThd
        {
            get { return this.numDeviationThd; }
            set
            {
                numDeviationThd = value;
                DoNotify();
            }
        }
        private int numLimitMethd;
        public int NumLimitMethd
        {
            get { return this.numLimitMethd; }
            set
            {
                numLimitMethd = value;
                DoNotify();
            }
        }


        private Action gainNumericCommand;
        public Action GainNumericCommand
        {
            get { return gainNumericCommand; }
            set
            {
                gainNumericCommand = value;
                DoNotify();
            }
        }

        private Action deviationThdNumericCommand;
        public Action DeviationThdNumericCommand
        {
            get { return deviationThdNumericCommand; }
            set
            {
                deviationThdNumericCommand = value;
                DoNotify();
            }
        }
        private Action limitMethdNumericCommand;
        public Action LimitMethdNumericCommand
        {
            get { return limitMethdNumericCommand; }
            set
            {
                limitMethdNumericCommand = value;
                DoNotify();
            }
        }

        private EumModelType modelType = EumModelType.ProductModel_1;

        public EumModelType ModelType
        {
            get { return modelType; }
            set
            {
                modelType = value;
                DoNotify();
            }
        }
         
        private ObservableCollection<RecipeDg> recipeDgList 
                                  = new ObservableCollection<RecipeDg>();
        public ObservableCollection<RecipeDg> RecipeDgList
        {
            get { return this.recipeDgList; }
            set
            {
                recipeDgList = value;
                DoNotify();
            }
        }

        private int recipeDgSelectIndex;
        public int RecipeDgSelectIndex
        {
            get { return this.recipeDgSelectIndex; }
            set
            {
                recipeDgSelectIndex = value;
                DoNotify();
            }
        }

        private ObservableCollection<EumModelType> modelTypeList
                               = new ObservableCollection<EumModelType>();
        public ObservableCollection<EumModelType> ModelTypeList
        {
            get { return this.modelTypeList; }
            set
            {
                modelTypeList = value;
                DoNotify();
            }
        }

        private string modelTypeSelectName;
        public string ModelTypeSelectName
        {
            get { return this.modelTypeSelectName; }
            set
            {
                modelTypeSelectName = value;
                DoNotify();
            }
        }

        private int modelTypeSelectIndex;
        public int ModelTypeSelectIndex
        {
            get { return this.modelTypeSelectIndex; }
            set
            {
                modelTypeSelectIndex = value;
                DoNotify();
            }
        }


        private ObservableCollection<ListViewToolsData> toolsOfPositionList
                                 = new ObservableCollection<ListViewToolsData>();
        public ObservableCollection<ListViewToolsData> ToolsOfPositionList
        {
            get { return this.toolsOfPositionList; }
            set
            {
                toolsOfPositionList = value;
                DoNotify();
            }
        }

        private int toolsOfPositionSelectIndex=-1;
        public int ToolsOfPositionSelectIndex
        {
            get { return this.toolsOfPositionSelectIndex; }
            set
            {
                toolsOfPositionSelectIndex = value;
                DoNotify();
            }
        }

        private ObservableCollection<ListViewToolsData> toolsOfGlueList
                                = new ObservableCollection<ListViewToolsData>();
        public ObservableCollection<ListViewToolsData> ToolsOfGlueList
        {
            get { return this.toolsOfGlueList; }
            set
            {
                toolsOfGlueList = value;
                DoNotify();
            }
        }

        private int toolsOfGlueSelectIndex = -1;
        public int ToolsOfGlueSelectIndex
        {
            get { return this.toolsOfGlueSelectIndex; }
            set
            {
                toolsOfGlueSelectIndex = value;
                DoNotify();
            }
        }



        /// <summary>
        /// 富文本信息
        /// </summary>
        private string richIfo;
        public string RichIfo
        {
            get { return richIfo; }
            set
            {
                richIfo = value;
                DoNotify();
            }
        }

        /// <summary>
        /// 文本信息清除
        /// </summary>
        private bool clearRichText;
        public bool ClearRichText
        {
            get { return clearRichText; }
            set
            {
                clearRichText = value;
                DoNotify();
            }
        }
    }


    #region ------------Converter----------
    public class ComparisonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals(parameter)) return Visibility.Visible;
            else return Visibility.Hidden;
            //return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b) return parameter;
            return Binding.DoNothing;
        }
    }

    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "none") return false;
            else if (value.ToString() == "all") return true;
            else
                return value.ToString() !=parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b) return parameter;
            return Binding.DoNothing;
        }
    }
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b) return parameter;
            return Binding.DoNothing;
        }
    }

    #endregion

    #region---------数据类型-------------
    [Serializable]
    public struct GlueRecheckDat
    {
        public GlueRecheckDat(double x, double y, double area)
        {
            X = x;
            Y = y;
            Area = area;
        }
        public double X;
        public double Y;
        public double Area;
    }

    public struct StuWindowInfoToPaint
    {
        public CoorditionDat coorditionDat;
        public string Info;
        public string color;
        public int size;
    }
    public struct StuWindowHobjectToPaint
    {
        public HObject obj;
        public string color;
    }

    public enum EumMenuItemEnable
    {
        first,
        last,     
        none,
        all
    }

    /// <summary>
    /// 当前相机工作方式
    /// </summary>
    public enum EunmCamWorkStatus
    {
        /// <summary>
        /// 自由模式
        /// </summary>
        Freestyle,
        /// <summary>
        /// 9点标定
        /// </summary>
        NinePointcLocation,
        /// <summary>
        /// 旋转标定
        /// </summary>
        RotatoLocation,
        /// <summary>
        /// 偏差标定
        /// </summary>
        DeviationLocation,
        /// <summary>
        /// 产品点位1测试
        /// </summary>
        NormalTest_T1,
        /// <summary>
        /// 产品点位2测试
        /// </summary>
        NormalTest_T2,
        /// <summary>
        /// 胶水测试
        /// </summary>
        NormalTest_G,
        /// <summary>
        /// 自动对焦
        /// </summary>
        AutoFocus,
        /// <summary>
        /// 胶水检测
        /// </summary>
        AOI,      
        /// <summary>
        /// 轨迹提取
        /// </summary>
        Trajectory,
        /// <summary>
        /// 无
        /// </summary>
        None
    }
    /// <summary>
    /// 员工操作权限
    /// </summary>
    public enum EumOperationAuthority
    {
        [Description("无")]
        None = 0,
        [Description("操作员")]
        Operator = 1,
        [Description("程序员")]
        Programmer,
        [Description("管理员")]
        Administrators
    }
    /// <summary>
    /// 辅助工具
    /// </summary>
    public enum EumAssistTool
    {
        None,
        Circle,
        Rectangle

    }

    /// <summary>
    /// 模板类型
    /// </summary>
    public enum EumModelType
    {
 
        ProductModel_1,  //当前为产品1模板 ,default

        ProductModel_2,  //当前为产品1模板 ,default

        CaliBoardModel, //当前为标定板模板

        GluetapModel   //点胶阀
    }

    [Serializable]
    /// <summary>
    /// 配方
    /// </summary>
    public class RecipeDg: NotifyBase
    {
        public RecipeDg()
        {
            Name = "default";
            IsUse = false;
        }
        public RecipeDg(string name, bool isUse)
        {
            Name = name;
            IsUse = isUse;
        }
        private string name;
        public string Name { 
            get=>this.name;
            set { this.name = value; DoNotify(); }
        }

        private bool isUse;
        public bool IsUse {
            get=>this.isUse;
            set { this.isUse = value; DoNotify(); }
        }

     
    }

    [Serializable]
    /// <summary>
    /// 配方
    /// </summary>
    public class RecipeDgBuf 
    {
        public RecipeDgBuf()
        {
            Name = "default";
            IsUse = false;
        }
        public RecipeDgBuf(string name, bool isUse)
        {
            Name = name;
            IsUse = isUse;
        }


        public string Name { get; set; }


        public bool IsUse { get; set; }

    }
    /// <summary>
    /// 胶水检测工程文件
    /// </summary>
    [Serializable]
    public class ProjectOfGlue
    {

        public List<string> toolNamesList = new List<string>();
        public Dictionary<string, GlueBaseTool> toolsDic = new Dictionary<string, GlueBaseTool>();
        [NonSerialized]
        public GlueDataManage dataManage = new GlueDataManage();
        public void Refresh()
        {
            Dictionary<string, GlueBaseTool> tools = new Dictionary<string, GlueBaseTool>();

            foreach (var s in toolNamesList)
                tools.Add(s, toolsDic[s]);

            toolsDic = tools;
        }
    }
    /// <summary>
    /// 定位检测工程文件
    /// </summary>
    [Serializable]
    public class ProjectOfPosition
    {

        public List<string> toolNamesList = new List<string>();
        public Dictionary<string, PosBaseTool> toolsDic = new Dictionary<string, PosBaseTool>();
        [NonSerialized]
        public PosDataManage dataManage = new PosDataManage();
        public void Refresh()
        {
            Dictionary<string, PosBaseTool> tools = new Dictionary<string, PosBaseTool>();

            foreach (var s in toolNamesList)
                tools.Add(s, toolsDic[s]);

            toolsDic = tools;
        }

    }

    [Serializable]
    public class ListViewToolsData : NotifyBase
    {
      
        public ListViewToolsData(int _id, string _toolName,
            string _toolStatus,string _toolNotes)
        {
            ID = _id;
            ToolName = _toolName;
            ToolStatus = _toolStatus;
            ToolNotes = _toolNotes;
      
        }
        private int id;
        public int ID {
            get { return this.id; }
            set
            {
                id = value;
                DoNotify();
            }

        }
        private string toolName;
        public string ToolName {

            get { return this.toolName; }
            set
            {
                toolName = value;
                DoNotify();
            }
        }
        private string toolStatus;
        public string ToolStatus {

            get { return this.toolStatus; }
            set
            {
                toolStatus = value;
                DoNotify();
            }

        }
        private string toolNotes;
        public string ToolNotes {

            get { return this.toolNotes; }
            set
            {
                toolNotes = value;
                DoNotify();
            }

        }
        private EumMenuItemEnable menuItemEnable = EumMenuItemEnable.all;
        public EumMenuItemEnable MenuItemEnable {

            get { return this.menuItemEnable; }
            set
            {
                menuItemEnable = value;
                DoNotify();
            }

        }
        //public bool ToolsOfPosition_ContextMenu_FirstEnable { get; set; }

        //public bool ToolsOfPosition_ContextMenu_LastEnable { get; set; }

        //public bool ToolsOfPosition_ContextMenu_AllEnable { get; set; }
    }


    #endregion
}
