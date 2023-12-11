using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection;
using ROIGenerateLib;
using System.IO;
using System.Threading;
using System.Configuration;
using FilesRAW.Common;


namespace VisionShowLib.UserControls
{

    /* 一个vision控件对应一个图像操作和一个ROI操作*/
    public partial class VisionShowTool : UserControl
    {
        #region  EventHandle

        public EventHandler<MouseEventArgs> Disp_MouseMoveHandle;
        public EventHandler<MouseEventArgs> Disp_MouseDownHandle;
        public EventHandler<MouseEventArgs> Disp_MouseUpHandle;
        public EventHandler 显示中心十字坐标Handle;//显示图像中心十字坐标
        public EventHandler LoadedImageNoticeHandle;//加载图片更新通知
        public EventHandler ImageGetRotationHandle;//图像旋转更新通知
        public EventHandler 彩色显示ChangeEventHandle;//图像彩色显示
        public EventHandler SaveWindowImageHnadle;//保存窗体图像
        public EventHandler CamGrabHandle;   //相机采集     
        public OutPointGray DoubleClickGetMousePosHandle;// 双击获取像素坐标

        #endregion

        #region   Field

        bool isOdd = true;
        DateTime t1, t2;
        public List<pixelPoint> pixelPointList = null;
        public List<HObject> xldObjectList = null;
        private HTuple hWindowsHandle; //图像显示窗口句柄
        private bool isDown;         //判断鼠标是否一直处于按下状态  
        EumMouseOperation currMouseStatus;   //鼠标状态
        private HObject showImage = null;        //输入图像
        List<MessageTuple> messageList = new List<MessageTuple>();//文本显示信息集合
        List<RegionsTuple> RegionsList = new List<RegionsTuple>();//区域显示信息集合
        public delegate void OutPointGray(HTuple x, HTuple y, HTuple gray);// 双击获取像素坐标
        public event OutPointGray PointGray;// 点坐标和灰度值获取事件
        //显示图像十字坐标
        HXLDCont crosscont1 = new HXLDCont();
        HXLDCont crosscont2 = new HXLDCont();
        HObject xldbuf = new HObject();//在十字光标上添加辅助工具对象
        private HWndCtrl viewController;//图像控制对象
        private ROIController roiController;//ROI操作对象      
        public bool IsShowCenterCross = false;//是否显示中心十字
        EumSystemPattern currEumSystemPattern;//系统工作模式
        public EumImageRotation eumImageRotation;//图像旋转角度
        double startX, startY;//鼠标按下时的位置
        private Cursor scaleCursor = null;//鼠标图标对象
        #endregion

        #region Construction

        public VisionShowTool()
        {
            //此参数在主函数里面设置一次就可以
            //HOperatorSet.SetSystem("temporary_mem_cache", "false");
            //HOperatorSet.SetSystem("clip_region", "false");
            InitializeComponent();

            statusStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;

            LocationLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;

            TimeLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;

            GrayLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;

            toolStripStatusLabel4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;

            toolStripStatusLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;


            hWindowsHandle = h_Disp.HalconWindow;
            PointGray = imageInfoShow;
            h_Disp.ContextMenuStrip = this.contextMenuStrip1;
            roiController = new ROIController();
            viewController = new HWndCtrl(h_Disp);
            viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
            viewController.useROIController(this.roiController);//传递ROI操作对象给图像控制对象，响应在图像上绘制的图像

            无操作ToolStripMenuItem.Checked = true;
            h_Disp.HMouseUp += new HalconDotNet.HMouseEventHandler(this.h_Disp_HMouseUp);
            h_Disp.HMouseDown += new HalconDotNet.HMouseEventHandler(this.h_Disp_HMouseDown);
            h_Disp.HMouseMove += new HalconDotNet.HMouseEventHandler(this.h_Disp_HMouseMove);
            h_Disp.HMouseWheel += new HalconDotNet.HMouseEventHandler(this.h_Disp_HMouseWheel);

            //Cursor是GDI资源，你在每一回MouseMove里面都创建一次，将导致最终用尽GDI资源。
            //引发gdi+ 中发生一般性错误。
            //解决方法： 只创建一次，比如（在构造函数里）
            scaleCursor = SetCursor(VisionShowLib.Resource.scale, new Point(0, 0));

            pixelPointList = new List<pixelPoint>();

            xldObjectList = new List<HObject>();

            toolStrip1.BackColor = Color.FromArgb(255, 109, 60);

            statusStrip1.BackColor = Color.FromArgb(255, 109, 60);



        }
        public VisionShowTool(string titleName) : this()
        {
            TitleName = titleName;
        }
        ~VisionShowTool()
        {
            if (showImage != null)
            {
                showImage.Dispose();
                showImage = null;
            }
            RegionsList.Clear();
            messageList.Clear();
            PointGray = null;
            crosscont1.Dispose();
            crosscont2.Dispose();
        }
        #endregion

        #region Property
        /// <summary>
        /// 是否彩图格式
        /// </summary>
        public bool IsShowCoLorPalette
        {
            get
            {
                if (this.InvokeRequired)
                {

                    return (bool)this.Invoke(new Func<bool>(() =>
                        {
                            return this.彩色显示ToolStripMenuItem.Checked;
                        }));
                }
                else
                    return this.彩色显示ToolStripMenuItem.Checked;

            }

            set
            {

                if (this.InvokeRequired)
                    this.Invoke(new Action(() => { this.彩色显示ToolStripMenuItem.Checked = value; }));
                else
                    this.彩色显示ToolStripMenuItem.Checked = value;
            }
        }

        Color toolStripBC = Color.FromArgb(255, 109, 60);
        [Description("头标题颜色"), Category("自定义")]
        [DefaultValue(typeof(Color), "255, 109, 60")]
        [Browsable(true)]
        public Color ToolStripBC
        {
            get => this.toolStripBC;
            set
            {
                toolStripBC = value;
                toolStrip1.BackColor = ToolStripBC;
                Invalidate();
            }
        }

        Color statusStripBC = Color.FromArgb(255, 109, 60);
        [Description("脚标题颜色"), Category("自定义")]
        [DefaultValue(typeof(Color), "255, 109, 60")]
        [Browsable(true)]
        public Color StatusStripBC
        {
            get => this.statusStripBC;
            set
            {
                statusStripBC = value;
                statusStrip1.BackColor = StatusStripBC;
                Invalidate();
            }
        }

