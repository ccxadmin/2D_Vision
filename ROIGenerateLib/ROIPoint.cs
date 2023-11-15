using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;

namespace ROIGenerateLib
{
    [Serializable]
    public  class ROIPoint : ROI
    {
      
       
        private double midR, midC;
        private int currTitleIndex = 0;
        protected static int titleIndex = 0;
        public ROIPoint(int imagewidth, int imageheight) : base(imagewidth, imageheight)
        {
            currTitleIndex = ++titleIndex;
            NumHandles = 1; 
            activeHandleIdx = 7;
            ROIConstanceSymbolInfo = EumROIConstanceSymbolInfo.Point;
        }

        public ROIPoint()
        {
            currTitleIndex = ++titleIndex;
            NumHandles = 1;
            activeHandleIdx = 7;
            ROIConstanceSymbolInfo = EumROIConstanceSymbolInfo.Point;
        }
        public override string ToString()
        {
            return string.Format("ROIPoint_{0}", currTitleIndex);

        }

        public override void createPoint(double row, double col)
        {
            midR = row;
            midC = col;
        }

        /// <summary>Creates a new ROI instance at the mouse position</summary>
        public override void createROI(double midX, double midY)
        {
            midR = midY;
            midC = midX;
           
        }

        /// <summary>Paints the ROI into the supplied window</summary>
        /// <param name="window">HALCON window</param>
        public override void draw(HalconDotNet.HWindow window)
        {
            window.DispPolygon(midR, midC);
            window.DispRectangle2(midR, midC, 0, ActiveBox, ActiveBox);
            window.DispCross(midR, midC, ActiveBox*5, 0);
        }

        /// <summary> 
        /// Returns the distance of the ROI handle being
        /// closest to the image point(x,y)
        /// </summary>
        public override double distToClosestHandle(double x, double y)
        {
            double max = 10000;
            double[] val = new double[NumHandles];
            val[0] = HMisc.DistancePp(y, x, midR, midC); // midpoint
          
            for (int i = 0; i < NumHandles; i++)
            {
                if (val[i] < max)
                {
                    max = val[i];
                    activeHandleIdx = i;
                }
            }// end of for 
            return val[activeHandleIdx];
        }

        /// <summary> 
        /// Paints the active handle of the ROI object into the supplied window 
        /// </summary>
        public override void displayActive(HalconDotNet.HWindow window)
        {
            switch (activeHandleIdx)
            {
                case 0:               
                    window.DispRectangle2(midR, midC, 0, ActiveBox, ActiveBox);
                    window.DispCross(midR, midC, ActiveBox*5, 0);
                    break;
             
            }
        }

        /// <summary>Gets the HALCON region described by the ROI</summary>
        public override HRegion getRegion()
        {
            HRegion region = new HRegion();
            region.GenRegionPoints(midR, midC);
          
            return region;
        }

       

        /// <summary>
        /// Gets the model information described by 
        /// the  ROI
        /// </summary> 
        public override HTuple getModelData()
        {
            return new HTuple(new double[] { midR, midC });
        }

        /// <summary> 
        /// Recalculates the shape of the ROI. Translation is 
        /// performed at the active handle of the ROI object 
        /// for the image coordinate (x,y)
        /// </summary>
        public override void moveByHandle(double newX, double newY)
        {
          

            switch (activeHandleIdx)
            {
               
                case 0: // midpoint 
              
                    midR = newY;
                    midC = newX;                 
                    break;
            }
        }
    }
}
