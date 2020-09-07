using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.Webserver
{
    internal class CameraStatus
    {
        public string Name { get; }

        public Bitmap LatestImage { get; set; }

        public DateTime ImageTimestamp { get; set; }

        public object ReadWriteLock { get;}

        public CameraStatus(string name)
        {
            this.Name = name;
            this.ReadWriteLock = new object();
        }
    }
}
