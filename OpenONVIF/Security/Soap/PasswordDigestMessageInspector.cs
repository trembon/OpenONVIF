using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenONVIF.Security.Soap
{
    internal class PasswordDigestMessageInspector : IClientMessageInspector
    {
        #region Properties
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordDigestMessageInspector"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public PasswordDigestMessageInspector(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
        #endregion

        #region IClientMessageInspector Members
        /// <summary>
        /// Afters the receive reply.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <param name="correlationState">State of the correlation.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        /// <summary>
        /// Befores the send request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // Serialize the token to XML
            XmlElement securityToken = GenerateSecurityHeaderXml(this.Username, this.Password);

            // add the XML to the header
            MessageHeader securityHeader = MessageHeader.CreateHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", securityToken, false);
            request.Headers.Add(securityHeader);

            // complete
            return Convert.DBNull;
        }
        #endregion

        private static XmlElement GenerateSecurityHeaderXml(string username, string password)
        {
            XmlDocument document = new XmlDocument();
            
            string id = string.Concat("SecurityToken-", Guid.NewGuid().ToString());
            DateTime created = DateTime.UtcNow;
            byte[] nonce = new byte[16];
            RandomNumberGenerator.Create().GetBytes(nonce);

            XmlElement rootElement = document.CreateElement("wsse", "UsernameToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            rootElement.SetAttribute("xmlns:wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            rootElement.SetAttribute("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", id);

            XmlElement usernameElement = document.CreateElement("wsse", "Username", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            usernameElement.InnerText = username;
            rootElement.AppendChild(usernameElement);
            
            byte[] passwordDigest = ComputePasswordDigest(nonce.Clone() as byte[], created, password);
            XmlElement passwordElement = document.CreateElement("wsse", "Password", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            passwordElement.SetAttribute("Type", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest");
            passwordElement.InnerText = Convert.ToBase64String(passwordDigest);
            rootElement.AppendChild(passwordElement);
            
            XmlElement nonceElement = document.CreateElement("wsse", "Nonce", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            nonceElement.InnerText = Convert.ToBase64String(nonce);
            rootElement.AppendChild(nonceElement);

            XmlElement createdElement = document.CreateElement("wsu", "Created", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            createdElement.InnerText = XmlConvert.ToString(created, "yyyy-MM-ddTHH:mm:ssZ");
            rootElement.AppendChild(createdElement);
            
            return rootElement;
        }

        private static byte[] ComputePasswordDigest(byte[] nonce, DateTime created, string secret)
        {
            if (nonce == null || (int)nonce.Length == 0)
                throw new ArgumentNullException("nonce");

            if (secret == null)
                throw new ArgumentNullException("secret");

            byte[] bytes = Encoding.UTF8.GetBytes(XmlConvert.ToString(created.ToUniversalTime(), "yyyy-MM-ddTHH:mm:ssZ"));
            byte[] numArray = Encoding.UTF8.GetBytes(secret);
            byte[] numArray1 = new byte[(int)nonce.Length + (int)bytes.Length + (int)numArray.Length];
            Array.Copy(nonce, numArray1, (int)nonce.Length);
            Array.Copy(bytes, 0, numArray1, (int)nonce.Length, (int)bytes.Length);
            Array.Copy(numArray, 0, numArray1, (int)nonce.Length + (int)bytes.Length, (int)numArray.Length);
            
            return SHA1.Create().ComputeHash(numArray1);
        }
    }
}
