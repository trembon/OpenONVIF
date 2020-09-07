using OpenONVIF.Surveillance.Camera.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.Camera
{
    internal class NewCameraFrameEventArgs: EventArgs
    {
        public Bitmap Frame { get; }

        public TimeOfDay TimeOfDay { get; }

        public NewCameraFrameEventArgs(Bitmap frame, TimeOfDay timeOfDay)
        {
            this.Frame = frame;
            this.TimeOfDay = timeOfDay;
        }
    }
}
