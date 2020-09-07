using NLog;
using OpenONVIF.Surveillance.Camera;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.Webserver
{
    internal class SurveillanceWebserver
    {
        private static readonly Logger log = LogManager.GetLogger("Webserver");

        private HttpListener listener;

        private Dictionary<string, CameraStatus> boundCameras;

        public SurveillanceWebserver()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:8180/");
        }

        public void Bind(IEnumerable<CameraConnection> cameras)
        {
            this.boundCameras = new Dictionary<string, CameraStatus>();

            foreach(var camera in cameras)
            {
                if (camera.Settings.Properties.EnableWebserver)
                {
                    this.boundCameras[camera.Name] = new CameraStatus(camera.Name);
                    camera.NewFrameEvent += Camera_NewFrameEvent;
                }
            }

            listener.Start();
            BeginHandleRequest();
        }

        private void Camera_NewFrameEvent(object sender, NewCameraFrameEventArgs e)
        {
            CameraConnection camera = sender as CameraConnection;
            if (camera == null)
                return;

            var status = boundCameras[camera.Name];
            lock (status.ReadWriteLock)
            {
                var oldImage = status.LatestImage;

                status.LatestImage = new Bitmap(e.Frame);

                if (oldImage != null)
                    oldImage.Dispose();

                status.ImageTimestamp = DateTime.Now;
            }
        }

        private void BeginHandleRequest()
        {
            listener.BeginGetContext(EndHandleRequest, null);
        }

        private void EndHandleRequest(IAsyncResult ar)
        {
            HttpListenerContext context = listener.EndGetContext(ar);

            new Thread(() => { ProcessContext(context); }).Start();

            BeginHandleRequest();
        }

        private void ProcessContext(HttpListenerContext context)
        {
            try
            {
                var parsed = ParseRequest(context);

                CameraStatus cameraStatus;
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                switch (parsed.Type)
                {
                    case "index":
                        // nothing yet
                        break;

                    case "camera_view":
                        cameraStatus = boundCameras[parsed.Data[0]];

                        string file = FileHandler.GetResource("OpenONVIF.Surveillance.Webserver.Files.camera.html");
                        file = file.Replace("{name}", cameraStatus.Name);
                        file = file.Replace("{url}", String.Format("/camera/{0}.png", cameraStatus.Name));

                        byte[] fileData = Encoding.UTF8.GetBytes(file);

                        context.Response.ContentType = "text/html";
                        context.Response.ContentLength64 = fileData.Length;
                        context.Response.AddHeader("Date", DateTime.Now.ToString("r"));

                        context.Response.OutputStream.Write(fileData, 0, fileData.Length);
                        break;

                    case "camera_image":
                        cameraStatus = boundCameras[parsed.Data[0]];
                        lock (cameraStatus.ReadWriteLock)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                cameraStatus.LatestImage.Save(ms, FileHandler.GetImageFormat(parsed.Data[1]));

                                context.Response.ContentType = parsed.Data[1];
                                context.Response.ContentLength64 = ms.Length;
                                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                                context.Response.AddHeader("Last-Modified", cameraStatus.ImageTimestamp.ToString("r"));

                                ms.WriteTo(context.Response.OutputStream);
                            }
                        }
                        break;

                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                }

                context.Response.OutputStream.Flush();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error serving request");

                // TODO: kontrollera vad som går fel vid avbryten överföring för att endast fånga det
                try
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.OutputStream.Flush();
                }
                catch { /* ignore */ }
            }
            finally
            {
                context.Response.OutputStream.Close();
            }
        }

        private ParsedRequest ParseRequest(HttpListenerContext context)
        {
            ParsedRequest parsed = new ParsedRequest();

            string[] segments = context
                .Request
                .Url
                .Segments
                .Select(s => s.Length == 1 ? s : s.Replace("/", ""))
                .SelectMany(s => s.Split('.'))
                .ToArray();

            if (segments.Length == 1 && segments[0] == "/")
            {
                parsed.Type = "index";
            }
            else if (segments.Length == 3 && segments[1] == "camera" && boundCameras.ContainsKey(segments[2]))
            {
                parsed.Type = "camera_view";
                parsed.Data.Add(segments[2]);
            }
            else if (segments.Length == 4 && segments[1] == "camera" && boundCameras.ContainsKey(segments[2]) && FileHandler.IsValidImage(segments[3]))
            {
                parsed.Type = "camera_image";
                parsed.Data.Add(segments[2]);
                parsed.Data.Add(FileHandler.GetImageMime(segments[3]));
            }

            return parsed;
        }
    }
}
