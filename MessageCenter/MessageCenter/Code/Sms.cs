using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    public class Sms : Message
    {
        private string sender;
        private string receiver;

        private string text;


        public Sms(string sender, string receiver, string text)
        {
            this.sender = sender;
            this.receiver = receiver;
            this.text = text;
        }
               
        public override KeyValuePair<StatusCode, string> Send()
        {

            return new KeyValuePair<StatusCode, string>(StatusCode.ERROR, "Sms'er er desværre ikke understøttet i denne prototype");
        }

    }
}