using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HalconDotNet;


namespace ROIGenerateLib
{

    /// <summary>
    /// This class is a base class containing virtual methods for handling
    /// ROIs. Therefore, an inheriting class needs to define/override these
    /// methods to provide the ROIController with the necessary information on
    /// its (= the ROIs) shape and position. The example project provides 
    /// derived ROI shapes for rectangles, lines, circles, and circular arcs.
    /// To use other shapes you must derive a new class from the base class 
    /// ROI and implement its methods.
    /// </summary>    
    [Serializable]
    public class ROI
	{

		// class members of inheriting ROI classes
		protected int   NumHandles;
		protected int	activeHandleIdx;

		/// <summary>
		/// Flag to define the ROI to be 'positive' or 'negative'.
		/// </summary>
		protected int     OperatorFlag;

		/// <summary>Parameter to define the line style of the ROI.</summary>
		public HTuple     flagLineStyle;

		/// <summary>Constant for a positive ROI flag.</summary>
		public const int  POSITIVE_FLAG	= ROIController.MODE_ROI_POS;

		/// <summary>Constant for a negative ROI flag.</summary>
		public const int  NEGATIVE_FLAG	= ROIController.MODE_ROI_NEG;

		//public const int  ROI_TYPE_LINE       = 10;
		//public const int  ROI_TYPE_CIRCLE     = 11;
		//public const int  ROI_TYPE_CIRCLEARC  = 12;
		//public const int  ROI_TYPE_RECTANCLE1 = 13;
		//public const int  ROI_TYPE_RECTANGLE2 = 14;
        public const double Scale_factor = 0.004;
        //边界安全距离
         const int SafeDistance = 5;
        protected  int ActiveBox = 10;
        //采样频率
        const int sampling_frequency = 2;
        protected int currx_direct = 1;
        protected int curry_direct = 1;
       
        //实例化ROI各自标=标注信息
        public EumROIConstanceSymbolInfo ROIConstanceSymbolInfo { get; set; }

         bool IsMovingLeft =false;
         bool IsMovingTop = false;
        protected double currx = 0;
        protected double curry = 0;

        protected HTuple  posOperation = new HTuple();
		protected HTuple  negOperation = new HTuple(new int[] { 2, 2 });

		/// <summary>Constructor of abstract ROI class.</summary>
		public ROI(int CurrImgWidth,int CurrImageHeight)
        {
            imageWidth = CurrImgWidth;
            imageHeight = CurrImageHeight;
            ActiveBox = (int)(CurrImgWidth * Scale_factor);
        }
        public ROI() { }
        
        private string color = "blue";
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        int imageWidth;int imageHeight;
		/// <summary>Creates a new ROI instance at the mouse position.</summary>
		/// <param name="midX">
		/// x (=column) coordinate for ROI
		/// </param>
		/// <param name="midY">
		/// y (=row) coordinate for ROI
		/// </param>
		public virtual void createROI(double midX, double midY) { }

		/// <summary>Paints the ROI into the supplied window.</summary>
		/// <param name="window">HALCON window</param>
		public virtual void draw(HalconDotNet.HWindow window) { }

		/// <summary> 
		/// Returns the distance of the ROI handle being
		/// closest to the image point(x,y)
		/// </summary>
		/// <param name="x">x (=column) coordinate</param>
		/// <param name="y">y (=row) coordinate</param>
		/// <returns> 
		/// Distance of the closest ROI handle.
		/// </returns>
		public virtual double distToClosestHandle(double x, double y)
		{
			return 0.0;
		}

