using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
namespace FunctionLib.Cam
{
    public interface Icam : IDisposable
    {

        long  ImageWidth{get;}
        long ImageHeight { get; }

        event ImgGetHandle setImgGetHandle;
        event EventHandler CamConnectHnadle;
        bool OpenCam(int camIndex, ref string msg);

        void CloseCam();

        bool OneShot();

        bool ContinueGrab();

        void StopGrab();

        int CamNum { get; }

        bool IsAlive { get;  }
        bool IsGrabing { get; }
        bool IsContinueGrab { get; }

        bool SetExposureTime(long dValue);
        bool SetGain(long dValue);
        bool GetExposureTime(out long dValue);
        bool GetExposureRangeValue(out long min,out long max);
        bool GetGainRangeValue(out long min, out long max);
        bool GetGain(out long dValue);
        CamType currCamType { get; }
        int CamIndex { get; }
    }

    public delegate void ImgGetHandle(HObject img);

    public enum CamType
    {
        海康,
        大华,
        巴斯勒,
        奥普特,
        康耐视,
        度申,
        凌云,
        NONE
    }

}
