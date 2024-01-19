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
using GlueBaseParam = GlueDetectionLib.参数.BaseParam;
using GlueRunResult = GlueDetectionLib.工具.RunResult;
using GlueTcpSendTool = GlueDetectionLib.工具.TcpSendTool;
using GlueTcpRecvTool = GlueDetectionLib.工具.TcpRecvTool;

using PosBaseTool = PositionToolsLib.工具.BaseTool;
using PosDataManage = PositionToolsLib.DataManage;
using PosBaseParam = PositionToolsLib.参数.BaseParam;
using PosRunResult = PositionToolsLib.工具.RunResult;
using PosTcpSendTool = PositionToolsLib.工具.TcpSendTool;
using PosTcpRecvTool = PositionToolsLib.工具.TcpRecvTool;

using System.Diagnostics;
using PositionToolsLib.窗体.Views;
using PositionToolsLib.窗体.ViewModels;
using PositionToolsLib.参数;
using CommunicationTools.Models;
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
using System.Collections.ObjectModel;
using PositionToolsLib.窗体.Models;
using PositionToolsLib.工具;
using GlueDetectionLib.工具;
using System.Threading.Channels;
using GlueDetectionLib.参数;
using static VisionShowLib.UserControls.VisionShowTool;
using System.Windows.Input;
using Microsoft.Win32;
using FunctionLib.AutoFocus;
using CommunicationTools.Views;
using CommunicationTools;
using FunctionLib.TCP;
using System.Net;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Media.Media3D;
using System.Numerics;

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
        public EventHandler CamConnectStatusHandle;//相机链接状态时间
        public Action<string> AppenTxtAction = null;
        public Action ClearTxtAction = null;
        public OutPointGray DoubleClickGetMousePosHandle;//双击获取像素坐标  
        public EventHandler AutoFocusDataHandle;//自动对焦事件
        /*-----------------------------------------文件配置---------------------------------------*/
        private string rootFolder = AppDomain.CurrentDomain.BaseDirectory; //根目录
        private string currCamStationName = "cam1";
        string currCalibName = "default";// 当前标定文件名称
        string currRecipeName = "default";// 当前配方文件名称                               // 
        string saveToUsePath = AppDomain.CurrentDomain.BaseDirectory + "配方\\default";// 配方文件夹路径
        string NineCalibFile = AppDomain.CurrentDomain.BaseDirectory + "标定矩阵\\九点标定\\default\\hv_HomMat2D.tup";
        string CorrectCalibFile = AppDomain.CurrentDomain.BaseDirectory + "标定矩阵\\标定助手\\default\\相机内参.dat";
        bool isColorPalette = false;
        /*-----------------------------------------通讯工具---------------------------------------*/
        object locker = new object();//同步锁
        bool ContinueRunFlag = false;//连续运行标志
        /*-----------------------------------------定位工具---------------------------------------*/
        EumModelType currModelType = EumModelType.ProductModel_1;
        int toolindexofPosition = 0;
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
        int toolindexofGlueAOI = 0;
        List<GlueRecheckDat> datas = new List<GlueRecheckDat>();//发送给运控的数据
        StuCoordinateData positionSharpData = new StuCoordinateData(0, 0, 0);
        /*-----------------------------------------其他工具---------------------------------------*/
        HObject GrabImg = null;
        HObject imgBuf = null;//图像缓存     
        FormNinePointsCalib f_NinePointsCalib = null;
        //public static VisionViewModel This { get; set; }
        public VisionModel Model { get; set; }
        public VisionShowTool ShowTool { get; set; }
        public Icam CurrCam = null;  //相机接口
        long currCamExpouse = 1000;
        int currCamGain = 0;
        EunmCamWorkStatus workstatus = EunmCamWorkStatus.None;//当前相机工作状态
        CamType currCamType = CamType.NONE;
        int DgPixelPointIndexer = 0;
        int DgRobotPointIndexer = 0;
        int DgRotatePointIndexer = 0;
        Dictionary<int, bool> NinePointStatusDic = new Dictionary<int, bool>();
        Dictionary<int, bool> RotatoStatusDic = new Dictionary<int, bool>();
        int camIndex = 0;
        //自动对焦偏差阈值
        int DeviationThd = -10;
        //限位阈值
        int LimitMethd = 20;
        HObject autoFocusRegion = null;
        #region Command
        public CommandBase WindowsLoadedCommand { get; set; }
        public CommandBase NinePointsCalibFormClickCommand { get; set; }
        public CommandBase ModelTypeSelectionChangedCommand { get; set; }
        public CommandBase PosToolBarBtnClickCommand { get; set; }
        public CommandBase GlueToolBarBtnClickCommand { get; set; }     
        public CommandBase PosMenuClickCommand { get; set; }
        public CommandBase GlueMenuClickCommand { get; set; }
        public CommandBase ToolsOfPosDoubleClickCommand { get; set; }
        public CommandBase ToolsOfGlueDoubleClickCommand { get; set; }
        public CommandBase ToolsOfPosMouseUpCommand { get; set; }
        public CommandBase ToolsOfGlueMouseUpCommand { get; set; }
        public CommandBase ToolsOfPosition_ContextMenuCommand { get; set; }
        public CommandBase ToolsOfGlue_ContextMenuCommand { get; set; }
        public CommandBase ClearTextCommand { get; set; }
        public CommandBase NewRecipeClickCommand { get; set; }
        public CommandBase DeleteRecipeClickCommand { get; set; }
        public CommandBase SaveRecpeClickCommand { get; set; }
        public CommandBase OpenCamButClickCommand { get; set; }
        public CommandBase CloseCamButClickCommand{ get; set; }
        public CommandBase ExpouseValueChangedCommand { get; set; }
        public CommandBase ExpouseNumericKeyDownCommand { get; set; }
        public CommandBase GainValueChangedCommand { get; set; }
        public CommandBase GainNumericKeyDownCommand { get; set; }
        public CommandBase CamTypeSelectionChangedCommand { get; set; }
        public CommandBase CamIndexerSelectionChangedCommand { get; set; }
        public CommandBase OneShotBtnClickCommand { get; set; }
        public CommandBase ContinueGrabBtnClickCommand { get; set; }
        public CommandBase SaveCamParamBtnClickCommand { get; set; }
        public CommandBase StopGrabBtnClickCommand { get; set; }
        public CommandBase NumDeviationThdKeyDownCommand { get; set; }
        public CommandBase NumLimitMethdKeyDownCommand { get; set; }
        public CommandBase RdbtnCheckedChangedCommand { get; set; }
        public CommandBase AssistCircleKeyDownCommand { get; set; }
        public CommandBase ScaleRuleCheckChangeCommand { get; set; }
        #endregion

        /*-----------------------------------------Construction---------------------------------------*/
        private VisionViewModel()
        {
            HOperatorSet.GenEmptyObj(out GrabImg);
            HOperatorSet.GenEmptyObj(out imgBuf);
            //This = this;
            Model = new VisionModel();
            //图像控件      
            ShowTool = new VisionShowTool();
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            ShowTool.ImageGetRotationHandle += new EventHandler(ImageGetRotationEvent);
            ShowTool.CamGrabHandle += new EventHandler(CamGrabEvent);
            ShowTool.DoubleClickGetMousePosHandle += new OutPointGray(DoubleClickGetMousePosEvent);
            ShowTool.显示中心十字坐标Handle += new EventHandler(显示中心十字坐标Event);
            ShowTool.彩色显示ChangeEventHandle += new EventHandler(彩色显示ChangeEvent);
            ShowTool.SaveWindowImageHnadle += new EventHandler(OnSaveWindowImageHnadle);
            ShowTool.BtnRunClick = Run_Click;
            ShowTool.BtnStopClick = Stop_Click;
      
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

            GlueToolBarBtnClickCommand = new CommandBase();
            GlueToolBarBtnClickCommand.DoExecute = new Action<object>((o) => GlueToolBarBtn_Click(o));
            GlueToolBarBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            PosMenuClickCommand = new CommandBase();
            PosMenuClickCommand.DoExecute = new Action<object>((o) => PosMenu_Click(o));
            PosMenuClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            GlueMenuClickCommand = new CommandBase();
            GlueMenuClickCommand.DoExecute = new Action<object>((o) => GlueMenu_Click(o));
            GlueMenuClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ToolsOfPosDoubleClickCommand = new CommandBase();
            ToolsOfPosDoubleClickCommand.DoExecute = new Action<object>((o) => ListViewToolsOfPosition_DoubleClick(o));
            ToolsOfPosDoubleClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ToolsOfGlueDoubleClickCommand = new CommandBase();
            ToolsOfGlueDoubleClickCommand.DoExecute = new Action<object>((o) => ListViewToolsOfGlue_DoubleClick(o));
            ToolsOfGlueDoubleClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            ToolsOfPosMouseUpCommand = new CommandBase();
            ToolsOfPosMouseUpCommand.DoExecute = new Action<object>((o) => ListViewToolsOfPosition_MouseUp());
            ToolsOfPosMouseUpCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ToolsOfGlueMouseUpCommand = new CommandBase();
            ToolsOfGlueMouseUpCommand.DoExecute = new Action<object>((o) => ListViewToolsOfGlue_MouseUp());
            ToolsOfGlueMouseUpCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            ToolsOfPosition_ContextMenuCommand = new CommandBase();
            ToolsOfPosition_ContextMenuCommand.DoExecute = new Action<object>((o) => ToolsOfPosition_ContextMenuClick(o));
            ToolsOfPosition_ContextMenuCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ToolsOfGlue_ContextMenuCommand = new CommandBase();
            ToolsOfGlue_ContextMenuCommand.DoExecute = new Action<object>((o) => ToolsOfGlue_ContextMenuClick(o));
            ToolsOfGlue_ContextMenuCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            ClearTextCommand = new CommandBase();
            ClearTextCommand.DoExecute = new Action<object>((o) => ClearTextClick());
            ClearTextCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            NewRecipeClickCommand = new CommandBase();
            NewRecipeClickCommand.DoExecute = new Action<object>((o) => NewRecipe_Click());
            NewRecipeClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DeleteRecipeClickCommand = new CommandBase();
            DeleteRecipeClickCommand.DoExecute = new Action<object>((o) => DeleteRecipe_Click());
            DeleteRecipeClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveRecpeClickCommand = new CommandBase();
            SaveRecpeClickCommand.DoExecute = new Action<object>((o) => SaveRecipe_Click());
            SaveRecpeClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            OpenCamButClickCommand = new CommandBase();
            OpenCamButClickCommand.DoExecute = new Action<object>((o) => OpenCam_Click());
            OpenCamButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            CloseCamButClickCommand = new CommandBase();
            CloseCamButClickCommand.DoExecute = new Action<object>((o) => CloseCam_Click());
            CloseCamButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ExpouseValueChangedCommand = new CommandBase();
            ExpouseValueChangedCommand.DoExecute = new Action<object>((o) => CloseCam_Click());
            ExpouseValueChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ExpouseValueChangedCommand = new CommandBase();
            ExpouseValueChangedCommand.DoExecute = new Action<object>((o) => ExpouseValueChanged(o));
            ExpouseValueChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            ExpouseNumericKeyDownCommand = new CommandBase();
            ExpouseNumericKeyDownCommand.DoExecute = new Action<object>((o) => ExpouseNumericKeyDown(o));
            ExpouseNumericKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            Model.ExpouseNumericCommand = new Action(() => ExpouseNumericEvent());

            GainValueChangedCommand = new CommandBase();
            GainValueChangedCommand.DoExecute = new Action<object>((o) => GainValueChanged(o));
            GainValueChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            GainNumericKeyDownCommand = new CommandBase();
            GainNumericKeyDownCommand.DoExecute = new Action<object>((o) => GainNumericKeyDown(o));
            GainNumericKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            Model.GainNumericCommand = new Action(() => GainNumericEvent());

            CamTypeSelectionChangedCommand = new CommandBase();
            CamTypeSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxCamType_SelectedIndexChanged(o));
            CamTypeSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            CamIndexerSelectionChangedCommand = new CommandBase();
            CamIndexerSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxCamIndex_SelectedIndexChanged(o));
            CamIndexerSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            OneShotBtnClickCommand = new CommandBase();
            OneShotBtnClickCommand.DoExecute = new Action<object>((o) => btnOneShot_Click());
            OneShotBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ContinueGrabBtnClickCommand = new CommandBase();
            ContinueGrabBtnClickCommand.DoExecute = new Action<object>((o) => btnContinueGrab_Click());
            ContinueGrabBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveCamParamBtnClickCommand = new CommandBase();
            SaveCamParamBtnClickCommand.DoExecute = new Action<object>((o) => btnSaveCamParma_Click());
            SaveCamParamBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            StopGrabBtnClickCommand = new CommandBase();
            StopGrabBtnClickCommand.DoExecute = new Action<object>((o) => btnStopGrab_Click());
            StopGrabBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

         
            NumDeviationThdKeyDownCommand = new CommandBase();
            NumDeviationThdKeyDownCommand.DoExecute = new Action<object>((o) => NumDeviationThd_KeyDown(o));
            NumDeviationThdKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            Model.DeviationThdNumericCommand = new Action(() => DeviationThdNumericEvent());

         
            NumLimitMethdKeyDownCommand = new CommandBase();
            NumLimitMethdKeyDownCommand.DoExecute = new Action<object>((o) => NumLimitMethd_KeyDown(o));
            NumLimitMethdKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            Model.LimitMethdNumericCommand = new Action(() => LimitMethdNumericEvent());

            RdbtnCheckedChangedCommand = new CommandBase();
            RdbtnCheckedChangedCommand.DoExecute = new Action<object>((o) => rdbtn_CheckedChanged(o));
            RdbtnCheckedChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            AssistCircleKeyDownCommand = new CommandBase();
            AssistCircleKeyDownCommand.DoExecute = new Action<object>((o) => AssistCircleKeyDown(o));
            AssistCircleKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            Model.AssistCircleCommand = new Action(() => AssistCircleEvent());

            ScaleRuleCheckChangeCommand = new CommandBase();
            ScaleRuleCheckChangeCommand.DoExecute = new Action<object>((o) => ScaleRule_CheckedChanged());
            ScaleRuleCheckChangeCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            #endregion

        }
        public VisionViewModel(Action<string> appenTxtAction,
            Action clearTxtAction,
               string camStationName = "camStationName_1"
            ) : this()
        {
            AppenTxtAction = appenTxtAction;
            ClearTxtAction = clearTxtAction;
            rootFolder = AppDomain.CurrentDomain.BaseDirectory + camStationName;
            ShowTool.TitleName = currCamStationName = camStationName;
            log = new Log(camStationName);
            //setOperationAuthority();       
            virtualConnect = new VirtualConnect("虚拟连接" + camStationName);
            BuiltConnect();
            LoadRecipeFile();//加载配方文件,同时获取当前启用的配方名称
            LoadMatrix();   //默认加载了default标定矩阵关系
            saveToUsePath = rootFolder + "\\配方\\"+ currRecipeName;
            CorrectCalibFile = rootFolder + "\\标定矩阵\\标定助手\\default\\相机内参.dat";
            if (!Directory.Exists(rootFolder))
                Directory.CreateDirectory(rootFolder);
            if (!Directory.Exists(rootFolder + "\\Config"))
                Directory.CreateDirectory(rootFolder + "\\Config");
            if (!Directory.Exists(rootFolder + "\\配方"))
                Directory.CreateDirectory(rootFolder + "\\配方");
            if (!Directory.Exists(saveToUsePath))
                Directory.CreateDirectory(saveToUsePath);

           
            LoadCommDev();//加载外部通讯工具     
            LoadRecipe();//加载配方
            LoadCamera();//加载相机
        }


        #region-------------窗体事件---------------
        /// <summary>
        /// 窗体加载
        /// </summary>
        void Windows_Load()
        {
            //LoadRecipe();//加载配方
        }
      
        /// <summary>
        /// 九点标定窗体显示
        /// </summary>
        void NinePointsCalibForm_Click()
        {

            //f_NinePointsCalib.BringIntoView();
            bool flag = f_NinePointsCalib.isClosedFlag;
            if(flag)
                f_NinePointsCalib = new FormNinePointsCalib(rootFolder,currCalibName);
            f_NinePointsCalib.Show();

        }
        /// <summary>
        /// 定位检测模板类型切换
        /// </summary>
        /// <param name="o"></param>
        void cobxModelTypeList_SelectedIndexChanged(object o)
        {
            if (currModelType == (EumModelType)Model.ModelTypeSelectIndex)//如果无切换则不重载
                return;
            currModelType = (EumModelType)Model.ModelTypeSelectIndex;
            if (currModelType == EumModelType.CaliBoardModel)
                f_NinePointsCalib = new FormNinePointsCalib(rootFolder,currCalibName);
            Model.ModelType= currModelType;
            string secondName = Enum.GetName(typeof(EumModelType), currModelType);
            LoadPositionFlow(secondName);
            Appentxt("模板手动切换完成");
           //切换模板重新加载相机曝光增益参数
            LoadCamParam();
            if (!PosBaseTool.ObjectValided(this.GrabImg))
                return;
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(this.GrabImg);          
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
                    dialog.InitialDirectory = saveToUsePath;
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
                            projectOfPos.GetNum();
                            projectOfPos.toolNamesList = new List<string>();
                            if (projectOfPos.dataManage == null)
                                projectOfPos.dataManage = new PosDataManage();
                            Dictionary<string, PosBaseTool> tem = new Dictionary<string, PosBaseTool>();

                            foreach (var s in projectOfPos.toolsDic)
                            {
                                projectOfPos.toolNamesList.Add(s.Value.GetToolName());
                                tem.Add(s.Value.GetToolName(), s.Value);
                                s.Value.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManageOfPos);
                                if (s.Value.GetType() == typeof(ImageCorrectTool))
                                    continue;
                                s.Value.SetMatrix(hv_HomMat2D);
                                s.Value.SetCalibFilePath(NineCalibFile);
                              
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
                        projectOfPos.GetNum();
                        Model.ToolsOfPositionList.Clear();
                    }
                    break;
                case "标定助手":
                    FormCalibAssistant f = new FormCalibAssistant(rootFolder);
                    f.Show();
                    break;
                case "通讯管理":
                    CommDevWindow fc = CommDevWindow.CreateInstance();
                    fc.Show();
                    break;
              
            }

        }
        //运行
        void Run_Click()
        {
            ContinueRunFlag = true;
            Model.ContinueRunFlag = true;
            Model.BtnOpenCamEnable = false;
            Model.BtnCloseCamEnable = false;
            Model.BtnOneShotEnable = false;
            Model.BtnContinueGrabEnable = false;
            Model.BtnStopGrabEnable = false;
            Model.IsCamAlive = false;//假定为false控件使能用

            foreach (var item in Model.ToolsOfPositionList)
                item.ContextMenuVisib = Visibility.Hidden;
            foreach (var item in Model.ToolsOfGlueList)
                item.ContextMenuVisib = Visibility.Hidden;
        }
        /// <summary>
        /// 停止
        /// </summary>
        void Stop_Click()
        {
            ContinueRunFlag = false;
            Model.ContinueRunFlag = false;
            if (CurrCam != null)
                Model.IsCamAlive = CurrCam.IsAlive;
            else
                Model.IsCamAlive = false;
            if (Model.IsCamAlive)
            {
                Model.BtnOpenCamEnable = false;
                Model.BtnCloseCamEnable = true;
                Model.BtnOneShotEnable = true;
                Model.BtnContinueGrabEnable = true;
                Model.BtnStopGrabEnable = true;
            }
            else
            {
                Model.BtnOpenCamEnable = true;
                Model.BtnCloseCamEnable = false;
                Model.BtnOneShotEnable = false;
                Model.BtnContinueGrabEnable = false;
                Model.BtnStopGrabEnable = false;
            }

            foreach (var item in Model.ToolsOfPositionList)
                item.ContextMenuVisib = Visibility.Visible;
            foreach (var item in Model.ToolsOfGlueList)
                item.ContextMenuVisib = Visibility.Visible;
        }
        /// <summary>
        /// 胶水检测工具栏按钮事件
        /// </summary>
        /// <param name="o"></param>
        void GlueToolBarBtn_Click(object o)
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
                    dialog.InitialDirectory = saveToUsePath;
                    // Show open file dialog box
                    bool? result = dialog.ShowDialog();

                    // Process open file dialog box results
                    if (result == true)
                    {
                        // Open document
                        string path = dialog.FileName;
                        try
                        {
                            ResetNumOfGlue();
                            this.projectOfGlue = GeneralUse.ReadSerializationFile<ProjectOfGlue>(path);
                            projectOfGlue.GetNum();
                            projectOfGlue.toolNamesList = new List<string>();
                            if (projectOfGlue.dataManage == null)
                                projectOfGlue.dataManage = new GlueDataManage();
                            Dictionary<string, GlueBaseTool> tem = new Dictionary<string, GlueBaseTool>();

                            foreach (var s in projectOfGlue.toolsDic)
                            {
                                projectOfGlue.toolNamesList.Add(s.Value.GetToolName());
                                tem.Add(s.Value.GetToolName(), s.Value);
                                s.Value.OnGetManageHandle = new GlueBaseTool.GetManageHandle(GetManageOfGlue);
                              
                            }
                            projectOfGlue.toolsDic = tem;
                            ShowTestFlowOfGlue();
                        }
                        catch (Exception er)
                        {
                            MessageBox.Show("胶水检测流程读取失败，异常信息:" + er.Message);
                        }
                    }
                    #endregion
                    break;
                case "保存流程":
                    #region 保存流程
                    //文件夹名称
                    string firstName = "胶水检测";       
                    if (!Directory.Exists(saveToUsePath + "\\" + firstName))
                        Directory.CreateDirectory(saveToUsePath + "\\" + firstName);

                    try
                    {
                        projectOfGlue.Refresh();
                        GeneralUse.WriteSerializationFile<ProjectOfGlue>(saveToUsePath + "\\"
                            + firstName + "\\" +  "AOI.proj",
                            projectOfGlue);
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("胶水检测流程保存失败，异常信息:" + er.Message);
                    }
                    #endregion
                    break;
                case "运行流程":
                    if (CurrCam != null)
                        if (CurrCam.IsGrabing)
                            btnStopGrab_Click();  //如果已在采集中则先停止采集
                    Task.Run(delegate ()
                    { return RunTestFlowOfGlue(); });
                    break;
                case "清空流程":
                    if (MessageBox.Show("清空流程？", "提醒", MessageBoxButton.OKCancel, MessageBoxImage.Question)
               == MessageBoxResult.OK)
                    {
                        toolindexofGlueAOI = -1;
                        projectOfGlue.toolNamesList.Clear();
                        projectOfGlue.toolsDic.Clear();
                        projectOfGlue.dataManage?.ResetBuf();//重载后清除数据缓存
                        ResetNumOfGlue();
                        projectOfGlue.GetNum();
                        Model.ToolsOfGlueList.Clear();
                    }
                    break;
              
            }

        }

        /// <summary>
        /// 定位检测菜单栏按钮事件（新增定位检测工具）
        /// </summary>
        /// <param name="o"></param>
        void PosMenu_Click(object o)
        {
            PosBaseTool tool = null;
            string tip = o.ToString();
            if (tip == "Tcp接收")
                if (projectOfPos.toolNamesList.Exists(t => t.Contains("Tcp接收")))
                {
                    Appentxt("流程中存在TCP接收工具，无需重复添加");
                    return;
                }
            if (tip == "Tcp发送")
                if (projectOfPos.toolNamesList.Exists(t => t.Contains("Tcp发送")))
                {
                    Appentxt("流程中存在Tcp发送工具，无需重复添加");
                    return;
                }
            toolindexofPosition++;

            projectOfPos.SetNum();
            switch (tip)
            {
                case "颜色转换":
                    tool = new PositionToolsLib.工具.ColorConvertTool();

                    break;
                case "膨胀":
                    tool = new PositionToolsLib.工具.DilationTool();

                    break;
                case "腐蚀":
                    tool = new PositionToolsLib.工具.ErosionTool();

                    break;
                case "开运算":
                    tool = new PositionToolsLib.工具.OpeningTool();

                    break;
                case "闭运算":
                    tool = new PositionToolsLib.工具.ClosingTool();

                    break;
                case "二值化":
                    tool = new PositionToolsLib.工具.BinaryzationTool();

                    break;
                case "模板匹配":
                    tool = new PositionToolsLib.工具.MatchTool();

                    break;
                case "查找直线":
                    tool = new PositionToolsLib.工具.FindLineTool();

                    break;
                case "拟合直线":
                    tool = new PositionToolsLib.工具.FitLineTool();

                    break;
                case "直线偏移":
                    tool = new PositionToolsLib.工具.LineOffsetTool();

                    break;
                case "平行直线":
                    tool = new PositionToolsLib.工具.CalParallelLineTool();

                    break;
                case "直线中心":
                    tool = new PositionToolsLib.工具.LineCentreTool();

                    break;
                case "直线交点":
                    tool = new PositionToolsLib.工具.LineIntersectionTool();

                    break;
                case "查找圆":
                    tool = new PositionToolsLib.工具.FindCircleTool();

                    break;
                case "Blob中心":
                    tool = new PositionToolsLib.工具.BlobTool();

                    break;
                case "坐标换算":
                    tool = new PositionToolsLib.工具.CoordConvertTool();

                    break;
                case "角度换算":
                    tool = new PositionToolsLib.工具.AngleConvertTool();

                    break;
                case "畸变校正":
                    tool = new PositionToolsLib.工具.ImageCorrectTool();

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

                    break;

                case "Tcp接收":
                    tool = new PosTcpRecvTool();

                    break;
                case "Tcp发送":
                    tool = new PosTcpSendTool();

                    break;
            }
            tool.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManageOfPos);
            if (tool.GetType() != typeof(ImageCorrectTool))
            {
                tool.SetMatrix(hv_HomMat2D);
                tool.SetCalibFilePath(NineCalibFile);
            }
            projectOfPos.toolNamesList.Add(tool.GetToolName());
            projectOfPos.toolsDic.Add(tool.GetToolName(), tool);
            Model.ToolsOfPositionList.Add(new
                ListViewToolsData(toolindexofPosition,
                tool.GetToolName(),
                "--", tool.remark));

        }
        /// <summary>
        /// 胶水检测菜单栏按钮事件（新增胶水检测工具）
        /// </summary>
        /// <param name="o"></param>
        void GlueMenu_Click(object o)
        {
            GlueBaseTool tool = null;
            string tip = o.ToString();
            if (tip == "Tcp接收")
                if (projectOfGlue.toolNamesList.Exists(t => t.Contains("Tcp接收")))
                {
                    Appentxt("流程中存在TCP接收工具，无需重复添加");
                    return;
                }
            if (tip == "Tcp发送")
                if (projectOfGlue.toolNamesList.Exists(t => t.Contains("Tcp发送")))
                {
                    Appentxt("流程中存在Tcp发送工具，无需重复添加");
                    return;
                }
            toolindexofGlueAOI++;

            projectOfGlue.SetNum();
            switch (tip)
            {
                case "颜色转换":
                    tool = new GlueDetectionLib.工具.ColorConvertTool();

                    break;
                case "膨胀":
                    tool = new GlueDetectionLib.工具.DilationTool();

                    break;
                case "腐蚀":
                    tool = new GlueDetectionLib.工具.ErosionTool();

                    break;
                case "开运算":
                    tool = new GlueDetectionLib.工具.OpeningTool();

                    break;
                case "闭运算":
                    tool = new GlueDetectionLib.工具.ClosingTool();

                    break;
                case "二值化":
                    tool = new GlueDetectionLib.工具.BinaryzationTool();

                    break;
                case "模板匹配":
                    tool = new GlueDetectionLib.工具.MatchTool();

                    break;
                case "漏胶":
                    tool = new GlueDetectionLib.工具.GlueMissTool();

                    break;
                case "偏位":
                    tool = new GlueDetectionLib.工具.GlueOffsetTool();

                    break;
                case "断胶":
                    tool = new GlueDetectionLib.工具.GlueGapTool();

                    break;
                case "胶宽":
                    tool = new GlueDetectionLib.工具.GlueCaliperWidthTool();

                    break;
                case "结果显示":
                    tool = new GlueDetectionLib.工具.ResultShowTool();

                    break;
                case "Tcp接收":
                    tool = new GlueTcpRecvTool();

                    break;
                case "Tcp发送":
                    tool = new GlueTcpSendTool();

                    break;
            }
            tool.OnGetManageHandle = new GlueBaseTool.GetManageHandle(GetManageOfGlue);
            projectOfGlue.toolNamesList.Add(tool.GetToolName());
            projectOfGlue.toolsDic.Add(tool.GetToolName(), tool);
            Model.ToolsOfGlueList.Add(new
                ListViewToolsData(toolindexofGlueAOI,
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
        /// 胶水检测工具流程鼠标单击事件
        /// </summary>
        void ListViewToolsOfGlue_MouseUp()
        {
            int index = Model.ToolsOfGlueSelectIndex;
            Console.WriteLine(index);
            if (index < 0 ||
               index >= Model.ToolsOfGlueList.Count)
                return;
            if (Model.ToolsOfGlueList.Count == 1)
                Model.ToolsOfGlueList[index].MenuItemEnable = EumMenuItemEnable.none;
            else
            {

                if (index == 0)
                    Model.ToolsOfGlueList[index].MenuItemEnable = EumMenuItemEnable.first;
                else if (index == Model.ToolsOfGlueList.Count - 1)
                    Model.ToolsOfGlueList[index].MenuItemEnable = EumMenuItemEnable.last;
                else
                    Model.ToolsOfGlueList[index].MenuItemEnable = EumMenuItemEnable.all;
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
                ColorConvertViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("膨胀"))
            {
                FormDilation f = new PositionToolsLib.窗体.Views.FormDilation(tool);
                DilationViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                DilationViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("腐蚀"))
            {
                FormErosion f = new PositionToolsLib.窗体.Views.FormErosion(tool);
                ErosionViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                ErosionViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("开运算"))
            {
                FormOpening f = new PositionToolsLib.窗体.Views.FormOpening(tool);
                OpeningViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                OpeningViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("闭运算"))
            {
                FormClosing f = new PositionToolsLib.窗体.Views.FormClosing(tool);
                ClosingViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                ClosingViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("二值化"))
            {
                FormBinaryzation f = new PositionToolsLib.窗体.Views.FormBinaryzation(tool);
                BinaryzationViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                BinaryzationViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("模板匹配"))
            {
               (tool.GetParam() as PositionToolsLib.参数.MatchParam).RootFolder = saveToUsePath+"\\定位检测\\"+ currModelType.ToString();
                FormMatch f = new PositionToolsLib.窗体.Views.FormMatch(tool);
                MatchViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                MatchViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("查找直线"))
            {
                FormFindLine f = new PositionToolsLib.窗体.Views.FormFindLine(tool);
                FindLineViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                FindLineViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("拟合直线"))
            {
                FormFitLine f = new PositionToolsLib.窗体.Views.FormFitLine(tool);
                FitLineViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                FitLineViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("直线偏移"))
            {
                FormLineOffset f = new PositionToolsLib.窗体.Views.FormLineOffset(tool);
                LineOffsetViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                LineOffsetViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("平行直线"))
            {
                FormCalParallelLine f = new PositionToolsLib.窗体.Views.FormCalParallelLine(tool);
                CalParallelLineViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                CalParallelLineViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("直线中心"))
            {
                FormLineCentre f = new PositionToolsLib.窗体.Views.FormLineCentre(tool);
                LineCentreViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                LineCentreViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("直线交点"))
            {
                FormLineIntersection f = new PositionToolsLib.窗体.Views.FormLineIntersection(tool);
                LineIntersectionViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                LineIntersectionViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("查找圆"))
            {
                FormFindCircle f = new PositionToolsLib.窗体.Views.FormFindCircle(tool);
                FindCircleViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                FindCircleViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("Blob中心"))
            {
                FormBlob f = new PositionToolsLib.窗体.Views.FormBlob(tool);
                BlobViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                BlobViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("坐标换算"))
            {
                FormCoordConvert f = new PositionToolsLib.窗体.Views.FormCoordConvert(tool);
                CoordConvertViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                CoordConvertViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("角度换算"))
            {
                FormAngleConvert f = new PositionToolsLib.窗体.Views.FormAngleConvert(tool);
                AngleConvertViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                AngleConvertViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("畸变校正"))
            {
                FormImageCorrect f = new PositionToolsLib.窗体.Views.FormImageCorrect(tool);
                ImageCorrectViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                ImageCorrectViewModel.This.OnSaveManageHandle = SaveManageOfPos;
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
                ResultShowViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("Tcp接收"))
            {
                FormTcpRecv f = new PositionToolsLib.窗体.Views.FormTcpRecv(tool);
                TcpRecvViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                TcpRecvViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
            else if (toolName.Contains("Tcp发送"))
            {
                FormTcpSend f = new PositionToolsLib.窗体.Views.FormTcpSend(tool);
                TcpSendViewModel.This.OnSaveParamHandle += OnSaveParamEventOfPosition;
                TcpSendViewModel.This.OnSaveManageHandle = SaveManageOfPos;
                f.ShowDialog();
            }
        }
        /// <summary>
        /// 胶水检测工具流程鼠标双击事件
        /// </summary>
        /// <param name="o"></param>
        void ListViewToolsOfGlue_DoubleClick(object o)
        {
            int index = Model.ToolsOfGlueSelectIndex;
            if (index < 0 ||
               index >= Model.ToolsOfGlueList.Count)
                return;
            string name = Model.ToolsOfGlueList[index].ToolName;
            string toolName = projectOfGlue.toolNamesList[index];
            GlueBaseTool tool = projectOfGlue.toolsDic[toolName];
            if (toolName.Contains("颜色转换"))
            {
                GlueDetectionLib.窗体.Views.FormColorConvert f = 
                    new GlueDetectionLib.窗体.Views.FormColorConvert(tool);
                GlueDetectionLib.窗体.ViewModels.ColorConvertViewModel.This.OnSaveParamHandle 
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.ColorConvertViewModel.This.OnSaveManageHandle
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("膨胀"))
            {
                GlueDetectionLib.窗体.Views.FormDilation f =
                    new GlueDetectionLib.窗体.Views.FormDilation(tool);
                GlueDetectionLib.窗体.ViewModels.DilationViewModel.This.OnSaveParamHandle
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.DilationViewModel.This.OnSaveManageHandle 
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("腐蚀"))
            {
                GlueDetectionLib.窗体.Views.FormErosion f =
                    new GlueDetectionLib.窗体.Views.FormErosion(tool);
                GlueDetectionLib.窗体.ViewModels.ErosionViewModel.This.OnSaveParamHandle 
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.ErosionViewModel.This.OnSaveManageHandle 
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("开运算"))
            {
                GlueDetectionLib.窗体.Views.FormOpening f =
                    new GlueDetectionLib.窗体.Views.FormOpening(tool);
                GlueDetectionLib.窗体.ViewModels.OpeningViewModel.This.OnSaveParamHandle 
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.OpeningViewModel.This.OnSaveManageHandle 
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("闭运算"))
            {
                GlueDetectionLib.窗体.Views.FormClosing f =
                    new GlueDetectionLib.窗体.Views.FormClosing(tool);
                GlueDetectionLib.窗体.ViewModels.ClosingViewModel.This.OnSaveParamHandle
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.ClosingViewModel.This.OnSaveManageHandle 
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("二值化"))
            {
                GlueDetectionLib.窗体.Views.FormBinaryzation f =
                    new GlueDetectionLib.窗体.Views.FormBinaryzation(tool);
                GlueDetectionLib.窗体.ViewModels.BinaryzationViewModel.This.OnSaveParamHandle 
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.BinaryzationViewModel.This.OnSaveManageHandle 
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("模板匹配"))
            {
                if (!Directory.Exists(saveToUsePath + "\\胶水检测"))
                    Directory.CreateDirectory(saveToUsePath + "\\胶水检测");
                (tool.GetParam() as GlueDetectionLib.参数.MatchParam).RootFolder = saveToUsePath + "\\胶水检测" ;
                GlueDetectionLib.窗体.Views.FormMatch f =
                    new GlueDetectionLib.窗体.Views.FormMatch(tool);
                GlueDetectionLib.窗体.ViewModels.MatchViewModel.This.OnSaveParamHandle
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.MatchViewModel.This.OnSaveManageHandle 
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("漏胶"))
            {
                GlueDetectionLib.窗体.Views.FormGlueMiss f = 
                    new GlueDetectionLib.窗体.Views.FormGlueMiss(tool);
                GlueDetectionLib.窗体.ViewModels.GlueMissViewModel.This.getPixelRatioHandle 
                    += OnGetPixelRatio;
                GlueDetectionLib.窗体.ViewModels.GlueMissViewModel.This.OnSaveParamHandle 
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.GlueMissViewModel.This.OnSaveManageHandle 
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("偏位"))
            {
                GlueDetectionLib.窗体.Views.FormGlueOffset f =
                    new GlueDetectionLib.窗体.Views.FormGlueOffset(tool);
                GlueDetectionLib.窗体.ViewModels.GlueOffsetViewModel.This.getPixelRatioHandle
                   += OnGetPixelRatio;
                GlueDetectionLib.窗体.ViewModels.GlueOffsetViewModel.This.OnSaveParamHandle 
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.GlueOffsetViewModel.This.OnSaveManageHandle
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("断胶"))
            {
                GlueDetectionLib.窗体.Views.FormGlueGap f =
                    new GlueDetectionLib.窗体.Views.FormGlueGap(tool);
                GlueDetectionLib.窗体.ViewModels.GlueGapViewModel.This.getPixelRatioHandle
                 += OnGetPixelRatio;
                GlueDetectionLib.窗体.ViewModels.GlueGapViewModel.This.OnSaveParamHandle 
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.GlueGapViewModel.This.OnSaveManageHandle 
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("胶宽"))
            {
                GlueDetectionLib.窗体.Views.FormGlueCaliperWidth f = 
                    new GlueDetectionLib.窗体.Views.FormGlueCaliperWidth(tool);
                GlueDetectionLib.窗体.ViewModels.GlueCaliperWidthViewModel .This.getPixelRatioHandle
                    += OnGetPixelRatio;
                GlueDetectionLib.窗体.ViewModels.GlueCaliperWidthViewModel.This.OnSaveParamHandle
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.GlueCaliperWidthViewModel.This.OnSaveManageHandle 
                    = SaveManageOfGlue;
                f.ShowDialog();
            }      
            else if (toolName.Contains("结果显示"))
            {
                GlueDetectionLib.窗体.Views.FormResultShow f = 
                    new GlueDetectionLib.窗体.Views.FormResultShow(tool);
                GlueDetectionLib.窗体.ViewModels.ResultShowViewModel.This.OnSaveParamHandle
                    += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.ResultShowViewModel.This.OnSaveManageHandle
                    = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("Tcp接收"))
            {
                GlueDetectionLib.窗体.Views.FormTcpRecv f =
                         new GlueDetectionLib.窗体.Views.FormTcpRecv(tool);
                GlueDetectionLib.窗体.ViewModels.TcpRecvViewModel.
                         This.OnSaveParamHandle += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.TcpRecvViewModel.
                           This.OnSaveManageHandle = SaveManageOfGlue;
                f.ShowDialog();
            }
            else if (toolName.Contains("Tcp发送"))
            {
                GlueDetectionLib.窗体.Views.FormTcpSend f =
                       new GlueDetectionLib.窗体.Views.FormTcpSend(tool);
                GlueDetectionLib.窗体.ViewModels.TcpSendViewModel.
                        This.OnSaveParamHandle += OnSaveParamEventOfGlue;
                GlueDetectionLib.窗体.ViewModels.TcpSendViewModel.
                           This.OnSaveManageHandle = SaveManageOfGlue;
                f.ShowDialog();
            }
        }
        /// <summary>
        /// 定位检测右键菜单栏
        /// </summary>
        /// <param name="o"></param>
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
                ListViewToolsData temData = Model.ToolsOfPositionList[index];
                Model.ToolsOfPositionList.Insert(index - 1, temData);
                Model.ToolsOfPositionList.RemoveAt(index + 1);

                projectOfPos.toolNamesList.Insert(index - 1, toolName);
                projectOfPos.toolNamesList.RemoveAt(index + 1);

            }
            else if (operateName.Equals("下移"))
            {
                ListViewToolsData temData = Model.ToolsOfPositionList[index];
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
        /// <summary>
        /// 胶水检测右键菜单栏
        /// </summary>
        /// <param name="o"></param>
        void ToolsOfGlue_ContextMenuClick(object o)
        {
            string operateName = ((MenuItem)o).Header.ToString();
            int index = Model.ToolsOfGlueSelectIndex;
            if (index < 0 ||
               index >= Model.ToolsOfGlueList.Count)
                return;

            string toolName = projectOfGlue.toolNamesList[index];
            if (operateName.Equals("上移"))
            {
                ListViewToolsData temData = Model.ToolsOfGlueList[index];
                Model.ToolsOfGlueList.Insert(index - 1, temData);
                Model.ToolsOfGlueList.RemoveAt(index + 1);

                projectOfGlue.toolNamesList.Insert(index - 1, toolName);
                projectOfGlue.toolNamesList.RemoveAt(index + 1);

            }
            else if (operateName.Equals("下移"))
            {
                ListViewToolsData temData = Model.ToolsOfGlueList[index];
                Model.ToolsOfGlueList.Insert(index + 2, temData);
                Model.ToolsOfGlueList.RemoveAt(index);

                projectOfGlue.toolNamesList.Insert(index + 2, toolName);
                projectOfGlue.toolNamesList.RemoveAt(index);

            }
            else if (operateName.Equals("删除"))
            {
                Model.ToolsOfGlueList.RemoveAt(index);
                if (projectOfGlue.dataManage.resultFlagDic.ContainsKey(toolName))
                    projectOfGlue.dataManage.resultFlagDic.Remove(toolName);
                if (projectOfGlue.dataManage.imageBufDic.ContainsKey(toolName))
                    projectOfGlue.dataManage.imageBufDic.Remove(toolName);
                if (projectOfGlue.dataManage.matrixBufDic.ContainsKey(toolName))
                    projectOfGlue.dataManage.matrixBufDic.Remove(toolName);
                if (projectOfGlue.dataManage.enumerableTooDic.Contains(toolName))
                    projectOfGlue.dataManage.enumerableTooDic.Remove(toolName);
                if (projectOfGlue.dataManage.resultBufDic.ContainsKey(toolName))
                    projectOfGlue.dataManage.resultBufDic.Remove(toolName);
                if (projectOfGlue.dataManage.resultInfoDic.ContainsKey(toolName))
                    projectOfGlue.dataManage.resultInfoDic.Remove(toolName);
           
                projectOfGlue.toolsDic.Remove(projectOfGlue.toolNamesList[index]);
                projectOfGlue.toolNamesList.RemoveAt(index);

            }
            else if (operateName.Equals("修改备注"))
            {

                FormRemarks f = new FormRemarks();
                f.Topmost = true;
                if (f.ShowDialog().Value)
                {
                    string remark = f.remarks;

                    Model.ToolsOfGlueList[index].ToolNotes = remark;

                    if (projectOfGlue.toolsDic.ContainsKey(toolName))
                        projectOfGlue.toolsDic[toolName].remark = remark;

                }
            }
        }
        /// <summary>
        /// 鼠标双击获取图像像素坐标点及灰度值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="gray"></param>
        void DoubleClickGetMousePosEvent(HTuple x, HTuple y, HTuple gray)
        {
            DoubleClickGetMousePosHandle?.Invoke(x, y, gray);

        }
        /// <summary>
        /// 打开相机按钮单击事件
        /// </summary>
        void OpenCam_Click()
        {
            if (CurrCam == null)
            {
                Appentxt(string.Format("工位：{0}，相机初始化失败，无法打开！", currCamStationName));
                //MessageBox.Show(string.Format("工位：{0}，相机初始化失败，无法打开！", currCamStationName), "Information",
                //                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Model.BtnOpenCamEnable = false;
            string msg = string.Empty;
            bool initFlag = false;
            initFlag = CurrCam.OpenCam(camIndex, ref msg);
            if (initFlag)
            {
                imageWidth = CurrCam.ImageWidth;
                imageHeight = CurrCam.ImageHeight;
                //曝光值范围获取
                bool getFlag = CurrCam.GetExposureRangeValue(out long minExposure, out long maxExposure);
                if (!getFlag)
                    Appentxt("曝光参数设置范围值获取失败！");
                else
                {
                    //相机曝光范围值设置
                    Model.ExpouseMaxValue = maxExposure;
                    Model.ExpouseMinValue = minExposure;
                    if (currCamExpouse > maxExposure)
                        currCamExpouse = maxExposure;
                    else if (currCamExpouse < minExposure)
                        currCamExpouse = minExposure;

                    //相机曝光设置 
                    Model.ExpouseSliderValue = currCamExpouse;
                    Model.ExpouseNumricValue = currCamExpouse;
                    bool flag = CurrCam.SetExposureTime(currCamExpouse);
                    if (!flag)
                        Appentxt(string.Format("工位：{0}，相机曝光设置失败！", currCamStationName));

                }

                //增益值范围获取
                bool getFlag2 = CurrCam.GetGainRangeValue(out long minGain, out long maxGain);
                if (!getFlag)
                    Appentxt("增益参数设置范围值获取失败！");
                {
                    //相机增益范围值设置
                    Model.GainMaxValue = (int)maxGain;
                    Model.GainMinValue = (int)minGain;
                    if (currCamGain > maxGain)
                        currCamGain = (int)maxGain;
                    else if (currCamGain < minGain)
                        currCamGain = (int)minGain;
                    //相机增益设置 
                    Model.GainSliderValue = currCamGain;
                    Model.GainNumricValue = currCamGain;
                    bool flag = CurrCam.SetGain(currCamGain);
                    if (!flag)
                        Appentxt(string.Format("工位：{0}，相机增益设置失败！", currCamStationName));
                }

                EnableCam(true);
                workstatus = EunmCamWorkStatus.Freestyle;
            }
            else
            {
                EnableCam(false);
                Appentxt(string.Format("工位：{0}，相机打开失败：{1}", currCamStationName, msg));
            }
        }
        /// <summary>
        /// 关闭相机按钮单击事件
        /// </summary>
        void CloseCam_Click()
        {
            Model.BtnCloseCamEnable = false;

            Task.Run(() =>
            {
                CurrCam.CloseCam();
            }).
                ContinueWith(t =>
                {
                    EnableCam(false);
                });

        }
        /// <summary>
        /// 相机单帧采集
        /// </summary>
        private void btnOneShot_Click()
        {
            workstatus = EunmCamWorkStatus.Freestyle;
            CurrCam.OneShot();
            camContinueGrabHandle?.Invoke(false);
        }
        /// <summary>
        /// 相机连续采集
        /// </summary>
        private void btnContinueGrab_Click()
        {
            workstatus = EunmCamWorkStatus.Freestyle;
            CurrCam.ContinueGrab();         
            Model.BtnOneShotEnable = false;
            Model.BtnContinueGrabEnable = false;
            Model.BtnStopGrabEnable = true;
            camContinueGrabHandle?.Invoke(true);
            ShowTool.SetEnable(false);
        }
        /// <summary>
        /// 相机参数保存
        /// </summary>
        private void btnSaveCamParma_Click()
        {
            //相机参数
            string path = saveToUsePath + "\\定位检测\\" +
                       Enum.GetName(typeof(EumModelType), currModelType);
            if (Directory.Exists(path))
                Directory.CreateDirectory(path);         
            GeneralUse.WriteValue("相机", "曝光", currCamExpouse.ToString(),
                   "config", path);
            GeneralUse.WriteValue("相机", "增益", currCamGain.ToString(),
                "config", path);
        
            //相机型号       
            GeneralUse.WriteValue("相机", "型号", currCamType.ToString(),
                     "config", rootFolder + "\\Config");
            GeneralUse.WriteValue("相机", "索引", camIndex.ToString(),
                "config", rootFolder + "\\Config");
            
            GeneralUse.WriteValue("自动对焦", "偏差阈值", DeviationThd.ToString(),
               "config", rootFolder + "\\Config");

            GeneralUse.WriteValue("自动对焦", "限位阈值", LimitMethd.ToString(),
                "config", rootFolder + "\\Config");

        }
        /// <summary>
        /// 相机停止采集
        /// </summary>
        private void btnStopGrab_Click()
        {
            CurrCam.StopGrab();
            Model.BtnOneShotEnable = true;
            Model.BtnContinueGrabEnable = true;
            Model.BtnStopGrabEnable = false;        
            camContinueGrabHandle?.Invoke(false);
            ShowTool.SetEnable(true);
        }
        /// <summary>
        /// 相机曝光值设置：slider bar
        /// </summary>
        /// <param name="obj"></param>
        void ExpouseValueChanged(object obj)
        {
            currCamExpouse = Model.ExpouseSliderValue;
            Model.ExpouseNumricValue = currCamExpouse;
            if (CurrCam != null)
                if (CurrCam.IsAlive)
                {
                    bool flag = CurrCam.SetExposureTime(currCamExpouse);
                    if (!flag)
                        Appentxt("相机曝光设置失败！");
                    //MessageBox.Show("相机曝光设置失败！");
                }
        }
        /// <summary>
        /// 相机曝光值输入
        /// </summary>
        /// <param name="obj"></param>
        void ExpouseNumericKeyDown(object obj)
        {
            KeyEventArgs args = (KeyEventArgs)obj;
            if (args.Key == Key.Enter)
            {
                currCamExpouse = Model.ExpouseNumricValue;
                Model.ExpouseSliderValue = currCamExpouse;
                if (CurrCam != null)
                    if (CurrCam.IsAlive)
                    {
                        bool flag = CurrCam.SetExposureTime(currCamExpouse);
                        if (!flag)
                            Appentxt("相机曝光设置失败！");
                        //MessageBox.Show("相机曝光设置失败！");
                    }
            }
        }
        void ExpouseNumericEvent()
        {
            currCamExpouse = Model.ExpouseNumricValue;
            Model.ExpouseSliderValue = currCamExpouse;
            if (CurrCam != null)
                if (CurrCam.IsAlive)
                {
                    bool flag = CurrCam.SetExposureTime(currCamExpouse);
                    if (!flag)
                        Appentxt("相机曝光设置失败！");
                    //MessageBox.Show("相机曝光设置失败！");
                }

        }

        /// <summary>
        /// 相机增益值设置：slider bar
        /// </summary>
        /// <param name="obj"></param>
        void GainValueChanged(object obj)
        {
            currCamGain = Model.GainSliderValue;
            Model.GainNumricValue = currCamGain;
            if (CurrCam != null)
                if (CurrCam.IsAlive)
                {
                    bool flag = CurrCam.SetGain(currCamGain);
                    if (!flag)
                        Appentxt("相机增益设置失败！");
                  
                }
        }
        /// <summary>
        /// 相机增益值输入
        /// </summary>
        /// <param name="obj"></param>
        void GainNumericKeyDown(object obj)
        {
            KeyEventArgs args = (KeyEventArgs)obj;
            if (args.Key == Key.Enter)
            {
                currCamGain = Model.GainNumricValue;
                Model.GainSliderValue = currCamGain;
                if (CurrCam != null)
                    if (CurrCam.IsAlive)
                    {
                        bool flag = CurrCam.SetGain(currCamGain);
                        if (!flag)
                            Appentxt("相机增益设置失败！");
                    }
            }
        }
        void GainNumericEvent()
        {
            currCamGain = Model.GainNumricValue;
            Model.GainSliderValue = currCamGain;
            if (CurrCam != null)
                if (CurrCam.IsAlive)
                {
                    bool flag = CurrCam.SetGain(currCamGain);
                    if (!flag)
                        Appentxt("相机增益设置失败！");
                }
        }
        /// <summary>
        /// 相机型号切换
        /// </summary>
        /// <param name="o"></param>
        void cobxCamType_SelectedIndexChanged(object o)
        {
            if(currCamType==(CamType) Model.SelectCamTypeIndex)
            {
                Appentxt("相机类型相同无需切换");
                return;
            }
                
            Model.CamIndexerList.Clear();
            currCamType = (CamType)Model.SelectCamTypeIndex;
            //先注销再实例化
            if (CurrCam != null)
            {
                if (CurrCam.IsAlive)
                    CurrCam.CloseCam();
                CurrCam.setImgGetHandle -= GetImageDelegate;//先关闭再注销掉             
                CurrCam.CamConnectHnadle -= CamConnectEvent;
                CurrCam.Dispose();
                CurrCam = null;
            }
            #region  相机实例化
            switch (currCamType)
            {
                case CamType.海康:

                    CurrCam = new HKCam("HKCam");
                    this.currCamType = CamType.海康;
                    break;
                case CamType.凌云:

                    CurrCam = new LusterCam("LusterCam");
                    this.currCamType = CamType.凌云;
                    break;
                case CamType.度申:
                    CurrCam = new DVPCam("DVPCam");
                    this.currCamType = CamType.度申;
                    break;
                case CamType.康耐视:

                    CurrCam = new CognexCam("CognexCam");
                    this.currCamType = CamType.康耐视;
                    break;
                case CamType.大华:

                    CurrCam = new DaHuaCam("DaHuaCam");
                    this.currCamType = CamType.大华;
                    break;
                case CamType.巴斯勒:

                    CurrCam = new BaslerCam("BaslerCam");
                    this.currCamType = CamType.巴斯勒;
                    break;
                case CamType.奥普特:

                    CurrCam = new OPTCam("OPTCam");
                    this.currCamType = CamType.奥普特;
                    break;
            }

            #endregion
            if (CurrCam == null)
            {
                Appentxt("相机实例化对象为空！");
                return;
            }
            for (int i = 0; i < CurrCam.CamNum; i++)
                Model.CamIndexerList.Add(i);
            if (Model.CamIndexerList.Count>0)
                Model.SelectCamIndexerIndex = camIndex=0;
            //注册相机图像采集事件
            CurrCam.setImgGetHandle += new ImgGetHandle(GetImageDelegate);
            CurrCam.CamConnectHnadle += new EventHandler(CamConnectEvent);


        }
        /// <summary>
        /// 相机索引切换
        /// </summary>
        void cobxCamIndex_SelectedIndexChanged(object o)
        {
            if (camIndex == Model.SelectCamTypeIndex)
            {
                Appentxt("相机索引相同无需切换");
                return;
            }
            camIndex = Model.SelectCamTypeIndex;
        }

        void NumDeviationThd_KeyDown(object o)
        {
            KeyEventArgs args = (KeyEventArgs)o;
            if (args.Key == Key.Enter)
            {
                DeviationThd = Model.NumDeviationThd;
                GeneralUse.WriteValue("自动对焦", "偏差阈值", DeviationThd.ToString()
                    , "config", rootFolder + "\\Config");
            }
        }
        void DeviationThdNumericEvent()
        {
            DeviationThd = Model.NumDeviationThd;
            GeneralUse.WriteValue("自动对焦", "偏差阈值", DeviationThd.ToString()
                , "config", rootFolder + "\\Config");
        }

        void NumLimitMethd_KeyDown(object o)
        {
            KeyEventArgs args = (KeyEventArgs)o;
            if (args.Key == Key.Enter)
            {
                LimitMethd = Model.NumLimitMethd;
                GeneralUse.WriteValue("自动对焦", "限位阈值", LimitMethd.ToString(), 
                    "config", rootFolder + "\\Config");
            }
        }

        void LimitMethdNumericEvent()
        {
            LimitMethd = Model.NumLimitMethd;
            GeneralUse.WriteValue("自动对焦", "限位阈值", LimitMethd.ToString(),
                "config", rootFolder + "\\Config");
        }

        /// <summary>
        /// 辅助工具
        /// </summary>
        /// <param name="o"></param>
        private void rdbtn_CheckedChanged(object o)
        {
            AddAssistToolToCross();

        }
        /// <summary>
        /// 辅助工具
        /// </summary>
        /// <param name="o"></param>
        void AssistCircleKeyDown(object o)
        {
            KeyEventArgs args = (KeyEventArgs)o;
            if (args.Key == Key.Enter)
                  AddAssistToolToCross();
        }
        /// <summary>
        ///辅助工具
        /// </summary>
        void AssistCircleEvent()
        {
            AddAssistToolToCross();
        }
        /// <summary>
        /// 辅助工具
        /// </summary>
        void ScaleRule_CheckedChanged()
        {
            AddAssistToolToCross();
        }

        #endregion

        #region--------------定位检测---------------
        public PosDataManage GetManageOfPos()
        {
            return this.projectOfPos.dataManage;
        }
        void SaveManageOfPos(PosDataManage manage)
        {
            this.projectOfPos.dataManage = manage;
        }
       
        /// <summary>
        /// 定位检测参数保存
        /// </summary>
        /// <param name="toolName"></param>
        /// <param name="par"></param>
        void OnSaveParamEventOfPosition(string toolName, PosBaseParam par)
        {
            if (projectOfPos.toolNamesList.Contains(toolName))
            {
                PosBaseTool tool = projectOfPos.toolsDic[toolName];
                tool.SetParam(par);
                projectOfPos.toolsDic[toolName] = tool;
               
                if (tool.GetType() == typeof(PosTcpRecvTool))
                {
                    if (projectOfPos.TcpRecvName== ((PosTcpRecvTool)tool).CommDevName)return;

                        //如果存在先注销事件
                     if (CommDeviceController.g_CommDeviceList.Exists(t => t.m_Name.Equals(projectOfPos.TcpRecvName)))
                    {
                        int index = CommDeviceController.g_CommDeviceList.
                                       FindIndex(t => t.m_Name.Equals(projectOfPos.TcpRecvName));

                        //先清除旧的事件订阅
                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect -= PosTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= PosTcpServer_ReceiveData;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= PosTcpServer_RemoteClose;
                        }                        
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= PosTcpClient_ReceiveData;
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= PosTcpClient_RemoteClose;
                        }
                          
                    }
                    //重新订阅事件
                    projectOfPos.TcpRecvName = ((PosTcpRecvTool)tool).CommDevName;
                    int e_index = CommDeviceController.g_CommDeviceList.
                                       FindIndex(t => t.m_Name.Equals(projectOfPos.TcpRecvName));
                    if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == (CommDevType.TcpServer))
                    {
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteConnect += PosTcpServer_RemoteConnect;
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).ReceiveData += PosTcpServer_ReceiveData;
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += PosTcpServer_RemoteClose;
                    }
                    else if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == CommDevType.TcpClient)
                    {
                        ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).ReceiveData += PosTcpClient_ReceiveData;
                        ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += PosTcpClient_RemoteClose;
                    }


                }
                else if (tool.GetType() == typeof(PosTcpSendTool))
                {
                    if (projectOfPos.TcpSendName == ((PosTcpSendTool)tool).CommDevName) return;
                    if (projectOfPos.TcpSendName == projectOfPos.TcpRecvName) return;
                    //如果存在先注销事件
                    if (CommDeviceController.g_CommDeviceList.Exists(t => t.m_Name.Equals(projectOfPos.TcpSendName)))
                    {                      
                        int index = CommDeviceController.g_CommDeviceList.
                                       FindIndex(t => t.m_Name.Equals(projectOfPos.TcpSendName));

                        //先清除旧的事件订阅
                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect -= PosTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= PosTcpServer_RemoteClose;
                        }
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= PosTcpClient_RemoteClose;
                        }

                    }
                    //重新订阅事件
                    projectOfPos.TcpSendName = ((PosTcpSendTool)tool).CommDevName;
                    int e_index = CommDeviceController.g_CommDeviceList.
                                       FindIndex(t => t.m_Name.Equals(projectOfPos.TcpSendName));
                    if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == (CommDevType.TcpServer))
                    {
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteConnect += PosTcpServer_RemoteConnect;
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += PosTcpServer_RemoteClose;
                    }
                    else if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == CommDevType.TcpClient)
                    {
                        ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += PosTcpClient_RemoteClose;
                    }


                }
            }
        }
        #endregion

        #region--------------胶水AOI---------------

        public GlueDataManage GetManageOfGlue()
        {
            return this.projectOfGlue.dataManage;
        }

        public void SaveManageOfGlue(GlueDataManage data)
        {
            this.projectOfGlue.dataManage = data;
        }
        /// <summary>
        /// 胶水检测参数保存
        /// </summary>
        /// <param name="toolName"></param>
        /// <param name="par"></param>
        void OnSaveParamEventOfGlue(string toolName, GlueBaseParam par)
        {
            if (projectOfGlue.toolNamesList.Contains(toolName))
            {
                GlueBaseTool tool = projectOfGlue.toolsDic[toolName];
                tool.SetParam(par);
                projectOfGlue.toolsDic[toolName] = tool;
                if (tool.GetType() == typeof(PosTcpRecvTool))
                {
                    if (projectOfGlue.TcpRecvName == ((GlueTcpRecvTool)tool).CommDevName) return;

                    //如果存在先注销事件
                    if (CommDeviceController.g_CommDeviceList.Exists(t => t.m_Name.Equals(projectOfGlue.TcpRecvName)))
                    {
                        int index = CommDeviceController.g_CommDeviceList.
                                       FindIndex(t => t.m_Name.Equals(projectOfGlue.TcpRecvName));

                        //先清除旧的事件订阅
                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect -= GlueTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= GlueTcpServer_ReceiveData;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= GlueTcpServer_RemoteClose;
                        }
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= GlueTcpClient_ReceiveData;
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= GlueTcpClient_RemoteClose;
                        }

                    }
                    //重新订阅事件
                    projectOfGlue.TcpRecvName = ((GlueTcpRecvTool)tool).CommDevName;
                    int e_index = CommDeviceController.g_CommDeviceList.
                                       FindIndex(t => t.m_Name.Equals(projectOfGlue.TcpRecvName));
                    if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == (CommDevType.TcpServer))
                    {
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteConnect += GlueTcpServer_RemoteConnect;
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).ReceiveData += GlueTcpServer_ReceiveData;
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += GlueTcpServer_RemoteClose;
                    }
                    else if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == CommDevType.TcpClient)
                    {
                        ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).ReceiveData += GlueTcpClient_ReceiveData;
                        ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += GlueTcpClient_RemoteClose;
                    }


                }
                else if (tool.GetType() == typeof(GlueTcpSendTool))
                {
                    if (projectOfGlue.TcpSendName == ((GlueTcpSendTool)tool).CommDevName) return;
                    if (projectOfGlue.TcpSendName == projectOfGlue.TcpRecvName) return;
                    //如果存在先注销事件
                    if (CommDeviceController.g_CommDeviceList.Exists(t => t.m_Name.Equals(projectOfGlue.TcpSendName)))
                    {
                        int index = CommDeviceController.g_CommDeviceList.
                                       FindIndex(t => t.m_Name.Equals(projectOfGlue.TcpSendName));

                        //先清除旧的事件订阅
                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect -= GlueTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= GlueTcpServer_RemoteClose;
                        }
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= GlueTcpClient_RemoteClose;
                        }

                    }
                    //重新订阅事件
                    projectOfGlue.TcpSendName = ((GlueTcpSendTool)tool).CommDevName;
                    int e_index = CommDeviceController.g_CommDeviceList.
                                       FindIndex(t => t.m_Name.Equals(projectOfGlue.TcpSendName));
                    if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == (CommDevType.TcpServer))
                    {
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteConnect += PosTcpServer_RemoteConnect;
                        ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += PosTcpServer_RemoteClose;
                    }
                    else if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == CommDevType.TcpClient)
                    {
                        ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += PosTcpClient_RemoteClose;
                    }


                }
            }
        }
        #endregion

        #region--------------配方---------------
        /// <summary>
        /// 新建配方
        /// </summary>
        void NewRecipe_Click()
        {
            Model.RecipeDgList.Add(new RecipeDg());
        }
        /// <summary>
        /// 删除配方
        /// </summary>
        private void DeleteRecipe_Click()
        {
            int index = Model.RecipeDgSelectIndex;
            if (index < 0||index>= Model.RecipeDgList.Count)
                return;
            Model.RecipeDgList.RemoveAt(index);
        }
        /// <summary>
        /// 保存配方
        /// </summary>
        private void SaveRecipe_Click()
        {
           
            string path = rootFolder + "\\Config\\配方.rep";
            DirectoryInfo[] dis = new DirectoryInfo(rootFolder + "\\配方").GetDirectories();

            string temRecipeName = "";
            //删除配方中不存在的文件
            foreach(var p in dis)
            {
                if (!Model.RecipeDgList.ToList().Exists(t => t.Name == p.Name))
                    Directory.Delete(rootFolder + "\\配方\\" + p.Name,true);
            }
            int checkC = 0;
            foreach (var s in Model.RecipeDgList)
            {
                if (!Directory.Exists(rootFolder + "\\配方\\" + s.Name))
                    Directory.CreateDirectory(rootFolder + "\\配方\\" + s.Name);
                if (s.IsUse)
                {
                    temRecipeName = s.Name;
                    checkC++;
                }                 
            }   
            if (checkC!=1)
            {
                MessageBox.Show("请确认是否有启用一个配方...");
                Appentxt("请确认是否有启用一个配方...");
                return;
            }
            ObservableCollection<RecipeDgBuf> RecipeDgListBuf = new ObservableCollection<RecipeDgBuf>();
            foreach (var s in Model.RecipeDgList)
                RecipeDgListBuf.Add(new RecipeDgBuf(s.Name, s.IsUse));
            GeneralUse.WriteSerializationFile<ObservableCollection<RecipeDgBuf>>
                   (path, RecipeDgListBuf);
            if(temRecipeName!="")
                if(temRecipeName!= currRecipeName)
                {
                    currRecipeName = temRecipeName;
                    saveToUsePath = rootFolder + "\\配方\\" + currRecipeName;
                    LoadRecipe();
                }
        }
        #endregion

        #region   Method
        /// <summary>
        /// 加载定位检测流程
        /// </summary>
        /// <param name="secondName"></param>
        /// <returns></returns>
         bool LoadPositionFlow(string secondName)
        {
            try
            {
                ResetNumOfPos();
                #region 需要先注销定位检测TCP
             
                foreach (var item in this.projectOfPos.toolsDic)
                {
                    if (item.Value.GetType() == typeof(PosTcpRecvTool))
                    {
                        projectOfPos.TcpRecvName = ((PosTcpRecvTool)item.Value).CommDevName;
                        int index = CommDeviceController.g_CommDeviceList.
                                              FindIndex(t => t.m_Name == projectOfPos.TcpRecvName);
                        if (index < 0) continue;

                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpServer)
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect -= PosTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= PosTcpServer_ReceiveData;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= PosTcpServer_RemoteClose;

                        }
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= PosTcpClient_ReceiveData;
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= PosTcpClient_RemoteClose;

                        }
                    }
                    else if (item.Value.GetType() == typeof(PosTcpSendTool))
                    {
                     
                        projectOfPos.TcpSendName = ((PosTcpSendTool)item.Value).CommDevName;
                        //输出与输入是同一个TCP
                        if (projectOfPos.TcpSendName == projectOfPos.TcpRecvName) continue;

                        int index = CommDeviceController.g_CommDeviceList.
                                              FindIndex(t => t.m_Name == projectOfPos.TcpSendName);
                        if (index < 0) continue;

                        //先清除旧的事件订阅
                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect -= PosTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= PosTcpServer_RemoteClose;
                        }
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= PosTcpClient_RemoteClose;
                        }
                    }
                }
                #endregion
                this.projectOfPos = GeneralUse.ReadSerializationFile<ProjectOfPosition>
                    (saveToUsePath + "\\" + "定位检测" + "\\" + secondName + ".proj");
                projectOfPos.GetNum();
                projectOfPos.toolNamesList = new List<string>();
                if (projectOfPos.dataManage == null)
                    projectOfPos.dataManage = new PosDataManage();
                Dictionary<string, PosBaseTool> tem = new Dictionary<string, PosBaseTool>();

                foreach (var s in projectOfPos.toolsDic)
                {
                    projectOfPos.toolNamesList.Add(s.Value.GetToolName());
                    tem.Add(s.Value.GetToolName(), s.Value);
                    s.Value.OnGetManageHandle = new PosBaseTool.GetManageHandle(GetManageOfPos);
                    if (s.Value.GetType() == typeof(ImageCorrectTool))
                        continue;
                    s.Value.SetMatrix(hv_HomMat2D);
                    s.Value.SetCalibFilePath(NineCalibFile);

                }
                projectOfPos.toolsDic = tem;
                ShowTestFlowOfPosition();
                //添加图像到缓存集合
                if (PosBaseTool.ObjectValided(this.GrabImg))
                    if (!projectOfPos.dataManage.imageBufDic.ContainsKey("原始图像"))
                        projectOfPos.dataManage.imageBufDic.Add("原始图像", this.GrabImg.Clone());
                    else
                        projectOfPos.dataManage.imageBufDic["原始图像"] = this.GrabImg.Clone();

                #region 后订阅+定位检测TCP
              
                foreach (var item in this.projectOfPos.toolsDic)
                {
                    if (item.Value.GetType() == typeof(PosTcpRecvTool))
                    {
                        projectOfPos.TcpRecvName = ((PosTcpRecvTool)item.Value).CommDevName;
                        int index = CommDeviceController.g_CommDeviceList.
                                              FindIndex(t => t.m_Name == projectOfPos.TcpRecvName);
                        if (index < 0) continue;

                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpServer)
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect += PosTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData += PosTcpServer_ReceiveData;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose += PosTcpServer_RemoteClose;

                        }
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData += PosTcpClient_ReceiveData;
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose += PosTcpClient_RemoteClose;

                        }
                    }
                    else if (item.Value.GetType() == typeof(PosTcpSendTool))
                    {
                        //重新订阅事件
                        projectOfPos.TcpSendName = ((PosTcpSendTool)item.Value).CommDevName;
                        //输出与输入是同一个TCP
                        if (projectOfPos.TcpSendName == projectOfPos.TcpRecvName) continue;

                        int e_index = CommDeviceController.g_CommDeviceList.
                                           FindIndex(t => t.m_Name.Equals(projectOfPos.TcpSendName));
                        if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == (CommDevType.TcpServer))
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteConnect += PosTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += PosTcpServer_RemoteClose;
                        }
                        else if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += PosTcpClient_RemoteClose;
                        }
                    }
                }
                #endregion

            }
            catch (Exception er)
            {
                Appentxt("定位检测流程加载失败，异常信息:" + er.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 加载胶水检测流程
        /// </summary>
        /// <returns></returns>
        bool LoadGlueAoiFlow()
        {

            try
            {

                ResetNumOfGlue();
                #region 需要先注销胶水检测TCP

                foreach (var item in this.projectOfGlue.toolsDic)
                {
                    if (item.Value.GetType() == typeof(GlueTcpRecvTool))
                    {
                        projectOfGlue.TcpRecvName = ((GlueTcpRecvTool)item.Value).CommDevName;
                        int index = CommDeviceController.g_CommDeviceList.
                                              FindIndex(t => t.m_Name == projectOfGlue.TcpRecvName);
                        if (index < 0) continue;

                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpServer)
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect -= GlueTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= GlueTcpServer_ReceiveData;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= GlueTcpServer_RemoteClose;

                        }
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData -= GlueTcpClient_ReceiveData;
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= GlueTcpClient_RemoteClose;

                        }
                    }
                    else if (item.Value.GetType() == typeof(GlueTcpSendTool))
                    {

                        projectOfGlue.TcpSendName = ((GlueTcpSendTool)item.Value).CommDevName;
                        //输出与输入是同一个TCP
                        if (projectOfGlue.TcpSendName == projectOfGlue.TcpRecvName) continue;

                        int index = CommDeviceController.g_CommDeviceList.
                                              FindIndex(t => t.m_Name == projectOfGlue.TcpSendName);
                        if (index < 0) continue;

                        //先清除旧的事件订阅
                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == (CommDevType.TcpServer))
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect -= GlueTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= GlueTcpServer_RemoteClose;
                        }
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose -= GlueTcpClient_RemoteClose;
                        }
                    }
                }
                #endregion
                this.projectOfGlue = GeneralUse.ReadSerializationFile<ProjectOfGlue>
                    (saveToUsePath + "\\" + "胶水检测" + "\\" + "AOI.proj");
                projectOfGlue.GetNum();
                projectOfGlue.toolNamesList = new List<string>();
                if (projectOfGlue.dataManage == null)
                    projectOfGlue.dataManage = new GlueDataManage();
                Dictionary<string, GlueBaseTool> tem = new Dictionary<string, GlueBaseTool>();

                foreach (var s in projectOfGlue.toolsDic)
                {
                    projectOfGlue.toolNamesList.Add(s.Value.GetToolName());
                    tem.Add(s.Value.GetToolName(), s.Value);
                    s.Value.OnGetManageHandle = new GlueBaseTool.GetManageHandle(GetManageOfGlue);
                }
                projectOfGlue.toolsDic = tem;
                ShowTestFlowOfGlue();
                //添加图像到缓存集合
                if (GlueBaseTool.ObjectValided(this.GrabImg))
                    if (!projectOfGlue.dataManage.imageBufDic.ContainsKey("原始图像"))
                        projectOfGlue.dataManage.imageBufDic.Add("原始图像", this.GrabImg.Clone());
                    else
                        projectOfGlue.dataManage.imageBufDic["原始图像"] = this.GrabImg.Clone();

                #region 后订阅+胶水检测TCP

                foreach (var item in this.projectOfGlue.toolsDic)
                {
                    if (item.Value.GetType() == typeof(GlueTcpRecvTool))
                    {
                        projectOfGlue.TcpRecvName = ((GlueTcpRecvTool)item.Value).CommDevName;
                        int index = CommDeviceController.g_CommDeviceList.
                                              FindIndex(t => t.m_Name == projectOfGlue.TcpRecvName);
                        if (index < 0) continue;

                        if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpServer)
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteConnect += GlueTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData += GlueTcpServer_ReceiveData;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose += GlueTcpServer_RemoteClose;

                        }
                        else if (CommDeviceController.g_CommDeviceList[index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).ReceiveData += GlueTcpClient_ReceiveData;
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[index].obj).RemoteClose += GlueTcpClient_RemoteClose;

                        }
                    }
                    else if (item.Value.GetType() == typeof(GlueTcpSendTool))
                    {
                        //重新订阅事件
                        projectOfGlue.TcpSendName = ((GlueTcpSendTool)item.Value).CommDevName;
                        //输出与输入是同一个TCP
                        if (projectOfGlue.TcpSendName == projectOfGlue.TcpRecvName) continue;

                        int e_index = CommDeviceController.g_CommDeviceList.
                                           FindIndex(t => t.m_Name.Equals(projectOfGlue.TcpSendName));
                        if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == (CommDevType.TcpServer))
                        {
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteConnect += GlueTcpServer_RemoteConnect;
                            ((TcpSocketServer)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += GlueTcpServer_RemoteClose;
                        }
                        else if (CommDeviceController.g_CommDeviceList[e_index].m_CommDevType == CommDevType.TcpClient)
                        {
                            ((TcpSocketClient)CommDeviceController.g_CommDeviceList[e_index].obj).RemoteClose += GlueTcpClient_RemoteClose;
                        }
                    }

                }
                #endregion
            }
            catch (Exception er)
            {
                Appentxt("胶水检测流程加载失败，异常信息:" + er.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 加载配方
        /// </summary>
        void LoadRecipeFile()
        {
            bool loadFlag = false;
            string path = rootFolder + "\\Config\\配方.rep";
            if (File.Exists(path))
            {               
                ObservableCollection<RecipeDgBuf> RecipeDgListBuf = new ObservableCollection<RecipeDgBuf>();
                RecipeDgListBuf = GeneralUse.ReadSerializationFile<ObservableCollection<RecipeDgBuf>>(path);
                Model.RecipeDgList.Clear();
                foreach (var s in RecipeDgListBuf)
                    Model.RecipeDgList.Add(new RecipeDg(s.Name, s.IsUse));         
            }
               
           if(Model.RecipeDgList.Count<=0)
            {
                Appentxt("当前无可用配方");
                return;
            }
            foreach (var s in Model.RecipeDgList)
                if (s.IsUse)
                {
                    loadFlag = true;
                    currRecipeName = s.Name;
                    break;
                }
            //默认自启动配方第一个
            if (!loadFlag)
            {
                Model.RecipeDgList[0].IsUse = true;
                currRecipeName = Model.RecipeDgList[0].Name;
            }               
            Model.RecipeDgSelectIndex = -1;
        }
        
        /// <summary>
        /// 加载配方
        /// </summary>
        void LoadRecipe()
        {
            //文件夹名称
            string firstName = "定位检测";
            if (!Directory.Exists(saveToUsePath + "\\" + firstName))
                Directory.CreateDirectory(saveToUsePath + "\\" + firstName);
            string secondName = GeneralUse.ReadValue("定位检测", "模板类型", "config", "ProductModel_1",
                          saveToUsePath + "\\" + firstName);
            currModelType = (EumModelType)Enum.Parse(typeof(EumModelType), secondName);
            Model.ModelType = currModelType;//当前模板类型
            Model.ModelTypeSelectIndex = (int)currModelType;
            if (currModelType == EumModelType.CaliBoardModel)
                f_NinePointsCalib = new FormNinePointsCalib(rootFolder,currCalibName);
            LoadPositionFlow(secondName);//加载定位检测流程
            LoadGlueAoiFlow();//加载胶水检测流程
            Appentxt(string.Format("当前加载配方：{0}，模板：{1}", currRecipeName, secondName));
            if (PosBaseTool.ObjectValided(this.GrabImg))
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(this.GrabImg);
                ShowTool.D_HImage = this.GrabImg;
            }
        }
        /// <summary>
        /// 加载相机参数(切换模板时重新加载相机曝光增益参数)
        /// </summary>
        /// <param name="path"></param>
        void LoadCamParam()
        {          
            //相机参数
            string path = saveToUsePath + "\\定位检测\\" +
                       Enum.GetName(typeof(EumModelType), currModelType);
            if (Directory.Exists(path))
                Directory.CreateDirectory(path);
            currCamExpouse = long.Parse(GeneralUse.ReadValue("相机", "曝光", "config", "1000", path));
            currCamGain = int.Parse(GeneralUse.ReadValue("相机", "增益", "config", "0", path));
            if (CurrCam != null)
            {
                if (currCamExpouse > Model.ExpouseMaxValue)
                    currCamExpouse = Model.ExpouseMaxValue;
                else if (currCamExpouse < Model.ExpouseMinValue)
                    currCamExpouse = Model.ExpouseMinValue;
                if (currCamExpouse < 1000) currCamExpouse = 1000;
                //相机曝光设置 
                Model.ExpouseSliderValue = currCamExpouse;
                Model.ExpouseNumricValue = currCamExpouse;
                bool expouseFlag = CurrCam.SetExposureTime(currCamExpouse);
                if (!expouseFlag)
                    Appentxt(string.Format("工位：{0}，相机曝光设置失败！", currCamStationName));

                if (currCamGain > Model.GainMaxValue)
                    currCamGain = Model.GainMaxValue;
                else if (currCamGain < Model.GainMinValue)
                    currCamGain = Model.GainMinValue;
                //相机增益设置 
                Model.GainSliderValue = currCamGain;
                Model.GainNumricValue = currCamGain;
                bool gainFlag = CurrCam.SetGain(currCamGain);
                if (!gainFlag)
                    Appentxt(string.Format("工位：{0}，相机增益设置失败！", currCamStationName));

            }
        }
        /// <summary>
        /// 加载相机
        /// </summary>
        void LoadCamera()
        {
            //相机参数
            string path = saveToUsePath + "\\定位检测\\"+
                       Enum.GetName(typeof(EumModelType), currModelType);
            if (Directory.Exists(path))
                Directory.CreateDirectory(path);
            currCamExpouse =long.Parse( GeneralUse.ReadValue("相机", "曝光", "config", "1000", path));
            currCamGain = int.Parse(GeneralUse.ReadValue("相机", "增益", "config", "0", path));

            //相机型号       
            currCamType = (CamType)Enum.Parse(typeof(CamType),
                     GeneralUse.ReadValue("相机", "型号", "config", "海康", rootFolder + "\\Config"));
            camIndex = int.Parse(GeneralUse.ReadValue("相机", "索引", "config", "0", rootFolder + "\\Config"));

            if (CurrCam != null)
            {
                if (CurrCam.IsAlive)
                    CurrCam.CloseCam();
                CurrCam.setImgGetHandle -= GetImageDelegate;//先关闭再注销掉             
                CurrCam.CamConnectHnadle -= CamConnectEvent;
                CurrCam.Dispose();
                CurrCam = null;
            }
            #region  相机实例化
            switch (currCamType)
            {
                case CamType.海康:

                    CurrCam = new HKCam("HKCam");
                    this.currCamType = CamType.海康;
                    break;
                case CamType.凌云:

                    CurrCam = new LusterCam("LusterCam");
                    this.currCamType = CamType.凌云;
                    break;
                case CamType.度申:
                    CurrCam = new DVPCam("DVPCam");
                    this.currCamType = CamType.度申;
                    break;
                case CamType.康耐视:

                    CurrCam = new CognexCam("CognexCam");
                    this.currCamType = CamType.康耐视;
                    break;
                case CamType.大华:

                    CurrCam = new DaHuaCam("DaHuaCam");
                    this.currCamType = CamType.大华;
                    break;
                case CamType.巴斯勒:

                    CurrCam = new BaslerCam("BaslerCam");
                    this.currCamType = CamType.巴斯勒;              
                    break;
                case CamType.奥普特:

                    CurrCam = new OPTCam("OPTCam");
                    this.currCamType = CamType.奥普特;
                    break;
            }

            #endregion
            if (CurrCam == null)
            {
                Appentxt("相机实例化对象为空！");
                return;
            }
            for (int i = 0; i < CurrCam.CamNum; i++)
                Model.CamIndexerList.Add(i);
            if (camIndex < CurrCam.CamNum)
                Model.SelectCamIndexerIndex = camIndex;
            //注册相机图像采集事件
            CurrCam.setImgGetHandle += new ImgGetHandle(GetImageDelegate);
            CurrCam.CamConnectHnadle += new EventHandler(CamConnectEvent);

            //打开软件自动开启相机
            string msg = string.Empty;
            bool initFlag = false;
            initFlag = CurrCam.OpenCam(camIndex, ref msg);
            if (initFlag)
            {
                imageWidth = CurrCam.ImageWidth;
                imageHeight = CurrCam.ImageHeight;
                //曝光值范围获取
                bool getFlag = CurrCam.GetExposureRangeValue(out long minExposure, out long maxExposure);
                if (!getFlag)
                    Appentxt("曝光参数设置范围值获取失败！");
                else
                {
                    //相机曝光范围值设置
                    Model.ExpouseMaxValue = maxExposure;
                    Model.ExpouseMinValue = minExposure;
                    if (currCamExpouse > maxExposure)
                        currCamExpouse = maxExposure;
                    else if(currCamExpouse< minExposure)
                        currCamExpouse = minExposure;

                    //相机曝光设置 
                    Model.ExpouseSliderValue = currCamExpouse;
                    Model.ExpouseNumricValue = currCamExpouse;
                    bool flag = CurrCam.SetExposureTime(currCamExpouse);
                    if (!flag)
                        Appentxt(string.Format("工位：{0}，相机曝光设置失败！", currCamStationName));

                }
             
                //增益值范围获取
                bool getFlag2 = CurrCam.GetGainRangeValue(out long minGain, out long maxGain);
                if (!getFlag)
                    Appentxt("增益参数设置范围值获取失败！");
                {
                    //相机增益范围值设置
                    Model.GainMaxValue = (int)maxGain;
                    Model.GainMinValue = (int)minGain;
                    if (currCamGain > maxGain)
                        currCamGain = (int)maxGain;
                    else if (currCamGain < minGain)
                        currCamGain = (int)minGain;
                    //相机增益设置 
                    Model.GainSliderValue = currCamGain;
                    Model.GainNumricValue = currCamGain;
                    bool flag = CurrCam.SetGain(currCamGain);
                    if (!flag)
                        Appentxt(string.Format("工位：{0}，相机增益设置失败！", currCamStationName));                 
                }

                EnableCam(true);
                workstatus = EunmCamWorkStatus.Freestyle;
            }
            else
            {
                EnableCam(false);
                Appentxt(string.Format("工位：{0}，相机打开失败：{1}", currCamStationName, msg));
            }

        }
        void EnableCam(bool flag)
        {
            Model.IsCamAlive = flag;
            Model.BtnOpenCamEnable = !flag;
            Model.BtnCloseCamEnable = flag;
            Model.BtnOneShotEnable = flag;
            Model.BtnContinueGrabEnable = flag;
            Model.BtnStopGrabEnable = flag;
        }

        void ManualOperateEnable(bool flag)
        {
            Model.BtnOneShotEnable = flag;
            Model.BtnContinueGrabEnable = flag;
            Model.BtnStopGrabEnable = flag;
        }
        /// <summary>
        /// 加载外部通讯工具
        /// </summary>
        void LoadCommDev()
        {
            //if (projectOfPos.toolNamesList.Exists(t => t.Contains("Tcp接收")))
            {

                string path = rootFolder + "\\CommDev";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!File.Exists(path + "\\外部通讯.cdv")) return;
                CommDeviceController.LoadCommDev(path + "\\外部通讯.cdv");
                bool flag = CommDeviceController.InitialConnect();
                if (!flag)
                    Appentxt("外部通讯加载失败");

               
            }
        }

        /// <summary>
        /// 坐标系变换矩阵
        /// </summary>
        /// <param name="calibName"></param>
        void LoadMatrix()
        {
            NineCalibFile = rootFolder + "\\标定矩阵\\九点标定\\" + currCalibName + "\\hv_HomMat2D.tup";
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\九点标定\\" + currCalibName))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\九点标定\\" + currCalibName);
            if (File.Exists(NineCalibFile))
                HOperatorSet.ReadTuple(NineCalibFile, out hv_HomMat2D);
            if(hv_HomMat2D!=null&& hv_HomMat2D.Length>0)
                foreach (var s in projectOfPos.toolsDic)
                {
                    if (s.Value.GetType() == typeof(ImageCorrectTool))
                        continue;
                    s.Value.SetMatrix(hv_HomMat2D);//给每个工具传递当前坐标变换矩阵
                    s.Value.SetCalibFilePath(NineCalibFile);
                }
            Appentxt(string.Format("当前加载标定矩阵：{0}", currCalibName));
        }     
        /// <summary>
        /// 运行检测流程
        /// </summary>
        private StuCoordinateData RunTestFlowOfPosition()
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
              

                Appentxt("流程中无结果显示工具，无法输出正确结果");
                InfoList.Add(string.Format("Pos_x:{0:f3}\nPos_y:{1:f3}\nPos_ang:{2:f3}",
                 0, 0, 0));
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
        /// </summary>
        /// 像素坐标转机械坐标
        /// </summary>
        /// <param name="fc"></param>
        /// <param name="Px">像素坐标X</param>
        /// <param name="Py">像素坐标Y</param>
        /// <param name="Rx">机械坐标X</param>
        /// <param name="Ry">机械坐标Y</param>
        bool Transformation_POINT(HTuple Px, HTuple Py,
                       out HTuple Rx, out HTuple Ry)
        {
            Rx = Ry = 0;
            if (hv_HomMat2D != null && hv_HomMat2D.Length > 0)
                HOperatorSet.AffineTransPoint2d(hv_HomMat2D, Px, Py,
                   out Rx, out Ry);
            else
            {
                Appentxt("未建立标定关系，请确认！");            
                return false;
            }
            return true;
        }

        /// <summary>
        /// 机械坐标转像素坐标
        /// </summary>
        /// <param name="Px">机械坐标X</param>
        /// <param name="Py">机械坐标Y</param>
        /// <param name="Rx">像素坐标X</param>
        /// <param name="Ry">像素坐标Y</param>
        /// <returns></returns>
        bool Transformation_POINT_INV(HTuple Rx, HTuple Ry,
                       out HTuple Px, out HTuple Py)
        {

            Px = Py = 0;
            if (hv_HomMat2D != null && hv_HomMat2D.Length > 0)
            {
                HOperatorSet.HomMat2dInvert(hv_HomMat2D, out HTuple homMat2DInvert);
                HOperatorSet.AffineTransPoint2d(homMat2DInvert, Rx, Ry,
                 out Px, out Py);
            }
            else
            {
                Appentxt("未建立标定关系，请确认！");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算胶水关键信息
        /// </summary>
        /// <param name="glueRow"></param>
        /// <param name="glueColumn"></param>
        /// <param name="basePointRow"></param>
        /// <param name="basePointColumn"></param>
        /// <param name="rect_r1"></param>
        /// <param name="rect_c1"></param>
        /// <param name="rect_r2"></param>
        /// <param name="rect_c2"></param>
        /// <returns></returns>
        StuGlueMainInfo CalGlueMainInfo(double glueRow, double glueColumn,
             double basePointRow, double basePointColumn,
             double rect_r1, double rect_c1, double rect_r2, double rect_c2)
        {
            StuGlueMainInfo infoData = new StuGlueMainInfo();
            Transformation_POINT(glueColumn, glueRow, out HTuple Rx1, out HTuple Ry1);
            Transformation_POINT(basePointColumn, basePointRow, out HTuple Rx2, out HTuple Ry2);
            double offsetX = Math.Abs(Rx1.D - Rx2.D);
            double offsetY = Math.Abs(Ry1.D - Ry2.D);
            //宽度
            Transformation_POINT(rect_r1, rect_c1, out HTuple Rx3, out HTuple Ry3);
            Transformation_POINT(rect_r1, rect_c2, out HTuple Rx4, out HTuple Ry4);
            double width = Math.Sqrt(Math.Pow(Rx3.D - Rx4.D, 2) + Math.Pow(Ry3.D - Ry4.D, 2));

            //高度
           Transformation_POINT(rect_r2, rect_c1, out HTuple Rx5, out HTuple Ry5);
            double height = Math.Sqrt(Math.Pow(Rx3.D - Rx5.D, 2) + Math.Pow(Ry3.D - Ry5.D, 2));

            infoData.Cx = offsetX;
            infoData.Cy = offsetY;
            infoData.Rc_Width = width;
            infoData.Rc_Height = height;
            return infoData;
        }
        /// <summary>
        /// 运行胶水检测流程
        /// </summary>
        /// <returns></returns>
        bool RunTestFlowOfGlue()
        {
            List<string> infoList = new List<string>();
            List<string> GlueInfoList = new List<string>();
            objs.Clear();
            infos.Clear();
            datas.Clear();
            long ctime = 0;
            if (!GlueBaseTool.ObjectValided(this.GrabImg))
            {
                Appentxt("图像为空");
                return false;
            }
            if (!projectOfGlue.dataManage.imageBufDic.ContainsKey("原始图像"))
                projectOfGlue.dataManage.imageBufDic.Add("原始图像", null);
            if (GlueBaseTool.ObjectValided(projectOfGlue.dataManage.imageBufDic["原始图像"]))
                projectOfGlue.dataManage.imageBufDic["原始图像"].Dispose();
            projectOfGlue.dataManage.imageBufDic["原始图像"] = this.GrabImg.Clone();
            int count = projectOfGlue.toolNamesList.Count;
            if (count <= 0)
            {
                Appentxt("流程为空");
                return false;
            }
            infoList.Add(string.Format("Channel:{0}", "unknow"));
            infoList.Add(string.Format("SN:{0}", "default"));
            //检测工具循环运行
            for (int i = 0; i < count; i++)
            {
                string name = projectOfGlue.toolNamesList[i];
                GlueRunResult rlt = projectOfGlue.toolsDic[name].Run();
                ctime += rlt.runTime;

                if (!rlt.runFlag)
                {
                    Appentxt(string.Format("工具：{0}，运行失败，异常信息：{1}", name, rlt.errInfo));
                    //break;
                    UpdateStatusOfGlue(i, false);
                   
                }
                else
                {
                    Appentxt(string.Format("工具：{0}，运行完成，时长：{1}ms", name, rlt.runTime));
                    UpdateStatusOfGlue(i, true);
                }
                   

            }
            infoList.Add(string.Format("CT:{0}ms", ctime));

            if (projectOfGlue.toolNamesList.Exists(t => t.Contains("结果显示")))
            {
                int index = projectOfGlue.toolNamesList.FindIndex(t => t.Contains("结果显示"));
                GlueBaseTool tool = projectOfGlue.toolsDic[projectOfGlue.toolNamesList[index]];
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage((tool.GetParam() as GlueDetectionLib.参数.ResultShowParam).OutputImg);
                ShowTool.DispRegion((tool.GetParam() as GlueDetectionLib.参数.ResultShowParam).ResultRegion, "green");
                ShowTool.AddregionBuffer((tool.GetParam() as GlueDetectionLib.参数.ResultShowParam).ResultRegion, "green");
                objs.Add(new StuWindowHobjectToPaint
                {
                    color = "green",
                    obj = (tool.GetParam() as GlueDetectionLib.参数.ResultShowParam).ResultRegion
                });
            }
            else
            {
                string toolName = projectOfGlue.toolNamesList[count - 1];
                GlueBaseTool tool = projectOfGlue.toolsDic[toolName];
                ShowTool.ClearAllOverLays();
                ShowTool.DispConcatedObj(tool.GetParam().InputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer(tool.GetParam().InputImg, EumCommonColors.green);
                if (projectOfGlue.dataManage.resultBufDic.ContainsKey(toolName))
                {
                    ShowTool.DispRegion(projectOfGlue.dataManage.resultBufDic[toolName], "green");
                    ShowTool.AddregionBuffer(projectOfGlue.dataManage.resultBufDic[toolName], "green");

                    objs.Add(new StuWindowHobjectToPaint
                    {
                        color = "green",
                        obj = projectOfGlue.dataManage.resultBufDic[toolName]
                    });
                }

            }

            #region----显示胶水-----

            int glueMissIndex = 0;
            List<bool> glueMissList = new List<bool>();
            List<bool> glueOffsetList = new List<bool>();
            List<bool> glueGapList = new List<bool>();
            List<bool> glueCaliperList = new List<bool>();
            HOperatorSet.GetImageSize(this.GrabImg, out HTuple width, out HTuple height);
            int ratio = (int)(width.D / 1000);
            int size = ratio * 75;
            if (size < 75)
                size = 75;
            for (int i = 0; i < count; i++)
            {
                string name = projectOfGlue.toolNamesList[i];
                GlueBaseTool tool = projectOfGlue.toolsDic[name];
                //漏胶
                if (tool is GlueMissTool)
                {
                    glueMissIndex++;
                    double CxDown = (tool.GetParam() as GlueMissParam).OffsetXDisMin;
                    double CxUp = (tool.GetParam() as GlueMissParam).OffsetXDisMax;
                    double CyDown = (tool.GetParam() as GlueMissParam).OffsetYDisMin;
                    double CyUp = (tool.GetParam() as GlueMissParam).OffsetYDisMax;
                    StuGlueMainInfo infoData = new StuGlueMainInfo();
                    if ((tool as GlueMissTool).glueInfo.areaList.Count > 0)
                    {
                        CoorditionDat coor = (tool as GlueMissTool).glueInfo.coorditions[0];
                        double area = (tool as GlueMissTool).glueInfo.areaList[0];
                        ShowTool.DispMessage("R" + glueMissIndex,
                            coor.Row, coor.Column, "red", 16);
                        ShowTool.AddTextBuffer("R" + glueMissIndex,
                            coor.Row, coor.Column, "red", 16);

                        infos.Add(new StuWindowInfoToPaint
                        {
                            size = size / 4,
                            color = "red",
                            Info = "R" + glueMissIndex,
                            coorditionDat = new CoorditionDat(coor.Row, coor.Column)
                        });
                        //不在此处显示，找个地方单独显示
                        // infoList.Add(string.Format("R" + glueMissIndex + "_Glue_Area:{0:f3}", area));

                        HOperatorSet.GenCrossContourXld(out HObject rcorss,
                                      coor.Row,
                                       coor.Column, 10, 0);
                        ShowTool.DispRegion(rcorss, "red");
                        ShowTool.AddregionBuffer(rcorss, "red");
                        objs.Add(new StuWindowHobjectToPaint
                        {
                            color = "red",
                            obj = rcorss
                        });

                        infoData = CalGlueMainInfo(coor.Row, coor.Column,
                             positionSharpData.y, positionSharpData.x,
                             (tool as GlueMissTool).glueInfo.rect_r1[0],
                             (tool as GlueMissTool).glueInfo.rect_c1[0],
                             (tool as GlueMissTool).glueInfo.rect_r2[0],
                             (tool as GlueMissTool).glueInfo.rect_c2[0]
                             );

                        infoData.Name = "R" + glueMissIndex;
                        infoData.Area = area;

                        infoData.AreaDown = (tool.GetParam() as GlueMissParam).AreaMin;
                        infoData.AreaUp = (tool.GetParam() as GlueMissParam).AreaMax;
                        infoData.CxDown = CxDown;
                        infoData.CxUp = CxUp;
                        infoData.CyDown = CyDown;
                        infoData.CyUp = CyUp;
                        GlueInfoList.Add(infoData.ToString());
                        datas.Add(new GlueRecheckDat(infoData.Cx, infoData.Cy, infoData.Area));
                    }
                    else
                    { //不在此处显示，找个地方单独显示
                        //infoList.Add(string.Format("R" + glueMissIndex + "_Glue_Area:{0}", 0));
                    }

                    bool testFlag = false;
                    if (projectOfGlue.dataManage.resultFlagDic.ContainsKey(tool.GetToolName()))
                        testFlag = projectOfGlue.dataManage.resultFlagDic[tool.GetToolName()];
                    bool runFlag = (tool.GetParam() as GlueMissParam).GlueMissRunStatus;
                    bool temFlag = infoData.compareX() && infoData.compareY();
                    glueMissList.Add(testFlag && runFlag && temFlag);
                }

                //偏位
                else if (tool is GlueOffsetTool)
                {

                    bool testFlag = false;
                    if (projectOfGlue.dataManage.resultFlagDic.ContainsKey(tool.GetToolName()))
                        testFlag = projectOfGlue.dataManage.resultFlagDic[tool.GetToolName()];
                    bool runFlag = (tool.GetParam() as GlueOffsetParam).GlueOffsetRunStatus;

                    glueOffsetList.Add(testFlag && runFlag);
                }
                //缺胶
                else if (tool is GlueGapTool)
                {
                    bool testFlag = false;
                    if (projectOfGlue.dataManage.resultFlagDic.ContainsKey(tool.GetToolName()))
                        testFlag = projectOfGlue.dataManage.resultFlagDic[tool.GetToolName()];
                    bool runFlag = (tool.GetParam() as GlueGapParam).GlueLossRunStatus;

                    glueGapList.Add(testFlag && runFlag);
                }
                //胶宽
                else if (tool is GlueCaliperWidthTool)
                {
                    bool testFlag = false;
                    if (projectOfGlue.dataManage.resultFlagDic.ContainsKey(tool.GetToolName()))
                        testFlag = projectOfGlue.dataManage.resultFlagDic[tool.GetToolName()];
                    bool runFlag = (tool.GetParam() as GlueCaliperWidthParam).GlueWidthRunStatus;

                    glueCaliperList.Add(testFlag && runFlag);
                }
            }
            #endregion

            #region----显示检测区域
            if (projectOfGlue.toolNamesList.Exists(t => t.Contains("结果显示")))
            {
                int index = projectOfGlue.toolNamesList.FindIndex(t => t.Contains("结果显示"));
                GlueBaseTool tool = projectOfGlue.toolsDic[projectOfGlue.toolNamesList[index]];
                if ((tool as GlueDetectionLib.工具.ResultShowTool).isShowInspectRegion)
                {
                    for (int j = 0; j < count; j++)
                    {
                        string name = projectOfGlue.toolNamesList[j];
                        HObject inspectROI = null;
                        if (name.Contains("模板匹配"))
                            inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueDetectionLib.参数.MatchParam).InspectROI;
                        else if (name.Contains("漏胶"))
                            inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueMissParam).ResultInspectROI;
                        else if (name.Contains("断胶"))
                            inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueGapParam).ResultInspectROI;
                        else if (name.Contains("胶宽"))
                            inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueCaliperWidthParam).ResultInspectRegions;
                        else if (name.Contains("偏位"))
                            inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueOffsetParam).ResultInspectROI;

                        if (GlueBaseTool.ObjectValided(inspectROI))
                        {
                            ShowTool.DispRegion(inspectROI, "blue");
                            ShowTool.AddregionBuffer(inspectROI, "blue");
                            objs.Add(new StuWindowHobjectToPaint
                            {
                                color = "blue",
                                obj = inspectROI
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
                    string name = projectOfGlue.toolNamesList[j];
                    HObject inspectROI = null;
                    if (name.Contains("模板匹配"))
                        inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueDetectionLib.参数. MatchParam).InspectROI;
                    else if (name.Contains("漏胶"))
                        inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueMissParam).ResultInspectROI;
                    else if (name.Contains("断胶"))
                        inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueGapParam).ResultInspectROI;
                    else if (name.Contains("胶宽"))
                        inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueCaliperWidthParam).ResultInspectRegions;
                    else if (name.Contains("偏位"))
                        inspectROI = (projectOfGlue.toolsDic[name].GetParam() as GlueOffsetParam).ResultInspectROI;

                    if (GlueBaseTool.ObjectValided(inspectROI))
                    {
                        ShowTool.DispRegion(inspectROI, "blue");
                        ShowTool.AddregionBuffer(inspectROI, "blue");
                        objs.Add(new StuWindowHobjectToPaint
                        {
                            color = "blue",
                            obj = inspectROI
                        });
                    }
                }
            }

            #endregion

            #region-----基础信息-----
            string content = "";
            foreach (var s in infoList)
                content += s + "\n";
            if (projectOfGlue.toolNamesList.Exists(t => t.Contains("结果显示")))
            {
                int index = projectOfGlue.toolNamesList.FindIndex(t => t.Contains("结果显示"));
                GlueBaseTool tool = projectOfGlue.toolsDic[projectOfGlue.toolNamesList[index]];
                ShowTool.DispMessage(content,
                     (tool as GlueDetectionLib.工具. ResultShowTool).inforCoorY,
                    (tool as GlueDetectionLib.工具.ResultShowTool).inforCoorX, "green", 25);
                ShowTool.AddTextBuffer(content,
                     (tool as GlueDetectionLib.工具.ResultShowTool).inforCoorY,
                    (tool as GlueDetectionLib.工具.ResultShowTool).inforCoorX, "green", 25);

                infos.Add(new StuWindowInfoToPaint
                {
                    size = size / 2,
                    color = "green",
                    Info = content,
                    //coorditionDat = new CoorditionDat(
                    //    (tool as ResultShowTool).inforCoorY,
                    //     (tool as ResultShowTool).inforCoorX)
                    coorditionDat = new CoorditionDat(size * 2, 10)

                });
            }
            else
            {
                ShowTool.DispMessage(content, 300, 10, "green", 25);
                ShowTool.AddTextBuffer(content, 300, 10, "green", 25);
                infos.Add(new StuWindowInfoToPaint
                {
                    size = size / 2,
                    color = "green",
                    Info = content,
                    coorditionDat = new CoorditionDat(300, 10)
                });
            }
            #endregion

            #region----胶水信息----

            for (int i = 0; i < GlueInfoList.Count; i++)
            {
                if (projectOfGlue.toolNamesList.Exists(t => t.Contains("结果显示")))
                {
                    int index = projectOfGlue.toolNamesList.FindIndex(t => t.Contains("结果显示"));
                    GlueBaseTool tool = projectOfGlue.toolsDic[projectOfGlue.toolNamesList[index]];
                    if ((tool as GlueDetectionLib.工具.ResultShowTool).GlueInfoDic != null)
                        if ((tool as GlueDetectionLib.工具.ResultShowTool).GlueInfoDic.Count > 0)
                        {
                            int currX, currY;
                            if ((tool as GlueDetectionLib.工具.ResultShowTool).GlueInfoDic.ContainsKey("R" + (1 + i)))
                            {
                                currX = (int)(tool as GlueDetectionLib.工具.ResultShowTool).GlueInfoDic["R" + (1 + i)].Column;
                                currY = (int)(tool as GlueDetectionLib.工具.ResultShowTool).GlueInfoDic["R" + (1 + i)].Row;

                            }
                            else
                            {
                                currX = 10 + i * 500;
                                currY = 2000 + i * 0;
                            }

                            ShowTool.DispMessage(GlueInfoList[i], currY, currX, "green", 12);
                            ShowTool.AddTextBuffer(GlueInfoList[i], currY, currX, "green", 12);
                            if (i % 2 == 0)
                                infos.Add(new StuWindowInfoToPaint
                                {
                                    size = size / 4,
                                    color = "green",
                                    Info = GlueInfoList[i],
                                    coorditionDat = new CoorditionDat(size * 9, 10 + (size * 3 + 100) * i)
                                });
                            else
                                infos.Add(new StuWindowInfoToPaint
                                {
                                    size = size / 4,
                                    color = "green",
                                    Info = GlueInfoList[i],
                                    coorditionDat = new CoorditionDat(size * 10, 10 + (size * 3 + 100) * i)
                                });

                        }
                        else
                        {
                            int currX = 10 + i * 500;
                            int currY = 2000 + i * 0;
                            ShowTool.DispMessage(GlueInfoList[i], currY, currX, "green", 12);
                            ShowTool.AddTextBuffer(GlueInfoList[i], currY, currX, "green", 12);
                            if (i % 2 == 0)
                                infos.Add(new StuWindowInfoToPaint
                                {
                                    size = size / 4,
                                    color = "green",
                                    Info = GlueInfoList[i],
                                    //coorditionDat = new CoorditionDat(currY, currX)
                                    coorditionDat = new CoorditionDat(size * 9, 10 + (size * 3 + 100) * i)
                                });
                            else
                                infos.Add(new StuWindowInfoToPaint
                                {
                                    size = size / 4,
                                    color = "green",
                                    Info = GlueInfoList[i],
                                    coorditionDat = new CoorditionDat(size * 10, 10 + (size * 3 + 100) * i)
                                });
                        }
                    else
                    {
                        int currX = 10 + i * 500;
                        int currY = 2000 + i * 0;
                        ShowTool.DispMessage(GlueInfoList[i], currY, currX, "green", 12);
                        ShowTool.AddTextBuffer(GlueInfoList[i], currY, currX, "green", 12);
                        if (i % 2 == 0)
                            infos.Add(new StuWindowInfoToPaint
                            {
                                size = size / 4,
                                color = "green",
                                Info = GlueInfoList[i],
                                //coorditionDat = new CoorditionDat(currY, currX)
                                coorditionDat = new CoorditionDat(size * 9, 10 + (size * 3 + 100) * i)
                            });
                        else
                            infos.Add(new StuWindowInfoToPaint
                            {
                                size = size / 4,
                                color = "green",
                                Info = GlueInfoList[i],
                                coorditionDat = new CoorditionDat(size * 10, 10 + (size * 3 + 100) * i)
                            });
                    }
                }
                else
                {
                    int currX = 10 + i * 500;
                    int currY = 2000 + i * 0;
                    ShowTool.DispMessage(GlueInfoList[i], currY, currX, "green", 12);
                    ShowTool.AddTextBuffer(GlueInfoList[i], currY, currX, "green", 12);
                    if (i % 2 == 0)
                        infos.Add(new StuWindowInfoToPaint
                        {
                            size = size / 4,
                            color = "green",
                            Info = GlueInfoList[i],
                            //coorditionDat = new CoorditionDat(currY, currX)
                            coorditionDat = new CoorditionDat(size * 9, 10 + (size * 3 + 100) * i)
                        });
                    else
                        infos.Add(new StuWindowInfoToPaint
                        {
                            size = size / 4,
                            color = "green",
                            Info = GlueInfoList[i],
                            coorditionDat = new CoorditionDat(size * 10, 10 + (size * 3 + 100) * i)
                        });
                }

            }


            #endregion

            #region---胶水最终结果信息--
            bool totalResult = true;//最终结果
          
            string contentGlue = "";
            for (int i = 0; i < glueMissList.Count; i++)
            {
                bool flag2 = true;
                bool flag3 = true;
                bool flag4 = true;
                bool flag1 = glueMissList[i];
                if (glueOffsetList.Count > i)
                    flag2 = glueOffsetList[i];
                if (glueGapList.Count > i)
                    flag3 = glueGapList[i];
                if (glueCaliperList.Count > i)
                    flag4 = glueCaliperList[i];
                bool totalFlag = flag1 && flag2 && flag3 && flag4;
                totalResult = totalResult && totalFlag;
              
                contentGlue += string.Format("R{0}:{1}\n", i + 1, totalFlag ? "OK" : "NG");
            }
            //HOperatorSet.GetImageSize(this.GrabImg, out HTuple width, out HTuple height);
            ShowTool.DispMessage(contentGlue, 10, width - 500, "green", 16);
            ShowTool.AddTextBuffer(contentGlue, 10, width - 500, "green", 16);
            infos.Add(new StuWindowInfoToPaint
            {
                size = size / 4,
                color = "green",
                Info = contentGlue,
                coorditionDat = new CoorditionDat(10, width - 500)
            });
            #endregion

            #region----显示十字光标-----
            //十字参考坐标系
            HOperatorSet.GenCrossContourXld(out HObject corss,
                positionSharpData.y,
                positionSharpData.x, 1000, 0);
            ShowTool.DispRegion(corss, "red");
            ShowTool.AddregionBuffer(corss, "red");
            objs.Add(new StuWindowHobjectToPaint
            {
                color = "red",
                obj = corss
            });
            #endregion

            #region-------最终结果显示----
            //最终结果显示
            if (totalResult)
            {
                ShowTool.DispMessage("OK", 10, 10, "green", 50);
                ShowTool.AddTextBuffer("OK", 10, 10, "green", 50);
                infos.Add(new StuWindowInfoToPaint
                {
                    size = size,
                    color = "green",
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
                    size = size,
                    color = "red",
                    Info = "NG",
                    coorditionDat = new CoorditionDat(10, 10)

                });
            }
            #endregion

            return totalResult;


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

        void UpdateStatusOfGlue(int index, bool flag)
        {
            if (index < 0 ||
               index >= Model.ToolsOfGlueList.Count)
                return;

            Model.ToolsOfGlueList[index].ToolStatus = flag ? "OK" : "NG";

        }
        /// <summary>
        /// 复位定位检测工具编号
        /// </summary>
        private void ResetNumOfPos()
        {
            PositionToolsLib.工具.AngleConvertTool.inum = 0;
            PositionToolsLib.工具.BinaryzationTool.inum = 0;
            PositionToolsLib.工具.BlobTool.inum = 0;
            PositionToolsLib.工具.CalParallelLineTool.inum = 0;
            PositionToolsLib.工具.ClosingTool.inum = 0;
            PositionToolsLib.工具.ColorConvertTool.inum = 0;
            PositionToolsLib.工具.CoordConvertTool.inum = 0;
            PositionToolsLib.工具.DilationTool.inum = 0;
            PositionToolsLib.工具.ErosionTool.inum = 0;
            PositionToolsLib.工具.FindCircleTool.inum = 0;
            PositionToolsLib.工具.FindLineTool.inum = 0;
            PositionToolsLib.工具.FitLineTool.inum = 0;
            PositionToolsLib.工具.ImageCorrectTool.inum = 0;
            PositionToolsLib.工具.LineCentreTool.inum = 0;
            PositionToolsLib.工具.LineIntersectionTool.inum = 0;
            PositionToolsLib.工具.LineOffsetTool.inum = 0;
            PositionToolsLib.工具.MatchTool.inum = 0;
            PositionToolsLib.工具.OpeningTool.inum = 0;
            PositionToolsLib.工具.ResultShowTool.inum = 0;
            PositionToolsLib.工具.TcpRecvTool.inum = 0;
            PositionToolsLib.工具.TcpSendTool.inum = 0;

        }
        /// <summary>
        /// 复位AOI检测工具编号
        /// </summary>
        private void ResetNumOfGlue()
        {
            GlueDetectionLib.工具.BinaryzationTool.inum = 0;        
            GlueDetectionLib.工具.ClosingTool.inum = 0;
            GlueDetectionLib.工具.ColorConvertTool.inum = 0;
            GlueDetectionLib.工具.DilationTool.inum = 0;
            GlueDetectionLib.工具.ErosionTool.inum = 0;
            GlueDetectionLib.工具.GlueCaliperWidthTool.inum = 0;
            GlueDetectionLib.工具.GlueGapTool.inum = 0;
            GlueDetectionLib.工具.GlueMissTool.inum = 0;
            GlueDetectionLib.工具.GlueOffsetTool.inum = 0;
            GlueDetectionLib.工具.MatchTool.inum = 0;
            GlueDetectionLib.工具.OpeningTool.inum = 0;
            GlueDetectionLib.工具.ResultShowTool.inum = 0;
            GlueDetectionLib.工具.TcpRecvTool.inum = 0;
            GlueDetectionLib.工具.TcpSendTool.inum = 0;
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
                ListViewToolsData(toolindexofPosition,
                           s, "--", projectOfPos.toolsDic[s].remark));
            }
        }
        /// <summary>
        /// 显示胶水检测流程
        /// </summary>
        private void ShowTestFlowOfGlue()
        {
            projectOfGlue.dataManage.ResetBuf();//重载后清除数据缓存
            Model.ToolsOfGlueList.Clear();
            toolindexofGlueAOI = 0;
            if (this.projectOfGlue == null) return;
            if (this.projectOfGlue.toolsDic.Count <= 0) return;

            foreach (var s in projectOfGlue.toolNamesList)
            {
                toolindexofGlueAOI++;
                Model.ToolsOfGlueList.Add(new
               ListViewToolsData(toolindexofGlueAOI,
                          s, "--", projectOfGlue.toolsDic[s].remark));
            }

          
        }
        /// <summary>
        /// 富文本信息清除
        /// </summary>
        private void ClearTextClick()
        {
            //Model.ClearRichText = false;
            //Model.ClearRichText = true;
            ClearTxtAction?.Invoke();
        }
        /// <summary>
        /// 添加测试文本及日志
        /// </summary>
        /// <param name="info"></param>
        private void Appentxt(string info)
        {
            string dConvertString = string.Format("{0}  {1}\r",
                              DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), info);
            //Model.RichIfo = dConvertString;
            AppenTxtAction?.Invoke(dConvertString);
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
        /// <summary>
        /// 根据九点标定关系计算单个像素物理尺寸
        /// </summary>
        /// <returns></returns>
        double OnGetPixelRatio()
        {
            this.Transformation_POINT(1000, 1000, out HTuple rx, out HTuple ry);
            this.Transformation_POINT(1000, 1001, out HTuple rx2, out HTuple ry2);
            return Math.Sqrt(Math.Pow(rx2 - rx, 2) + Math.Pow(ry2 - ry, 2));
        }

        static void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
      HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            // Local iconic variables 

            // Local control variables 

            HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
            HTuple hv_Row1Part = null, hv_Column1Part = null, hv_Row2Part = null;
            HTuple hv_Column2Part = null, hv_RowWin = null, hv_ColumnWin = null;
            HTuple hv_WidthWin = null, hv_HeightWin = null, hv_MaxAscent = null;
            HTuple hv_MaxDescent = null, hv_MaxWidth = null, hv_MaxHeight = null;
            HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
            HTuple hv_FactorColumn = new HTuple(), hv_UseShadow = null;
            HTuple hv_ShadowColor = null, hv_Exception = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
            HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
            HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_CurrentColor = new HTuple();
            HTuple hv_Box_COPY_INP_TMP = hv_Box.Clone();
            HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
            HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
            HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
            HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Column: The column coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for each new textline.
            //Box: If Box[0] is set to 'true', the text is written within an orange box.
            //     If set to' false', no box is displayed.
            //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
            //       the text is written in a box of that color.
            //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
            //       'true' -> display a shadow in a default color
            //       'false' -> display no shadow (same as if no second value is given)
            //       otherwise -> use given string as color string for the shadow color
            //
            //Prepare window
            HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
                out hv_Column2Part);
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
                out hv_WidthWin, out hv_HeightWin);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
            //
            //default settings
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
            {
                hv_Color_COPY_INP_TMP = "";
            }
            //
            hv_String_COPY_INP_TMP = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit("\n");
            //
            //Estimate extentions of text depending on font size.
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                out hv_MaxWidth, out hv_MaxHeight);
            if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
            {
                hv_R1 = hv_Row_COPY_INP_TMP.Clone();
                hv_C1 = hv_Column_COPY_INP_TMP.Clone();
            }
            else
            {
                //Transform image to window coordinates
                hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
            }
            //
            //Display text box depending on text size
            hv_UseShadow = 1;
            hv_ShadowColor = "gray";
            if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleEqual("true"))) != 0)
            {
                if (hv_Box_COPY_INP_TMP == null)
                    hv_Box_COPY_INP_TMP = new HTuple();
                hv_Box_COPY_INP_TMP[0] = "#fce9d4";
                hv_ShadowColor = "#f28d26";
            }
            if ((int)(new HTuple((new HTuple(hv_Box_COPY_INP_TMP.TupleLength())).TupleGreater(
                1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual("true"))) != 0)
                {
                    //Use default ShadowColor set above
                }
                else if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual(
                    "false"))) != 0)
                {
                    hv_UseShadow = 0;
                }
                else
                {
                    hv_ShadowColor = hv_Box_COPY_INP_TMP[1];
                    //Valid color?
                    try
                    {
                        HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                            1));
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        hv_Exception = "Wrong value of control parameter Box[1] (must be a 'true', 'false', or a valid color string)";
                        throw new HalconException(hv_Exception);
                    }
                }
            }
            if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleNotEqual("false"))) != 0)
            {
                //Valid color?
                try
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_Exception = "Wrong value of control parameter Box[0] (must be a 'true', 'false', or a valid color string)";
                    throw new HalconException(hv_Exception);
                }
                //Calculate box extents
                hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
                hv_Width = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                    hv_Width = hv_Width.TupleConcat(hv_W);
                }
                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ));
                hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
                hv_R2 = hv_R1 + hv_FrameHeight;
                hv_C2 = hv_C1 + hv_FrameWidth;
                //Display rectangles
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                //Set shadow color
                HOperatorSet.SetColor(hv_WindowHandle, hv_ShadowColor);
                if ((int)(hv_UseShadow) != 0)
                {
                    HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 1, hv_C1 + 1, hv_R2 + 1, hv_C2 + 1);
                }
                //Set box color
                HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            //Write text.
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                    )));
                if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                    "auto")))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                }
                hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
                HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
                HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                    hv_Index));
            }
            //Reset changed window settings
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);

            return;
        }

        private void disp_message(HTuple hv_WindowHandle, HTuple row, HTuple column,
            HTuple color, string info, int size)
        {
            if (hv_WindowHandle.D > 100000)
                HOperatorSet.SetFont(hv_WindowHandle, "Arial-" + size);
            else
                HOperatorSet.SetFont(hv_WindowHandle, "-Arial-" + size + "-*-*-*-*-1-");
            disp_message(hv_WindowHandle, info,
                        "image", row, column, color, "false");
        }
        private HObject GetWindowImage(HObject img, List<StuWindowInfoToPaint> infos,
        List<StuWindowHobjectToPaint> objs)
        {
            if (ObjectValided(img))
            {

                HOperatorSet.GetImageSize(img, out HTuple width, out HTuple height);
                HOperatorSet.OpenWindow(0, 0, width, height, 0,
                           "invisible", "", out HTuple hv_WindowHandle);
                HOperatorSet.SetPart(hv_WindowHandle, 0, 0, height, width);
                HOperatorSet.DispObj(img, hv_WindowHandle);

                #region-----显示信息-----
                //文本
                foreach (var s in infos)
                    disp_message(hv_WindowHandle, s.coorditionDat.Row, s.coorditionDat.Column,
                       s.color, s.Info, s.size);

                //区域+轮廓
                foreach (var t in objs)
                {
                    if (!ObjectValided(t.obj))
                        continue;
                    HOperatorSet.SetLineWidth(hv_WindowHandle, 3);
                    HOperatorSet.SetDraw(hv_WindowHandle, "margin");
                    HOperatorSet.SetColor(hv_WindowHandle, t.color);
                    HOperatorSet.DispObj(t.obj, hv_WindowHandle);
                }

                #endregion

                HOperatorSet.DumpWindowImage(out HObject DumpImg, hv_WindowHandle);

                HOperatorSet.CloseWindow(hv_WindowHandle);
                return DumpImg;
            }
            else
                return null;
        }

        /// <summary>
        /// 生成刻度线,间距2mm
        /// </summary>
        /// <param name="space"></param>
        /// <returns></returns>
        HObject Gen_TickMarks(double space = 2.0)
        {
            int _width = ShowTool.ImageWidth;
            int _height = ShowTool.ImageHeight;
            HTuple _Rx, _Ry;
            //图像中心为零点
            this.Transformation_POINT(_width / 2, _height / 2,
                            out _Rx, out _Ry);
            //物理转像素间距
            //像素
            HTuple _Px, _Py, _distance;
            //物理
            space = Model.NumOfScale;
            HTuple _Rx2 = _Rx + space;
            HTuple _Ry2 = _Ry;
            this.Transformation_POINT_INV(_Rx2, _Ry2, out _Px, out _Py);
            HOperatorSet.DistancePp(_height / 2, _width / 2,
                   _Py, _Px, out _distance);

            //x正
            HObject x_pos_ticks = null;
            HOperatorSet.GenEmptyObj(out x_pos_ticks);
            //x_pos_ticks.Dispose();
            for (int i = 0; _width / 2 + _distance * i < _width; i++)
            {
                //消除累计误差
                //物理
                HTuple tem_Rx2 = _Rx + space * i;
                HTuple tem_Ry2 = _Ry;
                this.Transformation_POINT_INV(tem_Rx2, tem_Ry2, out _Px, out _Py);
                HOperatorSet.DistancePp(_height / 2, _width / 2,
                       _Py, _Px, out HTuple tem_distance);

                //生成刻度尺
                HTuple hv_col = _width / 2 + tem_distance;
                HTuple hv_row1 = _height / 2 - 20;
                HTuple hv_row2 = _height / 2 + 20;
                HOperatorSet.GenContourPolygonXld(out HObject ho_Contour, hv_row1.TupleConcat(hv_row2),
                                hv_col.TupleConcat(hv_col));
                HOperatorSet.ConcatObj(x_pos_ticks, ho_Contour, out x_pos_ticks);
            }

            //x负
            HObject x_neg_ticks = null;
            HOperatorSet.GenEmptyObj(out x_neg_ticks);
            //x_neg_ticks.Dispose();
            for (int i = 0; _width / 2 - _distance * i > 0; i++)
            {
                //消除累计误差
                //物理
                HTuple tem_Rx2 = _Rx - space * i;
                HTuple tem_Ry2 = _Ry;
                this.Transformation_POINT_INV(tem_Rx2, tem_Ry2, out _Px, out _Py);
                HOperatorSet.DistancePp(_height / 2, _width / 2,
                       _Py, _Px, out HTuple tem_distance);

                //生成刻度尺
                HTuple hv_col = _width / 2 - tem_distance;
                HTuple hv_row1 = _height / 2 - 20;
                HTuple hv_row2 = _height / 2 + 20;
                HOperatorSet.GenContourPolygonXld(out HObject ho_Contour, hv_row1.TupleConcat(hv_row2),
                                hv_col.TupleConcat(hv_col));
                HOperatorSet.ConcatObj(x_neg_ticks, ho_Contour, out x_neg_ticks);
            }
            //y正
            HObject y_pos_ticks = null;
            HOperatorSet.GenEmptyObj(out y_pos_ticks);
            ////y_pos_ticks.Dispose();
            for (int i = 0; _height / 2 + _distance * i < _height; i++)
            {
                //消除累计误差
                //物理
                HTuple tem_Rx2 = _Rx;
                HTuple tem_Ry2 = _Ry + space * i;
                this.Transformation_POINT_INV(tem_Rx2, tem_Ry2, out _Px, out _Py);
                HOperatorSet.DistancePp(_height / 2, _width / 2,
                       _Py, _Px, out HTuple tem_distance);

                //生成刻度尺
                HTuple hv_row = _height / 2 + tem_distance;
                HTuple hv_col1 = _width / 2 - 20;
                HTuple hv_col2 = _width / 2 + 20;
                HOperatorSet.GenContourPolygonXld(out HObject ho_Contour, hv_row.TupleConcat(hv_row),
                                hv_col1.TupleConcat(hv_col2));
                HOperatorSet.ConcatObj(y_pos_ticks, ho_Contour, out y_pos_ticks);
            }
            //y负
            HObject y_neg_ticks = null;
            HOperatorSet.GenEmptyObj(out y_neg_ticks);
            //y_neg_ticks.Dispose();
            for (int i = 0; _height / 2 - _distance * i > 0; i++)
            {

                //消除累计误差
                //物理
                HTuple tem_Rx2 = _Rx;
                HTuple tem_Ry2 = _Ry - space * i;
                this.Transformation_POINT_INV(tem_Rx2, tem_Ry2, out _Px, out _Py);
                HOperatorSet.DistancePp(_height / 2, _width / 2,
                       _Py, _Px, out HTuple tem_distance);
                //生成刻度尺
                HTuple hv_row = _height / 2 - tem_distance;
                HTuple hv_col1 = _width / 2 - 20;
                HTuple hv_col2 = _width / 2 + 20;
                HOperatorSet.GenContourPolygonXld(out HObject ho_Contour, hv_row.TupleConcat(hv_row),
                                hv_col1.TupleConcat(hv_col2));
                HOperatorSet.ConcatObj(y_neg_ticks, ho_Contour, out y_neg_ticks);
            }

            HOperatorSet.ConcatObj(x_pos_ticks, x_neg_ticks, out HObject objectsConcat);
            HOperatorSet.ConcatObj(objectsConcat, y_pos_ticks, out HObject objectsConcat2);
            HOperatorSet.ConcatObj(objectsConcat2, y_neg_ticks, out HObject objectsConcat3);
            return objectsConcat3;
        }
        void AddAssistToolToCross()
        {
            if (!ShowTool.IsShowCenterCross)
                return;

            int _width = ShowTool.ImageWidth;
            int _height = ShowTool.ImageHeight;
            HObject xldOBJ = null;
            HOperatorSet.GenEmptyObj(out xldOBJ);
             //辅助工具
            switch (Model.AssistTool)
            {
                case EumAssistTool.None:
                  
                    break;        
                case EumAssistTool.Circle:
                    double ActualSizeOfRadium = (double)Model.AssistCircleRadius;

                    HTuple _Rx, _Ry;
                    bool Circleflag = this.Transformation_POINT(_width / 2, _height / 2,
                                    out _Rx, out _Ry);

                    //像素
                    HTuple _Px, _Py, _distanceX, _distanceY;

                    //物理
                    HTuple _Rx2 = _Rx;
                    HTuple _Ry2 = _Ry + ActualSizeOfRadium;
                    this.Transformation_POINT_INV(_Rx2, _Ry2, out _Px, out _Py);
                    HOperatorSet.DistancePp(_height / 2, _width / 2,
                           _Py, _Px, out _distanceY);
                    //物理
                    HTuple _Rx22 = _Rx + ActualSizeOfRadium;
                    HTuple _Ry22 = _Ry;
                    this.Transformation_POINT_INV(_Rx22, _Ry22, out _Px, out _Py);
                    HOperatorSet.DistancePp(_height / 2, _width / 2,
                           _Py, _Px, out _distanceX);
               
                  
                    HOperatorSet.GenEllipseContourXld(out HObject circleXldOBJ, _height / 2, _width / 2, 0, _distanceX, _distanceY,
                         0, 6.28318, "positive", 1);
                    // HOperatorSet.GenCircleContourXld(out HObject  circleXldOBJ, _height / 2, _width / 2, _distance, 0, 6.28318, "positive", 1);
                    xldOBJ = circleXldOBJ.Clone();
                    circleXldOBJ.Dispose();
                    HOperatorSet.GenEmptyObj(out autoFocusRegion);
                    autoFocusRegion.Dispose();                
                    HOperatorSet.GenEllipse(out autoFocusRegion, _height / 2, _width / 2, 0, _distanceX, _distanceY);
                    break;                   
                case EumAssistTool.Rectangle:
                    double ActualSizeOfWidth = Model.AssistRectWidth;

                    HTuple Rx, Ry;
                    bool Rectangleflag = this.Transformation_POINT(_width / 2, _height / 2,
                                   out Rx, out Ry);
                    //像素
                    HTuple Px, Py, distanceX, distanceY;

                    //物理
                    HTuple Rx2 = Rx;
                    HTuple Ry2 = Ry + ActualSizeOfWidth / 2;
                    this.Transformation_POINT_INV(Rx2, Ry2, out Px, out Py);
                    HOperatorSet.DistancePp(_height / 2, _width / 2,
                           Py, Px, out distanceY);
                    //物理
                    HTuple Rx22 = Rx + ActualSizeOfWidth / 2;
                    HTuple Ry22 = Ry;
                    this.Transformation_POINT_INV(Rx22, Ry22, out Px, out Py);
                    HOperatorSet.DistancePp(_height / 2, _width / 2,
                           Py, Px, out distanceX);


                    //HTuple R1_x = Rx - ActualSizeOfWidth / 2, R1_y = Ry - ActualSizeOfWidth / 2;
                    //HTuple R2_x = Rx + ActualSizeOfWidth / 2, R2_y = Ry - ActualSizeOfWidth / 2;
                    //HTuple R3_x = Rx + ActualSizeOfWidth / 2, R3_y = Ry + ActualSizeOfWidth / 2;
                    //HTuple R4_x = Rx - ActualSizeOfWidth / 2, R4_y = Ry + ActualSizeOfWidth / 2;

                    HTuple P1_x = _width / 2 - distanceX, P1_y = _height / 2 - distanceY;
                    HTuple P2_x = _width / 2 + distanceX, P2_y = _height / 2 - distanceY;
                    HTuple P3_x = _width / 2 + distanceX, P3_y = _height / 2 + distanceY;
                    HTuple P4_x = _width / 2 - distanceX, P4_y = _height / 2 + distanceY;
                    //this.Transformation_POINT_INV(R1_x, R1_y, out P1_x, out P1_y);
                    //this.Transformation_POINT_INV(R2_x, R2_y, out P2_x, out P2_y);
                    //this.Transformation_POINT_INV(R3_x, R3_y, out P3_x, out P3_y);
                    //this.Transformation_POINT_INV(R4_x, R4_y, out P4_x, out P4_y);
                             
                    HOperatorSet.GenContourPolygonXld(out HObject rectangleXldOBJ, P1_y.TupleConcat(P2_y, P3_y, P4_y, P1_y),
                        P1_x.TupleConcat(P2_x, P3_x, P4_x, P1_x));
                    xldOBJ = rectangleXldOBJ.Clone();
                    rectangleXldOBJ.Dispose();
                    //生成自动对焦区域
                    HOperatorSet.GenEmptyObj(out autoFocusRegion);
                    autoFocusRegion.Dispose();
                    HOperatorSet.GenRegionPolygon(out autoFocusRegion, P1_y.TupleConcat(P2_y, P3_y, P4_y, P1_y),
                        P1_x.TupleConcat(P2_x, P3_x, P4_x, P1_x));
                    break;
                  
            }
            //排布刻度线
            if (Model.ShowScaleRule)
            {
                HObject tickMarks = Gen_TickMarks();
                xldOBJ = xldOBJ.ConcatObj(tickMarks);
            }         

            ShowTool.AddAssistToolToCross(xldOBJ);
        }

        /// <summary>
        /// CurrCam.OneShot()==替换===>OneGrab();
        /// </summary>
        bool OneGrab(EunmCamWorkStatus status)
        {
            // lock (camlock)
            {
                if (CurrCam == null)
                {
                    Appentxt("相机对象为空");
                    return false;
                }
                if (!CurrCam.IsAlive)
                {
                    Appentxt("相机未在线");
                    return false;
                }


                Thread.Sleep(10);
                Task.Run(() =>
                {
                    if (CurrCam.IsGrabing)
                    {
                        CurrCam.StopGrab();  //如果已在采集中则先停止采集
                        Thread.Sleep(50);
                    }

                }).ContinueWith(t =>
                {
                    workstatus = status;
                    CurrCam.OneShot();
                });
                return true;
            }
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
            string strData = sender.ToString();
            Appentxt(string.Format("控制端接收信息：{0}", strData));
            if (!ContinueRunFlag)
            {
                Appentxt("请开启连续运行");
                return;
            }
            Monitor.Enter(locker);
            FlowHandle(strData);
            Monitor.Exit(locker);
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
        /******************************************************************/
        /// <summary>
        /// 获取当前工位所有可以使用配方名称集合
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllRecipeNames()
        {
            List<string> names = new List<string>();
            foreach (var s in Model.RecipeDgList)
                names.Add(s.Name);
            return names;
        }
        /// <summary>
        /// 获取当前正在使用的配方名称
        /// </summary>
        /// <returns></returns>
        public string  GetCurrRecipeName()
        {
            return this.currRecipeName;
        }
        /// <summary>
        /// 添加新配方
        /// path：需要添加的配方文件完整路径
        /// recipeName:自定义新配方名称;
        /// </summary>
        /// <param name="path">需要添加的配方文件完整路径</param>
        /// <param name="recipeName">自定义新配方名称</param>     
        /// <returns></returns>
        public bool NewRecipe(string recipeName)
        {
            if(Model.RecipeDgList.ToList().Exists(t=>t.Name== recipeName))
            {
                Appentxt(string.Format("已存在配方：{0}不可重复添加", recipeName));
                return false;
            }
            Model.RecipeDgList.Add(new RecipeDg(recipeName, false));
            string path = rootFolder + "\\Config\\配方.rep";
            foreach (var s in Model.RecipeDgList)
            {
                if (!Directory.Exists(rootFolder + "\\配方\\" + s.Name))
                    Directory.CreateDirectory(rootFolder + "\\配方\\" + s.Name);
            }

            ObservableCollection<RecipeDgBuf> RecipeDgListBuf = new ObservableCollection<RecipeDgBuf>();
            foreach (var s in Model.RecipeDgList)
                RecipeDgListBuf.Add(new RecipeDgBuf(s.Name, s.IsUse));
            GeneralUse.WriteSerializationFile<ObservableCollection<RecipeDgBuf>>
                   (path, RecipeDgListBuf);
            return true;
        }
        /// <summary>
        /// 将配方文件recipeName导出到特定文件夹路径path
        /// path:文件夹路径
        /// recipeName:配方文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="recipeName">配方文件</param>
        /// <returns></returns>
        public bool ExportRecipe(string path, string recipeName)
        {
            if (!Directory.Exists(path))
            {
                Appentxt("存放导出配方的文件夹路径不存在");
                return false;
            }
            if (!Directory.Exists(rootFolder + "\\配方\\" + recipeName))
            {
                Appentxt("配方文件不存在");
                return false;
            }
            return CopyFolder(rootFolder + "\\配方\\" + recipeName,
                path + "\\" + recipeName);
        }
        /// <summary>
        /// 切换配方
        /// </summary>
        /// <param name="recipeName"></param>
        /// <returns></returns>
        public bool SwitchRecipe(string recipeName)
        {
            Appentxt(string.Format("外部指令进行配方切换，配方名称：{0}", recipeName));
            if (currRecipeName == recipeName)
            {
                Appentxt("配方同名无需切换");
                return true;
            }
            if (!Model.RecipeDgList.ToList().Exists(t => t.Name == recipeName))
            {
                Appentxt("不存在配方：" + recipeName);
                return false;
            }
            int index = Model.RecipeDgList.ToList().FindIndex
                                      (t => t.Name == currRecipeName);
            Model.RecipeDgList[index].IsUse = false;
            index = Model.RecipeDgList.ToList().FindIndex
                                      (t => t.Name == recipeName);
            Model.RecipeDgList[index].IsUse = true;

            currRecipeName = recipeName;
            saveToUsePath = rootFolder + "\\配方\\" + currRecipeName;
            LoadRecipe();
            return true;
        }
        /// <summary>
        ///  获取当前工位所有可以使用标定关系名称集合
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllCalibNames()
        {
            List<string> names = new List<string>();
            string temPath = rootFolder + "\\标定矩阵\\九点标定";
            DirectoryInfo di = new DirectoryInfo(temPath);
            DirectoryInfo[] dis = di.GetDirectories();
            foreach (var s in dis)
                names.Add(s.Name);
            return names;
        }
        /// <summary>
        /// 获取当前正在使用的标定名称
        /// </summary>
        /// <returns></returns>
        public string GetCurrCalibName()
        {
            return this.currCalibName;
        }
        /// <summary>
        /// 切换标定矩阵
        /// </summary>
        /// <param name="calibName"></param>
        public bool SwithCalib(string calibName)
        {
            Appentxt(string.Format("外部指令进行标定关系切换，标定名称：{0}", calibName));
            if (currCalibName == calibName)
            {
                Appentxt("标定同名无需切换");
                return true;
            }
            if (!File.Exists(rootFolder + "\\标定矩阵\\九点标定\\" +
                calibName + "\\hv_HomMat2D.tup"))
            {
                Appentxt("标定文件不存在，切换失败");
                return false;
            }

            currCalibName = calibName;
            LoadMatrix();
            return true;
        }

        /// <summary>
        /// 获取当前正在使用的模板名称
        /// </summary>
        /// <returns></returns>
        public string GetCurrModelName()
        {
            return this.currModelType.ToString();
        }
        /// <summary>
        ///  模板切换
        /// </summary>
        /// <param name="eumModelType">模板切换类型参数</param>
        /// <returns>模板切换是否成功标志</returns>
        public bool SwitchModelType(EumModelType eumModelType)
        {
          
            Application.Current.Dispatcher.Invoke(() =>
            {            
                // 在UI线程上执行更新操作
                // 更新绑定数据的代码

                Appentxt(string.Format("指令开启模板切换,模板名称:{0}",
                                  Enum.GetName(typeof(EumModelType), eumModelType)));
                if (currModelType == eumModelType)//如果无切换则不重载
                {
                    Appentxt(string.Format("当前切换模板类型:{0}与当前正使用的同名！",
                           Enum.GetName(typeof(EumModelType), eumModelType)));
                    return true;
                }
                Model.ModelType = eumModelType;
                if (eumModelType == EumModelType.CaliBoardModel)
                    f_NinePointsCalib = new FormNinePointsCalib(rootFolder, currCalibName);
                currModelType = eumModelType;
                string secondName = Enum.GetName(typeof(EumModelType), currModelType);
                bool loadFlag = LoadPositionFlow(secondName);
                Model.ModelTypeSelectIndex = (int)eumModelType;
                return loadFlag;
                //if (!PosBaseTool.ObjectValided(this.GrabImg))
                //    return;
                //ShowTool.ClearAllOverLays();
                //ShowTool.DispImage(this.GrabImg);

                //return true;
            });
            return true;
        }
        /// <summary>
        /// 标定前准备
        /// </summary>
        /// <param name="calibName">传入标定文件名称</param>     
        public void StartCalib(string calibName)
        {
            Appentxt(string.Format("外部指令开启标定开始指令,标定名称:{0}", calibName));
            if (!Directory.Exists(rootFolder + "\\标定矩阵"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵");
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\九点标定"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\九点标定");
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\九点标定\\" + calibName))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\九点标定\\" + calibName);
          
            currCalibName = calibName;
        }
        /// <summary>
        /// 设置图像采集自由模式
        /// </summary>
        public void SetCameraFreeStyle()
        {
            workstatus = EunmCamWorkStatus.Freestyle;
        }
        /// <summary>
        /// 图片保存
        /// imageName:图片名称，不附格式后缀则默认jpg格式；
        /// 也可给图片名称附上格式后缀(xxx.bmp|xxx.png|xxx.jpg)则会以相应的格式进行保存;
        /// 图片名称也作为可选参数，当不传递参数则为自动以时间来命名，格式为jpg.
        /// </summary>
        /// <param name="DirPath">文件夹路径</param>
        /// <param name="imageName">图片名称</param>
        public void SaveImg(string DirPath, string imageName = "")
        {
            if (!ObjectValided(GrabImg))
            {
                Appentxt("图片为空保存失败！");
                return;
            }

            if (!Directory.Exists(DirPath))
                Directory.CreateDirectory(DirPath);
            if (string.IsNullOrEmpty(imageName))
                imageName = DateTime.Now.ToString("yyyy-MM-dd HH_mm_ff");
            string[] buf = imageName.Split('.');
            if (buf.Length > 1)
            {
                Task.Run(() =>
                {
                    if (buf[1].ToLower() == "png")
                        HOperatorSet.WriteImage(GrabImg, "png", 0,
                                      DirPath + "\\" + buf[0] + ".png");
                    else if (buf[1].ToLower() == "jpg")
                        HOperatorSet.WriteImage(GrabImg, "jpeg", 0,
                                        DirPath + "\\" + buf[0] + ".jpg");
                    else if (buf[1].ToLower() == "bmp")
                        HOperatorSet.WriteImage(GrabImg, "bmp", 0,
                                        DirPath + "\\" + buf[0] + ".bmp");
                    else
                        HOperatorSet.WriteImage(GrabImg, "jpeg", 0,
                                       DirPath + "\\" + buf[0] + ".jpg");
                });
            }
            else
            {
                Task.Run(() =>
                {
                    HOperatorSet.WriteImage(GrabImg, "jpeg", 0,
                                    DirPath + "\\" + imageName + ".jpg");
                });
            }
        }            
        /// <summary>
        /// 保存窗体图片
        /// </summary>
        /// <param name="img"></param>
        /// <param name="DirPath"></param>
        /// <param name="imageName"></param>
        public void SaveWindowImg(string DirPath, string imageName)
        {
            if (!ObjectValided(this.GrabImg))
            {
                Appentxt("图片为空保存失败！");
                return;
            }

            HObject img = GetWindowImage(this.GrabImg, infos, objs);

            if (!ObjectValided(img))
            {
                Appentxt("图片为空保存失败！");
                return;
            }
            Task.Run(() => {
                if (!Directory.Exists(DirPath))
                    Directory.CreateDirectory(DirPath);
                if (string.IsNullOrEmpty(imageName))
                    imageName = DateTime.Now.ToString("yyyy-MM-dd HH_mm_ff");
                HOperatorSet.WriteImage(img, "bmp", 0,
                                        DirPath + "\\" + imageName + ".bmp");
            });
        }
        /*****************************相机操作***************************/
        /// <summary>
        /// 外部指令打开相机
        /// </summary>
        /// <returns></returns>
        async public Task<bool> OpenCam()
        {
            Appentxt("外部调用OpenCam指令开启相机");
            bool openFlag = false;
            if (CurrCam == null)
            {
                Appentxt(string.Format("工位：{0}，相机初始化失败，无法打开！", currCamStationName));           
                return false;
            }
            await Task.Run<bool>(() =>
            {
                Model.BtnOpenCamEnable = false;
                string msg = string.Empty;
                bool initFlag = false;
                initFlag = CurrCam.OpenCam(camIndex, ref msg);
                if (initFlag)
                {
                    imageWidth = CurrCam.ImageWidth;
                    imageHeight = CurrCam.ImageHeight;
                    //曝光值范围获取
                    bool getFlag = CurrCam.GetExposureRangeValue(out long minExposure, out long maxExposure);
                    if (!getFlag)
                        Appentxt("曝光参数设置范围值获取失败！");
                    else
                    {
                        //相机曝光范围值设置
                        Model.ExpouseMaxValue = maxExposure;
                        Model.ExpouseMinValue = minExposure;
                        if (currCamExpouse > maxExposure)
                            currCamExpouse = maxExposure;
                        else if (currCamExpouse < minExposure)
                            currCamExpouse = minExposure;

                        //相机曝光设置 
                        Model.ExpouseSliderValue = currCamExpouse;
                        Model.ExpouseNumricValue = currCamExpouse;
                        bool flag = CurrCam.SetExposureTime(currCamExpouse);
                        if (!flag)
                            Appentxt(string.Format("工位：{0}，相机曝光设置失败！", currCamStationName));

                    }

                    //增益值范围获取
                    bool getFlag2 = CurrCam.GetGainRangeValue(out long minGain, out long maxGain);
                    if (!getFlag)
                        Appentxt("增益参数设置范围值获取失败！");
                    {
                        //相机增益范围值设置
                        Model.GainMaxValue = (int)maxGain;
                        Model.GainMinValue = (int)minGain;
                        if (currCamGain > maxGain)
                            currCamGain = (int)maxGain;
                        else if (currCamGain < minGain)
                            currCamGain = (int)minGain;
                        //相机增益设置 
                        Model.GainSliderValue = currCamGain;
                        Model.GainNumricValue = currCamGain;
                        bool flag = CurrCam.SetGain(currCamGain);
                        if (!flag)
                            Appentxt(string.Format("工位：{0}，相机增益设置失败！", currCamStationName));
                    }

                    EnableCam(true);
                    workstatus = EunmCamWorkStatus.Freestyle;
                    return true;
                }
                else
                {
                    EnableCam(false);
                    Appentxt(string.Format("工位：{0}，相机打开失败：{1}", currCamStationName, msg));
                    return false;
                }
            })
                .ContinueWith(t => { openFlag= t.Result; });
            return openFlag;
        }
        /// <summary>
        /// 外部指令关闭相机
        /// </summary>
        /// <returns></returns>
        async public void CloseCam()
        {
            Appentxt("外部调用CloseCam指令关闭相机");

            await Task.Run(() =>
              {
                  CurrCam.CloseCam();
              }).
                  ContinueWith(t =>
                  {
                      EnableCam(false);
                  });      
        }
        /// <summary>
        ///  相机曝光设置
        ///  默认值为1000
        /// </summary>
        /// <param name="dValue">设置曝光参数</param>
        /// <returns>返回设置是否成功标志</returns>
        public bool SetExposure(long dValue)
        {
            if (CurrCam == null)
            {
                Appentxt("相机未实例化！");
                return false;
            }
            if (!CurrCam.IsAlive)
            {
                Appentxt("相机未链接！");
                return false;
            }
            //if (dValue < 1000 || dValue > 200000)
            //{
            //    Appentxt("请设置1000~200000之间合适的整数！");
            //    return false;
            //}
            Appentxt(string.Format("外部指令开启曝光参数设定，设定值：{0}", dValue));
            currCamExpouse = dValue;
            Model.ExpouseNumricValue = Model.ExpouseSliderValue=currCamExpouse;
            bool flag = CurrCam.SetExposureTime(currCamExpouse);
            if (!flag)
            {
                Appentxt("相机曝光设置失败！");
                return false;
            }            
            return true;
        }
        /// <summary>
        /// 获取曝光值
        /// </summary>
        /// <returns></returns>
        public long GetExposure()
        {
            return this.currCamExpouse;
        }
        /// <summary>
        /// 相机增益设置
        /// 默认值为0
        /// </summary>
        /// <param name="dValue">设置增益参数</param>
        /// <returns>返回设置是否成功标志</returns>
        public bool SetGain(long dValue)
        {
            if (CurrCam == null)
            {
                Appentxt("相机未实例化！");
                return false;
            }
            if (!CurrCam.IsAlive)
            {
                Appentxt("相机未链接！");
                return false;
            }
            //if (dValue < 0 || dValue > 10)
            //{
            //    Appentxt("请设置0~10之间合适的整数！");
            //    return false;
            //}
            Appentxt(string.Format("外部指令开启增益参数设定，设定值：{0}", dValue));
            currCamGain = Model.GainSliderValue;
            Model.GainNumricValue = currCamGain;
            bool flag = CurrCam.SetGain(currCamGain);
            if (!flag)
            {
                Appentxt("相机增益设置失败！");
                return false;
            }
               
            return true;
           
        }
        /// <summary>
        /// 获取增益值
        /// </summary>
        /// <returns></returns>
        public long GetGain()
        {
            return this.currCamGain;
        }
        /// <summary>
        /// 连续采集
        /// </summary>
        /// <returns></returns>
        public void ContinueGrab(bool flag = false)
        {

            Thread.Sleep(20);
            // lock (camlock)
            {
                if (flag)
                    Appentxt("内部指令开启了连续采集");
                else
                    Appentxt("外部指令开启了连续采集");
                workstatus = EunmCamWorkStatus.Freestyle;
                if (CurrCam == null) return;
                if (!CurrCam.IsAlive) return;

                Task.Run(() =>
                {
                    if (CurrCam.IsGrabing)
                    {
                        CurrCam.StopGrab();  //如果已在采集中则先停止采集
                        Thread.Sleep(20);
                    }

                    bool flag = CurrCam.ContinueGrab();
                    return flag;
                }).ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        Model.BtnOneShotEnable = false;
                        Model.BtnContinueGrabEnable = false;
                        Model.BtnStopGrabEnable = true;
                        camContinueGrabHandle?.Invoke(true);
                        ShowTool.SetEnable(false);
                    }
                    else
                    {
                        Model.BtnOneShotEnable = true;
                        Model.BtnContinueGrabEnable = true;
                        Model.BtnStopGrabEnable = false;
                        ShowTool.SetEnable(true);
                        Appentxt("重新开启连续采集失败，代码编号：1823");
                    }
                });
            }
        }
        /// <summary>
        /// 停止采集
        /// </summary>
        /// <returns></returns>
        public void StopGrab()
        {

            //   lock (camlock)
            {
                Appentxt("外部指令开启了停止采集");
                if (CurrCam == null) return;
                if (!CurrCam.IsAlive) return;
                //Task.Run(() =>
                //{
                if (CurrCam.IsGrabing)
                {
                    CurrCam.StopGrab();  //如果已在采集中则先停止采集
                    Thread.Sleep(20);
                }
                //}).ContinueWith(t =>
                //{
                Model.BtnOneShotEnable = true;
                Model.BtnContinueGrabEnable = true;
                Model.BtnStopGrabEnable = false;
                ShowTool.SetEnable(true);
                //});
            }
        }
        /// <summary>
        /// 单帧采集
        /// </summary>
        /// <returns></returns>
        public void OneShot()
        {

            Thread.Sleep(20);
            // lock (camlock)
            {
                Appentxt("外部指令开启了单帧采集");
                workstatus = EunmCamWorkStatus.Freestyle;
                if (CurrCam == null) return;
                if (!CurrCam.IsAlive) return;
                Task.Run(() =>
                {
                    if (CurrCam.IsGrabing)
                    {
                        CurrCam.StopGrab();  //如果已在采集中则先停止采集
                        Thread.Sleep(20);
                    }
                }).ContinueWith(t =>
                {
                    CurrCam.OneShot();
                });
            }
        }
        /// <summary>
        /// 资源释放
        /// </summary>
        public async void Release()
        {
            ContinueRunFlag = false;
         
            if (CurrCam != null)
            {
                StopGrab();
                CurrCam.setImgGetHandle -= GetImageDelegate;               
                CloseCam();
                Thread.Sleep(100);
            }
            ShowTool.Dispose();
            string path = rootFolder + "\\CommDev";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            CommDeviceController.SaveCommDev(path + "\\外部通讯.cdv");      
            CommDeviceController.DisposeConnect();
            CommDeviceController.ReleaseDev();

            if (GrabImg != null)
                GrabImg.Dispose();
            Disconnect();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
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
        void OnSaveWindowImageHnadle(object sender, EventArgs e)
        {
            SaveFileDialog m_SaveFileDialog = new SaveFileDialog();
            m_SaveFileDialog.Filter = "BMP文件|*.bmp*";
            m_SaveFileDialog.DereferenceLinks = true;

            if ((bool)m_SaveFileDialog.ShowDialog())
            {
                string name = m_SaveFileDialog.FileName;
                int index = name.LastIndexOf("\\");
                string direc = name.Substring(0, index);
                string filename = name.Substring(10, name.Length - 1 - index);
                SaveWindowImg(direc, filename);
            }
        }
        void 彩色显示ChangeEvent(object sender, EventArgs e)
        {
            isColorPalette = ShowTool.IsShowCoLorPalette;
            GeneralUse.WriteValue("图像制式", "彩色",
                isColorPalette.ToString(), "config", rootFolder + "\\Config");
        }
        void 显示中心十字坐标Event(object sender, EventArgs e)
        {
           bool showCross = ShowTool.IsShowCenterCross;
            Model.ShowCross = showCross;
            if (!showCross)
            {
                Model.AssistTool = EumAssistTool.None;
                Model.ShowScaleRule = false;
            }
            else
            {
              
                AddAssistToolToCross();
            }
            
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
            workstatus = EunmCamWorkStatus.Freestyle;
            CurrCam.OneShot();
            camContinueGrabHandle?.Invoke(false);    
        }
        //图像旋转
        void ImageGetRotationEvent(object sender, EventArgs e)
        {
         
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
        /// <summary>
        /// 是否进行彩色转换
        /// </summary>
        /// <param name="is2gray"></param>
        void Rgb2Gray(bool is2gray)
        {
            if (!ObjectValided(GrabImg))
                return;
            HOperatorSet.CountChannels(GrabImg, out HTuple channels);
            //原图显示    
            if (channels[0].I != 3)
                ;
            else
            {
                //是否需要转换标志
                if (is2gray)
                    //黑白显示
                    HOperatorSet.Rgb1ToGray(GrabImg, out GrabImg);

            }
            string ImageRotation = Enum.GetName(typeof(EumImageRotation), ShowTool.eumImageRotation);
            string[] buf = ImageRotation.Split('_');
            int rotationAngle = int.Parse(buf[1]);
            //ShowTool.ClearAllOverLays();
            try
            {
                ShowTool.DispImage(ref GrabImg, -rotationAngle);
              
            }
            catch (HOperatorException er)
            {
                Appentxt(er.Message);
                //  currvisiontool.DispImage( GrabImg);
            }
        }
        #endregion

        #region Cam
        /// <summary>
        /// 图像获取委托事件
        /// </summary>
        /// <param name="img"></param>
        void GetImageDelegate(HObject img)
        {
            //GC.Collect();
            objs.Clear();
            infos.Clear();

            if (workstatus == EunmCamWorkStatus.Freestyle) //自由模式只采图不做检测
                System.Threading.Thread.Sleep(10);

        
            //GrabImg.Dispose();
            HOperatorSet.CopyObj(img, out GrabImg, 1, 1);

            HOperatorSet.CountChannels(GrabImg, out HTuple channels);
            if (channels[0].I != 3)
            {
                ShowTool.SetColorChangeBtnEnable(false);//彩色切换按钮使能
                isColorPalette = false;
                ShowTool.IsShowCoLorPalette = false;
            }
            else
                ShowTool.SetColorChangeBtnEnable(true);//彩色切换按钮使能

            //彩色转换       
            Rgb2Gray(!isColorPalette);
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
            if (workstatus == EunmCamWorkStatus.Freestyle) //自由模式只采图不做检测
            {
                //if (!CurrCam.IsAlive) return;

                if (!CurrCam.IsContinueGrab)        
                return;
            }
            else
            {

                ManualOperateEnable(false);
                Appentxt(string.Format("相机当前工作状态：{0}",
                         Enum.GetName(typeof(EunmCamWorkStatus), workstatus)));

                if (workstatus == EunmCamWorkStatus.NinePointcLocation)  //9点标定定位模式
                {
                    DgPixelPointIndexer++;
                    StuCoordinateData data = RunTestFlowOfPosition();
                    NinePointsCalibViewModel.This.Model.DgPixelPointDataList.Add(
                          new DgPixelPointData(DgPixelPointIndexer, data.x, data.y));

                    if (data.x == 0 && data.y == 0 && data.angle == 0)
                    {
                      
                        NinePointsCalibViewModel.This.Model.TxbPixelX = double.NaN;
                        NinePointsCalibViewModel.This.Model.TxbPixelY = double.NaN;        
                        NinePointStatusDic.Add(DgPixelPointIndexer, false);
                        //sendToRobCmdMsg(string.Format("{0},{1},{2}", "NP", i.ToString(), "NG"));//发送模板匹配NG
                        virtualConnect.WriteData(string.Format("{0},{1},{2}", "NP", 
                            DgPixelPointIndexer.ToString(), "NG"));//发送模板匹配NG
                        PosTcpSendTool tool = GetToolToSendOfPos();
                        if (tool != null)
                        {
                            tool.SendData(string.Format("{0},{1},{2}", "NP",
                                        DgPixelPointIndexer.ToString(), "NG"));
                        }
                        Appentxt(string.Format("模板匹配失败，当期模板类型：{0}，9点标定无法获取像素坐标点",
                                Enum.GetName(typeof(EumModelType), currModelType)));
                        //MessageBox.Show("模板匹配失败，9点标定无法获取像素坐标点");
                        return;
                    }
                    else
                    {
                        NinePointsCalibViewModel.This.Model.TxbPixelX = data.x;
                        NinePointsCalibViewModel.This.Model.TxbPixelY = data.y;             
                        NinePointStatusDic.Add(DgPixelPointIndexer, true);
                        // sendToRobCmdMsg(string.Format("{0},{1},{2}", "NP", i.ToString(), "OK"));//发送模板匹配OK
                        virtualConnect.WriteData(string.Format("{0},{1},{2}", "NP", 
                            DgPixelPointIndexer.ToString(), "OK"));//发送模板匹配OK
                        PosTcpSendTool tool = GetToolToSendOfPos();
                        if (tool != null)
                        {
                            tool.SendData(string.Format("{0},{1},{2}", "NP",
                                         DgPixelPointIndexer.ToString(), "OK"));
                        }
                    }
                }
                else if (workstatus == EunmCamWorkStatus.RotatoLocation)  //旋转中心计定位模式
                {
                    DgRotatePointIndexer++;
                   StuCoordinateData data = RunTestFlowOfPosition();
                    NinePointsCalibViewModel.This.Model.DgRotatePointDataList.Add(
                         new DgRotatePointData (DgRotatePointIndexer, data.x, data.y));
                    if (data.x == 0 && data.y == 0 && data.angle == 0)
                    {

                        NinePointsCalibViewModel.This.Model.TxbRotatePixelX = double.NaN;
                        NinePointsCalibViewModel.This.Model.TxbRotatePixelY = double.NaN;                     
                        RotatoStatusDic.Add(DgRotatePointIndexer, false);
                        //sendToRobCmdMsg(string.Format("{0},{1},{2}", "C", k.ToString(), "NG"));//发送模板匹配NG
                        virtualConnect.WriteData(string.Format("{0},{1},{2}", "C",
                            DgRotatePointIndexer.ToString(), "NG"));//发送模板匹配NG   
                        // MessageBox.Show("定位失败，无法获取像素坐标点");
                        PosTcpSendTool tool = GetToolToSendOfPos();
                        if (tool != null)
                        {
                            tool.SendData(string.Format("{0},{1},{2}", "C",
                            DgRotatePointIndexer.ToString(), "NG"));
                        }
                        Appentxt("定位失败，无法获取像素坐标点");
                        return;
                    }
                    else
                    {
                        NinePointsCalibViewModel.This.Model.TxbRotatePixelX = data.x;
                        NinePointsCalibViewModel.This.Model.TxbRotatePixelY = data.y;
                        RotatoStatusDic.Add(DgRotatePointIndexer, true);
                        //sendToRobCmdMsg(string.Format("{0},{1},{2}", "C", k.ToString(), "OK"));//发送模板匹配OK
                        virtualConnect.WriteData(string.Format("{0},{1},{2}", "C",
                            DgRotatePointIndexer.ToString(), "OK"));//发送模板匹配OK
                        PosTcpSendTool tool = GetToolToSendOfPos();
                        if (tool != null)
                        {
                            tool.SendData(string.Format("{0},{1},{2}", "C",
                                DgRotatePointIndexer.ToString(), "OK"));
                        }
                    }

                }
                else if (workstatus == EunmCamWorkStatus.DeviationLocation)  //标定偏差校验
                {
                   StuCoordinateData data = RunTestFlowOfPosition();

                    Create_Physical_coorsys();
                    if (data.x == 0 && data.y == 0 && data.angle == 0)
                    {
                        virtualConnect.WriteData("NG");//发送模板匹配NG
                        PosTcpSendTool tool = GetToolToSendOfPos();
                        if (tool != null)
                        {
                            tool.SendData("NG");
                        }
                        Appentxt("定位失败，无法获取像素坐标点");
                        return;
                    }
                    else
                    {
                        HTuple centreRow = this.imageHeight / 2;
                        HTuple centreCol = this.imageWidth / 2;
                        HTuple _Rx, _Ry, _Rx2, _Ry2;
                        this.Transformation_POINT(data.x,
                                data.y, out _Rx, out _Ry);  //像素坐标转物理坐标                              
                        //_Rx = data.x;
                        //_Ry = data.y;
                        this.Transformation_POINT(centreCol,
                            centreRow, out _Rx2, out _Ry2);//像素坐标转物理坐标

                        HOperatorSet.GenContourPolygonXld(out HObject ho_Contour,
                              centreRow.TupleConcat(data.y), centreCol.TupleConcat(data.x));
                        ShowTool.DispRegion(ho_Contour, "green");
                        ShowTool.AddregionBuffer(ho_Contour, "green");
                        double offsetX = _Rx.D - _Rx2.D;
                        double offsetY = _Ry.D - _Ry2.D;
                        //  double distance = Math.Sqrt(Math.Pow(_Rx- _Rx2,2)+ Math.Pow(_Ry - _Ry2, 2));
                        virtualConnect.WriteData(string.Format("offsetX:{0:f3};offsetY:{1:f3}", offsetX, offsetY));//发送偏差数据
                        PosTcpSendTool tool = GetToolToSendOfPos();
                        if (tool != null)
                        {
                            tool.SendData(string.Format("offsetX:{0:f3};offsetY:{1:f3}", offsetX, offsetY));
                        }
                        Appentxt(string.Format("标定偏差校验，发送数据 offsetX:{0:f3};offsetY:{1:f3}", offsetX, offsetY));
                    }
                    ContinueGrab(true);
                }
                else if (workstatus == EunmCamWorkStatus.NormalTest_T1)  //正常定位测试(产品1)
                {
         
                   StuCoordinateData data = RunTestFlowOfPosition();
                    Create_Physical_coorsys();
                      
                    string buff = "[发送特征点位数据]";
                    buff += string.Format("x:{0:f3},y:{1:f3},a:{2:f3};",
                               data.x, data.y, data.angle);
                    Appentxt(buff);                   
                    virtualConnect.WriteData(buff.Replace("[发送特征点位数据]", ""));
                    PosTcpSendTool tool = GetToolToSendOfPos();
                    if (tool != null)
                    {
                        tool.SendData(buff.Replace("[发送特征点位数据]", ""));
                    }

                    stopwatch.Stop();
                    int spend = (int)stopwatch.ElapsedMilliseconds;
                    ShowTool.DetectionTime = spend;

                }
                else if (workstatus == EunmCamWorkStatus.NormalTest_T2)  //正常定位测试(产品2)
                {
                    StuCoordinateData data = RunTestFlowOfPosition();

                    Create_Physical_coorsys();
                   
                    string buff = "[发送特征点位数据]";
                    buff += string.Format("x:{0:f3},y:{1:f3},a:{2:f3};",
                             data.x, data.y, data.angle);
                    Appentxt(buff);                 
                    virtualConnect.WriteData(buff.Replace("[发送特征点位数据]", ""));

                    PosTcpSendTool tool = GetToolToSendOfPos();
                    if (tool != null)
                    {
                        tool.SendData(buff.Replace("[发送特征点位数据]", ""));
                    }
                    stopwatch.Stop();
                    int spend = (int)stopwatch.ElapsedMilliseconds;
                    ShowTool.DetectionTime = spend;

                }
                else if (workstatus == EunmCamWorkStatus.NormalTest_G)  //正常定位测试(点胶阀)
                {
                 
                    StuCoordinateData data = RunTestFlowOfPosition();
                    Create_Physical_coorsys();                  
                    string buff = "[发送特征点位数据]";
                    buff += string.Format("x:{0:f3},y:{1:f3},a:{2:f3};",
                              data.x, data.y, data.angle);
                    Appentxt(buff);
                    virtualConnect.WriteData(buff.Replace("[发送特征点位数据]", ""));
                    PosTcpSendTool tool = GetToolToSendOfPos();
                    if (tool != null)
                    {
                        tool.SendData(buff.Replace("[发送特征点位数据]", ""));
                    }
                    stopwatch.Stop();
                    int spend = (int)stopwatch.ElapsedMilliseconds;
                    ShowTool.DetectionTime = spend;
                }
                else if (workstatus == EunmCamWorkStatus.AutoFocus)  //自动对焦
                {
                    StuProcessFocusSendData stuProcessFocusSendData = new StuProcessFocusSendData(-1);
                    bool flag = AutoFocus.calculateState(GrabImg, autoFocusRegion, DeviationThd, LimitMethd);
                    if (!flag)
                    {
                        stuProcessFocusSendData.eumtendency = Eumtendency.error;
                        Appentxt("对焦错误，请检查流程及参数！");
                    }
                    else
                        stuProcessFocusSendData = AutoFocus.sendCmdOfAutoAdjust();

                    Appentxt(stuProcessFocusSendData.ToString());
                    AutoFocusDataHandle?.Invoke(stuProcessFocusSendData, null);
                }
                else if (workstatus == EunmCamWorkStatus.AOI) //胶水AOI检测
                {

                    Appentxt("图像采集完成，开始胶水AOI检测");
                    bool resultFlag = RunTestFlowOfGlue();
                    Appentxt("胶水AOI检测完成，结果：" + (resultFlag ? "OK" : "NG"));
                    virtualConnect.WriteData(resultFlag ? "OK" : "NG");//发送胶水AOI检测结果
                    GlueTcpSendTool tool = GetToolToSendOfGlue();
                    if (tool != null)
                    {
                        tool.SendData(resultFlag ? "OK" : "NG");
                    }
                }         
                else if (workstatus == EunmCamWorkStatus.Trajectory)
                {
                    
                }

            }
        }
        //相机连接状态
        void CamConnectEvent(object sender, EventArgs e)
        {
            CamConnectStatusHandle?.Invoke(sender, e);
        }

        #endregion

        #region TCP
        /// <summary>
        /// 获取定位检测TCP发送工具
        /// </summary>
        /// <returns></returns>
        PosTcpSendTool GetToolToSendOfPos()
        {
            //string name = this.projectOfPos.TcpSendName;
            int index = this.projectOfPos.toolNamesList.FindIndex(t => t.Contains("Tcp发送"));

            if (index < 0) return null;
            string name = this.projectOfPos.toolNamesList[index];
            if (this.projectOfPos.toolNamesList.Contains(name))
            {
                PosBaseTool tool = this.projectOfPos.toolsDic[name];
                if (tool.GetType() == typeof(PosTcpSendTool))
                    return (PosTcpSendTool)tool;
            }
            return null;
        }
        /// <summary>
        /// 获取定位检测TCP发送工具
        /// </summary>
        /// <returns></returns>
        GlueTcpSendTool GetToolToSendOfGlue()
        {
            //string name = this.projectOfGlue.TcpSendName;
            int index = this.projectOfGlue.toolNamesList.FindIndex(t => t.Contains("Tcp发送"));

            if (index < 0) return null;
            string name = this.projectOfGlue.toolNamesList[index];
            if (this.projectOfGlue.toolNamesList.Contains(name))
            {
                GlueBaseTool tool = this.projectOfGlue.toolsDic[name];
                if (tool.GetType() == typeof(GlueTcpSendTool))
                    return (GlueTcpSendTool)tool;
            }
            return null;
        }
        /// <summary>
        /// 流程处理
        /// </summary>
        /// <param name="strData"></param>
        void FlowHandle(string strData)
        {
            if (strData.Contains("NP,") || strData.Contains("C,") || strData.Contains("Deviation")
       || strData.Contains("T1") || strData.Contains("T2") || strData.Contains("G")
         || strData.Contains("AF") || strData.Contains("AOI") || strData.Contains("Trajectory"))

            {
                if (strData.Contains("NP,")) //9点标定流程
                {
                    if (currModelType != EumModelType.CaliBoardModel)
                        SwitchModelType(EumModelType.CaliBoardModel);
                    string[] tempdataArray = strData.Split(',');
                    #region---//9点标定流程----------
                    switch (tempdataArray[1])
                    {
                        case "S":

                            //检测当前是否已经做好模板
                            //检查当前是否相机正常连接
                            //清除历史标记点位
                            //发送准备好信号，等待9点标记
                            DgPixelPointIndexer = 0;
                            NinePointsCalibViewModel.This.Model.DgPixelPointDataList.Clear();
                            DgRotatePointIndexer = 0;
                            NinePointsCalibViewModel.This.Model.DgRobotPointDataList.Clear();

                            NinePointStatusDic.Clear();
                            if (this.projectOfPos.toolsDic.Count > 0 &&
                                    CurrCam.IsAlive)
                            {
                                virtualConnect.WriteData("NP,S,OK");   //准备OK
                                Appentxt("9点标定准备好，开始标定");
                            }
                            else
                                virtualConnect.WriteData("NP,S,NG");  //未准备好
                            break;
                        case "E":
                            //校验9次模板匹配是否OK
                            //检测当前标定关系转换是否正常                     
                            //发送标定结果信号

                            bool flag = true;
                            foreach (var s in NinePointStatusDic)
                                flag &= s.Value;
                            Task.Factory.StartNew(new Action(() =>
                            {
                                NinePointsCalibModel model = NinePointsCalibViewModel.This.Model;
                                bool genFlag = NinePointsCalibTool.GenNineCaliMatrix(ref model,
                                      out string info);
                                if (!genFlag) Appentxt(info);
                                NinePointsCalibViewModel.This.Model = model;
                                NinePointsCalibTool.SaveNineCaliData(NinePointsCalibViewModel.This.Model,
                                    rootFolder, currCalibName);

                            })).ContinueWith(t =>
                            {
                                virtualConnect.WriteData(string.Format("{0},{1}", "NP,E", flag ? "OK" : "NG"));
                                Appentxt("9点标定结束，标定结果" + (flag ? "OK" : "NG"));
                            });
                            break;
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                        case "6":
                        case "7":
                        case "8":
                        case "9":
                            //记录xy机械坐标点
                            //相机采集，模板匹配
                            //发送匹配结果信号
                            int key = int.Parse(tempdataArray[1]);
                            if (NinePointStatusDic.ContainsKey(key))
                            {
                                //TCPInfoAddText(string.Format("当前已经标记过第{0}点位", key));
                                virtualConnect.WriteData(string.Format("{0},{1},{2}", tempdataArray[0],
                                    tempdataArray[1], "NG"));
                            }
                            else
                            {
                                DgRobotPointIndexer = int.Parse(tempdataArray[1]);
                                double rx = double.Parse(tempdataArray[2]);
                                double ry = double.Parse(tempdataArray[3]);
                                NinePointsCalibViewModel.This.Model.DgRobotPointDataList.Add(
                                    new DgRobotPointData(DgRobotPointIndexer, rx, ry));

                                ManualOperateEnable(false);
                                OneGrab(EunmCamWorkStatus.NinePointcLocation);

                            }
                            break;

                    }
                    #endregion
                }
                else if (strData.Contains("C,"))//旋转中心标定流程
                {
                    if (currModelType != EumModelType.CaliBoardModel)
                        SwitchModelType(EumModelType.CaliBoardModel);

                    string[] tempdataArray = strData.Split(',');
                    #region---//旋转中心标定流程---------
                    switch (tempdataArray[1])
                    {
                        case "S":

                            //检测当前是否已经做好模板
                            //检查当前是否相机正常连接
                            //清除历史标记点位
                            //发送准备好信号，等待旋转中心标定
                            DgRotatePointIndexer = 0;
                            NinePointsCalibViewModel.This.Model.DgRotatePointDataList.Clear();
                            RotatoStatusDic.Clear();
                            if (this.projectOfPos.toolsDic.Count > 0 &&
                                    CurrCam.IsAlive)
                            {
                                virtualConnect.WriteData("C,S,OK");   //准备OK
                                Appentxt("旋转中心标定准备好,开始标定");
                            }

                            else
                                virtualConnect.WriteData("C,S,NG");  //未准备好

                            break;
                        case "E":
                            //校验5次模板匹配是否OK
                            //计算旋转中心                
                            //发送旋转中心标定结果信号

                            bool flag = true;
                            foreach (var s in RotatoStatusDic)
                                flag &= s.Value;

                            Task.Factory.StartNew(new Action(() =>
                            {
                                NinePointsCalibModel model = NinePointsCalibViewModel.This.Model;
                                bool calFlag = NinePointsCalibTool.CalRotateCenter(ref model, out string info);
                                if (!calFlag) Appentxt(info);
                                NinePointsCalibViewModel.This.Model = model;
                                NinePointsCalibTool.SaveRatateData(NinePointsCalibViewModel.This.Model,
                                    rootFolder, currCalibName);

                            })).ContinueWith(t =>
                            {
                                virtualConnect.WriteData(string.Format("{0},{1}", "C,E", flag ? "OK" : "NG"));
                                Appentxt("旋转中心标定结束，标定结果" + (flag ? "OK" : "NG"));
                            });
                            break;
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                            //相机采集，模板匹配
                            //发送匹配结果信号
                            int key = int.Parse(tempdataArray[1]);
                            if (RotatoStatusDic.ContainsKey(key))
                            {
                                //CPInfoAddText(string.Format("当前旋转已经标记过第{0}点位", key));
                                virtualConnect.WriteData(string.Format("{0},{1},{2}", tempdataArray[0],
                                    tempdataArray[1], "NG"));
                            }
                            else
                            {

                                ManualOperateEnable(false);
                                OneGrab(EunmCamWorkStatus.RotatoLocation);

                            }
                            break;

                    }
                    #endregion
                }
                else if (strData.Equals("Deviation"))//标定偏差校验
                {
                    if (currModelType != EumModelType.CaliBoardModel)
                        SwitchModelType(EumModelType.CaliBoardModel);
                    if (this.projectOfPos.toolsDic.Count > 0 &&
                                  CurrCam.IsAlive)
                    {
                        Appentxt("标定准备好,开始偏差校验");
                        ManualOperateEnable(false);
                        OneGrab(EunmCamWorkStatus.DeviationLocation);
                    }

                    else
                    {
                        virtualConnect.WriteData("NG");  //未准备好
                        Appentxt("标定未准备好,校验失败");
                    }

                }
                else if (strData.Equals("T1"))
                {
                    if (currModelType != EumModelType.ProductModel_1)
                        SwitchModelType(EumModelType.ProductModel_1);

                    stopwatch.Restart();
                    ManualOperateEnable(false);
                    Appentxt("开始自动检测,使用模板为产品1模板！");
                    OneGrab(EunmCamWorkStatus.NormalTest_T1);
                   
                }
                else if (strData.Equals("T2"))
                {
                    if (currModelType != EumModelType.ProductModel_2)
                        SwitchModelType(EumModelType.ProductModel_2);

                    stopwatch.Restart();
                    ManualOperateEnable(false);
                    Appentxt("开始自动检测,使用模板为产品2模板！");
                    OneGrab(EunmCamWorkStatus.NormalTest_T2);

                }
                else if (strData.Equals("G"))
                {

                    if (currModelType != EumModelType.GluetapModel)
                        SwitchModelType(EumModelType.GluetapModel);

                    stopwatch.Restart();
                    ManualOperateEnable(false);
                    Appentxt("开始点胶阀示教检测");
                    OneGrab(EunmCamWorkStatus.NormalTest_G);
                  
                }
                else if (strData.Contains("AF"))
                {
                    switch (strData)
                    {
                        case "Request AF":
                            if (!GuidePositioning_HDevelopExport.ObjectValided(ShowTool.D_HImage) ||
                                ShowTool.D_HImage == null)
                            {
                                AutoFocusDataHandle?.Invoke("Operation exception", null);

                                Appentxt("图像为空，请采集一张图片！");
                                return;
                            }

                            AutoFocus.resetData();
                            bool showCross = ShowTool.IsShowCenterCross;
                            if (!showCross)
                            {
                                Model.AssistTool = EumAssistTool.None;
                                AddAssistToolToCross();
                            }
                            System.Threading.Thread.Sleep(500);

                            if (!ObjectValided(autoFocusRegion) ||
                               autoFocusRegion == null)
                            {
                                AutoFocusDataHandle?.Invoke("Operation exception", null);
                                Appentxt("对焦区域为空，请先打开辅助工具！");
                                return;
                            }
                            AutoFocusDataHandle?.Invoke("Ready AF", null);
                            //workstatus = EunmcurrCamWorkStatus.AutoFocus;
                            Appentxt("开始自动对焦");
                            break;
                        case "AFT":
                            ManualOperateEnable(false);
                            OneGrab(EunmCamWorkStatus.AutoFocus); //Z自动对焦         
                            Appentxt("自动对焦进行中");
                            break;
                        case "Finish AF":
                            workstatus = EunmCamWorkStatus.None;
                            AutoFocusDataHandle?.Invoke("OK AF", null);
                            Appentxt("自动对焦结束");
                            break;
                    }

                }
                else if (strData.Equals("AOI"))//胶水外观检测请求
                {
                    stopwatch.Restart();
                    ManualOperateEnable(false);
                    Appentxt("开始胶水AOI外观检测");
                    OneGrab(EunmCamWorkStatus.AOI);
                   

                }
                else if (strData.Contains("Trajectory"))//轨迹提取
                {

                }
                else
                    return;
            }
        }
        /// <summary>
        /// 远程连接
        /// </summary>
        private void PosTcpServer_RemoteConnect(string key)
        {
          
            Appentxt(string.Format("Pos客户端[{0}]上线", key));
         
        }
        /// <summary>
        /// 远程关闭
        /// </summary>
        /// <param name="key"></param>
        private void PosTcpServer_RemoteClose(string key)
        {
            Appentxt(string.Format("Pos客户端[{0}]下线", key));

        }

        /// <summary>
        /// 远程关闭
        /// </summary>
        /// <param name="key"></param>
        private void PosTcpClient_RemoteClose(string key)
        {       
            Appentxt( string.Format("Pos服务端[{0}]断开连接", key));       
        }

        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void PosTcpServer_ReceiveData(IPEndPoint remote, byte[] buffer, int count)
        {

            if (buffer == null || count <= 0 || buffer.Length < count)
                return;
            string buf = "";
            string strData = string.Empty;
            strData = System.Text.Encoding.Default.GetString(buffer, 0, count);
            buf += "[Pos：" + remote.ToString() + "]";
            buf += "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "]";
            buf += strData;
            Appentxt(buf);
            if (!ContinueRunFlag)
            {
                Appentxt("请开启连续运行");
                return;
            }
            if (strData.Contains("AOI")) return;//定位流程忽略AOI触发信号
            Monitor.Enter(locker);
            FlowHandle(strData);
            Monitor.Exit(locker);
        }
        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void PosTcpClient_ReceiveData(IPEndPoint remote, byte[] buffer, int count)
        {
            if (buffer == null || count <= 0 || buffer.Length < count)
                return;
            string buf = "";
            string strData = string.Empty;
            strData = System.Text.Encoding.Default.GetString(buffer, 0, count);
            buf += "[Pos：" + remote.ToString() + "]";
            buf += "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "]";
            buf += strData;
            Appentxt(buf);
            if (!ContinueRunFlag)
            {
                Appentxt("请开启连续运行");
                return;
            }
            if (strData.Contains("AOI")) return;//定位流程忽略AOI触发信号
            Monitor.Enter(locker);
            FlowHandle(strData);
            Monitor.Exit(locker);
        }
        /// <summary>
        /// 远程连接
        /// </summary>
        private void GlueTcpServer_RemoteConnect(string key)
        {

            Appentxt(string.Format("Glue客户端[{0}]上线", key));

        }
        /// <summary>
        /// 远程关闭
        /// </summary>
        /// <param name="key"></param>
        private void GlueTcpServer_RemoteClose(string key)
        {
            Appentxt(string.Format("Glue客户端[{0}]下线", key));

        }

        /// <summary>
        /// 远程关闭
        /// </summary>
        /// <param name="key"></param>
        private void GlueTcpClient_RemoteClose(string key)
        {
            Appentxt(string.Format("Glue服务端[{0}]断开连接", key));
        }

        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void GlueTcpServer_ReceiveData(IPEndPoint remote, byte[] buffer, int count)
        {

            if (buffer == null || count <= 0 || buffer.Length < count)
                return;
            string buf = "";
            string strData = string.Empty;
            strData = System.Text.Encoding.Default.GetString(buffer, 0, count);
            buf += "[Glue：" + remote.ToString() + "]";
            buf += "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "]";
            buf += strData;
            Appentxt(buf);
            if (!ContinueRunFlag)
            {
                Appentxt("请开启连续运行");
                return;
            }
            if (!strData.Contains("AOI")) return;//只处理AOI流程
            Monitor.Enter(locker);
            FlowHandle(strData);
            Monitor.Exit(locker);
        }
        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        private void GlueTcpClient_ReceiveData(IPEndPoint remote, byte[] buffer, int count)
        {
            if (buffer == null || count <= 0 || buffer.Length < count)
                return;
            string buf = "";
            string strData = string.Empty;
            strData = System.Text.Encoding.Default.GetString(buffer, 0, count);
            buf += "[Glue：" + remote.ToString() + "]";
            buf += "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "]";
            buf += strData;
            Appentxt(buf);
            if (!ContinueRunFlag)
            {
                Appentxt("请开启连续运行");
                return;
            }
            if (!strData.Contains("AOI")) return;//只处理AOI流程
            Monitor.Enter(locker);
            FlowHandle(strData);
            Monitor.Exit(locker);
        }
        #endregion

    }
}
