using ControlShareResources.Common;
using HalconDotNet;
using PositionToolsLib.参数;
using PositionToolsLib.工具;
using PositionToolsLib.窗体.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisionShowLib.UserControls;

namespace PositionToolsLib.窗体.ViewModels
{
    public class FindCircleViewModel : BaseViewModel
    {
        HObject inspectXLD = null;//圆检测轮廓
        HTuple matrix2D = null;
        HTuple direction = "outer";
        public static FindCircleViewModel This { get; set; }
        public FindCircleModel Model { get; set; }

        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase UsePosiCorrectCheckedCommand { get; set; }
        public CommandBase MatrixSelectionChangedCommand { get; set; }
        public CommandBase DrawRegionClickCommand { get; set; }

        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }


        public FindCircleViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new FindCircleModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
          
            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            UsePosiCorrectCheckedCommand = new CommandBase();
            UsePosiCorrectCheckedCommand.DoExecute = new Action<object>((o) => chxbUsePosiCorrect_CheckedChanged());
            UsePosiCorrectCheckedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            MatrixSelectionChangedCommand = new CommandBase();
            MatrixSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxMatrixList_SelectedIndexChanged(o));
            MatrixSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DrawRegionClickCommand = new CommandBase();
            DrawRegionClickCommand.DoExecute = new Action<object>((o) => btnDrawRegion_Click());
            DrawRegionClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ShowData();
            cobxImageList_SelectedIndexChanged(null);
            cobxMatrixList_SelectedIndexChanged(null);
        }

        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData()
        {
            BaseParam par = baseTool.GetParam();

            //检测区域
            if (BaseTool.ObjectValided((par as FindCircleParam).InspectXLD))
                HOperatorSet.CopyObj((par as FindCircleParam).InspectXLD, out inspectXLD, 1, -1);
           
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as FindCircleParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;

            foreach (var s in dataManage.matrixBufDic)
                Model.MatrixList.Add(s.Key);
            string matrixName = (par as FindCircleParam).MatrixName;
            int index2 = Model.MatrixList.IndexOf(matrixName);
            Model.SelectMatrixIndex = index2;

            Model.SelectImageName = (par as FindCircleParam).InputImageName;
            Model.SelectMatrixName=(par as FindCircleParam).MatrixName  ;
            Model.NumEdgeThd = (par as FindCircleParam).EdgeThd;
            Model.NumCaliperCount = (par as FindCircleParam).CaliperNum;
            Model.NumCaliperWidth = (par as FindCircleParam).CaliperWidth;
            Model.NumCaliperHeight = (par as FindCircleParam).CaliperHeight;
            Model.SelectTransitionIndex = (int)(par as FindCircleParam).Transition;
            Model.SelectEdgeIndex= (int)(par as FindCircleParam).Select;
            Model.UsePosiCorrectChecked = (par as FindCircleParam).UsePosiCorrect;
            if ((par as FindCircleParam).UsePosiCorrect)
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
            if (!DilationTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as FindCircleParam).InputImageName = Model.SelectImageName;
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
            (par as FindCircleParam).MatrixName = Model.SelectMatrixName;
        }
        private void chxbUsePosiCorrect_CheckedChanged()
        {
            Model.MatrixEnable = Model.UsePosiCorrectChecked;

            if (Model.UsePosiCorrectChecked)
            {
                if (Model.SelectMatrixName != "")
                {
                    matrix2D = dataManage.matrixBufDic[Model.SelectMatrixName];
                    BaseParam par = baseTool.GetParam();
                    (par as FindCircleParam).MatrixName = Model.SelectMatrixName;

                }

            }


        }

        /// <summary>
        /// 绘制检测区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrawRegion_Click()
        {
            if (!MatchTool.ObjectValided(imgBuf))
            {
                ShowTool.DispAlarmMessage("未获取图像", 500, 20, 30);
                return;
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.DesignModel);
            ShowTool.setMouseStateOfNone();

            if (MessageBox.Show("准备创建圆检测区域？", "Information",
                       MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
                Model.BtnDrawRegionEnable = false;
                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");


                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);
                //绘制区域
                draw_spoke(imgBuf, ShowTool.HWindowsHandle, out HTuple hv_ROIRows,
                     out HTuple hv_ROICols, out direction);



                HOperatorSet.GenContourPolygonXld(out inspectXLD, hv_ROIRows,
                    hv_ROICols);

                //搜索区域
                get_spoke_region(imgBuf, ShowTool.HWindowsHandle, out HObject inspectROI, Model.NumCaliperCount,
                     Model.NumCaliperHeight, Model.NumCaliperWidth,
                    hv_ROIRows, hv_ROICols, direction);


                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(inspectROI, "blue");
                ShowTool.AddregionBuffer(inspectROI, "blue");

                ShowTool.AddRightMenu();
                Model.BtnDrawRegionEnable = true;
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.RunningModel);
        }

        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as FindCircleParam).EdgeThd = Model.NumEdgeThd;
            (par as FindCircleParam).CaliperNum = Model.NumCaliperCount;
            (par as FindCircleParam).CaliperWidth = Model.NumCaliperWidth;
            (par as FindCircleParam).CaliperHeight = Model.NumCaliperHeight;
            (par as FindCircleParam).Transition = (EumTransition)Model.SelectTransitionIndex;
            (par as FindCircleParam).Select = (EumSelect)Model.SelectEdgeIndex;
            (par as FindCircleParam).Direction = (EumDirection)Enum.Parse(typeof(EumDirection), this.direction);
            (par as FindCircleParam).InspectXLD = inspectXLD.Clone();
            (par as FindCircleParam).UsePosiCorrect = Model.UsePosiCorrectChecked;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
        }

        /// <summary>
        /// 图像结果显示测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {

            BaseParam par = baseTool.GetParam();
            (par as FindCircleParam).EdgeThd = Model.NumEdgeThd;
            (par as FindCircleParam).CaliperNum = Model.NumCaliperCount;
            (par as FindCircleParam).CaliperWidth = Model.NumCaliperWidth;
            (par as FindCircleParam).CaliperHeight = Model.NumCaliperHeight;
            (par as FindCircleParam).Transition = (EumTransition)Model.SelectTransitionIndex;
            (par as FindCircleParam).Select = (EumSelect)Model.SelectEdgeIndex;
            (par as FindCircleParam).Direction = (EumDirection)Enum.Parse(typeof(EumDirection), this.direction);
            (par as FindCircleParam).InspectXLD = inspectXLD.Clone();
            (par as FindCircleParam).UsePosiCorrect = Model.UsePosiCorrectChecked;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {

                ShowTool.DispConcatedObj((par as FindCircleParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as FindCircleParam).OutputImg, EumCommonColors.green);

                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                //更新结果表格数据
                UpdateResultView(new CircleResultData(1,
                 (par as FindCircleParam).Column, (par as FindCircleParam).Row, 
                    (par as FindCircleParam).Radius));
            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispMessage("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
            }
            ShowTool.DispRegion((par as FindCircleParam).ResultInspectROI, "blue");
            ShowTool.AddregionBuffer((par as FindCircleParam).ResultInspectROI, "blue");
        }
        /// <summary>
        /// 更新圆检测结果表格数据
        /// </summary>
        /// <param name="CircleResultData"></param>
        void UpdateResultView(CircleResultData data)
        {
            Model.DgResultOfCircleList.Clear();
            Model.DgResultOfCircleList.Add(data);
        }
        /*---------------------------------外部方法导入---------------------------*/
        #region ----External procedures----
        static public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
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
        public void draw_spoke(HObject ho_Image, HTuple hv_WindowHandle, out HTuple hv_ROIRows,
           out HTuple hv_ROICols, out HTuple hv_Direct)
        {

            // Local iconic variables 

            HObject ho_Regions, ho_ContOut1, ho_Contour;

            // Local control variables 

            HTuple hv_Rows = null, hv_Cols = null, hv_Weights = null;
            HTuple hv_Length1 = null, hv_RowC = null, hv_ColumnC = null;
            HTuple hv_Radius = null, hv_StartPhi = null, hv_EndPhi = null;
            HTuple hv_PointOrder = null, hv_Row1 = null, hv_Column1 = null;
            HTuple hv_Row2 = null, hv_Column2 = null, hv_DistanceStart = null;
            HTuple hv_DistanceEnd = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ContOut1);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            hv_ROIRows = new HTuple();
            hv_ROICols = new HTuple();
            hv_Direct = new HTuple();
            //提示
            if (hv_WindowHandle.D > 100000)
                HOperatorSet.SetFont(hv_WindowHandle, "Arial-" + "12");
            else
                HOperatorSet.SetFont(hv_WindowHandle, "-Arial-" + "12" + "-*-*-*-*-1-");
            //提示      
            disp_message(hv_WindowHandle, new HTuple("1、画4个以上点确定一个圆弧,点击右键确认"),
                "window", 12, 12, "red", "false");
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            //沿着圆弧或圆的边缘画点
            ho_ContOut1.Dispose();
            HOperatorSet.DrawNurbs(out ho_ContOut1, hv_WindowHandle, "true", "true", "true",
                "true", 3, out hv_Rows, out hv_Cols, out hv_Weights);
            //至少要4个点
            HOperatorSet.TupleLength(hv_Weights, out hv_Length1);
            if ((int)(new HTuple(hv_Length1.TupleLess(4))) != 0)
            {
                //提示
                if (hv_WindowHandle.D > 100000)
                    HOperatorSet.SetFont(hv_WindowHandle, "Arial-" + "12");
                else
                    HOperatorSet.SetFont(hv_WindowHandle, "-Arial-" + "12" + "-*-*-*-*-1-");
                disp_message(hv_WindowHandle, "提示：点数太少，请重画", "window", 32, 12, "red",
                    "false");
                hv_ROIRows = new HTuple();
                hv_ROICols = new HTuple();
                ho_Regions.Dispose();
                ho_ContOut1.Dispose();
                ho_Contour.Dispose();

                return;
            }
            //获取点
            hv_ROIRows = hv_Rows.Clone();
            hv_ROICols = hv_Cols.Clone();
            //产生xld
            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);
            //用回归线法（不抛出异常点，所有点权重一样）拟合圆
            HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 1, 2, out hv_RowC,
                out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
            //根据拟合结果产生xld，并保持到显示对象
            //gen_circle_contour_xld (ContCircle, RowC, ColumnC, Radius, StartPhi, EndPhi, PointOrder, 3)
            //concat_obj (Regions, ContCircle, Regions)

            //获取圆或圆弧xld上的点坐标
            //get_contour_xld (ContCircle, RowXLD, ColXLD)
            //显示图像和圆弧
            HOperatorSet.DispObj(ho_Image, hv_WindowHandle);
            HOperatorSet.DispObj(ho_Contour, hv_WindowHandle);

            //产生并显示圆心
            //gen_cross_contour_xld (Cross, RowC, ColumnC, 60, 0.785398)

            //提示
            if (hv_WindowHandle.D > 100000)
                HOperatorSet.SetFont(hv_WindowHandle, "Arial-" + "12");
            else
                HOperatorSet.SetFont(hv_WindowHandle, "-Arial-" + "12" + "-*-*-*-*-1-");
            //提示
            disp_message(hv_WindowHandle, "2、远离圆心，画箭头确定边缘检测方向，点击右键确认",
                "window", 12, 12, "red", "false");
            //画线，确定检测方向
            HOperatorSet.DrawLine(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
                out hv_Column2);
            //求圆心到检测方向直线起点的距离
            HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row1, hv_Column1, out hv_DistanceStart);
            //求圆心到检测方向直线终点的距离
            HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row2, hv_Column2, out hv_DistanceEnd);
            if ((int)(new HTuple(hv_DistanceStart.TupleGreater(hv_DistanceEnd))) != 0)
            {
                //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                hv_Direct = "inner";
            }
            else
            {
                //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                hv_Direct = "outer";
            }
            //求圆或圆弧xld上的点的数量
            //tuple_length (ColXLD, Length2)
            //判断检测的边缘数量是否过少
            //if (Elements<3)
            //ROIRows := []
            //ROICols := []
            //disp_message (WindowHandle, '检测的边缘数量太少，请重新设置!', 'window', 52, 12, 'red', 'false')
            //return ()
            //endif
            //如果xld是圆弧，有Length2个点，从起点开始，等间距（间距为Length2/(Elements-1)）取Elements个点，作为卡尺工具的中点
            //如果xld是圆，有Length2个点，以0°为起点，从起点开始，等间距（间距为Length2/(Elements)）取Elements个点，作为卡尺工具的中点
            //for i := 0 to Elements-1 by 1

            //if (RowXLD[0]=RowXLD[Length2-1])
            //xld的起点和终点坐标相对，为圆
            //tuple_int (1.0*Length2/(Elements)*(i), j)

            //else
            //否则为圆弧
            //tuple_int (1.0*Length2/(Elements-1)*(i), j)
            //endif
            //索引越界，强制赋值为最后一个索引
            //if (j>=Length2)
            //j := Length2-1
            //continue
            //endif
            //获取卡尺工具中心
            //RowE := RowXLD[j]
            //ColE := ColXLD[j]

            //如果圆心到检测方向直线的起点的距离大于圆心到检测方向直线的终点的距离，搜索方向由圆外指向圆心
            //如果圆心到检测方向直线的起点的距离不大于圆心到检测方向直线的终点的距离，搜索方向由圆心指向圆外
            //if (DistanceStart>DistanceEnd)
            //求卡尺工具的边缘搜索方向
            //求圆心指向边缘的矢量的角度
            //tuple_atan2 (-RowE+RowC, ColE-ColumnC, ATan)
            //角度反向
            //ATan := rad(180)+ATan
            //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
            //Direct := 'inner'
            //else
            //求卡尺工具的边缘搜索方向
            //求圆心指向边缘的矢量的角度
            //tuple_atan2 (-RowE+RowC, ColE-ColumnC, ATan)
            //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
            //Direct := 'outer'
            //endif

            //产生卡尺xld，并保持到显示对象
            //gen_rectangle2_contour_xld (Rectangle1, RowE, ColE, ATan, DetectHeight/2, DetectWidth/2)
            //concat_obj (Regions, Rectangle1, Regions)

            //用箭头xld指示边缘搜索方向，并保持到显示对象
            //if (i=0)
            //RowL2 := RowE+DetectHeight/2*sin(-ATan)
            //RowL1 := RowE-DetectHeight/2*sin(-ATan)
            //ColL2 := ColE+DetectHeight/2*cos(-ATan)
            //ColL1 := ColE-DetectHeight/2*cos(-ATan)
            //gen_arrow_contour_xld (Arrow1, RowL1, ColL1, RowL2, ColL2, DetectHeight, 5)
            //concat_obj (Regions, Arrow1, Regions)
            //endif
            //endfor

            ho_Regions.Dispose();
            ho_ContOut1.Dispose();
            ho_Contour.Dispose();

            return;
        }

        public void get_spoke_region(HObject ho_Image, HTuple hv_WindowHandle, out HObject ho_Regions, HTuple hv_Elements,
          HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_ROIRows, HTuple hv_ROICols,
          HTuple hv_Direct)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Contour, ho_ContCircle, ho_Rectangle1 = null;
            HObject ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_RowC = null;
            HTuple hv_ColumnC = null, hv_Radius = null, hv_StartPhi = null;
            HTuple hv_EndPhi = null, hv_PointOrder = null, hv_RowXLD = null;
            HTuple hv_ColXLD = null, hv_Length2 = null, hv_i = null;
            HTuple hv_j = new HTuple(), hv_ArcType = new HTuple();
            HTuple hv_RowE = new HTuple(), hv_ColE = new HTuple();
            HTuple hv_ATan = new HTuple(), hv_RowL2 = new HTuple();
            HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);

            //产生xld
            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);
            //用回归线法（不抛出异常点，所有点权重一样）拟合圆
            HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 1, 2, out hv_RowC,
                out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
            //根据拟合结果产生xld，并保持到显示对象
            ho_ContCircle.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
                hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
            //concat_obj (Regions, ContCircle, Regions)

            //获取圆或圆弧xld上的点坐标
            HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);

            //求圆或圆弧xld上的点的数量
            HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
            if ((int)(new HTuple(hv_Elements.TupleLess(3))) != 0)
            {
                //提示
                if (hv_WindowHandle.D > 100000)
                    HOperatorSet.SetFont(hv_WindowHandle, "Arial-" + "12");
                else
                    HOperatorSet.SetFont(hv_WindowHandle, "-Arial-" + "12" + "-*-*-*-*-1-");
                disp_message(hv_WindowHandle, "检测的边缘数量太少，请重新设置!", "window", 52, 12, "red",
                    "false");
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            //如果xld是圆弧，有Length2个点，从起点开始，等间距（间距为Length2/(Elements-1)）取Elements个点，作为卡尺工具的中点
            //如果xld是圆，有Length2个点，以0°为起点，从起点开始，等间距（间距为Length2/(Elements)）取Elements个点，作为卡尺工具的中点
            HTuple end_val24 = hv_Elements - 1;
            HTuple step_val24 = 1;
            for (hv_i = 0; hv_i.Continue(end_val24, step_val24); hv_i = hv_i.TupleAdd(step_val24))
            {

                if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                    hv_Length2 - 1)))) != 0)
                {
                    //xld的起点和终点坐标相对，为圆
                    HOperatorSet.TupleInt(((1.0 * hv_Length2) / hv_Elements) * hv_i, out hv_j);
                    hv_ArcType = "circle";
                }
                else
                {
                    //否则为圆弧
                    HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                    hv_ArcType = "arc";
                }
                //索引越界，强制赋值为最后一个索引
                if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                {
                    hv_j = hv_Length2 - 1;
                    //continue
                }
                //获取卡尺工具中心
                hv_RowE = hv_RowXLD.TupleSelect(hv_j);
                hv_ColE = hv_ColXLD.TupleSelect(hv_j);

                //超出图像区域，不检测，否则容易报异常
                if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(hv_Height - 1))).TupleOr(
                    new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
                    hv_Width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
                {
                    continue;
                }
                //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                if ((int)(new HTuple(hv_Direct.TupleEqual("inner"))) != 0)
                {
                    //求卡尺工具的边缘搜索方向
                    //求圆心指向边缘的矢量的角度
                    HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                    //角度反向
                    hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;
                }
                else
                {
                    //求卡尺工具的边缘搜索方向
                    //求圆心指向边缘的矢量的角度
                    HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                }


                //产生卡尺xld，并保持到显示对象
                ho_Rectangle1.Dispose();
                HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle1, hv_RowE, hv_ColE, hv_ATan,
                    hv_DetectHeight / 2, hv_DetectWidth / 2);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle1, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }
                //用箭头xld指示边缘搜索方向，并保持到显示对象
                if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
                {
                    hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                    hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                    ho_Arrow1.Dispose();
                    gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                        5, 5);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }
                }

            }

            ho_Contour.Dispose();
            ho_ContCircle.Dispose();
            ho_Rectangle1.Dispose();
            ho_Arrow1.Dispose();

            return;
        }

        public void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
          HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TempArrow = null;

            // Local control variables 

            HTuple hv_Length = null, hv_ZeroLengthIndices = null;
            HTuple hv_DR = null, hv_DC = null, hv_HalfHeadWidth = null;
            HTuple hv_RowP1 = null, hv_ColP1 = null, hv_RowP2 = null;
            HTuple hv_ColP2 = null, hv_Index = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);
            //This procedure generates arrow shaped XLD contours,
            //pointing from (Row1, Column1) to (Row2, Column2).
            //If starting and end point are identical, a contour consisting
            //of a single point is returned.
            //
            //input parameteres:
            //Row1, Column1: Coordinates of the arrows' starting points
            //Row2, Column2: Coordinates of the arrows' end points
            //HeadLength, HeadWidth: Size of the arrow heads in pixels
            //
            //output parameter:
            //Arrow: The resulting XLD contour
            //
            //The input tuples Row1, Column1, Row2, and Column2 have to be of
            //the same length.
            //HeadLength and HeadWidth either have to be of the same length as
            //Row1, Column1, Row2, and Column2 or have to be a single element.
            //If one of the above restrictions is violated, an error will occur.
            //
            //
            //Init
            ho_Arrow.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            //
            //Calculate the arrow length
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
            //
            //Mark arrows with identical start and end point
            //(set Length to -1 to avoid division-by-zero exception)
            hv_ZeroLengthIndices = hv_Length.TupleFind(0);
            if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
            {
                if (hv_Length == null)
                    hv_Length = new HTuple();
                hv_Length[hv_ZeroLengthIndices] = -1;
            }
            //
            //Calculate auxiliary variables.
            hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
            hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
            hv_HalfHeadWidth = hv_HeadWidth / 2.0;
            //
            //Calculate end points of the arrow head.
            hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
            hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
            hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
            hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
            //
            //Finally create output XLD contour for each input point pair
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                {
                    //Create_ single points for arrows with identical start and end point
                    ho_TempArrow.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(hv_Index),
                        hv_Column1.TupleSelect(hv_Index));
                }
                else
                {
                    //Create arrow contour
                    ho_TempArrow.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                        hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                        hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                        hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                        ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                        hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                        hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                        hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                    ho_Arrow.Dispose();
                    ho_Arrow = ExpTmpOutVar_0;
                }
            }
            ho_TempArrow.Dispose();

            return;
        }

        #endregion
    }
}
