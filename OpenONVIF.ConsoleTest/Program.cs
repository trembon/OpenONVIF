using OpenONVIF.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DiscoveredCamera> result = null;

            using(var discoveryRequest = new DiscoveryRequest())
            {
                discoveryRequest.CameraDiscovered += (s, e) => System.Console.WriteLine("Camera '{0}' found!", e.DiscoveredCamera);

                discoveryRequest.FindCameras(TimeSpan.FromSeconds(2));
                result = discoveryRequest.Wait();

                System.Console.WriteLine("{0} cameras found!", result.Count);
            }

            var camera = CameraFactory.ConnectToCamera(result[0], "admin", "admin");

            var ip = camera.DeviceService.GetIP();
            var dns = camera.DeviceService.GetDNS();
            var name = camera.DeviceService.GetHostname();

            var profiles = camera.MediaService.GetProfiles();

            var videoUrl = camera.MediaService.GetVideoURL();
            var snapshotUrl = camera.MediaService.GetSnapshotURL();

            System.Console.ReadLine();
        }
    }
}
