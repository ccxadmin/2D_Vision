using ControlShareResources.Common;
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

namespace MainFormLib.Models
{
    public class VisionModel : NotifyBase
    {

       public VisionModel()
        {
            foreach (EumModelType s  in Enum.GetValues(typeof(EumModelType))) 
            { modelTypeList.Add(s); }
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
        //private EumMenuItemEnable menuItemEnable = EumMenuItemEnable.all;

        //public EumMenuItemEnable MenuItemEnable
        //{
        //    get { return menuItemEnable; }
        //    set
        //    {
        //        menuItemEnable = value;
        //        DoNotify();
        //    }
        //}
        



        public ObservableCollection<RecipeDg> recipeDgList 
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


        public ObservableCollection<EumModelType> modelTypeList
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


        public ObservableCollection<ListViewToolsOfPositionData> toolsOfPositionList
                                 = new ObservableCollection<ListViewToolsOfPositionData>();
        public ObservableCollection<ListViewToolsOfPositionData> ToolsOfPositionList
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
    #endregion

    #region---------数据类型-------------
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

    /// <summary>
    /// 当前相机工作方式
    /// </summary>
    public enum EunmcurrCamWorkStatus
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
        GlueAOI,      
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
    /// <summary>
    /// 配方
    /// </summary>
    public class RecipeDg
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
    public class ListViewToolsOfPositionData : NotifyBase
    {
      
        public ListViewToolsOfPositionData(int _id, string _toolName,
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
