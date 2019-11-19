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

        public static readonly string supportEmail = "andr4376@gmail.com";



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
            if (Initialize() == StatusCode.ERROR)
            {
                Utility.WriteLog("Failed to initialize DatabaseManager");

                return;
            }

            // LoadAllMessageTemplates();

        }
        private MessageTemplate ExtractMessageTemplateData(SQLiteDataReader dataReader)
        {
            MessageTemplate tmpMessage = new MessageTemplate(
                dataReader.GetInt32(0),//id
                dataReader.GetString(1),//title
                dataReader.GetString(2),//text
                dataReader.GetInt32(3)//messagetype
                );

            return tmpMessage;
        }

        private StatusCode LoadAllMessageTemplates()
        {
            StatusCode returnCode = StatusCode.OK;


            List<MessageTemplate> listOfMessages = new List<MessageTemplate>();
            MessageTemplate tmpMessage = null;

            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand("select * from " + messageTemplatesTableName + ";", DBConnect);

            try
            {
                using (SQLiteDataReader dataReader = Command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {


                        tmpMessage = ExtractMessageTemplateData(dataReader);

                        listOfMessages.Add(tmpMessage);


                    }
                }
            }
            catch (Exception)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af alle beskedskabeloner!");
                throw;
#endif
            }

            DBConnect.Close();
            messages = listOfMessages;


            return returnCode;
        }

        private StatusCode Initialize()
        {

            System.Diagnostics.Debug.WriteLine("Initializing DatabaseManager");



            DBConnect = new SQLiteConnection("Data source = " + AppDataManager.Instance.DbFile + "; Version = 3; ");

            //Setup Database file - if it goes well, Create the table if needed, else return error.

            StatusCode code = CreateDbFileIfNotExists();
            switch (code)
            {
                //Ingen db fil fundet - ny er oprettet
                case StatusCode.OK:
                    if (CreateTables() != StatusCode.OK)
                    {
                        return StatusCode.ERROR;
                    }

                    break;

                //DB fil findes i forvejen
                case StatusCode.FORHINDRING:
                    break;

                //fejl ved oprettelse / identificering af db fil
                case StatusCode.ERROR:
                    break;

            }
            return code;

        }




        private StatusCode CreateDbFileIfNotExists()
        {
            try
            {
                if (!(File.Exists(AppDataManager.Instance.DbFile)))
                {
                    SQLiteConnection.CreateFile(AppDataManager.Instance.DbFile);
                    Utility.PrintWarningMessage("Programmet opretter en ny database - Hvis der burdte findes en database i forvejen, findes den ikke på følgende sti: " + AppDataManager.Instance.DbFile + "." +
                    "\nKontakt venligst teknisk support på følgende mail: " + supportEmail);
                    Utility.WriteLog("Db file created @" + AppDataManager.Instance.DbFile);

                }
                else
                {
                    Utility.WriteLog("Db file found @" + AppDataManager.Instance.DbFile);
                    return StatusCode.FORHINDRING;
                }
            }
            catch (System.Exception e)
            {

                Utility.WriteLog(e.Message);

                Utility.PrintWarningMessage("Oops! Programmet kunne ikke oprette database filen - Hvis der burdte findes en database i forvejen, findes den ikke på følgende sti: " + AppDataManager.Instance.DbFile + "." +
                    "\nKontakt venligst teknisk support på følgende mail: " + supportEmail);



                return StatusCode.ERROR;
            }

            return StatusCode.OK;
        }

        /// <summary>
        /// Populates the DB with test MessageTemplates if the solution is running in debug mode.
        /// </summary>
        /// <returns>status</returns>
        private StatusCode PopulateDb()
        {

            Random rnd = new Random();

            StatusCode status = StatusCode.OK;

#if DEBUG
            for (int i = 0; i < 25; i++)
            {
                MessageTemplate testMessage = new MessageTemplate("Besked om økonomiske vanskligheder" + i, "Kære "
                    + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_FULLNAME) + ",\n" +
                    "Jeg må desværre informere dig om at du befinder dig i en afsindigt ulækker økonomisk situation - Hvis jeg var dig, ville jeg " +
                    "stikke af til Mexico, før vi kommer og tager dine knæskalder. blablabla, her er dit Cpr nummer, " + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_CPR) + "" +
                    ", dit fornavn: " + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_FIRSTNAME) + "...\n"
                    + "Vi kan se at dit telefon nummer er "+ MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_PHONENUMBER)+", din email er "
                    + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_EMAIL)+", og vi ved du er "+ MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_AGE) + "!\n" +
                    "Tag og pas på du,\n" +
                    "Mvh, "+MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.EMPLOYEE_FULLNAME)+"\n Sparekassen Kronjylland -" + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.DEPARTMENT)+" afdelingen"
                    , rnd.Next(0, 1 + 1));

                status = AddMessageTemplate(testMessage);

                if (status != StatusCode.OK)
                {
                    break;
                }
            }
