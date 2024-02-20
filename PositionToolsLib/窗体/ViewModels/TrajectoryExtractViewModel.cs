using ControlShareResources.Common;
using FunctionLib.Location;
using HalconDotNet;
using PositionToolsLib.参数;
using PositionToolsLib.工具;
using PositionToolsLib.窗体.Models;
using PositionToolsLib.窗体.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisionShowLib.UserControls;

namespace PositionToolsLib.窗体.ViewModels
{
    public class TrajectoryExtractViewModel : BaseViewModel
    {
        public Action FramCompleteLoad = null;
        EumTrackType currTrackType = EumTrackType.AnyCurve;
        HTuple matrix2D = null;
        Dictionary<string, HObject> trajectoryInspectObjDic = new Dictionary<string, HObject>();
        //public Action<TrajectoryTypeBaseTool,EumTrackType> SetFrameAction = null;
        public static TrajectoryExtractViewModel This { get; set; }
        public TrajectoryExtractModel Model { get; set; }
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase UsePosiCorrectCheckedCommand { get; set; }
        public CommandBase MatrixSelectionChangedCommand { get; set; }
        public CommandBase TrajectorySelectionChangedCommand { get; set; }
        public CommandBase DgMouseDoubleClickCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }
      
        public TrajectoryExtractViewModel(BaseTool tool) : base(tool)
        {
            //SetFrameAction = setFrameAction;
            dataManage = tool.GetManage();
            This = this;
            Model = new TrajectoryExtractModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
            FramCompleteLoad = OnFramCompleteLoad;

            #region Command

            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            UsePosiCorrectCheckedCommand = new CommandBase();
            UsePosiCorrectCheckedCommand.DoExecute = new Action<object>((o) => chxbUsePosiCorrect_CheckedChanged());
            UsePosiCorrectCheckedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            MatrixSelectionChangedCommand = new CommandBase();
            MatrixSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxMatrixList_SelectedIndexChanged(o));
            MatrixSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TrajectorySelectionChangedCommand = new CommandBase();
            TrajectorySelectionChangedCommand.DoExecute = new Action<object>((o) => cobxTrajectoryList_SelectedIndexChanged(o));
            TrajectorySelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DgMouseDoubleClickCommand = new CommandBase();
            DgMouseDoubleClickCommand.DoExecute = new Action<object>((o) => dataGridViewEx1_DoubleClick(o));
            DgMouseDoubleClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            #endregion
            ShowData();
            cobxImageList_SelectedIndexChanged(null);
            cobxMatrixList_SelectedIndexChanged(null);
            cobxTrajectoryList_SelectedIndexChanged(null);
        }
        /// <summary>
        /// 轨迹参数page加载完成
        /// </summary>
        void OnFramCompleteLoad()
        {
            BaseParam par = baseTool.GetParam();
            (par as TrajectoryExtractParam).TrackType = currTrackType;
    
            switch (currTrackType)
            {
                case EumTrackType.AnyCurve:
                    if ((par as TrajectoryExtractParam).TrajectoryTool == null ||
                          (par as TrajectoryExtractParam).TrajectoryTool.GetType() !=
                          typeof(TrajectoryTypeAnyCurveTool))
                    {
                        (par as TrajectoryExtractParam).TrajectoryTool =
                                   new TrajectoryTypeAnyCurveTool();
                        this.trajectoryInspectObjDic?.Clear();
                    }
                       
                    if (AnyCurvePage.This != null)
                    {
                        AnyCurvePage.This.OnSaveTool = OnSaveToolEvent;
                        AnyCurvePage.This.OnDrawRegion = OnDrawRegionEvent;
                        AnyCurvePage.This.OnDrawMaskRegion = OnDrawMaskRegionEvent;
                        AnyCurvePage.This.OnAnyCurveIdentify = OnTrajectoryIdentifyEvent;
                        AnyCurvePage.This.SetTool((par as TrajectoryExtractParam).TrajectoryTool);
                    }
                    break;
                case EumTrackType.Line:
                    if ((par as TrajectoryExtractParam).TrajectoryTool == null||
                        (par as TrajectoryExtractParam).TrajectoryTool.GetType()!=
                        typeof(TrajectoryTypeLineTool))
                    {
                        (par as TrajectoryExtractParam).TrajectoryTool =
                                   new TrajectoryTypeLineTool();
                        this.trajectoryInspectObjDic?.Clear();
                    }
                       
                    if (LinePage.This != null)
                    {
                        LinePage.This.OnSaveTool = OnSaveToolEvent;
                        LinePage.This.OnDrawRegion= OnDrawRegionEvent;
                        LinePage.This.OnLineIdentify = OnTrajectoryIdentifyEvent;
                        LinePage.This.SetTool((par as TrajectoryExtractParam).TrajectoryTool);
                    }
                   
                
                    break;
                case EumTrackType.Circle:
                    if ((par as TrajectoryExtractParam).TrajectoryTool == null ||
                        (par as TrajectoryExtractParam).TrajectoryTool.GetType() !=
                        typeof(TrajectoryTypeCircleTool))
                    {
                        (par as TrajectoryExtractParam).TrajectoryTool =
                                   new TrajectoryTypeCircleTool();
                        this.trajectoryInspectObjDic?.Clear();
                    }
                       
                    if (CirclePage.This != null)
                    {
                        CirclePage.This.OnSaveTool = OnSaveToolEvent;
                        CirclePage.This.OnDrawRegion = OnDrawRegionEvent;
                        CirclePage.This.OnCircleIdentify = OnTrajectoryIdentifyEvent;
                        CirclePage.This.SetTool((par as TrajectoryExtractParam).TrajectoryTool);
                    }
                    break;
                case EumTrackType.Rectangle:
                    if ((par as TrajectoryExtractParam).TrajectoryTool == null ||
                         (par as TrajectoryExtractParam).TrajectoryTool.GetType() !=
                        typeof(TrajectoryTypeRectangleTool))
                    {
                        (par as TrajectoryExtractParam).TrajectoryTool =
                                   new TrajectoryTypeRectangleTool();
                        this.trajectoryInspectObjDic?.Clear();
                    }
                        
                    if (RectanglePage.This != null)
                    {
                        RectanglePage.This.OnSaveTool = OnSaveToolEvent;
                        RectanglePage.This.OnDrawRegion = OnDrawRegionEvent;
                        RectanglePage.This.OnRectangleIdentify = OnTrajectoryIdentifyEvent;
                        RectanglePage.This.SetTool((par as TrajectoryExtractParam).TrajectoryTool);
                    }
                    break;
                case EumTrackType.RRectangle:
                    if ((par as TrajectoryExtractParam).TrajectoryTool == null||
                         (par as TrajectoryExtractParam).TrajectoryTool.GetType() !=
                        typeof(TrajectoryTypeRRectangleTool))
                    {
                        (par as TrajectoryExtractParam).TrajectoryTool =
                                   new TrajectoryTypeRRectangleTool();
                        this.trajectoryInspectObjDic?.Clear();
                    }
                      
                    if (RRectanglePage.This != null)
                    {
                        RRectanglePage.This.OnSaveTool = OnSaveToolEvent;
                        RRectanglePage.This.OnDrawRegion = OnDrawRegionEvent;
                        RRectanglePage.This.OnRRectangleIdentify = OnTrajectoryIdentifyEvent;
                        RRectanglePage.This.SetTool((par as TrajectoryExtractParam).TrajectoryTool);
                    }
                    break;
            }
           
        }
        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData()
        {
            BaseParam par = baseTool.GetParam();
            this.trajectoryInspectObjDic = (par as TrajectoryExtractParam).TrajectoryInspectObjDic;
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as TrajectoryExtractParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
            Model.SelectImageName = (par as TrajectoryExtractParam).InputImageName;

            foreach (var s in dataManage.matrixBufDic)
                Model.MatrixList.Add(s.Key);
            string matrixName = (par as TrajectoryExtractParam).MatrixName;
            int index2 = Model.MatrixList.IndexOf(matrixName);
            Model.SelectMatrixIndex = index2;    
            Model.SelectMatrixName = (par as TrajectoryExtractParam).MatrixName;


            currTrackType = (par as TrajectoryExtractParam).TrackType;
            int index3=  Model.TrackTypeList.IndexOf(currTrackType);
            Model.SelectTrajectoryIndex = index3;
            Model.SelectTrajectoryName = currTrackType.ToString();


            Model.UsePosiCorrectChecked = (par as TrajectoryExtractParam).UsePosiCorrect;
            if ((par as TrajectoryExtractParam).UsePosiCorrect)
                Model.MatrixEnable = true;
            else
                Model.MatrixEnable = false;
        }
        /// <summary>
        /// 图像加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadedImageNoticeEvent(object sender, EventArgs e)
        {
            HOperatorSet.GenEmptyObj(out imgBuf);
            imgBuf.Dispose();
            imgBuf = ShowTool.D_HImage;
        }

        /// <summary>
        ///输入图像选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxImageList_SelectedIndexChanged(object value)
        {
            if (Model.SelectImageIndex == -1) return;
            if (!dataManage.imageBufDic.ContainsKey(Model.SelectImageName)) return;
            if (!BaseTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as TrajectoryExtractParam).InputImageName = Model.SelectImageName;
        }

        /// <summary>
        ///输入矩阵旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxMatrixList_SelectedIndexChanged(object value)
        {
            if (Model.SelectMatrixIndex == -1) return;
            matrix2D = dataManage.matrixBufDic[Model.SelectMatrixName];
            BaseParam par = baseTool.GetParam();
            (par as TrajectoryExtractParam).MatrixName = Model.SelectMatrixName;
          
        }
        /// <summary>
        /// 轨迹类型选择
        /// </summary>
        /// <param name="value"></param>
        private void cobxTrajectoryList_SelectedIndexChanged(object value)
        {
            if (Model.SelectTrajectoryIndex == -1) return;
            currTrackType = Model.TrackTypeList[Model.SelectTrajectoryIndex];
            BaseParam par = baseTool.GetParam();
            (par as TrajectoryExtractParam).TrackType = currTrackType;

            switch (currTrackType)
            {
                case EumTrackType.AnyCurve:
                    Model.FramePath = new Uri("../pages/AnyCurvePage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case EumTrackType.Line:
                  
                    Model.FramePath = new Uri("../pages/LinePage.xaml", UriKind.RelativeOrAbsolute);
              
                    break;
                case EumTrackType.Circle:
                    Model.FramePath = new Uri("../pages/CirclePage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case EumTrackType.Rectangle:
                    Model.FramePath = new Uri("../pages/RectanglePage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case EumTrackType.RRectangle:
                    Model.FramePath = new Uri("../pages/RRectanglePage.xaml", UriKind.RelativeOrAbsolute);
                    break;
            }

        }
        /// <summary>
        /// 轨迹参数page保存事件
        /// </summary>
        /// <param name="tool"></param>
        void OnSaveToolEvent(TrajectoryTypeBaseTool tool)
        {
            BaseParam par = baseTool.GetParam();
            (par as TrajectoryExtractParam).TrajectoryTool= tool;
            baseTool.SetParam(par);
        }
        /// <summary>
        /// 轨迹参数page轨迹识别
        /// </summary>
        void OnTrajectoryIdentifyEvent()
        {
            if (currTrackType == EumTrackType.Line)
                OnLineTrajectoryIdentify();
           else if (currTrackType == EumTrackType.RRectangle)
                OnRRectangleTrajectoryIdentify();
            else if (currTrackType == EumTrackType.Rectangle)
                OnRectangleTrajectoryIdentify();
            else if (currTrackType == EumTrackType.Circle)
                OnCircleTrajectoryIdentify();
            else if (currTrackType == EumTrackType.AnyCurve)
                OnAnyCurveTrajectoryIdentify();
        }
        /// <summary>
        ///  轨迹参数page检测区域绘制
        /// </summary>
        /// <param name="btnName"></param>
        void OnDrawRegionEvent(string btnName)
        {
            if (!BaseTool.ObjectValided(imgBuf))
            {
                ShowTool.DispAlarmMessage("未获取图像", 500, 20, 30);
                return;
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.DesignModel);
            ShowTool.setMouseStateOfNone();
            if (currTrackType == EumTrackType.Line)
            {
                if (trajectoryInspectObjDic.ContainsKey("Line:直线1"))
                    trajectoryInspectObjDic["Line:直线1"] = DrawLineDetectRegion().Clone();
                else
                    trajectoryInspectObjDic.Add("Line:直线1", DrawLineDetectRegion().Clone());
            }
            else if (currTrackType == EumTrackType.RRectangle)
            {
                int index = RRectanglePage.This.LineIndex;
                if (index < 0)
                {
                    return;
                }
                BaseParam par = baseTool.GetParam();
                TrajectoryTypeRRectangleParam parma = (par as TrajectoryExtractParam).TrajectoryTool.param as TrajectoryTypeRRectangleParam;
                if (trajectoryInspectObjDic.ContainsKey("RRectangle:直线" + (index + 1)))


                    trajectoryInspectObjDic["RRectangle:直线" + (index + 1)] = DrawLineDetectRegion(parma.GetParamFormIndex(index)).Clone();
                else
                    trajectoryInspectObjDic.Add("RRectangle:直线" + (index + 1), DrawLineDetectRegion(parma.GetParamFormIndex(index)).Clone());
            }
            else if (currTrackType == EumTrackType.Rectangle)
            {
                int index = RectanglePage.This.LineIndex;
                if (index < 0)
                {
                    return;
                }
                BaseParam par = baseTool.GetParam();
                TrajectoryTypeRectangleParam parma = (par as TrajectoryExtractParam).TrajectoryTool.param as TrajectoryTypeRectangleParam;
                if (trajectoryInspectObjDic.ContainsKey("Rectangle:直线" + (index + 1)))


                    trajectoryInspectObjDic["Rectangle:直线" + (index + 1)] = DrawLineDetectRegion(parma.GetParamFormIndex(index)).Clone();
                else
                    trajectoryInspectObjDic.Add("Rectangle:直线" + (index + 1), DrawLineDetectRegion(parma.GetParamFormIndex(index)).Clone());
            }
            else if (currTrackType == EumTrackType.Circle)
            {
                if (trajectoryInspectObjDic.ContainsKey("Circle:圆1"))
                    trajectoryInspectObjDic["Circle:圆1"] = DrawCircleDetectRegion().Clone();
                else
                    trajectoryInspectObjDic.Add("Circle:圆1", DrawCircleDetectRegion().Clone());
            }
            else if (currTrackType == EumTrackType.AnyCurve)
            {
                if (trajectoryInspectObjDic.ContainsKey("AnyCurve:任意1"))
                    trajectoryInspectObjDic["AnyCurve:任意1"] = DrawAnyCurveDetectRegion().Clone();
                else
                    trajectoryInspectObjDic.Add("AnyCurve:任意1", DrawAnyCurveDetectRegion().Clone());
            }
        }
        #region  AnyCurvePage
        /// <summary>
        /// 绘制任意轨迹检测区域
        /// </summary>
        /// <returns></returns>
        HObject DrawAnyCurveDetectRegion()
        {
            if (MessageBox.Show("准备创建任意轨迹检测区域？", "Information",
                      MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                TrajectoryExtractParam par = baseTool.GetParam() as TrajectoryExtractParam;

                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
                //Model.BtnDrawRegionEnable = false;
                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");
                TrajectoryTypeAnyCurveParam param = par.TrajectoryTool.param as TrajectoryTypeAnyCurveParam;
                EumRegionTypeOfGJ typeOfGJ = param.RegionType;
                //绘制区域
                HObject inspectROI;
                if (typeOfGJ == EumRegionTypeOfGJ.any)
                    HOperatorSet.DrawRegion(out  inspectROI, ShowTool.HWindowsHandle);
                else if (typeOfGJ == EumRegionTypeOfGJ.rectangle)
                {
                    HOperatorSet.DrawRectangle1(ShowTool.HWindowsHandle, out HTuple row1, out HTuple column1,
                        out HTuple row2, out HTuple column2);
                    HOperatorSet.GenRectangle1(out  inspectROI, row1, column1, row2, column2);
                }             
                else
                {
                    HOperatorSet.DrawCircle(ShowTool.HWindowsHandle, out HTuple row, out HTuple column,
                      out HTuple radius);
                    HOperatorSet.GenCircle(out  inspectROI, row, column, radius);
                }
                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);
               
                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(inspectROI, "blue");
                ShowTool.AddregionBuffer(inspectROI, "blue");

                ShowTool.AddRightMenu();
                //Model.BtnDrawRegionEnable = true;
                return inspectROI;
            }
            else
                return null;

        }
        /// <summary>
        /// 绘制任意轨迹掩膜区域
        /// </summary>
        void OnDrawMaskRegionEvent()
        {
            TrajectoryExtractParam par = baseTool.GetParam() as TrajectoryExtractParam;
            TrajectoryTypeAnyCurveParam param = par.TrajectoryTool.param as TrajectoryTypeAnyCurveParam;
            //if (!VisionShowTool.ObjectValided(param.InspectRegion))
            //{
            //    ShowTool.DispAlarmMessage("轨迹检测区域未设置！", 500, 20, 30);
            //    return;
            //}
            if (!VisionShowTool.ObjectValided(imgBuf))
            {
                ShowTool.DispAlarmMessage("图像为空！", 500, 20, 30); 
                return;
            }
            HObject mask=null;
            if (VisionShowTool.ObjectValided(param.MaskRegion))
              HOperatorSet.CopyObj(param.MaskRegion, out  mask, 1, -1);

            PositionToolsLib.窗体.Views.FormTemplateMaking _frmTemplateMaking =
                 new PositionToolsLib.窗体.Views.FormTemplateMaking(imgBuf,
                  mask, "", EumMakeType.Trajectory, null);
            TemplateMakingViewModel.This.SetModelMaskROIHandle += new EventHandler<ReturnData>(SetModelMaskROIEvent);
            _frmTemplateMaking.ShowDialog();

           
        }
        void SetModelMaskROIEvent(object sender, ReturnData e)
        {        
            if (e.type == EumMakeType.Trajectory)
            {
                if (e.region!=null)
                {
                    TrajectoryExtractParam par = baseTool.GetParam() as TrajectoryExtractParam;
                  
                    (par.TrajectoryTool.param as TrajectoryTypeAnyCurveParam).MaskRegion = e.region.Clone();
                    baseTool.SetParam(par);
                    if (trajectoryInspectObjDic.ContainsKey("AnyCurve:掩膜1"))
                        trajectoryInspectObjDic["AnyCurve:掩膜1"] = e.region.Clone();
                    else
                        trajectoryInspectObjDic.Add("AnyCurve:掩膜1", e.region.Clone());

                    ShowTool.DispRegion(e.region, "orange");
                    ShowTool.AddregionBuffer(e.region, "orange");

                }                 
            }

        }
        /// <summary>
        /// 任意轨迹识别
        /// </summary>
        bool OnAnyCurveTrajectoryIdentify()
        {
            BaseParam par = baseTool.GetParam();
            //检测工具
            TrajectoryTypeBaseTool TypeTool = (par as TrajectoryExtractParam).TrajectoryTool;
            //输入图像
            par.InputImg = dataManage.imageBufDic[(par as TrajectoryExtractParam).InputImageName];
            TypeTool.param.InputImage = par.InputImg;
            //检测区域
            if (!trajectoryInspectObjDic.ContainsKey("AnyCurve:任意1"))
            {
                MessageBox.Show("检测区域不存在");
                return false;
            }

            HObject inspectROI = trajectoryInspectObjDic["AnyCurve:任意1"];
            if (Model.UsePosiCorrectChecked)
            {
                //仿射变换矩阵
                HTuple matrix2D = dataManage.matrixBufDic[(par as TrajectoryExtractParam).MatrixName];
                if (matrix2D != null)
                    HOperatorSet.AffineTransContourXld(trajectoryInspectObjDic["AnyCurve:任意1"],
                      out inspectROI, matrix2D);
                else
                {
                    MessageBox.Show("检测区域位置补正异常");
                }
            }
            (TypeTool.param as TrajectoryTypeAnyCurveParam).InspectRegion = inspectROI.Clone();

            //掩膜区域
            HOperatorSet.GenEmptyObj(out HObject maskROI);
            if(trajectoryInspectObjDic.ContainsKey("AnyCurve:掩膜1"))
            {
                maskROI = trajectoryInspectObjDic["AnyCurve:掩膜1"];
                if (Model.UsePosiCorrectChecked)
                {
                    //仿射变换矩阵
                    HTuple matrix2D = dataManage.matrixBufDic[(par as TrajectoryExtractParam).MatrixName];
                    if (matrix2D != null)
                        HOperatorSet.AffineTransContourXld(trajectoryInspectObjDic["AnyCurve:掩膜1"],
                          out maskROI, matrix2D);
                    else
                    {
                        MessageBox.Show("检测区域位置补正异常");
                    }
                }
            }           
            (TypeTool.param as TrajectoryTypeAnyCurveParam).MaskRegion = maskROI.Clone();


            //子页面工具运行
            TemRunResult temrlt = TypeTool.Run();
            if (!temrlt.runFlag)
                MessageBox.Show(temrlt.info);
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(par.InputImg);
            //轮廓，卡尺区域等
            if (temrlt.runFlag)
            {
                ShowTool.DispRegion(temrlt.resultContour, "green");
                ShowTool.AddregionBuffer(temrlt.resultContour, "green");
            }
            //检测区域显示
            //if (BaseTool.ObjectValided(TypeTool.param.ResultInspectROI))
            //{
            //    ShowTool.DispRegion(TypeTool.param.ResultInspectROI, "blue");
            //    ShowTool.AddregionBuffer(TypeTool.param.ResultInspectROI, "blue");
            //}
            return true;
        }

        #endregion

        #region  CirclePage
        /// <summary>
        /// 绘制圆检测区域
        /// </summary>
        /// <returns></returns>
        HObject DrawCircleDetectRegion()
        {
            if (MessageBox.Show("准备创建圆检测区域？", "Information",
                      MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                TrajectoryExtractParam par = baseTool.GetParam() as TrajectoryExtractParam;

                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
                //Model.BtnDrawRegionEnable = false;
                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");


                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);
                //绘制区域
                FindCircleViewModel.draw_spoke(imgBuf,ShowTool.HWindowsHandle,
                     out HTuple hv_ROIRows,
                     out HTuple hv_ROICols, out HTuple direction);

                HOperatorSet.GenContourPolygonXld(out HObject inspectXLD, hv_ROIRows,
                    hv_ROICols);

                //搜索区域
                FindCircleViewModel.get_spoke_region(imgBuf, ShowTool.HWindowsHandle, out HObject inspectROI,
                  (par.TrajectoryTool.param as TrajectoryTypeCircleParam).CaliperNum,
                    (par.TrajectoryTool.param as TrajectoryTypeCircleParam).CaliperHeight,
                    (par.TrajectoryTool.param as TrajectoryTypeCircleParam).CaliperWidth,
                   hv_ROIRows, hv_ROICols, direction);
                string dir = direction.S;
                (par.TrajectoryTool.param as TrajectoryTypeCircleParam).Direction =
                           (EumDirection)Enum.Parse(typeof(EumDirection), dir);

                baseTool.SetParam(par);
                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(inspectROI, "blue");
                ShowTool.AddregionBuffer(inspectROI, "blue");

                ShowTool.AddRightMenu();
                //Model.BtnDrawRegionEnable = true;
                return inspectXLD;
            }
            else
                return null;

        }
      
        /// <summary>
        /// 圆识别
        /// </summary>
        bool OnCircleTrajectoryIdentify()
        {
            BaseParam par = baseTool.GetParam();
            //检测工具
            TrajectoryTypeBaseTool TypeTool = (par as TrajectoryExtractParam).TrajectoryTool;
            //输入图像
            par.InputImg = dataManage.imageBufDic[(par as TrajectoryExtractParam).InputImageName];
            TypeTool.param.InputImage = par.InputImg;
            //检测区域
            if (!trajectoryInspectObjDic.ContainsKey("Circle:圆1"))
            {
                MessageBox.Show("检测区域不存在");
                return false;
            }

            HObject xld = trajectoryInspectObjDic["Circle:圆1"];
            if (Model.UsePosiCorrectChecked)
            {
                //仿射变换矩阵
                HTuple matrix2D = dataManage.matrixBufDic[(par as TrajectoryExtractParam).MatrixName];
                if (matrix2D != null)
                    HOperatorSet.AffineTransContourXld(trajectoryInspectObjDic["Circle:圆1"],
                      out xld, matrix2D);
                else
                {
                    MessageBox.Show("检测区域位置补正异常");
                }
            }
            (TypeTool.param as TrajectoryTypeCircleParam).InspectXLD = xld.Clone();
            //子页面工具运行
            TemRunResult temrlt = TypeTool.Run();
            if (!temrlt.runFlag)
                MessageBox.Show(temrlt.info);
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(par.InputImg);
            //直线轮廓，卡尺区域等
            if (temrlt.runFlag)
            {
                ShowTool.DispRegion(temrlt.resultContour, "green");
                ShowTool.AddregionBuffer(temrlt.resultContour, "green");
            }
            //检测区域显示
            //if (BaseTool.ObjectValided(TypeTool.param.ResultInspectROI))
            //{
            //    ShowTool.DispRegion(TypeTool.param.ResultInspectROI, "blue");
            //    ShowTool.AddregionBuffer(TypeTool.param.ResultInspectROI, "blue");
            //}
            return true;
        }

        #endregion

        #region  LinePage
        /// <summary>
        /// 绘制直线检测区域
        /// </summary>
        /// <returns></returns>
        HObject DrawLineDetectRegion()
        {
            if (MessageBox.Show("准备创建直线检测区域？", "Information",
                      MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                TrajectoryExtractParam par = baseTool.GetParam() as TrajectoryExtractParam;

                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
                //Model.BtnDrawRegionEnable = false;
                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");


                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);
                //绘制区域
                FindLineViewModel.draw_rake(ShowTool.HWindowsHandle, out HTuple hv_Row12, out HTuple hv_Column12,
                                              out HTuple hv_Row22, out HTuple hv_Column22);

                HOperatorSet.GenContourPolygonXld(out HObject inspectXLD, hv_Row12.TupleConcat(hv_Row22),
                    hv_Column12.TupleConcat(hv_Column22));

                //搜索区域
                FindLineViewModel.get_rake_region(imgBuf, out HObject inspectROI,
                  (par.TrajectoryTool.param as TrajectoryTypeLineParam).CaliperNum,
                    (par.TrajectoryTool.param as TrajectoryTypeLineParam).CaliperHeight,
                    (par.TrajectoryTool.param as TrajectoryTypeLineParam).CaliperWidth,
                    hv_Row12, hv_Column12, hv_Row22,
                    hv_Column22);

                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(inspectROI, "blue");
                ShowTool.AddregionBuffer(inspectROI, "blue");

                ShowTool.AddRightMenu();
                //Model.BtnDrawRegionEnable = true;
                return inspectXLD;
            }
            else
                return null;
        
        }
       
        /// <summary>
        /// 直线识别
        /// </summary>
        bool OnLineTrajectoryIdentify()
        {
            BaseParam par = baseTool.GetParam();
            //检测工具
            TrajectoryTypeBaseTool TypeTool = (par as TrajectoryExtractParam).TrajectoryTool;
            //输入图像
            par.InputImg = dataManage.imageBufDic[(par as TrajectoryExtractParam).InputImageName];
            TypeTool.param.InputImage = par.InputImg;
            //检测区域
            if(!trajectoryInspectObjDic.ContainsKey("Line:直线1"))
            {
                MessageBox.Show("检测区域不存在");
                return false;
            }
               
            HObject xld = trajectoryInspectObjDic["Line:直线1"];
            if (Model.UsePosiCorrectChecked)
            {
                //仿射变换矩阵
                HTuple matrix2D = dataManage.matrixBufDic[(par as TrajectoryExtractParam).MatrixName];
                if (matrix2D != null)
                    HOperatorSet.AffineTransContourXld(trajectoryInspectObjDic["Line:直线1"],
                      out  xld, matrix2D);
                else
                {
                    MessageBox.Show("检测区域位置补正异常");
                }
            }          
            (TypeTool.param as TrajectoryTypeLineParam).InspectXLD = xld.Clone();
            //子页面工具运行
            TemRunResult temrlt = TypeTool.Run();
            if (!temrlt.runFlag)
                MessageBox.Show(temrlt.info);
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(par.InputImg);
            //直线轮廓，卡尺区域等
            if (temrlt.runFlag)
            {
                ShowTool.DispRegion(temrlt.resultContour, "green");
                ShowTool.AddregionBuffer(temrlt.resultContour, "green");              
            }
            //检测区域显示
            //if (BaseTool.ObjectValided(TypeTool.param.ResultInspectROI))
            //{
            //    ShowTool.DispRegion(TypeTool.param.ResultInspectROI, "blue");
            //    ShowTool.AddregionBuffer(TypeTool.param.ResultInspectROI, "blue");
            //}
            return true;
        }

        #endregion

        #region RRectanglePage

        /// <summary>
        /// 绘制直线检测区域
        /// </summary>
        /// <returns></returns>
        HObject DrawLineDetectRegion(TrajectoryTypeLineParam parm)
        {
            if (MessageBox.Show("准备创建直线检测区域？", "Information",
                      MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                //TrajectoryExtractParam par = baseTool.GetParam() as TrajectoryExtractParam;

                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
                //Model.BtnDrawRegionEnable = false;
                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");


                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);
                //绘制区域
                FindLineViewModel.draw_rake(ShowTool.HWindowsHandle, out HTuple hv_Row12, out HTuple hv_Column12,
                                              out HTuple hv_Row22, out HTuple hv_Column22);

                HOperatorSet.GenContourPolygonXld(out HObject inspectXLD, hv_Row12.TupleConcat(hv_Row22),
                    hv_Column12.TupleConcat(hv_Column22));

                //搜索区域
                FindLineViewModel.get_rake_region(imgBuf, out HObject inspectROI,
                 parm.CaliperNum,
                    parm.CaliperHeight,
                    parm.CaliperWidth,
                    hv_Row12, hv_Column12, hv_Row22,
                    hv_Column22);

                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(inspectROI, "blue");
                ShowTool.AddregionBuffer(inspectROI, "blue");

                ShowTool.AddRightMenu();
                //Model.BtnDrawRegionEnable = true;
                return inspectXLD;
            }
            else
                return null;

        }
        /// <summary>
        /// 旋转矩形识别
        /// </summary>
        /// <returns></returns>
        bool OnRRectangleTrajectoryIdentify()
        {
            BaseParam par = baseTool.GetParam();
            //检测工具
            TrajectoryTypeBaseTool TypeTool = (par as TrajectoryExtractParam).TrajectoryTool;
            //输入图像
            par.InputImg = dataManage.imageBufDic[(par as TrajectoryExtractParam).InputImageName];
            TypeTool.param.InputImage = par.InputImg;
            for (int i = 0; i < 4; i++)
            {
                //检测区域1
                if (!trajectoryInspectObjDic.ContainsKey("RRectangle:直线"+(i+1)))
                {
                    MessageBox.Show(string.Format("检测区域{0}不存在", "RRectangle:直线" + (i + 1)));
                    return false;
                }
                HObject xld = trajectoryInspectObjDic["RRectangle:直线" + (i + 1)];
                if (Model.UsePosiCorrectChecked)
                {
                    //仿射变换矩阵
                    HTuple matrix2D = dataManage.matrixBufDic[(par as TrajectoryExtractParam).MatrixName];
                    if (matrix2D != null)
                        HOperatorSet.AffineTransContourXld(trajectoryInspectObjDic["RRectangle:直线" + (i + 1)],
                          out xld, matrix2D);
                    else
                    {
                        MessageBox.Show("检测区域1位置补正异常");
                    }
                }
                (TypeTool.param as TrajectoryTypeRRectangleParam).GetParamFormIndex(i).InspectXLD = xld.Clone();

            }
            //子页面工具运行
            TemRunResult temrlt = TypeTool.Run();
            if (!temrlt.runFlag)
                MessageBox.Show(temrlt.info);
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(par.InputImg);
            //直线轮廓，卡尺区域等
            if (temrlt.runFlag)
            {
                ShowTool.DispRegion(temrlt.resultContour, "green");
                ShowTool.AddregionBuffer(temrlt.resultContour, "green");
            }
            //检测区域显示
            //if (BaseTool.ObjectValided(TypeTool.param.ResultInspectROI))
            //{
            //    ShowTool.DispRegion(TypeTool.param.ResultInspectROI, "blue");
            //    ShowTool.AddregionBuffer(TypeTool.param.ResultInspectROI, "blue");
            //}
            return true;
        }
        #endregion

        #region RectanglePage     
        /// <summary>
        /// 矩形识别
        /// </summary>
        /// <returns></returns>
        bool OnRectangleTrajectoryIdentify()
        {
            BaseParam par = baseTool.GetParam();
            //检测工具
            TrajectoryTypeBaseTool TypeTool = (par as TrajectoryExtractParam).TrajectoryTool;
            //输入图像
            par.InputImg = dataManage.imageBufDic[(par as TrajectoryExtractParam).InputImageName];
            TypeTool.param.InputImage = par.InputImg;
            for (int i = 0; i < 4; i++)
            {
                //检测区域1
                if (!trajectoryInspectObjDic.ContainsKey("Rectangle:直线" + (i + 1)))
                {
                    MessageBox.Show(string.Format("检测区域{0}不存在", "Rectangle:直线" + (i + 1)));
                    return false;
                }
                HObject xld = trajectoryInspectObjDic["Rectangle:直线" + (i + 1)];
                if (Model.UsePosiCorrectChecked)
                {
                    //仿射变换矩阵
                    HTuple matrix2D = dataManage.matrixBufDic[(par as TrajectoryExtractParam).MatrixName];
                    if (matrix2D != null)
                        HOperatorSet.AffineTransContourXld(trajectoryInspectObjDic["Rectangle:直线" + (i + 1)],
                          out xld, matrix2D);
                    else
                    {
                        MessageBox.Show("检测区域1位置补正异常");
                    }
                }
                (TypeTool.param as TrajectoryTypeRectangleParam).GetParamFormIndex(i).InspectXLD = xld.Clone();

            }
            //子页面工具运行
            TemRunResult temrlt = TypeTool.Run();
            if (!temrlt.runFlag)
                MessageBox.Show(temrlt.info);
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(par.InputImg);
            //直线轮廓，卡尺区域等
            if (temrlt.runFlag)
            {
                ShowTool.DispRegion(temrlt.resultContour, "green");
                ShowTool.AddregionBuffer(temrlt.resultContour, "green");
            }
            //检测区域显示
            //if (BaseTool.ObjectValided(TypeTool.param.ResultInspectROI))
            //{
            //    ShowTool.DispRegion(TypeTool.param.ResultInspectROI, "blue");
            //    ShowTool.AddregionBuffer(TypeTool.param.ResultInspectROI, "blue");
            //}
            return true;
        }
        #endregion

        /// <summary>
        /// 是否启用位置补正
        /// </summary>
        private void chxbUsePosiCorrect_CheckedChanged()
        {
            Model.MatrixEnable = Model.UsePosiCorrectChecked;

            if (Model.UsePosiCorrectChecked)
            {
                if (Model.SelectMatrixName != "")
                {
                    matrix2D = dataManage.matrixBufDic[Model.SelectMatrixName];
                    BaseParam par = baseTool.GetParam();
                    (par as TrajectoryExtractParam).MatrixName = Model.SelectMatrixName;

                }

            }

        }
        /// <summary>
        /// 参数保存
        /// </summary>
       private void  btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();       
            (par as TrajectoryExtractParam).UsePosiCorrect = Model.UsePosiCorrectChecked;
            (par as TrajectoryExtractParam).TrajectoryInspectObjDic =
                this.trajectoryInspectObjDic;
             OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
        }
        /// <summary>
        /// 测试
        /// </summary>
        private void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();          
            //外部工具
            RunResult rlt = baseTool.Run();

            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {

                ShowTool.DispConcatedObj((par as TrajectoryExtractParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as TrajectoryExtractParam).OutputImg, EumCommonColors.green);

                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                //更新结果表格数据
                UpdateResultView((par as TrajectoryExtractParam).TrajectoryDataPoints);
            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispMessage("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
            }
            ShowTool.DispRegion((par as TrajectoryExtractParam).TrajectoryTool.param.ResultInspectROI, "blue");
            ShowTool.AddregionBuffer((par as TrajectoryExtractParam).TrajectoryTool.param.ResultInspectROI, "blue");
        }
        /// <summary>
        /// 更新检测结果表格数据
        /// </summary>
        /// <param name="datas"></param>
        void UpdateResultView(List<DgTrajectoryData> datas)
        {
            Model.DgTrajectoryDataList.Clear();
            foreach (var s in datas)
            Model.DgTrajectoryDataList.Add(s);
        }

        /// <summary>
        /// 结果显示表格双击
        /// </summary>
        /// <param name="o"></param>
        private void dataGridViewEx1_DoubleClick(object o)
        {

            int index = Model.DgTrajectorySelectIndex;
            if (index < 0 || index > Model.DgTrajectoryDataList.Count) return;
            BaseParam par = baseTool.GetParam();
            List<DgTrajectoryData> points = (par as TrajectoryExtractParam).TrajectoryDataPoints;
            if (points == null || points.Count <= 0 ||
                index >= points.Count) return;
            int count = points.Count;

            for (int i = 0; i < count; i++)
            {
                bool transFlag = baseTool.Transformation_POINT_INV(points[i].X, points[i].Y,
                    out HTuple Cx, out HTuple Cy);//转像素

                HOperatorSet.GenCrossContourXld(out HObject cross, Cy.D,
                    Cx.D, 10, 0);
                ShowTool.DispRegion(cross, (i == index) ? "red" : "green");
                //ShowTool.AddregionBuffer(objectSelected, "blue");     
            }
        }
    }
}
