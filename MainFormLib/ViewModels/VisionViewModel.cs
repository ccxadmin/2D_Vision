using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ControlShareResources.Common;
using HalconDotNet;
using MainFormLib.Models;
using MainFormLib.Views;
using VisionShowLib.UserControls;

using GlueBaseTool = GlueDetectionLib.工具.BaseTool;
using GlueDataManage = GlueDetectionLib.DataManage;
using GlueaseParam = GlueDetectionLib.参数.BaseParam;
using GlueRunResult = GlueDetectionLib.工具.RunResult;

using PosBaseTool = PositionToolsLib.工具.BaseTool;
using PosDataManage = PositionToolsLib.DataManage;
using PosBaseParam = PositionToolsLib.参数.BaseParam;
using PosRunResult = PositionToolsLib.工具.RunResult;


using System.Diagnostics;
using PositionToolsLib.窗体.Views;
using PositionToolsLib.窗体.ViewModels;
using PositionToolsLib.参数;
using LightSourceController.Models;
using FunctionLib;
using System.IO;
using OSLog;
using FilesRAW.Common;
using System.Windows;
using GlueDetectionLib;
using FunctionLib.Cam;
using PositionToolsLib;
using FunctionLib.Location;
using System.Drawing.Imaging;
using System.Threading;

namespace MainFormLib.ViewModels
{
    public class VisionViewModel
    {
        Log log;
        VirtualConnect virtualConnect = null;
        public delegate void GetDataHandle(string data);
        public event GetDataHandle GetDataOfCaliHandle = null;
        public delegate void CamContinueGrabHandle(bool isGrabing);
        public CamContinueGrabHandle camContinueGrabHandle;//相机是否连续采集状态
        public delegate void ImageSizeChangeHandle(int width, int height);
        public event ImageSizeChangeHandle imageSizeChangeHandle; //图像旋转后尺寸变化通知事件
        /*-----------------------------------------文件配置---------------------------------------*/
        private string rootFolder = AppDomain.CurrentDomain.BaseDirectory; //根目录
        private string currCamStationName = "cam1";
        string currCalibName = "default";// 当前标定文件名称                                           
        string saveToUsePath = AppDomain.CurrentDomain.BaseDirectory + "配方\\default";// 配方文件夹路径
        /*-----------------------------------------定位工具---------------------------------------*/
        EumModelType currModelType = EumModelType.ProductModel_1;
        int toolindexofPosition = -1;
        Stopwatch stopwatch = new Stopwatch();
        //工程文件
        private ProjectOfPosition projectOfPos = new ProjectOfPosition();
        //区域
        List<StuWindowHobjectToPaint> objs = new List<StuWindowHobjectToPaint>();
        //文本
        List<StuWindowInfoToPaint> infos = new List<StuWindowInfoToPaint>();

        private HTuple hv_HomMat2D = null;//坐标系变换矩阵
        /*-----------------------------------------胶水工具---------------------------------------*/
        //工程文件
        private ProjectOfGlue projectOfGlue = new ProjectOfGlue();

        /*-----------------------------------------其他工具---------------------------------------*/
        HObject GrabImg = null;
        HObject imgBuf = null;//图像缓存     
        FormNinePointsCalib f_NinePointsCalib = null;
        //public static VisionViewModel This { get; set; }
        public VisionModel Model { get; set; }
        public VisionShowTool ShowTool { get; set; }
        public Icam CurrCam = null;  //相机接口
        EunmcurrCamWorkStatus workstatus = EunmcurrCamWorkStatus.None;//当前相机工作状态



        #region Command
        public CommandBase WindowsLoadedCommand { get; set; }
        public CommandBase NinePointsCalibFormClickCommand { get; set; }
        public CommandBase ModelTypeSelectionChangedCommand { get; set; }
        public CommandBase PosToolBarBtnClickCommand { get; set; }
        public CommandBase PosMenuClickCommand { get; set; }
        public CommandBase ToolsOfPosDoubleClickCommand { get; set; }
        public CommandBase ToolsOfPosMouseUpCommand { get; set; }
        public CommandBase ToolsOfPosition_ContextMenuCommand { get; set; }




        #endregion
        private VisionViewModel()
        {

            HOperatorSet.GenEmptyObj(out imgBuf);
            //This = this;
            Model = new VisionModel();
            //图像控件      
            ShowTool = new VisionShowTool();
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            ShowTool.ImageGetRotationHandle += new EventHandler(ImageGetRotationEvent);
            ShowTool.CamGrabHandle += new EventHandler(CamGrabEvent);


            #region Command
            WindowsLoadedCommand = new CommandBase();
            WindowsLoadedCommand.DoExecute = new Action<object>((o) => Windows_Load());
            WindowsLoadedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            NinePointsCalibFormClickCommand = new CommandBase();
            NinePointsCalibFormClickCommand.DoExecute = new Action<object>((o) => NinePointsCalibForm_Click());
            NinePointsCalibFormClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ModelTypeSelectionChangedCommand = new CommandBase();
            ModelTypeSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxModelTypeList_SelectedIndexChanged(o));
            ModelTypeSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            PosToolBarBtnClickCommand = new CommandBase();
            PosToolBarBtnClickCommand.DoExecute = new Action<object>((o) => PosToolBarBtn_Click(o));
            PosToolBarBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            PosMenuClickCommand = new CommandBase();
            PosMenuClickCommand.DoExecute = new Action<object>((o) => PosMenu_Click(o));
            PosMenuClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ToolsOfPosDoubleClickCommand = new CommandBase();
            ToolsOfPosDoubleClickCommand.DoExecute = new Action<object>((o) => ListViewToolsOfPosition_DoubleClick(o));
            ToolsOfPosDoubleClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ToolsOfPosMouseUpCommand = new CommandBase();
            ToolsOfPosMouseUpCommand.DoExecute = new Action<object>((o) => ListViewToolsOfPosition_MouseUp());
            ToolsOfPosMouseUpCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ToolsOfPosition_ContextMenuCommand = new CommandBase();
            ToolsOfPosition_ContextMenuCommand.DoExecute = new Action<object>((o) => ToolsOfPosition_ContextMenuClick(o));
            ToolsOfPosition_ContextMenuCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            #endregion

        }
        public VisionViewModel(string camStationName = "camStationName_1") : this()
        {
            ShowTool.TitleName = currCamStationName = camStationName;
            log = new Log(camStationName);
            //setOperationAuthority();       
            virtualConnect = new VirtualConnect("虚拟连接" + camStationName);
            BuiltConnect();

            rootFolder = AppDomain.CurrentDomain.BaseDirectory + camStationName;
            if (!Directory.Exists(rootFolder))
                Directory.CreateDirectory(rootFolder);
            if (!Directory.Exists(rootFolder + "\\Config"))
                Directory.CreateDirectory(rootFolder + "\\Config");
            if (!Directory.Exists(rootFolder + "\\配方"))
                Directory.CreateDirectory(rootFolder + "\\配方");

        }


