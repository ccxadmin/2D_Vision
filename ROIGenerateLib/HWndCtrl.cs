using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using HalconDotNet;

namespace ROIGenerateLib
{
	public delegate void IconicDelegate(int val);
	public delegate void FuncDelegate();
   
    /// <summary>
    /// This class works as a wrapper class for the HALCON window
    /// HWindow. HWndCtrl is in charge of the visualization.
    /// You can move and zoom the visible image part by using GUI component 
    /// inputs or with the mouse. The class HWndCtrl uses a graphics stack 
    /// to manage the iconic objects for the display. Each object is linked 
    /// to a graphical context, which determines how the object is to be drawn.
    /// The context can be changed by calling changeGraphicSettings().
    /// The graphical "modes" are defined by the class GraphicsContext and 
    /// map most of the dev_set_* operators provided in HDevelop.
    /// </summary>
    public class HWndCtrl
	{
		/// <summary>No action is performed on mouse events</summary>
		public const int MODE_VIEW_NONE       = 10;
		/// <summary>Zoom is performed on mouse events</summary>
		public const int MODE_VIEW_ZOOM       = 11;
		/// <summary>Move is performed on mouse events</summary>
		public const int MODE_VIEW_MOVE       = 12;

		/// <summary>Magnification is performed on mouse events</summary>
		public const int MODE_VIEW_ZOOMWINDOW	= 13;

		public const int MODE_INCLUDE_ROI     = 1;

		public const int MODE_EXCLUDE_ROI     = 2;

        public const int MODE_Run = 16;  //窗体运行模式

        /// <summary>
        /// Constant describes delegate message to signal new image
        /// </summary>
        public const int EVENT_UPDATE_IMAGE   = 31;
		/// <summary>
		/// Constant describes delegate message to signal error
		/// when reading an image from file
		/// </summary>
		public const int ERR_READING_IMG      = 32;
		/// <summary> 
		/// Constant describes delegate message to signal error
		/// when defining a graphical context
		/// </summary>
		public const int ERR_DEFINING_GC      = 33;

		/// <summary> 
		/// Maximum number of HALCON objects that can be put on the graphics 
		/// stack without loss. For each additional object, the first entry 
		/// is removed from the stack again.
		/// </summary>
		private const int MAXNUMOBJLIST       = 1;


		public int    stateView;
		private bool   mousePressed = false;
		private double startX,startY;

		/// <summary>HALCON window</summary>
		private HWindowControl viewPort;

		/// <summary>
		/// Instance of ROIController, which manages ROI interaction
		/// </summary>
		private ROIController roiManager;

		/* dispROI is a flag to know when to add the ROI models to the 
		   paint routine and whether or not to respond to mouse events for 
		   ROI objects */
		private int  dispROI;

        HTuple currimgWidth, currimgHeight;
        /* Basic parameters, like dimension of window and displayed image part */
        private int   windowWidth;
		private int   windowHeight;
		private int   image_showWidth;
		private int   image_showHeight;
        //图像显示区域左上角值和右下角值
        double partRow0, partCol0, partRow1, PartCol1;
        /* Image coordinates, which describe the image part that is displayed  
		   in the HALCON window */
        private double ImgRow1, ImgCol1, ImgRow2, ImgCol2;

		/// <summary>Error message when an exception is thrown</summary>
		public string  exceptionText = "";

        public EumSystemPattern currEumSystemPattern { get; set; }
        /* Delegates to send notification messages to other classes */
        /// <summary>
        /// Delegate to add information to the HALCON window after 
        /// the paint routine has finished
        /// </summary>
        public FuncDelegate   addInfoDelegate;

		/// <summary>
		/// Delegate to notify about failed tasks of the HWndCtrl instance
		/// </summary>
		public IconicDelegate NotifyIconObserver;


	
		private double  zoomWndFactor;
		private double  zoomAddOn;
		private int     zoomWndSize;

        
		/// <summary> 
		/// List of HALCON objects to be drawn into the HALCON window. 
		/// The list shouldn't contain more than MAXNUMOBJLIST objects, 
		/// otherwise the first entry is removed from the list.
		/// </summary>
		private ArrayList HObjList;

