using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace MessageCenter.Code
{
    public enum MESSAGE_VARIABLES
    {
        CUSTOMER_FULLNAME,
        CUSTOMER_FIRSTNAME,
        CUSTOMER_LASTNAME,
        CUSTOMER_BIRTHDAY,
        CUSTOMER_PHONENUMBER,
        CUSTOMER_EMAIL,
        CUSTOMER_AGE,
        CUSTOMER_CPR,
        DEPARTMENT,
        EMPLOYEE_FULLNAME,
        EMPLOYEE_FIRSTNAME,
        EMPLOYEE_LASTNAME,
        EMPLOYEE_PHONENUMBER,
        EMPLOYEE_EMAIL
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageHandler
    {
        private Customer receiver;

        private Employee sender;

        private MessageTemplate msgTemplate;

        private static Dictionary<MESSAGE_VARIABLES, string> messageVariables;

        private Message message;

        private List<MessageAttachment> attachments;

        public string cCAdress = string.Empty;

        /// <summary>
        /// the "key" to accessing the attachments list
        /// </summary>
        public static readonly object attachmentsKey = new object();

        /// <summary>
        /// Returns the list of attachments that is locked so that only one thread can access it at a time
        /// </summary>
        public List<MessageAttachment> Attachments
        {
            get
            {
                return Instance.attachments;
            }
            set
            {
                Instance.attachments = value;

            }
        }

        public Message Msg
        {
            get { return Instance.message; }
            set { Instance.message = value; }
        }

        public Customer Receiver
        {
            get { return Instance.receiver; }
            set { Instance.receiver = value; }
        }

        public Employee Sender
        {
            get { return Instance.sender; }
            set { Instance.sender = value; }
        }
        public static string GetMessageVariable(MESSAGE_VARIABLES variable)
        {

            if (messageVariables == null)
            {
                SetupMessageVariables();
            }

            return messageVariables[variable];

        }

        public static Dictionary<MESSAGE_VARIABLES, string> GetMessageVariables
        {
            get
            {
                if (messageVariables == null)
                {
                    SetupMessageVariables();
                }
                return messageVariables;
            }
        }


        public static MessageHandler Instance
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }

                HttpSessionState session = HttpContext.Current.Session;
                if (session["MessageHandler"] == null)
                {
                    session["MessageHandler"] = new MessageHandler();
                }
                return (MessageHandler)session["MessageHandler"];

            }
        }





        public MessageTemplate MsgTemplate
        {
            get { return Instance.msgTemplate; }
            set
            {
                Instance.msgTemplate = value;

                if (Instance.msgTemplate != null && Instance.msgTemplate.MessageType == MessageType.MAIL)
                {
                    GetAttachments();
                }
            }
        }

   

      

        public bool IsReady
        {
            get { return (Sender != null && Receiver != null && MsgTemplate != null); }
        }


        public MessageHandler()
        {


        }

        private static void SetupMessageVariables()
        {
            messageVariables = new Dictionary<MESSAGE_VARIABLES, string>()
            {
                {MESSAGE_VARIABLES.CUSTOMER_AGE,"[customerAge]" },
                {MESSAGE_VARIABLES.CUSTOMER_BIRTHDAY,"[customerBirthDay]" },
                {MESSAGE_VARIABLES.CUSTOMER_EMAIL,"[customerEmail]" },
                {MESSAGE_VARIABLES.CUSTOMER_FIRSTNAME,"[customerFirstName]" },
                {MESSAGE_VARIABLES.CUSTOMER_FULLNAME,"[customerFullName]" },
                {MESSAGE_VARIABLES.CUSTOMER_LASTNAME,"[customerLastName]" },
                {MESSAGE_VARIABLES.CUSTOMER_PHONENUMBER,"[customerPhoneNumber]" },
                {MESSAGE_VARIABLES.CUSTOMER_CPR,"[customerCpr]" },
                {MESSAGE_VARIABLES.DEPARTMENT,"[department]" },
                {MESSAGE_VARIABLES.EMPLOYEE_EMAIL,"[employeeEmail]" },
                {MESSAGE_VARIABLES.EMPLOYEE_FIRSTNAME,"[employeeFirstName]" },
                {MESSAGE_VARIABLES.EMPLOYEE_FULLNAME,"[employeeFullName]" },
                {MESSAGE_VARIABLES.EMPLOYEE_LASTNAME,"[employeeLastName]" },
                {MESSAGE_VARIABLES.EMPLOYEE_PHONENUMBER,"[employeePhoneNumber]" },

            };
        }

        public override string ToString()
        {

            string txt = "MessageHandler properties"
                + "\n" +
                "Sender TUser: " +
                (Sender == null ? "NULL" : Sender.Tuser)

                + "\n" +
                "Receiver ID: " +
                (Receiver == null ? "NULL" : Receiver.Id.ToString())

                + "\n" +
                "Message id: " +
                (MsgTemplate == null ? "NULL" : MsgTemplate.Id.ToString());

            return txt;
        }



        public static void Reset()
        {
            if (HttpContext.Current.Session["MessageHandler"] == null)
            {
                return;
            }

            FileManager.Instance.DeleteDirectory(
               ((MessageHandler)HttpContext.Current.Session["MessageHandler"]).GetTempFilesPath());

            HttpContext.Current.Session["MessageHandler"] = null;
        }

        public string GetValueFromMessageVariable(MESSAGE_VARIABLES variable)
        {
            string value = string.Empty;
            switch (variable)
            {
                case MESSAGE_VARIABLES.CUSTOMER_FULLNAME:
                    value = receiver.FullName;
                    break;

                case MESSAGE_VARIABLES.CUSTOMER_FIRSTNAME:
                    value = receiver.FirstName;

                    break;
                case MESSAGE_VARIABLES.CUSTOMER_LASTNAME:
                    value = receiver.LastName;
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_BIRTHDAY:
                    value = receiver.Birthday;
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_PHONENUMBER:
                    value = receiver.PhoneNumber;
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_EMAIL:
                    value = receiver.Email;
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_AGE:
                    value = receiver.Age.ToString();
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_CPR:
                    value = receiver.Cpr;
                    break;

                case MESSAGE_VARIABLES.DEPARTMENT:
                    value = sender.Department;
                    break;
                case MESSAGE_VARIABLES.EMPLOYEE_FULLNAME:
                    value = sender.FullName;
                    break;
                case MESSAGE_VARIABLES.EMPLOYEE_FIRSTNAME:
                    value = sender.FirstName;
                    break;
                case MESSAGE_VARIABLES.EMPLOYEE_LASTNAME:
                    value = sender.LastName;
                    break;
                case MESSAGE_VARIABLES.EMPLOYEE_PHONENUMBER:
                    value = sender.PhoneNumber;
                    break;
                case MESSAGE_VARIABLES.EMPLOYEE_EMAIL:
                    value = sender.Email;
                    break;
                default:
                    break;
            }

            return value;

        }

        public void SetBlankMessage(int type)
        {
            //An empty message template with the given type (is converted to enum)
            MsgTemplate = new MessageTemplate(type);

            //For path naming
            Sender = SignIn.Instance.User;

            Attachments = new List<MessageAttachment>();

        }

        public void FillMessageWithData()
        {
            if (messageVariables == null)
            {
                SetupMessageVariables();
            }

            //Replace the message text's variables
            ReplaceMainText();


            if (Attachments != null)
            {

                //The thread needs a reference to the Instance because the Singleton instance is retrieved with the 
                //current HttpContext which is not accessible from thread.
                //In other words - Singletons was a mistake
                MessageHandler instanceReference = Instance;

                //Edit the attachments asynchronously
                new Thread(
                    () => AttachmentsInsertData(instanceReference)) //Alternate paramterized thread
                { IsBackground = true } //In case it lingers, it gets removed on server restart
                .Start();

               

            }

        }

        private void AttachmentsInsertData(MessageHandler messageHandler)
        {
            lock (attachmentsKey)
            {
                foreach (MessageAttachment attachment in messageHandler.attachments)
                {
                    try
                    {
                        //Try to insert customer / employee data
                        attachment.EditAttachment(messageHandler);
                    }
                    catch (Exception e)
                    {
                        Utility.WriteLog(
                            "Error - attempt to edit attachment: "+attachment.FileName + " failed! \nmError message: \n"+e.ToString());
                        
                    }
                    
                }
            }
        }


        private void ReplaceMainText()
        {

            foreach (KeyValuePair<MESSAGE_VARIABLES, string> variable in messageVariables)
            {
                string value = GetValueFromMessageVariable(variable.Key);

                if (value == string.Empty)
                {
                    continue;
                }

                MsgTemplate.Text = MsgTemplate.Text.Replace(
                    GetMessageVariable(variable.Key), value);

            }
        }

        public void SetupMessage()
        {


            switch (MsgTemplate.MessageType)
            {
                case MessageType.MAIL:
                    Msg = new Mail(
                 Sender.Email,
                 Receiver.Email,
                 MsgTemplate.Title,
                 MsgTemplate.Text,
                 cCAdress);
                    break;

                //TODO:
                case MessageType.SMS:
                    Msg = new Sms(
                        Sender.PhoneNumber,
                        Receiver.PhoneNumber,
                        MsgTemplate.Text);
                    break;
                default:
                    break;
            }

        }

        public void AddAttachments()
        {
            if (Msg is Mail)
            {
                lock (attachmentsKey) // waits here if the attachments are currently in use by another thread
                {
                    foreach (MessageAttachment attachment in Attachments)
                    {
                        if (attachment.FileType == "docx")
                        {
                            attachment.ConvertDocToPDF();
                        }

                        ((Mail)Msg).AttachFile(attachment);

                    }
                }

            }
        }

        public void SendMessage()
        {

            if (this.MsgTemplate.MessageType == MessageType.MAIL)
            {
                AddAttachments();
            }

            Msg.Send();

            Reset();
        }


        public List<MessageAttachment> GetAttachments()
        {
            if (MsgTemplate.Id == null)
            {
                return null;
            }

            Utility.WriteLog("Getting all attachments for messageTemplate id " + MsgTemplate.Id);

            this.Attachments = DatabaseManager.Instance.GetAttachmentsFromMessageId(MsgTemplate.Id);

            if (Attachments == null)
            {
                Utility.PrintWarningMessage("Der opstod fejl ved udhentning af beskedens vedhæftede filer fra databasen." +
                    " Programmet kunne ikke identificere beskedskabelonen - kontakt venligt teknisk support:" +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));
                return null;

            }

            Utility.WriteLog(Attachments.Count + " attachments found for messageTemplate with id: " + MsgTemplate.Id);


            foreach (MessageAttachment attachment in Attachments)
            {
                StatusCode createFilesStatus = attachment.CreateTempFile();

                if (createFilesStatus == StatusCode.ERROR)
                {
                    return null;
                }
            }

            return Attachments;
        }

        public string GetTempFilesPath()
        {
            string path = string.Empty;

            if (msgTemplate != null && sender != null)
            {
                path = FileManager.Instance.GetTempDirectory(msgTemplate, sender.Tuser);

            }

            return path;
        }

        public void RemoveAttachment(int index)
        {
            lock (attachmentsKey)
            {
                Attachments[index].RemoveTempFile();
                Attachments.Remove(Attachments[index]);
            }

        }
    }
}