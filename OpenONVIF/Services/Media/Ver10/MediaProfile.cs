using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Services.Media.Ver10
{
    public class MediaProfile : IMediaProfile
    {
        public string Name { get; internal set; }

        public string Token { get; internal set; }

        public Services.Media.AudioEncoding AudioEncoding { get; internal set; }

        public int AudioBitrate { get; internal set; }

        public Services.Media.VideoEncoding VideoEncoding { get; internal set; }

        public int VideoBitrate { get; internal set; }

        public int VideoFrameRate { get; internal set; }

        public int VideoWidth { get; internal set; }

        public int VideoHeight { get; internal set; }
    }
}
