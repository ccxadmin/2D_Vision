using ControlShareResources.Common;
using PositionToolsLib.参数;
using PositionToolsLib.工具;
using PositionToolsLib.窗体.Models;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using VisionShowLib.UserControls;


namespace PositionToolsLib.窗体.ViewModels
{
    public class GlueCaliperWidthViewModel : BaseViewModel
    {
        public static GlueCaliperWidthViewModel This { get; set; }
        public GlueCaliperWidthModel Model { get; set; }

        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }
        //获取像素转换比
        public CommandBase GetPixelRatioButClickCommand { get; set; }
        // 矩阵旋转
        public CommandBase MatrixSelectionChangedCommand { get; set; }
        // 绘制卡尺区域
        public CommandBase DrawRegionButClickCommand { get; set; }
        //新增
        public CommandBase 新增toolStripMenuItemMenuClickCommand { get; set; }
        //删除
        public CommandBase 删除toolStripMenuItemMenuClickCommand { get; set; }
        //表格双击
        public CommandBase DgMouseDoubleClickCommand { get; set; }
        //单元格编辑
        public CommandBase DgCurrentCellChangedCommand { get; set; }
        public CommandBase UsePosiCorrectCheckedChangedCommand { get; set; }
      

        List<StuRegionBuf> regionBufList = null;//绘制卡尺缓存     
        //HObject imgBuf = null;//图像缓存
        HObject glueCaliperRegion = null;//卡尺区域     
        HTuple matrix2D = null;
        string currRegionName = string.Empty;//当前设置的卡尺区域名称
        int selectIndex = -1;
        //DataManage dataManage = null;
        EumDrawRegionType drawRegionType = EumDrawRegionType.any;


