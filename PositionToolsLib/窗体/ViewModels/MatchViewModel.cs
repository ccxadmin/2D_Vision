using ControlShareResources.Common;
using FunctionLib.Location;
using HalconDotNet;
using PositionToolsLib.参数;
using PositionToolsLib.工具;
using PositionToolsLib.窗体.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionShowLib.UserControls;

namespace PositionToolsLib.窗体.ViewModels
{
    public class MatchViewModel : BaseViewModel
    {
        HObject modelSearchRegion = null;//搜索区域
        HObject modelRegion = null;//模板区域
        HObject maskRegion = null;//掩膜区域
        EumModelSearch eumModelSearch = EumModelSearch.全图搜索;
        string s_rootFolder;//文件存放路径

        public static MatchViewModel This { get; set; }
        public MatchModel Model { get; set; }
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase SearchModelROISelectionChangedCommand { get; set; }
        public CommandBase DrawModelRegionClickCommand { get; set; }
        public CommandBase DrawModelSearchRegionClickCommand { get; set; }
        public CommandBase TranModelClickCommand { get; set; }
        public CommandBase MaskSetClickCommand { get; set; }
        public CommandBase SaveParamClickCommand { get; set; }
        public CommandBase ModelTestClickCommand { get; set; }

        public MatchViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new MatchModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
           

            #region Command
            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SearchModelROISelectionChangedCommand = new CommandBase();
            SearchModelROISelectionChangedCommand.DoExecute = new Action<object>((o) => cobxSearchModelROI_SelectedIndexChanged(o));
            SearchModelROISelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DrawModelRegionClickCommand = new CommandBase();
            DrawModelRegionClickCommand.DoExecute = new Action<object>((o) => btnDrawModelRegion_Click());
            DrawModelRegionClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DrawModelSearchRegionClickCommand = new CommandBase();
            DrawModelSearchRegionClickCommand.DoExecute = new Action<object>((o) => btnDrawModelSearchRegion_Click());
            DrawModelSearchRegionClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TranModelClickCommand = new CommandBase();
            TranModelClickCommand.DoExecute = new Action<object>((o) => btnTranModel_Click());
            TranModelClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            MaskSetClickCommand = new CommandBase();
            MaskSetClickCommand.DoExecute = new Action<object>((o) => btnMaskSet_Click());
            MaskSetClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveParamClickCommand = new CommandBase();
            SaveParamClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveParamClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ModelTestClickCommand = new CommandBase();
            ModelTestClickCommand.DoExecute = new Action<object>((o) => btnModelTest_Click());
            ModelTestClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            #endregion
            ShowData();
            cobxImageList_SelectedIndexChanged(null);
          
        }

