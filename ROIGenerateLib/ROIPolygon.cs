using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;

namespace ROIGenerateLib
{
    [Serializable]
    public class ROIPolygon:ROI
    {
       
        private double midR, midC;  // 中心点

        //点集合
        HTuple RowArray =new HTuple();
        HTuple ColumnArray =new HTuple();
     
        private HTuple hom2D;
        int defaultPointcount;
        private int currTitleIndex = 0;
        protected static int titleIndex = 0;
        public ROIPolygon(int imagewidth, int imageheight,int Pointcount = 8):base(imagewidth, imageheight)
        {
            currTitleIndex= ++titleIndex;
            NumHandles = Pointcount + 1;
            activeHandleIdx =0;
            defaultPointcount = Pointcount;
            ROIConstanceSymbolInfo = EumROIConstanceSymbolInfo.Polygon;
        }
        public ROIPolygon()
        {
            activeHandleIdx = 0;
            currTitleIndex = ++titleIndex;
            defaultPointcount = 8;
            NumHandles = 9;
            ROIConstanceSymbolInfo = EumROIConstanceSymbolInfo.Polygon;
        }
        public override string ToString()
        {
            return string.Format("ROIPolygon_{0}", currTitleIndex);

        }
        public override void createPolygon(double[] rows, double[] cols)
        {
            int count = rows.Length;
            for (int i = 0; i < count; i++)
            {
                RowArray.Append(rows[i]);
                ColumnArray.Append(cols[i]);  //中心点
            }

            HOperatorSet.GenRegionPolygon(out HObject region, RowArray.TupleConcat(RowArray[0]),
                ColumnArray.TupleConcat(ColumnArray[0]));
            HOperatorSet.AreaCenter(region, out HTuple area, out HTuple row, out HTuple column);
            //中心点
            midR = row.D;
            midC = column.D;
            //首端插入中心点
            RowArray= RowArray.TupleInsert(0, row.D);
            ColumnArray= ColumnArray.TupleInsert(0, column.D);

            defaultPointcount = count;
            NumHandles = count+1;

        }
        /// <summary>Creates a new ROI instance at the mouse position</summary>
        public override void createROI(double midX, double midY)
        {
            midR = midY;
            midC = midX;
            int initspace = ActiveBox*50;
            int halfpoint = defaultPointcount / 2;
            int quarterpoint = defaultPointcount / 4;
            double eachGap = (initspace / 4)/(quarterpoint+1);
            RowArray.Append(midR); ColumnArray.Append(midC);  //中心点
            //第一象限
            for (int i = 1; i<= quarterpoint; i++)
            {
                int j = quarterpoint + 1 - i;
                RowArray.Append(eachGap * (-i) + midR);
                ColumnArray.Append(eachGap * j + midC);
            }
            //第二象限
            for (int i = quarterpoint; i >= 1; i--)
            {
                int j = quarterpoint + 1 - i;
                RowArray.Append(eachGap * (-i) + midR);
                ColumnArray.Append(eachGap * (-j) + midC);
            }
            //第三象限
            for (int i = 1; i <= quarterpoint; i++)
            {
                int j = quarterpoint + 1 - i;
                RowArray.Append(eachGap * i + midR);
                ColumnArray.Append(eachGap * (-j )+ midC);
            }
            //第四象限
            for (int i = quarterpoint; i >= 1; i--)
            {
                int j = quarterpoint + 1 - i;
                RowArray.Append(eachGap * i + midR);
                ColumnArray.Append(eachGap * j + midC);
            }
           
        }

        /// <summary>Paints the ROI into the supplied window</summary>
        /// <param name="window">HALCON window</param>
        public override void draw(HalconDotNet.HWindow window)
        {
            int count = RowArray.TupleLength();

            HTuple temrow ;
            HTuple temcolumn ;
            temrow= RowArray.TupleSelectRange(1, count - 1).TupleConcat(RowArray[1]);
            temcolumn= ColumnArray.TupleSelectRange(1, count - 1).TupleConcat(ColumnArray[1]);
           
            window.DispPolygon(temrow, temcolumn);

          for(int i=0;i< RowArray.TupleLength(); i++)
            window.DispRectangle2(RowArray.TupleSelect(i),
                ColumnArray.TupleSelect(i),
                     0, ActiveBox, ActiveBox);
           
        }

