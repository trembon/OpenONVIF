using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Discovery
{
    /// <summary>
    /// Event arguments for the camera discovered event.
    /// </summary>
    public sealed class CameraDiscoveredEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the discovered camera.
        /// </summary>
        /// <value>
        /// The discovered camera.
        /// </value>
        public DiscoveredCamera DiscoveredCamera { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraDiscoveredEventArgs"/> class.
        /// </summary>
        /// <param name="camera">The camera.</param>
        internal CameraDiscoveredEventArgs(DiscoveredCamera camera)
        {
            this.DiscoveredCamera = camera;
        }
    }
}
