using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace DalsaCamera
{
    public interface IDalsaCam
    {
        string DeviceName { get; set; }
        string ConfigFileName { get; set; }
        bool CamIsOK { get; set; }
        HObject Imgae { get; set; }
        HObject ObjDisp { get; set; }
        double Exposure { get; set; }
        double Gain { get; set; }

        event Action<HObject> OnProcImage;
        event Action<HObject> OnShowImage;

        bool InitCamera(string configpath);
        void SetExposure(double value);

        void SetGain(double value);

        bool Snap();

        bool Grab();

        bool Freeze();

        void FreeDalsaCam();


    }
}