        void ShowData()
        {       
            BaseParam par = baseTool.GetParam(); 
            if (BaseTool.ObjectValided((par as MatchParam).InspectROI))
            {
                modelSearchRegion.Dispose();
                HOperatorSet.CopyObj((par as MatchParam).InspectROI, out modelSearchRegion, 1, -1);
            }
            if (BaseTool.ObjectValided((par as MatchParam).MaskROI))
            {
                maskRegion?.Dispose();
                HOperatorSet.CopyObj((par as MatchParam).MaskROI, out maskRegion, 1, -1);
            }
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as MatchParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;

            Model.SelectImageName = (par as MatchParam).InputImageName;
            Model.StartAngle=(par as MatchParam).StartAngle ;
            Model.RangeAngle=(par as MatchParam).RangeAngle  ;
            Model.Contrast=(par as MatchParam).ContrastValue ;
            Model.MatchScore=(par as MatchParam).MatchScore  ;
            Model.ScaleDownLimit=(par as MatchParam).MatchDownScale ;
            Model.ScaleUpLimit=(par as MatchParam).MatchUpScale  ;
            Model.BaseXText = (par as MatchParam).ModelBaseCol.ToString("f3");
            Model.BaseYText = (par as MatchParam).ModelBaseRow.ToString("f3");
            Model.BaseAngleText = ((par as MatchParam).ModelBaseRadian * 180.0 / Math.PI).ToString("f3");//弧度转角度显示

            eumModelSearch = (par as MatchParam).ModelSearch;
            Model.SearchModelROISelectIndex = (int)eumModelSearch;         
            if (eumModelSearch == EumModelSearch.全图搜索)
            {
                if (modelSearchRegion != null)
                    modelSearchRegion.Dispose();
                Model.BtnDrawModelSearchRegionEnable = false;
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
        private void cobxImageList_SelectedIndexChanged(object o)
        {
            if (Model.SelectImageIndex == -1) return;
            if (!MatchTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as MatchParam).InputImageName = Model.SelectImageName;
        }

        /// <summary>
        /// 模板搜索区域类型设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxSearchModelROI_SelectedIndexChanged(object o)
        {
            if (Model.SearchModelROISelectIndex == -1) return;
            if (Model.SearchModelROISelectIndex == 0)
            {
                eumModelSearch = EumModelSearch.全图搜索;
                if (modelSearchRegion != null)
                    modelSearchRegion.Dispose();
                Model.BtnDrawModelSearchRegionEnable = false;
            }
            else if (Model.SearchModelROISelectIndex == 1)
            {
                eumModelSearch = EumModelSearch.局部搜索;
                Model.BtnDrawModelSearchRegionEnable = true;

            }
            BaseParam par = baseTool.GetParam();
            (par as MatchParam).ModelSearch = eumModelSearch;
            baseTool.SetParam(par);
            if (MatchTool.ObjectValided(imgBuf))
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
            }
        }

        /// <summary>
        /// 绘制模板区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrawModelRegion_Click()
        {
            if (!MatchTool.ObjectValided(imgBuf))
            {
                ShowTool.DispAlarmMessage("未获取图像！", 500, 20, 30);
                return;
            }
            ShowTool.setMouseStateOfNone();

            if (MessageBox.Show("准备创建模板ROI？", "Information",
                MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {


                //重新创建模板查找区域时，先将掩膜区域清除！
                if (MatchTool.ObjectValided(maskRegion))
                {
                    maskRegion.Dispose();
                    HOperatorSet.GenEmptyObj(out maskRegion);
                    //(baseParam as MatchParam).MaskROI = maskRegion;//模板掩膜区域
                }

                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");

                SetModelROIData _setModelROIData =
                        GuidePositioning_HDevelopExport.SetModelROI(ShowTool.HWindowsHandle, imgBuf);

                HOperatorSet.GenCrossContourXld(out HObject cross, _setModelROIData.modelOrigionRow,
                    _setModelROIData.modelOrigionColumn, 30, 0);

                MatchParam par = baseTool.GetParam() as MatchParam;
                par.ModelBaseRow = _setModelROIData.modelOrigionRow.D;
                par.ModelBaseCol = _setModelROIData.modelOrigionColumn.D;
                baseTool.SetParam(par);

                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);
                ShowTool.DispRegion(cross, "red");
                ShowTool.AddregionBuffer(cross, "red");
                ShowTool.DispRegion(_setModelROIData.modelSearchROI, "blue");
                ShowTool.AddregionBuffer(_setModelROIData.modelSearchROI, "blue");
                modelRegion = _setModelROIData.modelSearchROI.Clone();
                ShowTool.AddRightMenu();
                //releaseMouse();



            }
        }

        /// <summary>
        /// 绘制模板搜索区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrawModelSearchRegion_Click()
        {

            if (!MatchTool.ObjectValided(imgBuf))
            {
                ShowTool.DispAlarmMessage("未获取图像", 500, 20, 30);
                return;
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.DesignModel);
            ShowTool.setMouseStateOfNone();

            if (MessageBox.Show("准备创建模板搜索区域ROI？不设置则为全图搜索。", "Information",
                       MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);

                //limitMouse();
                ShowTool.Focus();
                ShowTool.RemoveRightMenu();

                HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "margin");
                HOperatorSet.SetColor(ShowTool.HWindowsHandle, "green");

                HOperatorSet.DrawRectangle1(ShowTool.HWindowsHandle, out HTuple hv_Row1, out HTuple hv_Column1,
                    out HTuple hv_Row2, out HTuple hv_Column2);
                modelSearchRegion.Dispose();
                HOperatorSet.GenRectangle1(out modelSearchRegion, hv_Row1, hv_Column1, hv_Row2,
                    hv_Column2);
                MatchParam par = baseTool.GetParam() as MatchParam;
                if (BaseTool.ObjectValided(modelSearchRegion))
                    par.InspectROI = modelSearchRegion;
                baseTool.SetParam(par);
                //releaseMouse();
                ShowTool.DispRegion(modelSearchRegion, "green");
                ShowTool.AddregionBuffer(modelSearchRegion, "green");
                //(baseParam as MatchParam).InspectROI = ModelSearchRegion.Clone();//模板搜索区域
                ShowTool.AddRightMenu();


            }
            //ShowTool.SetSystemPatten(EumSystemPattern.RunningModel);
        }

        /// <summary>
        /// 模板训练
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTranModel_Click()
        {
            MatchParam par = baseTool.GetParam() as MatchParam;
            par.InputImg = dataManage.imageBufDic[par.InputImageName].Clone();
            //par.InputImg = imgBuf;
            if (BaseTool.ObjectValided(modelSearchRegion))
                par.InspectROI = modelSearchRegion;
            par.ModelROI = modelRegion;
            par.MaskROI = maskRegion;
            par.StartAngle = Model.StartAngle;
            par.RangeAngle = Model.RangeAngle;
            par.ContrastValue = Model.Contrast;
            par.MatchScore = Model.MatchScore;
            par.MatchDownScale = Model.ScaleDownLimit;
            par.MatchUpScale = Model.ScaleUpLimit;

            Task.Run(() =>
            {

                baseTool.SetParam(par);
                bool flag = (baseTool as MatchTool).TemplateTraining(out HObject modelTrans);
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(imgBuf);

                ShowTool.DispRegion(par.ModelROI, "blue");
                ShowTool.AddregionBuffer(par.ModelROI, "blue");
                if (modelTrans == null ||
                       !BaseTool.ObjectValided(modelTrans))
                {

                    //log.Error("制作模板", string.Format("训练失败，图像：{0}，发生错误：{1}，模板轮廓无效!",
                    //              IsUsingPretreatment ? "imageBuffer" : "GrabImg", errmsg));
                    //MessageBox.Show(string.Format("训练失败，图像：{0}，发生错误：{1}，模板轮廓无效!",
                    //            IsUsingPretreatment ? "imageBuffer" : "GrabImg", errmsg));

                    ShowTool.DispAlarmMessage(string.Format("训练失败，图像：{0}，发生错误：模板轮廓无效!",
                                  imgBuf), 500, 20, 30);

                }
                else

                {

                    ShowTool.DispRegion(modelTrans, "green");
                    ShowTool.AddregionBuffer(modelTrans, "green");

                    // MessageBox.Show("训练成功!");
                }


            })
           .ContinueWith(t => {
               //Dispatcher.Invoke(new Action(() => {
              
               Model.BaseXText = (baseTool.GetParam() as MatchParam).ModelBaseCol.ToString("f3");
               Model.BaseYText = (baseTool.GetParam() as MatchParam).ModelBaseRow.ToString("f3");
               Model.BaseAngleText = ((baseTool.GetParam() as MatchParam).ModelBaseRadian * 180.0 / Math.PI).ToString("f3");
               //}));
           });
        }

        /// <summary>
        /// 掩膜设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMaskSet_Click()
        {


            //bool flag = (baseTool as MatchTool).TemplateTraining(out HObject modelTrans);
            BaseParam par = baseTool.GetParam();
            if (!MatchTool.ObjectValided((par as MatchParam).ModelImgOfWhole) ||
                            !MatchTool.ObjectValided((par as MatchParam).ModelContour))
            {
                ShowTool.DispAlarmMessage("请先创建合适的模板，再进行掩膜设置！", 500, 20, 30);

                //MessageBox.Show("请先创建合适的模板，再经行掩膜设置！", "Information",
                //         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PositionToolsLib.窗体.Views.FormTemplateMaking _frmTemplateMaking =
                  new PositionToolsLib.窗体.Views.FormTemplateMaking(imgBuf,
                   maskRegion, s_rootFolder, EumMakeType.Model, (par as MatchParam).ModelContour);
            TemplateMakingViewModel.This.SetModelMaskROIHandle += new EventHandler<ReturnData>(SetModelMaskROIEvent);
            _frmTemplateMaking.ShowDialog();

        }

        void SetModelMaskROIEvent(object sender, ReturnData e)
        {
            if (e.type == EumMakeType.Model)
                maskRegion = e.region;
            else
            {
                HObject mask = e.region;
                HOperatorSet.CopyObj(mask, out maskRegion, 1, -1);
            }

        }

        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as MatchParam).StartAngle = Model.StartAngle;
            (par as MatchParam).RangeAngle = Model.RangeAngle;
            (par as MatchParam).ContrastValue = Model.Contrast;
            (par as MatchParam).MatchScore = Model.MatchScore;
            (par as MatchParam).MatchDownScale = Model.ScaleDownLimit;
            (par as MatchParam).MatchUpScale = Model.ScaleUpLimit;
            if (BaseTool.ObjectValided(modelSearchRegion))
                (par as MatchParam).InspectROI = modelSearchRegion;
            (par as MatchParam).ModelSearch = eumModelSearch;
            baseTool.SetParam(par);
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
        }

