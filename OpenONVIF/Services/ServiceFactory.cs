using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

// TODO: testable with implemented interface

namespace OpenONVIF.Services
{
    internal static class ServiceFactory
    {
        private static Dictionary<string, Type> serviceNamespaces;

        static ServiceFactory()
        {
            serviceNamespaces = new Dictionary<string, Type>();
            serviceNamespaces.Add("http://www.onvif.org/ver10/device/wsdl", typeof(Device.Ver10.DeviceService));
            serviceNamespaces.Add("http://www.onvif.org/ver10/media/wsdl", typeof(Media.Ver10.MediaService));
            //http://www.onvif.org/ver10/events/wsdl
            //http://www.onvif.org/ver20/analytics/wsdl
        }
        
        internal static CustomBinding CreateServiceBinding()
        {
            var httpTransportBinding = new HttpTransportBindingElement { AuthenticationScheme = AuthenticationSchemes.Basic };
            var textMessageEncodingBinding = new TextMessageEncodingBindingElement { MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None) };
            return new CustomBinding(textMessageEncodingBinding, httpTransportBinding);
        }

        internal static EndpointAddress CreateEndpoint(string serviceUrl)
        {
            return new EndpointAddress(serviceUrl);
        }

        internal static IService CreateService(string serviceNamespace, Camera camera, string serviceUrl)
        {
            IService service = null;
            if (serviceNamespaces.ContainsKey(serviceNamespace))
                service = Activator.CreateInstance(serviceNamespaces[serviceNamespace], BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { camera, serviceUrl }, CultureInfo.CurrentCulture) as IService;

            return service;
        }
    }
}
