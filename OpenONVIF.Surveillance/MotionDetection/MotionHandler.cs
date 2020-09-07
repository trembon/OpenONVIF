using AForge.Vision.Motion;
using OpenONVIF.Surveillance.Camera;
using OpenONVIF.Surveillance.Camera.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.MotionDetection
{
    internal sealed class MotionHandler : IDisposable
    {
        private CameraConnection camera;

        private CurrentMotion currentMotion;
        private TimeOfDay lastTimeOfDay = TimeOfDay.Undetermined;

        private MotionDetector motionDetector;
        private SimpleBackgroundModelingDetector detector;
        private BlobCountingObjectsProcessing processor;

        public MotionHandler(CameraConnection camera)
        {
            this.camera = camera;

            if (camera.Settings.Properties.EnableMotionDetection)
            {
                // create the motion sensor and processing
                detector = new SimpleBackgroundModelingDetector();
                processor = new BlobCountingObjectsProcessing(20, 20); // minimum size of objects to find

                // create the motion detector
                motionDetector = new MotionDetector(detector, processor);
                motionDetector.MotionZones = camera.Settings.MotionZones;
                
                // bind events
                this.camera.NewFrameEvent += Camera_NewFrameEvent;
            }
        }

        private void Camera_NewFrameEvent(object sender, NewCameraFrameEventArgs e)
        {
            // create a clone of the current image to save if any motion is detected
            using (Bitmap clone = new Bitmap(e.Frame))
            {
                // process the current frame
                var result = motionDetector.ProcessFrame(clone);
                float calculatedResult = float.Parse(result.ToString("N4"));

                // check if there was any motion detected, by both motion and object found count
                if (calculatedResult >= camera.Settings.Properties.MotionSense && processor.ObjectsCount > 0)
                {
                    if (currentMotion == null)
                        currentMotion = new CurrentMotion(camera);

                    DateTime now = DateTime.Now;

                    string processedFile = Path.Combine(camera.BasePath, "motion", String.Format("{0}_framed_{1}.jpg", currentMotion.MotionID, now.Ticks));
                    Directory.CreateDirectory(Path.GetDirectoryName(processedFile));

                    clone.Save(processedFile, ImageFormat.Jpeg);
                    e.Frame.Save(Path.Combine(camera.BasePath, "motion", String.Format("{0}_raw_{1}.jpg", currentMotion.MotionID, now.Ticks)), ImageFormat.Jpeg);

                    currentMotion.AddMotion(processedFile, calculatedResult, processor.ObjectsCount, now);

                    camera.Log.Info("Motion detected (sense: {0}, count: {1})", calculatedResult, processor.ObjectsCount);
                }
                else
                {
                    if (currentMotion != null)
                    {
                        // TODO: verifiera att detta fungerar bra, eller om man ska nollställa sensorn, eller ha två sensorer för natt och dag.
                        if (lastTimeOfDay == e.TimeOfDay)
                        {
                            currentMotion.Process();
                        }
                        currentMotion = null;
                    }

                    lastTimeOfDay = e.TimeOfDay;
                }
            }
        }

        public void Dispose()
        {
            if (currentMotion != null)
            {
                currentMotion.Process();
                currentMotion = null;
            }
        }
    }
}
