namespace EgoalTech.Commons.Logger
{
    using EgoalTech.Commons.Exception;
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class DefaultEmailSender : IEmailSender
    {
        public void Send(string subject, string body, string from, bool isBodyHTML, string[] to, Attachment[] attachements = null)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient(this.Host, this.Port) {
                    Credentials = new NetworkCredential(this.UserName, this.Password),
                    EnableSsl = this.EnableSsl
                };
                message.IsBodyHtml = isBodyHTML;
                message.From = new MailAddress(from);
                foreach (string str in to)
                {
                    message.To.Add(str);
                }
                message.Subject = subject;
                message.Body = body;
                if (attachements != null)
                {
                    foreach (Attachment attachment in attachements)
                    {
                        message.Attachments.Add(attachment);
                    }
                }
                client.Send(message);
            }
            catch (Exception exception)
            {
                throw new FailedToSendEmailException($"Failed to send email from {from} to {string.Join(",", to)}", exception);
            }
        }

        public bool EnableSsl { get; set; }

        public string Host { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }
    }
}

