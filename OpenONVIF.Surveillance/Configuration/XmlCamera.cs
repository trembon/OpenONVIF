using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenONVIF.Surveillance.Configuration
{
    internal class XmlCamera
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImageURL { get; }

        public Rectangle[] MotionZones { get; }

        public XmlCameraProperties Properties { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlCamera"/> class.
        /// </summary>
        /// <param name="xmlElement">The XML element.</param>
        public XmlCamera(XElement xmlElement)
        {
            this.Name = xmlElement.Attribute("name").Value;
            this.ImageURL = xmlElement.Attribute("imageUrl").Value;

            this.Properties = new XmlCameraProperties(xmlElement.Element("properties"));

            this.MotionZones = ParseMotionZones(xmlElement);
        }

        private Rectangle[] ParseMotionZones(XElement xmlElement)
        {
            var zones = new List<Rectangle>();

            // parse motion zones
            var element = xmlElement.Element("motionZones");
            if (element != null)
            {
                foreach(var zone in element.Elements("zone"))
                {
                    int x = int.Parse(zone.Attribute("x").Value);
                    int y = int.Parse(zone.Attribute("y").Value);
                    int width = int.Parse(zone.Attribute("width").Value);
                    int height = int.Parse(zone.Attribute("height").Value);

                    zones.Add(new Rectangle(x, y, width, height));
                }
            }

            if (zones.Count == 0)
                return null;

            return zones.ToArray();
        }
    }
}
