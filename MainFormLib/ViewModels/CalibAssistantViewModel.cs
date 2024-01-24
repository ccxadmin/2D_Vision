using ControlShareResources.Common;
using HalconDotNet;
using MainFormLib.Models;
using OSLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionShowLib.UserControls;
using static System.Windows.Forms.ImageList;
using System.IO;
using FilesRAW.Common;
using System.Collections.ObjectModel;

namespace MainFormLib.ViewModels
{
    public class CalibAssistantViewModel 
    {
        public static CalibAssistantViewModel This { get; set; }
        public CalibAssistantModel Model { get; set; }
        public VisionShowTool ShowTool { get; set; }
        public CommandBase GenBoardFileBtnClickCommand { get; set; }
        public CommandBase DgMouseDoubleClickCommand { get; set; }
        public CommandBase DgDragDropCommand { get; set; }
        public CommandBase DeleteMenuItemClickCommand { get; set; }
        public CommandBase ClearMenuItemClickCommand { get; set; }
        public CommandBase DgDragEnterCommand { get; set; }
        public CommandBase ReadyCalibClickCommand { get; set; }
        public CommandBase CalibFinishClickCommand { get; set; }
        public CommandBase ManualCalibClickCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        //public CommandBase TestButClickCommand { get; set; }
        HObject imgBuf = null;//图像缓存     
        CamParams camParams = new CamParams();//相机参数
        CalTab calTab = new CalTab();//标定板参数
        HTuple hv_CamParam;//相机内参
        HTuple hv_WorldPose;//相机位姿
        double RMS = 0;//标定误差系数
        HTuple hv_CalibDataID = null;
        List<int> DoneStepList = new List<int>();
        int totalStep = 0;
        int i = 0;
        private string currCalibName = "default";//标定文件夹名称
        private string rootFolder = Environment.CurrentDirectory; //根目录
        //图像字典
        Dictionary<string, HObject> ImagesDic = new Dictionary<string, HObject>();
        public CalibAssistantViewModel(string _rootFolder,string _calibName = "default") 
        {
            if(Directory.Exists(_rootFolder))
                rootFolder = _rootFolder;
            currCalibName = _calibName;
            HOperatorSet.GenEmptyObj(out imgBuf);
            This = this;
            Model = new CalibAssistantModel();
            //图像控件      
            ShowTool = new VisionShowTool();
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);

