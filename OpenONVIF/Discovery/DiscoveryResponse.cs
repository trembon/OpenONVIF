using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenONVIF.Discovery
{
    /// <summary>
    /// The discovery respone from a camera, parsed from XML.
    /// </summary>
    [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public class DiscoveryResponse
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        [XmlElement(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public HeaderClass Header { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [XmlElement(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public BodyClass Body { get; set; }

        /// <summary>
        /// Gets or sets the soapenv.
        /// </summary>
        /// <value>
        /// The soapenv.
        /// </value>
        [XmlAttribute(AttributeName = "soapenv", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Soapenv { get; set; }

        /// <summary>
        /// Gets or sets the wsadis.
        /// </summary>
        /// <value>
        /// The wsadis.
        /// </value>
        [XmlAttribute(AttributeName = "wsadis", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wsadis { get; set; }

        /// <summary>
        /// Gets or sets the d.
        /// </summary>
        /// <value>
        /// The d.
        /// </value>
        [XmlAttribute(AttributeName = "d", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string D { get; set; }

        /// <summary>
        /// Gets or sets the dn.
        /// </summary>
        /// <value>
        /// The dn.
        /// </value>
        [XmlAttribute(AttributeName = "dn", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dn { get; set; }

        /// <summary>
        /// The header class.
        /// </summary>
        [XmlRoot(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public class HeaderClass
        {
            /// <summary>
            /// Gets or sets to.
            /// </summary>
            /// <value>
            /// To.
            /// </value>
            [XmlElement(ElementName = "To", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public string To { get; set; }

            /// <summary>
            /// Gets or sets the action.
            /// </summary>
            /// <value>
            /// The action.
            /// </value>
            [XmlElement(ElementName = "Action", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public string Action { get; set; }

            /// <summary>
            /// Gets or sets the message identifier.
            /// </summary>
            /// <value>
            /// The message identifier.
            /// </value>
            [XmlElement(ElementName = "MessageID", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public string MessageID { get; set; }

            /// <summary>
            /// Gets or sets the relates to.
            /// </summary>
            /// <value>
            /// The relates to.
            /// </value>
            [XmlElement(ElementName = "RelatesTo", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public string RelatesTo { get; set; }

            /// <summary>
            /// Gets or sets the application sequence.
            /// </summary>
            /// <value>
            /// The application sequence.
            /// </value>
            [XmlElement(ElementName = "AppSequence", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
            public AppSequenceClass AppSequence { get; set; }

            /// <summary>
            /// The app sequence class.
            /// </summary>
            [XmlRoot(ElementName = "AppSequence", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
            public class AppSequenceClass
            {
                /// <summary>
                /// Gets or sets the message number.
                /// </summary>
                /// <value>
                /// The message number.
                /// </value>
                [XmlAttribute(AttributeName = "MessageNumber")]
                public string MessageNumber { get; set; }

                /// <summary>
                /// Gets or sets the instance identifier.
                /// </summary>
                /// <value>
                /// The instance identifier.
                /// </value>
                [XmlAttribute(AttributeName = "InstanceId")]
                public string InstanceId { get; set; }
            }
        }

        /// <summary>
        /// The body class
        /// </summary>
        [XmlRoot(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public class BodyClass
        {
            /// <summary>
            /// Gets or sets the probe matches.
            /// </summary>
            /// <value>
            /// The probe matches.
            /// </value>
            [XmlElement(ElementName = "ProbeMatches", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
            public ProbeMatchesClass ProbeMatches { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [XmlRoot(ElementName = "ProbeMatches", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
            public class ProbeMatchesClass
            {
                /// <summary>
                /// Gets or sets the probe match.
                /// </summary>
                /// <value>
                /// The probe match.
                /// </value>
                [XmlElement(ElementName = "ProbeMatch", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
                public ProbeMatchClass ProbeMatch { get; set; }

                /// <summary>
                /// The probe match class
                /// </summary>
                [XmlRoot(ElementName = "ProbeMatch", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
                public class ProbeMatchClass
                {
                    /// <summary>
                    /// Gets or sets the endpoint reference.
                    /// </summary>
                    /// <value>
                    /// The endpoint reference.
                    /// </value>
                    [XmlElement(ElementName = "EndpointReference", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
                    public EndpointReferenceClass EndpointReference { get; set; }

                    /// <summary>
                    /// Gets or sets the types.
                    /// </summary>
                    /// <value>
                    /// The types.
                    /// </value>
                    [XmlElement(ElementName = "Types", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
                    public string Types { get; set; }

                    /// <summary>
                    /// Gets or sets the scopes.
                    /// </summary>
                    /// <value>
                    /// The scopes.
                    /// </value>
                    [XmlElement(ElementName = "Scopes", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
                    public string Scopes { get; set; }

                    /// <summary>
                    /// Gets or sets the x addrs.
                    /// </summary>
                    /// <value>
                    /// The x addrs.
                    /// </value>
                    [XmlElement(ElementName = "XAddrs", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
                    public string XAddrs { get; set; }

                    /// <summary>
                    /// Gets or sets the metadata version.
                    /// </summary>
                    /// <value>
                    /// The metadata version.
                    /// </value>
                    [XmlElement(ElementName = "MetadataVersion", Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery")]
                    public string MetadataVersion { get; set; }

                    /// <summary>
                    /// The endpoint reference class.
                    /// </summary>
                    [XmlRoot(ElementName = "EndpointReference", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
                    public class EndpointReferenceClass
                    {
                        /// <summary>
                        /// Gets or sets the address.
                        /// </summary>
                        /// <value>
                        /// The address.
                        /// </value>
                        [XmlElement(ElementName = "Address", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
                        public string Address { get; set; }
                    }
                }
            }
        }
    }
}
