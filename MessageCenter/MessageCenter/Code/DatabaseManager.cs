using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MessageCenter.Code
{
    public class DatabaseManager
    {

        private static DatabaseManager instance;

        public List<MessageTemplate> messages;

        public static readonly string dbFileName = "Database.db";

        private static string messageTemplatesTableName = "MessageTemplates";

        private SQLiteConnection DBConnect;

        private readonly string supportEmail = "andr4376@gmail.com";



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
            if (Initialize() == ReturnCode.ERROR)
            {
                Utility.WriteLog("Failed to initialize DatabaseManager");

                return;
            }
            //TODO: load pages - see ConsoleEntries project - read from DB
            LoadMessageTemplates();

        }

        private ReturnCode LoadMessageTemplates()
        {
            ReturnCode returnCode = ReturnCode.OK;

            //TODO: db get all messages

            return returnCode;
        }

        private ReturnCode Initialize()
        {

            System.Diagnostics.Debug.WriteLine("Initializing DatabaseManager");



            DBConnect = new SQLiteConnection("Data source = " + AppDataManager.Instance.dbFile + "; Version = 3; ");

            //Setup Database file - if it goes well, Create the table if needed, else return error.

            ReturnCode code = CreateDbFileIfNotExists();
            switch (code)
            {
                //Ingen db fil fundet - ny er oprettet
                case ReturnCode.OK:
                    if (CreateTables() != ReturnCode.OK)
                    {
                        return ReturnCode.ERROR;
                    }
                    
                    break;

                //DB fil findes i forvejen
                case ReturnCode.FORHINDRING:
                    break;

                //fejl ved oprettelse / identificering af db fil
                case ReturnCode.ERROR:
                    break;

            }
            return code;

        }




        private ReturnCode CreateDbFileIfNotExists()
        {
            try
            {
                if (!(File.Exists(AppDataManager.Instance.dbFile)))
                {
                    SQLiteConnection.CreateFile(AppDataManager.Instance.dbFile);
                    Utility.WriteWarningMessage("Programmet opretter en ny database - Hvis der burdte findes en database i forvejen, findes den ikke på følgende sti: " + AppDataManager.Instance.dbFile + "." +
                    "\nKontakt venligst teknisk support på følgende mail: " + supportEmail);
                    Utility.WriteLog("Db file created @" + AppDataManager.Instance.dbFile);



                }
                else
                {
                    Utility.WriteLog("Db file found @" + AppDataManager.Instance.dbFile);
                    return ReturnCode.FORHINDRING;
                }
            }
            catch (System.Exception e)
            {

                Utility.WriteLog(e.Message);

                Utility.WriteWarningMessage("Oops! Programmet kunne ikke oprette database filen - Hvis der burdte findes en database i forvejen, findes den ikke på følgende sti: " + AppDataManager.Instance.dbFile + "." +
                    "\nKontakt venligst teknisk support på følgende mail: " + supportEmail);



                return ReturnCode.ERROR;
            }

            return ReturnCode.OK;
        }

        private ReturnCode PopulateDb()
        {
            string cmd = "insert into " + messageTemplatesTableName + " values (null," +
               "''," +
               "''," +
               "''," +
               "''," +
               "''," +
               "''," +
               "''," +
               "'')";

            ExecuteSQLiteNonQuery(cmd);

            ReturnCode returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode != ReturnCode.OK)
            {
                Utility.WriteLog("SQLite Error: " + cmd);
                Utility.WriteWarningMessage("Fejl ved tilføjelse af test beskeder");
            }
            return returnCode;
        }

        private ReturnCode CreateTables()
        {

            string cmd = "DROP TABLE IF EXISTS " + messageTemplatesTableName;

            ReturnCode returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode != ReturnCode.OK)
            {
                Utility.WriteLog("SQLite Error: " + cmd);
                Utility.WriteWarningMessage("Fejl ved oprettelse af database - kontakt venligt it support");
                return returnCode;
            }

            cmd = "Create table " + messageTemplatesTableName + "" +
            "(id integer primary key, " +
            "title varchar, " +
            "text varchar, " +
            "date varchar)";

            returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode == ReturnCode.OK)
            {
                Utility.WriteLog("Table '" + messageTemplatesTableName + "' has been created!");

#if DEBUG
                if (PopulateDb() != ReturnCode.OK)
                {
                    returnCode = ReturnCode.ERROR;
                }
                
#endif

            }
            else
            {
                Utility.WriteLog("ERROR: Could not create table '" + messageTemplatesTableName + "'!");
                Utility.WriteWarningMessage("Fejl ved oprettelse af database - kontakt venligt it support");

            }

            return returnCode;

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
                Utility.WriteLog("Error in executing SQLiteNonQuery! Error messages: \n" + e.Message);
                Utility.WriteLog("SQLite command: " + command);

                returnCode = ReturnCode.ERROR;
            }
            DBConnect.Close();

            return returnCode;
        }



        public List<MessageTemplate> GetMessageTemplates()
        {
            if (messages == null || messages.Count == 0)
            {
                if (GetAllMessagesFromDB() == ReturnCode.ERROR)
                {
                    return null;
                }
            }

            return messages;
        }

        public Dictionary<string, string> GetMessageTemplatesDictionaryTitleId()
        {

            if (GetAllMessagesFromDB() == ReturnCode.ERROR)
            {
                return null;
            }


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

        private ReturnCode GetAllMessagesFromDB()
        {
            //TODO: get messages from db instead! DELETE THIS
            messages = new List<MessageTemplate>() { new MessageTemplate("Send fødselsdagsbesked til kunde"),
                new MessageTemplate("Inviter kunde til årlig møde"),
                new MessageTemplate("Dette er en test beskedsskabelon med en meget lang titel\n for at teste hvordan brugergrænsefladen reagere"), new MessageTemplate("Besked mht. mistænksom bevægelse på kundens netbank"),
                 new MessageTemplate("Test beskedsskabelon 1"), new MessageTemplate("Test beskedsskabelon 2"),
                 new MessageTemplate("Test beskedsskabelon 3"), new MessageTemplate("Test beskedsskabelon 4"),
                 new MessageTemplate("Test beskedsskabelon 5"), new MessageTemplate("Test beskedsskabelon 6"),
                 new MessageTemplate("Test beskedsskabelon 7"), new MessageTemplate("Test beskedsskabelon 8"),
                 new MessageTemplate("Test beskedsskabelon 9"), new MessageTemplate("Test beskedsskabelon 10"),
                 new MessageTemplate("Test beskedsskabelon 11"), new MessageTemplate("Test beskedsskabelon 12"),
                 new MessageTemplate("Test beskedsskabelon 13"), new MessageTemplate("Test beskedsskabelon 14"),
                 new MessageTemplate("Test beskedsskabelon 15"), new MessageTemplate("Test beskedsskabelon 16"),
                 new MessageTemplate("Test beskedsskabelon 17"), new MessageTemplate("Test beskedsskabelon 18"),
                 new MessageTemplate("Test beskedsskabelon 19"), new MessageTemplate("Test beskedsskabelon 20"),
                 new MessageTemplate("Test beskedsskabelon 21"), new MessageTemplate("Test beskedsskabelon 22"),
             new MessageTemplate("Test beskedsskabelon 23"), new MessageTemplate("Test beskedsskabelon 24")};
            //

            return ReturnCode.OK;
        }

        public List<MessageTemplate> GetMessagesContainingText(string text)
        {
            GetAllMessagesFromDB();
            List<MessageTemplate> tmp = messages;


            return tmp = messages.Where(x =>
            x.title.ToUpper().
            Contains(text.ToUpper()))
            .ToList();
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