        /// <summary> 
        /// Returns the distance of the ROI handle being
        /// closest to the image point(x,y)
        /// </summary>
        public override double distToClosestHandle(double x, double y)
        {
            NumHandles = RowArray.TupleLength();
            double max = 10000;
            double[] val = new double[NumHandles];           
           
            for (int i = 0; i < RowArray.TupleLength(); i++)  //包括中心点
            {
                val[i] = HMisc.DistancePp(y, x, RowArray[i].D,
                             ColumnArray[i].D); // border handle 
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
            window.DispRectangle2(RowArray[activeHandleIdx].D,
                               ColumnArray[activeHandleIdx].D,
                              0, ActiveBox, ActiveBox);
        }

        /// <summary>Gets the HALCON region described by the ROI</summary>
        public override HRegion getRegion()
        {
            HRegion region = new HRegion();
            HXLDCont polycont = new HXLDCont();
            region.Dispose();
            polycont.Dispose();
            int count = RowArray.TupleLength();
            
            HTuple temrow;
            HTuple temcolumn;
            temrow = RowArray.TupleSelectRange(1, count - 1).TupleConcat(RowArray[1]);
            temcolumn = ColumnArray.TupleSelectRange(1, count - 1).TupleConcat(ColumnArray[1]);
            polycont.GenContourPolygonXld(temrow, temcolumn);          
            region=polycont.GenRegionContourXld("filled");
            polycont.Dispose();

            return region;
        }



        /// <summary>
        /// Gets the model information described by 
        /// the  ROI
        /// </summary> 
        public override HTuple getModelData()
        {
            return new HTuple(RowArray, ColumnArray);
        }

        /// <summary> 
        /// Recalculates the shape of the ROI. Translation is 
        /// performed at the active handle of the ROI object 
        /// for the image coordinate (x,y)
        /// </summary>
        public override void moveByHandle(double newX, double newY)
        {
                      
            if (activeHandleIdx == 0)
            {
             
                HTuple temrow = new HTuple();
                HTuple temcolumn = new HTuple();
                HOperatorSet.VectorAngleToRigid(new HTuple(midR), new HTuple(midC), 0,
                       new HTuple(newY), new HTuple(newX), 0, out hom2D);           
                HOperatorSet.AffineTransPixel(hom2D, RowArray, ColumnArray
                    , out temrow,out temcolumn);
                                            
                RowArray=temrow;           
                ColumnArray=temcolumn;
                midR = temrow[0];
                midC = temcolumn[0];
            }
            else
            {
                RowArray[activeHandleIdx] = newY;
                ColumnArray[activeHandleIdx] = newX;
            }
        
        }


        public void AddNewPoint()
        {
            List<HTuple> temputleList = new List<HTuple>();
            //选中中心点时不做操作
            if (activeHandleIdx == 0 || activeHandleIdx == -1)
                return;
            for (int i = 0; i < RowArray.TupleLength(); i++)
            {
                temputleList.Add(RowArray.TupleSelect(i));
            }
            temputleList.Insert(activeHandleIdx, new HTuple(RowArray[activeHandleIdx].D - 50));
            RowArray = new HTuple();
            foreach (var ss in temputleList)
                RowArray.Append(ss);

            temputleList.Clear();

            for (int i = 0; i < ColumnArray.TupleLength(); i++)
            {
                temputleList.Add(ColumnArray.TupleSelect(i));
            }
            temputleList.Insert(activeHandleIdx, new HTuple(ColumnArray[activeHandleIdx].D + 50));
            ColumnArray = new HTuple();
            foreach (var ss in temputleList)
                ColumnArray.Append(ss);
          
        }
        public void DelectSelectPoint()
        {
            List<HTuple> temputleList = new List<HTuple>();
            //选中中心点时不做操作
            if (activeHandleIdx == 0|| activeHandleIdx==-1)
                return;

            for (int i = 0; i < RowArray.TupleLength(); i++)
            {
                temputleList.Add(RowArray.TupleSelect(i));
            }
            temputleList.RemoveAt(activeHandleIdx);
            RowArray = new HTuple();
            foreach (var ss in temputleList)
                RowArray.Append(ss);

            temputleList.Clear();

            for (int i = 0; i < ColumnArray.TupleLength(); i++)
            {
                temputleList.Add(ColumnArray.TupleSelect(i));
            }
            temputleList.RemoveAt(activeHandleIdx);
            ColumnArray = new HTuple();
            foreach (var ss in temputleList)
                ColumnArray.Append(ss);  
            activeHandleIdx--;

        }
    }
}