		/// <summary>
		/// Instance that describes the graphical context for the
		/// HALCON window. According on the graphical settings
		/// attached to each HALCON object, this graphical context list 
		/// is updated constantly.
		/// </summary>
		private GraphicsContext	mGC;

      
        /// <summary> 
        /// Initializes the image dimension, mouse delegation, and the 
        /// graphical context setup of the instance.
        /// </summary>
        /// <param name="view"> HALCON window </param>
        public HWndCtrl(HWindowControl view)
		{
			viewPort = view;
			stateView = MODE_VIEW_NONE;
			windowWidth = viewPort.Size.Width;
			windowHeight = viewPort.Size.Height;

			zoomWndFactor = (double)image_showWidth / viewPort.Width;
			zoomAddOn = Math.Pow(0.9, 5);
			zoomWndSize = 150;

		
			dispROI = MODE_INCLUDE_ROI;//1;

            viewPort.HMouseUp += new HalconDotNet.HMouseEventHandler(this.mouseUp);
            viewPort.HMouseDown += new HalconDotNet.HMouseEventHandler(this.mouseDown);
            viewPort.HMouseMove += new HalconDotNet.HMouseEventHandler(this.mouseMoved);
            viewPort.HMouseWheel += new HalconDotNet.HMouseEventHandler(this.mouseWheel);
            viewPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown);
            viewPort.KeyUp+= new System.Windows.Forms.KeyEventHandler(this.KeyUp);
            // graphical stack 
            HObjList = new ArrayList(20);
			mGC = new GraphicsContext();
			mGC.gcNotification = new GCDelegate(exceptionGC);

            ROIChangeHandleList.Clear();

        }

    


        /// <summary>
        /// Registers an instance of an ROIController with this window 
        /// controller (and vice versa).
        /// </summary>
        /// <param name="rC"> 
        /// Controller that manages interactive ROIs for the HALCON window 
        /// </param>
        public void useROIController(ROIController rC)
		{
			roiManager = rC;
            rC.ROIUpdateNotify_HWndCtrl_Handle += new EventHandler(ROIUpdateNotify_HWndCtrl_Event);
		}
        public ROIController getROIController()
        {
            return this.roiManager;
        }


        public void ResetROI()
        {
            if (roiManager != null)
            {
                roiManager.setROISign(ROIController.MODE_ROI_NONE);
                roiManager.resetROI();
            }
            
        }

        /// <summary>
        /// Adjust window settings by the values supplied for the left 
        /// upper corner and the right lower corner
        /// </summary>
        /// <param name="r1">y coordinate of left upper corner</param>
        /// <param name="c1">x coordinate of left upper corner</param>
        /// <param name="r2">y coordinate of right lower corner</param>
        /// <param name="c2">x coordinate of right lower corner</param>
        private void setImagePart(int r1, int c1, int r2, int c2)
		{
			ImgRow1 = r1;
			ImgCol1 = c1;
            ImgRow2 = r2;
            ImgCol2 = c2;
            image_showHeight = r2-r1+1;
		   image_showWidth = c2-c1+1;
           // viewPort.HalconWindow.SetPart(r1, c1, r2, c2);

            System.Drawing.Rectangle rect = viewPort.ImagePart;
            rect.X = (int)ImgCol1;
            rect.Y = (int)ImgRow1;
            rect.Height = (int)image_showHeight;
            rect.Width = (int)image_showWidth;
            viewPort.ImagePart = rect;

        }
		/// <summary>
		/// Sets the view mode for mouse events in the HALCON window
		/// (zoom, move, magnify or none).
		/// </summary>
		/// <param name="mode">One of the MODE_VIEW_* constants</param>
		public void setViewState(int mode)
		{
			stateView = mode;

			//if (roiManager != null)
			//	roiManager.resetROI();
		}