        /// <summary>
        /// 手动测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModelTest_Click()
        {
            Model.DgResultOfMatchList.Clear();
            BaseParam par = baseTool.GetParam();
            (par as MatchParam).StartAngle = Model.StartAngle;
            (par as MatchParam).RangeAngle = Model.RangeAngle;
            (par as MatchParam).ContrastValue = Model.Contrast;
            (par as MatchParam).MatchScore = Model.MatchScore;
            (par as MatchParam).MatchDownScale = Model.ScaleDownLimit;
            (par as MatchParam).MatchUpScale = Model.ScaleUpLimit;
            if (modelSearchRegion != null)
                (par as MatchParam).InspectROI = modelSearchRegion;
            (par as MatchParam).ModelSearch = eumModelSearch;
            baseTool.SetParam(par);
            RunResult rlt = (baseTool as MatchTool).Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispConcatedObj((par as MatchParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as MatchParam).OutputImg, EumCommonColors.green);
                ShowTool.DispRegion((par as MatchParam).InspectROI, "blue");
                ShowTool.AddregionBuffer((par as MatchParam).InspectROI, "blue");
                ShowTool.DispMessage("OK", 10, width - 500, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - 500, "green", 100);
                ShowTool.DispMessage(string.Format("位置X:{0:f3},Y:{1:f3}",
                    (par as MatchParam).MatchResultColumns.TupleSelect(0).D,
                    (par as MatchParam).MatchResultRows.TupleSelect(0).D), 10, 10, "green", 16);
                ShowTool.AddTextBuffer(string.Format("位置X:{0:f3},Y:{1:f3}",
                    (par as MatchParam).MatchResultColumns.TupleSelect(0).D,
                    (par as MatchParam).MatchResultRows.TupleSelect(0).D), 10, 10, "green", 16);
                ShowTool.DispMessage(string.Format("角度:{0:f3}",
                   (par as MatchParam).MatchResultRadians.TupleSelect(0).TupleDeg().D
                   ), 100, 10, "green", 16);//弧度转角度
                ShowTool.AddTextBuffer(string.Format("角度:{0:f3}",
                   (par as MatchParam).MatchResultRadians.TupleSelect(0).TupleDeg().D
                   ), 100, 10, "green", 16);//弧度转角度
                ShowTool.DispMessage(string.Format("得分:{0:f3}",
                    (par as MatchParam).MatchResultScores.TupleSelect(0).D
                   ), 200, 10, "green", 16);
                ShowTool.AddTextBuffer(string.Format("得分:{0:f3}",
                   (par as MatchParam).MatchResultScores.TupleSelect(0).D
                  ), 200, 10, "green", 16);
                Model.DgResultOfMatchList.Clear();
                Model.DgResultOfMatchList.Add(new DgResultOfMatch(1,
                    (par as MatchParam).MatchResultScores.TupleSelect(0).D,
                      (par as MatchParam).MatchResultColumns.TupleSelect(0).D,
               (par as MatchParam).MatchResultRows.TupleSelect(0).D,             
             (par as MatchParam).MatchResultRadians.TupleSelect(0).TupleDeg().D));

            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispRegion((par as MatchParam).InspectROI, "blue");
                ShowTool.AddregionBuffer((par as MatchParam).InspectROI, "blue");
                ShowTool.DispMessage("NG", 10, width - 500, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - 500, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
            }

        }

      
    }
}
