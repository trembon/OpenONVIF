using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.Webserver
{
    internal static class FileHandler
    {
        private static Dictionary<string, string> imageMimes;

        static FileHandler()
        {
            imageMimes = new Dictionary<string, string>();
            imageMimes.Add("png", "image/png");
            imageMimes.Add("jpg", "image/jpeg");
            imageMimes.Add("jpeg", "image/jpeg");
            imageMimes.Add("gif", "image/gif");
        }

        internal static string GetResource(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(path))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Embedded file was not found.", path, ex);
            }
        }

        internal static bool IsValidImage(string extension)
        {
            return imageMimes.ContainsKey(extension);
        }

        internal static string GetImageMime(string extension)
        {
            if (IsValidImage(extension))
                return imageMimes[extension];

            throw new ArgumentException("Invalid image extension");
        }

        internal static ImageFormat GetImageFormat(string mime)
        {
            switch (mime)
            {
                case "image/png": return ImageFormat.Png;
                case "image/jpeg": return ImageFormat.Jpeg;
                case "image/gif": return ImageFormat.Gif;
            }

            throw new ArgumentException("Invalid image mime");
        }
    }
}
