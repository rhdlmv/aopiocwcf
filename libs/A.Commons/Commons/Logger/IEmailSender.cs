namespace A.Commons.Logger
{
    using System;
    using System.Net.Mail;
    using System.Runtime.InteropServices;

    public interface IEmailSender
    {
        void Send(string subject, string body, string from, bool isBodyHTML, string[] to, Attachment[] attachements = null);
    }
}

