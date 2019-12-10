using System;
using System.Collections.Generic;
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

        public Mail(string from, string to, string title, string text, string cc)
        {
            //The email message object
            mailMessage = new MailMessage();

            //the email client connects to the email service (gmail)
            smtpClient = new SmtpClient(smtpHost);

            //Setup email:
            mailMessage.From = new MailAddress(from);
            mailMessage.To.Add(to); //allows multiple adresses, seperated by ','

            if (cc != string.Empty)
            {
                mailMessage.CC.Add(cc);//allows multiple adresses, seperated by ','
            }
            mailMessage.Subject = title;
            mailMessage.Body = text;

            //TODO: add rich text editing
            //   mailMessage.Body = ConvertTextToHtml(text);

            //TODO: man kan lave rich text, men \n bliver ikke til linjeskift...
            // mailMessage.IsBodyHtml = true;

            smtpClient.Port = smtpPort;

            //Login to the mail service (sparkronmessagecenter@gmail.com's google login)
            smtpClient.Credentials =
                new System.Net.NetworkCredential(mailCredentialUsername, mailCredentialPassword);
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

        public override KeyValuePair<StatusCode, string> Send()
        {
            StatusCode sentStatus;
            string description = string.Empty;

            try
            {
                //Sends the Email
                smtpClient.Send(mailMessage);

                this.Reset();

                sentStatus = StatusCode.OK;


            }
            catch (Exception exception)
            {
                sentStatus = StatusCode.ERROR;
                Utility.WriteLog("Der opstod fejl ved at sende email: " + exception.ToString());

                description = "Der opstod en ukendt fejl da vi forsøgte at sende mailen! - send den nedenstående fejlbesked til teknisk support ("
                    + Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL) + "): \n" +
                    exception.ToString();
                
            }

            return new KeyValuePair<StatusCode, string>(sentStatus,description);
        }

        public void AttachFile(MessageAttachment messageAttachment)
        {
            if (messageAttachment == null)
            {
                return;
            }
            this.mailMessage.Attachments.Add(new Attachment(messageAttachment.FilePath));
        }

        public override void Reset()
        {
            //Dispose of attachments so that the files are not "in use"
            //Allows the applikation to immediately delete the temporary files (attachments)
            if (mailMessage != null)
            {
                mailMessage.Attachments.Dispose();
            }

        }
    }
}