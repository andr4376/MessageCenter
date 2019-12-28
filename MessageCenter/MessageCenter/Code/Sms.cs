using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    public class Sms : Message
    {
        private string sender;
        private string recipient;

        private string text;


        public Sms(string sender, string recipient, string text)
        {
            this.sender = sender;
            this.recipient = recipient;
            this.text = text;
        }

        public override void Reset()
        {
           //SMS not implemented
        }

        public override KeyValuePair<StatusCode, string> Send()
        {

            return new KeyValuePair<StatusCode, string>(StatusCode.ERROR, " - Sms'er er desværre ikke understøttet i denne prototype");
        }

    }
}