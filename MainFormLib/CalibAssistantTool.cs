//
// File generated by HDevelop for HALCON/.NET (C#) Version 17.12
//
using System;
using HalconDotNet;

namespace MainFormLib
{
    /// <summary>
    ///�궨���ֹ���
    /// </summary>
    static public  class CalibAssistantTool
    {
        

        // Chapter: Graphics / Text
        // Short Description: Set font independent of OS 
        static void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
            HTuple hv_Bold, HTuple hv_Slant)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = null, hv_Fonts = new HTuple();
            HTuple hv_Style = null, hv_Exception = new HTuple(), hv_AvailableFonts = null;
            HTuple hv_Fdx = null, hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
            HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();

            // Initialize local and output iconic variables 
            //This procedure sets the text font of the current window with
            //the specified attributes.
            //
            //Input parameters:
            //WindowHandle: The graphics window for which the font will be set
            //Size: The font size. If Size=-1, the default of 16 is used.
            //Bold: If set to 'true', a bold font is used
            //Slant: If set to 'true', a slanted font is used
            //
            HOperatorSet.GetSystem("operating_system", out hv_OS);
            // dev_get_preferences(...); only in hdevelop
            // dev_set_preferences(...); only in hdevelop
            if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
            {
                hv_Size_COPY_INP_TMP = 16;
            }
            if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
            {
                //Restore previous behaviour
                hv_Size_COPY_INP_TMP = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt();
            }
            else
            {
                hv_Size_COPY_INP_TMP = hv_Size_COPY_INP_TMP.TupleInt();
            }
            if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
            {
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Courier";
                hv_Fonts[1] = "Courier 10 Pitch";
                hv_Fonts[2] = "Courier New";
                hv_Fonts[3] = "CourierNew";
                hv_Fonts[4] = "Liberation Mono";
            }
            else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
            {
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Consolas";
                hv_Fonts[1] = "Menlo";
                hv_Fonts[2] = "Courier";
                hv_Fonts[3] = "Courier 10 Pitch";
                hv_Fonts[4] = "FreeMono";
                hv_Fonts[5] = "Liberation Mono";
            }
            else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
            {
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Luxi Sans";
                hv_Fonts[1] = "DejaVu Sans";
                hv_Fonts[2] = "FreeSans";
                hv_Fonts[3] = "Arial";
                hv_Fonts[4] = "Liberation Sans";
            }
            else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
            {
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Times New Roman";
                hv_Fonts[1] = "Luxi Serif";
                hv_Fonts[2] = "DejaVu Serif";
                hv_Fonts[3] = "FreeSerif";
                hv_Fonts[4] = "Utopia";
                hv_Fonts[5] = "Liberation Serif";
            }
            else
            {
                hv_Fonts = hv_Font_COPY_INP_TMP.Clone();
            }
            hv_Style = "";
            if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
            {
                hv_Style = hv_Style + "Bold";
            }
            else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Bold";
                throw new HalconException(hv_Exception);
            }
            if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
            {
                hv_Style = hv_Style + "Italic";
            }
            else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Slant";
                throw new HalconException(hv_Exception);
            }
            if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
            {
                hv_Style = "Normal";
            }
            HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
            hv_Font_COPY_INP_TMP = "";
            for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
            {
                hv_Indices = hv_AvailableFonts.TupleFind(hv_Fonts.TupleSelect(hv_Fdx));
                if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(hv_Fdx);
                        break;
                    }
                }
            }
            if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                throw new HalconException("Wrong value of control parameter Font");
            }
            hv_Font_COPY_INP_TMP = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
            HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);
            // dev_set_preferences(...); only in hdevelop

            return;
        }


        /// <summary>
        ///  �궨ǰ׼����������궨���
        /// </summary>
        /// <param name="StartCamPar"></param>
        /// <param name="calibObjDescr">�궨�������ļ�</param>
        /// <returns></returns>
        static public HTuple ReadyCalibrate(HTuple StartCamPar, HTuple calibObjDescr)
        {
            HOperatorSet.CreateCalibData("calibration_object", 1, 1, out HTuple hv_CalibDataID);
            HOperatorSet.SetCalibDataCamParam(hv_CalibDataID, 0, "area_scan_division",
                StartCamPar);
            HOperatorSet.SetCalibDataCalibObject(hv_CalibDataID, 0, calibObjDescr);
            return hv_CalibDataID;
        }
        /// <summary>
        /// һ�α궨����markʶ����
        /// </summary>
        /// <param name="ho_Image">����ͼ��</param>
        /// <param name="CalibDataID"></param>
        /// <param name="calibObjPoseIdx"></param>
        /// <returns></returns>
        static public HObject OnceCalibrate(HObject ho_Image, HTuple CalibDataID, HTuple calibObjPoseIdx)
        {
            HTuple hv_Row, hv_Column, hv_Index, hv_Pose;
            HObject ho_Caltab, ho_Cross;
            HObject concatObj;
            HOperatorSet.GenEmptyObj(out ho_Caltab);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out concatObj);
            try
            {
                HOperatorSet.FindCalibObject(ho_Image, CalibDataID, 0, 0, calibObjPoseIdx, new HTuple(),
                 new HTuple());
                ho_Caltab.Dispose();
                HOperatorSet.GetCalibDataObservContours(out ho_Caltab, CalibDataID, "caltab",
                    0, 0, calibObjPoseIdx);
                HOperatorSet.GetCalibDataObservPoints(CalibDataID, 0, 0, calibObjPoseIdx, out hv_Row,
                    out hv_Column, out hv_Index, out hv_Pose);
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                concatObj.Dispose();
                HOperatorSet.ConcatObj(ho_Caltab, ho_Cross, out concatObj);
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
            ho_Caltab.Dispose();
            ho_Cross.Dispose();
            return concatObj;
        }
        /// <summary>
        /// ����궨��ȡ����ڲ�
        /// </summary>
        /// <param name="CalibDataID"></param>
        /// <returns></returns>
        static public HTuple CamCalibrate(HTuple CalibDataID, out HTuple hv_Error)
        {
            HTuple hv_CamParam;
            HOperatorSet.CalibrateCameras(CalibDataID, out hv_Error);

            ////��ѯ��У׼����ģ���д洢���������� ��ȡ����ڲ���
            HOperatorSet.GetCalibData(CalibDataID, "camera", 0, "params", out hv_CamParam);

            return hv_CamParam;
        }
        /// <summary>
        /// ����ڲ��ļ�����
        /// </summary>
        /// <param name="hv_CamParam"></param>
        /// <param name="saveDirect"></param>
        static public void SaveCalibData(HTuple hv_CamParam,string saveDirect)
        {
            //Write the internal camera parameters to a file
            HOperatorSet.WriteCamPar(hv_CamParam, saveDirect+ "\\����ڲ�.dat");
        }
        /// <summary>
        /// ��ȡ����ڲ�
        /// </summary>
        /// <param name="readDirect"></param>
        /// <returns></returns>
        static public HTuple ReadCalibData(string readDirect)
        {
            try
            {
                HOperatorSet.ReadCamPar(readDirect + "\\����ڲ�.dat", out HTuple hv_CamParam);
                return hv_CamParam;
            }
            catch
            {
                return null;
            }
          
        }
        /// <summary>
        /// ͼ��У��
        /// </summary>
        /// <param name="ho_Image">�����ԭʼͼ��</param>
        /// <param name="hv_CamParam">����ڲ�</param>
        /// <returns></returns>
        static public HObject ImageCorret(HObject ho_Image,  HTuple hv_CamParam)
        {
            HObject ho_Map, ho_ImageMapped;
            HOperatorSet.GenEmptyObj(out ho_Map);
            HOperatorSet.GenEmptyObj(out ho_ImageMapped);
            HTuple hv_CamParamOut;
            try
            {  
                //Change the radial distortion: mode 'adaptive'
                HOperatorSet.ChangeRadialDistortionCamPar("adaptive", hv_CamParam, 0, out hv_CamParamOut);
                //get_domain (Image, Domain)
                //* change_radial_distortion_image (Image, Domain, ImageRectifiedAdaptive, CamParam, CamParamOut)
                //* dev_display (ImageRectifiedAdaptive)
                ////ͼ��У��2
                ho_Map.Dispose();
                HOperatorSet.GenRadialDistortionMap(out ho_Map, hv_CamParam, hv_CamParamOut,
                    "bilinear");
                ho_ImageMapped.Dispose();
                HOperatorSet.MapImage(ho_Image, ho_Map, out ho_ImageMapped);

            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }

            ho_Map.Dispose();
            return ho_ImageMapped;
        }

        /// <summary>
        /// ���ɱ궨���ļ�
        /// </summary>
        /// <param name="stuCalTabParam"></param>
        static public bool gen_caltab_file(CalTab stuCalTabParam)
        {
            try
            {

                //*gen_caltab(7, 7, 0.0125, 0.5, 'caltab.descr', 'caltab.ps')
                HOperatorSet.GenCaltab(stuCalTabParam.XNum,
                        stuCalTabParam.YNum,
                        stuCalTabParam.MarkDist / 1000f,
                        stuCalTabParam.DiameterRatio,
                        stuCalTabParam.CalPlateDescr,
                        stuCalTabParam.CalPlatePSFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public HTuple startCamPar { get; set; }// ����ڲ�����
        // StartCamPar := [0.016,0,0.0000074,0.0000074,326,247,652,494]

        /// <summary>
        /// ��ȡ����ڲ�
        /// </summary>
        /// <param name="stuCamParams"></param>
        /// <returns></returns>
        static public HTuple getStartCamPar(CamParams stuCamParams)
        {
            HTuple startCamPar = new HTuple();
            startCamPar[0] = stuCamParams.F / 1000f;
            startCamPar[1] = 0;
            startCamPar[2] = stuCamParams.Sx / 1000000f;
            startCamPar[3] = stuCamParams.Sy / 1000000f;
            //startCamPar[0] = 0.016;
            //startCamPar[1] = 0;
            //startCamPar[2] = 0.0000074;
            //startCamPar[3] = 0.0000074;

            startCamPar[4] = stuCamParams.Cx;
            startCamPar[5] = stuCamParams.Cy;
            startCamPar[6] = stuCamParams.Width;
            startCamPar[7] = stuCamParams.Height;
            return startCamPar;
        }


    }

 
    [Serializable]
    /// <summary>
    /// �궨�����
    /// </summary>
    public class CalTab
    {
        public CalTab()
        {
            XNum = 7;
            YNum = 7;
            MarkDist = 30;
            DiameterRatio = 0.5;
            CalPlateDescr = "caltab.ps";
            CalPlatePSFile = "caltab.descr";
        }
        public CalTab(int xNum, int yNum, double markDist,
            double diameterRatio, string calPlateDescr, string calPlatePSFile)
        {
            XNum = xNum;
            YNum = yNum;
            MarkDist = markDist;
            DiameterRatio = diameterRatio;
            CalPlateDescr = calPlateDescr;
            CalPlatePSFile = calPlatePSFile;
        }

        public int XNum;  //X����ԭ�������
        public int YNum;  //Y����ԭ�������
        public double MarkDist;//Բ�����ľ�
        public double DiameterRatio; //Բ��ֱ����Բ�����ľ�ı�ֵ
        public string CalPlateDescr;//�궨�������ļ����ļ�·����.descr��
        public string CalPlatePSFile;//�궨��ͼ���ļ����ļ�·����.ps��

    }

    [Serializable]
    /// <summary>
    /// �������
    /// </summary>
    public class CamParams
    {
        public CamParams()
        {
            F = 12;
            Sx = 1.85;
            Sy = 1.85;
            Cx = 2012;
            Cy = 1518;
            Width = 4024;
            Height = 3036;
        }

        public CamParams(int f, double sx, double sy, int cx,
              int cy, int width, int height)
        {
            F = f;
            Sx = sx;
            Sy = sy;
            Cx = cx;
            Cy = cy;
            Width = width;
            Height = height;
        }
        public int F;  //����f,Զ�ľ�ͷKΪ0
       //  public HTuple K;  //����k
        public double Sx; //��Ԫ�ߴ��
        public double Sy; //��Ԫ�ߴ��
        public int Cx; //ͼ�����ĵ������
        public int Cy; //ͼ�����ĵ������
        public int Width;  //ͼ��ߴ��
        public int Height; //ͼ��ߴ��

    }

}


