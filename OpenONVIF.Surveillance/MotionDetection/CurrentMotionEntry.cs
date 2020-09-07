using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.MotionDetection
{
    public class CurrentMotionEntry
    {
        public string ProcessedFrameFile { get; }

        public DateTime Timestamp { get; }

        public float MotionCount { get; }

        public int ObjectCount { get; }

        public CurrentMotionEntry(string processedFrameFile, float motionCount, int objectCount, DateTime timestamp)
        {
            this.ProcessedFrameFile = processedFrameFile;
            this.MotionCount = motionCount;
            this.ObjectCount = objectCount;
            this.Timestamp = timestamp;
        }
    }
}