        int detectionTime = 0;
        /// <summary>
        /// 检测时间设定
        /// </summary>
        public int DetectionTime
        {
            get => this.detectionTime;
            set
            {
                this.detectionTime = value;

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.TimeLabel.Text = string.Format("{0}ms", value);
                    }));
                }
                else
                {
                    this.TimeLabel.Text = string.Format("{0}ms", value);
                }

            }
        }

        /// <summary>
        /// ROI操作对象
        /// </summary>
        public ROIController RoiController { get => this.roiController; }
        /// <summary>
        /// 图像控制对象
        /// </summary>
        public HWndCtrl ViewController { get => this.viewController; }
        /// <summary>
        /// 窗口句柄
        /// </summary>
        public HTuple HWindowsHandle
        {
            get => this.hWindowsHandle;
        }

        /// <summary>
        /// 输入图像
        /// </summary>
        [DefaultValue(null)]
        public HObject D_HImage
        {
            get => this.showImage;
            set
            {
                if (ObjectValided(value))
                {
                    HOperatorSet.GenEmptyObj(out showImage);
                    showImage.Dispose();
                    showImage = value;

                }
            }
        }
        /// <summary>
        /// 图像宽度
        /// </summary>
        [DefaultValue(0)]
        public int ImageWidth
        {
            get
            {
                HTuple width, height;
                if (showImage == null) return 0;
                HOperatorSet.GetImageSize(showImage, out width, out height);
                return width.I;
            }
        }
        /// <summary>
        /// 图像高度
        /// </summary>
        [DefaultValue(0)]
        public int ImageHeight
        {
            get
            {
                HTuple width, height;
                if (showImage == null) return 0;
                HOperatorSet.GetImageSize(showImage, out width, out height);
                return height.I;
            }
        }
        /// <summary>
        /// 图像显示窗体宽度
        /// </summary>
        [DefaultValue(300)]
        public int WinWidth
        {
            get => h_Disp.Width;

        }
        /// <summary>
        /// 图像显示窗体高度
        /// </summary>
        [DefaultValue(300)]
        public int WinHeight
        {
            get => h_Disp.Height;

        }
        /// <summary>
        /// 标题
        /// </summary>
        [DefaultValue("cam1")]
        public string TitleName
        {
            get => this.lblTitleName.Text;
            set
            {
                if (this.toolStrip1.InvokeRequired)
                    this.Invoke(new Action(() =>
                    {
                        this.lblTitleName.Text = value;
                    }));
                else
                    this.lblTitleName.Text = value;
            }

        }
        #endregion

        #region Method
        /// <summary>
        /// 设置缩放模式
        /// </summary>
        public void SetScale()
        {
            currMouseStatus = EumMouseOperation.Scale2;

            viewController.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
        }
        /// <summary>
        /// 图像放大，scale=放大比例
        /// </summary>
        /// <param name="scale"></param>
        public void scale_img(double scale = 0.8)
        {
            if (scale > 1) return;


            ////ROI设计模式时，更新创建ROI区域
            //if (currEumSystemPattern == EumSystemPattern.DesignModel)
            viewController.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            try
            {
                //若无图像则不执行
                if (!ObjectValided(showImage))
                {
                    return;
                }

                viewController.zoomImage(ImageWidth / 2, ImageHeight / 2, scale);

                FlashWindows();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 图像缩小，scale=缩小比例
        /// </summary>
        /// <param name="scale"></param>
        public void zoom_img(double scale = 1.2)
        {
            if (scale < 1) return;


            ////ROI设计模式时，更新创建ROI区域
            //if (currEumSystemPattern == EumSystemPattern.DesignModel)
            viewController.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            try
            {

                //若无图像则不执行
                if (!ObjectValided(showImage))
                {
                    return;
                }

                viewController.zoomImage(ImageWidth / 2, ImageHeight / 2, scale);

                FlashWindows();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        /// <summary>
        /// 设置上下标题颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColorOfTopBottomTitle(Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    this.toolStrip1.BackColor = color;
                    this.statusStrip1.BackColor = color;
                }));
            }
            else
            {
                this.toolStrip1.BackColor = color;
                this.statusStrip1.BackColor = color;
            }

        }
        /// <summary>
        /// 设置图像旋转按钮使能
        /// </summary>
        /// <param name="isEnable"></param>
        public void SetEnableOfRotation(bool isEnable)
        {
            图像旋转toolStripButton.Enabled = isEnable;
        }

        /// <summary>
        /// 设置图像旋转
        /// </summary>
        /// <param name="ImageRotation"></param>
        public void SetImageRotation(string ImageRotation)
        {
            //string ImageRotation = GeneralUse.ReadValue("图像旋转", "角度", "配置", "angle_0");

            string[] buf = ImageRotation.Split('_');

            if (int.Parse(buf[1]) == 360)
            {
                buf[1] = "0";
                ImageRotation = "angle_0";
            }

            this.图像旋转toolStripButton.ToolTipText = string.Format("图像已旋转{0}度", buf[1]);

            eumImageRotation = (EumImageRotation)Enum.Parse(typeof(EumImageRotation),
                                                                 ImageRotation);
        }
        /// <summary>
        /// 彩图切换按钮使能变更
        /// </summary>
        /// <param name="isEnable"></param>
        public void SetColorChangeBtnEnable(bool isEnable)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetColorChangeBtnEnable), isEnable);
            }
            else
            {
                彩色显示ToolStripMenuItem.Enabled = isEnable;
            }

        }
        /// <summary>
        /// 区域绘制模式
        /// fill：填充；margin：轮廓
        /// </summary>
        /// <param name="eumDrawModel"></param>
        public void setDraw(EumDrawModel eumDrawModel)
        {
            HOperatorSet.SetDraw(hWindowsHandle, Enum.GetName(typeof(EumDrawModel), eumDrawModel));
        }

        /// <summary>
        /// 在十字光标上添加辅助工具
        /// </summary>
        /// <param name="_xld">显示对象</param>
        public void AddAssistToolToCross(HObject _xld)
        {
            ClearAllOverLays();
            if (!ObjectValided(D_HImage)) return;
            if (xldbuf != null)
                xldbuf.Dispose();

            HOperatorSet.SetSystem("flush_graphic", "false");   //图像刷新OFF            
            {
                crosscont1.Dispose();
                crosscont1.GenContourPolygonXld((new HTuple(ImageHeight / 2)).TupleConcat(
    ImageHeight / 2), (new HTuple(0)).TupleConcat(ImageWidth));
                crosscont2.Dispose();
                crosscont2.GenContourPolygonXld((new HTuple(0)).TupleConcat(
    ImageHeight), (new HTuple(ImageWidth / 2)).TupleConcat(ImageWidth / 2));
            }
            HOperatorSet.SetSystem("flush_graphic", "true");//图像刷新on        

            DispImage(D_HImage);
            UpdateRegions(crosscont1, "red");
            UpdateRegions(crosscont2, "red");
            xldbuf = _xld;
            UpdateRegions(xldbuf, "red");
        }

        /// <summary>
        /// 图像控件背景颜色设置
        /// </summary>
        /// <param name="setcolor">颜色种类</param>
        public void SetBackgroundColor(EumControlBackColor setcolor)
        {

            h_Disp.HalconWindow.CloseWindow();

            string colorname = Enum.GetName(typeof(EumControlBackColor), setcolor).Replace("_", " ");
            HOperatorSet.SetWindowAttr("background_color", colorname); /////必须的先设置一下background_color属性，再OpenWindow一下

            h_Disp.HalconWindow.OpenWindow(0, 0, h_Disp.Width, h_Disp.Height, h_Disp.Handle, "visible", "");

            hWindowsHandle = h_Disp.HalconWindow;
        }

        /// <summary>
        /// 设置鼠标状态为无操作
        /// </summary>
        public void setMouseStateOfNone()
        {
            currMouseStatus = EumMouseOperation.None;
            平移ToolStripMenuItem.Checked = false;
            无操作ToolStripMenuItem.Checked = true;
            缩放ToolStripMenuItem.Checked = false;
            ////设计模式
            //if (currEumSystemPattern == EumSystemPattern.DesignModel)
            viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);

        }
        /// <summary>
        /// 移除控件右键菜单
        /// </summary>
        public void RemoveRightMenu()
        {
            if (h_Disp.InvokeRequired)
                h_Disp.Invoke(new Action(RemoveRightMenu));
            else
                h_Disp.ContextMenuStrip = null;

        }
        /// <summary>
        /// 添加控件右键菜单
        /// </summary>
        public void AddRightMenu()
        {
            if (h_Disp.InvokeRequired)
                h_Disp.Invoke(new Action(AddRightMenu));
            else
                h_Disp.ContextMenuStrip = this.contextMenuStrip1;

        }
        /// <summary>
        /// 设置系统运行模式
        /// </summary>
        /// <param name="pattenvalue">模式值</param>
        public void SetSystemPatten(EumSystemPattern pattenvalue)
        {
            currEumSystemPattern = viewController.currEumSystemPattern = pattenvalue;
            if (currEumSystemPattern == EumSystemPattern.RunningModel)
                显示中心十字坐标ToolStripMenuItem.Enabled = true;
            else
            {
                显示中心十字坐标ToolStripMenuItem.Checked = false;
                显示中心十字坐标ToolStripMenuItem.Enabled = false;
                IsShowCenterCross = false;
            }

        }

        /// <summary>
        /// 鼠标外形设置
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="hotPoint"></param>
        /// <returns></returns>
        private static Cursor SetCursor(Image cursor, Point hotPoint)
        {
            int hotX = hotPoint.X;
            int hotY = hotPoint.Y;
            using (Bitmap myNewCursor = new Bitmap(cursor.Width * 2 - hotX, cursor.Height * 2 - hotY))
            using (Graphics g = Graphics.FromImage(myNewCursor))
            {
                g.Clear(Color.FromArgb(0, 0, 0, 0));
                g.DrawImage(cursor, cursor.Width - hotX, cursor.Height - hotY, cursor.Width, cursor.Height);
                try
                {
                    IntPtr iptr = myNewCursor.GetHicon();
                    return new Cursor(iptr);
                }
                catch (Exception er)
                {
                    return null;
                }

            }
        }

        /// <summary>
        /// 判断图像或区域是否存在
        /// </summary>
        /// <param name="obj">被判断对象</param>
        /// <returns>对象是否存在标志</returns>
        public static bool ObjectValided(HObject obj)
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
        /// 读取图像
        /// </summary>
        /// <param name="imagePath">图像路径</param>
        /// <param name="image">图像对象</param>
        /// <returns>读取成功标志</returns>
        public bool ReadImage(HTuple imagePath, out HObject image)
        {
            image = null;

            if (File.Exists(imagePath))
            {
                HOperatorSet.ReadImage(out image, imagePath);
                if (image != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// 旋转显示图像
        /// </summary>
        /// <param name="image">被显示对象</param>
        public void DispImage(ref HObject Rotimage)
        {
            if (!ObjectValided(Rotimage))  //图像为空时.
                return;
            HObject Rimage = null;
            HOperatorSet.GenEmptyObj(out Rimage);
            Rimage.Dispose();
            switch (eumImageRotation)
            {
                case EumImageRotation.angle_0:
                    HOperatorSet.CopyObj(Rotimage, out Rimage, 1, 1);
                    // HOperatorSet.RotateImage(image, out Rimage, 0, "constant");
                    break;
                case EumImageRotation.angle_90:
                    HOperatorSet.RotateImage(Rotimage, out Rimage, -90, "constant");
                    break;
                case EumImageRotation.angle_180:
                    HOperatorSet.RotateImage(Rotimage, out Rimage, -90, "constant");
                    break;
                case EumImageRotation.angle_270:
                    HOperatorSet.RotateImage(Rotimage, out Rimage, -90, "constant");
                    break;
                case EumImageRotation.angle_360:
                    HOperatorSet.RotateImage(Rotimage, out Rimage, -90, "constant");
                    break;
                default:
                    // HOperatorSet.RotateImage(image, out Rimage, 0, "constant");
                    HOperatorSet.CopyObj(Rotimage, out Rimage, 1, 1);
                    break;
            }
            HOperatorSet.SetSystem("flush_graphic", "false");   //图像刷新OFF     
            viewController.addIconicVar(Rimage);  //设置合适的图像大小显示区域，并讲Img添加到ObjectList
            //viewController.changeGraphicSettings(GraphicsContext.GC_DRAWMODE, "margin");
            // viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);//图像为无操作模式，即此时不可移动和缩放
            viewController.ResetROI();//清除ROI缓存,并初始化ROI操作模式
            //viewController.setDispLevel(HWndCtrl.MODE_EXCLUDE_ROI);  //1:显示ROIlist中的ROI；2：不显示ROIlist中的ROI         
            viewController.repaint();  //重绘ObjectList中的图像和ROIlist中的ROI,是否能显示ROI前提是MODE_INCLUDE_ROI；
                                       //同步更新中心十字和辅助工具                  
            if (IsShowCenterCross)
            {
                crosscont1.Dispose();
                crosscont1.GenContourPolygonXld((new HTuple(ImageHeight / 2)).TupleConcat(
      ImageHeight / 2), (new HTuple(0)).TupleConcat(ImageWidth));
                crosscont2.Dispose();
                crosscont2.GenContourPolygonXld((new HTuple(0)).TupleConcat(
  ImageHeight), (new HTuple(ImageWidth / 2)).TupleConcat(ImageWidth / 2));

                UpdateRegions(crosscont1, "red");
                UpdateRegions(crosscont2, "red");
                UpdateRegions(xldbuf, "red");
            }
            HOperatorSet.SetSystem("flush_graphic", "true");//图像刷新on
            h_Disp.HalconWindow.SetColor("white");
            h_Disp.HalconWindow.DispLine(-100.0, -100.0, -101.0, -101.0);

            if (ObjectValided(Rotimage))
                Rotimage.Dispose();
            HOperatorSet.CopyObj(Rimage, out Rotimage, 1, 1);
        }
        /// <summary>
        /// 旋转显示图像
        /// </summary>
        /// <param name="image">被显示对象</param>
        public void DispImage(ref HObject Rotimage, HTuple AngleSetting)
        {
            if (!ObjectValided(Rotimage))  //图像为空时.
                return;

            HObject Rimage = null;
            HOperatorSet.GenEmptyObj(out Rimage);
            Rimage.Dispose();
            HOperatorSet.RotateImage(Rotimage, out Rimage, AngleSetting, "constant");

            HOperatorSet.SetSystem("flush_graphic", "false");   //图像刷新OFF     
            viewController.addIconicVar(Rimage);  //设置合适的图像大小显示区域，并讲Img添加到ObjectList
            //viewController.changeGraphicSettings(GraphicsContext.GC_DRAWMODE, "margin");
            // viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);//图像为无操作模式，即此时不可移动和缩放
            viewController.ResetROI();//清除ROI缓存,并初始化ROI操作模式
            //viewController.setDispLevel(HWndCtrl.MODE_EXCLUDE_ROI);  //1:显示ROIlist中的ROI；2：不显示ROIlist中的ROI         
            viewController.repaint();  //重绘ObjectList中的图像和ROIlist中的ROI,是否能显示ROI前提是MODE_INCLUDE_ROI；
                                       //同步更新中心十字和辅助工具                  
            if (IsShowCenterCross)
            {
                crosscont1.Dispose();
                crosscont1.GenContourPolygonXld((new HTuple(ImageHeight / 2)).TupleConcat(
      ImageHeight / 2), (new HTuple(0)).TupleConcat(ImageWidth));
                crosscont2.Dispose();
                crosscont2.GenContourPolygonXld((new HTuple(0)).TupleConcat(
  ImageHeight), (new HTuple(ImageWidth / 2)).TupleConcat(ImageWidth / 2));

                UpdateRegions(crosscont1, "red");
                UpdateRegions(crosscont2, "red");
                UpdateRegions(xldbuf, "red");


            }
            HOperatorSet.SetSystem("flush_graphic", "true");//图像刷新on
            h_Disp.HalconWindow.SetColor("white");
            h_Disp.HalconWindow.DispLine(-100.0, -100.0, -101.0, -101.0);

            if (ObjectValided(Rotimage))
                Rotimage.Dispose();
            HOperatorSet.CopyObj(Rimage, out Rotimage, 1, 1);
        }
        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="image">被显示对象</param>
        public void DispImage(HObject image)
        {
            if (!ObjectValided(image))  //图像为空时.
                return;
            HOperatorSet.SetSystem("flush_graphic", "false");   //图像刷新OFF     
            viewController.addIconicVar(image);  //设置合适的图像大小显示区域，并讲Img添加到ObjectList
            //viewController.changeGraphicSettings(GraphicsContext.GC_DRAWMODE, "margin");
            // viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);//图像为无操作模式，即此时不可移动和缩放
            viewController.ResetROI();//清除ROI缓存,并初始化ROI操作模式
            //viewController.setDispLevel(HWndCtrl.MODE_EXCLUDE_ROI);  //1:显示ROIlist中的ROI；2：不显示ROIlist中的ROI         
            viewController.repaint();  //重绘ObjectList中的图像和ROIlist中的ROI,是否能显示ROI前提是MODE_INCLUDE_ROI；
                                       //同步更新中心十字和辅助工具                  
            if (IsShowCenterCross)
            {
                crosscont1.Dispose();
                crosscont1.GenContourPolygonXld((new HTuple(ImageHeight / 2)).TupleConcat(
      ImageHeight / 2), (new HTuple(0)).TupleConcat(ImageWidth));
                crosscont2.Dispose();
                crosscont2.GenContourPolygonXld((new HTuple(0)).TupleConcat(
  ImageHeight), (new HTuple(ImageWidth / 2)).TupleConcat(ImageWidth / 2));

                UpdateRegions(crosscont1, "red");
                UpdateRegions(crosscont2, "red");
                UpdateRegions(xldbuf, "red");


            }
            HOperatorSet.SetSystem("flush_graphic", "true");//图像刷新on
            h_Disp.HalconWindow.SetColor("white");
            h_Disp.HalconWindow.DispLine(-100.0, -100.0, -101.0, -101.0);
        }

        /// <summary>
        /// 适应窗口，即图像保持原比例显示在图像控件的中间
        /// </summary>
        public void FitWindows()
        {
            h_Disp.Cursor = Cursors.Arrow;
            if (viewController?.getListCount() < 1)  //图像为空时.
                return;
            HOperatorSet.SetSystem("flush_graphic", "false");   //图像刷新OFF   
            viewController?.FitWindowsSize();
            viewController?.repaint();
            //设计模式不显示以下内容
            if (currEumSystemPattern == EumSystemPattern.RunningModel)
            {
                foreach (var s in RegionsList)
                    UpdateRegions(s.showRegion, s.showColor);

                foreach (var s in messageList)
                    UpdataMessage(s.showMessage, s.showRow, s.showCol, s.showColor, s.showSize);

                if (IsShowCenterCross)
                {
                    crosscont1.Dispose();
                    crosscont1.GenContourPolygonXld((new HTuple(ImageHeight / 2)).TupleConcat(
              ImageHeight / 2), (new HTuple(0)).TupleConcat(ImageWidth));
                    crosscont2.Dispose();
                    crosscont2.GenContourPolygonXld((new HTuple(0)).TupleConcat(
          ImageHeight), (new HTuple(ImageWidth / 2)).TupleConcat(ImageWidth / 2));

                    UpdateRegions(crosscont1, "red");
                    UpdateRegions(crosscont2, "red");
                    UpdateRegions(xldbuf, "red");

                }
            }
            HOperatorSet.SetSystem("flush_graphic", "true");//图像刷新on
            h_Disp.HalconWindow.SetColor("white");
            h_Disp.HalconWindow.DispLine(-100.0, -100.0, -101.0, -101.0);
            //HOperatorSet.SetColor(hWindowsHandle, "white");
            //HOperatorSet.DispLine(hWindowsHandle, -100.0, -100.0, -101.0, -101.0);
        }

        /// <summary>
        /// 刷新窗口,适用于鼠标进行图像放大或缩小动作，，因为缩放因子在鼠标滚动时会进行重算
        /// </summary>
        void FlashWindows()
        {
            try
            {

                HOperatorSet.SetSystem("flush_graphic", "false");   //图像刷新OFF            
                HOperatorSet.ClearWindow(hWindowsHandle);
                //HOperatorSet.SetSystem("flush_graphic", "true");//图像刷新on
                //显示图像
                if (viewController?.getListCount() < 1)  //图像为空时.
                    return;
                viewController?.repaint();


                //设计模式不显示以下内容
                if (currEumSystemPattern == EumSystemPattern.RunningModel)
                {
                    foreach (var s in RegionsList)
                        UpdateRegions(s.showRegion, s.showColor);
                    foreach (var s in messageList)
                        UpdataMessage(s.showMessage, s.showRow, s.showCol, s.showColor, s.showSize);
                    if (IsShowCenterCross)
                    {

                        crosscont2.Dispose();
                        crosscont2.GenContourPolygonXld((new HTuple(0)).TupleConcat(
              ImageHeight), (new HTuple(ImageWidth / 2)).TupleConcat(ImageWidth / 2));
                        UpdateRegions(crosscont2, "red");

                        crosscont1.Dispose();
                        crosscont1.GenContourPolygonXld((new HTuple(ImageHeight / 2)).TupleConcat(
                  ImageHeight / 2), (new HTuple(0)).TupleConcat(ImageWidth));
                        UpdateRegions(crosscont1, "red");

                        UpdateRegions(xldbuf, "red");

                    }
                }
                HOperatorSet.SetSystem("flush_graphic", "true");//图像刷新on
                h_Disp.HalconWindow.SetColor("white");
                h_Disp.HalconWindow.DispLine(-100.0, -100.0, -101.0, -101.0);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 更新绘制区域
        /// </summary>
        /// <param name="repaintRegion"></param>
        /// <param name="color"></param>
        void UpdateRegions(HObject repaintRegion, string color)
        {
            try
            {
                if (!ObjectValided(repaintRegion))
                {
                    return;
                }
                HOperatorSet.SetColor(hWindowsHandle, color);
                HOperatorSet.DispObj(repaintRegion, hWindowsHandle);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 刷新图像窗口上的文字
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="row">坐标R</param>
        /// <param name="col">坐标C</param>
        /// <param name="color">文字颜色</param>
        /// <param name="size">文字尺寸</param>
        void UpdataMessage(HTuple message, HTuple row, HTuple col, HTuple color, HTuple size)
        {
            HTuple length1, length2, length3, length4, length5;
            try
            {
                HOperatorSet.TupleLength(message, out length1);
                HOperatorSet.TupleLength(row, out length2);
                HOperatorSet.TupleLength(col, out length3);
                HOperatorSet.TupleLength(color, out length4);
                HOperatorSet.TupleLength(size, out length5);
                int len1 = length1.I;
                int len2 = length2.I;
                int len3 = length3.I;
                int len4 = length4.I;
                int len5 = length5.I;

                if (len1 == len2 && len1 == len3 && len1 == len4 && len1 == len5)
                {
                    for (int i = 0; i < length1; i++)
                    {
                        if (hWindowsHandle.D > 100000)
                            HOperatorSet.SetFont(hWindowsHandle, "Arial-" + size.ToString());
                        else
                            HOperatorSet.SetFont(hWindowsHandle, "-Arial-" + size.ToString() + "-*-*-*-*-1-");

                        disp_message(hWindowsHandle, message[i], new HTuple("image"), row[i], col[i], color[i], new HTuple("false"));
                    }
                }
                else
                {
                    throw new Exception("写入信息个数与点位坐标个数不匹配");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 显示区域
        /// </summary>
        /// <param name="region"></param>
        /// <param name="color"></param>
        public void DispRegion(HObject region, string color)
        {
            if (!ObjectValided(region))
            {
                return;
            }
            setDraw(EumDrawModel.margin);
            HOperatorSet.SetColor(hWindowsHandle, color);
            HOperatorSet.DispObj(region, hWindowsHandle);
        }
        /// <summary>
        /// 区域缓存清除
        /// </summary>
        public void RegionBufferClear()
        {
            if (RegionsList != null)
                RegionsList.Clear();
        }
        /// <summary>
        /// 添加区域缓存
        /// </summary>
        /// <param name="region"></param>
        /// <param name="color"></param>
        public void AddregionBuffer(HObject region, string color)
        {

            if (!ObjectValided(region)) return;
            RegionsTuple d_RegionsTuple = new RegionsTuple();
            d_RegionsTuple.showColor = color;
            d_RegionsTuple.showRegion = region.Clone();
            RegionsList.Add(d_RegionsTuple);

        }
        /// <summary>
        /// 显示文本信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        public void DispMessage(string message, HTuple row, HTuple col, HTuple color, int size)
        {
            if (hWindowsHandle.D > 100000)
                HOperatorSet.SetFont(hWindowsHandle, "Arial-" + size.ToString());
            else
                HOperatorSet.SetFont(hWindowsHandle, "-Arial-" + size.ToString() + "-*-*-*-*-1-");
            disp_message(hWindowsHandle, message, new HTuple("image"), row, col, color, new HTuple("false"));

        }
        //显示报警信息
        public void DispAlarmMessage(string message, HTuple row, HTuple col, int size)
        {
            if (hWindowsHandle.D > 100000)
                HOperatorSet.SetFont(hWindowsHandle, "Arial-" + size.ToString());
            else
                HOperatorSet.SetFont(hWindowsHandle, "-Arial-" + size.ToString() + "-*-*-*-*-1-");
            disp_message(hWindowsHandle, message, new HTuple("image"), row, col, "red", new HTuple("true"));

        }
        /// <summary>
        /// 文本缓存清除
        /// </summary>
        public void TextBufferClear()
        {
            if (messageList != null)
                messageList.Clear();
        }
        /// <summary>
        /// 添加文本缓存
        /// </summary>
        /// <param name="message"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        public void AddTextBuffer(string message, HTuple row, HTuple col, HTuple color, int size)
        {

            MessageTuple d_MessageTuple = new MessageTuple();
            d_MessageTuple.showMessage = message;
            d_MessageTuple.showRow = row;
            d_MessageTuple.showCol = col;
            d_MessageTuple.showColor = color;
            d_MessageTuple.showSize = size;
            messageList.Add(d_MessageTuple);

        }

        /// <summary>
        /// 清除所有显示，包括显示的图像
        /// </summary>
        public void ClearAllOverLays()
        {
            HOperatorSet.SetSystem("flush_graphic", "false");   //图像刷新OFF            
            HOperatorSet.ClearWindow(hWindowsHandle);
            HOperatorSet.SetSystem("flush_graphic", "true");//图像刷新on        
            messageList.Clear();
            RegionsList.Clear();
            viewController.clearList();
            roiController.resetROI();

        }

        /// <summary>
        /// 显示多组合HObject（包含：图像，区域，轮廓）
        /// </summary>
        /// <param name="concatedObj"></param>
        /// <param name="color"></param>
        public void DispConcatedObj(HObject concatedObj, EumCommonColors color)
        {
            if (!ObjectValided(concatedObj)) return;
            HOperatorSet.CountObj(concatedObj, out HTuple number);
            int imageIndex = -1;
            bool isExistImage = false;
            //如果存在图像需要先显示图像再显示区域和轮廓
            for (int i = 1; i <= number; i++)
            {
                HOperatorSet.SelectObj(concatedObj, out HObject objectSelected, i);
                //图像显示
                if (objectSelected.GetObjClass().S == "image")
                {
                    isExistImage = true;
                    imageIndex = i;
                    break;
                }
                objectSelected.Dispose();
            }
            if (isExistImage)
            {
                HOperatorSet.SelectObj(concatedObj, out HObject objectSelected, imageIndex);
                DispImage(objectSelected);
            }
            setDraw(EumDrawModel.margin);
            for (int j = 1; j <= number; j++)
            {
                HOperatorSet.SelectObj(concatedObj, out HObject objectSelected2, j);
                //区域和轮廓
                if (objectSelected2.GetObjClass().S != "image")
                {
                    HOperatorSet.SetColor(hWindowsHandle, Enum.GetName(typeof(EumCommonColors), color));
                    HOperatorSet.DispObj(objectSelected2, hWindowsHandle);
                }
            }
        }

        /// <summary>
        /// 添加区域或轮廓缓存
        /// </summary>
        /// <param name="concatedObj">区域或轮廓</param>
        /// <param name="color">颜色</param>
        public void AddConcatedObjBuffer(HObject concatedObj, EumCommonColors color)
        {
            HOperatorSet.CountObj(concatedObj, out HTuple number);
            //如果存在图像需要先显示图像再显示区域和轮廓
            for (int i = 1; i <= number; i++)
            {
                HOperatorSet.SelectObj(concatedObj, out HObject objectSelected, i);
                if (objectSelected.GetObjClass().S != "image")
                {
                    RegionsTuple d_RegionsTuple = new RegionsTuple();
                    d_RegionsTuple.showColor = Enum.GetName(typeof(EumCommonColors), color);
                    d_RegionsTuple.showRegion = objectSelected.Clone();
                    RegionsList.Add(d_RegionsTuple);
                }
                objectSelected.Dispose();
            }
        }

        /// <summary>
        /// 保存显示图像
        /// </summary>
        /// <param name="path">保存位置</param>
        public void SaveImage(string path)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                //若文件夹不存在则创建
                if (!file.Directory.Exists)
                    file.Directory.Create();
                string ex = Path.GetExtension(path);
                ex = ex.Replace(".", "");
                if (ex == "jpg")
                {
                    ex = "jpeg";
                }
                HOperatorSet.WriteImage(showImage, ex, 255, path);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 保存窗口截图
        /// </summary>
        /// <param name="path">保存位置</param>
        public void SaveDumpImage(string path)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                //若文件夹不存在则创建
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                string ex = Path.GetExtension(path);
                ex = ex.Replace(".", "");
                if (ex == "jpg")
                {
                    ex = "jpeg";
                }
                HOperatorSet.DumpWindow(hWindowsHandle, ex, path);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region-----图像转换-----
        /// <summary>
        /// 彩色图像 HObject -> HImage3
        /// </summary>
        public HImage HObject2HImage3(HObject hObj)
        {
            HImage image = new HImage();
            HTuple type, width, height, pointerRed, pointerGreen, pointerBlue;
            HOperatorSet.GetImagePointer3(hObj, out pointerRed, out pointerGreen, out pointerBlue,
                                          out type, out width, out height);
            image.GenImage3(type, width, height, pointerRed, pointerGreen, pointerBlue);
            return image;
        }

        /// <summary>
        /// 灰度图像 HObject -> HImage1
        /// </summary>
        public HImage HObject2HImage1(HObject hObj)
        {
            HImage image = new HImage();
            HTuple type, width, height, pointer;
            HOperatorSet.GetImagePointer1(hObj, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
            return image;
        }
        /// <summary>
        /// bitmap转HObject（3通道图）
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="image"></param>
        public static void Bitmap2HObjectBpp24(Bitmap bmp, out HObject image)  //90ms
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                HOperatorSet.GenImageInterleaved(out image, srcBmpData.Scan0, "bgr", bmp.Width, bmp.Height, 0, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmpData);

            }
            catch (Exception ex)
            {
                image = null;
            }
        }
        /// <summary>
        /// bitmap转HObject（灰度图）
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="image"></param>
        public static void Bitmap2HObjectBpp8(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

                HOperatorSet.GenImage1(out image, "byte", bmp.Width, bmp.Height, srcBmpData.Scan0);
                bmp.UnlockBits(srcBmpData);
            }
            catch (Exception ex)
            {
                image = null;
            }
        }
        /// <summary>
        /// HObject转bitmap（3通道图）
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        public static Bitmap Honject2Bitmap24(HObject hObject)
        {
            //获取图像尺寸
            HTuple width0 = new HTuple();
            HTuple height0 = new HTuple();
            HTuple Pointer = new HTuple();
            HTuple type = new HTuple();
            HTuple width = new HTuple();
            HTuple height = new HTuple();
            HObject InterImage = new HObject();
            HOperatorSet.GetImageSize(hObject, out width0, out height0);
            HOperatorSet.GetImageSize(hObject, out width0, out height0);
            //创建交错格式图像
            HOperatorSet.InterleaveChannels(hObject, out InterImage, "rgb", 4 * width0, 0);
            //获取交错格式图像指针
            HOperatorSet.GetImagePointer1(InterImage, out Pointer, out type, out width, out height);
            IntPtr ptr = Pointer;
            //构建新Bitmap图像
            Bitmap bitmap = new Bitmap(width / 4, height, width, PixelFormat.Format24bppRgb, ptr);
            return bitmap;
        }

        /// <summary>
        /// HObject转bitmap（灰度图）
        /// </summary>
        /// <param name="image"></param>
        static public Bitmap HObject2Bitmap8(HObject image)
        {
            HTuple hpoint, type, width, height;
            const int Alpha = 255;
            HOperatorSet.GetImagePointer1(image, out hpoint, out type, out width, out height);
            Bitmap res = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette pal = res.Palette;
            for (int i = 0; i <= 255; i++)
            { pal.Entries[i] = Color.FromArgb(Alpha, i, i, i); }

            res.Palette = pal; Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = res.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            int PixelSize = Bitmap.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
            IntPtr ptr1 = bitmapData.Scan0;
            IntPtr ptr2 = hpoint; int bytes = width * height;
            byte[] rgbvalues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbvalues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr1, bytes);
            res.UnlockBits(bitmapData);
            return res;
        }

        #endregion
        #endregion

        #region Menu
        private void VisionShowTool_Load(object sender, EventArgs e)
        {
            //  SetBackgroundColor(EumControlBackColor.white);
        }

        #region HwindowMenu
        /// <summary>
        /// 鼠标滚轮滚动进行图像缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void h_Disp_HMouseWheel(object sender, HMouseEventArgs e)
        {
            if (currMouseStatus == EumMouseOperation.Scale ||
                currMouseStatus == EumMouseOperation.Scale2)
            {
                if (currMouseStatus == EumMouseOperation.Scale)
                    this.Cursor = scaleCursor;
                //运行模式
                if (currEumSystemPattern == EumSystemPattern.RunningModel)
                {
                    double scale;
                    try
                    {

                        //若无图像则不执行
                        if (!ObjectValided(showImage))
                        {
                            return;
                        }
                        if (e.Delta >= 0)
                            scale = 0.9;
                        else
                            scale = 1 / 0.9;
                        viewController.zoomImage(e.X, e.Y, scale);

                        FlashWindows();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 按住鼠标左键后可平移图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void h_Disp_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (currMouseStatus == EumMouseOperation.Move)
            {
                //运行模式
                if (currEumSystemPattern == EumSystemPattern.RunningModel)
                {
                    double motionX, motionY;

                    try
                    {
                        if (!this.h_Disp.Focused)
                        {
                            return;
                        }

                        //若无图像则不执行
                        if (showImage == null)
                        {
                            return;
                        }
                        if (isDown) //若按下鼠标按键
                        {

                            motionX = ((e.X - startX));
                            motionY = ((e.Y - startY));

                            if (((int)motionX != 0) || ((int)motionY != 0))
                            {
                                viewController.moveImage(motionX, motionY);
                                FlashWindows();
                                startX = e.X - motionX;
                                startY = e.Y - motionY;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void h_Disp_HMouseDown(object sender, HMouseEventArgs e)
        {
            //运行模式
            if (currEumSystemPattern == EumSystemPattern.RunningModel)
            {
                //若无图像则不执行
                if (showImage == null)
                {
                    return;
                }
                startX = e.X;
                startY = e.Y;
                isDown = true;
            }
        }
        /// <summary>
        /// 鼠标抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void h_Disp_HMouseUp(object sender, HMouseEventArgs e)
        {
            //运行模式
            if (currEumSystemPattern == EumSystemPattern.RunningModel)
            {
                //若无图像则不执行
                if (!ObjectValided(showImage))
                {
                    return;
                }
                startX = -1;
                startY = -1;
                isDown = false;
            }
        }
        /// <summary>
        /// 图像显示控件尺寸改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void h_Disp_SizeChanged(object sender, EventArgs e)
        {
            FitWindows();
        }
        /// <summary>
        /// 鼠标进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void h_Disp_MouseEnter(object sender, EventArgs e)
        {

            switch (currMouseStatus)
            {
                case EumMouseOperation.None:
                    this.Cursor = Cursors.Default;
                    break;
                case EumMouseOperation.Move:
                    this.Cursor = Cursors.SizeAll;
                    break;
                case EumMouseOperation.Scale:
                    this.Cursor = scaleCursor;
                    break;
                case EumMouseOperation.Scale2:
                    this.Cursor = Cursors.Default;
                    break;
            }

        }
        /// <summary>
        /// 鼠标移动显示当前像素值及坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void h_Disp_MouseMove(object sender, MouseEventArgs e)
        {
            Disp_MouseMoveHandle?.Invoke(sender, e);
            HTuple ptX, ptY, hv_Button;
            HTuple Grey;
            if (viewController.getListCount() > 0 &&
                   currMouseStatus == EumMouseOperation.None)
            {
                try
                {

                    if ((e.Button & MouseButtons.Left) == MouseButtons.Left
                        && isDown
                        && (e.X > this.Width || e.Y > this.Height))
                    {
                        HOperatorSet.CopyObj(this.showImage, out HObject data, 1, 1);
                        var effect = this.DoDragDrop(data, DragDropEffects.All);
                    }

                    HOperatorSet.GetMposition(hWindowsHandle, out ptY, out ptX, out hv_Button);
                    //获取灰度值
                    HOperatorSet.GetGrayval(showImage, ptY, ptX, out Grey);
                }
                catch (Exception ex)
                {
                    ptX = 0;
                    ptY = 0;
                    hv_Button = -1;
                    Grey = 0;
                }

                PointGray?.Invoke(ptX, ptY, Grey);
            }

        }
        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void h_Disp_MouseDown(object sender, MouseEventArgs e)
        {
            Disp_MouseDownHandle?.Invoke(sender, e);
        }
        /// <summary>
        /// 鼠标抬起，同时相应双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void h_Disp_MouseUp(object sender, MouseEventArgs e)
        {
            Disp_MouseUpHandle?.Invoke(sender, e);
            if (isOdd)
            {
                t1 = DateTime.Now;
            }
            else
            {
                t2 = DateTime.Now;
            }
            isOdd = !isOdd;
            if (Math.Abs((t1 - t2).TotalMilliseconds) < 500)
            {

                HTuple ptX = 0, ptY = 0, hv_Button;
                HTuple Grey = 0;
                if (viewController.getListCount() > 0 &&
                       currMouseStatus == EumMouseOperation.None)
                {
                    try
                    {
                        HOperatorSet.GetMposition(hWindowsHandle, out ptY, out ptX, out hv_Button);
                        //获取灰度值
                        HOperatorSet.GetGrayval(showImage, ptY, ptX, out Grey);
                    }
                    catch (Exception ex)
                    {
                        ptX = 0;
                        ptY = 0;
                        hv_Button = -1;
                        Grey = 0;
                    }
                    DoubleClickGetMousePosHandle?.Invoke(ptX, ptY, Grey);
                }
            }
        }

        /// <summary>
        /// 图像信息显示
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="gray"></param>
        void imageInfoShow(HTuple x, HTuple y, HTuple gray)
        {

            this.Invoke(new Action(() =>
            {
                if (gray.TupleLength() > 1)
                    this.GrayLabel.Text = string.Format("{0},{1},{2}", gray.TupleSelect(0), gray.TupleSelect(1), gray.TupleSelect(2));
                else
                    this.GrayLabel.Text = string.Format("{0}", gray.TupleSelect(0));

                this.LocationLabel.Text = string.Format("{0},{1}", x.TupleSelect(0), y.TupleSelect(0));
            }));
        }
        #endregion
        #region RightMenu
        public void 无操作ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currMouseStatus = EumMouseOperation.None;
            平移ToolStripMenuItem.Checked = false;
            无操作ToolStripMenuItem.Checked = true;
            缩放ToolStripMenuItem.Checked = false;
            ////设计模式
            //if (currEumSystemPattern == EumSystemPattern.DesignModel)
            viewController.setViewState(HWndCtrl.MODE_VIEW_NONE);
        }
        public void 平移ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currMouseStatus = EumMouseOperation.Move;
            平移ToolStripMenuItem.Checked = true;
            无操作ToolStripMenuItem.Checked = false;
            缩放ToolStripMenuItem.Checked = false;

            ////ROI设计模式时，更新创建ROI区域
            //if (currEumSystemPattern == EumSystemPattern.DesignModel)
            viewController.setViewState(HWndCtrl.MODE_VIEW_MOVE);

        }
        private void 缩放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currMouseStatus = EumMouseOperation.Scale;
            平移ToolStripMenuItem.Checked = false;
            无操作ToolStripMenuItem.Checked = false;
            缩放ToolStripMenuItem.Checked = true;

            ////ROI设计模式时，更新创建ROI区域
            //if (currEumSystemPattern == EumSystemPattern.DesignModel)
            viewController.setViewState(HWndCtrl.MODE_VIEW_ZOOM);

        }
        public void 显示中心十字坐标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ObjectValided(D_HImage)) return;
            显示中心十字坐标ToolStripMenuItem.Checked = !显示中心十字坐标ToolStripMenuItem.Checked;
            IsShowCenterCross = 显示中心十字坐标ToolStripMenuItem.Checked;
            FitWindows();
            显示中心十字坐标Handle?.Invoke(sender, e);

        }
        private void 文本区域清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAllOverLays();
            if (!ObjectValided(D_HImage)) return;
            DispImage(D_HImage);
        }
        private void 加载图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog m_OpenFileDialog = new OpenFileDialog();
            m_OpenFileDialog.Multiselect = true;
            m_OpenFileDialog.Filter = "JPEG文件,BMP文件|*.jpg*;*.bmp*|所有文件(*.*)|*.*";
            if (m_OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                HObject GrabImg = null;
                HOperatorSet.GenEmptyObj(out GrabImg);
                GrabImg.Dispose();
                ReadImage(m_OpenFileDialog.FileName, out GrabImg);
                ClearAllOverLays();

                HOperatorSet.CountChannels(GrabImg, out HTuple channels);
                if (channels[0].I == 3)
                    if (!彩色显示ToolStripMenuItem.Checked)
                        HOperatorSet.Rgb1ToGray(GrabImg, out GrabImg);

                string ImageRotation = Enum.GetName(typeof(EumImageRotation), eumImageRotation);
                string[] buf = ImageRotation.Split('_');
                int angle = int.Parse(buf[1]);
                DispImage(ref GrabImg, -angle);
                D_HImage = GrabImg;

                LoadedImageNoticeHandle?.Invoke(D_HImage, new EventArgs());
            }
        }
        private void 适应窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            h_Disp.Cursor = Cursors.Arrow;
            FitWindows();

        }
        private void 保存图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ObjectValided(D_HImage)) return;
            h_Disp.Cursor = Cursors.Arrow;
            try
            {
                SaveFileDialog m_SaveFileDialog = new SaveFileDialog();
                m_SaveFileDialog.Filter = "JPEG文件|*.jpg*|BMP文件|*.bmp*";
                m_SaveFileDialog.DereferenceLinks = true;

                if (m_SaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string tembuf = m_SaveFileDialog.FilterIndex == 1 ? ".jpg" : ".bmp";
                    string name = m_SaveFileDialog.FileName;
                    string tempath = string.Concat(name, tembuf);
                    ThreadPool.QueueUserWorkItem((s) => SaveImage(tempath));
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }

        }
        private void 彩色显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            彩色显示ToolStripMenuItem.Checked = !彩色显示ToolStripMenuItem.Checked;

            彩色显示ChangeEventHandle?.Invoke(sender, e);
        }
        private void 保存窗体图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveWindowImageHnadle?.Invoke(sender, e);
        }
        #endregion
        #region LeftMenu
        private void 放大toolStripButton_Click(object sender, EventArgs e)
        {
            scale_img();
            currMouseStatus = EumMouseOperation.None;
        }
        private void 缩小toolStripButton_Click(object sender, EventArgs e)
        {
            zoom_img();
            currMouseStatus = EumMouseOperation.None;
        }
        private void 自适应toolStripButton_Click(object sender, EventArgs e)
        {
            FitWindows();
            currMouseStatus = EumMouseOperation.None;
        }
        private void 平移toolStripButton_Click(object sender, EventArgs e)
        {
            平移ToolStripMenuItem_Click(null, null);
        }
        private void 无操作toolStripButton_Click(object sender, EventArgs e)
        {
            无操作ToolStripMenuItem_Click(null, null);
        }
        private void 图像旋转toolStripButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("图像即将旋转,并需要重新标定，请确认！", "提醒"
                 , MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {

                if (eumImageRotation == EumImageRotation.angle_0)
                {
                    eumImageRotation = EumImageRotation.angle_90;
                    this.图像旋转toolStripButton.ToolTipText = "图像已旋转90度";
                }
                else if (eumImageRotation == EumImageRotation.angle_90)
                {
                    eumImageRotation = EumImageRotation.angle_180;
                    this.图像旋转toolStripButton.ToolTipText = "图像已旋转180度";
                }
                else if (eumImageRotation == EumImageRotation.angle_180)
                {
                    eumImageRotation = EumImageRotation.angle_270;
                    this.图像旋转toolStripButton.ToolTipText = "图像已旋转270度";
                }
                else if (eumImageRotation == EumImageRotation.angle_270)
                {
                    eumImageRotation = EumImageRotation.angle_360;
                    this.图像旋转toolStripButton.ToolTipText = "图像已旋转360度";
                }
                else if (eumImageRotation == EumImageRotation.angle_360)
                {
                    eumImageRotation = EumImageRotation.angle_90;
                    this.图像旋转toolStripButton.ToolTipText = "图像已旋转90度";
                }
                //配置文件保存

                string ImageRotation = Enum.GetName(typeof(EumImageRotation), eumImageRotation);
                //   GeneralUse.WriteValue("图像旋转","角度", ImageRotation,"配置");

                //若无图像则不执行
                if (!ObjectValided(showImage))
                {
                    return;
                }

                DispImage(ref showImage);

                ImageGetRotationHandle?.Invoke(ImageRotation, null);
            }
        }
        private void 图像采集toolStripButton_Click(object sender, EventArgs e)
        {
            CamGrabHandle?.Invoke(sender, e);
        }
        #endregion               

        #endregion

        #region halcon方法
        /// <summary>
        /// 在屏幕上显示文字
        /// </summary>
        /// <param name="hv_WindowHandle"></param>
        /// <param name="hv_String"></param>
        /// <param name="hv_CoordSystem"></param>
        /// <param name="hv_Row"></param>
        /// <param name="hv_Column"></param>
        /// <param name="hv_Color"></param>
        /// <param name="hv_Box"></param>
        private void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
        HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            // Local control variables 

            HTuple hv_Red, hv_Green, hv_Blue, hv_Row1Part;
            HTuple hv_Column1Part, hv_Row2Part, hv_Column2Part, hv_RowWin;
            HTuple hv_ColumnWin, hv_WidthWin, hv_HeightWin, hv_MaxAscent;
            HTuple hv_MaxDescent, hv_MaxWidth, hv_MaxHeight, hv_R1 = new HTuple();
            HTuple hv_C1 = new HTuple(), hv_FactorRow = new HTuple(), hv_FactorColumn = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Index = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_W = new HTuple(), hv_H = new HTuple();
            HTuple hv_FrameHeight = new HTuple(), hv_FrameWidth = new HTuple();
            HTuple hv_R2 = new HTuple(), hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_CurrentColor = new HTuple();

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
            //Box: If set to 'true', the text is written within a white box.
            //
            //prepare window
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
                //transform image to window coordinates
                hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
            }
            //
            //display text box depending on text size
            if ((int)(new HTuple(hv_Box.TupleEqual("true"))) != 0)
            {
                //calculate box extents
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
                //display rectangles
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                HOperatorSet.SetColor(hv_WindowHandle, "light gray");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 3, hv_C1 + 3, hv_R2 + 3, hv_C2 + 3);
                HOperatorSet.SetColor(hv_WindowHandle, "white");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            else if ((int)(new HTuple(hv_Box.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Box";
                throw new HalconException(hv_Exception);
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
            //reset changed window settings
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);

            return;
        }
        #endregion

    }

    #region Data
    /// <summary>
    /// 鼠标操作
    /// </summary>
    public enum EumMouseOperation
    {
        None,
        Move,
        Scale,
        Scale2
    }
    /// <summary>
    /// 图像背景颜色
    /// </summary>
    public enum EumControlBackColor
    {
        white,
        black,
        gray,
        blue,
        green,
        yellow,
        orange,
        pink,
        sky_blue
    }
    /// <summary>
    /// 绘制模式
    /// </summary>
    public enum EumDrawModel
    {
        /// <summary>
        /// 边缘模式
        /// </summary>
        margin,
        /// <summary>
        /// 填充模式
        /// </summary>
        fill
    }
    /// <summary>
    /// 图像旋转
    /// </summary>
    public enum EumImageRotation
    {
        /// <summary>
        /// 旋转0度
        /// </summary>
        angle_0,
        /// <summary>
        ///  旋转90度
        /// </summary>
        angle_90,
        /// <summary>
        ///  旋转180度
        /// </summary>
        angle_180,
        /// <summary>
        /// 旋转270
        /// </summary>
        angle_270,
        /// <summary>
        /// 旋转360
        /// </summary>
        angle_360

    }
    /// <summary>
    /// 常用颜色枚举
    /// </summary>
    public enum EumCommonColors
    {
        /// <summary>
        /// 绿色
        /// </summary>
        green,
        /// <summary>
        /// 红色
        /// </summary>
        red,
        /// <summary>
        /// 蓝色
        /// </summary>
        blue,
        /// <summary>
        /// 白色
        /// </summary>
        white,
        /// <summary>
        /// 黑色
        /// </summary>
        black,
        /// <summary>
        /// 粉红色
        /// </summary>
        pink,
        /// <summary>
        /// 橙色
        /// </summary>
        orange,
        /// <summary>
        /// 黄色
        /// </summary>
        yellow,


    }

    /// <summary>
    /// 像素坐标描述
    /// </summary>
    [Serializable]
    public class pixelPoint
    {
        public pixelPoint(HTuple _row, HTuple _column)
        {
            row = _row;
            column = _column;
        }
        public HTuple row { get; set; } = 0;
        public HTuple column { get; set; } = 0;
    }
    /// <summary>
    /// 文本信息描述
    /// </summary>
    [Serializable]
    internal class MessageTuple
    {
        public MessageTuple()
        {
            showMessage = new HTuple("");
            showRow = new HTuple(0);
            showCol = new HTuple(0);
            showColor = new HTuple("green");
            showSize = new HTuple(16);
        }
        public HTuple showMessage;
        public HTuple showRow;
        public HTuple showCol;
        public HTuple showColor;
        public HTuple showSize;
    }
    /// <summary>
    /// 区域信息描述
    /// </summary>
    [Serializable]
    internal class RegionsTuple
    {
        public RegionsTuple()
        {
            showRegion = new HObject();
            showColor = new HTuple("green");

        }
        public HObject showRegion;
        public HTuple showColor;

    }
    #endregion
}