        //检测区域在图像中移动时是否靠经或超出边界区域
        protected bool CheckRegoionCloseToBorde(double newx, double newy, ref int x_direct, ref int y_direct)
        {

            HTuple Rows, columns, rowtrans, columntrans;
            HRegion currRegion = getRegion();
            HXLDCont borderRegin = currRegion.GenContourRegionXld("border");
            borderRegin.GetContourXld(out Rows, out columns);
            if (currRegion == null) return false;

            IsMovingLeft = newx - currx <= 0;
            IsMovingTop = newy - curry <= 0;
            currx_direct = 1; curry_direct = 1;
            HHomMat2D temMat2d = new HHomMat2D();
            temMat2d.VectorAngleToRigid( curry, currx,0, newy,newx,0);
            temMat2d.AffineTransPixel(Rows, columns,out rowtrans,out columntrans);       
         
          
            HTuple MAXtemrow = rowtrans.TupleMax();
            HTuple MINtemrow = rowtrans.TupleMin();
            HTuple MAXtemcolumn = columntrans.TupleMax();
            HTuple MINtemcolumn = columntrans.TupleMin();

            currRegion.Dispose(); borderRegin.Dispose();
            //x

            if ((MINtemcolumn.D <= 0 + SafeDistance && IsMovingLeft) ||
                (MAXtemcolumn.D >= imageWidth - SafeDistance && !IsMovingLeft))
                x_direct = -1;


            //y

            if ((MINtemrow.D <= 0 + SafeDistance && IsMovingTop) ||
                (MAXtemrow.D >= imageHeight - SafeDistance && !IsMovingTop))
                y_direct = -1;


            if (x_direct == 1)
                currx = newx;
            if ( y_direct == 1)
                curry = newy;

            x_direct = currx_direct; y_direct = curry_direct;
            if (x_direct == 1 && y_direct == 1)
                return true;

            else 
                return false;


        }
        /// <summary> 
        /// Paints the active handle of the ROI object into the supplied window. 
        /// </summary>
        /// <param name="window">HALCON window</param>
        public virtual void displayActive(HalconDotNet.HWindow window) { }

		/// <summary> 
		/// Recalculates the shape of the ROI. Translation is 
		/// performed at the active handle of the ROI object 
		/// for the image coordinate (x,y).
		/// </summary>
		/// <param name="x">x (=column) coordinate</param>
		/// <param name="y">y (=row) coordinate</param>
		public virtual void moveByHandle(double x, double y) { }

		/// <summary>Gets the HALCON region described by the ROI.</summary>
		public virtual HRegion getRegion()
		{
			return null;
		}

		public virtual double getDistanceFromStartPoint(double row, double col)
		{
			return 0.0;
		}
		/// <summary>
		/// Gets the model information described by 
		/// the ROI.
		/// </summary> 
		public virtual HTuple getModelData()
		{
			return null;
		}

		/// <summary>Number of handles defined for the ROI.</summary>
		/// <returns>Number of handles</returns>
		public int getNumHandles()
		{
			return NumHandles;
		}

		/// <summary>Gets the active handle of the ROI.</summary>
		/// <returns>Index of the active handle (from the handle list)</returns>
		public int getActHandleIdx()
		{
			return activeHandleIdx;
		}

		/// <summary>
		/// Gets the sign of the ROI object, being either 
		/// 'positive' or 'negative'. This sign is used when creating a model
		/// region for matching applications from a list of ROIs.
		/// </summary>
		public int getOperatorFlag()
		{
			return OperatorFlag;
		}

		/// <summary>
		/// Sets the sign of a ROI object to be positive or negative. 
		/// The sign is used when creating a model region for matching
		/// applications by summing up all positive and negative ROI models
		/// created so far.
		/// </summary>
		/// <param name="flag">Sign of ROI object</param>
		public void setOperatorFlag(int flag)
		{
			OperatorFlag = flag;

			switch (OperatorFlag)
			{
				case ROI.POSITIVE_FLAG:
					flagLineStyle = posOperation;
					break;
				case ROI.NEGATIVE_FLAG:
					flagLineStyle = negOperation;
					break;
				default:
					flagLineStyle = posOperation;
					break;
			}
		}

        /************************************************************************************/
        //矩形
        public virtual void createRectangle1(double row1, double col1, double row2, double col2) { }
        //旋转矩形
        public virtual void createRectangle2(double row, double col, double phi, double length1, double length2) { }
       //圆
        public virtual void createCircle(double row, double col, double radius) { }
       //圆弧
        public virtual void createCircularArc(double row, double col, double radius,
                                                   double startPhi, double extentPhi, string direct){ }
       //直线
        public virtual void createLine(double beginRow, double beginCol, double endRow, double endCol) { }
       
        //椭圆
        public virtual void createEllipse(double row, double col, double _phi, double ra, double rb) { }

        //点
        public virtual void createPoint(double row, double col) { }
        //多边形
        public virtual void createPolygon(double[] rows, double[] cols) { }


    }//end of class
}//end of namespace
