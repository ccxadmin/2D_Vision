using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//using GlueBaseTool = GlueDetectionLib.工具.BaseTool;
//using GlueDataManage = GlueDetectionLib.DataManage;
//using GlueaseParam =GlueDetectionLib.参数.BaseParam;
//using GlueRunResult = GlueDetectionLib.工具.RunResult;

using BaseTool = PositionToolsLib.工具.BaseTool;
using DataManage = PositionToolsLib.DataManage;
using BaseParam = PositionToolsLib.参数.BaseParam;
using RunResult = PositionToolsLib.工具.RunResult;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using HalconDotNet;
using FunctionLib.Cam;
using PositionToolsLib.工具;
using PositionToolsLib.窗体.Models;
using PositionToolsLib;

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
            foreach (EumOutputType s in Enum.GetValues(typeof(EumOutputType)))
            { OutputTypeList.Add(s); }

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


        private EumOutputType outputType = EumOutputType.Location ;

        public EumOutputType OutputType
        {
            get { return outputType; }
            set
            {
                outputType = value;
                DoNotify();
            }
        }

        private ObservableCollection<EumOutputType> outputTypeList
                              = new ObservableCollection<EumOutputType>();
        public ObservableCollection<EumOutputType> OutputTypeList
        {
            get { return this.outputTypeList; }
            set
            {
                outputTypeList = value;
                DoNotify();
            }
        }

        private string outputTypeSelectName;
        public string OutputTypeSelectName
        {
            get { return this.outputTypeSelectName; }
            set
            {
                outputTypeSelectName = value;
                DoNotify();
            }
        }

        private int outputTypeSelectIndex;
        public int OutputTypeSelectIndex
        {
            get { return this.outputTypeSelectIndex; }
            set
            {
                outputTypeSelectIndex = value;
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

        private EumAssistTool assistTool = EumAssistTool.None;
        public EumAssistTool AssistTool
        {
            get { return this.assistTool; }
            set
            {
                assistTool = value;
                DoNotify();
            }
        }

        private double assistCircleRadius = 1;
        public double AssistCircleRadius
        {
            get { return this.assistCircleRadius; }
            set
            {
                assistCircleRadius = value;
                DoNotify();
            }
        }

        private double assistRectWidth = 1;
        public double AssistRectWidth
        {
            get { return this.assistRectWidth; }
            set
            {
                assistRectWidth = value;
                DoNotify();
            }
        }
        private double numOfScale = 1;
        public double NumOfScale
        {
            get { return this.numOfScale; }
            set
            {
                numOfScale = value;
                DoNotify();
            }
        }
        
        private Action assistCircleCommand;
        public Action AssistCircleCommand
        {
            get { return assistCircleCommand; }
            set
            {
                assistCircleCommand = value;
                DoNotify();
            }
        }

        private bool continueRunFlag;
        public bool ContinueRunFlag
        {
            get { return continueRunFlag; }
            set
            {
                continueRunFlag = value;
                DoNotify();
            }
        }

        private bool showCross;
        public bool ShowCross
        {
            get { return showCross; }
            set
            {
                showCross = value;
                DoNotify();
            }
        }
        private bool showScaleRule;
        public bool ShowScaleRule
        {
            get { return showScaleRule; }
            set
            {
                showScaleRule = value;
                DoNotify();
            }
        }

        private bool cobxCamTypeEnable;
        public bool CobxCamTypeEnable
        {
            get { return cobxCamTypeEnable; }
            set
            {
                cobxCamTypeEnable = value;
                DoNotify();
            }
        }
        private bool cobxCamIndexerEnable;
        public bool CobxCamIndexerEnable
        {
            get { return cobxCamIndexerEnable; }
            set
            {
                cobxCamIndexerEnable = value;
                DoNotify();
            }
        }
        private bool posListViewEnable=true;
        public bool PosListViewEnable
        {
            get { return posListViewEnable; }
            set
            {
                posListViewEnable = value;
                DoNotify();
            }
        }

        private bool glueListViewEnable=true;
        public bool GlueListViewEnable
        {
            get { return glueListViewEnable; }
            set
            {
                glueListViewEnable = value;
                DoNotify();
            }
        }
        private bool posToolBarEnable = true;
        public bool PosToolBarEnable
        {
            get { return posToolBarEnable; }
            set
            {
                posToolBarEnable = value;
                DoNotify();
            }
        }
        private bool glueToolBarEnable = true;
        public bool GlueToolBarEnable
        {
            get { return glueToolBarEnable; }
            set
            {
                glueToolBarEnable = value;
                DoNotify();
            }
        }

        private int width = 350;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                DoNotify();
            }
        }

        /***********线扫相机**************/
        /// <summary>
        ///选择线扫相机类型索引编号
        /// </summary>
        private int scanCamTypeSelectIndex;
        public int ScanCamTypeSelectIndex
        {
            get { return this.scanCamTypeSelectIndex; }
            set
            {
                scanCamTypeSelectIndex = value;
                DoNotify();
            }
        }
        private string txbDeviceName;
        public string TxbDeviceName
        {
            get { return this.txbDeviceName; }
            set
            {
                txbDeviceName = value;
                DoNotify();
            }
        }

        private string txbConfigPath;
        public string TxbConfigPath
        {
            get { return this.txbConfigPath; }
            set
            {
                txbConfigPath = value;
                DoNotify();
            }
        }
        public long scanExpouseMaxValue;
        public long ScanExpouseMaxValue
        {
            get { return this.scanExpouseMaxValue; }
            set
            {
                scanExpouseMaxValue = value;
                DoNotify();
            }
        }

        public long scanExpouseMinValue;
        public long ScanExpouseMinValue
        {
            get { return this.scanExpouseMinValue; }
            set
            {
                scanExpouseMinValue = value;
                DoNotify();
            }
        }

        public long sliderExpouseOfScan = 50;
        public long SliderExpouseOfScan
        {
            get { return this.sliderExpouseOfScan; }
            set
            {
                sliderExpouseOfScan = value;
                DoNotify();
            }
        }
        private long numExpouseOfScan;
        public long NumExpouseOfScan
        {
            get { return this.numExpouseOfScan; }
            set
            {
                numExpouseOfScan = value;
                DoNotify();
            }
        }

        public int scanGainMaxValue;
        public int ScanGainMaxValue
        {
            get { return this.scanGainMaxValue; }
            set
            {
                scanGainMaxValue = value;
                DoNotify();
            }
        }

        public int scanGainMinValue;
        public int ScanGainMinValue
        {
            get { return this.scanGainMinValue; }
            set
            {
                scanGainMinValue = value;
                DoNotify();
            }
        }

        public int sliderGainOfScan;
        public int SliderGainOfScan
        {
            get { return this.sliderGainOfScan; }
            set
            {
                sliderGainOfScan = value;
                DoNotify();
            }
        }
        private int numGainOfScan;
        public int NumGainOfScan
        {
            get { return this.numGainOfScan; }
            set
            {
                numGainOfScan = value;
                DoNotify();
            }
        }

        private Action scanExpouseNumericCommand;
        public Action ScanExpouseNumericCommand
        {
            get { return scanExpouseNumericCommand; }
            set
            {
                scanExpouseNumericCommand = value;
                DoNotify();
            }
        }
        private Action scanGainNumericCommand;
        public Action ScanGainNumericCommand
        {
            get { return scanGainNumericCommand; }
            set
            {
                scanGainNumericCommand = value;
                DoNotify();
            }
        }
       
        private bool btnOpenScanCameraEnable=true;
        public bool BtnOpenScanCameraEnable
        {
            get { return this.btnOpenScanCameraEnable; }
            set
            {
                btnOpenScanCameraEnable = value;
                DoNotify();
            }
        }
        private bool btnCloseScanCameraEnable;
        public bool BtnCloseScanCameraEnable
        {
            get { return this.btnCloseScanCameraEnable; }
            set
            {
                btnCloseScanCameraEnable = value;
                DoNotify();
            }
        }
        private bool btnGrabScanCameraEnable;
        public bool BtnGrabScanCameraEnable
        {
            get { return this.btnGrabScanCameraEnable; }
            set
            {
                btnGrabScanCameraEnable = value;
                DoNotify();
            }
        }

        private bool btnShotScanCameraEnable;
        public bool BtnShotScanCameraEnable
        {
            get { return this.btnShotScanCameraEnable; }
            set
            {
                btnShotScanCameraEnable = value;
                DoNotify();
            }
        }

        private string btnGrabContent="Grab";
        public string BtnGrabContent
        {
            get { return this.btnGrabContent; }
            set
            {
                btnGrabContent = value;
                DoNotify();
            }
        }
        private bool frameCamEnable;
        public bool FrameCamEnable
        {
            get { return this.frameCamEnable; }
            set
            {
                frameCamEnable = value;
                DoNotify();
            }
        }
        private bool scanCamEnable;
        public bool ScanCamEnable
        {
            get { return this.scanCamEnable; }
            set
            {
                scanCamEnable = value;
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
    public class VisibConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals(parameter)) return Visibility.Visible;
            else return Visibility.Collapsed;
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


    public class CheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
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

    public enum EumUsingCamType
    { 
      Frame,
      Scan
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
        /// 尺寸测量
        /// </summary>
        Size,
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


    public enum EumOutputType
    {
        Location,    //定位
        Size,       //尺寸
        Trajectory,  //轨迹
        AOI     //AOI
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

    public enum EumScanCamType
    {
        Gige,
        CameraLink

    }
    /// <summary>
    /// 线扫相机参数
    /// </summary>
    [Serializable]
    public class CameraParamOfScan
    {
        /// <summary>
        /// 相机连接方式
        /// </summary>
        public EumScanCamType camType = EumScanCamType.Gige;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName = "";
        /// <summary>
        /// 曝光
        /// </summary>
        public long expouse = 50;
        /// <summary>
        /// 增益
        /// </summary>
        public int Gain = 0;
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public string configPath = "";
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
    /// 视觉检测工程文件
    /// </summary>
    [Serializable]
    public class Project
    {

        public List<string> toolNamesList = new List<string>();
        public Dictionary<string, BaseTool> toolsDic = new Dictionary<string, BaseTool>();
        [NonSerialized]
        public DataManage dataManage = new DataManage();
        [NonSerialized]
        int[] nums ;
        public string TcpRecvName;
        public string TcpSendName;
        public void Refresh()
        {
            Dictionary<string, BaseTool> tools = new Dictionary<string, BaseTool>();

            foreach (var s in toolNamesList)
                tools.Add(s, toolsDic[s]);

            toolsDic = tools;
        }
        public void GetNum()
        {
            nums = new int[29];
            nums[0] = AngleConvertTool.inum;    
            nums[1] = BinaryzationTool.inum ;
            nums[2] = BlobTool.inum ;
            nums[3] = CalParallelLineTool.inum ;
            nums[4] = ClosingTool.inum ;
            nums[5] = ColorConvertTool.inum ;
            nums[6] = CoordConvertTool.inum;
            nums[7] = DilationTool.inum ;
            nums[8] = DistanceLLTool.inum;
            nums[9] =DistancePLTool.inum;
            nums[10] = DistancePPTool.inum;
            nums[11] = ErosionTool.inum ;
            nums[12] = FindCircleTool.inum ;
            nums[13] = FindLineTool.inum ;
            nums[14] = FitLineTool.inum ;
            nums[15] = GlueCaliperWidthTool.inum;
            nums[16] = GlueGapTool.inum;
            nums[17] = GlueMissTool.inum;
            nums[18] = GlueOffsetTool.inum;
            nums[19] = ImageCorrectTool.inum;
            nums[20] = LineCentreTool.inum ;
            nums[21] = LineIntersectionTool.inum ;
            nums[22] = LineOffsetTool.inum;
            nums[23] = MatchTool.inum ;
            nums[24] = OpeningTool.inum ;
            nums[25] = ResultShowTool.inum ;
            nums[26] = TcpRecvTool.inum;
            nums[27] = TcpSendTool.inum;
            nums[28] = TrajectoryExtractTool.inum;
        }
        public void SetNum()
        {
            if (nums == null) return;
            AngleConvertTool.inum = nums[0];
            BinaryzationTool.inum = nums[1];
            BlobTool.inum = nums[2];
            CalParallelLineTool.inum = nums[3];
            ClosingTool.inum = nums[4];
            ColorConvertTool.inum = nums[5];
            CoordConvertTool.inum = nums[6];
            DilationTool.inum = nums[7];
            DistanceLLTool.inum = nums[8];
            DistancePLTool.inum = nums[9];
            DistancePPTool.inum = nums[10];
            ErosionTool.inum = nums[11];
            FindCircleTool.inum = nums[12];
            FindLineTool.inum = nums[13];
            FitLineTool.inum = nums[14];
            GlueCaliperWidthTool.inum=nums[15];
            GlueGapTool.inum=nums[16] ;
            GlueMissTool.inum=nums[17] ;
            GlueOffsetTool.inum=nums[18] ;
            ImageCorrectTool.inum = nums[19];
            LineCentreTool.inum = nums[20];
            LineIntersectionTool.inum = nums[21];
            LineOffsetTool.inum = nums[22];
            MatchTool.inum = nums[23];
            OpeningTool.inum = nums[24];
            ResultShowTool.inum = nums[25];
            TcpRecvTool.inum = nums[26];
            TcpSendTool.inum = nums[27];
            TrajectoryExtractTool.inum = nums[28];
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
        private Visibility contextMenuVisib = Visibility.Visible;
        public Visibility ContextMenuVisib
        {
            get { return contextMenuVisib; }
            set
            {
                contextMenuVisib = value;
                DoNotify();
            }
        }

        //public bool ToolsOfPosition_ContextMenu_FirstEnable { get; set; }

        //public bool ToolsOfPosition_ContextMenu_LastEnable { get; set; }

        //public bool ToolsOfPosition_ContextMenu_AllEnable { get; set; }
    }


    #endregion
}
