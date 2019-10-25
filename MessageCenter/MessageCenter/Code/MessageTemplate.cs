using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    public class MessageTemplate
    {

        public static int idS=0;
        public  int id;

        public string title;


        public MessageTemplate(string title)
        {
            this.title = title;
            this.id = idS;
            idS++;
        }

    }
}