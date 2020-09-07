using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenONVIF.Surveillance.Configuration
{
    internal class XmlCameraProperties
    {
        public bool EnableSnapshots { get; set; }

        public int SnapshotInternal { get; set; }

        public bool EnableMotionDetection { get; set; }

        public bool DetermineTimeOfDayByGreyscale { get; set; }

        public int GreyscaleThreshold { get; set; }

        public bool EnableWebserver { get; set; }

        /// <summary>
        /// Gets the motion sense.
        /// </summary>
        /// <value>
        /// The motion sense.
        /// </value>
        public float MotionSense { get; }

        public XmlCameraProperties(XElement xmlElement)
        {
            this.EnableSnapshots = GetProperty(xmlElement, "enableSnapshots", true);
            this.SnapshotInternal = GetProperty(xmlElement, "snapshotInternal", 60);

            this.EnableMotionDetection = GetProperty(xmlElement, "enableMotionDetection", true);
            this.MotionSense = GetProperty(xmlElement, "motionSense", 0F);

            this.DetermineTimeOfDayByGreyscale = GetProperty(xmlElement, "determineTimeOfDayByGreyscale", true);
            this.GreyscaleThreshold = GetProperty(xmlElement, "greyscaleThreshold", 10);

            this.EnableWebserver = GetProperty(xmlElement, "enableWebserver", true);
        }

        private T GetProperty<T>(XElement root, string propertyName, T defaultValue)
        {
            var properties = root.Elements("set");

            var property = properties.Where(x => x.Attribute("name").Value == propertyName).FirstOrDefault();
            if(property != null)
                return (T)Convert.ChangeType(property.Attribute("value").Value, typeof(T));

            return defaultValue;
        }
    }
}