#endif
            return status;
        }

        /// <summary>
        /// Adds a new message template to the database
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private StatusCode AddMessageTemplate(MessageTemplate message)
        {
            string cmd = "insert into " + messageTemplatesTableName + " values (null," +
              "'" + message.Title + "'," +
              "'" + message.Text + "', " +
              +message.MessageTypeId + ")";

            StatusCode returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode != StatusCode.OK)
            {
                Utility.WriteLog("SQLite Error: " + cmd);
                Utility.PrintWarningMessage("Fejl ved tilføjelse af test beskeder");

            }
            return returnCode;
        }

        private StatusCode CreateTables()
        {

            string cmd = "DROP TABLE IF EXISTS " + messageTemplatesTableName;

            StatusCode returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode != StatusCode.OK)
            {
                Utility.WriteLog("SQLite Error: " + cmd);
                Utility.PrintWarningMessage("Fejl ved oprettelse af database - kontakt venligt it support");
                return returnCode;
            }

            cmd = "Create table " + messageTemplatesTableName + "" +
            "(id integer primary key, " +
            "title varchar, " +
            "text varchar, " +
            "messageType integer)";

            returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode == StatusCode.OK)
            {
                Utility.WriteLog("Table '" + messageTemplatesTableName + "' has been created!");

#if DEBUG
                if (PopulateDb() != StatusCode.OK)
                {
                    returnCode = StatusCode.ERROR;
                }

#endif

            }
            else
            {
                Utility.WriteLog("ERROR: Could not create table '" + messageTemplatesTableName + "'!");
                Utility.PrintWarningMessage("Fejl ved oprettelse af database - kontakt venligt it support");

            }

            return returnCode;

        }

        public StatusCode ExecuteSQLiteNonQuery(string command)
        {
            StatusCode returnCode = StatusCode.OK;

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

                returnCode = StatusCode.ERROR;
            }
            DBConnect.Close();

            return returnCode;
        }


        /// <summary>
        /// Returns a list containing all MessageTemplates stored in the DB
        /// </summary>
        /// <returns>all MessageTemplates stored in the DB</returns>
        public List<MessageTemplate> GetAllMessageTemplates()
        {
            List<MessageTemplate> listOfMessages = new List<MessageTemplate>();
            MessageTemplate tmpMessage = null;

            DBConnect.Open();

            SQLiteCommand Command = new SQLiteCommand("select * from "
            + messageTemplatesTableName + ";", DBConnect);

            try
            {
                using (SQLiteDataReader dataReader = Command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        tmpMessage = ExtractMessageTemplateData(dataReader);

                        listOfMessages.Add(tmpMessage);
                    }
                }
            }
            catch (Exception)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af alle beskedskabeloner!");
                throw;
#endif
            }

            DBConnect.Close();

            return listOfMessages;
        }
        /*
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
                        messagesDictionary.Add(message.Id.ToString(), message.Title);

                    }
                    return messagesDictionary;
                }
                */
        private StatusCode GetAllMessagesFromDB()
        {
            //TODO: get messages from db instead! DELETE THIS
            messages = new List<MessageTemplate>();
            //

            return StatusCode.OK;
        }



        public List<MessageTemplate> GetMessagesTitleContainsText(string text)
        {
            List<MessageTemplate> messagesContainingString = DBQueryTitleContains(text);

            if (messagesContainingString != null)
            {
                Utility.WriteLog(messagesContainingString.Count + " messages found containing '" + text + "'");

            }

            return messagesContainingString;
        }

        private List<MessageTemplate> DBQueryTitleContains(string textToContain)
        {

            List<MessageTemplate> listOfMessages = new List<MessageTemplate>();
            MessageTemplate tmpMessage = null;

            DBConnect.Open();

            SQLiteCommand Command = new SQLiteCommand("select * from "
            + messageTemplatesTableName +
            " WHERE title LIKE '%" + textToContain + "%';", DBConnect);

            try
            {
                using (SQLiteDataReader dataReader = Command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        tmpMessage = ExtractMessageTemplateData(dataReader);

                        listOfMessages.Add(tmpMessage);
                    }
                }
            }
            catch (Exception)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af alle beskedskabeloner!");
                throw;
#endif
            }

            DBConnect.Close();

            return listOfMessages;
        }
        public MessageTemplate GetMessageTemplateFromId(int id)
        {

            MessageTemplate tmpMessage = null;

            DBConnect.Open();

            SQLiteCommand command = new SQLiteCommand("select * from "
            + messageTemplatesTableName +
            " WHERE id = " + id + " " +
            "LIMIT 1;", DBConnect);


            try
            {
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        tmpMessage = ExtractMessageTemplateData(dataReader);

                        if (tmpMessage != null)
                        {
                            break;
                        }

                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af beskedskabelon med id " + id.ToString() + "!" +
                    "\n SQL kommando: " + command);
#if DEBUG
                throw;
#endif
            }

            DBConnect.Close();

            return tmpMessage;
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
