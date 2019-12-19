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

       

        private SQLiteConnection DBConnect;

        private string MessageTemplatesTableName
        {
            get
            {

                return Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.MESSAGE_TEMPLATE_TABLE_NAME);
            }
        }

        private string MessageLogTableName
        {
            get
            {
                return Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.MESSAGE_LOG_TABLE_NAME); ;
            }
        }

        private string AttachmentsTableName
        {
            get
            {
                return Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.ATTACHMENTS_TABLE_NAME);
            }
        }

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
                Utility.PrintWarningMessage("Der kunne ikke oprettes forbindelse til databasen - kontakt venligst teknisk support");

                return;
            }

            // LoadAllMessageTemplates();

        }
        private MessageTemplate ExtractMessageTemplateData(SQLiteDataReader dataReader)
        {
            MessageTemplate tmpMessage = new MessageTemplate(
                dataReader.GetInt32(dataReader.GetOrdinal("id")),
                dataReader.GetString(dataReader.GetOrdinal("title")),
                dataReader.GetString(dataReader.GetOrdinal("text")),
                dataReader.GetInt32(dataReader.GetOrdinal("messagetype"))
                );

            return tmpMessage;
        }



        /// <summary>
        /// Returns a messageattachment object from the sqlite response
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        private MessageAttachment ExtractAttachmentData(SQLiteDataReader dataReader)
        {

            int id = dataReader.GetInt32(dataReader.GetOrdinal("id"));
            int messageId = dataReader.GetInt32(dataReader.GetOrdinal("messageId"));
            string fileName = dataReader.GetString(dataReader.GetOrdinal("fileName"));
            byte[] fileData = null;

            //Extract file data 
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var fileDataStream = dataReader.GetStream(dataReader.GetOrdinal("fileData")))
                {
                    fileDataStream.CopyTo(memoryStream);
                }
                fileData = memoryStream.ToArray();
            }

            MessageAttachment attachment = new MessageAttachment(
                id, messageId, fileName, fileData);


            return attachment;
        }



        /*
private StatusCode LoadAllMessageTemplates()
{
   StatusCode returnCode = StatusCode.OK;


   List<MessageTemplate> listOfMessages = new List<MessageTemplate>();
   MessageTemplate tmpMessage = null;

   DBConnect.Open();



   SQLiteCommand Command = new SQLiteCommand("select * from " + MessageTemplatesTableName + ";", DBConnect);

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
*/
        private StatusCode Initialize()
        {

            System.Diagnostics.Debug.WriteLine("Initializing DatabaseManager");



            DBConnect = new SQLiteConnection("Data source = " + FileManager.Instance.DbFile + "; Version = 3; ");

            //Setup Database file - if it goes well, Create the table if needed, else return error.

            StatusCode code = CreateDbFileIfNotExists();

            string script = File.ReadAllText(
                FileManager.Instance.GetFilePath(
                    "SqlScripts/CreateTables.sql"));

            ExecuteSQLiteNonQuery(script);

            switch (code)
            {
                //Ingen db fil fundet - ny er oprettet
                case StatusCode.OK:

                    /*if (CreateTables() != StatusCode.OK)
                   {
                       return StatusCode.ERROR;
                   }*/

                    //Insert DEMO message templates - one Mail and one SMS
                    script = File.ReadAllText(
                FileManager.Instance.GetFilePath(
                    "SqlScripts/InsertInitialData.sql"));

                    ExecuteSQLiteNonQuery(script);

                    foreach (MessageAttachment demoAttachment in MessageAttachment.GetTestAttachment())
                    {
                        //Add attachments to the first element in messageTemplates table (mail)
                        AddAttachmentToDB(demoAttachment, 1);
                    }
                    break;

                //DB fil findes i forvejen
                case StatusCode.FORHINDRING:
                    break;

                //fejl ved oprettelse / identificering af db fil
                case StatusCode.ERROR:
                    //Fejlhåndteres i constructor
                    break;

            }
            return code;

        }




        private StatusCode CreateDbFileIfNotExists()
        {
            try
            {
                if (!(File.Exists(FileManager.Instance.DbFile)))
                {
                    SQLiteConnection.CreateFile(FileManager.Instance.DbFile);
                    Utility.PrintWarningMessage("Programmet opretter en ny database - Hvis der burdte findes en database i forvejen, findes den ikke på følgende sti: " + FileManager.Instance.DbFile + "." +
                    "\nKontakt venligst teknisk support på følgende mail: " +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));
                    Utility.WriteLog("Db file created @" + FileManager.Instance.DbFile);

                }
                else
                {
                    Utility.WriteLog("Db file found @" + FileManager.Instance.DbFile);
                    return StatusCode.FORHINDRING;
                }
            }
            catch (System.Exception e)
            {

                Utility.WriteLog(e.Message);

                Utility.PrintWarningMessage("Oops! Programmet kunne ikke oprette database filen - Hvis der burdte findes en database i forvejen, findes den ikke på følgende sti: " + FileManager.Instance.DbFile + "." +
                    "\nKontakt venligst teknisk support på følgende mail: " +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));



                return StatusCode.ERROR;
            }

            return StatusCode.OK;
        }


        /// <summary>
        /// Adds a new message template to the database
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int? AddMessageTemplate(MessageTemplate message)
        {
            int? id = GetNextId(MessageTemplatesTableName);

            string idString = id == null ? "null" : id.ToString();

            string cmd = "insert into " + MessageTemplatesTableName + " values (" + idString + "," +
              "'" + message.Title + "'," +
              "'" + message.Text + "', " +
              +message.MessageTypeId + ")";

            StatusCode returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode != StatusCode.OK)
            {
                Utility.WriteLog("SQLite Error: " + cmd);
                Utility.PrintWarningMessage("Fejl ved tilføjelse af test beskeder");
                return id;
            }

            return id;
        }

        /// <summary>
        /// Deletes the MessageTemplate with the input id, and all attachments related to it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StatusCode DeleteMessageTemplate(int id)
        {
            string command = string.Format("delete from {0} where id = {1};"
                , MessageTemplatesTableName, id.ToString());

            StatusCode result = ExecuteSQLiteNonQuery(command);

            if (result == StatusCode.OK)
            {
                command = string.Format("delete from {0} where messageId = {1};"
                , AttachmentsTableName, id.ToString());

                result = ExecuteSQLiteNonQuery(command);
            }

            return result;
        }

        public int? GetNextId(string tablename)
        {
            string cmdText = "SELECT MAX(id)+1 from " + tablename + ";";

            int? id = null;

            DBConnect.Open();

            SQLiteCommand sqliteCmd = new SQLiteCommand(cmdText, DBConnect);

            try
            {
                using (SQLiteDataReader dataReader = sqliteCmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        id = dataReader.GetInt32(dataReader.GetOrdinal("MAX(id)+1"));

                    }
                }
            }
            catch (Exception e)
            {
                //There are no elements in the table
                //return null which is fine

            }

            DBConnect.Close();

            return id;
        }


        public StatusCode AddAttachmentToDB(MessageAttachment attachment, int messageTemplateId)
        {
            StatusCode status = StatusCode.OK;

            SQLiteCommand cmd = new SQLiteCommand(
                "insert into " + AttachmentsTableName + " values (null," +
              messageTemplateId +
              ", '" + attachment.FileName + "', @fileData)"
              , DBConnect);

            //Create @fileData parameter that converts 
            SQLiteParameter parameter = new SQLiteParameter("@fileData", System.Data.DbType.Binary);
            parameter.Value = attachment.FileData;
            cmd.Parameters.Add(parameter);

            DBConnect.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                Utility.WriteLog("Error in executing SQLiteNonQuery! Error messages: \n" + e.Message);
                status = StatusCode.ERROR;
            }

            DBConnect.Close();


            return status;
        }


        private StatusCode ExecuteSQLiteNonQuery(string command)
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
            + MessageTemplatesTableName + ";", DBConnect);

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
     



        public List<MessageTemplate> GetMessagesTitleContainsText(string text)
        {
            List<MessageTemplate> messagesContainingString = DBQueryTitleContains(text);

            if (messagesContainingString != null)
            {
                Utility.WriteLog(messagesContainingString.Count + " messages found containing '" + text + "'");

            }

            return messagesContainingString;
        }


        /// <summary>
        /// Returns a list of message attachments related to the input message id
        /// </summary>
        /// <param name="messageId">id of the message you wish to fetch attachments for</param>
        /// <returns></returns>
        public List<MessageAttachment> GetAttachmentsFromMessageId(int? messageId)
        {
            if (messageId == null)
            {
                return null;
            }

            List<MessageAttachment> attachments = new List<MessageAttachment>();

            DBConnect.Open();

            SQLiteCommand command = new SQLiteCommand("select * from "
            + AttachmentsTableName +
            " WHERE messageId = " + messageId + ";", DBConnect);

            MessageAttachment tmpAttachment = null;

            try
            {
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read()) //foreach attachment related to the message id
                    {
                        //Convert response to attachment
                        tmpAttachment = ExtractAttachmentData(dataReader);

                        if (tmpAttachment != null)
                        {
                            attachments.Add(tmpAttachment);
                        }

                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af vedhæftede filer for besked med id " + messageId.ToString() + "!" +
                    "\n SQL kommando: " + command);
#if DEBUG
                throw;
#endif
            }

            DBConnect.Close();

            return attachments;
        }

        /// <summary>
        /// Returns a list of message templates, which title contains the input string
        /// </summary>
        /// <param name="textToContain"></param>
        /// <returns></returns>
        private List<MessageTemplate> DBQueryTitleContains(string textToContain)
        {

            List<MessageTemplate> listOfMessages = new List<MessageTemplate>();
            MessageTemplate tmpMessage = null;

            DBConnect.Open();

            SQLiteCommand Command = new SQLiteCommand("select * from "
            + MessageTemplatesTableName +
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
            + MessageTemplatesTableName +
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


        /// <summary>
        /// Stores and logs information about the recently sent message (or attempt)
        /// </summary>
        /// <param name="messageTemplateId"></param>
        /// <param name="status"></param>
        /// <param name="senderTuser"></param>
        /// <param name="ricipientCpr"></param>
        /// <param name="ricipientAdresse"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        public void LogSentMessage(int? messageTemplateId ,StatusCode status, string senderTuser, string ricipientCpr,
            string ricipientAdresse, string title, string text)
        {
            string timeStamp = DateTime.Now.ToString();

            string command = string.Format(
                "insert into {0} VALUES(null," +
                "{1}," +
                "'{2}'," +
                "'{3}'," +
                "'{4}'," +
                "'{5}'," +
                "'{6}'," +
                "'{7}'," +
                "'{8}');",
                MessageLogTableName,              //0   
                messageTemplateId.ToString(),     //1
                status,                           //2
                senderTuser,                      //ect.
                ricipientCpr,
                ricipientAdresse,
                title,
                text,
                timeStamp
                );

           StatusCode sqliteStatus =
                ExecuteSQLiteNonQuery(command);
            

            Utility.WriteLog("\nLogging sent message attempt... SQLite execution status: " + sqliteStatus.ToString() + "! MessageInfo:\n"
               + "Message Template ID: " + messageTemplateId.ToString() + "\n"
               + "Message Status: " + status + "\n"
               + "Sender TUser: " + senderTuser + "\n"
               + "ricipientCpr: " + ricipientCpr + "\n"
               + "ricipientAdresse: " + ricipientAdresse + "\n"
               + "title: " + title + "\n"
               + "timeStamp: " + timeStamp+"\n");         
                                 
        }
    }

}



