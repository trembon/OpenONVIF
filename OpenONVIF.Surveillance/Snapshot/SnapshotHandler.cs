using OpenONVIF.Surveillance.Camera;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.Snapshot
{
    internal class SnapshotHandler : IDisposable
    {
        private CameraConnection camera;

        private DateTime lastSnapshot;

        public SnapshotHandler(CameraConnection camera)
        {
            this.camera = camera;

            if (camera.Settings.Properties.EnableSnapshots)
            {
                // bind events
                this.camera.NewFrameEvent += Camera_NewFrameEvent;
            }
        }

        private void Camera_NewFrameEvent(object sender, NewCameraFrameEventArgs e)
        {
            DateTime now = DateTime.Now;

            TimeSpan timeSinceLastSnapshot = now - lastSnapshot;
            if(timeSinceLastSnapshot.TotalSeconds >= camera.Settings.Properties.SnapshotInternal)
            {
                string snapshotFile = Path.Combine(camera.BasePath, "snapshots", String.Format("snap_{0}.jpg", now.Ticks));

                Directory.CreateDirectory(Path.GetDirectoryName(snapshotFile));
                e.Frame.Save(snapshotFile, ImageFormat.Jpeg);

                lastSnapshot = now;

                camera.Log.Info("Snapshot taken");
            }
        }

        public void Dispose()
        {
            lastSnapshot = DateTime.MinValue;
        }
    }
}
