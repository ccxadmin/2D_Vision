using ControlShareResources.Common;
using GlueDetectionLib.参数;
using GlueDetectionLib.工具;
using GlueDetectionLib.窗体.Models;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionShowLib.UserControls;

namespace GlueDetectionLib.窗体.ViewModels
{
    public class GlueGapViewModel : BaseViewModel
    {
        //DataManage dataManage = null;
        //HObject imgBuf = null;//图像缓存
        HObject glueSearchRegion = null;//搜索区域
        HObject glueShapeRegion = null;//胶水外形区域
        HObject inspectROI = null;//缺胶检测区域
        List<StuRegionBuf> regionBufList = null;//绘制区域缓存     
        EumGenRegionWay genRegionWay;//检测区域生成方式
        HTuple matrix2D = null;

        EumDrawRegionType drawRegionType = EumDrawRegionType.any;

        public static GlueGapViewModel This { get; set; }
        public GlueGapModel Model { get; set; }

        #region Command Define
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        //绘制胶水外形自动提取区域
        public CommandBase DrawRegionOfAutoBtnClickCommand { get; set; }
        //获取自动区域类型
        public CommandBase AutoRegionTypeSelectionChangedCommand { get; set; }
        //获取手动区域类型
        public CommandBase ManulRegionTypeSelectionChangedCommand { get; set; }
        //自动提取胶水外形
        public CommandBase AutoExtractGlueRegionBtnClickCommand { get; set; }
        public CommandBase UseAutoGenInnerCheckedChangedCommand { get; set; }
        public CommandBase UseAutoGenInner2CheckedChangedCommand { get; set; }
        //生成内圈检测区域1
        public CommandBase AutoGenInnerRegionBtnClickCommand { get; set; }
        //生成内圈检测区域2
        public CommandBase AutoGenInnerRegion2BtnClickCommand { get; set; }
        public CommandBase UnionWaySelectionChangedCommand { get; set; }
        public CommandBase UnionWay2SelectionChangedCommand { get; set; }
        public CommandBase RdbtnCheckedChangedCommand { get; set; }
        public CommandBase ManualDrawRegionCheckedCommand { get; set; }
        public CommandBase ManualDrawRegionBtnClickCommand { get; set; }
        public CommandBase UnionWay3SelectionChangedCommand { get; set; }
        public CommandBase ManualDrawRegion2CheckedCommand { get; set; }
        public CommandBase ManualDrawRegionBtn2ClickCommand { get; set; }
        public CommandBase UnionWay4SelectionChangedCommand { get; set; }
        public CommandBase UsePosiCorrectCheckedCommand { get; set; }
        public CommandBase MatrixSelectionChangedCommand { get; set; }
        public CommandBase GetPixelRatioBtnClickCommand { get; set; }
        public CommandBase BaseRegionCheckedChangeCommad { get; set; }
        public CommandBase ExtratBaseRegionClickCommand { get; set; }
        //保存按钮
        public CommandBase SaveButClickCommand { get; set; }
        //测试按钮
        public CommandBase TestButClickCommand { get; set; }
        public CommandBase MenuItemClickCommand { get; set; }
        #endregion
        public GlueGapViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new GlueGapModel();

            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
        
            #region  Command
            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DrawRegionOfAutoBtnClickCommand = new CommandBase();
            DrawRegionOfAutoBtnClickCommand.DoExecute = new Action<object>((o) => btnDrawRegionOfAuto_Click());
            DrawRegionOfAutoBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            AutoRegionTypeSelectionChangedCommand = new CommandBase();
            AutoRegionTypeSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxAutoRegionType_SelectedIndexChanged(o));
            AutoRegionTypeSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            AutoExtractGlueRegionBtnClickCommand = new CommandBase();
            AutoExtractGlueRegionBtnClickCommand.DoExecute = new Action<object>((o) => btnAutoExtractGlueRegion_Click());
            AutoExtractGlueRegionBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            UseAutoGenInnerCheckedChangedCommand = new CommandBase();
            UseAutoGenInnerCheckedChangedCommand.DoExecute = new Action<object>((o) => chxbUseAutoGenInner_CheckedChanged());
            UseAutoGenInnerCheckedChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            UseAutoGenInner2CheckedChangedCommand = new CommandBase();
            UseAutoGenInner2CheckedChangedCommand.DoExecute = new Action<object>((o) => chxbUseAutoGenInner2_CheckedChanged());
            UseAutoGenInner2CheckedChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            AutoGenInnerRegionBtnClickCommand = new CommandBase();
            AutoGenInnerRegionBtnClickCommand.DoExecute = new Action<object>((o) => btnAutoGenInnerRegion_Click());
            AutoGenInnerRegionBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            AutoGenInnerRegion2BtnClickCommand = new CommandBase();
            AutoGenInnerRegion2BtnClickCommand.DoExecute = new Action<object>((o) => btnAutoGenInnerRegion2_Click());
            AutoGenInnerRegion2BtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
          
