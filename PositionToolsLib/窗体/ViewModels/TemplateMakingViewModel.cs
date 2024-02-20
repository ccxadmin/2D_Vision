using ControlShareResources.Common;
using FilesRAW.Common;
using FunctionLib.Location;
using HalconDotNet;
using PositionToolsLib.窗体.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisionShowLib.UserControls;

namespace PositionToolsLib.窗体.ViewModels
{
    public class TemplateMakingViewModel
    {
        EumworkType eumworkType = EumworkType.擦拭;
        HObject MarkRegion = null;
        bool IsStartDrawMask = false;
        int panSize = 1;  //画笔大小
        EumPanType eumPanType = EumPanType.圆; //画笔类型
        EumMakeType makeType = EumMakeType.Model;
        string rootPath = AppDomain.CurrentDomain.BaseDirectory;
        HObject GrabImg = null;
        HObject originalContour = null;
        HObject originalMaskROI = null;
        public static TemplateMakingViewModel This { get; set; }
        public TemplateMakingModel Model { get; set; }
        //图像显示工具
        public VisionShowTool ShowTool { get; set; }
        HObject imgBuf = null;//图像缓存
        public EventHandler<ReturnData> SetModelMaskROIHandle = null;

        public CommandBase PanTypeSelectionChangedCommand { get; set; }
        public CommandBase BarPanSizeValueChangedCommand { get; set; }
        public CommandBase TxbBarValueKeyDownCommand { get; set; }
        public CommandBase OnClosingCommand { get; set; }
        public CommandBase SaveBtnClickCommand { get; set; }
        public CommandBase ResetBtnClickCommand { get; set; }
        public CommandBase RdbtnCheckedChangedCommand { get; set; }