		/********************************************************************/
	
		/*******************************************************************/
		private void exceptionGC(string message)
		{
			exceptionText = message;
			NotifyIconObserver(ERR_DEFINING_GC);
		}

		/// <summary>
		/// Paint or don't paint the ROIs into the HALCON window by 
		/// defining the parameter to be equal to 1 or not equal to 1.
		/// </summary>
		public void setDispLevel(int mode)
		{
			dispROI = mode;
		}

		/****************************************************************************/
		/*                          graphical element                               */
		/****************************************************************************/
		public void zoomImage(double x, double y, double scale)
		{
			double lengthC, lengthR;
			double percentC, percentR;
			int    lenC, lenR;

			percentC = (x - ImgCol1) / (ImgCol2 - ImgCol1);
			percentR = (y - ImgRow1) / (ImgRow2 - ImgRow1);


			lengthC = (ImgCol2 - ImgCol1) * scale;
			lengthR = (ImgRow2 - ImgRow1) * scale;

			ImgCol1 = x - lengthC * percentC;
			ImgCol2 = x + lengthC * (1 - percentC);

			ImgRow1 = y - lengthR * percentR;
			ImgRow2 = y + lengthR * (1 - percentR);

			lenC = (int)Math.Round(lengthC);
			lenR = (int)Math.Round(lengthR);

			System.Drawing.Rectangle rect = viewPort.ImagePart;
			rect.X = (int)Math.Round(ImgCol1);
			rect.Y = (int)Math.Round(ImgRow1);
			rect.Width = (lenC > 0) ? lenC : 1;
			rect.Height = (lenR > 0) ? lenR : 1;
			viewPort.ImagePart = rect;

			zoomWndFactor *= scale;
			
		}

		/// <summary>
		/// Scales the image in the HALCON window according to the 
		/// value scaleFactor
		/// </summary>
		private void zoomImage(double scaleFactor)
		{
			double midPointX, midPointY;

			if (((ImgRow2 - ImgRow1) == scaleFactor * image_showHeight) &&
				((ImgCol2 - ImgCol1) == scaleFactor * image_showWidth))
			{
				repaint();
				return;
			}

			ImgRow2 = ImgRow1 + image_showHeight;
			ImgCol2 = ImgCol1 + image_showWidth;

			midPointX = ImgCol1;
			midPointY = ImgRow1;

			zoomWndFactor = (double)image_showWidth / viewPort.Width;
			zoomImage(midPointX, midPointY, scaleFactor);
		}


		/// <summary>
		/// Scales the HALCON window according to the value scale
		/// </summary>
		public void scaleWindow(double scale)
		{
			ImgRow1 = 0;
			ImgCol1 = 0;

			ImgRow2 = image_showHeight;
			ImgCol2 = image_showWidth;

			viewPort.Width = (int)(ImgCol2 * scale);
			viewPort.Height = (int)(ImgRow2 * scale);

			zoomWndFactor = ((double)image_showWidth / viewPort.Width);
		}

		/// <summary>
		/// Recalculates the image-window-factor, which needs to be added to 
		/// the scale factor for zooming an image. This way the zoom gets 
		/// adjusted to the window-image relation, expressed by the equation 
		/// imageWidth/viewPort.Width.
		/// </summary>
		public void setZoomWndFactor()
		{
			zoomWndFactor = ((double)image_showWidth / viewPort.Width);
		}

		/// <summary>
		/// Sets the image-window-factor to the value zoomF
		/// </summary>
		public void setZoomWndFactor(double zoomF)
		{
			zoomWndFactor = zoomF;
		}

		/*******************************************************************/
		public void moveImage(double motionX, double motionY)
		{
          
            ImgRow1 += -motionY;
			ImgRow2 += -motionY;

			ImgCol1 += -motionX;
			ImgCol2 += -motionX;

         //   setImagePart((int)ImgRow1, (int)ImgCol1, (int)ImgRow2-1, (int)ImgCol2-1);

            System.Drawing.Rectangle rect = viewPort.ImagePart;
            rect.X = (int)Math.Round(ImgCol1);
            rect.Y = (int)Math.Round(ImgRow1);
            viewPort.ImagePart = rect;

        }

