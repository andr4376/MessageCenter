using System;
using System.Net.Mail;

namespace MessageCenter.Code
{
    public class Mail : Message
    {

        private MailMessage mailMessage = new MailMessage();
        private SmtpClient smtpClient;


        //Prototype: Jeg bruger Gmail, og en google konto til at sende mails 
        private readonly string smtpHost = "smtp.gmail.com";
        private readonly string mailCredentialUsername = "sparkronmessagecenter@gmail.com";
        private readonly string mailCredentialPassword = "sparkronmc";
        private readonly int smtpPort = 587;
        //

        public Mail(string from, string to, string title, string text)
        {
            mailMessage = new MailMessage();
            smtpClient = new SmtpClient(smtpHost);
            mailMessage.From = new MailAddress(from);

            mailMessage.To.Add(to);
            mailMessage.Subject = title;


            mailMessage.Body = text;
            //TODO: add rich text editing
            //   mailMessage.Body = ConvertTextToHtml(text);

            //TODO: man kan lave rich text, men \n bliver ikke til linjeskift...
            // mailMessage.IsBodyHtml = true;

            smtpClient.Port = smtpPort;
            smtpClient.Credentials = new System.Net.NetworkCredential(mailCredentialUsername, mailCredentialPassword);
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
                Utility.WriteLog("Der opstod fejl ved at sende email: "+exception.ToString());
                Utility.PrintWarningMessage("Der opstod fejl ved at sende email: " + exception.ToString()+" - kontakt venligt teknisk support");
            }

            mailMessage.Attachments.Clear();
        }

        public void AttachFile(MessageAttachment messageAttachment)
        {
            if (messageAttachment == null)
            {
                return;
            }
            this.mailMessage.Attachments.Add(new Attachment(messageAttachment.FilePath));
        }
    }
}