        public GlueCaliperWidthViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new GlueCaliperWidthModel();
          
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
          
       
            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            MatrixSelectionChangedCommand = new CommandBase();
            MatrixSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxMatrixList_SelectedIndexChanged(o));
            MatrixSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            GetPixelRatioButClickCommand = new CommandBase();
            GetPixelRatioButClickCommand.DoExecute = new Action<object>((o) => btnGetPixelRatio_Click());
            GetPixelRatioButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DrawRegionButClickCommand = new CommandBase();
            DrawRegionButClickCommand.DoExecute = new Action<object>((o) => btnDrawRegion_Click());
            DrawRegionButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            新增toolStripMenuItemMenuClickCommand = new CommandBase();
            新增toolStripMenuItemMenuClickCommand.DoExecute = new Action<object>((o) => 新增toolStripMenuItem_Click());
            新增toolStripMenuItemMenuClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            删除toolStripMenuItemMenuClickCommand = new CommandBase();
            删除toolStripMenuItemMenuClickCommand.DoExecute = new Action<object>((o) => 删除toolStripMenuItem_Click());
            删除toolStripMenuItemMenuClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DgMouseDoubleClickCommand = new CommandBase();
            DgMouseDoubleClickCommand.DoExecute = new Action<object>((o) => dataGridViewEx1_DoubleClick(o));
            DgMouseDoubleClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
          
            DgCurrentCellChangedCommand = new CommandBase();
            DgCurrentCellChangedCommand.DoExecute = new Action<object>((o) => dataGridViewEx1_CellValueChanged(o));
            DgCurrentCellChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            UsePosiCorrectCheckedChangedCommand = new CommandBase();
            UsePosiCorrectCheckedChangedCommand.DoExecute = new Action<object>((o) => chxbUsePosiCorrect_CheckedChanged());
            UsePosiCorrectCheckedChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            HOperatorSet.GenEmptyObj(out glueCaliperRegion);

            ShowData();
            cobxImageList_SelectedIndexChanged(null);
        }

        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData()
        {
            BaseParam par = baseTool.GetParam();
            this.regionBufList = (par as GlueCaliperWidthParam).RegionBufList;
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as GlueCaliperWidthParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;


            foreach (var s in dataManage.matrixBufDic)
                Model.MatrixList.Add(s.Key);
            string matrixName = (par as GlueCaliperWidthParam).MatrixName;
            int index2 = Model.MatrixList.IndexOf(matrixName);
            Model.SelectMatrixIndex = index2;

            Model.SelectImageName = (par as GlueCaliperWidthParam).InputImageName;
            Model.PixelRatio = (par as GlueCaliperWidthParam).PixleRatio.ToString();
            Model.CobxMatrixListEnable = Model.UsePosiCorrect = (par as GlueCaliperWidthParam).UsePosiCorrect;
            Model.CaliperHeight = (par as GlueCaliperWidthParam).CaliperHeight;
            Model.CaliperEdgeThd = (par as GlueCaliperWidthParam).CaliperEdgeThd;
            Model.DistanceMin = (par as GlueCaliperWidthParam).DistanceMin;
            Model.DistanceMax = (par as GlueCaliperWidthParam).DistanceMax;

            if (regionBufList == null) return;
        
            Model.DgDataOfGlueCaliperWidthList.Clear();
            foreach (var s in regionBufList)
            {
               
                DgDataOfGlueCaliperWidth dat = new DgDataOfGlueCaliperWidth(s.isUse, s.regionName, s.status);
                Model.DgDataOfGlueCaliperWidthList.Add(dat);
            }
            if (Model.DgDataOfGlueCaliperWidthList.Count > 0)
                Model.DgDataSelectIndex = Model.DgDataOfGlueCaliperWidthList.Count - 1;

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
            if (!GlueCaliperWidthTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as GlueCaliperWidthParam).InputImageName = Model.SelectImageName;
        }

        /// <summary>
        ///输入矩阵旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxMatrixList_SelectedIndexChanged(object value)
        {
            matrix2D = dataManage.matrixBufDic[Model.SelectMatrixName];
            BaseParam par = baseTool.GetParam();
            (par as GlueCaliperWidthParam).MatrixName = Model.SelectMatrixName;
        }

        /// <summary>
        /// 获取像素转换比
        /// </summary>
        /// <param name="value"></param>
        private void btnGetPixelRatio_Click()
        {
            if (getPixelRatioHandle != null)
            {
                double tem = getPixelRatioHandle.Invoke();
                if (tem <= 0)
                    tem = 1;
                Model.PixelRatio = tem.ToString();
            }
        }

        /// <summary>
        /// 图像闭运算测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as GlueCaliperWidthParam).PixleRatio = double.Parse(Model.PixelRatio);
            (par as GlueCaliperWidthParam).UsePosiCorrect = Model.UsePosiCorrect;
            (par as GlueCaliperWidthParam).CaliperHeight = Model.CaliperHeight;
            (par as GlueCaliperWidthParam).CaliperEdgeThd = Model.CaliperEdgeThd;
            (par as GlueCaliperWidthParam).DistanceMin = Model.DistanceMin;
            (par as GlueCaliperWidthParam).DistanceMax = Model.DistanceMax;
            int count = Model.DgDataOfGlueCaliperWidthList.Count;
            for (int i = 0; i < count; i++)
            {
                bool isCheck = Model.DgDataOfGlueCaliperWidthList[i].Use;
                StuRegionBuf stuRegion = regionBufList[i];
                stuRegion.isUse = isCheck;
                stuRegion.regionName = Model.DgDataOfGlueCaliperWidthList[i].CaliperName;
                regionBufList[i] = stuRegion;

            }
            (par as GlueCaliperWidthParam).RegionBufList = this.regionBufList;

            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            //运行状态
            if (rlt.runFlag)
            {
                ShowTool.DispConcatedObj((par as GlueCaliperWidthParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as GlueCaliperWidthParam).OutputImg, EumCommonColors.green);
                ShowTool.DispRegion((par as GlueCaliperWidthParam).ResultInspectRegions, "blue");
                ShowTool.AddregionBuffer((par as GlueCaliperWidthParam).ResultInspectRegions, "blue");
                //判定结果
                string toolName = baseTool.GetToolName();
                if (dataManage.resultFlagDic.ContainsKey(toolName))
                {
                    if (dataManage.resultFlagDic[baseTool.GetToolName()])
                    {
                        ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                        ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                    }
                    else
                    {
                        ShowTool.DispMessage("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                        ShowTool.AddTextBuffer("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                    }
                }
                else
                {
                    ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                    ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                }

                int num = (par as GlueCaliperWidthParam).DistanceList.Count;
                for (int i = 0; i <= num / 3 - 1; i++)
                {
                    double distance = (par as GlueCaliperWidthParam).DistanceList[3 * i + 2];
                    double row = (par as GlueCaliperWidthParam).DistanceList[3 * i + 0];
                    double col = (par as GlueCaliperWidthParam).DistanceList[3 * i + 1];
                    ShowTool.DispMessage(distance.ToString("f3"), row, col, "green", 16);
                    ShowTool.AddTextBuffer(distance.ToString("f3"), row, col, "green", 16);
                }

            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispRegion((par as GlueCaliperWidthParam).ResultInspectRegions, "blue");
                ShowTool.AddregionBuffer((par as GlueCaliperWidthParam).ResultInspectRegions, "blue");
                ShowTool.DispMessage("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
            }
        }
        /// <summary>
        /// 图像闭运算参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as GlueCaliperWidthParam).PixleRatio = double.Parse(Model.PixelRatio);
            (par as GlueCaliperWidthParam).UsePosiCorrect = Model.UsePosiCorrect;
            (par as GlueCaliperWidthParam).CaliperHeight = Model.CaliperHeight;
            (par as GlueCaliperWidthParam).CaliperEdgeThd = Model.CaliperEdgeThd;
            (par as GlueCaliperWidthParam).DistanceMin = Model.DistanceMin;
            (par as GlueCaliperWidthParam).DistanceMax = Model.DistanceMax;
            int count = Model.DgDataOfGlueCaliperWidthList.Count;
            for (int i = 0; i < count; i++)
            {
                bool isCheck = Model.DgDataOfGlueCaliperWidthList[i].Use;
                StuRegionBuf stuRegion = regionBufList[i];
                stuRegion.isUse = isCheck;
                stuRegion.regionName = Model.DgDataOfGlueCaliperWidthList[i].CaliperName;
                regionBufList[i] = stuRegion;

            }

            (par as GlueCaliperWidthParam).RegionBufList = this.regionBufList;

            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
        }

        /// <summary>
        /// 同名校验
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exceptIndex"></param>
        /// <returns></returns>
        bool SameCheck(string name, int exceptIndex = -1)
        {
            int count = Model.DgDataOfGlueCaliperWidthList.Count;

            for (int i = 0; i < count; i++)
            {
                if (i == exceptIndex)
                    continue;
                string curName = Model.DgDataOfGlueCaliperWidthList[i].CaliperName;
                if (curName == name)
                    return false;
            }
            return true;

        }
        /// <summary>
        /// 手动绘制卡尺区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrawRegion_Click()
        {
            currRegionName = "";
            if (Model.DgDataSelectIndex>= 0
                && Model.DgDataSelectIndex< Model.DgDataOfGlueCaliperWidthList.Count)
            {
                selectIndex = Model.DgDataSelectIndex;              
                currRegionName = Model.DgDataOfGlueCaliperWidthList[Model.DgDataSelectIndex].CaliperName;
               
            }
            if (!GlueCaliperWidthTool.ObjectValided(imgBuf))
            {
                ShowTool.DispAlarmMessage("未获取图像", 500, 20, 30);
                return;
            }
            if (currRegionName == "")
            {
                ShowTool.DispAlarmMessage("未选择需要设置的卡尺名称", 500, 10, 20);
                return;
            }
            if (!SameCheck(currRegionName, selectIndex))
            {
                ShowTool.DispAlarmMessage("存在相同卡尺名称，请修改再绘制区域！", 500, 10, 20);
                return;
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.DesignModel);
            ShowTool.setMouseStateOfNone();

            if (MessageBox.Show("准备手动绘制卡尺区域？", "Information",
                       MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
             
                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");
                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);

                //绘制区域
                draw_rake(ShowTool.HWindowsHandle, out HTuple hv_Row12, out HTuple hv_Column12,
                                              out HTuple hv_Row22, out HTuple hv_Column22);

                HOperatorSet.GenContourPolygonXld(out HObject Contour, hv_Row12.TupleConcat(hv_Row22),
                    hv_Column12.TupleConcat(hv_Column22));

                int index = regionBufList.FindIndex(t => t.regionName == currRegionName);
                if (index >= 0 && index < regionBufList.Count)
                {
                    StuRegionBuf stuRegion = regionBufList[index];
                    stuRegion.region = Contour.Clone();
                    regionBufList[index] = stuRegion;
                    if (selectIndex >= 0 &&
                          selectIndex < Model.DgDataOfGlueCaliperWidthList.Count)
                        Model.DgDataOfGlueCaliperWidthList[selectIndex].ToolStatus = "已修改";
                }
                else
                {
                    StuRegionBuf stuRegion = new StuRegionBuf
                    {
                        isUse = true,
                        regionName = currRegionName,
                        region = Contour.Clone()
                    };
                    regionBufList.Add(stuRegion);
                    if (selectIndex >= 0 &&
                        selectIndex < Model.DgDataOfGlueCaliperWidthList.Count)
                        Model.DgDataOfGlueCaliperWidthList[selectIndex].ToolStatus = "已设置";
                }

          
                //搜索区域
                get_rake2_region(imgBuf, out glueCaliperRegion, Model.CaliperHeight, hv_Row12, hv_Column12, hv_Row22,
                    hv_Column22);


                //HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(glueCaliperRegion, "blue");
                ShowTool.AddregionBuffer(glueCaliperRegion, "blue");

                ShowTool.AddRightMenu();

              
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.RunningModel);
        }

        private void chxbUsePosiCorrect_CheckedChanged()
        {
            Model.CobxMatrixListEnable = Model.UsePosiCorrect;

            if (Model.UsePosiCorrect)
            {
                if (Model.SelectMatrixName != "")
                {
                    matrix2D = dataManage.matrixBufDic[Model.SelectMatrixName];
                    BaseParam par = baseTool.GetParam();
                    (par as GlueCaliperWidthParam).MatrixName = Model.SelectMatrixName;
                }

            }
        }
        private void 新增toolStripMenuItem_Click()
        {
            //新增时就更新表格          
            StuRegionBuf dat = new StuRegionBuf
            {
                isUse = true,
                regionName = "卡尺区域1",
                status = "未设置"
            };
            regionBufList.Add(dat);
           
            DgDataOfGlueCaliperWidth dg = new DgDataOfGlueCaliperWidth(dat.isUse,
                dat.regionName, dat.status);
            Model.DgDataOfGlueCaliperWidthList.Add(dg);

            int count = Model.DgDataOfGlueCaliperWidthList.Count;
            if(count>0)
                Model.DgDataSelectIndex= count-1;
          
        }

        private void 删除toolStripMenuItem_Click()
        {
            int count = Model.DgDataOfGlueCaliperWidthList.Count;
           
            if (Model.DgDataSelectIndex > 0&& Model.DgDataSelectIndex<count)
            {
                int index = Model.DgDataSelectIndex;             
                Model.DgDataOfGlueCaliperWidthList.RemoveAt(index);
                regionBufList.RemoveAt(index);
            }

        }

        private void dataGridViewEx1_DoubleClick(object o)
        {
            
            int index = Model.DgDataSelectIndex;
            if(index<0 || index> Model.DgDataOfGlueCaliperWidthList.Count) return;
            glueCaliperRegion = regionBufList[index].region;
            if (BaseTool.ObjectValided(glueCaliperRegion) &&
                     BaseTool.ObjectValided(ShowTool.D_HImage))
            {
                ShowTool.DispRegion(glueCaliperRegion, "blue");
                ShowTool.AddregionBuffer(glueCaliperRegion, "blue");
            }
        }
    

        private void dataGridViewEx1_CellValueChanged(object o)
        {

            DataGridCellInfo info = (DataGridCellInfo)o;
            if (Model.DgDataSelectIndex >= 0)
                if (info.Column.SortMemberPath == "CaliperName")
                {
                    bool isCheck = Model.DgDataOfGlueCaliperWidthList[Model.DgDataSelectIndex].Use;
                    StuRegionBuf stuRegion = regionBufList[Model.DgDataSelectIndex];
                    stuRegion.isUse = isCheck;
                    stuRegion.regionName = Model.DgDataOfGlueCaliperWidthList[Model.DgDataSelectIndex].CaliperName;
                    regionBufList[Model.DgDataSelectIndex] = stuRegion;
                }

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

        static void draw_rake(HTuple hv_WindowHandle, out HTuple hv_Row1, out HTuple hv_Column1,
              out HTuple hv_Row2, out HTuple hv_Column2)
        {



            // Local iconic variables 

            HObject ho_Regions;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            if (hv_WindowHandle.D > 100000)
                HOperatorSet.SetFont(hv_WindowHandle, "Arial-" + "12");
            else
                HOperatorSet.SetFont(hv_WindowHandle, "-Arial-" + "12" + "-*-*-*-*-1-");
            //提示
            disp_message(hv_WindowHandle, new HTuple("点击鼠标左键画一条直线,点击右键确认"),
                "image", 12, 12, "red", "false");
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            //画矢量检测直线
            HOperatorSet.DrawLine(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
                out hv_Column2);
            //产生直线xld
            //gen_contour_polygon_xld (RegionLines, [Row1,Row2], [Column1,Column2])
            //存储到显示对象
            //concat_obj (Regions, RegionLines, Regions)
            //计算直线与x轴的夹角，逆时针方向为正向。
            //angle_lx (Row1, Column1, Row2, Column2, ATan)

            //边缘检测方向垂直于检测直线：直线方向正向旋转90°为边缘检测方向
            //ATan := ATan+rad(90)

            //根据检测直线按顺序产生测量区域矩形，并存储到显示对象
            //for i := 1 to Elements by 1
            //如果只有一个测量矩形，作为卡尺工具，宽度为检测直线的长度
            //if (Elements=1)
            //RowC := (Row1+Row2)*0.5
            //ColC := (Column1+Column2)*0.5
            //distance_pp (Row1, Column1, Row2, Column2, Distance)
            //gen_rectangle2_contour_xld (Rectangle, RowC, ColC, ATan, DetectHeight/2, Distance/2)
            //else
            //如果有多个测量矩形，产生该测量矩形xld
            //RowC := Row1+(((Row2-Row1)*(i-1))/(Elements-1))
            //ColC := Column1+(Column2-Column1)*(i-1)/(Elements-1)
            //gen_rectangle2_contour_xld (Rectangle, RowC, ColC, ATan, DetectHeight/2, DetectWidth/2)
            //endif
            //把测量矩形xld存储到显示对象
            //concat_obj (Regions, Rectangle, Regions)
            //if (i=1)
            //在第一个测量矩形绘制一个箭头xld，用于只是边缘检测方向
            //RowL2 := RowC+DetectHeight/2*sin(-ATan)
            //RowL1 := RowC-DetectHeight/2*sin(-ATan)
            //ColL2 := ColC+DetectHeight/2*cos(-ATan)
            //ColL1 := ColC-DetectHeight/2*cos(-ATan)
            //gen_arrow_contour_xld (Arrow1, RowL1, ColL1, RowL2, ColL2, 5, 5)
            //把xld存储到显示对象
            //concat_obj (Regions, Arrow1, Regions)
            //endif
            //endfor

            ho_Regions.Dispose();

            return;
        }

        // External procedures 
        static private void get_rake2_region(HObject ho_Image, out HObject ho_Regions, HTuple hv_DetectHeight,
            HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionLines, ho_Rectangle;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_ATan = null;
            HTuple hv_RowC = null, hv_ColC = null, hv_Distance = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);

            //产生直线xld
            ho_RegionLines.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_RegionLines, hv_Row1.TupleConcat(hv_Row2),
                hv_Column1.TupleConcat(hv_Column2));
            //存储到显示对象
            //concat_obj (Regions, RegionLines, Regions)
            //计算直线与x轴的夹角，逆时针方向为正向。
            HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_ATan);

            //边缘检测方向：直线方向为边缘检测方向
            hv_ATan = hv_ATan + ((new HTuple(0)).TupleRad());

            //根据检测直线按顺序产生测量区域矩形，并存储到显示对象
            hv_RowC = (hv_Row1 + hv_Row2) * 0.5;
            hv_ColC = (hv_Column1 + hv_Column2) * 0.5;
            //判断是否超出图像,超出不检测边缘
            if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
            {
                ho_RegionLines.Dispose();
                ho_Rectangle.Dispose();

                return;
            }
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC, hv_ATan,
                hv_Distance / 2, hv_DetectHeight / 2);


            //把测量矩形xld存储到显示对象
            {
                HObject ExpTmpOutVar_0;
                HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle, out ExpTmpOutVar_0);
                ho_Regions.Dispose();
                ho_Regions = ExpTmpOutVar_0;
            }

            //在第一个测量矩形绘制一个箭头xld，用于只是边缘检测方向
            //RowL2 := RowC+Distance/2*sin(-ATan)
            //RowL1 := RowC-Distance/2*sin(-ATan)
            //ColL2 := ColC+Distance/2*cos(-ATan)
            //ColL1 := ColC-Distance/2*cos(-ATan)
            //gen_arrow_contour_xld (Arrow1, RowL1, ColL1, RowL2, ColL2, DetectHeight/2, DetectHeight/2)
            //把xld存储到显示对象
            //concat_obj (Regions, Arrow1, Regions)

            ho_RegionLines.Dispose();
            ho_Rectangle.Dispose();

            return;
        }

        #endregion
    }
}
