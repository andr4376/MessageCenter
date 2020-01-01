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
    /// Singleton class that handles message types, recipients, senders, attachments and sending of messages.
    /// </summary>
    public class MessageHandler
    {

        /// <summary>
        /// The customer receiving the message
        /// </summary>
        private Customer recipient;


        /// <summary>
        /// The employee sending the message
        /// </summary>
        private Employee sender;

        /// <summary>
        /// The chosen message template
        /// </summary>
        private MessageTemplate msgTemplate;

        /// <summary>
        ///A dictionary of message variables - Fx EMPLOYEE_FULLNAME -> [employeeFullName]
        /// </summary>
        private static Dictionary<MESSAGE_VARIABLES, string> messageVariables;

        /// <summary>
        /// The message itself - Is abstract and will become one of the inheriting classes fx Mail
        /// </summary>
        private Message message;

        /// <summary>
        /// List of attachments for the message 
        /// </summary>
        private List<MessageAttachment> attachments;

        /// <summary>
        /// CC adresses for mails 
        /// </summary>
        public string cCAdress = string.Empty;

        /// <summary>
        /// the "key" to accessing the attachments list
        /// </summary>
        public readonly object attachmentsKey = new object();

        /// <summary>
        /// Returns the list of attachments 
        /// </summary>
        public List<MessageAttachment> Attachments
        {
            get
            {
                return attachments;
            }
            private set
            {
                attachments = value;

            }
        }

        /// <summary>
        /// Get/Set Message
        /// </summary>
        public Message Msg
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// Get/set the recipient customer
        /// </summary>
        public Customer Recipient
        {
            get { return recipient; }
            set { recipient = value; }
        }

        /// <summary>
        /// Get/set the sender Employee
        /// </summary>
        public Employee Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        /// <summary>
        /// Get the message variable string value: fx EMPLOYEE_EMAIL -> [employeeEmail]
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetMessageVariable(MESSAGE_VARIABLES variable)
        {

            if (messageVariables == null)
            {
                SetupMessageVariables();
            }

            return messageVariables[variable];

        }

        /// <summary>
        /// Returns the Message variables dictionary
        /// </summary>
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


        /// <summary>
        /// Modified Singleton pattern that returns the client's own instance of the MessageHandler
        /// </summary>
        public static MessageHandler Instance
        {
            get
            {
                HttpSessionState session = HttpContext.Current.Session;
                if (session["MessageHandler"] == null)
                {
                    session["MessageHandler"] = new MessageHandler();
                }
                return (MessageHandler)session["MessageHandler"];

            }
        }




        /// <summary>
        /// Get/set for the messageHandler's MsgTemplate 
        /// </summary>
        public MessageTemplate MsgTemplate
        {
            get { return msgTemplate; }
            set
            {
                msgTemplate = value;

                //Automatically fetch all attachments when the msgTemplate is set
                if (msgTemplate != null)
                    FetchAttachmentsFromDb();

            }
        }




        /// <summary>
        /// Returns whether or not the Message is setup and ready to be send
        /// </summary>
        public bool IsReady
        {
            get { return (Sender != null && Recipient != null && MsgTemplate != null); }
        }

        /// <summary>
        /// Private constructor so the class can not be instanciated from outside this class
        /// </summary>
        private MessageHandler()
        {
        }

        /// <summary>
        /// Defines the message variables and their enum counterpart
        /// </summary>
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
                (Recipient == null ? "NULL" : Recipient.Id.ToString())

                + "\n" +
                "Message id: " +
                (MsgTemplate == null ? "NULL" : MsgTemplate.Id.ToString());

            return txt;
        }


        /// <summary>
        /// Resets the singleton instance and deletes all temporary files related to the messagehandler
        /// </summary>
        public static void Reset()
        {

            if (HttpContext.Current.Session["MessageHandler"] == null)
            {
                return;
            }

            FileManager.Instance.DeleteDirectory(
               (Instance.GetTempFilesPath()));

            HttpContext.Current.Session["MessageHandler"] = null;
        }

        /// <summary>
        /// Returns the the value of the input message variable- fx. CUSTOMER_FULLNAME -> "Jane Doe"
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public string GetValueFromMessageVariable(MESSAGE_VARIABLES variable)
        {
            string value = string.Empty;
            switch (variable)
            {
                case MESSAGE_VARIABLES.CUSTOMER_FULLNAME:
                    value = recipient.FullName;
                    break;

                case MESSAGE_VARIABLES.CUSTOMER_FIRSTNAME:
                    value = recipient.FirstName;

                    break;
                case MESSAGE_VARIABLES.CUSTOMER_LASTNAME:
                    value = recipient.LastName;
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_BIRTHDAY:
                    value = recipient.Birthday;
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_PHONENUMBER:
                    value = recipient.PhoneNumber;
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_EMAIL:
                    value = recipient.Email;
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_AGE:
                    value = recipient.Age.ToString();
                    break;
                case MESSAGE_VARIABLES.CUSTOMER_CPR:
                    value = recipient.Cpr;
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

        /// <summary>
        /// Sets a blank message handler - used for creating new message templates
        /// </summary>
        /// <param name="type"></param>
        public void SetBlankMessage(int type)
        {
            //An empty message template with the given type (is converted to enum)
            MsgTemplate = new MessageTemplate(type);

            //For path naming if User adds attached files to the new template
            Sender = SignIn.Instance.User;

            //Normally set when picking a message template.
            Attachments = new List<MessageAttachment>();

        }

        /// <summary>
        /// Inserts Customer and employee data into the message template and all of its valid attachments
        /// </summary>
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
                { IsBackground = true }
                .Start();



            }

        }

        /// <summary>
        /// Edit all attachments for the input message handler.
        /// </summary>
        /// <param name="messageHandler"></param>
        private void AttachmentsInsertData(MessageHandler messageHandler)
        {
            //no other processes are allowed to interact with the files while they're being editted.
            lock (messageHandler.attachmentsKey)
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
                            "Error - attempt to edit attachment: " + attachment.FileName + " failed! \nmError message: \n" + e.ToString());

                    }

                }
            }
        }

        /// <summary>
        /// Replace the main text's message variables with values.
        /// </summary>
        private void ReplaceMainText()
        {
            //for each variables defines
            foreach (KeyValuePair<MESSAGE_VARIABLES, string> variable in messageVariables)
            {
                //fx CUSTOMER_FULL_NAME -> "Jane Doe"
                string value = GetValueFromMessageVariable(variable.Key);

                if (value == string.Empty)
                {
                    continue;
                }

                //Fx Replace [customerFullName] with "Jane Doe"
                MsgTemplate.Text = MsgTemplate.Text.Replace(
                    GetMessageVariable(variable.Key), value);

            }
        }

        /// <summary>
        /// Instantiates the messagehandler's "message variable" based on the type of the messagetemplate.
        /// </summary>
        public void SetupMessage()
        {

            switch (MsgTemplate.MessageType)
            {
                case MessageType.MAIL:
                    Msg = new Mail(
                 Sender.Email,
                 Recipient.Email,
                 MsgTemplate.Title,
                 MsgTemplate.Text,
                 cCAdress);
                    break;

                //TODO:
                case MessageType.SMS:
                    Msg = new Sms(
                        Sender.PhoneNumber,
                        Recipient.PhoneNumber,
                        MsgTemplate.Text);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Adds the attachments to the message (fx. mails)
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<StatusCode, string> AddAttachmentsToMessage()
        {
            StatusCode status = StatusCode.OK;
            string description = string.Empty;

            if (Msg != null && Msg is Mail)//currently only mails can have attachments
            {

                lock (attachmentsKey) // waits here if the attachments are currently in use by another thread
                {
                    foreach (MessageAttachment attachment in Attachments)
                    {
                        if (attachment.FileType == "docx")
                        {
                            //converts word documents to PDF, before sending them                            
                            status =
                            attachment.ConvertDocToPDF();


                            if (status != StatusCode.OK)
                            {
                                description = "Der opstod fejl ved behandling af den vedhæftede fil '" + attachment.FileName + "', og besked blev derfor ikke afsendt";
                                break; //exit loop and return report

                            }

                        }

                        ((Mail)Msg).AttachFile(attachment);

                    }
                }

            }
            return new KeyValuePair<StatusCode, string>(status, description);
        }

        /// <summary>
        /// Sends the message, and returns a report descibing its success
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<StatusCode, string> SendMessage()
        {

            string description = string.Empty;


            //Add attachments (if supported by the messagetype)
            KeyValuePair<StatusCode, string> report =
            AddAttachmentsToMessage();

            if (report.Key != StatusCode.OK)
            {
                Msg.Reset(); //Removes attachments from message ect. 
                return report; //Return what went wrong
            }

            //Attempt to send message
            report =
              Msg.Send();

            if (report.Key != StatusCode.OK)//If something went wrong
            {
                description = "Besked kunne ikke afsendes";

                description += " " + report.Value;//add what went wrong to description

            }

            //Log attempt
            LogSentMessage(report.Key);


            //Clean up temp files and remove Singleton object reference
            if (report.Key != StatusCode.FORHINDRING) //Stay on page if "Forhindring" - the complications can be fixed by user
                Msg.Reset(); Reset();


            return new KeyValuePair<StatusCode, string>(report.Key, description);
        }

        /// <summary>
        /// Store information about the currently sent message (or attempt at sending)
        /// </summary>
        /// <param name="status"></param>
        private void LogSentMessage(StatusCode status)
        {
            string ricipientAdresse = msgTemplate.MessageType == MessageType.MAIL ? recipient.Email : recipient.PhoneNumber;

            DatabaseManager.Instance.LogSentMessage(
                msgTemplate.Id,     //the selected message tmplate
                status,             //was it sent correctly?
                sender.Tuser,       //employee who sent the message
                recipient.Cpr,      //the selected customer
                ricipientAdresse,   //the adresse which the message was sent to
                msgTemplate.Title,  //the title of the message (might have been modified by employee)
                msgTemplate.Text);  //the text (might have been modified by employee)
        }


        /// <summary>
        /// Retrives all attachments related to the current message template
        /// </summary>
        /// <returns></returns>
        public List<MessageAttachment> FetchAttachmentsFromDb()
        {
            if (MsgTemplate.Id == null)
            {
                return null;
            }

            Utility.WriteLog("Getting all attachments for messageTemplate id " + MsgTemplate.Id);

            //Get attachments that are related to this message template's Id
            this.Attachments = DatabaseManager.Instance.GetAttachmentsFromMessageId(MsgTemplate.Id);

            if (Attachments == null)
            {
                //Error
                Utility.PrintWarningMessage("Der opstod fejl ved udhentning af beskedens vedhæftede filer fra databasen." +
                    " Programmet kunne ikke identificere beskedskabelonen - kontakt venligt teknisk support:" +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));
                return null;

            }

            Utility.WriteLog(Attachments.Count + " attachments found for messageTemplate with id: " + MsgTemplate.Id);


            // Create temprorary files for each newly fetched attachments
            foreach (MessageAttachment attachment in Attachments)
            {
                StatusCode createFilesStatus =
                    attachment.CreateTempFile(); //Store file in app_data/temp/.../

                if (createFilesStatus == StatusCode.ERROR)
                {
                    return null;
                }
            }

            return Attachments;
        }

        /// <summary>
        /// Returns a unique path for this message's temprorary folder based on the MessageTemplate and the signed in employee.
        /// </summary>
        /// <returns></returns>
        public string GetTempFilesPath()
        {
            string path = string.Empty;

            if (msgTemplate != null && sender != null)
            {
                path = FileManager.Instance.GetTempDirectory(msgTemplate, sender.Tuser);

            }

            return path;
        }

        /// <summary>
        /// Removes an attachment from the message
        /// </summary>
        /// <param name="index">The index of the attachment</param>
        public void RemoveAttachment(int index)
        {
            //Listbox attachments and attachments list is 1:1

            if (Attachments.Count - 1 < index)
            {
                Utility.WriteLog("MessageHandler.RemoveAttachment() was called with an index that is out of bounds!: Count: "
                    + Attachments.Count + " input index: " + index);
                return;
            }


            Utility.WriteLog("Removing attached file: '" + Attachments[index].FileName + "'");

            Attachments[index].RemoveTempFile();
            Attachments.Remove(Attachments[index]);
        }

        /// <summary>
        /// Removes an attachment from the message
        /// </summary>
        /// <param name="index">The index of the attachment</param>
        public void RemoveAttachment(string fileName)
        {
            Utility.WriteLog("Removing attached file: '" + fileName + "'");

            foreach (MessageAttachment attachment in Attachments)
            {
                if (attachment.FileName == fileName)
                {
                    Attachments.Remove(attachment);
                    return;
                }

            }

        }

        /// <summary>
        /// Handles adding attachments to the message and ensures unique namings of file names
        /// </summary>
        /// <param name="newAttachment">the MessageAttachment you wish to add</param>
        /// <returns></returns>
        public MessageAttachment AddAttachment(MessageAttachment newAttachment)
        {

            foreach (MessageAttachment attachment in Attachments)
            {
                //If an existing attachment shares name with the new attachment
                if (attachment.FileName == newAttachment.FileName)
                {
                    string newFileName;
                    try
                    {
                        Utility.WriteLog("Another file with the filename '" + newAttachment.FileName + "' already exists! generating new name...");


                        //Anything before ".(file type)" Fx. "Test"
                        string fileName = newAttachment.FileName.Replace("." + newAttachment.FileType, "");

                        //The file type Fx. "png"
                        string fileType = newAttachment.FileType;

                        //Fx. Test_.png
                        newFileName = fileName + "_." + fileType;


                    }
                    catch (Exception e)
                    {

                        Utility.WriteLog("ERROR in MessageHandler.AddAttachments!: " + e.ToString());

                        //If anything should go wrong somehow, just create a unique name
                        newFileName = "attachment" + DateTime.Now.Millisecond + "." + newAttachment.FileType;


                    }
                    //Update the attachments file name
                    newAttachment.FileName = newFileName;

                    Utility.WriteLog("Conflicting file has been renamed to '" + newAttachment.FileName + "'");

                }
            }

            //Add attachment
            Attachments.Add(newAttachment);

            Utility.WriteLog("'" + newAttachment.FileName + "' has succesfully been added to the Message");

            return newAttachment;
        }
    }
}