/*
        private StatusCode CreateTables()
        {
            //Create MessageTemplates table

            string cmd = "DROP TABLE IF EXISTS " + MessageTemplatesTableName;

            StatusCode returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode != StatusCode.OK)
            {
                Utility.WriteLog("SQLite Error: " + cmd);

                return returnCode;
            }

            cmd = "Create table " + MessageTemplatesTableName + "" +
            "(id integer primary key, " +
            "title varchar, " +
            "text varchar, " +
            "messageType integer)";

            returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode == StatusCode.OK)
            {
                Utility.WriteLog("Table '" + MessageTemplatesTableName + "' has been created!");

#if DEBUG
                if (PopulateDb() != StatusCode.OK)
                {
                    returnCode = StatusCode.ERROR;
                }
#endif
            }
            else
            {
                Utility.WriteLog("ERROR: Could not create table '" + MessageTemplatesTableName + "'!");


            }

            //Create attachment table
            cmd = "DROP TABLE IF EXISTS " + AttachmentsTableName;

            returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode != StatusCode.OK)
            {
                Utility.WriteLog("SQLite Error: " + cmd);

                return returnCode;
            }

            cmd = "Create table " + AttachmentsTableName + "" +
            "(id integer primary key, " +
            "messageId integer, " +
            "fileName varchar, " +
            "fileData BLOB)";

            returnCode = ExecuteSQLiteNonQuery(cmd);

            if (returnCode == StatusCode.OK)
            {
                Utility.WriteLog("Table '" + AttachmentsTableName + "' has been created!");

#if DEBUG
                if (PopulateDb() != StatusCode.OK)
                {
                    returnCode = StatusCode.ERROR;
                }

#endif

            }
            else
            {
                Utility.WriteLog("ERROR: Could not create table '" + AttachmentsTableName + "'!");


            }

            return returnCode;

        }
        */


