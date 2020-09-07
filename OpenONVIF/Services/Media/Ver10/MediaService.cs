using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Services.Media.Ver10
{
    internal class MediaService : BaseService, IMediaService
    {
        private MediaClient mediaClient;

        private IMediaProfile currentProfile;

        public IMediaProfile CurrentProfile
        {
            get
            {
                if (currentProfile == null)
                    currentProfile = this.GetProfiles().FirstOrDefault();

                return currentProfile;
            }
            set
            {
                if (this.currentProfile == null && value != null)
                {
                    this.currentProfile = value;
                }
            }
        }

        internal MediaService(Camera camera, string serviceUrl) : base(camera)
        {
            mediaClient = new MediaClient(ServiceFactory.CreateServiceBinding(), ServiceFactory.CreateEndpoint(serviceUrl));
            mediaClient.Endpoint.Behaviors.Add(camera.PasswordDigestBehavior);
            mediaClient.ClientCredentials.UserName.UserName = camera.PasswordDigestBehavior.Username;
            mediaClient.ClientCredentials.UserName.Password = camera.PasswordDigestBehavior.Password;
        }
        
        public IEnumerable<IMediaProfile> GetProfiles()
        {
            var value = CallCamera(() => mediaClient.GetProfiles());
            if (value == null)
                return new List<IMediaProfile>();
            
            return value.Select(p => new MediaProfile {
                Name = p.Name,
                Token = p.token,

                AudioEncoding = (Services.Media.AudioEncoding)Enum.Parse(typeof(Services.Media.AudioEncoding), p.AudioEncoderConfiguration.Encoding.ToString()),
                AudioBitrate = p.AudioEncoderConfiguration.Bitrate,
                
                VideoEncoding = (Services.Media.VideoEncoding)Enum.Parse(typeof(Services.Media.VideoEncoding), p.VideoEncoderConfiguration.Encoding.ToString()),
                VideoBitrate = p.VideoEncoderConfiguration.RateControl.BitrateLimit,
                VideoFrameRate = p.VideoEncoderConfiguration.RateControl.FrameRateLimit,
                VideoHeight = p.VideoEncoderConfiguration.Resolution.Height,
                VideoWidth = p.VideoEncoderConfiguration.Resolution.Width
            });
        }

        public string GetVideoURL()
        {
            StreamSetup streamSetup = new StreamSetup();
            streamSetup.Stream = StreamType.RTPUnicast;
            streamSetup.Transport = new Transport();
            streamSetup.Transport.Protocol = TransportProtocol.HTTP;

            var value = CallCamera(() => mediaClient.GetStreamUri(streamSetup, CurrentProfile.Token));
            if (value == null)
                return null;

            return value.Uri;
        }

        public string GetSnapshotURL()
        {
            var value = CallCamera(() => mediaClient.GetSnapshotUri(CurrentProfile.Token));
            if (value == null)
                return null;

            return value.Uri;
        }
    }
}
