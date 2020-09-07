using NLog;
using OpenONVIF.Surveillance.Camera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.MotionDetection
{
    internal class CurrentMotion
    {
        private static int idCount;
        private static DateTime idCountDate;

        private CameraConnection camera;

        private List<CurrentMotionEntry> entries;

        public string MotionID { get; }

        public CurrentMotion(CameraConnection camera)
        {
            if (idCountDate != DateTime.Today)
            {
                idCount = 0;
                idCountDate = DateTime.Today;
            }
            idCount++;

            this.MotionID = idCount.ToString().PadLeft(4, '0');

            this.camera = camera;
            this.entries = new List<CurrentMotionEntry>();

            camera.Log.Info("Motion started, ID {0}", MotionID);
        }

        public void AddMotion(string processedFrameFile, float motionCount, int objectCount, DateTime timestamp)
        {
            entries.Add(new CurrentMotionEntry(processedFrameFile, motionCount, objectCount, timestamp));
        }

        public void Process()
        {
            camera.Log.Info("Motion ended, ID {0}", MotionID);
            camera.Log.Info("Stats - Nr of items: {0}, Average motion count: {1}, Average object count: {2}", entries.Count, entries.Average(e => e.MotionCount), entries.Average(e => e.ObjectCount));
            camera.Log.Info("Image in center of it: {0}", entries.OrderBy(e => e.Timestamp).Skip(entries.Count / 2).First().ProcessedFrameFile);
            
            File.WriteAllLines(Path.Combine(camera.BasePath, "motion", String.Format("{0}_data.txt", this.MotionID)), entries.Select(e => String.Format("{0} - Motion: {1}, Objects: {2} (Image: {3})", e.Timestamp, e.MotionCount, e.ObjectCount, e.ProcessedFrameFile)).ToArray());
        }
    }
}
