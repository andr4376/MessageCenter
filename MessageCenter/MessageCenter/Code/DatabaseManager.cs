using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace MessageCenter.Code
{
    public class DatabaseManager
    {

        private static DatabaseManager instance;

        public List<MessageTemplate> messages;

        private static string dbPath = "C:\\MessageCenter\\Database\\Database.db";

        private static string messageTemplatesTableName = "MessageTemplates";

        private SQLiteConnection DBConnect;

        private static string supportEmail = "andr4376@gmail.com";



        public static DatabaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseManager();
                }
                return instance;

            }
        }

        private DatabaseManager()
        {
                                 
            if (Initialize() != ReturnCode.OK)
            {
                //ERROR 
                System.Diagnostics.Debug.WriteLine("Failed to initialize DatabaseManager");

            }
         
        }

        private ReturnCode Initialize()
        {
            System.Diagnostics.Debug.WriteLine("Initializing DatabaseManager");

            ReturnCode returnCode = ReturnCode.OK;

            DBConnect = new SQLiteConnection("Data source = " + dbPath + "; Version = 3; ");

            returnCode = CreateDbFileIfNotExists();

            returnCode = CreateTablesIfNotExists();


            return returnCode;
        }

        private ReturnCode CreateDbFileIfNotExists()
        {

            if (CreateDbDirectories() != ReturnCode.OK)
            {
                return ReturnCode.ERROR;
            }


            if (CreateDbFile() != ReturnCode.OK)
            {
                return ReturnCode.ERROR;
            }


            return ReturnCode.OK;
        }


        private ReturnCode CreateDbDirectories()
        {
            try
            {
                if (!(Directory.Exists("C:\\MessageCenter")))
                {
                    Directory.CreateDirectory("C:\\MessageCenter");
                }

                if (!(Directory.Exists("C:\\MessageCenter\\Database")))
                {
                    Directory.CreateDirectory("C:\\MessageCenter\\Database");
                    System.Diagnostics.Debug.WriteLine("Db directories created");

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Db directories found");

                }

            }
            catch (System.Exception e)
            {

                System.Diagnostics.Debug.WriteLine(e.Message);
                string directoryPath = dbPath;
                directoryPath.Replace("\\Database.db", "");

                Utility.WriteWarningMessage("Oops! Programmet kunne ikke oprette stien til database filen ('"
                    + directoryPath +
                    "'). Programmet blev enten nægtet adgang, eller stien er ugyldig" +
                    "\nKontakt venligst teknisk support på følgende mail: " + supportEmail);
                return ReturnCode.ERROR;
            }

            return ReturnCode.OK;
        }

        private ReturnCode CreateDbFile()
        {
            try
            {
                if (!(File.Exists(dbPath)))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    Utility.WriteWarningMessage("Programmet opretter en ny database - Hvis der burdte findes en database i forvejen, findes den ikke på følgende sti: " + dbPath + "." +
                    "\nKontakt venligst teknisk support på følgende mail: " + supportEmail);
                    System.Diagnostics.Debug.WriteLine("Db file created @" + dbPath);

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Db file found @"+dbPath);
                }
            }
            catch (System.Exception e)
            {

                System.Diagnostics.Debug.WriteLine(e.Message);

                Utility.WriteWarningMessage("Oops! Programmet kunne ikke oprette database filen - Hvis der burdte findes en database i forvejen, findes den ikke på følgende sti: " + dbPath + "." +
                    "\nKontakt venligst teknisk support på følgende mail: " + supportEmail);

                return ReturnCode.ERROR;
            }

            return ReturnCode.OK;
        }

        private ReturnCode CreateTablesIfNotExists()
        {
            string createEntriesTable = "Create table IF NOT EXISTS " + messageTemplatesTableName + "" +
                "(id integer primary key, title varchar, text varchar, date varchar)";

            return ExecuteSQLiteNonQuery(createEntriesTable);

        }

        public ReturnCode ExecuteSQLiteNonQuery(string command)
        {
            ReturnCode returnCode = ReturnCode.OK;

            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand(command, DBConnect);

            try
            {
                Command.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error in executing SQLiteNonQuery! Error messages: \n" + e.Message);
                returnCode = ReturnCode.ERROR;
            }
            DBConnect.Close();

            return returnCode;
        }



        public List<MessageTemplate> GetMessageTemplates()
        {
            return messages;
        }

        public Dictionary<string, string> GetMessageTemplatesDictionaryTitleId()
        {
            if (messages == null || messages.Count == 0)
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



/*
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
             */
