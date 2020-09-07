using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using OpenONVIF.Extensions;
using System.Xml;
using OpenONVIF.Resources;
using System.Threading;

namespace OpenONVIF.Discovery
{
    /// <summary>
    /// Class to perform a discovery request to find ONVIF cameras on the network.
    /// </summary>
    public sealed class DiscoveryRequest : IDisposable
    {
        #region Constants
        private const string SOAP_MESSAGE_FILE = "OpenONVIF.Discovery.Resources.DiscoverySOAPMessage.xml";
        private const string MULTICAST_IP = "239.255.255.250";
        private const int MULTICAST_PORT = 3702;
        #endregion

        #region Private fields
        private static bool isSearching;
        private static object isSearchingLock = new object();

        private System.Timers.Timer timer;
        private UdpClient listeningClient;
        private object listeningClientLock = new object();

        private Guid messageGuid;

        private AutoResetEvent resetEvent;
        private List<DiscoveredCamera> discoveredCameras;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a camera is discovered on the network.
        /// </summary>
        public event EventHandler<CameraDiscoveredEventArgs> CameraDiscovered;

        /// <summary>
        /// Occurs when the discovery timeout expired and the client is not longer looking for cameras.
        /// </summary>
        public event EventHandler TimeoutExpired;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is searching for cameras.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is searching for cameras; otherwise, <c>false</c>.
        /// </value>
        public bool IsSearching
        {
            get
            {
                lock (isSearchingLock)
                    return isSearching;
            }
            private set
            {
                lock (isSearchingLock)
                    isSearching = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="DiscoveryRequest"/> class.
        /// </summary>
        static DiscoveryRequest()
        {
            isSearching = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryRequest"/> class.
        /// </summary>
        public DiscoveryRequest()
        {
            resetEvent = new AutoResetEvent(false);
        }
        #endregion

        /// <summary>
        /// Sends out a discovery message to all cameras on the same network as the current machine.
        /// </summary>
        /// <param name="timeout">Wait for the specified time before triggering the TimeoutExpired event.</param>
        /// <exception cref="System.InvalidOperationException">Client (or another instance) is already searching for cameras.</exception>
        public void FindCameras(TimeSpan timeout)
        {
            if (IsSearching)
                throw new InvalidOperationException("Client (or another instance) is already searching for cameras.");

            // create variables
            discoveredCameras = new List<DiscoveredCamera>();
            messageGuid = Guid.NewGuid();

            // creates an udp client to listen on camera responses
            StartListenForCameras();

            // create the discovery message to send to the cameras
            string message = ResourceHelper.GetEmbeddedResourceAsString(SOAP_MESSAGE_FILE);
            message = String.Format(message, messageGuid);
            byte[] bytes = Encoding.ASCII.GetBytes(message);

            // send the message from all network cards found on the local computer
            var multicastIP = IPAddress.Parse(MULTICAST_IP);
            foreach (IPAddress localIP in GetLocalIPAddresses())
            {
                // create a new udp client for each request
                using (UdpClient client = new UdpClient())
                {
                    // set the udp client to join the discovery multicast group
                    client.MulticastLoopback = true;
                    client.JoinMulticastGroup(multicastIP, localIP);
                    client.Client.Bind(new IPEndPoint(localIP, MULTICAST_PORT));

                    // send the discovery message
                    var endpoint = new IPEndPoint(multicastIP, MULTICAST_PORT);
                    client.Send(bytes, bytes.Length, endpoint);
                }
            }

            StartTimeout(timeout);
        }

        #region Search methods
        /// <summary>
        /// Starts the listen for cameras.
        /// </summary>
        private void StartListenForCameras()
        {
            listeningClient = new UdpClient(MULTICAST_PORT);
            listeningClient.JoinMulticastGroup(IPAddress.Parse(MULTICAST_IP));
            listeningClient.MulticastLoopback = true;
            listeningClient.BeginReceive(ListeningReceive, new object());
        }

        /// <summary>
        /// Receive method to handle the camera responses.
        /// </summary>
        /// <param name="ar">The async result.</param>
        private void ListeningReceive(IAsyncResult ar)
        {
            IPEndPoint source = null;
            byte[] bytes = null;

            lock (listeningClientLock)
            {
                // check so client is still active
                if (listeningClient == null)
                    return;

                // receive the bytes from the source
                source = new IPEndPoint(IPAddress.Any, MULTICAST_PORT);
                bytes = listeningClient.EndReceive(ar, ref source);

                // continue to listen for more packets
                listeningClient.BeginReceive(ListeningReceive, new object());
            }

            try
            {
                // convert the response to readable text
                string response = Encoding.UTF8.GetString(bytes);
                using (StringReader responseReader = new StringReader(response))
                {
                    // parse the xml soap message to a discovery response
                    XmlSerializer serializer = new XmlSerializer(typeof(DiscoveryResponse));
                    DiscoveryResponse discoveryMessage = (DiscoveryResponse)serializer.Deserialize(XmlReader.Create(responseReader));

                    // validate response guid in the response, so it corresponds to the guid we sent with the discovery request
                    Guid responseID;
                    if (Guid.TryParse(discoveryMessage.Header.RelatesTo.Substring(5), out responseID) && responseID == messageGuid)
                    {
                        // get the main soap service url for the camera
                        string deviceServiceURL = discoveryMessage.Body.ProbeMatches.ProbeMatch.XAddrs;

                        // get the properties about the camera
                        string[] properties = discoveryMessage.Body.ProbeMatches.ProbeMatch.Scopes.Split(' ');
                        string name = properties.First(p => p.StartsWith("onvif://www.onvif.org/name/")).Substring("onvif://www.onvif.org/name/".Length);
                        string model = properties.First(p => p.StartsWith("onvif://www.onvif.org/model/")).Substring("onvif://www.onvif.org/model/".Length);
                        string location = properties.First(p => p.StartsWith("onvif://www.onvif.org/location/")).Substring("onvif://www.onvif.org/location/".Length);

                        // create a new "camera" instance and trigger event that it is found
                        var camera = new DiscoveredCamera(name, model, source.Address, location, deviceServiceURL);
                        CameraDiscovered.Trigger(this, new CameraDiscoveredEventArgs(camera));

                        // add the camera to the local list
                        discoveredCameras.Add(camera);
                    }
                }
            }
            catch { /* Probably just an invalid response, nothing to do here */ }
        }
        #endregion

        #region Wait methods
        /// <summary>
        /// Waits for this instance to complete the discovery process, blocks the current thread, and return a list of all found cameras.
        /// </summary>
        public List<DiscoveredCamera> Wait()
        {
            return this.WaitInternal();
        }

        /// <summary>
        /// Waits for this instance to complete the discovery process and return a list of all found cameras.
        /// </summary>
        public async Task<List<DiscoveredCamera>> WaitAsync()
        {
            return await Task.Run(() =>
            {
                resetEvent.WaitOne();
                return discoveredCameras;
            });
        }

        /// <summary>
        /// Waits for this instance to complete the discovery process, blocks the current thread, and return a list of all found cameras.
        /// </summary>
        private List<DiscoveredCamera> WaitInternal()
        {
            resetEvent.WaitOne();
            return discoveredCameras;
        }
        #endregion

        #region Timeout methods
        /// <summary>
        /// Starts the timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        private void StartTimeout(TimeSpan timeout)
        {
            timer = new System.Timers.Timer();
            timer.AutoReset = false;
            timer.Interval = timeout.TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        /// <summary>
        /// Handles the Elapsed event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeoutExpired.Trigger(this, new EventArgs());

            resetEvent.Set();

            timer.Stop();
            IsSearching = false;

            lock (listeningClientLock)
            {
                if (listeningClient != null)
                {
                    listeningClient.Dispose();
                    listeningClient = null;
                }
            }
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }

            lock (listeningClientLock)
            {
                if (listeningClient != null)
                {
                    listeningClient.Dispose();
                    listeningClient = null;
                }
            }

        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Gets the local ip addresses of the current computer.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IPAddress> GetLocalIPAddresses()
        {
            string hostname = Dns.GetHostName();
            IPAddress[] ipAddresses = Dns.GetHostAddresses(hostname);
            return ipAddresses.Where(i => i.AddressFamily == AddressFamily.InterNetwork);
        }
        #endregion
    }
}
