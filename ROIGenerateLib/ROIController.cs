using System;
using HalconDotNet;
using System.Collections;
using System.Collections.Generic;

namespace ROIGenerateLib
{

	public delegate void FuncROIDelegate();

    /// <summary>
    /// This class creates and manages ROI objects. It responds 
    /// to  mouse device inputs using the methods mouseDownAction and 
    /// mouseMoveAction. You don't have to know this class in detail when you 
    /// build your own C# project. But you must consider a few things if 
    /// you want to use interactive ROIs in your application: There is a
    /// quite close connection between the ROIController and the HWndCtrl 
    /// class, which means that you must 'register' the ROIController
    /// with the HWndCtrl, so the HWndCtrl knows it has to forward user input
    /// (like mouse events) to the ROIController class.  
    /// The visualization and manipulation of the ROI objects is done 
    /// by the ROIController.
    /// This class provides special support for the matching
    /// applications by calculating a model region from the list of ROIs. For
    /// this, ROIs are added and subtracted according to their sign.
    /// </summary>
    [Serializable]
    public class ROIController
	{

        public bool IsControlPressFlag { get; set; }
		/// <summary>
		/// Constant for setting the ROI mode: positive ROI sign.
		/// </summary>
		public const int MODE_ROI_POS           = 21;

		/// <summary>
		/// Constant for setting the ROI mode: negative ROI sign.
		/// </summary>
		public const int MODE_ROI_NEG           = 22;

		/// <summary>
		/// Constant for setting the ROI mode: no model region is computed as
		/// the sum of all ROI objects.
		/// </summary>
		public const int MODE_ROI_NONE          = 23;

		/// <summary>Constant describing an update of the model region</summary>
		public const int EVENT_UPDATE_ROI       = 50;

		public const int EVENT_CHANGED_ROI_SIGN = 51;

		/// <summary>Constant describing an update of the model region</summary>
		public const int EVENT_MOVING_ROI       = 52;

		public const int EVENT_DELETED_ACTROI  	= 53;

		public const int EVENT_DELETED_ALL_ROIS = 54;

		public const int EVENT_ACTIVATED_ROI   	= 55;

		public const int EVENT_CREATED_ROI   	  = 56;


		private ROI     roiMode;
		private int     stateROI;
		private double  currX, currY;


        /// <summary>Index of the active ROI object</summary>
       public  List<int> activeROIidx = new List<int>();
        public int      deletedIdx;

        /// <summary>��������Ϊֹ����������ROI������б�</summary>
        private List<ROI> ROIList;

		/// <summary>
		/// Region obtained by summing up all negative 
		/// and positive ROI objects from the ROIList 
		/// </summary>
		public HRegion ModelROI;

		private string activeCol    = "green";
		private string activeHdlCol = "red";
		private string inactiveCol  = "yellow";

		/// <summary>
		/// Reference to the HWndCtrl, the ROI Controller is registered to
		/// </summary>
		//public HWndCtrl viewController;
        //ROI����֪ͨ�����ػ��¼�
        public EventHandler ROIUpdateNotify_HWndCtrl_Handle;

        public EumROIupdate HwinMouseOpreate { get; set; }
        /// <summary>
        /// ��������ROI����
        /// </summary>
        public List<ROI> HwinOpreateROI { get; set; }
        /// <summary>
        /// Delegate that notifies about changes made in the model region
        /// </summary>
        public IconicDelegate  NotifyRCObserver;

		/// <summary>Constructor</summary>
		public ROIController()
		{
			stateROI = MODE_ROI_NONE;
			ROIList = new List<ROI>();
            activeROIidx = new List<int>();

            //activeROIidx = -1;
			ModelROI = new HRegion();
			NotifyRCObserver = new IconicDelegate(dummyI);
			deletedIdx = -1;
			currX = currY = -1;
            HwinOpreateROI = new List<ROI>();

        }

		/// <summary>Registers the HWndCtrl to this ROIController instance</summary>
		//public void setViewController(HWndCtrl view)
		//{
		//	viewController = view;
		//}

		/// <summary>Gets the ModelROI object</summary>
		public HRegion getModelRegion()
		{
			return ModelROI;
		}

