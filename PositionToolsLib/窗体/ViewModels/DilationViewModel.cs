﻿using ControlShareResources.Common;
using PositionToolsLib.参数;
using PositionToolsLib.窗体.Models;
using PositionToolsLib.工具;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.ViewModels
{
    public class DilationViewModel : BaseViewModel
    {
        public static DilationViewModel This { get; set; }
        public DilationModel Model { get; set; }

        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }

        public DilationViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new DilationModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
       
            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

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
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);

            string imageName = (par as DilationParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;

            int maskWidthValue = (par as DilationParam).MaskWidth;
            int index2 = Model.MaskWidthList.IndexOf(maskWidthValue);
            Model.SelectMaskWidthIndex = index2;

            int maskHeightValue = (par as DilationParam).MaskHeight;
            int index3 = Model.MaskHeightList.IndexOf(maskHeightValue);
            Model.SelectMaskHeightIndex = index3;

            Model.SelectImageName = (par as DilationParam).InputImageName;
            Model.SelectMaskWidth = (par as DilationParam).MaskWidth;
            Model.SelectMaskHeight = (par as DilationParam).MaskHeight;
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
            (par as DilationParam).InputImageName = Model.SelectImageName;
        }

        /// <summary>
        /// 图像闭运算测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as DilationParam).MaskWidth = Model.SelectMaskWidth;
            (par as DilationParam).MaskHeight = Model.SelectMaskHeight;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispImage((par as DilationParam).OutputImg);
                ShowTool.D_HImage = (par as DilationParam).OutputImg;
                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);

            }
            else
            {
                ShowTool.DispImage(imgBuf);
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
            (par as DilationParam).MaskWidth = Model.SelectMaskWidth;
            (par as DilationParam).MaskHeight = Model.SelectMaskHeight;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
        }

    }
}