        #region-------------窗体事件---------------
        /// <summary>
        /// 窗体加载
        /// </summary>
        void Windows_Load()
        {

        }
      
        /// <summary>
        /// 九点标定窗体显示
        /// </summary>
        void NinePointsCalibForm_Click()
        {

            //f_NinePointsCalib.BringIntoView();
            f_NinePointsCalib.Show();

        }
        /// <summary>
        /// 定位检测模板类型切换
        /// </summary>
        /// <param name="o"></param>
        void cobxModelTypeList_SelectedIndexChanged(object o)
        {
            if (Model.ModelType == EumModelType.CaliBoardModel)
                f_NinePointsCalib = new FormNinePointsCalib("");
        }
        /// <summary>
        /// 定位检测工具栏按钮事件
        /// </summary>
        /// <param name="o"></param>
        void PosToolBarBtn_Click(object o)
        {
            Button b = (Button)o;
            switch (b.ToolTip)
            {
                case "打开流程":
                    #region 打开流程
                    // Configure open file dialog box
                    var dialog = new Microsoft.Win32.OpenFileDialog();
                    dialog.FileName = "Open Project"; // Default file name
                    dialog.DefaultExt = ".proj"; // Default file extension
                    dialog.Filter = "Open Project (.proj)|*.proj"; // Filter files by extension

                    // Show open file dialog box
                    bool? result = dialog.ShowDialog();

                    // Process open file dialog box results
                    if (result == true)
                    {
                        // Open document
                        string path = dialog.FileName;
                        try
                        {
                            ResetNumOfPos();
                            this.projectOfPos = GeneralUse.ReadSerializationFile<ProjectOfPosition>(path);
                            projectOfPos.toolNamesList = new List<string>();
                            if (projectOfPos.dataManage == null)
                                projectOfPos.dataManage = new PosDataManage();
                            Dictionary<string, PosBaseTool> tem = new Dictionary<string, PosBaseTool>();

                            foreach (var s in projectOfPos.toolsDic)
                            {
                                projectOfPos.toolNamesList.Add(s.Value.GetToolName());
                                tem.Add(s.Value.GetToolName(), s.Value);
                                s.Value.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                            }
                            projectOfPos.toolsDic = tem;


                            ShowTestFlowOfPosition();
                        }
                        catch (Exception er)
                        {
                            MessageBox.Show("定位检测流程读取失败，异常信息:" + er.Message);
                        }
                    }
                    #endregion
                    break;
                case "保存流程":
                    #region 保存流程
                    //文件夹名称
                    string firstName = "定位检测";
                    string secondName = Enum.GetName(typeof(EumModelType), currModelType);
                    if (!Directory.Exists(saveToUsePath + "\\" + firstName))
                        Directory.CreateDirectory(saveToUsePath + "\\" + firstName);

                    try
                    {
                        projectOfPos.Refresh();
                        GeneralUse.WriteSerializationFile<ProjectOfPosition>(saveToUsePath + "\\"
                            + firstName + "\\" + secondName + ".proj",
                            projectOfPos);

                        GeneralUse.WriteValue("定位检测", "模板类型", secondName, "config",
                                      saveToUsePath + "\\" + firstName);
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("定位检测流程保存失败，异常信息:" + er.Message);
                    }
                    #endregion
                    break;
                case "运行流程":
                    if (CurrCam != null)
                        if (CurrCam.IsGrabing)
                            btnStopGrab_Click();  //如果已在采集中则先停止采集
                    Task.Run(delegate ()
                    { return RunTestFlowOfPosition(); });
                    break;
                case "清空流程":
                    if (MessageBox.Show("清空流程？", "提醒", MessageBoxButton.OKCancel, MessageBoxImage.Question)
               == MessageBoxResult.OK)
                    {
                        toolindexofPosition = -1;
                        projectOfPos.toolNamesList.Clear();
                        projectOfPos.toolsDic.Clear();
                        projectOfPos.dataManage?.ResetBuf();//重载后清除数据缓存
                        ResetNumOfPos();
                        Model.ToolsOfPositionList.Clear();
                    }
                    break;
                case "标定助手":
                    FormCalibAssistant f = new FormCalibAssistant("");
                    f.Show();
                    break;

            }

        }
        //相机停止采集
        private void btnStopGrab_Click()
        {          
            CurrCam.StopGrab();         
            camContinueGrabHandle?.Invoke(false);
        }

