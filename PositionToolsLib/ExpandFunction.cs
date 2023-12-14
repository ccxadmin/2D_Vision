using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;


namespace PositionToolsLib
{
   public class ExpandFunction
    {

      static  public void CalParallelLine(HTuple hv_r1, HTuple hv_c1, HTuple hv_r2, HTuple hv_c2,
     HTuple hv_r21, HTuple hv_c21, HTuple hv_r22, HTuple hv_c22, out HTuple hv_row1,
     out HTuple hv_column1, out HTuple hv_row2, out HTuple hv_column2)
        {

            // Local iconic variables 

            HObject ho_Contour, ho_Line, ho_ParallelContours;
            HObject ho_ParallelContours1;

            // Local control variables 

            HTuple hv_rx = null, hv_cx = null, hv_RowProj = null;
            HTuple hv_ColProj = null, hv_rc = null, hv_cc = null, hv_Distance = null;
            HTuple hv_RowBegin = null, hv_ColBegin = null, hv_RowEnd = null;
            HTuple hv_ColEnd = null, hv_Nr = null, hv_Nc = null, hv_Dist = null;
            HTuple hv_RowBegin2 = null, hv_ColBegin2 = null, hv_RowEnd2 = null;
            HTuple hv_ColEnd2 = null, hv_Nr2 = null, hv_Nc2 = null;
            HTuple hv_Dist2 = null, hv_Row = null, hv_Column = null;
            HTuple hv_IsOverlapping = null, hv_Row3 = null, hv_Column3 = null;
            HTuple hv_IsOverlapping1 = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Line);
            HOperatorSet.GenEmptyObj(out ho_ParallelContours);
            HOperatorSet.GenEmptyObj(out ho_ParallelContours1);
            hv_row1 = new HTuple();
            hv_column1 = new HTuple();
            hv_row2 = new HTuple();
            hv_column2 = new HTuple();

            hv_rx = (hv_r1 + hv_r2) / 2.0;
            hv_cx = (hv_c1 + hv_c2) / 2.0;

            HOperatorSet.ProjectionPl(hv_rx, hv_cx, hv_r21, hv_c21, hv_r22, hv_c22, out hv_RowProj,
                out hv_ColProj);

            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_rx.TupleConcat(hv_RowProj),
                hv_cx.TupleConcat(hv_ColProj));

            hv_rc = (hv_rx + hv_RowProj) / 2.0;
            hv_cc = (hv_cx + hv_ColProj) / 2.0;

            //gen_cross_contour_xld (Cross, rc, cc, 20, 0.785398)

            HOperatorSet.DistancePp(hv_rc, hv_cc, hv_rx, hv_cx, out hv_Distance);
            ho_Line.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Line, hv_r1.TupleConcat(hv_r2), hv_c1.TupleConcat(
                hv_c2));
            ho_ParallelContours.Dispose();
            HOperatorSet.GenParallelContourXld(ho_Line, out ho_ParallelContours, "regression_normal",
                hv_Distance);
            HOperatorSet.FitLineContourXld(ho_ParallelContours, "regression", -1, 0, 5, 2,
                out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr,
                out hv_Nc, out hv_Dist);
            ho_ParallelContours1.Dispose();
            HOperatorSet.GenParallelContourXld(ho_Line, out ho_ParallelContours1, "regression_normal",
                -hv_Distance);
            HOperatorSet.FitLineContourXld(ho_ParallelContours1, "regression", -1, 0, 5,
                2, out hv_RowBegin2, out hv_ColBegin2, out hv_RowEnd2, out hv_ColEnd2, out hv_Nr2,
                out hv_Nc2, out hv_Dist2);

            HOperatorSet.IntersectionContoursXld(ho_Contour, ho_ParallelContours, "mutual",
                out hv_Row, out hv_Column, out hv_IsOverlapping);
            HOperatorSet.IntersectionContoursXld(ho_Contour, ho_ParallelContours1, "mutual",
                out hv_Row3, out hv_Column3, out hv_IsOverlapping1);

            if ((int)(new HTuple((new HTuple(hv_Row.TupleLength())).TupleGreater(0))) != 0)
            {
                hv_row1 = hv_RowBegin.Clone();
                hv_column1 = hv_ColBegin.Clone();
                hv_row2 = hv_RowEnd.Clone();
                hv_column2 = hv_ColEnd.Clone();
            }
            else
            {
                hv_row1 = hv_RowBegin2.Clone();
                hv_column1 = hv_ColBegin2.Clone();
                hv_row2 = hv_RowEnd2.Clone();
                hv_column2 = hv_ColEnd2.Clone();
            }

            ho_Contour.Dispose();
            ho_Line.Dispose();
            ho_ParallelContours.Dispose();
            ho_ParallelContours1.Dispose();

            return;
        }

    }
}
