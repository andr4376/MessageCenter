using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
            if (Initialize() == ReturnCode.ERROR)
            {
                System.Diagnostics.Debug.WriteLine("Failed to initialize DatabaseManager");

                return;
            }
            //TODO: load pages - see ConsoleEntries project - read from DB
            LoadMessageTemplates();

        }

        private ReturnCode LoadMessageTemplates()
        {
            ReturnCode returnCode = ReturnCode.OK;



            return returnCode;
        }

        private ReturnCode Initialize()
        {
            System.Diagnostics.Debug.WriteLine("Initializing DatabaseManager");


            DBConnect = new SQLiteConnection("Data source = " + dbPath + "; Version = 3; ");

            //Setup Database file - if it goes well, Create the table if needed, else return error.
            return SetupDbFile() == ReturnCode.OK ? CreateTablesIfNotExists() : ReturnCode.ERROR;

        }

        private ReturnCode SetupDbFile()
        {

            if (CreateDbDirectoriesIfNotExists() != ReturnCode.OK)
            {
                return ReturnCode.ERROR;
            }


            if (CreateDbFileIfNotExists() != ReturnCode.OK)
            {
                return ReturnCode.ERROR;
            }


            return ReturnCode.OK;
        }


        private ReturnCode CreateDbDirectoriesIfNotExists()
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

                // lav \ om til /, så den kan skrives til UI
                directoryPath = Regex.Replace(directoryPath, @"\\", "/");

                //fjern filen fra mappestien
                directoryPath = directoryPath.Replace("/Database.db", "");

                //skriv fejlbesked til UI
                Utility.WriteWarningMessage("Oops! Programmet kunne ikke oprette stien til database filen ('"
                    + directoryPath +
                    "'). Programmet blev enten nægtet adgang, eller stien er ugyldig... " +
                    "Kontakt venligst teknisk support på følgende mail: " + supportEmail);

                return ReturnCode.ERROR;
            }

            return ReturnCode.OK;
        }

        private ReturnCode CreateDbFileIfNotExists()
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
                    System.Diagnostics.Debug.WriteLine("Db file found @" + dbPath);
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

            string createMessageTemplatesTable = "Create table IF NOT EXISTS " + messageTemplatesTableName + "" +
                "(id integer primary key, title varchar, text varchar, date varchar)";

            ReturnCode returnCode = ExecuteSQLiteNonQuery(createMessageTemplatesTable);

            if (returnCode == ReturnCode.OK)
            {
                System.Diagnostics.Debug.WriteLine("Table '" + messageTemplatesTableName + "' is ready!");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Could not create table '" + messageTemplatesTableName + "'!");
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
                System.Diagnostics.Debug.WriteLine("Error in executing SQLiteNonQuery! Error messages: \n" + e.Message);
                System.Diagnostics.Debug.WriteLine("SQLite command: " + command);

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