            GenBoardFileBtnClickCommand = new CommandBase();
            GenBoardFileBtnClickCommand.DoExecute = new Action<object>((o) => btnGenBoardFile_Click());
            GenBoardFileBtnClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DgMouseDoubleClickCommand = new CommandBase();
            DgMouseDoubleClickCommand.DoExecute = new Action<object>((o) => dgCalibImageInfo_DoubleClick());
            DgMouseDoubleClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DeleteMenuItemClickCommand = new CommandBase();
            DeleteMenuItemClickCommand.DoExecute = new Action<object>((o) => dgCalibImageInfo_DeleteMenuItemClick());
            DeleteMenuItemClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ClearMenuItemClickCommand = new CommandBase();
            ClearMenuItemClickCommand.DoExecute = new Action<object>((o) => dgCalibImageInfo_ClearMenuItemClick());
            ClearMenuItemClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            DgDragDropCommand = new CommandBase();
            DgDragDropCommand.DoExecute = new Action<object>((o) => dgCalibImageInfo_DragDrop(o));
            DgDragDropCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DgDragEnterCommand = new CommandBase();
            DgDragEnterCommand.DoExecute = new Action<object>((o) => dgCalibImageInfo_DragEnter(o));
            DgDragEnterCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ReadyCalibClickCommand = new CommandBase();
            ReadyCalibClickCommand.DoExecute = new Action<object>((o) => btnReadyCalib_Click());
            ReadyCalibClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            CalibFinishClickCommand = new CommandBase();
            CalibFinishClickCommand.DoExecute = new Action<object>((o) => btnCalibFinish_Click());
            CalibFinishClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ManualCalibClickCommand = new CommandBase();
            ManualCalibClickCommand.DoExecute = new Action<object>((o) => btnManualCalib_Click());
            ManualCalibClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            //TestButClickCommand = new CommandBase();
            //TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            //TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            LoadCalibAssistantParam();
            showCalibAssistantData();
        }
        /// <summary>
        /// 设置当前标定助手文件夹名称
        /// </summary>
        /// <param name="name"></param>
        public void SetCalibName(string name)
        {
            currCalibName = name;
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        /// <param name="_rootFolder"></param>
        /// <param name="_calibName"></param>
        public void ReLoadData(string _rootFolder, string _calibName = "default")
        {
            if (Directory.Exists(_rootFolder))
                rootFolder = _rootFolder;
            currCalibName = _calibName;
            LoadCalibAssistantParam();
            showCalibAssistantData();
        }
        /// <summary>
        /// 加载参数
        /// </summary>
        /// <returns></returns>
        bool LoadCalibAssistantParam()
        {
            try
            {
                string filePath = rootFolder + "\\标定矩阵\\标定助手\\"+ currCalibName;
                if (!Directory.Exists(rootFolder + "\\标定矩阵"))
                    Directory.CreateDirectory(rootFolder + "\\标定矩阵");
                if (!Directory.Exists(rootFolder + "\\标定矩阵\\标定助手"))
                    Directory.CreateDirectory(rootFolder + "\\标定矩阵\\标定助手");           
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                hv_CamParam = CalibAssistantTool.ReadCalibData(filePath);
                hv_WorldPose = CalibAssistantTool.ReadCalibPose(filePath);
                //if (hv_CamParam == null || hv_CamParam.Length <= 0)
                //    Appentxt("相机内参加载失败！");
                camParams = GeneralUse.ReadSerializationFile<CamParams>(filePath + "\\相机参数");
                calTab = GeneralUse.ReadSerializationFile<CalTab>(filePath + "\\标定板参数");
                RMS = double.Parse(GeneralUse.ReadValue("标定", "误差系数", "config", "0", filePath));
                Model.DgImageCorrectInfoList = GeneralUse.ReadSerializationFile<ObservableCollection<ImageCorrectInfo>>(filePath + "\\图像信息");
                string imageFile = filePath + "\\imags";
                DirectoryInfo di = new DirectoryInfo(imageFile);
                FileInfo[] fi = di.GetFiles();

                foreach (var s in fi)
                {
                    string name = s.FullName;
                    if (s.Extension.Equals(".jpg"))
                    {
                        HOperatorSet.ReadImage(out HObject imgBuf, s.FullName);
                        ImagesDic.Add(s.Name.Replace(".jpg", ""), imgBuf);
                    }
                }

            }
            catch
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 数据显示
        /// </summary>
        void showCalibAssistantData()
        {
            //相机参数
           Model.TxbF = camParams.F;
           Model.TxbSx = camParams.Sx;
           Model.TxbSy= camParams.Sy;
           Model.TxbCx = camParams.Cx;
           Model.TxbCy = camParams.Cy;
           Model.TxbWidth = camParams.Width;
           Model.TxbHeight = camParams.Height;


            //标定板参数
           Model.TxbBoardXNum = calTab.XNum;
           Model.TxbBoardYNum = calTab.YNum;
           Model.TxbBoardMarkDis = calTab.MarkDist;
           Model.TxbBoardMarkDisRotia = calTab.DiameterRatio;
            Model.TxbBoardFilePath = calTab.CalPlateDescr;
            Model.TxbBoardThick = calTab.Thickness;
            //RMS
           Model.TxbCalibRMS = RMS;
     
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
        }

        /// <summary>
        /// 标定板描述文件生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenBoardFile_Click()
        {
            if (!Directory.Exists(rootFolder + "\\标定矩阵"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵");
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\标定助手"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\标定助手");
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\标定助手\\"+ currCalibName))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\标定助手\\"+ currCalibName);           
            string filePath = rootFolder + "\\标定矩阵\\标定助手\\" + currCalibName;
            calTab.XNum = Model.TxbBoardXNum;
            calTab.YNum = Model.TxbBoardYNum;
            calTab.MarkDist = Model.TxbBoardMarkDis;
            calTab.DiameterRatio = Model.TxbBoardMarkDisRotia;
            calTab.Thickness= Model.TxbBoardThick ;
            calTab.CalPlatePSFile = filePath + "\\标定板PS文件.ps";
            calTab.CalPlateDescr = filePath + "\\标定板描述文件.descr";
            bool flag = CalibAssistantTool.gen_caltab_file(calTab);
            if (!flag)
            {
                Model.TxbBoardFilePath = "ERROR";         
                MessageBox.Show("标定板文件生成失败！");
            }
            else
                Model.TxbBoardFilePath = calTab.CalPlateDescr;


        }

        /// <summary>
        /// 表格拖
        /// </summary>
        /// <param name="o"></param>
        void dgCalibImageInfo_DragEnter(object o)
        {
            DragEventArgs e = (DragEventArgs)o;
            if (!e.Data.GetDataPresent(typeof(HObject)))
                e.Effects = DragDropEffects.None;
            else
                e.Effects = DragDropEffects.Copy;

        }
        /// <summary>
        /// 表格放
        /// </summary>
        /// <param name="o"></param>
        void dgCalibImageInfo_DragDrop(object o)
        {
            DragEventArgs e = (DragEventArgs)o;
            HObject tem = (HObject)e.Data.GetData(typeof(HObject));
            //判断图像是否有效
            if (ObjectValided(tem))
            {
                //新增时就更新表格
              
                string temName = DateTime.Now.ToString("HH_mm_ss");//时分秒
                ImageCorrectInfo dat = new ImageCorrectInfo(true, i, temName,  "未标定");
                int index = Model.DgImageCorrectInfoList.ToList().FindIndex(t => t.Name.Equals(temName));
                if (index < 0)
                {
                    Model.DgImageCorrectInfoList.Add(dat);
                    index = Model.DgImageCorrectInfoList.Count - 1;
                }
                else
                    Model.DgImageCorrectInfoList[index] = dat;
       
                Model.DgDataSelectIndex = Model.DgImageCorrectInfoList.Count - 1;
                //图像字典缓存
                if (ImagesDic.ContainsKey(temName))
                    ImagesDic[temName] = tem;
                else
                    ImagesDic.Add(temName, tem);
                i++;
            }

        }
        /// <summary>
        /// 表格双击
        /// </summary>
        void dgCalibImageInfo_DoubleClick()
        {
         
            int index = Model.DgDataSelectIndex;
            if (index < 0 ||
               index >= Model.DgImageCorrectInfoList.Count) return;
            string temName = Model.DgImageCorrectInfoList[index].Name;
            HObject temImage = null;
            HOperatorSet.GenEmptyObj(out temImage);
            temImage.Dispose();
            if (ImagesDic.ContainsKey(temName))
            {
                HOperatorSet.CopyObj(ImagesDic[temName], out temImage, 1, 1);
                ShowTool.DispImage(temImage);
                imgBuf.Dispose();
                imgBuf = ShowTool.D_HImage= temImage;
            }
            else
                MessageBox.Show(string.Format("图像{0}不存在", temName));
        }
        /// <summary>
        /// 表格项删除
        /// </summary>
        void dgCalibImageInfo_DeleteMenuItemClick()
        {
            int index = Model.DgDataSelectIndex;
            if (index < 0 ||
               index >= Model.DgImageCorrectInfoList.Count) return;

            string name = Model.DgImageCorrectInfoList[index].Name;

            //图像信息删除
            if (ImagesDic.ContainsKey(name))
                ImagesDic.Remove(name);

            Model.DgImageCorrectInfoList.RemoveAt(index);

        }
        /// <summary>
        /// 表格项清除
        /// </summary>
        void dgCalibImageInfo_ClearMenuItemClick()
        {         
            ImagesDic.Clear();
            Model.DgImageCorrectInfoList.Clear();
            i = 0;
        }


        /// <summary>
        ///  手动标定
        /// </summary>
        void btnManualCalib_Click()
        {
           
            int index = Model.DgDataSelectIndex;        
            if (index < 0 || index > Model.DgImageCorrectInfoList.Count) return;
            string temName = Model.DgImageCorrectInfoList[index].Name;

            if (ImagesDic.ContainsKey(temName))
            {

                HOperatorSet.CopyObj(ImagesDic[temName], out HObject temImage, 1, 1);
                ShowTool.ClearAllOverLays();
                ShowTool.DispImage(temImage);
                ShowTool.D_HImage = temImage;
                HObject markObj = CalibAssistantTool.OnceCalibrate(temImage, hv_CalibDataID, index + 1);
                if (ObjectValided(markObj))
                {
                    ShowTool.DispRegion(markObj, "green");
                    ShowTool.AddregionBuffer(markObj, "green");
                    ShowTool.DispMessage(string.Format("图像:{0},索引:{1}标定成功！", temName, index),
                        10, 10, "green", 16);
                    ShowTool.AddTextBuffer(string.Format("图像:{0},索引:{1}标定成功！", temName, index),
                        10, 10, "green", 16);
                    //同步更新表格信息      
                    //Model.DgImageCorrectInfoList[index].ImageInfo = "标定成功";

                    ImageCorrectInfo dat = Model.DgImageCorrectInfoList[index];
                    dat.ImageInfo = "标定成功";
                    Model.DgImageCorrectInfoList.RemoveAt(index);
                    Model.DgImageCorrectInfoList.Insert(index,dat);

                    if (!DoneStepList.Contains(index))
                    {
                        totalStep++;
                        DoneStepList.Add(index);

                    }
                }
                else
                {
                    ShowTool.DispMessage(string.Format("图像:{0},索引:{1}标定失败！", temName, index),
                         10, 10, "red", 16);
                    ShowTool.AddTextBuffer(string.Format("图像:{0},索引:{1}标定失败！", temName, index),
                        10, 10, "red", 16);
                    //同步更新表格信息           
                    Model.DgImageCorrectInfoList[index].ImageInfo = "标定失败";
                }
               
            }
            else
            {
                MessageBox.Show(string.Format("图像{0}不存在", temName));
            }

        }
        /// <summary>
        /// 标定结束
        /// </summary>
        void btnCalibFinish_Click()
        {
            if (totalStep >= 9)
            {
                totalStep = 0;
                DoneStepList.Clear();
                hv_CamParam = CalibAssistantTool.CamCalibrate(hv_CalibDataID,
                    Model.TxbBoardThick,
                    out HTuple hv_Error,
                    out  hv_WorldPose);
                //绘制第一点的坐标系
                CalibAssistantTool.disp_3d_coord_system(ShowTool.HWindowsHandle,
                    hv_CamParam,
                    hv_WorldPose,0.1);

                Model.TxbCalibRMS = hv_Error.D;
                RMS = hv_Error.D;
                MessageBox.Show("标定成功，流程结束！");
            }
            else
            {
                totalStep = 0;
                DoneStepList.Clear();
                MessageBox.Show("标定失败，流程被终止！");
            }
            Model.BtnReadyCalibEnable = true;
            Model.BtnManualCalibEnable = false;

        }
        /// <summary>
        /// 标定准备
        /// </summary>
        void btnReadyCalib_Click()
        {
            hv_CalibDataID = null;
            DoneStepList.Clear();
            totalStep = 0;
            camParams.F = Model.TxbF;
            camParams.Sx = Model.TxbSx;
            camParams.Sy = Model.TxbSy;
            camParams.Cx = Model.TxbCx;
            camParams.Cy = Model.TxbCy;
            camParams.Width = Model.TxbWidth;
            camParams.Height = Model.TxbHeight;
            //标定准备
            HTuple StartCamPar = CalibAssistantTool.getStartCamPar(camParams);
            hv_CalibDataID = CalibAssistantTool.ReadyCalibrate(StartCamPar, calTab.CalPlateDescr);
            if (hv_CalibDataID != null || hv_CalibDataID.Length > 0)
            {
                Model.BtnReadyCalibEnable = false;
                Model.BtnManualCalibEnable = true;
            }
            else
            {
                Model.BtnReadyCalibEnable = true;
                Model.BtnManualCalibEnable = false;
                MessageBox.Show("准备失败，参数设置异常！");
            }
        }
        /// <summary>
        /// 参数保存
        /// </summary>
        void btnSaveParam_Click()
        {
            try
            {
                string filePath = rootFolder + "\\标定矩阵\\标定助手\\"+ currCalibName;
                if (!Directory.Exists(rootFolder + "\\标定矩阵"))
                    Directory.CreateDirectory(rootFolder + "\\标定矩阵");
                if (!Directory.Exists(rootFolder + "\\标定矩阵\\标定助手"))
                    Directory.CreateDirectory(rootFolder + "\\标定矩阵\\标定助手");
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                if (hv_CamParam != null && hv_CamParam.Length > 0)
                    CalibAssistantTool.SaveCalibData(hv_CamParam, filePath);
                if ( hv_WorldPose != null &&  hv_WorldPose.Length > 0)
                    CalibAssistantTool.SaveCalibPose(hv_WorldPose, filePath);

                camParams.F = Model.TxbF;
                camParams.Sx = Model.TxbSx;
                camParams.Sy = Model.TxbSy;
                camParams.Cx = Model.TxbCx;
                camParams.Cy = Model.TxbCy;
                camParams.Width = Model.TxbWidth;
                camParams.Height = Model.TxbHeight;
                GeneralUse.WriteSerializationFile<CamParams>(filePath + "\\相机参数", camParams);
                calTab.XNum = Model.TxbBoardXNum;
                calTab.YNum = Model.TxbBoardYNum;
                calTab.MarkDist = Model.TxbBoardMarkDis;
                calTab.DiameterRatio = Model.TxbBoardMarkDisRotia;
                GeneralUse.WriteSerializationFile<CalTab>(filePath + "\\标定板参数", calTab);
                GeneralUse.WriteValue("标定", "误差系数", RMS.ToString(), "config", filePath);
                GeneralUse.WriteSerializationFile<ObservableCollection<ImageCorrectInfo>>(filePath + "\\图像信息", Model.DgImageCorrectInfoList);

                string imageFile = filePath + "\\imags";
                Directory.Delete(imageFile,true);
                Directory.CreateDirectory(imageFile);
                foreach (var s in ImagesDic)
                    SaveImg(s.Value, imageFile, s.Key);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }
        /// <summary>
        /// 手动测试
        /// </summary>
        void btnTest_Click()
        {
            if (hv_CamParam == null || hv_CamParam.Length <= 0)
            {
                MessageBox.Show("相机未标定，无法获取相机内参！");
                return;
            }
            if (ObjectValided(imgBuf))
            {
                HObject correctImg = CalibAssistantTool.ImageCorret(imgBuf, hv_CamParam);
                ShowTool.ClearAllOverLays();
                ShowTool.D_HImage = correctImg;
                ShowTool.DispImage(correctImg);
            }
        }

        /// 判断图像或区域是否存在
        /// </summary>
        /// <param name = "obj" > 区域 </ param >
        /// < returns ></ returns >
        static public bool ObjectValided(HObject obj)
        {
            try
            {
                if (obj == null)
                    return false;
                if (!obj.IsInitialized())
                {
                    return false;
                }
                if (obj.CountObj() < 1)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 图片保存
        /// </summary>
        /// <param name="img"></param>
        /// <param name="DirPath"></param>
        /// <param name="imageName"></param>
        static public void SaveImg(HObject img, string DirPath, string imageName = "")
        {
            if (!ObjectValided(img))
            {
                //Appentxt("图片为空保存失败！");
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
                        HOperatorSet.WriteImage(img, "png", 0,
                                      DirPath + "\\" + buf[0] + ".png");
                    else if (buf[1].ToLower() == "jpg")
                        HOperatorSet.WriteImage(img, "jpeg", 0,
                                        DirPath + "\\" + buf[0] + ".jpg");
                    else if (buf[1].ToLower() == "bmp")
                        HOperatorSet.WriteImage(img, "bmp", 0,
                                        DirPath + "\\" + buf[0] + ".bmp");
                    else
                        HOperatorSet.WriteImage(img, "jpeg", 0,
                                       DirPath + "\\" + buf[0] + ".jpg");
                });
            }
            else
            {
                Task.Run(() =>
                {
                    HOperatorSet.WriteImage(img, "jpeg", 0,
                                    DirPath + "\\" + imageName + ".jpg");
                });
            }
        }
    }
}
