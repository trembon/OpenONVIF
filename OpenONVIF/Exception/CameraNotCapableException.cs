using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Exception
{
    public class CameraNotCapableException : System.Exception
    {
        public string ServiceType { get; }

        public CameraNotCapableException(string serviceType)
        {
            this.ServiceType = serviceType;
        }
    }
}
