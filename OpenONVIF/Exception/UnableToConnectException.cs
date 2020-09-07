using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Exception
{
    public class UnableToConnectException : System.Exception
    {
        public string DeviceServiceURL { get; }

        public string Username { get; }

        public UnableToConnectException(string deviceServiceURL, string username)
        {
            this.DeviceServiceURL = deviceServiceURL;
            this.Username = username;
        }
    }
}
