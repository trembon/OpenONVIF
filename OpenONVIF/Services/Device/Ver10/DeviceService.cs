using OpenONVIF.Security.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Services.Device.Ver10
{
    internal class DeviceService : BaseService, IDeviceService
    {
        private DeviceClient deviceClient;

        internal DeviceService(Camera camera, string serviceUrl) : base(camera)
        {
            deviceClient = new DeviceClient(ServiceFactory.CreateServiceBinding(), ServiceFactory.CreateEndpoint(serviceUrl));
            deviceClient.Endpoint.Behaviors.Add(camera.PasswordDigestBehavior);
            deviceClient.ClientCredentials.UserName.UserName = camera.PasswordDigestBehavior.Username;
            deviceClient.ClientCredentials.UserName.Password = camera.PasswordDigestBehavior.Password;

            // load capabilities for the service
            var capabilities = deviceClient.GetCapabilities(new CapabilityCategory[] { CapabilityCategory.Device });
            // TODO: parse capabilities
        }

        public IReadOnlyList<IService> GetServices()
        {
            var loadedServices = Camera.AvailableServices;
            if (loadedServices.Count > 0)
                return loadedServices;

            var services = deviceClient.GetServices(false);

            List<IService> availableServices = new List<IService>(services.Length);
            foreach (var rawService in services)
            {
                var service = ServiceFactory.CreateService(rawService.Namespace, this.Camera, rawService.XAddr);
                if (service != null)
                    availableServices.Add(service);
            }

            return availableServices.AsReadOnly();
        }

        public IEnumerable<System.Net.IPAddress> GetDNS()
        {
            var dnsInformation = CallCamera(() => deviceClient.GetDNS());
            if (dnsInformation.FromDHCP)
                return dnsInformation.DNSFromDHCP.Select(ip => System.Net.IPAddress.Parse(ip.IPv4Address)); 

            return dnsInformation.DNSManual.Select(ip => System.Net.IPAddress.Parse(ip.IPv4Address));
        }

        public IEnumerable<System.Net.IPAddress> GetIP()
        {
            var networkInterfaces = CallCamera(() => deviceClient.GetNetworkInterfaces());
            return networkInterfaces.Select(ni => System.Net.IPAddress.Parse(ni.IPv4.Config.LinkLocal.Address));
        }

        public string GetHostname()
        {
            var hostname = CallCamera(() => deviceClient.GetHostname());
            return hostname.Name;
        }
    }
}
