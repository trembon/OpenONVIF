using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Resources
{
    internal static class ResourceHelper
    {
        internal static string GetEmbeddedResourceAsString(string path)
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
            catch (System.Exception ex)
            {
                throw new FileNotFoundException("Embedded file was not found.", path, ex);
            }
        }
    }
}
