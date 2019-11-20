using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        private static MessageHandler instance;

        private static Dictionary<MESSAGE_VARIABLES, string> messageVariables;

        private Message message;

        public static string GetMessageVariable(MESSAGE_VARIABLES variable)
        {

            if (messageVariables == null)
            {
                SetupMessageVariables();
            }

            return messageVariables[variable];

        }



        public static MessageHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageHandler();
                }
                return instance;
            }
        }




        public MessageTemplate MsgTemplate
        {
            get { return msgTemplate; }
            set { msgTemplate = value; }
        }

        public Employee Sender
        {
            get { return sender; }
            set { sender = value; }

        }

        public Customer Receiver
        {
            get { return receiver; }
            set { receiver = value; }

        }

        public bool IsReady
        {
            get { return (sender != null && receiver != null && msgTemplate != null); }
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
                (sender == null ? "NULL" : sender.Tuser)

                + "\n" +
                "Receiver ID: " +
                (receiver == null ? "NULL" : receiver.Id.ToString())

                + "\n" +
                "Message id: " +
                (msgTemplate == null ? "NULL" : msgTemplate.Id.ToString());

            return txt;
        }



        public static void Reset()
        {
            instance = null;
        }

        public void FillMessageWithData()
        {
            if (messageVariables == null)
            {
                SetupMessageVariables();
            }

            ReplaceMainText();
        }


        private void ReplaceMainText()
        {

            foreach (KeyValuePair<MESSAGE_VARIABLES, string> variable in messageVariables)
            {
                string tmpText = "";
                switch (variable.Key)
                {
                    case MESSAGE_VARIABLES.CUSTOMER_FULLNAME:
                        tmpText = receiver.FullName;
                        break;

                    case MESSAGE_VARIABLES.CUSTOMER_FIRSTNAME:
                        tmpText = receiver.FirstName;

                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_LASTNAME:
                        tmpText = receiver.LastName;
                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_BIRTHDAY:
                        tmpText = receiver.Birthday;
                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_PHONENUMBER:
                        tmpText = receiver.PhoneNumber;
                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_EMAIL:
                        tmpText = receiver.Email;
                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_AGE:
                        tmpText = receiver.Age.ToString();
                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_CPR:
                        tmpText = receiver.Cpr;
                        break;

                    case MESSAGE_VARIABLES.DEPARTMENT:
                        tmpText = sender.Department;
                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_FULLNAME:
                        tmpText = sender.FullName;
                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_FIRSTNAME:
                        tmpText = sender.FirstName;
                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_LASTNAME:
                        tmpText = sender.LastName;
                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_PHONENUMBER:
                        tmpText = sender.PhoneNumber;
                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_EMAIL:
                        tmpText = sender.Email;
                        break;
                    default:
                        break;
                }

                msgTemplate.Text = msgTemplate.Text.Replace(
                    GetMessageVariable(variable.Key), tmpText);

            }
        }

        public void SetupMessage()
        {


            switch (msgTemplate.MessageType)
            {
                case MessageType.MAIL:
                    this.message = new Mail(
                 sender.Email,
                 receiver.Email,
                 msgTemplate.Title,
                 msgTemplate.Text);
                    break;

                    //TODO:
                case MessageType.SMS:
                    Utility.PrintWarningMessage("sms ikke implementeret, prøv anden besked skabelon :)");
                    throw new Exception();
                    break;
                default:
                    break;
            }

        }

        public void AddAttachments()
        {
            //TODO:
        }

        public void SendMessage()
        {
            this.message.Send();
        }
    }
}