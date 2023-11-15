using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;

namespace ROIGenerateLib
{
    [Serializable]
    public class ROIEllipse:ROI
    {
     
        private double radius1, radius2,phi,LastOnepih;
        private double row1, col1;  // first handle
        private double row2, col2;  // second handle
        private double row3, col3;  // third handle
        private double midR, midC;  // four handle

        protected static int titleIndex = 0;
        private int currTitleIndex = 0;
        private HTuple hom2D ;
        private HXLDCont arrowHandleXLD;
        public ROIEllipse(int imagewidth, int imageheight) : base(imagewidth, imageheight)
        {
            currTitleIndex = ++titleIndex;
            NumHandles = 4; // two at corner of ellipse + midpoint
            activeHandleIdx = 5;
            arrowHandleXLD = new HXLDCont();
            arrowHandleXLD.GenEmptyObj();
            ROIConstanceSymbolInfo = EumROIConstanceSymbolInfo.Ellipse;
        }


        public ROIEllipse()
        {
            currTitleIndex = ++titleIndex;
            NumHandles = 4; // two at corner of ellipse + midpoint
            activeHandleIdx = 5;
            arrowHandleXLD = new HXLDCont();
            arrowHandleXLD.GenEmptyObj();
            ROIConstanceSymbolInfo = EumROIConstanceSymbolInfo.Ellipse;
        }

        public override string ToString()
        {
            return string.Format("ROIEllipse_{0}", currTitleIndex);

        }

        public override void createEllipse(double row, double col,double _phi , double ra, double rb)
        {
            base.createEllipse(row, col, _phi, ra, rb);
            midR = row;
            midC = col;

            radius1 = ra;   //ra
            radius2 = rb;  //rb
            LastOnepih = 0;
            phi = _phi;
            //ra方向操作
            row1 = midR;
            col1 = midC + radius1;
            // ra拉伸操作
            row2 = midR;
            col2 = midC - radius1;
            // rb拉伸操作
            row3 = midR - radius2;
            col3 = midC;

            updateHandlePos();
        }

        /// <summary>Creates a new ROI instance at the mouse position</summary>
        public override void createROI(double midX, double midY)
        {
         

           midR = midY;
            midC = midX;

            radius1 = 10* ActiveBox;   //ra
            radius2 = 10/2* ActiveBox;  //rb

            LastOnepih=phi = 0.0;

                    
            //ra方向操作
              row1 = midR;
            col1 = midC + radius1;
           // ra拉伸操作
              row2 = midR;
            col2 = midC - radius1;
           // rb拉伸操作
              row3 = midR - radius2;
            col3 = midC;
        }

        /// <summary>Paints the ROI into the supplied window</summary>
        /// <param name="window">HALCON window</param>
        public override void draw(HalconDotNet.HWindow window)
        {
            window.DispEllipse(midR, midC, phi, radius1, radius2);

            window.DispArrow(midR, midC, midR + (Math.Sin(-phi) * radius1 * 1),
            midC + (Math.Cos(-phi) * radius1 * 1), ActiveBox/2);
            window.DispRectangle2(row1, col1, 0, ActiveBox, ActiveBox);
            window.DispRectangle2(row2, col2, 0, ActiveBox, ActiveBox);
            window.DispRectangle2(row3, col3, 0, ActiveBox, ActiveBox);
            window.DispRectangle2(midR, midC, 0, ActiveBox, ActiveBox);
        }

        /// <summary> 
        /// Returns the distance of the ROI handle being
        /// closest to the image point(x,y)
        /// </summary>
        public override double distToClosestHandle(double x, double y)
        {
            double max = 10000;
            double[] val = new double[NumHandles];
            val[0] = HMisc.DistancePp(y, x, row1, col1); // border handle 
            val[1] = HMisc.DistancePp(y, x, row2, col2); // border2 handle 
            val[2] = HMisc.DistancePp(y, x, row3, col3); // border2 handle 
            val[3] = HMisc.DistancePp(y, x, midR, midC); // midpoint 
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
                    window.DispArrow(midR, midC, midR + (Math.Sin(-phi) * radius1 * 1),
             midC + (Math.Cos(-phi) * radius1 * 1), ActiveBox/2);
                    window.DispRectangle2(row1, col1, 0, ActiveBox, ActiveBox);
                    break;
                case 1:
                    window.DispRectangle2(row2, col2, 0, ActiveBox, ActiveBox);
                    break;
                case 2:
                    window.DispRectangle2(row3, col3, 0, ActiveBox, ActiveBox);
                    break;
                case 3:
                    window.DispRectangle2(midR, midC, 0, ActiveBox, ActiveBox);
                    break;
            }
        }

        /// <summary>Gets the HALCON region described by the ROI</summary>
        public override HRegion getRegion()
        {
            HRegion region = new HRegion();
            region.GenEllipse(midR, midC, phi,radius1,radius2);
            return region;
        }

        

        /// <summary>
        /// Gets the model information described by 
        /// the  ROI
        /// </summary> 
        public override HTuple getModelData()
        {
            return new HTuple(new double[] { midR, midC,phi, radius1 ,radius2});
        }

        /// <summary> 
        /// Recalculates the shape of the ROI. Translation is 
        /// performed at the active handle of the ROI object 
        /// for the image coordinate (x,y)
        /// </summary>
        public override void moveByHandle(double newX, double newY)
        {
            HTuple distance;
            double shiftX, shiftY;
            double vX, vY, x = 0, y = 0;
            switch (activeHandleIdx)
            {
                case 0:
                  
                    vY = midR-newY;
                    vX = newX - midC;
                   
                    phi = Math.Atan2(vY, vX);
                  
                    updateHandlePos();
                    break;
                case 1: // handle at circle border

                    row2 = newY;
                    col2 = newX;
                  col1 = newX + radius1 * 2;
                    HOperatorSet.DistancePp(new HTuple(row2), new HTuple(col2),
                                            new HTuple(midR), new HTuple(midC),
                                            out distance);

                    radius1 = distance[0].D;
                    break;
                case 2: // midpoint 
                    row3 = newY;
                    col3 = newX;
                    HOperatorSet.DistancePp(new HTuple(row3), new HTuple(col3),
                                            new HTuple(midR), new HTuple(midC),
                                            out distance);

                    radius2 = distance[0].D;
                    break;
                case 3: // midpoint 

                    shiftY = midR - newY;
                    shiftX = midC - newX;

                    midR = newY;
                    midC = newX;

                    row1 -= shiftY;
                    col1 -= shiftX;
                    row2 -= shiftY;
                    col2 -= shiftX;
                    row3 -= shiftY;
                    col3 -= shiftX;
                   
                    break;
            }
        }
        private void updateHandlePos()
        {
           
            HOperatorSet.VectorAngleToRigid(new HTuple(midR),new HTuple(midC), LastOnepih,
               new HTuple(midR), new HTuple(midC),phi, out hom2D);
         
            HTuple rowtrans, columntrans; 
            HOperatorSet.AffineTransPixel(hom2D, row1, col1, out rowtrans, out columntrans);         
            row1 = rowtrans; col1 = columntrans;
            HOperatorSet.AffineTransPixel(hom2D, row2, col2, out rowtrans, out columntrans);
            row2 = rowtrans; col2 = columntrans;
            HOperatorSet.AffineTransPixel(hom2D, row3, col3, out rowtrans, out columntrans);
            row3 = rowtrans; col3 = columntrans;
        
            LastOnepih =phi;
        }

    }//end of class
}