        /// <summary>
        /// 定位检测菜单栏按钮事件（新增定位检测工具）
        /// </summary>
        /// <param name="o"></param>
        void PosMenu_Click(object o)
        {
            toolindexofPosition++;
            PosBaseTool tool = null;
            string tip = o.ToString();

            switch (tip)
            {
                case "颜色转换":
                    tool = new PositionToolsLib.工具.ColorConvertTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "膨胀":
                    tool = new PositionToolsLib.工具.DilationTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "腐蚀":
                    tool = new PositionToolsLib.工具.ErosionTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "开运算":
                    tool = new PositionToolsLib.工具.OpeningTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "闭运算":
                    tool = new PositionToolsLib.工具.ClosingTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "二值化":
                    tool = new PositionToolsLib.工具.BinaryzationTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "模板匹配":
                    tool = new PositionToolsLib.工具.MatchTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "查找直线":
                    tool = new PositionToolsLib.工具.FindLineTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "拟合直线":
                    tool = new PositionToolsLib.工具.FitLineTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "直线偏移":
                    tool = new PositionToolsLib.工具.LineOffsetTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "平行直线":
                    tool = new PositionToolsLib.工具.CalParallelLineTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "直线中心":
                    tool = new PositionToolsLib.工具.LineCentreTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "直线交点":
                    tool = new PositionToolsLib.工具.LineIntersectionTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "查找圆":
                    tool = new PositionToolsLib.工具.FindCircleTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "Blob中心":
                    tool = new PositionToolsLib.工具.BlobTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "坐标换算":
                    tool = new PositionToolsLib.工具.CoordConvertTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "角度换算":
                    tool = new PositionToolsLib.工具.AngleConvertTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "畸变校正":
                    tool = new PositionToolsLib.工具.ImageCorrectTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
                case "点点距离":
                    break;
                case "点线距离":
                    break;
                case "线线距离":
                    break;
                case "轨迹提取":
                    break;
                case "结果显示":
                    tool = new PositionToolsLib.工具.ResultShowTool();
                    tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManage);
                    break;
            }
            projectOfPos.toolNamesList.Add(tool.GetToolName());
            projectOfPos.toolsDic.Add(tool.GetToolName(), tool);
            Model.ToolsOfPositionList.Add(new
                ListViewToolsOfPositionData(toolindexofPosition,
                tool.GetToolName(),
                "--", tool.remark));

        }
        /// <summary>
        /// 定位检测工具流程鼠标单击事件
        /// </summary>
        void ListViewToolsOfPosition_MouseUp()
        {

            int index = Model.ToolsOfPositionSelectIndex;
            Console.WriteLine(index);
            if (index < 0 ||
               index >= Model.ToolsOfPositionList.Count)
                return;
            if (Model.ToolsOfPositionList.Count == 1)
                Model.ToolsOfPositionList[index].MenuItemEnable = EumMenuItemEnable.none;
            else
            {

                if (index == 0)
                    Model.ToolsOfPositionList[index].MenuItemEnable = EumMenuItemEnable.first;
                else if (index == Model.ToolsOfPositionList.Count - 1)
                    Model.ToolsOfPositionList[index].MenuItemEnable = EumMenuItemEnable.last;
                else
                    Model.ToolsOfPositionList[index].MenuItemEnable = EumMenuItemEnable.all;
            }
        }
        /// <summary>
        /// 定位检测工具流程鼠标双击事件
        /// </summary>
        /// <param name="o"></param>
        void ListViewToolsOfPosition_DoubleClick(object o)
        {
            int index = Model.ToolsOfPositionSelectIndex;
            if (index < 0 ||
               index >= Model.ToolsOfPositionList.Count)
                return;
            string name = Model.ToolsOfPositionList[index].ToolName;
            string toolName = projectOfPos.toolNamesList[index];
            PosBaseTool tool = projectOfPos.toolsDic[toolName];
            if (toolName.Contains("颜色转换"))
            {
                FormColorConvert f = new PositionToolsLib.窗体.Views.FormColorConvert(tool);
                ColorConvertViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                ColorConvertViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("膨胀"))
            {
                FormDilation f = new PositionToolsLib.窗体.Views.FormDilation(tool);
                DilationViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                DilationViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("腐蚀"))
            {
                FormErosion f = new PositionToolsLib.窗体.Views.FormErosion(tool);
                ErosionViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                ErosionViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("开运算"))
            {
                FormOpening f = new PositionToolsLib.窗体.Views.FormOpening(tool);
                OpeningViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                OpeningViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("闭运算"))
            {
                FormClosing f = new PositionToolsLib.窗体.Views.FormClosing(tool);
                ClosingViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                ClosingViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("二值化"))
            {
                FormBinaryzation f = new PositionToolsLib.窗体.Views.FormBinaryzation(tool);
                BinaryzationViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                BinaryzationViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("模板匹配"))
            {
                FormMatch f = new PositionToolsLib.窗体.Views.FormMatch(tool);
                MatchViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                MatchViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("查找直线"))
            {
                FormFindLine f = new PositionToolsLib.窗体.Views.FormFindLine(tool);
                FindLineViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                FindLineViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("拟合直线"))
            {
                FormFitLine f = new PositionToolsLib.窗体.Views.FormFitLine(tool);
                FitLineViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                FitLineViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("直线偏移"))
            {
                FormLineOffset f = new PositionToolsLib.窗体.Views.FormLineOffset(tool);
                LineOffsetViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                LineOffsetViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("平行直线"))
            {
                FormCalParallelLine f = new PositionToolsLib.窗体.Views.FormCalParallelLine(tool);
                CalParallelLineViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                CalParallelLineViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("直线中心"))
            {
                FormLineCentre f = new PositionToolsLib.窗体.Views.FormLineCentre(tool);
                LineCentreViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                LineCentreViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("直线交点"))
            {
                FormLineIntersection f = new PositionToolsLib.窗体.Views.FormLineIntersection(tool);
                LineIntersectionViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                LineIntersectionViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("查找圆"))
            {
                FormFindCircle f = new PositionToolsLib.窗体.Views.FormFindCircle(tool);
                FindCircleViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                FindCircleViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("Blob中心"))
            {
                FormBlob f = new PositionToolsLib.窗体.Views.FormBlob(tool);
                BlobViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                BlobViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("坐标换算"))
            {
                FormCoordConvert f = new PositionToolsLib.窗体.Views.FormCoordConvert(tool);
                CoordConvertViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                CoordConvertViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("角度换算"))
            {
                FormAngleConvert f = new PositionToolsLib.窗体.Views.FormAngleConvert(tool);
                AngleConvertViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                AngleConvertViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("畸变校正"))
            {
                FormImageCorrect f = new PositionToolsLib.窗体.Views.FormImageCorrect(tool);
                ImageCorrectViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                ImageCorrectViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }
            else if (toolName.Contains("点点距离"))
            {

            }
            else if (toolName.Contains("点线距离"))
            {

            }
            else if (toolName.Contains("线线距离"))
            {

            }
            else if (toolName.Contains("轨迹提取"))
            {

            }
            else if (toolName.Contains("结果显示"))
            {
                FormResultShow f = new PositionToolsLib.窗体.Views.FormResultShow(tool);
                ResultShowViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                ResultShowViewModel.This.OnSaveManageHandle = SaveManage;
                f.ShowDialog();
            }

        }

        void ToolsOfPosition_ContextMenuClick(object o)
        {
            string operateName = ((MenuItem)o).Header.ToString();
            int index = Model.ToolsOfPositionSelectIndex;
            if (index < 0 ||
               index >= Model.ToolsOfPositionList.Count)
                return;

            string toolName = projectOfPos.toolNamesList[index];
            if (operateName.Equals("上移"))
            {
                ListViewToolsOfPositionData temData = Model.ToolsOfPositionList[index];
                Model.ToolsOfPositionList.Insert(index - 1, temData);
                Model.ToolsOfPositionList.RemoveAt(index + 1);

                projectOfPos.toolNamesList.Insert(index - 1, toolName);
                projectOfPos.toolNamesList.RemoveAt(index + 1);

            }
            else if (operateName.Equals("下移"))
            {
                ListViewToolsOfPositionData temData = Model.ToolsOfPositionList[index];
                Model.ToolsOfPositionList.Insert(index + 2, temData);
                Model.ToolsOfPositionList.RemoveAt(index);

                projectOfPos.toolNamesList.Insert(index + 2, toolName);
                projectOfPos.toolNamesList.RemoveAt(index);

            }
            else if (operateName.Equals("删除"))
            {
                Model.ToolsOfPositionList.RemoveAt(index);
                if (projectOfPos.dataManage.resultFlagDic.ContainsKey(toolName))
                    projectOfPos.dataManage.resultFlagDic.Remove(toolName);
                if (projectOfPos.dataManage.imageBufDic.ContainsKey(toolName))
                    projectOfPos.dataManage.imageBufDic.Remove(toolName);
                if (projectOfPos.dataManage.matrixBufDic.ContainsKey(toolName))
                    projectOfPos.dataManage.matrixBufDic.Remove(toolName);
                if (projectOfPos.dataManage.enumerableTooDic.Contains(toolName))
                    projectOfPos.dataManage.enumerableTooDic.Remove(toolName);
                if (projectOfPos.dataManage.resultBufDic.ContainsKey(toolName))
                    projectOfPos.dataManage.resultBufDic.Remove(toolName);
                if (projectOfPos.dataManage.resultInfoDic.ContainsKey(toolName))
                    projectOfPos.dataManage.resultInfoDic.Remove(toolName);
                if (projectOfPos.dataManage.LineDataDic.ContainsKey(toolName))
                    projectOfPos.dataManage.LineDataDic.Remove(toolName);
                if (projectOfPos.dataManage.PositionDataDic.ContainsKey(toolName))
                    projectOfPos.dataManage.PositionDataDic.Remove(toolName);

                projectOfPos.toolsDic.Remove(projectOfPos.toolNamesList[index]);
                projectOfPos.toolNamesList.RemoveAt(index);

            }
            else if (operateName.Equals("修改备注"))
            {

                FormRemarks f = new FormRemarks();
                f.Topmost = true;
                if (f.ShowDialog().Value)
                {
                    string remark = f.remarks;

                    Model.ToolsOfPositionList[index].ToolNotes = remark;

                    if (projectOfPos.toolsDic.ContainsKey(toolName))
                        projectOfPos.toolsDic[toolName].remark = remark;

                }
            }
        }



        #endregion

        #region--------------定位检测---------------
        public PosDataManage GetManage()
        {
            return this.projectOfPos.dataManage;
        }
        void SaveManage(PosDataManage manage)
        {
            this.projectOfPos.dataManage = manage;
        }
        /// <summary>
        /// 定位检测参数保存
        /// </summary>
        /// <param name="toolName"></param>
        /// <param name="par"></param>
        void OnSaveParamEventOfPosition(string toolName, BaseParam par)
        {
            if (projectOfPos.toolNamesList.Contains(toolName))
            {
                PosBaseTool tool = projectOfPos.toolsDic[toolName];
                tool.SetParam(par);
                projectOfPos.toolsDic[toolName] = tool;
            }
        }
        #endregion

        #region--------------胶水AOI---------------
        #endregion

        #region  Private Method
        /// <summary>
        /// 运行检测流程
        /// </summary>
        private  StuCoordinateData RunTestFlowOfPosition()
        {
            objs.Clear();
            infos.Clear();
            List<string> InfoList = new List<string>();

            if (!PosBaseTool.ObjectValided(this.GrabImg))
            {
                Appentxt("图像为空");
                return new StuCoordinateData(0, 0, 0);
            }
            if (!projectOfPos.dataManage.imageBufDic.ContainsKey("原始图像"))
                projectOfPos.dataManage.imageBufDic.Add("原始图像", null);
            if (PosBaseTool.ObjectValided(projectOfPos.dataManage.imageBufDic["原始图像"]))
                projectOfPos.dataManage.imageBufDic["原始图像"].Dispose();
            projectOfPos.dataManage.imageBufDic["原始图像"] = this.GrabImg.Clone();
            int count = projectOfPos.toolNamesList.Count;
            if (count <= 0)
            {
                Appentxt("流程为空");
                return new StuCoordinateData(0, 0, 0);
            }
            InfoList.Add(string.Format("Channel:{0}", "unknow"));
            InfoList.Add(string.Format("SN:{0}", "default"));
            long ctime = 0;
            //工具循环运行
            for (int i = 0; i < count; i++)
            {
                string name = projectOfPos.toolNamesList[i];
                PosRunResult rlt = projectOfPos.toolsDic[name].Run();
                ctime += rlt.runTime;
                //UpdateStatusOfPosition(i, rlt.runFlag);
                if (!rlt.runFlag)
                {
                    Appentxt(string.Format("工具：{0}，运行失败，异常信息：{1}", name, rlt.errInfo));
                    //break;

                }
                else
                    Appentxt(string.Format("工具：{0}，运行完成，时长：{1}ms", name, rlt.runTime));

            }
            StuCoordinateData data = new StuCoordinateData(0, 0, 0);
          
            InfoList.Add(string.Format("CT:{0}ms", ctime));

            if (projectOfPos.toolNamesList.Exists(t => t.Contains("结果显示")))
            {
                int index = projectOfPos.toolNamesList.FindIndex(t => t.Contains("结果显示"));
                PosBaseTool tool = projectOfPos.toolsDic[projectOfPos.toolNamesList[index]];
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage((tool.GetParam() as PositionToolsLib.参数.ResultShowParam).OutputImg);
                ShowTool.DispRegion((tool.GetParam() as PositionToolsLib.参数.ResultShowParam).ResultRegion, "green");
                ShowTool.AddregionBuffer((tool.GetParam() as PositionToolsLib.参数.ResultShowParam).ResultRegion, "green");
                objs.Add(new StuWindowHobjectToPaint
                {
                    color = "green",
                    obj = (tool.GetParam() as PositionToolsLib.参数.ResultShowParam).ResultRegion.Clone()
                });
                data = (tool.GetParam() as PositionToolsLib.参数.ResultShowParam).CoordinateData;
        
                InfoList.Add(string.Format("Pos_x:{0:f3}\nPos_y:{1:f3}\nPos_ang:{2:f3}",
                    data.x, data.y, data.angle));

            }
            else
            {
                string toolName = projectOfPos.toolNamesList[count - 1];
                PosBaseTool tool = projectOfPos.toolsDic[toolName];
                ShowTool.ClearAllOverLays();
                ShowTool.DispConcatedObj(tool.GetParam().InputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer(tool.GetParam().InputImg, EumCommonColors.green);
                bool flag = true;
                if (tool.GetType() == typeof(PositionToolsLib.工具.MatchTool))
                    flag = (tool.GetParam() as PositionToolsLib.参数.MatchParam).MatchRunStatus;
                else if (tool.GetType() == typeof(PositionToolsLib.工具.FindLineTool))
                    flag = (tool.GetParam() as PositionToolsLib.参数.FindLineParam).FindLineRunStatus;
                else if (tool.GetType() == typeof(PositionToolsLib.工具.FindCircleTool))
                    flag = (tool.GetParam() as PositionToolsLib.参数.FindCircleParam).FindCircleRunStatus;
                else if (tool.GetType() == typeof(PositionToolsLib.工具.BlobTool))
                    flag = (tool.GetParam() as PositionToolsLib.参数.BlobParam).BlobRunStatus;
                else if (tool.GetType() == typeof(PositionToolsLib.工具.FitLineTool))
                    flag = (tool.GetParam() as PositionToolsLib.参数.FitLineParam).FitLineRunStatus;
                else if (tool.GetType() == typeof(PositionToolsLib.工具.LineOffsetTool))
                    flag = (tool.GetParam() as PositionToolsLib.参数.LineOffsetParam).LineOffsetRunStatus;


                if (projectOfPos.dataManage.resultBufDic.ContainsKey(toolName) && flag)
                {
                    ShowTool.DispRegion(projectOfPos.dataManage.resultBufDic[toolName], "green");
                    ShowTool.AddregionBuffer(projectOfPos.dataManage.resultBufDic[toolName], "green");

                    objs.Add(new StuWindowHobjectToPaint
                    {
                        color = "green",
                        obj = projectOfPos.dataManage.resultBufDic[toolName].Clone()
                    });
                }
                
                if (projectOfPos.dataManage.PositionDataDic.ContainsKey(toolName) && flag)
                {
                    data = projectOfPos.dataManage.PositionDataDic[toolName];
                   
                    InfoList.Add(string.Format("Pos_x:{0:f3}\nPos_y:{1:f3}\nPos_ang:{2:f3}",
                    data.x, data.y, data.angle));

                }
                else
                {
                    InfoList.Add(string.Format("Pos_x:{0:f3}\nPos_y:{1:f3}\nPos_ang:{2:f3}",
                  0, 0, 0));
                }

                if (!flag)
                    data = new StuCoordinateData(0, 0, 0);

            }

            #region------显示检测区域----
            if (projectOfPos.toolNamesList.Exists(t => t.Contains("结果显示")))
            {
                int index = projectOfPos.toolNamesList.FindIndex(t => t.Contains("结果显示"));
                PosBaseTool tool = projectOfPos.toolsDic[projectOfPos.toolNamesList[index]];
                if ((tool as PositionToolsLib.工具.ResultShowTool).isShowInspectRegion)
                {
                    for (int j = 0; j < count; j++)
                    {
                        string name = projectOfPos.toolNamesList[j];
                        HObject inspectROI = null;
                        if (name.Contains("模板匹配"))
                            inspectROI = (projectOfPos.toolsDic[name].GetParam() as PositionToolsLib.参数.MatchParam).InspectROI;
                        else if (name.Contains("查找直线"))
                            inspectROI = (projectOfPos.toolsDic[name].GetParam() as PositionToolsLib.参数.FindLineParam).ResultInspectROI;
                        else if (name.Contains("查找圆"))
                            inspectROI = (projectOfPos.toolsDic[name].GetParam() as PositionToolsLib.参数.FindCircleParam).ResultInspectROI;
                        else if (name.Contains("Blob"))
                            inspectROI = (projectOfPos.toolsDic[name].GetParam() as PositionToolsLib.参数.BlobParam).ResultInspectROI;

                        if (PosBaseTool.ObjectValided(inspectROI))
                        {
                            ShowTool.DispRegion(inspectROI, "blue");
                            ShowTool.AddregionBuffer(inspectROI, "blue");
                            objs.Add(new StuWindowHobjectToPaint
                            {
                                color = "blue",
                                obj = inspectROI.Clone()
                            });
                        }
                    }
                }
            }
            //默认显示
            else
            {
                for (int j = 0; j < count; j++)
                {
                    string name = projectOfPos.toolNamesList[j];
                    HObject inspectROI = null;
                    if (name.Contains("模板匹配"))
                        inspectROI = (projectOfPos.toolsDic[name].GetParam() as PositionToolsLib.参数.MatchParam).InspectROI;
                    else if (name.Contains("查找直线"))
                        inspectROI = (projectOfPos.toolsDic[name].GetParam() as PositionToolsLib.参数.FindLineParam).ResultInspectROI;
                    else if (name.Contains("查找圆"))
                        inspectROI = (projectOfPos.toolsDic[name].GetParam() as PositionToolsLib.参数.FindCircleParam).ResultInspectROI;
                    else if (name.Contains("Blob"))
                        inspectROI = (projectOfPos.toolsDic[name].GetParam() as PositionToolsLib.参数.BlobParam).ResultInspectROI;

                    if (PosBaseTool.ObjectValided(inspectROI))
                    {
                        ShowTool.DispRegion(inspectROI, "blue");
                        ShowTool.AddregionBuffer(inspectROI, "blue");
                        objs.Add(new StuWindowHobjectToPaint
                        {
                            color = "blue",
                            obj = inspectROI.Clone()
                        });
                    }
                }
            }
            #endregion

            #region-----计算最终结果---
            bool totalResult = true;//最终结果
            int cindex = 0;
            foreach (var s in projectOfPos.dataManage.resultFlagDic)
            {
                totalResult = totalResult && s.Value;
                UpdateStatusOfPosition(cindex, s.Value);
                cindex++;
            }
            #endregion

            #region-----显示需要的信息-----

            HOperatorSet.GetImageSize(this.GrabImg, out HTuple width, out HTuple height);
            int ratio = (int)(width.D / 1000);
            int size = ratio * 75;
            if (size < 75)
                size = 75;
            if (totalResult)
            {
                ShowTool.DispMessage("OK", 10, 10, "green", 50);
                ShowTool.AddTextBuffer("OK", 10, 10, "green", 50);
                infos.Add(new StuWindowInfoToPaint
                {
                    color = "green",
                    size = size,
                    Info = "OK",
                    coorditionDat = new CoorditionDat(10, 10)
                });
            }
            else
            {
                ShowTool.DispMessage("NG", 10, 10, "red", 50);
                ShowTool.AddTextBuffer("NG", 10, 10, "red", 50);
                infos.Add(new StuWindowInfoToPaint
                {
                    color = "red",
                    size = size,
                    Info = "NG",
                    coorditionDat = new CoorditionDat(10, 10)
                });
            }

            string content = "";
            foreach (var s in InfoList)
                content += s + "\n";
            if (projectOfPos.toolNamesList.Exists(t => t.Contains("结果显示")))
            {
                int index = projectOfPos.toolNamesList.FindIndex(t => t.Contains("结果显示"));
                PosBaseTool tool = projectOfPos.toolsDic[projectOfPos.toolNamesList[index]];

                double row = (tool as PositionToolsLib.工具.ResultShowTool).inforCoorY;
                double col = (tool as PositionToolsLib.工具.ResultShowTool).inforCoorX;

                ShowTool.DispMessage(content, row, col, "green", 25);
                ShowTool.AddTextBuffer(content, row, col, "green", 25);
                infos.Add(new StuWindowInfoToPaint
                {
                    color = "green",
                    size = size / 2,
                    Info = content,
                    coorditionDat = new CoorditionDat(size * 2, 10)
                });
            }
            else
            {
                ShowTool.DispMessage(content, 300, 10, "green", 25);
                ShowTool.AddTextBuffer(content, 300, 10, "green", 25);
                infos.Add(new StuWindowInfoToPaint
                {
                    color = "green",
                    size = size / 2,
                    Info = content,
                    coorditionDat = new CoorditionDat(size * 2, 10)
                });
            }
            #endregion

            if (!totalResult)
                data = new PositionToolsLib.StuCoordinateData(0, 0, 0);
            return data;
        }
        /// <summary>
        /// 更新工具运行状态
        /// </summary>
        /// <param name="index"></param>
        /// <param name="flag"></param>
        void UpdateStatusOfPosition(int index, bool flag)
        {
           
            if (index < 0 ||
               index >= Model.ToolsOfPositionList.Count)
                return;
    
            Model.ToolsOfPositionList[index].ToolStatus= flag ? "OK" : "NG";      
        }
        /// <summary>
        /// 复位定位检测工具编号
        /// </summary>
        private void ResetNumOfPos()
        {
            PositionToolsLib.工具.BinaryzationTool.inum = 0;
            PositionToolsLib.工具.BlobTool.inum = 0;
            PositionToolsLib.工具.CalParallelLineTool.inum = 0;
            PositionToolsLib.工具.ClosingTool.inum = 0;
            PositionToolsLib.工具.ColorConvertTool.inum = 0;
            PositionToolsLib.工具.DilationTool.inum = 0;
            PositionToolsLib.工具.ErosionTool.inum = 0;
            PositionToolsLib.工具.FindCircleTool.inum = 0;
            PositionToolsLib.工具.FindLineTool.inum = 0;
            PositionToolsLib.工具.FitLineTool.inum = 0;
            PositionToolsLib.工具.LineCentreTool.inum = 0;
            PositionToolsLib.工具.LineIntersectionTool.inum = 0;
            PositionToolsLib.工具.MatchTool.inum = 0;
            PositionToolsLib.工具.OpeningTool.inum = 0;
            PositionToolsLib.工具.ResultShowTool.inum = 0;
        }
        /// <summary>
        /// 显示定位检测流程
        /// </summary>
        private void ShowTestFlowOfPosition()
        {
            projectOfPos.dataManage?.ResetBuf();//重载后清除数据缓存
            Model.ToolsOfPositionList.Clear();
            toolindexofPosition = 0;
            if (this.projectOfPos == null) return;
            if (this.projectOfPos.toolsDic.Count <= 0) return;

            foreach (var s in projectOfPos.toolNamesList)
            {
                toolindexofPosition++;
                Model.ToolsOfPositionList.Add(new
                ListViewToolsOfPositionData(toolindexofPosition,
                           s, "--", projectOfPos.toolsDic[s].remark));
            }
        }


        /// <summary>
        /// 富文本信息清除
        /// </summary>
        private void ClearTextClick()
        {
            Model.ClearRichText = false;
            Model.ClearRichText = true;
        }
        /// <summary>
        /// 添加测试文本及日志
        /// </summary>
        /// <param name="info"></param>
        private void Appentxt(string info)
        {
            string dConvertString = string.Format("{0}  {1}\r",
                              DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), info);
            Model.RichIfo = dConvertString;
            log.Info("测试信息", info);

        }
        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        private bool CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                //如果目标路径不存在,则创建目标路径
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                //得到原文件根目录下的所有文件
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest, true);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);//构建目标路径,递归复制文件
                }
                return true;
            }
            catch (Exception er)
            {
                // currvisiontool.DispMessage(er.Message, 300, 500, "red", 50);
                Appentxt(er.Message);
                //MessageBox.Show(e.Message);
                return false;
            }

        }

        /// <summary>
        /// 创建物理坐标系，显示在图像的右上角
        /// </summary>
        void Create_Physical_coorsys()
        {
            if (hv_HomMat2D == null || hv_HomMat2D.Length < 1 ||
                !PosBaseTool.ObjectValided(GrabImg)) return;
            // Task.Run(() =>
            //{
            List<HObject> temArrowList =
                GuidePositioning_HDevelopExport.CreateSys(GrabImg, hv_HomMat2D);
            if (temArrowList == null || temArrowList.Count < 2) return;

            HTuple hv_Row, hv_Col;
            HTuple hv_MeanRow, hv_MeanCol;
            HOperatorSet.GetContourXld(temArrowList[0], out hv_Row, out hv_Col);

            HOperatorSet.TupleMean(hv_Row, out hv_MeanRow);
            HOperatorSet.TupleMean(hv_Col, out hv_MeanCol);

            HTuple hv_Row1, hv_Col1;
            HTuple hv_MeanRow1, hv_MeanCol1;
            HOperatorSet.GetContourXld(temArrowList[1], out hv_Row1, out hv_Col1);
            HOperatorSet.TupleMean(hv_Row1, out hv_MeanRow1);
            HOperatorSet.TupleMean(hv_Col1, out hv_MeanCol1);

            //this.currvisiontool.Invoke(new Action(() =>
            //{
            ShowTool.AddregionBuffer(temArrowList[0], "red");
            ShowTool.DispRegion(temArrowList[0], "red");
            ShowTool.DispMessage("X", hv_MeanRow, hv_MeanCol, "red", 16);
            ShowTool.AddTextBuffer("X", hv_MeanRow, hv_MeanCol, "red", 16);

            ShowTool.AddregionBuffer(temArrowList[1], "green");
            ShowTool.DispRegion(temArrowList[1], "green");
            ShowTool.DispMessage("Y", hv_MeanRow1, hv_MeanCol1, "green", 16);
            ShowTool.AddTextBuffer("Y", hv_MeanRow1, hv_MeanCol1, "green", 16);
            //}));

            //});
        }
        #endregion

        #region   External Communication
        /// <summary>
        /// 建立虚拟连接
        /// </summary>
        /// <returns></returns>
        private bool BuiltConnect()
        {
            if (virtualConnect == null) return false;
            else if (virtualConnect.IsRunning) return true;
            virtualConnect.GetDataHandle += new EventHandler(GetDataEvent);
            virtualConnect.sendDataHandle += new VirtualConnect.SendDataHandle(SendDataEvent);
            return virtualConnect.StartConnect();

        }
        /// <summary>
        /// 与外部通讯事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GetDataEvent(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        void SendDataEvent(string data)
        {
            GetDataOfCaliHandle?.Invoke(data);
        }
        /// <summary>
        /// 外部数据写入DLL库
        /// </summary>
        /// <param name="data"></param>
        public void ExternWriteData(string data)
        {
            if (virtualConnect != null)
            {
                if (virtualConnect.IsRunning)
                {
                    virtualConnect.ReadData(data);
                }
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (virtualConnect == null) return false;
            else if (!virtualConnect.IsRunning) return true;
            virtualConnect.GetDataHandle -= GetDataEvent;
            return virtualConnect.Disconnnect();

        }
        #endregion

        #region  Property
        /// <summary>
        /// 当前实例工位名称
        /// </summary>
        public string CurrCamStationName
        {
            get => this.currCamStationName;
        }

        long imageWidth = 0;
        /// <summary>
        /// 图像宽度
        /// </summary>
        public long ImageWidth { get => this.imageWidth; }

        long imageHeight = 0;
        /// <summary>
        /// 图像高度
        /// </summary>
        public long ImageHeight { get => this.imageHeight; }
        #endregion

        #region Image

        void 彩色显示ChangeEvent(object sender, EventArgs e)
        {
          
        }
        void 显示中心十字坐标Event(object sender, EventArgs e)
        {
           
        }
        /// <summary>
        /// 图像采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CamGrabEvent(object sender, EventArgs e)
        {
            if (CurrCam == null || !CurrCam.IsAlive)
            {
                Appentxt("相机不在线");
                return;
            }
            if (CurrCam.IsGrabing)
            {
                CurrCam.StopGrab();  //如果已在采集中则先停止采集
                Thread.Sleep(20);
            }
           
            Thread.Sleep(20);
            workstatus = EunmcurrCamWorkStatus.Freestyle;
            CurrCam.OneShot();
            camContinueGrabHandle?.Invoke(false);    
        }
        //图像旋转
        void ImageGetRotationEvent(object sender, EventArgs e)
        {

            HOperatorSet.GenEmptyObj(out GrabImg);
            GrabImg.Dispose();
            GrabImg = ShowTool.D_HImage;

            HTuple _width, _height;
            if (GuidePositioning_HDevelopExport.ObjectValided(GrabImg))
                HOperatorSet.GetImageSize(GrabImg, out _width, out _height);
            else
            {
                _width = 0;
                _height = 0;
            }
            imageWidth = _width.I;
            imageHeight = _height.I;
            imageSizeChangeHandle?.Invoke(_width.I, _height.I);

            Create_Physical_coorsys();
            string ImageRotation = sender.ToString();
            GeneralUse.WriteValue("图像旋转", "角度", ImageRotation, "config", rootFolder + "\\Config");
            Appentxt(string.Format("图像旋转{0}度", ImageRotation));

            //添加图像到缓存集合
            if (GlueBaseTool.ObjectValided(this.GrabImg))
                if (!projectOfGlue.dataManage.imageBufDic.ContainsKey("原始图像"))
                    projectOfGlue.dataManage.imageBufDic.Add("原始图像", this.GrabImg.Clone());
                else
                    projectOfGlue.dataManage.imageBufDic["原始图像"] = this.GrabImg.Clone();
            //添加图像到缓存集合
            if (PosBaseTool.ObjectValided(this.GrabImg))
                if (!projectOfPos.dataManage.imageBufDic.ContainsKey("原始图像"))
                    projectOfPos.dataManage.imageBufDic.Add("原始图像", this.GrabImg.Clone());
                else
                    projectOfPos.dataManage.imageBufDic["原始图像"] = this.GrabImg.Clone();

        }
        /// <summary>
        /// 图像加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadedImageNoticeEvent(object sender, EventArgs e)
        {
            imgBuf.Dispose();
            imgBuf = ShowTool.D_HImage;

            objs.Clear();
            infos.Clear();
            HOperatorSet.GenEmptyObj(out GrabImg);
            GrabImg.Dispose();
            GrabImg = ShowTool.D_HImage;
            //添加图像到缓存集合
            if (GlueBaseTool.ObjectValided(this.GrabImg))
                if (!projectOfGlue.dataManage.imageBufDic.ContainsKey("原始图像"))
                    projectOfGlue.dataManage.imageBufDic.Add("原始图像", this.GrabImg.Clone());
                else
                    projectOfGlue.dataManage.imageBufDic["原始图像"] = this.GrabImg.Clone();
            //添加图像到缓存集合
            if (PosBaseTool.ObjectValided(this.GrabImg))
                if (!projectOfPos.dataManage.imageBufDic.ContainsKey("原始图像"))
                    projectOfPos.dataManage.imageBufDic.Add("原始图像", this.GrabImg.Clone());
                else
                    projectOfPos.dataManage.imageBufDic["原始图像"] = this.GrabImg.Clone();
            HOperatorSet.GetImageSize(GrabImg, out HTuple width, out HTuple height);
            imageWidth = width.I;
            imageHeight = height.I;
            Create_Physical_coorsys();
        }

        #endregion
    }
}