/*
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
            MessageTemplate testMessage = new MessageTemplate("Besked om økonomiske vanskligheder nr." + i, "Kære "
                + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_FULLNAME) + ",\n" +
                "Jeg må desværre informere dig om at du befinder dig i en afsindigt ulækker økonomisk situation - Hvis jeg var dig, ville jeg " +
                "stikke af til Mexico, før vi kommer og tager dine knæskalder. blablabla, her er dit Cpr nummer, " + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_CPR) + "" +
                ", dit fornavn: " + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_FIRSTNAME) + "...\n"
                + "Vi kan se at dit telefon nummer er " + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_PHONENUMBER) + ", din email er "
                + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_EMAIL) + ", og vi ved du er " + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.CUSTOMER_AGE) + "!\n" +
                "Tag og pas på du,\n" +
                "Mvh, " + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.EMPLOYEE_FULLNAME) + "\n Sparekassen Kronjylland -" + MessageHandler.GetMessageVariable(MESSAGE_VARIABLES.DEPARTMENT) + " afdelingen"
                , rnd.Next(0, 1 + 1));

            status = AddMessageTemplate(testMessage) == null ? StatusCode.ERROR : StatusCode.OK;

            if (status != StatusCode.OK)
            {
                break;
            }
        }
#endif
        return status;
    }
    */