		/// <summary>
		/// Resets all parameters that concern the HALCON window display 
		/// setup to their initial values and clears the ROI list.
		/// </summary>
		public void FitWindowsSize()
		{
            int winRow, winCol, winWidth, winHeight;
            viewPort.HalconWindow.GetWindowExtents(out winRow, out winCol, out winWidth, out winHeight);
            if (currimgWidth == null) return;
            //判断行缩放还是列缩放
            double scaleC = currimgWidth.D / winWidth;
            double scaleR = currimgHeight.D / winHeight;

            double w, h;
            if (scaleC < scaleR)
            {

                h = winHeight * scaleR;
                w = winWidth * scaleR;
                partRow0 = 0;
                partCol0 = (currimgWidth - w) / 2.0;
            }
            else
            {
                h = winHeight * scaleC;
                w = winWidth * scaleC;

                partRow0 = (currimgHeight - h) / 2.0;
                partCol0 = 0;

            }
            partRow1 = partRow0 + h-1 ;
            PartCol1 = partCol0 + w-1 ;

            zoomWndFactor = w / winWidth;
            setImagePart((int)partRow0, (int)partCol0, (int)partRow1, (int)PartCol1);


        }


        /*************************************************************************/
        /*      			 Event handling for mouse	   	                     */
        /*************************************************************************/
        /*************************************************************************/
        private void mouseWheel(object sender, HalconDotNet.HMouseEventArgs e)
        {

            //设计模式
            if (currEumSystemPattern == EumSystemPattern.DesignModel)
            {
                List<int> activeROIidx = new List<int>();
                double scale;

                if (roiManager != null && (dispROI == MODE_INCLUDE_ROI))
                {
                    activeROIidx = roiManager.mouseDownAction(e.X, e.Y);
                }

                if (activeROIidx.Count==0)
                {
                    switch (stateView)
                    {
                        case MODE_VIEW_MOVE:
                            startX = e.X;
                            startY = e.Y;
                            break;
                        case MODE_VIEW_ZOOM:
                            if (e.Delta >= 0)
                                scale = 0.9;
                            else
                                scale = 1 / 0.9;
                            zoomImage(e.X, e.Y, scale);
                            repaint();
                            break;
                        case MODE_VIEW_NONE:
                            break;
                                        
                    }
                }

            }
        }
        /*************************************************************************/
        private void mouseDown(object sender, HalconDotNet.HMouseEventArgs e)
		{
           
            //左键使能
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mousePressed = true;

                //设计模式
                if (currEumSystemPattern == EumSystemPattern.DesignModel)
                {
                    List<int> activeROIidx = new List<int>();
                    //double scale;

                  

                    if (roiManager != null && (dispROI == MODE_INCLUDE_ROI))
                    {
                        activeROIidx = roiManager.mouseDownAction(e.X, e.Y);
                    }                 

                    if (activeROIidx.Count==0)
                    {
                        switch (stateView)
                        {
                            case MODE_VIEW_MOVE:
                                startX = e.X;
                                startY = e.Y;
                                break;
                           case MODE_VIEW_ZOOM:                       
                              break;
                            case MODE_VIEW_NONE:
                                break;
                          
                        }
                    }
                    //end of if
                }
            }
        }
        /*******************************************************************/
     
        void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                if(roiManager!=null)
                    roiManager.IsControlPressFlag = true;
            }
               
        }
                            
        void KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode== Keys.ControlKey)
            {
                if (roiManager != null)
                    roiManager.IsControlPressFlag = false;
            }
              

        }
        /*******************************************************************/
        private void mouseUp(object sender, HalconDotNet.HMouseEventArgs e)
		{
           
            //左键使能
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mousePressed = false;

                //设计模式
                if (currEumSystemPattern == EumSystemPattern.DesignModel)
                {
                    if (roiManager != null
                    && (roiManager.activeROIidx.Count > 0)
                    && (dispROI == MODE_INCLUDE_ROI))
                    {
                        if (roiManager.HwinOpreateROI.Count <= 0 || roiManager.HwinMouseOpreate ==
                            EumROIupdate.none)
                            return;
                        roiManager.NotifyRCObserver(ROIController.EVENT_UPDATE_ROI);
                        if (roiManager.HwinMouseOpreate != EumROIupdate.delete)
                        {
                            //有则更新无则增加,此处为事件参数缓存，等待下一步操作
                            foreach (var s in roiManager.activeROIidx)
                                if (ROIChangeHandleList.IsConstainKey(s))
                                    ROIChangeHandleList[s] = new ArrayList(2) { roiManager.getsetROIList[s], roiManager.HwinMouseOpreate };
                                else
                                    ROIChangeHandleList.Add(s, (roiManager.getsetROIList[s]), roiManager.HwinMouseOpreate);

                            foreach (var s in ROIChangeHandleList.Dictionary1)
                            {
                                int keyvalue = s.Key;
                                ROIUpdateNotifyHandle?.Invoke(s.Value,
                                    ROIChangeHandleList.Dictionary2[keyvalue]);
                            }
                            //roiManager.HwinOpreateROI.Clear();
                            ROIChangeHandleList.Clear();

                        }
                        else
                        {
                            foreach (var s in roiManager.HwinOpreateROI)
                                ROIUpdateNotifyHandle?.Invoke(s,
                                     EumROIupdate.delete);
                            ROIChangeHandleList.Clear();

                        }
                    }
                    /*
                     * 2023.4.10 暂不使用此编辑模式
                     */
                    //没有ROI处于激活状态，此时可触发事件
                    //else if (roiManager != null
                    //&& (roiManager.activeROIidx.Count == 0)
                    //&& (dispROI == MODE_INCLUDE_ROI))
                    //{
                    //    foreach (var s in ROIChangeHandleList.Dictionary1)
                    //    {
                    //        int keyvalue = s.Key;
                    //        ROIUpdateNotifyHandle?.Invoke(s.Value,
                    //            ROIChangeHandleList.Dictionary2[keyvalue]);
                    //    }
                     
                    //    ROIChangeHandleList.Clear();
                    //}


                }
               
            }
        }

        Dictionaryexpand<int, ROI, EumROIupdate> ROIChangeHandleList = new Dictionaryexpand<int, ROI, EumROIupdate>();
        public delegate void deleROIUpdateNotify(ROI currRegionList, EumROIupdate EumROIupdateArgs);
        public deleROIUpdateNotify ROIUpdateNotifyHandle;

        /*******************************************************************/
       
        private void mouseMoved(object sender, HalconDotNet.HMouseEventArgs e)
		{
			double motionX, motionY;

            if (!mousePressed)
                return;
            //设计模式
            if (currEumSystemPattern == EumSystemPattern.DesignModel)
            {
                if (roiManager != null && (roiManager.activeROIidx.Count >0) && (dispROI == MODE_INCLUDE_ROI))
                {
                    roiManager.mouseMoveAction(e.X, e.Y);
                }
                else if (stateView == MODE_VIEW_MOVE)
                {
                    motionX = ((e.X - startX));
                    motionY = ((e.Y - startY));

                    if (((int)motionX != 0) || ((int)motionY != 0))
                    {
                        moveImage(motionX, motionY);
                        repaint();
                        startX = e.X - motionX;
                        startY = e.Y - motionY;
                    }
                }
            
            }
		}
        /*******************************************************************/
      
        /// <summary>
        /// Triggers a repaint of the HALCON window
        /// </summary>
        public void repaint()
		{
			repaint(viewPort.HalconWindow);
		}
        void ROIUpdateNotify_HWndCtrl_Event(object sender, EventArgs e)
        {       
            repaint(viewPort.HalconWindow);
           
        }
        /// <summary>
        /// Repaints the HALCON window 'window'
        /// </summary>
        private void repaint(HalconDotNet.HWindow window)
		{
			int count = HObjList.Count;
			HObjectEntry entry;
            //HSystem.SetSystem("flush_graphic", "false");
            window.ClearWindow();
            //HSystem.SetSystem("flush_graphic", "true");
            mGC.stateOfSettings.Clear();
			for (int i=0; i < count; i++)
			{
				entry = ((HObjectEntry)HObjList[i]);
				mGC.applyContext(window, entry.gContext);
				window.DispObj(entry.HObj);
			}
            addInfoDelegate?.Invoke();
        
            //设计模式
            if (currEumSystemPattern == EumSystemPattern.DesignModel)
            {
                if (roiManager != null && (dispROI == MODE_INCLUDE_ROI))
                    roiManager.paintData(window);
            }

            //HSystem.SetSystem("flush_graphic", "true");
            //window.SetColor("white");
            //window.DispLine(-100.0, -100.0, -101.0, -101.0);
        }

        /********************************************************************/
        /*                      GRAPHICSSTACK                               */
        /********************************************************************/

        /// <summary>
        /// Adds an iconic object to the graphics stack similar to the way
        /// it is defined for the HDevelop graphics stack.
        /// </summary>
        /// <param name="obj">Iconic object</param>
        public void addIconicVar(HObject Img)
        {
           
            int winRow, winCol, winWidth, winHeight;
          
            HObjectEntry entry;
            if (Img == null)
                return;

            //清除图像队列,每次保存一张
            clearList();
            HOperatorSet.GetImageSize(Img, out currimgWidth, out currimgHeight);
            if (imageWid == null)
                imageWid = currimgWidth;
            // image.GetImageSize(out imgWidth, out imgHeight);
            viewPort.HalconWindow.GetWindowExtents(out winRow, out winCol, out winWidth, out winHeight);

            //判断行缩放还是列缩放
            double scaleC = currimgWidth.D / winWidth;
            double scaleR = currimgHeight.D / winHeight;

            double w, h;
            if (scaleC < scaleR)
            {

                h = winHeight * scaleR;
                w = winWidth * scaleR;
                partRow0 = 0;
                partCol0 = (currimgWidth - w) / 2.0;
            }
            else
            {
                h = winHeight * scaleC;
                w = winWidth * scaleC;

                partRow0 = (currimgHeight - h) / 2.0;
                partCol0 = 0;

            }
            partRow1 = partRow0 + h-1 ;
            PartCol1 = partCol0 + w-1 ;
            
            zoomWndFactor = w / winWidth;
            if(eumFitWindows== EumFitWindows.First||
                  imageWid.I != currimgWidth.I)
            {
                imageWid = currimgWidth;
                setImagePart((int)partRow0, (int)partCol0, (int)partRow1, (int)PartCol1);
                eumFitWindows = EumFitWindows.NotFirst;
            }
           

            entry = new HObjectEntry(Img, mGC.copyContextList());

            HObjList.Add(entry);

            if (HObjList.Count > MAXNUMOBJLIST)
                HObjList.RemoveAt(1);
        }

        HTuple imageWid = null;
        EumFitWindows eumFitWindows = EumFitWindows.First;
        enum EumFitWindows
        {
           First,
           NotFirst

        }

		/// <summary>
		/// Clears all entries from the graphics stack 
		/// </summary>
		public void clearList()
		{
			HObjList.Clear();
		}


		/// <summary>
		/// Returns the number of items on the graphics stack
		/// </summary>
		public int getListCount()
		{
			return HObjList.Count;
		}

		/// <summary>
		/// Changes the current graphical context by setting the specified mode
		/// (constant starting by GC_*) to the specified value.
		/// </summary>
		/// <param name="mode">
		/// Constant that is provided by the class GraphicsContext
		/// and describes the mode that has to be changed, 
		/// e.g., GraphicsContext.GC_COLOR
		/// </param>
		/// <param name="val">
		/// Value, provided as a string, 
		/// the mode is to be changed to, e.g., "blue" 
		/// </param>
		public void changeGraphicSettings(string mode, string val)
		{
			switch (mode)
			{
				case GraphicsContext.GC_COLOR:
					mGC.setColorAttribute(val);
					break;
				case GraphicsContext.GC_DRAWMODE:
					mGC.setDrawModeAttribute(val);
					break;
				case GraphicsContext.GC_LUT:
					mGC.setLutAttribute(val);
					break;
				case GraphicsContext.GC_PAINT:
					mGC.setPaintAttribute(val);
					break;
				case GraphicsContext.GC_SHAPE:
					mGC.setShapeAttribute(val);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Changes the current graphical context by setting the specified mode
		/// (constant starting by GC_*) to the specified value.
		/// </summary>
		/// <param name="mode">
		/// Constant that is provided by the class GraphicsContext
		/// and describes the mode that has to be changed, 
		/// e.g., GraphicsContext.GC_LINEWIDTH
		/// </param>
		/// <param name="val">
		/// Value, provided as an integer, the mode is to be changed to, 
		/// e.g., 5 
		/// </param>
		public void changeGraphicSettings(string mode, int val)
		{
			switch (mode)
			{
				case GraphicsContext.GC_COLORED:
					mGC.setColoredAttribute(val);
					break;
				case GraphicsContext.GC_LINEWIDTH:
					mGC.setLineWidthAttribute(val);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Changes the current graphical context by setting the specified mode
		/// (constant starting by GC_*) to the specified value.
		/// </summary>
		/// <param name="mode">
		/// Constant that is provided by the class GraphicsContext
		/// and describes the mode that has to be changed, 
		/// e.g.,  GraphicsContext.GC_LINESTYLE
		/// </param>
		/// <param name="val">
		/// Value, provided as an HTuple instance, the mode is 
		/// to be changed to, e.g., new HTuple(new int[]{2,2})
		/// </param>
		public void changeGraphicSettings(string mode, HTuple val)
		{
			switch (mode)
			{
				case GraphicsContext.GC_LINESTYLE:
					mGC.setLineStyleAttribute(val);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Clears all entries from the graphical context list
		/// </summary>
		public void clearGraphicContext()
		{
			mGC.clear();
		}

		/// <summary>
		/// Returns a clone of the graphical context list (hashtable)
		/// </summary>
		public Hashtable getGraphicContext()
		{
			return mGC.copyContextList();
		}

	}//end of class

    public class Dictionaryexpand<TKey, TValue1, TValue2> :  IEnumerable
    {
        public Dictionaryexpand()
        {
            Dictionary1 = new Dictionary<TKey, TValue1>();
            Dictionary2 = new Dictionary<TKey, TValue2>();
        }
       public Dictionary<TKey, TValue1> Dictionary1 { get; set; }
        public Dictionary<TKey, TValue2> Dictionary2 { get; set; }

        public ArrayList this[TKey key]      
        {
            get
            {
               return  new ArrayList(2) { Dictionary1[key], Dictionary1[key] };
            }
            set
            {
                Dictionary1[key] = (TValue1)value[0];
                Dictionary2[key] = (TValue2)value[1];
            }
           
        }

        public  int Count
        {
            get => Dictionary1.Count;
        }

     
        public void Add(TKey key, TValue1 value1, TValue2 value2)
        {
            Dictionary1.Add(key, value1);
            Dictionary2.Add(key, value2);
        }
         public void Clear()
        {
            Dictionary1.Clear();
            Dictionary2.Clear();

        }

  
        public void Remove(TKey key)
        {
            Dictionary1.Remove(key);
            Dictionary2.Remove(key);
        }
        public bool IsConstainKey(TKey key)
        {
            return Dictionary1.ContainsKey(key);
        }
          
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }



}//end of namespace
