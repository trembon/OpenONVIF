using OpenONVIF.Discovery;
using OpenONVIF.Exception;
using OpenONVIF.Security.Soap;
using OpenONVIF.Services;
using OpenONVIF.Services.Device;
using OpenONVIF.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

// TODO: implement interface

namespace OpenONVIF
{
    /// <summary>
    /// Represents an ONVIF Camera.
    /// </summary>
    public class Camera
    {
        public IReadOnlyList<IService> AvailableServices { get; internal set; }

        public IDeviceService DeviceService
        {
            get
            {
                var deviceService = this.AvailableServices.OfType<IDeviceService>().FirstOrDefault();

                if (deviceService == null)
                    throw new CameraNotCapableException("DeviceService"); // TODO: correct exception

                return deviceService;
            }
        }

        public IMediaService MediaService
        {
            get
            {
                var mediaService = this.AvailableServices.OfType<IMediaService>().FirstOrDefault();

                if (mediaService == null)
                    throw new CameraNotCapableException("MediaService"); // TODO: correct exception

                return mediaService;
            }
        }

        internal PasswordDigestBehavior PasswordDigestBehavior { get; private set; }

        internal Camera(PasswordDigestBehavior passwordDigestBehavior)
        {
            this.PasswordDigestBehavior = passwordDigestBehavior;

            this.AvailableServices = new List<IService>();
        }
    }
}
