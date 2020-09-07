using OpenONVIF.Discovery;
using OpenONVIF.Exception;
using OpenONVIF.Security.Soap;
using OpenONVIF.Services.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: testable with implemented interface

namespace OpenONVIF
{
    public static class CameraFactory
    {
        #region Connect methods
        /// <summary>
        /// Connects to a camera, with an anonymous connection.
        /// </summary>
        /// <param name="discoveredCamera">The discovered camera.</param>
        /// <returns></returns>
        public static Camera ConnectToCamera(DiscoveredCamera discoveredCamera)
        {
            return ConnectToCamera(discoveredCamera.DeviceServiceURL, null, null);
        }

        /// <summary>
        /// Connects to a camera.
        /// </summary>
        /// <param name="discoveredCamera">The discovered camera.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static Camera ConnectToCamera(DiscoveredCamera discoveredCamera, string username, string password)
        {
            return ConnectToCamera(discoveredCamera.DeviceServiceURL, username, password);
        }

        /// <summary>
        /// Connects to a camera, with an anonymous connection.
        /// </summary>
        /// <param name="deviceServiceURL">The device service URL.</param>
        /// <returns></returns>
        public static Camera ConnectToCamera(string deviceServiceURL)
        {
            return ConnectToCamera(deviceServiceURL, null, null);
        }

        /// <summary>
        /// Connects to a camera.
        /// </summary>
        /// <param name="deviceServiceURL">The device service URL.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static Camera ConnectToCamera(string deviceServiceURL, string username, string password)
        {
            PasswordDigestBehavior passwordDigestBehavior = null;
            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password))
                passwordDigestBehavior = new PasswordDigestBehavior(username, password);

            Camera camera = new Camera(passwordDigestBehavior);

            IDeviceService deviceService = null;
            try
            {
                deviceService = new Services.Device.Ver10.DeviceService(camera, deviceServiceURL);
            }
            catch (System.Exception ex)
            {
                // TODO: log and create a reason why it was unable to connect
            }

            if (deviceService == null)
                throw new UnableToConnectException(deviceServiceURL, username);
            
            camera.AvailableServices = deviceService.GetServices();
            
            return camera;
        }
        #endregion
    }
}
