﻿using MessageCenter.Models;
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

        public MessageHandler(Customer _receiver, Employee _sender, MessageTemplate _message)
        {
            this.receiver = _receiver;
            this.sender = _sender;
            this.message = _message;
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





    }
}