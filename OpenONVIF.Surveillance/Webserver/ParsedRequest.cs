using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.Webserver
{
    internal class ParsedRequest
    {
        public string Type { get; set; }

        public List<string> Data { get; set; }

        public ParsedRequest()
        {
            Type = "unknown";
            Data = new List<string>();
        }
    }
}