		/// <summary>Gets the List of ROIs created so far</summary>
		public List<ROI> getsetROIList
		{
            get
            {
                return ROIList;
            }
            set
            {
                this.ROIList = value;
            }
		}

       
		/// <summary>Get the active ROI</summary>
		public List<ROI>  getActiveROI()
		{
            List<ROI> roiList = new List<ROI>();
            foreach (var s in activeROIidx)
            {
                if (s != -1)
                    roiList.Add(ROIList[s]);             
            }
		
			return roiList;
		}	
		/// <summary>
		/// To create a new ROI object the application class initializes a 
		/// 'seed' ROI instance and passes it to the ROIController.
		/// The ROIController now responds by manipulating this new ROI
		/// instance.
		/// </summary>
		/// <param name="r">
		/// 'Seed' ROI object forwarded by the application forms class.
		/// </param>
		public void setROIShape(ROI r)
		{
			roiMode = r;
			roiMode.setOperatorFlag(stateROI);
		}


		/// <summary>
		/// Sets the sign of a ROI object to the value 'mode' (MODE_ROI_NONE,
		/// MODE_ROI_POS,MODE_ROI_NEG)
		/// </summary>
		public void setROISign(int mode,int index=0)
		{
			stateROI = mode;
            if (activeROIidx.Count <= 0) return;

            
				ROIList[index].setOperatorFlag(stateROI);
			
				NotifyRCObserver(ROIController.EVENT_CHANGED_ROI_SIGN);
			
		}

		/// <summary>
		/// Removes the ROI object that is marked as active. 
		/// If no ROI object is active, then nothing happens. 
		/// </summary>
		public void removeActive()
		{
            HwinOpreateROI.Clear();
            HwinMouseOpreate = EumROIupdate.none;
            List<ROI> temdata = new List<ROI>();
            foreach (var s in activeROIidx)
            {
                if (s != -1)
                {
                    temdata.Add(ROIList[s]);                  
                    deletedIdx = s;                
                }
            }
            foreach (var m in temdata)
            {
                HwinOpreateROI.Add(m);
                ROIList.Remove(m);
            }
         
            temdata = null;
            HwinMouseOpreate = EumROIupdate.delete;
            activeROIidx.Clear();
            //viewController.repaint();
            ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null,null);
            NotifyRCObserver(EVENT_DELETED_ACTROI);  


		}

