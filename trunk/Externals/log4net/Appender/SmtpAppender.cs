#region Copyright & License

//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using log4net.Core;

namespace log4net.Appender
{
    /// <summary>
    /// Send an e-mail when a specific logging event occurs, typically on errors 
    /// or fatal errors.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The number of logging events delivered in this e-mail depend on
    /// the value of <see cref="BufferingAppenderSkeleton.BufferSize"/> option. The
    /// <see cref="SmtpAppender"/> keeps only the last
    /// <see cref="BufferingAppenderSkeleton.BufferSize"/> logging events in its 
    /// cyclic buffer. This keeps memory requirements at a reasonable level while 
    /// still delivering useful application context.
    /// </para>
    /// <note type="caution">
    /// Authentication and setting the server Port are only available on the MS .NET 1.1 runtime.
    /// For these features to be enabled you need to ensure that you are using a version of
    /// the log4net assembly that is built against the MS .NET 1.1 framework and that you are
    /// running the your application on the MS .NET 1.1 runtime. On all other platforms only sending
    /// unauthenticated messages to a server listening on port 25 (the default) is supported.
    /// </note>
    /// <para>
    /// Authentication is supported by setting the <see cref="Authentication"/> property to
    /// either <see cref="SmtpAuthentication.Basic"/> or <see cref="SmtpAuthentication.Ntlm"/>.
    /// If using <see cref="SmtpAuthentication.Basic"/> authentication then the <see cref="Username"/>
    /// and <see cref="Password"/> properties must also be set.
    /// </para>
    /// <para>
    /// To set the SMTP server port use the <see cref="Port"/> property. The default port is 25.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public class SmtpAppender : BufferingAppenderSkeleton
    {
        #region Public Instance Constructors

        #endregion // Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// The mode to use to authentication with the SMTP server
        /// </summary>
        /// <remarks>
        /// <note type="caution">Authentication is only available on the MS .NET 1.1 runtime.</note>
        /// <para>
        /// Valid Authentication mode values are: <see cref="SmtpAuthentication.None"/>, 
        /// <see cref="SmtpAuthentication.Basic"/>, and <see cref="SmtpAuthentication.Ntlm"/>. 
        /// The default value is <see cref="SmtpAuthentication.None"/>. When using 
        /// <see cref="SmtpAuthentication.Basic"/> you must specify the <see cref="Username"/> 
        /// and <see cref="Password"/> to use to authenticate.
        /// When using <see cref="SmtpAuthentication.Ntlm"/> the Windows credentials for the current
        /// thread, if impersonating, or the process will be used to authenticate. 
        /// </para>
        /// </remarks>
        public SmtpAuthentication Authentication
        {
            get { return m_authentication; }
            set { m_authentication = value; }
        }

        /// <summary>
        /// Gets or sets the e-mail address of the sender.
        /// </summary>
        /// <value>
        /// The e-mail address of the sender.
        /// </value>
        /// <remarks>
        /// <para>
        /// The e-mail address of the sender.
        /// </para>
        /// </remarks>
        public string From
        {
            get { return m_from; }
            set { m_from = value; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <remarks>
        /// Use the BufferingAppenderSkeleton Fix methods instead 
        /// </remarks>
        /// <remarks>
        /// <para>
        /// Obsolete property.
        /// </para>
        /// </remarks>
        [Obsolete("Use the BufferingAppenderSkeleton Fix methods")]
        public bool LocationInfo
        {
            get { return false; }
            set { ; }
        }

        /// <summary>
        /// The password to use to authenticate with the SMTP server
        /// </summary>
        /// <remarks>
        /// <note type="caution">Authentication is only available on the MS .NET 1.1 runtime.</note>
        /// <para>
        /// A <see cref="Username"/> and <see cref="Password"/> must be specified when 
        /// <see cref="Authentication"/> is set to <see cref="SmtpAuthentication.Basic"/>, 
        /// otherwise the password will be ignored. 
        /// </para>
        /// </remarks>
        public string Password
        {
            get { return m_password; }
            set { m_password = value; }
        }

        /// <summary>
        /// The port on which the SMTP server is listening
        /// </summary>
        /// <remarks>
        /// <note type="caution">Server Port is only available on the MS .NET 1.1 runtime.</note>
        /// <para>
        /// The port on which the SMTP server is listening. The default
        /// port is <c>25</c>. The Port can only be changed when running on
        /// the MS .NET 1.1 runtime.
        /// </para>
        /// </remarks>
        public int Port
        {
            get { return m_port; }
            set { m_port = value; }
        }

        /// <summary>
        /// Gets or sets the priority of the e-mail message
        /// </summary>
        /// <value>
        /// One of the <see cref="MailPriority"/> values.
        /// </value>
        /// <remarks>
        /// <para>
        /// Sets the priority of the e-mails generated by this
        /// appender. The default priority is <see cref="MailPriority.Normal"/>.
        /// </para>
        /// <para>
        /// If you are using this appender to report errors then
        /// you may want to set the priority to <see cref="MailPriority.High"/>.
        /// </para>
        /// </remarks>
        public MailPriority Priority
        {
            get { return m_mailPriority; }
            set { m_mailPriority = value; }
        }

        /// <summary>
        /// Gets or sets the name of the SMTP relay mail server to use to send 
        /// the e-mail messages.
        /// </summary>
        /// <value>
        /// The name of the e-mail relay server. If SmtpServer is not set, the 
        /// name of the local SMTP server is used.
        /// </value>
        /// <remarks>
        /// <para>
        /// The name of the e-mail relay server. If SmtpServer is not set, the 
        /// name of the local SMTP server is used.
        /// </para>
        /// </remarks>
        public string SmtpHost
        {
            get { return m_smtpHost; }
            set { m_smtpHost = value; }
        }

        /// <summary>
        /// Gets or sets the subject line of the e-mail message.
        /// </summary>
        /// <value>
        /// The subject line of the e-mail message.
        /// </value>
        /// <remarks>
        /// <para>
        /// The subject line of the e-mail message.
        /// </para>
        /// </remarks>
        public string Subject
        {
            get { return m_subject; }
            set { m_subject = value; }
        }

        /// <summary>
        /// Gets or sets a semicolon-delimited list of recipient e-mail addresses.
        /// </summary>
        /// <value>
        /// A semicolon-delimited list of e-mail addresses.
        /// </value>
        /// <remarks>
        /// <para>
        /// A semicolon-delimited list of recipient e-mail addresses.
        /// </para>
        /// </remarks>
        public string To
        {
            get { return m_to; }
            set { m_to = value; }
        }

        /// <summary>
        /// The username to use to authenticate with the SMTP server
        /// </summary>
        /// <remarks>
        /// <note type="caution">Authentication is only available on the MS .NET 1.1 runtime.</note>
        /// <para>
        /// A <see cref="Username"/> and <see cref="Password"/> must be specified when 
        /// <see cref="Authentication"/> is set to <see cref="SmtpAuthentication.Basic"/>, 
        /// otherwise the username will be ignored. 
        /// </para>
        /// </remarks>
        public string Username
        {
            get { return m_username; }
            set { m_username = value; }
        }

        #endregion // Public Instance Properties

        #region Override implementation of BufferingAppenderSkeleton

        /// <summary>
        /// Sends the contents of the cyclic buffer as an e-mail message.
        /// </summary>
        /// <param name="events">The logging events to send.</param>
        protected override void SendBuffer(LoggingEvent[] events)
        {
            // Note: this code already owns the monitor for this
            // appender. This frees us from needing to synchronize again.
            try
            {
                var writer = new StringWriter(CultureInfo.InvariantCulture);

                var t = Layout.Header;
                if (t != null)
                    writer.Write(t);

                for (var i = 0; i < events.Length; i++)
                {
                    // Render the event and append the text to the buffer
                    RenderLoggingEvent(writer, events[i]);
                }

                t = Layout.Footer;
                if (t != null)
                    writer.Write(t);

                SendEmail(writer.ToString());
            }
            catch (Exception e)
            {
                ErrorHandler.Error("Error occurred while sending e-mail notification.", e);
            }
        }

        #endregion // Override implementation of BufferingAppenderSkeleton

        #region Override implementation of AppenderSkeleton

        /// <summary>
        /// This appender requires a <see cref="Layout"/> to be set.
        /// </summary>
        /// <value><c>true</c></value>
        /// <remarks>
        /// <para>
        /// This appender requires a <see cref="Layout"/> to be set.
        /// </para>
        /// </remarks>
        protected override bool RequiresLayout
        {
            get { return true; }
        }

        #endregion // Override implementation of AppenderSkeleton

        #region Protected Methods

        /// <summary>
        /// Send the email message
        /// </summary>
        /// <param name="messageBody">the body text to include in the mail</param>
        protected virtual void SendEmail(string messageBody)
        {
            // .NET 2.0 has a new API for SMTP email System.Net.Mail
            // This API supports credentials and multiple hosts correctly.
            // The old API is deprecated.

            // Create and configure the smtp client
            var smtpClient = new SmtpClient();
            if (m_smtpHost != null && m_smtpHost.Length > 0)
                smtpClient.Host = m_smtpHost;
            smtpClient.Port = m_port;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            if (m_authentication == SmtpAuthentication.Basic)
            {
                // Perform basic authentication
                smtpClient.Credentials = new NetworkCredential(m_username, m_password);
            }
            else if (m_authentication == SmtpAuthentication.Ntlm)
            {
                // Perform integrated authentication (NTLM)
                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
            }

            var mailMessage = new MailMessage();
            mailMessage.Body = messageBody;
            mailMessage.From = new MailAddress(m_from);
            mailMessage.To.Add(m_to);
            mailMessage.Subject = m_subject;
            mailMessage.Priority = m_mailPriority;

            // TODO: Consider using SendAsync to send the message without blocking. This would be a change in
            // behaviour compared to .NET 1.x. We would need a SendCompletedCallback to log errors.
            smtpClient.Send(mailMessage);
        }

        #endregion // Protected Methods

        #region Private Instance Fields

        // authentication fields
        SmtpAuthentication m_authentication = SmtpAuthentication.None;
        string m_from;
        MailPriority m_mailPriority = MailPriority.Normal;
        string m_password;

        // server port, default port 25
        int m_port = 25;
        string m_smtpHost;
        string m_subject;
        string m_to;
        string m_username;

        #endregion // Private Instance Fields

        #region SmtpAuthentication Enum

        /// <summary>
        /// Values for the <see cref="SmtpAppender.Authentication"/> property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// SMTP authentication modes.
        /// </para>
        /// </remarks>
        public enum SmtpAuthentication
        {
            /// <summary>
            /// No authentication
            /// </summary>
            None,

            /// <summary>
            /// Basic authentication.
            /// </summary>
            /// <remarks>
            /// Requires a username and password to be supplied
            /// </remarks>
            Basic,

            /// <summary>
            /// Integrated authentication
            /// </summary>
            /// <remarks>
            /// Uses the Windows credentials from the current thread or process to authenticate.
            /// </remarks>
            Ntlm
        }

        #endregion // SmtpAuthentication Enum
    }
}