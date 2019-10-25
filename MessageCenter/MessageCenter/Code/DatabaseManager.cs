using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    public class DatabaseManager
    {

        private static DatabaseManager instance;

        public List<MessageTemplate> messages;

        public static DatabaseManager Instance
        {
            get {
                if (instance == null)
                {
                    instance = new DatabaseManager();
                }
                  return instance;

            }
        }

        private DatabaseManager()
        {
            if (Initialize() != 0)
            {
                //ERROR 
                System.Diagnostics.Debug.WriteLine("Failed to initialize DatabaseManager");
            }

        }

        private int Initialize()
        {
            System.Diagnostics.Debug.WriteLine("Initializing DatabaseManager");

            messages = new List<MessageTemplate>() { new MessageTemplate("en"),
                new MessageTemplate("dhnk,jfsjksknabdkjsahdkjashdjksahkjdhkjsahdjksahdkjashdasddhjkfsddsf"),
                new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FIRE"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FIRE"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("sd"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FIRE"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FsadasdIRE"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FIRE"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FIasRE"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FIRE"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("asdasdasssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FIRE"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FIRE"),
                 new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("asdsadasdasdasdasdas"),
             new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FdasdasdasdsadasdsadsaIRE")};

            return 0;
        }

        public List<MessageTemplate> GetMessageTemplates()
        {
            return messages;
        }

        public Dictionary<string,string> GetMessageTemplatesDictionaryTitleId()
        {
            if (messages.Count==0)
            {
                return null;
            }

            Dictionary<string, string> messagesDictionary = new Dictionary<string, string>();

            foreach (MessageTemplate message in messages)
            {
                messagesDictionary.Add(message.id.ToString(), message.title);

            }
            return messagesDictionary;
        }


    }
}