using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageHandler
    {
        private Customer receiver;

        private Employee sender;

        private MessageTemplate message;

        private static MessageHandler instance;

        

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

       


        public MessageTemplate Message
        {
            get { return message; }
            set { message = value; }
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
            get { return (sender != null && receiver != null && message != null); }
        }


        public MessageHandler()
        {



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
                (message == null ? "NULL" : message.Id.ToString());

            return txt;
        }



        public static void Reset()
        {
            instance = null;
        }

    }
}