using OpenONVIF.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Services.Media
{
    public interface IMediaService
    {
        IMediaProfile CurrentProfile { get; set; }

        IEnumerable<IMediaProfile> GetProfiles();

        string GetVideoURL();

        string GetSnapshotURL();
    }
}