            UnionWaySelectionChangedCommand = new CommandBase();
            UnionWaySelectionChangedCommand.DoExecute = new Action<object>((o) => cobxUnionWay_SelectedIndexChanged(o));
            UnionWaySelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            UnionWay2SelectionChangedCommand = new CommandBase();
            UnionWay2SelectionChangedCommand.DoExecute = new Action<object>((o) => cobxUnionWay2_SelectedIndexChanged(o));
            UnionWay2SelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            RdbtnCheckedChangedCommand = new CommandBase();
            RdbtnCheckedChangedCommand.DoExecute = new Action<object>((o) => rdbtn_CheckedChanged(o));
            RdbtnCheckedChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ManulRegionTypeSelectionChangedCommand = new CommandBase();
            ManulRegionTypeSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxManulRegionType_SelectedIndexChanged(o));
            ManulRegionTypeSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ManualDrawRegionCheckedCommand = new CommandBase();
            ManualDrawRegionCheckedCommand.DoExecute = new Action<object>((o) => chxbManualDrawRegion_CheckedChanged());
            ManualDrawRegionCheckedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ManualDrawRegionBtnClickCommand = new CommandBase();
            ManualDrawRegionBtnClickCommand.DoExecute = new Action<object>((o) => btnManualDrawRegion_Click());
            ManualDrawRegionBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            UnionWay3SelectionChangedCommand = new CommandBase();
            UnionWay3SelectionChangedCommand.DoExecute = new Action<object>((o) => cobxUnionWay3_SelectedIndexChanged(o));
            UnionWay3SelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ManualDrawRegion2CheckedCommand = new CommandBase();
            ManualDrawRegion2CheckedCommand.DoExecute = new Action<object>((o) => chxbManualDrawRegion2_CheckedChanged());
            ManualDrawRegion2CheckedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ManualDrawRegionBtn2ClickCommand = new CommandBase();
            ManualDrawRegionBtn2ClickCommand.DoExecute = new Action<object>((o) => btnManualDrawRegion2_Click());
            ManualDrawRegionBtn2ClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            UnionWay4SelectionChangedCommand = new CommandBase();
            UnionWay4SelectionChangedCommand.DoExecute = new Action<object>((o) => cobxUnionWay4_SelectedIndexChanged(o));
            UnionWay4SelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            UsePosiCorrectCheckedCommand = new CommandBase();
            UsePosiCorrectCheckedCommand.DoExecute = new Action<object>((o) => chxbUsePosiCorrect_CheckedChanged());
            UsePosiCorrectCheckedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            MatrixSelectionChangedCommand = new CommandBase();
            MatrixSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxMatrixList_SelectedIndexChanged(o));
            MatrixSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            GetPixelRatioBtnClickCommand = new CommandBase();
            GetPixelRatioBtnClickCommand.DoExecute = new Action<object>((o) => btnGetPixelRatio_Click());
            GetPixelRatioBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            BaseRegionCheckedChangeCommad = new CommandBase();
            BaseRegionCheckedChangeCommad.DoExecute = new Action<object>((o) => checkBox1_CheckedChanged());
            BaseRegionCheckedChangeCommad.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ExtratBaseRegionClickCommand = new CommandBase();
            ExtratBaseRegionClickCommand.DoExecute = new Action<object>((o) => btnExtratBaseRegion_Click());
            ExtratBaseRegionClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSave_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            
            MenuItemClickCommand = new CommandBase();
            MenuItemClickCommand.DoExecute = new Action<object>((o) => 重置ToolStripMenuItem_Click());
            MenuItemClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            #endregion
            HOperatorSet.GenEmptyObj(out glueSearchRegion);
            HOperatorSet.GenEmptyObj(out glueShapeRegion);

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
            //检测区域
            if (BaseTool.ObjectValided((par as GlueGapParam).InspectROI))
                HOperatorSet.CopyObj((par as GlueGapParam).InspectROI, out inspectROI, 1, -1);

            this.regionBufList = (par as GlueGapParam).RegionBufList;
            this.genRegionWay = (par as GlueGapParam).GenRegionWay;
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as GlueGapParam).InputImageName;
            int index1 = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index1;

            foreach (var s in dataManage.matrixBufDic)
                Model.MatrixList.Add(s.Key);
            string matrixName = (par as GlueGapParam).MatrixName;
            int index2 = Model.MatrixList.IndexOf(matrixName);
            Model.SelectMatrixIndex = index2;


            Model.AutoRegionTypeSelectIndex = (baseTool as GlueGapTool).autoRegionTypeSelectIndex;
            Model.SelectPolarityIndex = (baseTool as GlueGapTool).selectPolarityIndex;
            Model.MorphProcessSelectIndex = (baseTool as GlueGapTool).morphProcessSelectIndex;
            Model.NumRadius = (baseTool as GlueGapTool).numRadius;
            Model.ConvertUnitsSelectIndex = (baseTool as GlueGapTool).convertUnitsSelectIndex;
            Model.ManulRegionTypeSelectIndex = (baseTool as GlueGapTool).manulRegionTypeSelectIndex;
            Model.ShowBaseRegionChecked = (baseTool as GlueGapTool).showBaseRegionChecked;

            Model.SelectImageName = (par as GlueGapParam).InputImageName;
            Model.PixelRatio = (par as GlueGapParam).PixleRatio.ToString();
            Model.GrayDown = (par as GlueGapParam).GrayMin;
            Model.GrayUp = (par as GlueGapParam).GrayMax;
            Model.AreaDown=(par as GlueGapParam).AreaMin;
            Model.AreaUp=(par as GlueGapParam).AreaMax;
            Model.MatrixEnable= Model.UsePosiCorrectChecked = (par as GlueGapParam).UsePosiCorrect;

            if (genRegionWay == EumGenRegionWay.auto)
                Model.GenRegionWay = Models.EumGenRegionWay.auto;
            else
                Model.GenRegionWay = Models.EumGenRegionWay.manual;

            if (regionBufList.Exists(t => t.regionName == "自动内圈区域1"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "自动内圈区域1");
                StuRegionBuf regionBuf = regionBufList[index];
                Model.UseAutoGenInner1Checked = regionBuf.isUse;
                {
                    Model.BtnAutoGenInnerRegion1Enable = regionBuf.isUse;
                    Model.CobxUnionWayEnable = regionBuf.isUse;
                }

                Model.UnionWaySelectIndex = (int)regionBuf.regionUnionWay;

            }
            if (regionBufList.Exists(t => t.regionName == "自动内圈区域2"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "自动内圈区域2");
                StuRegionBuf regionBuf = regionBufList[index];
                Model.UseAutoGenInner2Checked = regionBuf.isUse;

                {
                    Model.BtnAutoGenInnerRegion2Enable = regionBuf.isUse;
                    Model.CobxUnionWay2Enable = regionBuf.isUse;
                }
                Model.UnionWay2SelectIndex = (int)regionBuf.regionUnionWay;
            }
            if (regionBufList.Exists(t => t.regionName == "手动检测区域1"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "手动检测区域1");
                StuRegionBuf regionBuf = regionBufList[index];
                Model.UseManualDrawRegionChecked = regionBuf.isUse;
                {
                    Model.BtnManualDrawRegionEnable = regionBuf.isUse;
                    Model.CobxUnionWay3Enable = regionBuf.isUse;
                }
                Model.UnionWay3SelectIndex = (int)regionBuf.regionUnionWay;

            }
            if (regionBufList.Exists(t => t.regionName == "手动检测区域2"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "手动检测区域2");
                StuRegionBuf regionBuf = regionBufList[index];
                Model.UseManualDrawRegion2Checked = regionBuf.isUse;
                {
                    Model.BtnManualDrawRegion2Enable = regionBuf.isUse;
                    Model.CobxUnionWay4Enable = regionBuf.isUse;
                }
                Model.UnionWay4SelectIndex = (int)regionBuf.regionUnionWay;
            }

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

            if (!GlueCaliperWidthTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as GlueGapParam).InputImageName = Model.SelectImageName;
        }
       
        /// <summary>
        /// 绘制自动区域类型索引
        /// </summary>
        /// <param name="value"></param>
        private void cobxAutoRegionType_SelectedIndexChanged(object value)
        {
            drawRegionType = (EumDrawRegionType)Model.AutoRegionTypeSelectIndex;
        }
        /// <summary>
        /// 自动提取胶水外形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoExtractGlueRegion_Click()
        {
            if (!GlueGapTool.ObjectValided(imgBuf))
            {
                MessageBox.Show("图像为空！");
                return;
            }
            if (!GlueGapTool.ObjectValided(glueSearchRegion))
            {
                MessageBox.Show("搜索区域为空！");
                return;
            }
            HOperatorSet.ReduceDomain(imgBuf, glueSearchRegion, out HObject imageReduced);
            glueShapeRegion.Dispose();
            if (Model.SelectPolarityIndex == 0)
                HOperatorSet.BinaryThreshold(imageReduced, out glueShapeRegion, "max_separability", "light", out HTuple UsedThreshold);
            else
                HOperatorSet.BinaryThreshold(imageReduced, out glueShapeRegion, "max_separability", "dark", out HTuple UsedThreshold);
            HOperatorSet.FillUp(glueShapeRegion, out glueShapeRegion);
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.DispRegion(glueShapeRegion, "green");
            ShowTool.AddregionBuffer(glueShapeRegion, "green");
        }

        private void chxbUseAutoGenInner_CheckedChanged()
        {
            bool checkFlag = Model.UseAutoGenInner1Checked;
            if (checkFlag)
            {
                Model. BtnAutoGenInnerRegion1Enable = true;
                Model.CobxUnionWayEnable = true;
            }
            else
            {
                Model.BtnAutoGenInnerRegion1Enable = false;
                Model.CobxUnionWayEnable = false;
                if (regionBufList.Exists(t => t.regionName == "自动内圈区域1"))
                {
                    int index = regionBufList.FindIndex(t => t.regionName == "自动内圈区域1");
                    StuRegionBuf buf = regionBufList[index];
                    buf.isUse = false;
                    regionBufList[index] = buf;
                }
            }
        }

        private void chxbUseAutoGenInner2_CheckedChanged()
        {
            bool checkFlag = Model.UseAutoGenInner2Checked;
            if (checkFlag)
            {
                Model.BtnAutoGenInnerRegion2Enable = true;
                Model.CobxUnionWay2Enable = true;
            }
            else
            {
                Model.BtnAutoGenInnerRegion2Enable = false;
                Model.CobxUnionWay2Enable = false;
                if (regionBufList.Exists(t => t.regionName == "自动内圈区域2"))
                {
                    int index = regionBufList.FindIndex(t => t.regionName == "自动内圈区域2");
                    StuRegionBuf buf = regionBufList[index];
                    buf.isUse = false;
                    regionBufList[index] = buf;
                }
            }
        }
        /// <summary>
        /// 绘制胶水外形自动提取区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrawRegionOfAuto_Click()
        {
            if (!GlueGapTool.ObjectValided(imgBuf))
            {
                ShowTool.DispAlarmMessage("未获取图像", 500, 20, 30);
                return;
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.DesignModel);
            ShowTool.setMouseStateOfNone();

            if (MessageBox.Show("准备创建自动提取胶水搜索区域ROI？", "Information",
                       MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
             

                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");

                //HOperatorSet.DrawRectangle1(ShowTool.HWindowsHandle, out HTuple hv_Row1, out HTuple hv_Column1,
                //    out HTuple hv_Row2, out HTuple hv_Column2);
                //glueSearchRegion.Dispose();
                //HOperatorSet.GenRectangle1(out glueSearchRegion, hv_Row1, hv_Column1, hv_Row2,
                //    hv_Column2);
                drawRegionType = (EumDrawRegionType)Model.AutoRegionTypeSelectIndex;

                HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);
                if (drawRegionType == EumDrawRegionType.any)
                    HOperatorSet.DrawRegion(out glueSearchRegion, ShowTool.HWindowsHandle);
                else if (drawRegionType == EumDrawRegionType.rectangle)
                {
                    HOperatorSet.DrawRectangle1(ShowTool.HWindowsHandle, out HTuple row1, out HTuple column1,
                        out HTuple row2, out HTuple column2);
                    HOperatorSet.GenRectangle1(out glueSearchRegion, row1, column1, row2, column2);
                }
                else if (drawRegionType == EumDrawRegionType.rarectangle)
                {
                    HOperatorSet.DrawRectangle2(ShowTool.HWindowsHandle, out HTuple row, out HTuple column,
                         out HTuple phi, out HTuple length1, out HTuple length2);
                    HOperatorSet.GenRectangle2(out glueSearchRegion, row, column, phi, length1, length2);
                }
                else
                {
                    HOperatorSet.DrawCircle(ShowTool.HWindowsHandle, out HTuple row, out HTuple column,
                      out HTuple radius);
                    HOperatorSet.GenCircle(out glueSearchRegion, row, column, radius);
                }
                HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(glueSearchRegion, "blue");
                ShowTool.AddregionBuffer(glueSearchRegion, "blue");

                ShowTool.AddRightMenu();

               
            }
           
        }
        /// <summary>
        /// 自动生成内圈区域1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoGenInnerRegion_Click()
        {
            if (!GlueGapTool.ObjectValided(imgBuf))
            {
                MessageBox.Show("图像为空！");
                return;
            }
            if (!GlueGapTool.ObjectValided(glueShapeRegion))
            {
                MessageBox.Show("胶水外形区域为空！");
                return;
            }

            HObject innerRegion = null;
            int radius = Model.NumRadius;
            //物理单位
            if (Model.ConvertUnitsSelectIndex == 1)
            {
                if (!double.TryParse(Model.PixelRatio, out double value))
                    value = 1.0;
                radius = (int)(radius / value);
            }
            if (radius < 1)
                HOperatorSet.CopyObj(glueShapeRegion, out innerRegion, 1, -1);
            else
            {
                if (Model.MorphProcessSelectIndex == 0)
                    HOperatorSet.DilationCircle(glueShapeRegion, out innerRegion, radius);
                else
                    HOperatorSet.ErosionCircle(glueShapeRegion, out innerRegion, radius);
            }

            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.DispRegion(innerRegion, "orange");
            ShowTool.AddregionBuffer(innerRegion, "orange");
            EumRegionUnionWay way = EumRegionUnionWay.concat;
            if (Model.UnionWaySelectIndex == 1)
                way = EumRegionUnionWay.difference;
            if (regionBufList.Exists(t => t.regionName == "自动内圈区域1"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "自动内圈区域1");
                StuRegionBuf buf = regionBufList[index];
                buf.regionUnionWay = way;
                buf.isUse = true;
                buf.region = innerRegion.Clone();
                regionBufList[index] = buf;
            }
            else
            {
                regionBufList.Add(new StuRegionBuf
                {
                    regionName = "自动内圈区域1",
                    isUse = true,
                    region = innerRegion.Clone(),
                    regionUnionWay = way
                });
            }
        }
        /// <summary>
        /// 自动生成内圈区域2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoGenInnerRegion2_Click()
        {
            if (!GlueGapTool.ObjectValided(imgBuf))
            {
                MessageBox.Show("图像为空！");
                return;
            }
            if (!GlueGapTool.ObjectValided(glueShapeRegion))
            {
                MessageBox.Show("胶水外形区域为空！");
                return;
            }

            HObject innerRegion2 = null;
            int radius = Model.NumRadius;

            if (radius < 1)
                HOperatorSet.CopyObj(glueShapeRegion, out innerRegion2, 1, -1);
            else
            {
                if (Model.MorphProcessSelectIndex == 0)
                    HOperatorSet.DilationCircle(glueShapeRegion, out innerRegion2, radius);
                else
                    HOperatorSet.ErosionCircle(glueShapeRegion, out innerRegion2, radius);
            }

            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.DispRegion(innerRegion2, "orange");
            ShowTool.AddregionBuffer(innerRegion2, "orange");
            EumRegionUnionWay way = EumRegionUnionWay.concat;
            if (Model.UnionWay2SelectIndex == 1)
                way = EumRegionUnionWay.difference;
            if (regionBufList.Exists(t => t.regionName == "自动内圈区域2"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "自动内圈区域2");
                StuRegionBuf buf = regionBufList[index];
                buf.regionUnionWay = way;
                buf.isUse = true;
                buf.region = innerRegion2.Clone();
                regionBufList[index] = buf;
            }
            else
            {
                regionBufList.Add(new StuRegionBuf
                {
                    regionName = "自动内圈区域2",
                    isUse = true,
                    region = innerRegion2.Clone(),
                    regionUnionWay = way
                });
            }
        }

        private void cobxUnionWay_SelectedIndexChanged(object o)
        {
            EumRegionUnionWay way = EumRegionUnionWay.concat;
            if (Model.UnionWaySelectIndex == 1)
                way = EumRegionUnionWay.difference;
            if (regionBufList.Exists(t => t.regionName == "自动内圈区域1"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "自动内圈区域1");
                StuRegionBuf buf = regionBufList[index];
                buf.regionUnionWay = way;
                regionBufList[index] = buf;
            }
        }
        private void cobxUnionWay2_SelectedIndexChanged(object o)
        {
            EumRegionUnionWay way = EumRegionUnionWay.concat;
            if (Model.UnionWay2SelectIndex == 1)
                way = EumRegionUnionWay.difference;
            if (regionBufList.Exists(t => t.regionName == "自动内圈区域2"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "自动内圈区域2");
                StuRegionBuf buf = regionBufList[index];
                buf.regionUnionWay = way;
                regionBufList[index] = buf;
            }
        }

        /// <summary>
        /// 手自动区域设置切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbtn_CheckedChanged(object o)
        {
            if (Model.GenRegionWay== Models.EumGenRegionWay.auto)
            {

                genRegionWay = EumGenRegionWay.auto;
                Model.UseManualDrawRegionChecked = false;
                Model.UseManualDrawRegion2Checked = false;
                Model.BtnManualDrawRegionEnable = false;
                Model.CobxUnionWay3Enable = false;
                Model.BtnManualDrawRegion2Enable = false;
                Model.CobxUnionWay4Enable = false;
            }
            else
            {
                genRegionWay = EumGenRegionWay.manual;
                Model.UseAutoGenInner1Checked = false;
                Model.UseAutoGenInner2Checked = false;
                Model.BtnAutoGenInnerRegion1Enable = false;
                Model.CobxUnionWayEnable = false;
                Model.BtnAutoGenInnerRegion2Enable = false;
                Model.CobxUnionWay2Enable = false;
            }
        }

        /// <summary>
        /// 绘制手动区域类型索引
        /// </summary>
        /// <param name="value"></param>
        private void cobxManulRegionType_SelectedIndexChanged(object value)
        {
            drawRegionType = (EumDrawRegionType)Model.ManulRegionTypeSelectIndex;
        }
        private void chxbManualDrawRegion_CheckedChanged()
        {
            bool checkFlag = Model.UseManualDrawRegionChecked;
            if (checkFlag)
            {
                Model.BtnManualDrawRegionEnable = true;
                Model.CobxUnionWay3Enable = true;
            }
            else
            {
                Model.BtnManualDrawRegionEnable = false;
                Model.CobxUnionWay3Enable = false;
                if (regionBufList.Exists(t => t.regionName == "手动检测区域1"))
                {
                    int index = regionBufList.FindIndex(t => t.regionName == "手动检测区域1");
                    StuRegionBuf buf = regionBufList[index];
                    buf.isUse = false;
                    regionBufList[index] = buf;
                }
            }
        }
        //手动设置检测区域
        private void btnManualDrawRegion_Click()
        {
            if (!GlueGapTool.ObjectValided(imgBuf))
            {
                ShowTool.DispAlarmMessage("未获取图像", 500, 20, 30);
                return;
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.DesignModel);
            ShowTool.setMouseStateOfNone();

            if (MessageBox.Show("准备手动创建胶水检测区域？", "Information",
                       MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                HObject temRegion = null;
                HOperatorSet.GenEmptyObj(out temRegion);
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
               

                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");

                drawRegionType = (EumDrawRegionType)Model.ManulRegionTypeSelectIndex;

                HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);
                if (drawRegionType == EumDrawRegionType.any)
                    HOperatorSet.DrawRegion(out temRegion, ShowTool.HWindowsHandle);
                else if (drawRegionType == EumDrawRegionType.rectangle)
                {
                    HOperatorSet.DrawRectangle1(ShowTool.HWindowsHandle, out HTuple row1, out HTuple column1,
                        out HTuple row2, out HTuple column2);
                    HOperatorSet.GenRectangle1(out temRegion, row1, column1, row2, column2);
                }
                else if (drawRegionType == EumDrawRegionType.rarectangle)
                {
                    HOperatorSet.DrawRectangle2(ShowTool.HWindowsHandle, out HTuple row, out HTuple column,
                         out HTuple phi, out HTuple length1, out HTuple length2);
                    HOperatorSet.GenRectangle2(out temRegion, row, column, phi, length1, length2);
                }
                else
                {
                    HOperatorSet.DrawCircle(ShowTool.HWindowsHandle, out HTuple row, out HTuple column,
                      out HTuple radius);
                    HOperatorSet.GenCircle(out temRegion, row, column, radius);
                }

                HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(temRegion, "blue");
                ShowTool.AddregionBuffer(temRegion, "blue");
                ShowTool.AddRightMenu();
              
                //添加检测区域到集合
                EumRegionUnionWay way = EumRegionUnionWay.concat;
                if (Model.UnionWay3SelectIndex == 1)
                    way = EumRegionUnionWay.difference;
                if (regionBufList.Exists(t => t.regionName == "手动检测区域1"))
                {
                    int index = regionBufList.FindIndex(t => t.regionName == "手动检测区域1");
                    StuRegionBuf buf = regionBufList[index];
                    buf.regionUnionWay = way;
                    buf.isUse = true;
                    buf.region = temRegion.Clone();
                    regionBufList[index] = buf;
                }
                else
                {
                    regionBufList.Add(new StuRegionBuf
                    {
                        regionName = "手动检测区域1",
                        isUse = true,
                        region = temRegion.Clone(),
                        regionUnionWay = way
                    });
                }

            }
            //ShowTool.SetSystemPatten(EumSystemPattern.RunningModel);
        }
        private void cobxUnionWay3_SelectedIndexChanged(object o)
        {
            EumRegionUnionWay way = EumRegionUnionWay.concat;
            if (Model.UnionWay3SelectIndex ==1)
                way = EumRegionUnionWay.difference;
            if (regionBufList.Exists(t => t.regionName == "手动检测区域1"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "手动检测区域1");
                StuRegionBuf buf = regionBufList[index];
                buf.regionUnionWay = way;
                regionBufList[index] = buf;
            }
        }
        private void chxbManualDrawRegion2_CheckedChanged()
        {
            bool checkFlag = Model.UseManualDrawRegion2Checked;
            if (checkFlag)
            {
                Model.BtnManualDrawRegion2Enable = true;
                Model.CobxUnionWay4Enable = true;
            }
            else
            {
                Model.BtnManualDrawRegion2Enable = false;
                Model.CobxUnionWay4Enable = false;
                if (regionBufList.Exists(t => t.regionName == "手动检测区域2"))
                {
                    int index = regionBufList.FindIndex(t => t.regionName == "手动检测区域2");
                    StuRegionBuf buf = regionBufList[index];
                    buf.isUse = false;
                    regionBufList[index] = buf;
                }
            }
        }
        private void btnManualDrawRegion2_Click()
        {
            if (!GlueGapTool.ObjectValided(imgBuf))
            {
                ShowTool.DispAlarmMessage("未获取图像", 500, 20, 30);
                return;
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.DesignModel);
            ShowTool.setMouseStateOfNone();

            if (MessageBox.Show("准备手动创建胶水检测区域？", "Information",
                       MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                HObject temRegion = null;
                HOperatorSet.GenEmptyObj(out temRegion);
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
             

                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");
                drawRegionType = (EumDrawRegionType)Model.ManulRegionTypeSelectIndex;

                HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);
                if (drawRegionType == EumDrawRegionType.any)
                    HOperatorSet.DrawRegion(out temRegion, ShowTool.HWindowsHandle);
                else if (drawRegionType == EumDrawRegionType.rectangle)
                {
                    HOperatorSet.DrawRectangle1(ShowTool.HWindowsHandle, out HTuple row1, out HTuple column1,
                        out HTuple row2, out HTuple column2);
                    HOperatorSet.GenRectangle1(out temRegion, row1, column1, row2, column2);
                }
                else if (drawRegionType == EumDrawRegionType.rarectangle)
                {
                    HOperatorSet.DrawRectangle2(ShowTool.HWindowsHandle, out HTuple row, out HTuple column,
                         out HTuple phi, out HTuple length1, out HTuple length2);
                    HOperatorSet.GenRectangle2(out temRegion, row, column, phi, length1, length2);
                }
                else
                {
                    HOperatorSet.DrawCircle(ShowTool.HWindowsHandle, out HTuple row, out HTuple column,
                      out HTuple radius);
                    HOperatorSet.GenCircle(out temRegion, row, column, radius);
                }

                HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(temRegion, "blue");
                ShowTool.AddregionBuffer(temRegion, "blue");
                ShowTool.AddRightMenu();
            
                //添加检测区域到集合
                EumRegionUnionWay way = EumRegionUnionWay.concat;
                if (Model.UnionWay4SelectIndex == 1)
                    way = EumRegionUnionWay.difference;
                if (regionBufList.Exists(t => t.regionName == "手动检测区域2"))
                {
                    int index = regionBufList.FindIndex(t => t.regionName == "手动检测区域2");
                    StuRegionBuf buf = regionBufList[index];
                    buf.regionUnionWay = way;
                    buf.isUse = true;
                    buf.region = temRegion.Clone();
                    regionBufList[index] = buf;
                }
                else
                {
                    regionBufList.Add(new StuRegionBuf
                    {
                        regionName = "手动检测区域2",
                        isUse = true,
                        region = temRegion.Clone(),
                        regionUnionWay = way
                    });
                }

            }
            //ShowTool.SetSystemPatten(EumSystemPattern.RunningModel);
        }
        private void cobxUnionWay4_SelectedIndexChanged(object o)
        {
            EumRegionUnionWay way = EumRegionUnionWay.concat;
            if (Model.UnionWay4SelectIndex == 1)
                way = EumRegionUnionWay.difference;
            if (regionBufList.Exists(t => t.regionName == "手动检测区域2"))
            {
                int index = regionBufList.FindIndex(t => t.regionName == "手动检测区域2");
                StuRegionBuf buf = regionBufList[index];
                buf.regionUnionWay = way;
                regionBufList[index] = buf;
            }
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
                    (par as GlueGapParam).MatrixName = Model.SelectMatrixName;
                }

            }
        }
        /// <summary>
        ///输入矩阵旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxMatrixList_SelectedIndexChanged(object o)
        {
            matrix2D = dataManage.matrixBufDic[Model.SelectMatrixName];
            BaseParam par = baseTool.GetParam();
            (par as GlueGapParam).MatrixName = Model.SelectMatrixName;
        }
        /// <summary>
        /// 获取像素转换比
        /// </summary>
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
        /// 是否显示基准区域
        /// </summary>
        private void checkBox1_CheckedChanged()
        {
            if (Model.ShowBaseRegionChecked)
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
                if (GlueGapTool.ObjectValided((baseTool.GetParam() as GlueGapParam).ResultBaseRegion))
                {
                    ShowTool.DispRegion((baseTool.GetParam() as GlueGapParam).ResultBaseRegion, "green");
                    ShowTool.AddregionBuffer((baseTool.GetParam() as GlueGapParam).ResultBaseRegion, "green");
                }
            }
            else
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
            }
        }
        /// <summary>
        /// 区域操作集合
        /// </summary>
        /// <returns></returns>
        HObject UnionRegion()
        {
            HObject uRegion = null;
            HOperatorSet.GenEmptyObj(out uRegion);
            //先加
            foreach (var s in this.regionBufList)
            {
                if (!s.isUse)
                    continue;
                if (s.regionUnionWay == EumRegionUnionWay.concat)
                    HOperatorSet.ConcatObj(uRegion, s.region, out uRegion);
            }
            //后减
            foreach (var s in this.regionBufList)
            {
                if (!s.isUse)
                    continue;
                if (s.regionUnionWay == EumRegionUnionWay.difference)
                    HOperatorSet.Difference(uRegion, s.region, out uRegion);
            }
            return uRegion;
        }
        /// <summary>
        /// 提取基准区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExtratBaseRegion_Click()
        {
            //开始检测
            inspectROI = UnionRegion();
            HOperatorSet.ReduceDomain(imgBuf,
                inspectROI, out HObject imageReduced);
            HOperatorSet.Threshold(imageReduced, out HObject region,
                  Model.GrayDown, Model.GrayUp);
            BaseParam par = baseTool.GetParam();
            (par as GlueGapParam).ResultBaseRegion = region.Clone();
            if (Model.ShowBaseRegionChecked)
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
                if (GlueGapTool.ObjectValided((par as GlueGapParam).ResultBaseRegion))
                {
                    ShowTool.DispRegion((par as GlueGapParam).ResultBaseRegion, "green");
                    ShowTool.AddregionBuffer((par as GlueGapParam).ResultBaseRegion, "green");
                }
            }
        }
        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click()
        {
            BaseParam par = baseTool.GetParam();

            (baseTool as GlueGapTool).autoRegionTypeSelectIndex = Model.AutoRegionTypeSelectIndex;
            (baseTool as GlueGapTool).selectPolarityIndex = Model.SelectPolarityIndex;
            (baseTool as GlueGapTool).morphProcessSelectIndex = Model.MorphProcessSelectIndex;
            (baseTool as GlueGapTool).numRadius = Model.NumRadius;
            (baseTool as GlueGapTool).convertUnitsSelectIndex = Model.ConvertUnitsSelectIndex;
            (baseTool as GlueGapTool).manulRegionTypeSelectIndex = Model.ManulRegionTypeSelectIndex;
            (baseTool as GlueGapTool).showBaseRegionChecked = Model.ShowBaseRegionChecked;


            (par as GlueGapParam).PixleRatio =double.Parse( Model.PixelRatio);
            (par as GlueGapParam).GrayMin = Model.GrayDown;
            (par as GlueGapParam).GrayMax = Model.GrayUp;
            (par as GlueGapParam).AreaMin = Model.AreaDown;
            (par as GlueGapParam).AreaMax = Model.AreaUp;
            (par as GlueGapParam).UsePosiCorrect = Model.UsePosiCorrectChecked;
            (par as GlueGapParam).RegionBufList = this.regionBufList;
            (par as GlueGapParam).GenRegionWay = this.genRegionWay;
            inspectROI = UnionRegion();
            (par as GlueGapParam).InspectROI = inspectROI.Clone();
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
        }
        /// <summary>
        /// 手动测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as GlueGapParam).PixleRatio = double.Parse(Model.PixelRatio);
            (par as GlueGapParam).GrayMin = Model.GrayDown;
            (par as GlueGapParam).GrayMax = Model.GrayUp;
            (par as GlueGapParam).AreaMin = Model.AreaDown;
            (par as GlueGapParam).AreaMax = Model.AreaUp;
            (par as GlueGapParam).UsePosiCorrect = Model.UsePosiCorrectChecked;
            inspectROI = UnionRegion();
            (par as GlueGapParam).InspectROI = inspectROI.Clone();

            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);


            if (rlt.runFlag)
            {
                ShowTool.DispConcatedObj((par as GlueGapParam).OutputImg, EumCommonColors.red);
                ShowTool.AddConcatedObjBuffer((par as GlueGapParam).OutputImg, EumCommonColors.red);
                ShowTool.DispRegion((par as GlueGapParam).ResultInspectROI, "blue");
                ShowTool.AddregionBuffer((par as GlueGapParam).ResultInspectROI, "blue");
                //ShowTool.DispMessage("OK", 100, width - 500, "green", 50);
                //ShowTool.AddTextBuffer("OK", 100, width - 500, "green", 50);
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
            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispRegion((par as GlueGapParam).ResultInspectROI, "blue");
                ShowTool.AddregionBuffer((par as GlueGapParam).ResultInspectROI, "blue");
                ShowTool.DispMessage("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
            }

            //显示面积值
            int i = 0;
            foreach (var s in (baseTool as GlueGapTool).glueInfo.areaList)
            {
                ShowTool.DispMessage(s.ToString("f5"), 10 + 150 * i, 10, "green", 16);
                ShowTool.AddTextBuffer(s.ToString("f5"), 10 + 150 * i, 10, "green", 16);
                i++;
            }

        }

        private void 重置ToolStripMenuItem_Click()
        {
            if (this.regionBufList != null)
                this.regionBufList.Clear();
            if (genRegionWay == EumGenRegionWay.auto)
            {
                Model.UseAutoGenInner1Checked = false;
                Model.UseAutoGenInner2Checked = false;
                Model.BtnAutoGenInnerRegion1Enable = false;
                Model.CobxUnionWayEnable = false;
                Model.BtnAutoGenInnerRegion2Enable = false;
                Model.CobxUnionWay2Enable = false;
            }
            else
            {
                Model.UseManualDrawRegionChecked = false;
                Model.UseManualDrawRegion2Checked = false;
                Model.BtnManualDrawRegionEnable = false;
                Model.CobxUnionWay3Enable = false;
                Model.BtnManualDrawRegion2Enable = false;
                Model.CobxUnionWay4Enable = false;
            }
        }

    }
}
