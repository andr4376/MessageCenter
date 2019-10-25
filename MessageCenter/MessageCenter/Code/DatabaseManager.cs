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

            messages = new List<MessageTemplate>() { new MessageTemplate("en"), new MessageTemplate("dhnk,jfsjksknabdkjsahdkjashdjksahkjdhkjsahdjksahdkjashdasddhjkfsddsf"), new MessageTemplate("fdsa35f51dsa"), new MessageTemplate("FIRE"), };

            return 0;
        }

        public List<MessageTemplate> GetMessageTemplates()
        {
            return messages;
        }

        
    }
}