using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenONVIF.Surveillance.Configuration
{
    internal class XmlConfiguration
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private const string XML_PATH_KEY = "XmlPath";

        internal string ImagePath { get; }

        internal List<XmlCamera> Cameras { get; }

        public XmlConfiguration(string filePath)
        {
            Cameras = new List<XmlCamera>();

            try
            {
                XDocument document = XDocument.Load(filePath, LoadOptions.None);
                var rootNode = document.Root.Element("surveillance");

                var settingsNode = rootNode.Element("settings");
                this.ImagePath = settingsNode.Attribute("imagePath").Value;

                var cameraElements = rootNode.Element("cameras").Elements("camera");
                foreach (var cameraElement in cameraElements)
                    Cameras.Add(new XmlCamera(cameraElement));
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to load configuration from file '{0}'", filePath);
            }
        }

        #region Singleton
        private static XmlConfiguration instance;
        private static object instanceLock = new object();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public static XmlConfiguration Instance
        {
            get
            {
                lock(instanceLock)
                {
                    if (instance == null)
                    {
                        // get the configured path of the configuration file
                        string xmlPath = ConfigurationManager.AppSettings[XML_PATH_KEY];
                        string filePath = Path.Combine(Environment.CurrentDirectory, xmlPath);

                        // check if file exists at the relative path, or at a fulle path
                        if (File.Exists(filePath))
                        {
                            instance = new XmlConfiguration(filePath);
                        }
                        else if (File.Exists(xmlPath))
                        {
                            instance = new XmlConfiguration(xmlPath);
                        }
                        else
                        {
                            // else throw file not found exception to close the app
                            throw new FileNotFoundException(String.Format("The configuration file '{0}' was not found.", xmlPath));
                        }
                    }

                    return instance;
                }
            }
        }
        #endregion
    }
}
