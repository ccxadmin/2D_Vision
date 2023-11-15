using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;

namespace ROIGenerateLib
{
    [Serializable]
    public class ROICombine:ROI
    {
        HTuple regionPointsRows, regionPointsColumns;
        HRegion currRegion = null;
        private double midR, midC;  //center point
        HHomMat2D MatH2D;
    
        public ROICombine(HRegion inputRegion, int imagewidth=0, int imageheight=0):base(imagewidth, imageheight)
        {
            NumHandles = 1; // center point
            activeHandleIdx = 7;
            currRegion= inputRegion.CopyObj(1,1);
            HXLDCont borderRegin= currRegion.GenContourRegionXld("border");
            borderRegin.GetContourXld(out regionPointsRows, out regionPointsColumns);
            //currRegion.GetRegionPoints(out regionPointsRows,out regionPointsColumns);
            inputRegion.AreaCenter(out midR,out midC);
            MatH2D = new HHomMat2D();
            ROIConstanceSymbolInfo = EumROIConstanceSymbolInfo.Combine;
        }

        /// <summary>Creates a new ROI instance at the mouse position</summary>
        public override void createROI(double midX, double midY)
        {
            HTuple newregionPointsRows, newregionPointsColumns;
            HRegion temregion = new HRegion();         
            temregion.Dispose();
          
            MatH2D.HomMat2dIdentity();
            MatH2D.VectorAngleToRigid(midR, midC, 0,
                midY, midX, 0);

            MatH2D.AffineTransPixel(regionPointsRows, regionPointsColumns, out newregionPointsRows, out newregionPointsColumns);
            regionPointsRows = newregionPointsRows;
            regionPointsColumns = newregionPointsColumns;
          
            // temregion = currRegion.AffineTransRegion(MatH2D, "constant");

            //currRegion =temregion.CopyObj(1, 1);
            //temregion.Dispose();

            curry = midR = midY;
            currx = midC = midX;

        }

        /// <summary>Paints the ROI into the supplied window</summary>
        /// <param name="window">HALCON window</param>
        public override void draw(HalconDotNet.HWindow window)
        {
                            
            window.DispRegion(currRegion);          
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
                    break;
               
            }
        }

        /// <summary>Gets the HALCON region described by the ROI</summary>
        public override HRegion getRegion()
        {
            HXLDCont borderRegin = new HXLDCont();
            borderRegin.Dispose();
            borderRegin.GenContourPolygonXld(regionPointsRows, regionPointsColumns);
            currRegion = borderRegin.GenRegionContourXld("filled");
            borderRegin.Dispose();
            //currRegion.GenRegionPoints(regionPointsRows, regionPointsColumns);
            return currRegion;
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

                    HTuple newregionPointsRows, newregionPointsColumns;
               
                    MatH2D.HomMat2dIdentity();
                    MatH2D.VectorAngleToRigid(midR, midC, 0,
                        newY, newX, 0);

                    MatH2D.AffineTransPixel(regionPointsRows, regionPointsColumns, out newregionPointsRows, out newregionPointsColumns);
                                
                    bool boordeFlag = CheckRegoionCloseToBorde(newX, newY, ref currx_direct, ref curry_direct);
                    if (boordeFlag)
                    {
                        //HRegion temregion = new HRegion();
                        //temregion.Dispose();
                        MatH2D.HomMat2dIdentity();
                        MatH2D.VectorAngleToRigid(midR, midC, 0,
                            newY, newX, 0);

                        MatH2D.AffineTransPixel(regionPointsRows, regionPointsColumns, out newregionPointsRows, out newregionPointsColumns);
                        regionPointsRows = newregionPointsRows;
                        regionPointsColumns = newregionPointsColumns;
                        //currRegion.GenRegionPoints(regionPointsRows, regionPointsColumns);
                        //temregion = currRegion.AffineTransRegion(MatH2D, "constant");
                        ////nearest_neighbor 
                        //currRegion = temregion.CopyObj(1, 1);
                        //temregion.Dispose();


                        midR = newY;
                        midC = newX;
                        //regionPointsRows = newregionPointsRows;
                        //regionPointsColumns = newregionPointsColumns;
                    }
                    else
                    {
                        //horizontal
                        if (currx_direct == -1 && curry_direct != -1)
                        {
                            midR = newY;
                            regionPointsRows = newregionPointsRows;

                        }

                        //Vertical
                        else if (curry_direct == -1 && currx_direct != -1)
                        {
                            midC = newX;
                            regionPointsColumns = newregionPointsColumns;
                        }

                        else if (currx_direct == -1 && curry_direct == -1)
                        {

                        }
                    }
                    HXLDCont borderRegin = new HXLDCont();
                    borderRegin.Dispose();
                    borderRegin.GenContourPolygonXld(regionPointsRows, regionPointsColumns);
                    currRegion= borderRegin.GenRegionContourXld("filled");
                    borderRegin.Dispose();
                    // currRegion.GenRegionPoints(regionPointsRows, regionPointsColumns);
                    break;
                   
             
            }
        }

    }
}
