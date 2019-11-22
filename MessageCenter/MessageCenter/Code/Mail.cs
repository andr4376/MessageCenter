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
            mailMessage.Body = ConvertTextToHtml(text);

            //TODO: man kan lave rich text, men \n bliver ikke til linjeskift...
          // mailMessage.IsBodyHtml = true;

            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("sparkronmessagecenter@gmail.com", "sparkronmc");
            smtpClient.EnableSsl = true;
        }

        /// <summary>
        /// TODO:
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ConvertTextToHtml(string text)
        {
            return text.Replace("\n", "<br>");
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