        public TemplateMakingViewModel(HObject img, HObject _maskROI,
                                    string _rootPath, EumMakeType
            type = EumMakeType.Model, HObject modelcontour = null)
        {
            This = this;
            Model = new TemplateMakingModel();

            makeType = type;
            if(!string.IsNullOrEmpty(_rootPath))
               rootPath = _rootPath;
            //画笔大小
            panSize = int.Parse(GeneralUse.ReadValue("画笔大小", "模板配置", "config", "1", rootPath + "\\Config"));
            Model.PanValue = panSize.ToString();
            Model.PanSize = panSize;
            //画笔类型
            string penType = GeneralUse.ReadValue("画笔类型", "模板配置", "config", "圆", rootPath + "\\Config");
            if (penType != "圆" && penType != "矩形")
            {
                // MessageBox.Show("画笔类型错误，已默认被修改为圆类型！");
                penType = "圆";
            }
            eumPanType = (EumPanType)Enum.Parse(typeof(EumPanType), penType);
            Model.CobxPanTypeSelectName = penType;

            HOperatorSet.GenEmptyObj(out originalContour);
            if (modelcontour != null)
            {
                originalContour.Dispose();
                HOperatorSet.CopyObj(modelcontour, out originalContour, 1, -1);
            }

            HOperatorSet.GenEmptyObj(out originalMaskROI);
            if (GuidePositioning_HDevelopExport.ObjectValided(_maskROI))
            {
                originalMaskROI.Dispose();
                HOperatorSet.CopyObj(_maskROI, out originalMaskROI, 1, -1);
            }
            HOperatorSet.GenEmptyObj(out MarkRegion);
            if (GuidePositioning_HDevelopExport.ObjectValided(originalMaskROI))
            {
                MarkRegion.Dispose();
                HOperatorSet.CopyObj(originalMaskROI, out MarkRegion, 1, -1);
            }
            Model.CobxPanTypeSelectIndex = 0;
            ///////////////////////////////////////////////////////////////////////

            PanTypeSelectionChangedCommand = new CommandBase();
            PanTypeSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxPanType_SelectedIndexChanged(o));
            PanTypeSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            BarPanSizeValueChangedCommand = new CommandBase();
            BarPanSizeValueChangedCommand.DoExecute = new Action<object>((o) => BarPanSize_ValueChanged(o));
            BarPanSizeValueChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TxbBarValueKeyDownCommand = new CommandBase();
            TxbBarValueKeyDownCommand.DoExecute = new Action<object>((o) => txbBarValue_KeyDown(o));
            TxbBarValueKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            OnClosingCommand = new CommandBase();
            OnClosingCommand.DoExecute = new Action<object>((o) => frmTemplateMaking_FormClosing());
            OnClosingCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveBtnClickCommand = new CommandBase();
            SaveBtnClickCommand.DoExecute = new Action<object>((o) => btnSave_Click());
            SaveBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ResetBtnClickCommand = new CommandBase();
            ResetBtnClickCommand.DoExecute = new Action<object>((o) => btnReset_Click());
            ResetBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            RdbtnCheckedChangedCommand = new CommandBase();
            RdbtnCheckedChangedCommand.DoExecute = new Action<object>((o) => rdbtn_CheckedChanged(o));
            RdbtnCheckedChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            //图像控件      
            ShowTool = new VisionShowTool();
            HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "fill");
            ShowTool.Disp_MouseDownHandle += uCvisionLayout1_MouseDown;
            ShowTool.Disp_MouseUpHandle += uCvisionLayout1_MouseUp;
            //ShowTool.Disp_MouseMoveHandle += uCvisionLayout1_MouseMove;
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            ShowTool.DispImage(img);
            ShowTool.D_HImage = GrabImg = img;
            ShowTool.DispRegion(originalContour, "green");
            ShowTool.AddregionBuffer(originalContour, "green");
            ShowTool.DispRegion(originalMaskROI, "orange", EumDrawModel.fill);
            ShowTool.AddregionBuffer(originalMaskROI, "orange");
            ShowTool.RemoveRightMenu();         
            ShowTool.SetScale();        
            rdbtn_CheckedChanged(null);

        }

        private void uCvisionLayout1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "fill");
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                GuidePositioning_HDevelopExport.isContinue_drawing = true;
                ////////MouseMove----->MouseDown
                {
                    if (!IsStartDrawMask)
                        return;
                    if (eumworkType == EumworkType.擦拭)
                    {
                        if (eumPanType == EumPanType.圆)
                            GuidePositioning_HDevelopExport.AddRegion2(GrabImg, ref MarkRegion,
                                     ShowTool.HWindowsHandle, panSize, true, originalContour);
                        else
                            GuidePositioning_HDevelopExport.AddRegion2(GrabImg, ref MarkRegion,
                                      ShowTool.HWindowsHandle, panSize, false, originalContour);

                    }
                    else if (eumworkType == EumworkType.清除)
                    {
                        if (!GuidePositioning_HDevelopExport.ObjectValided(MarkRegion))
                        {
                            //MessageBox.Show("掩膜区域为空，无需清除!");
                            return;
                        }
                        else
                        {
                            HObject temregoion = null;
                            HOperatorSet.GenEmptyObj(out temregoion);
                            temregoion.Dispose();
                            HOperatorSet.CopyObj(MarkRegion, out temregoion, 1, -1);
                            if (eumPanType == EumPanType.圆)
                                GuidePositioning_HDevelopExport.Subregion2(GrabImg, temregoion, ref MarkRegion,
                                          ShowTool.HWindowsHandle, panSize, true, originalContour);
                            else
                                GuidePositioning_HDevelopExport.Subregion2(GrabImg, temregoion, ref MarkRegion,
                                           ShowTool.HWindowsHandle, panSize, false, originalContour);
                            temregoion.Dispose();
                        }
                    }
                    else
                    { }
                }

            }
        }

        private void uCvisionLayout1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GuidePositioning_HDevelopExport.isContinue_drawing = false;
            if (IsStartDrawMask)
            {
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(ShowTool.D_HImage);             
                ShowTool.DispRegion(originalContour, "green");
                ShowTool.AddregionBuffer(originalContour, "green");
                ShowTool.DispRegion(MarkRegion, "orange", EumDrawModel.fill);
                ShowTool.AddregionBuffer(MarkRegion, "orange");
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

        private void cobxPanType_SelectedIndexChanged(object o)
        {
            string value = Model.CobxPanTypeSelectName;
            if (value == "圆")
                eumPanType = EumPanType.圆;
            else
                eumPanType = EumPanType.矩形;

            //画笔类型
            string penType = Model.CobxPanTypeSelectName;
            GeneralUse.WriteValue("画笔类型", "模板配置", penType, "config", rootPath + "\\Config");
        }

        private void BarPanSize_ValueChanged(object o)
        {
            panSize = Model.PanSize;
            Model.PanValue = panSize.ToString();
            GeneralUse.WriteValue("画笔大小", "模板配置", panSize.ToString(), "config", rootPath + "\\Config");
        }

        private void txbBarValue_KeyDown(object obj)
        {
            KeyEventArgs args = (KeyEventArgs)obj;
            if (args.Key == Key.Enter)
            {
                panSize = int.Parse(Model.PanValue);
                if (panSize > 100 || panSize < 1)
                {
                    // MessageBox.Show("值范围错误！");
                    return;
                }
                Model.PanSize = panSize;
                GeneralUse.WriteValue("画笔大小", "模板配置", panSize.ToString(), "config", rootPath + "\\Config");
            }


        }

        private void frmTemplateMaking_FormClosing()
        {
            IsStartDrawMask = false;
            //画笔大小
            GeneralUse.WriteValue("画笔大小", "模板配置", panSize.ToString(), "config", rootPath + "\\Config");
            //画笔类型
            string penType = Model.CobxPanTypeSelectName;
            GeneralUse.WriteValue("画笔类型", "模板配置", penType, "config", rootPath + "\\Config");

            //  SetModelMaskROIHandle?.Invoke(MarkRegion, null);
        }
        /// <summary>
        /// 保存
        /// </summary>
        private void btnSave_Click()
        {
            SetModelMaskROIHandle?.Invoke(null, new ReturnData(MarkRegion, makeType));
        }
        /// <summary>
        /// 绘制重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click()
        {
            MarkRegion.Dispose();
            HOperatorSet.GenEmptyObj(out MarkRegion);
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(GrabImg);
            ShowTool.DispRegion(originalContour, "green");
            ShowTool.AddregionBuffer(originalContour, "green");
            //HOperatorSet.SetDraw(ShowTool.HWindowsHandle, "fill");
        }

        /// <summary>
        /// 手自动区域设置切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbtn_CheckedChanged(object o)
        {
            if (!GuidePositioning_HDevelopExport.ObjectValided(GrabImg))
                return;
            IsStartDrawMask = true;
            if (Model.WorkType == Models.EumworkType.擦拭)
                eumworkType = EumworkType.擦拭;
            else
                eumworkType = EumworkType.清除;
        }
    }
}