        /// <summary>
        /// ѡ�м���ROI
        /// </summary>
        /// <param name="index"></param>
        /// <param name="eumSystemPattern"></param>
        public void selectROI(int index, EumSystemPattern eumSystemPattern)
        {
            if (eumSystemPattern == EumSystemPattern.RunningModel) return;
            this.activeROIidx.Clear();
            if (index >=0 || index <=ROIList.Count - 1)
                this.activeROIidx.Add(index);
            //this.activeROIidx[0] = index;
            this.NotifyRCObserver(ROIController.EVENT_ACTIVATED_ROI);
            ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);
        }
        /// <summary>
        /// Calculates the ModelROI region for all objects contained 
        /// in ROIList, by adding and subtracting the positive and 
        /// negative ROI objects.
        /// </summary>
        public bool defineModelROI()
		{
			HRegion tmpAdd, tmpDiff, tmp;
			double row, col;

            List<ROI> delebuffer = new List<ROI>();
            HwinOpreateROI.Clear();
            HwinMouseOpreate = EumROIupdate.none;

            tmpAdd = new HRegion();
			tmpDiff = new HRegion();
      tmpAdd.GenEmptyRegion();
      tmpDiff.GenEmptyRegion();

			for (int i=0; i < ROIList.Count; i++)
			{
				switch (ROIList[i].getOperatorFlag())
				{
					case ROI.POSITIVE_FLAG:                   
                        tmp = ROIList[i].getRegion();
                        delebuffer.Add(ROIList[i]);
                        tmpAdd = tmp.Union2(tmpAdd);
						break;
					case ROI.NEGATIVE_FLAG:                     
                        tmp = ROIList[i].getRegion();
                        delebuffer.Add(ROIList[i]);
                        tmpDiff = tmp.Union2(tmpDiff);
						break;
					
				}//end of switch
			}//end of for
            activeROIidx.Clear();
            setROISign(ROIController.MODE_ROI_NONE);
            foreach(var ss in delebuffer)
                 ROIList.Remove(ss);
            ModelROI = null;
            delebuffer.Clear();

            if (tmpAdd.AreaCenter(out row, out col) > 0)
			{
				tmp = tmpAdd.Difference(tmpDiff);
				if (tmp.AreaCenter(out row, out col) > 0)
					ModelROI = tmp;

			}
        
            HwinOpreateROI.Add(new ROICombine(ModelROI));
            HwinMouseOpreate = EumROIupdate.combine;
            //in case the set of positiv and negative ROIs dissolve 
            if (ModelROI == null || ROIList.Count == 0)
				return false;
          
            return true;
		}


		/// <summary>
		/// Clears all variables managing ROI objects
		/// </summary>
		public void resetROI()
		{
			ROIList.Clear();
			activeROIidx.Clear();
			ModelROI = null;
			roiMode = null;
			NotifyRCObserver(EVENT_DELETED_ALL_ROIS);
		}

		/// <summary>Defines the colors for the ROI objects</summary>
		/// <param name="aColor">Color for the active ROI object</param>
		/// <param name="inaColor">Color for the inactive ROI objects</param>
		/// <param name="aHdlColor">
		/// Color for the active handle of the active ROI object
		/// </param>
		public void setDrawColor_ROIDesign(string aColor,
								   string aHdlColor,
								   string inaColor)
		{
			if (aColor != "")
				activeCol = aColor;
			if (aHdlColor != "")
				activeHdlCol = aHdlColor;
			if (inaColor != "")
				inactiveCol = inaColor;
		}

     

        /// <summary>
        /// Paints all objects from the ROIList into the HALCON window
        /// </summary>
        /// <param name="window">HALCON window</param>
        public void paintData(HalconDotNet.HWindow window)
		{
			//window.SetDraw("margin");
			window.SetLineWidth(1);

			if (ROIList.Count > 0)
			{
				window.SetColor(inactiveCol);
			//	window.SetDraw("margin");

				for (int i=0; i < ROIList.Count; i++)
				{
					window.SetLineStyle(ROIList[i].flagLineStyle);
					ROIList[i].draw(window);
				}

                foreach (var s in activeROIidx)
                {
                    if (s == -1) continue;
                    window.SetColor(activeCol);
                    window.SetLineStyle(ROIList[s].flagLineStyle);
                   ROIList[s].draw(window);

                    window.SetColor(activeHdlCol);
                   ROIList[s].displayActive(window);
                }	
			}

            window.SetLineStyle(new HTuple());  //������ʽ�ظ�Ĭ��ֵ
        }

		/// <summary>
		/// Reaction of ROI objects to the 'mouse button down' event: changing
		/// the shape of the ROI and adding it to the ROIList if it is a 'seed'
		/// ROI.
		/// </summary>
		/// <param name="imgX">x coordinate of mouse event</param>
		/// <param name="imgY">y coordinate of mouse event</param>
		/// <returns></returns>
		public List<int> mouseDownAction(double imgX, double imgY)
		{
			int idxROI= -1;
			double max = 10000, dist = 0;
			double epsilon = 35.0;          //maximal shortest distance to one of
        //    HwinOpreateROI.Clear();
       //     HwinMouseOpreate = EumROIupdate.none;                              //the handles

            if (roiMode != null)			 //either a new ROI object is created
			{
                HwinOpreateROI.Clear();
                HwinMouseOpreate = EumROIupdate.none;

                roiMode.createROI(imgX, imgY);
				ROIList.Add(roiMode);
              
                HwinOpreateROI.Add(roiMode);
                HwinMouseOpreate = EumROIupdate.create;
                roiMode = null;             
                activeROIidx.Clear();
                activeROIidx.Add(ROIList.Count - 1);
         
                //viewController.repaint();
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);
                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
			}
			else if (ROIList.Count > 0)		// ... or an existing one is manipulated
			{
				//activeROIidx.Clear();
				for (int i =0; i < ROIList.Count; i++)
				{
					dist = ROIList[i].distToClosestHandle(imgX, imgY);
					if ((dist < max) && (dist < epsilon))
					{
						max = dist;
						idxROI = i;
					}
				}//end of for
				if (idxROI >= 0)
				{
                    if (!IsControlPressFlag)
                        activeROIidx.Clear();
                    activeROIidx.Add( idxROI);
					NotifyRCObserver(ROIController.EVENT_ACTIVATED_ROI);
				}
                else
                    activeROIidx.Clear();
                //viewController.repaint();
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);
            }
			return activeROIidx;
		}

		/// <summary>
		/// Reaction of ROI objects to the 'mouse button move' event: moving
		/// the active ROI.
		/// </summary>
		/// <param name="newX">x coordinate of mouse event</param>
		/// <param name="newY">y coordinate of mouse event</param>
		public void mouseMoveAction(double newX, double newY)
		{
            HwinOpreateROI.Clear();
            HwinMouseOpreate = EumROIupdate.none;
            if ((newX == currX) && (newY == currY))
				return;
           
            foreach (var s in activeROIidx)
                ROIList[s].moveByHandle(newX, newY);

            //viewController.repaint();
            ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);
            currX = newX;
			currY = newY;

            foreach (var s in activeROIidx)         
                HwinOpreateROI.Add(ROIList[s]);
            HwinMouseOpreate = EumROIupdate.move;
        


        NotifyRCObserver(ROIController.EVENT_MOVING_ROI);
		}

        public void AddPolygnPoint()
        {
            int count = activeROIidx.Count;
            if (count <= 0) return;
            ROIPolygon tem =   ROIList[activeROIidx[0]] as ROIPolygon;

            if (tem == null) return;
            tem.AddNewPoint();
            //viewController.repaint();
            ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);
        }

        public void DelSelectPoint()
        {
            int count = activeROIidx.Count;
            if (count <= 0) return;
            ROIPolygon tem = ROIList[activeROIidx[0]] as ROIPolygon;

            if (tem == null) return;
            tem.DelectSelectPoint();
            //viewController.repaint();
            ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);
        }
        /***********************************************************/
    
		public void dummyI(int v)
		{
            

        }

        /*************************��ָ����λ������ROI*****************************************/
        /// <summary>
        /// ��ָ��λ������ROI--Rectangle1
        /// </summary>
        /// <param name="row1"></param>
        /// <param name="col1"></param>
        /// <param name="row2"></param>
        /// <param name="col2"></param>
        /// <param name="rois"></param>
        public  void genRect1(double row1, double col1, double row2, double col2, ref List<ROI> rois)
        {
            setROIShape(new ROIRectangle1());

            if (rois == null)
            {
                rois = new List<ROI>();
            }

            if (roiMode != null)			 //either a new ROI object is created
            {
                roiMode.createRectangle1(row1, col1, row2, col2);
               
                rois.Add(roiMode);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx.Clear();
                activeROIidx.Add(ROIList.Count - 1);
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);//֪ͨ���»���
                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
        }
        /// <summary>
        /// ��ָ��λ������ROI--Rectangle2
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="phi"></param>
        /// <param name="length1"></param>
        /// <param name="length2"></param>
        /// <param name="rois"></param>
        public void genRect2(double row, double col, double phi, double length1, double length2, ref List<ROI> rois)
        {
            setROIShape(new ROIRectangle2());

            if (rois == null)
            {
                rois = new List<ROI>();
            }

            if (roiMode != null)			 //either a new ROI object is created
            {
                roiMode.createRectangle2(row, col, phi, length1, length2);
               
                rois.Add(roiMode);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx.Clear();
                activeROIidx.Add(ROIList.Count - 1);
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);//֪ͨ���»���
                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
        }
        /// <summary>
        /// ��ָ��λ������ROI--Circle
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="radius"></param>
        /// <param name="rois"></param>
        public void genCircle(double row, double col, double radius, ref List<ROI> rois)
        {
            setROIShape(new ROICircle());

            if (rois == null)
            {
                rois = new List<ROI>();
            }

            if (roiMode != null)			 //either a new ROI object is created
            {
                roiMode.createCircle(row, col, radius);
              
                rois.Add(roiMode);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx.Clear();
                activeROIidx.Add(ROIList.Count - 1);
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);//֪ͨ���»���
                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
        }
        /// <summary>
        ///  ��ָ��λ������ROI--CircularArc
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="radius"></param>
        /// <param name="startPhi"></param>
        /// <param name="extentPhi"></param>
        /// <param name="direct"></param>
        /// <param name="rois"></param>
        public void genCircularArc(double row, double col, double radius, double startPhi, double extentPhi, string direct, ref List<ROI> rois)
        {
            setROIShape(new ROICircularArc());

            if (rois == null)
            {
                rois = new List<ROI>();
            }

            if (roiMode != null)			 //either a new ROI object is created
            {
                roiMode.createCircularArc(row, col, radius, startPhi, extentPhi, direct);
               
                rois.Add(roiMode);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx.Clear();
                activeROIidx.Add(ROIList.Count - 1);
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);//֪ͨ���»���
                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
        }
        /// <summary>
        /// ��ָ��λ������ROI--Line
        /// </summary>
        /// <param name="beginRow"></param>
        /// <param name="beginCol"></param>
        /// <param name="endRow"></param>
        /// <param name="endCol"></param>
        /// <param name="rois"></param>
        public void genLine(double beginRow, double beginCol, double endRow, double endCol, ref System.Collections.Generic.List<ROI> rois)
        {
            this.setROIShape(new ROILine());

            if (rois == null)
            {
                rois = new System.Collections.Generic.List<ROI>();
            }

            if (roiMode != null)			 //either a new ROI object is created
            {
                roiMode.createLine(beginRow, beginCol, endRow, endCol);

                rois.Add(roiMode);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx.Clear();
                activeROIidx.Add(ROIList.Count - 1);
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);//֪ͨ���»���
                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
        }

        /// <summary>
        /// ��ָ��λ������ROI--Ellipse
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="ra"></param>
        /// <param name="rb"></param>
        /// <param name="rois"></param>
        public void genEllipse(double row, double col,  double _phi, double ra, double rb ,ref List<ROI> rois)
        {
            setROIShape(new ROIEllipse());

            if (rois == null)
            {
                rois = new List<ROI>();
            }

            if (roiMode != null)			 //either a new ROI object is created
            {
                roiMode.createEllipse(row, col, _phi, ra, rb);

                rois.Add(roiMode);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx.Clear();
                activeROIidx.Add(ROIList.Count - 1);
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);//֪ͨ���»���
                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
        }

        /// <summary>
        /// ��ָ��λ������ROI--Point
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="rois"></param>
        public void genPoint(double row, double col, ref List<ROI> rois)
        {
            setROIShape(new ROIPoint());

            if (rois == null)
            {
                rois = new List<ROI>();
            }

            if (roiMode != null)			 //either a new ROI object is created
            {
                roiMode.createPoint(row, col);

                rois.Add(roiMode);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx.Clear();
                activeROIidx.Add(ROIList.Count - 1);
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);//֪ͨ���»���
                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
        }

        /// <summary>
        /// ��ָ��λ������ROI--Polygon
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="rois"></param>
        public void genPolygon(double[] rows, double[] cols, ref List<ROI> rois)
        {
            setROIShape(new ROIPolygon());

            if (rois == null)
            {
                rois = new List<ROI>();
            }

            if (roiMode != null)			 //either a new ROI object is created
            {
                roiMode.createPolygon(rows, cols);

                rois.Add(roiMode);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx.Clear();
                activeROIidx.Add(ROIList.Count - 1);
                ROIUpdateNotify_HWndCtrl_Handle?.Invoke(null, null);//֪ͨ���»���
                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
        }
        /// <summary>
        ///  ��ȡ��ǰѡ��ROI����Ϣ
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ROI smallestActiveROI(out List<double> data, out int index)
        {
            try
            {
                int activeROIIdx = this.activeROIidx[0];
                index = activeROIIdx;
                data = new List<double>();

                if (activeROIIdx > -1)
                {
                    ROI region = this.getActiveROI()[0];
                    Type type = region.GetType();
                    HTuple smallest = region.getModelData();              
                    return region;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {
                data = null;
                index = 0;
                return null;
            }
        }



    }//end of class
}//end of namespace
