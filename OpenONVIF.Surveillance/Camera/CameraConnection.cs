using AForge.Video;
using AForge.Vision.Motion;
using NLog;
using OpenONVIF.Surveillance.Camera.Enums;
using OpenONVIF.Surveillance.Configuration;
using OpenONVIF.Surveillance.Extensions;
using OpenONVIF.Surveillance.MotionDetection;
using OpenONVIF.Surveillance.Snapshot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.Camera
{
    internal class CameraConnection
    {
        private bool active;
        private AutoResetEvent autoEvent;
        private WebClient webClient;

        private MotionHandler motionHandler;
        private SnapshotHandler snapshotHandler;

        private object newFrameLock = new object();

        #region Properties
        public string Name
        {
            get
            {
                return this.Settings.Name;
            }
        }

        public string BasePath
        {
            get
            {
                return Path.Combine(XmlConfiguration.Instance.ImagePath, this.Name, DateTime.Today.ToShortDateString());
            }
        }

        public Logger Log { get; }

        public XmlCamera Settings { get; }
        #endregion

        #region Events
        internal event EventHandler<NewCameraFrameEventArgs> NewFrameEvent;
        #endregion

        public CameraConnection(XmlCamera settings)
        {
            this.active = false;
            this.Settings = settings;
            this.webClient = new WebClient();

            // set the variables
            this.Log = LogManager.GetLogger("Camera - " + this.Name);

            this.motionHandler = new MotionHandler(this);
            this.snapshotHandler = new SnapshotHandler(this);
        }

        public void StartProcessing()
        {
            active = true;
            autoEvent = new AutoResetEvent(false);
            Task.Delay(50).ContinueWith(_ => GetImage());
        }

        public void StopProcessing()
        {
            active = false;
            autoEvent.WaitOne();
        }

        private void GetImage()
        {
            if (!active)
            {
                autoEvent.Set();
                return;
            }

            try
            {
                using (var stream = webClient.OpenRead(this.Settings.ImageURL))
                {
                    using (Bitmap image = Bitmap.FromStream(stream) as Bitmap)
                    {
                        TimeOfDay timeOfDay = TimeOfDay.Undetermined;
                        if (this.Settings.Properties.DetermineTimeOfDayByGreyscale)
                        {
                            if (image.IsGreyScale(this.Settings.Properties.GreyscaleThreshold))
                            {
                                timeOfDay = TimeOfDay.Night;
                            }
                            else
                            {
                                timeOfDay = TimeOfDay.Day;
                            }
                        }

                        var eventData = new NewCameraFrameEventArgs(image, timeOfDay);
                        NewFrameEvent.Invoke(this, eventData);

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to download image.");
            }

            Task.Delay(150).ContinueWith(_ =>
            {
                GetImage();
            });
        }
    }
}
