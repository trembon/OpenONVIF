using OpenONVIF.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Services.Device
{
    public interface IDeviceService
    {
        IReadOnlyList<IService> GetServices();

        [AuthenticationNeeded]
        IEnumerable<System.Net.IPAddress> GetIP();
            
        [AuthenticationNeeded]
        IEnumerable<System.Net.IPAddress> GetDNS();

        [AuthenticationNeeded]
        string GetHostname();
    }
}
