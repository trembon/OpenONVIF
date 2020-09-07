using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Services.Media
{
    public interface IMediaProfile
    {
        string Name { get; }

        string Token { get; }

        AudioEncoding AudioEncoding { get; }

        int AudioBitrate { get; }

        VideoEncoding VideoEncoding { get; }

        int VideoBitrate { get; }

        int VideoFrameRate { get; }

        int VideoWidth { get; }

        int VideoHeight { get; }
    }
}
