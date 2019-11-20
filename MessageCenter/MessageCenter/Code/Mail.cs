using System;
using System.Net.Mail;

namespace MessageCenter.Code
{
    public class Mail : Message
    {

        private MailMessage mailMessage = new MailMessage();
        private SmtpClient smtpClient;

        public Mail(string from, string to, string title, string text)
        {
            mailMessage = new MailMessage();
            smtpClient = new SmtpClient("smtp.gmail.com");
            mailMessage.From = new MailAddress(from);

            mailMessage.To.Add(to);
            mailMessage.Subject = title;
            mailMessage.Body = text;
            mailMessage.IsBodyHtml = true; //TODO: sikre at de ikke giver problemer

            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("sparkronmessagecenter@gmail.com", "sparkronmc");
            smtpClient.EnableSsl = true;
        }

        public override void Send()
        {
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception exception)
            {
                Utility.WriteLog(exception.ToString());
                Utility.PrintWarningMessage(exception.ToString());
            }
        }
    }
}