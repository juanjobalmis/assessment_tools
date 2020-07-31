using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentTools.Utilities
{
    using System;
    using System.Net;
    using System.Net.Mail;

    namespace EnviarCorreoElectronico
    {
        public class MailSender : IDisposable
        {            
            private SmtpServerCredentials Configuration { get; set; }
            private MailMessage Email { get; set; }
            private SmtpClient Client { get; set; }

            public MailSender(SmtpServerCredentials Configuration)
            {
                this.Configuration = Configuration;
                Client = new SmtpClient(Configuration.Host, Configuration.Port)
                {
                    EnableSsl = Configuration.EnableSSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Configuration.User, Configuration.PassWord)
                };
            }
            public void Send(string addressee, string subject, string message, bool isHtml = true)
            {
                using (Email = new MailMessage(Configuration.User, addressee, subject, message))
                {
                    Email.IsBodyHtml = isHtml;
                    Client.Send(Email);
                }
                Email = null;
            }
            public void Dispose()
            {
                Client.Dispose();
                if (Email != null)
                    Email.Dispose();
            }
        }
    }
}
