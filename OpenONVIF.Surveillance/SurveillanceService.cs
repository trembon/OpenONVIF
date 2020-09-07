using AForge.Video;
using NLog;
using OpenONVIF.Surveillance.Camera;
using OpenONVIF.Surveillance.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance
{
    internal class SurveillanceService
    {
        private List<CameraConnection> connections;

        public int NrOfCameras
        {
            get
            {
                if (connections == null)
                    return 0;

                return connections.Count;
            }
        }

        public IReadOnlyList<CameraConnection> Cameras
        {
            get { return connections.AsReadOnly(); }
        }

        public SurveillanceService()
        {
            connections = new List<CameraConnection>();

            foreach (var xmlCamera in XmlConfiguration.Instance.Cameras)
                connections.Add(new CameraConnection(xmlCamera));
        }

        public void Start()
        {
            foreach (var camera in connections)
                camera.StartProcessing();
        }

        public void Stop()
        {
            foreach (var camera in connections)
                camera.StopProcessing();
        }
    }
}
