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
    public class BlobViewModel : BaseViewModel
    {
        public static BlobViewModel This { get; set; }
        public BlobModel Model { get; set; }
        List<ParticleFeaturesData> particleFeaturesDataList = new List<ParticleFeaturesData>();//表格数据集合
        HObject inspectROI = null;//检测区域
        HTuple matrix2D = null;//位置偏移补正
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase UsePosiCorrectCheckedCommand { get; set; }
        public CommandBase MatrixSelectionChangedCommand { get; set; }
        public CommandBase NewMenuItemClickCommand { get; set; }
        public CommandBase DelMenuItemClickCommand { get; set; }
        public CommandBase DrawRegionClickCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }
        public CommandBase DgMouseDoubleClickCommand { get; set; }
        public BlobViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new BlobModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
                           
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

            NewMenuItemClickCommand = new CommandBase();
            NewMenuItemClickCommand.DoExecute = new Action<object>((o) => 新增toolStripMenuItem_Click());
            NewMenuItemClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DelMenuItemClickCommand = new CommandBase();
            DelMenuItemClickCommand.DoExecute = new Action<object>((o) => 删除toolStripMenuItem_Click());
            DelMenuItemClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DrawRegionClickCommand = new CommandBase();
            DrawRegionClickCommand.DoExecute = new Action<object>((o) => btnDrawRegion_Click());
            DrawRegionClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

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
            if(!dataManage.imageBufDic.ContainsKey(Model.SelectImageName)) return;
            if (!BlobTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as BlobParam).InputImageName = Model.SelectImageName;
        }
        void GetItemList()
        {
            Model.ItemList.Clear();
            List<string> Features = Enum.GetNames(typeof(EumParticleFeatures)).ToList<string>();
            foreach (var s in Features)
                Model.ItemList.Add(s);
        }
        /// <summary>
        /// 结果显示表格双击
        /// </summary>
        /// <param name="o"></param>
        private void dataGridViewEx1_DoubleClick(object o)
        {

            int index = Model.DgResultOfBlobSelectIndex;
            if (index < 0 || index > Model.DgResultOfBlobList.Count) return;
            BaseParam par = baseTool.GetParam();
            HObject regions = (par as BlobParam).ResultRegions;
            if (!BaseTool.ObjectValided(regions)) return;

            HOperatorSet.CountObj(regions,out HTuple nums);
            if (index + 1 > nums) return;
            HOperatorSet.SelectObj(regions,out HObject objectSelected,index+1);
            ShowTool.DispRegion(objectSelected, "red");
            //ShowTool.AddregionBuffer(objectSelected, "blue");     
        }
        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData()
        {
            BaseParam par = baseTool.GetParam();
            //检测区域
            if (BaseTool.ObjectValided((par as BlobParam).InspectROI))
                HOperatorSet.CopyObj((par as BlobParam).InspectROI, out inspectROI, 1, -1);
    
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as BlobParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
            Model.SelectImageName = (par as BlobParam).InputImageName;


            foreach (var s in dataManage.matrixBufDic)
                Model.MatrixList.Add(s.Key);
            string matrixName = (par as BlobParam).MatrixName;
            int index2 = Model.MatrixList.IndexOf(matrixName);
            Model.SelectMatrixIndex = index2;

            particleFeaturesDataList = (par as BlobParam).ParticleFeaturesDataList;
            if (particleFeaturesDataList == null) return;
            Model.DgDataOfBlobList.Clear();        
            GetItemList();
            foreach (var s in particleFeaturesDataList)
            {             
                Model.DgDataOfBlobList.Add(s);

            }
            if (Model.DgDataOfBlobList.Count > 0)
                Model.DgDataSelectIndex = Model.DgDataOfBlobList.Count - 1;

           Model.NumGrayMin = (par as BlobParam).GrayMin;
           Model.NumGrayMax = (par as BlobParam).GrayMax;

            Model.UsePosiCorrectChecked = (par as BlobParam).UsePosiCorrect;
            if ((par as BlobParam).UsePosiCorrect)
               Model.MatrixEnable = true;
            else
                Model.MatrixEnable = false;
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
                    (par as BlobParam).MatrixName = Model.SelectMatrixName;

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
            if (Model.SelectMatrixIndex == -1) return;
            matrix2D = dataManage.matrixBufDic[Model.SelectMatrixName];
            BaseParam par = baseTool.GetParam();
            (par as BlobParam).MatrixName = Model.SelectMatrixName;
        }

        private void 新增toolStripMenuItem_Click()
        {
            //新增时就更新表格    
            List<string> name_list = Enum.GetNames(typeof(EumParticleFeatures)).ToList<string>();
            ParticleFeaturesData dat = new ParticleFeaturesData(false, "area", 0, 999);
            particleFeaturesDataList.Add(dat);
            //GetItemList();
            Model.DgDataOfBlobList.Add(dat);
            Model.DgDataSelectIndex = Model.DgDataOfBlobList.Count - 1;
          
        }

        private void 删除toolStripMenuItem_Click()
        {
            if (Model.DgDataOfBlobList.Count > 0)
            {
                int index = Model.DgDataSelectIndex;
                if (index < 0) return;           
                particleFeaturesDataList.RemoveAt(index);
                Model.DgDataOfBlobList.RemoveAt(index);
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

            if (MessageBox.Show("准备创建Blob检测区域？", "Information",
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

                //HOperatorSet.DrawRectangle1(ShowTool.HWindowsHandle, out HTuple hv_Row1, out HTuple hv_Column1,
                //    out HTuple hv_Row2, out HTuple hv_Column2);
                //glueSearchRegion.Dispose();
                //HOperatorSet.GenRectangle1(out glueSearchRegion, hv_Row1, hv_Column1, hv_Row2,
                //    hv_Column2);

                HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 3);
                //HOperatorSet.DrawRegion(out inspectROI, ShowTool.HWindowsHandle);
                HOperatorSet.DrawRectangle1(ShowTool.HWindowsHandle, out HTuple row1, out HTuple column1,
                       out HTuple row2, out HTuple column2);
               
                HOperatorSet.GenRectangle1(out inspectROI, row1, column1, row2, column2);

                HOperatorSet.SetLineWidth(ShowTool.HWindowsHandle, 1);
                //releaseMouse();
                ShowTool.DispRegion(inspectROI, "blue");
                ShowTool.AddregionBuffer(inspectROI, "blue");
                ShowTool.AddRightMenu();
                Model.BtnDrawRegionEnable = true;
            }
            //ShowTool.SetSystemPatten(EumSystemPattern.RunningModel);
        }
        /// <summary>
        /// 图像结果显示参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            particleFeaturesDataList.Clear();
            int count = Model.DgDataOfBlobList.Count;

            for (int i = 0; i < count; i++)
            {
                bool isUse = Model.DgDataOfBlobList[i].Use;
                string feature = Model.DgDataOfBlobList[i].Item;
                int min = Model.DgDataOfBlobList[i].LimitDown;
                int max = Model.DgDataOfBlobList[i].LimitUp;
                ParticleFeaturesData dat = new ParticleFeaturesData(isUse, feature, min, max);
                particleFeaturesDataList.Add(dat);
            }
            BaseParam par = baseTool.GetParam();
            (par as BlobParam).ParticleFeaturesDataList = particleFeaturesDataList;
            (par as BlobParam).GrayMin = Model.NumGrayMin;
            (par as BlobParam).GrayMax = Model.NumGrayMax;
            (par as BlobParam).InspectROI = inspectROI.Clone();
            (par as BlobParam).UsePosiCorrect = Model.UsePosiCorrectChecked;
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
            particleFeaturesDataList.Clear();
            int count = Model.DgDataOfBlobList.Count;

            for (int i = 0; i < count; i++)
            {
                bool isUse = Model.DgDataOfBlobList[i].Use;
                string feature = Model.DgDataOfBlobList[i].Item;
                int min = Model.DgDataOfBlobList[i].LimitDown;
                int max = Model.DgDataOfBlobList[i].LimitUp;
                ParticleFeaturesData dat = new ParticleFeaturesData(isUse, feature, min, max);
                particleFeaturesDataList.Add(dat);
            }
            BaseParam par = baseTool.GetParam();
            (par as BlobParam).ParticleFeaturesDataList = particleFeaturesDataList;
            (par as BlobParam).GrayMin = Model.NumGrayMin;
            (par as BlobParam).GrayMax = Model.NumGrayMax;
            (par as BlobParam).InspectROI = inspectROI.Clone();
            (par as BlobParam).UsePosiCorrect = Model.UsePosiCorrectChecked;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {

                ShowTool.DispConcatedObj((par as BlobParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as BlobParam).OutputImg, EumCommonColors.green);

                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                //更新结果表格数据
                UpdateResultView((par as BlobParam).BlobFeaturesResult);
                if ((par as BlobParam).BlobFeaturesResult.Count > 0)
                {
                    foreach (var s in (par as BlobParam).BlobFeaturesResult)
                    {
                        ShowTool.DispMessage(s.area.ToString("f3"), s.row, s.column, "green", 16);
                        ShowTool.AddTextBuffer(s.area.ToString("f3"), s.row, s.column, "green", 16);
                    }

                }
            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispMessage("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
            }
     
            ShowTool.DispRegion((par as BlobParam).ResultInspectROI, "blue");
            ShowTool.AddregionBuffer((par as BlobParam).ResultInspectROI, "blue");
        }
        void UpdateResultView(List<StuBlobFeaturesResult> blobFeaturesResult)
        {
            Model.DgResultOfBlobList.Clear();
            int i = 0;
            foreach (var s in blobFeaturesResult)
            {
                i++;
                BlobFeaturesResultData dat = new BlobFeaturesResultData(i,s.column, s.row, s.area);
                Model.DgResultOfBlobList.Add(dat);
            }
        }
    }
}
