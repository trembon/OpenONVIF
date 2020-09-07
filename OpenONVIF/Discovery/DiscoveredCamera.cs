using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Discovery
{
    /// <summary>
    /// Class that represents a found camera with a discover request.
    /// </summary>
    public sealed class DiscoveredCamera
    {
        #region Properties
        /// <summary>
        /// Gets the name of the camera.
        /// </summary>
        /// <value>
        /// The name of the camera.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the model of the camera.
        /// </summary>
        /// <value>
        /// The model of the camera.
        /// </value>
        public string Model { get; }

        /// <summary>
        /// Gets the ip address to the camera.
        /// </summary>
        /// <value>
        /// The ip address to the camera.
        /// </value>
        public IPAddress IPAddress { get; }

        /// <summary>
        /// Gets the location of the camera.
        /// </summary>
        /// <value>
        /// The location of the camera.
        /// </value>
        public string Location { get; }

        /// <summary>
        /// Gets the device service URL to the camera.
        /// </summary>
        /// <value>
        /// The device service URL to the camera.
        /// </value>
        public string DeviceServiceURL { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveredCamera"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="model">The model.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="location">The location.</param>
        /// <param name="deviceServiceUrl">The device service URL.</param>
        internal DiscoveredCamera(string name, string model, IPAddress ip, string location, string deviceServiceUrl)
        {
            this.Name = name;
            this.IPAddress = ip;
            this.Location = location;
            this.DeviceServiceURL = deviceServiceUrl;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, IPAddress);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return DeviceServiceURL.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is DiscoveredCamera)
                return ((DiscoveredCamera)obj).DeviceServiceURL.Equals(DeviceServiceURL, StringComparison.InvariantCultureIgnoreCase);

            return false;
        }
        #endregion
    }
}
