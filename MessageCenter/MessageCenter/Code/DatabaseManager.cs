using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

namespace MessageCenter.Code
{
    /// <summary>
    /// A modified singleton class that manages database communication
    /// </summary>
    public class DatabaseManager
    {
        private SQLiteConnection DBConnect;

        /// <summary>
        /// Returns the message template table name from the configurations file
        /// </summary>
        private string MessageTemplatesTableName
        {
            get
            {
                return Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.MESSAGE_TEMPLATE_TABLE_NAME);
            }
        }

        /// <summary>
        /// Returns the message logs table name from the configurations file
        /// </summary>
        private string MessageLogTableName
        {
            get
            {
                return Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.MESSAGE_LOG_TABLE_NAME);
            }
        }

        /// <summary>
        /// Returns the attachments table name from the configurations file
        /// </summary>
        private string AttachmentsTableName
        {
            get
            {
                return Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.ATTACHMENTS_TABLE_NAME);
            }
        }

        /// <summary>
        /// The current user's databasemanager instance
        /// </summary>
        public static DatabaseManager Instance
        {
            get
            {
                HttpSessionState session = HttpContext.Current.Session;
                if (session["databaseManager"] == null)
                {
                    session["databaseManager"] = new DatabaseManager();
                }
                return (DatabaseManager)session["databaseManager"];
            }
        }

        /// <summary>
        /// private constructor, so this class cannot be instanciated from outside this class 
        /// </summary>
        private DatabaseManager()
        {
            //initialize 
            if (Initialize() == StatusCode.ERROR)
            {
                //error
                Utility.WriteLog("Failed to initialize DatabaseManager");
                Utility.PrintWarningMessage("Der kunne ikke oprettes forbindelse til databasen - kontakt venligst teknisk support");

                return;
            }

            // LoadAllMessageTemplates();

        }

        /// <summary>
        /// Extracts a message template object from an sqlite data reader
        /// </summary>
        /// <param name="dataReader">the result of the query</param>
        /// <returns></returns>
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



       

        /// <summary>
        /// initializing code for the database manager - this is run when the singleton is instanciated
        /// </summary>
        /// <returns></returns>
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



        /// <summary>
        /// creates a db file if one does not exist
        /// </summary>
        /// <returns></returns>
        private StatusCode CreateDbFileIfNotExists()
        {
            try
            {
                //if file does not exist
                if (!(File.Exists(FileManager.Instance.DbFile)))
                {
                    //create db file
                    SQLiteConnection.CreateFile(FileManager.Instance.DbFile);

                    //inform user, in case there should already exist a db
                    Utility.PrintWarningMessage("Programmet opretter en ny database - Hvis der burdte findes en database i forvejen, findes den ikke på følgende sti: " + FileManager.Instance.DbFile + "." +
                    "\nKontakt venligst teknisk support på følgende mail: " +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));

                    Utility.WriteLog("Db file created @" + FileManager.Instance.DbFile);

                }
                else
                {
                    //file found - no need to create a new one
                    Utility.WriteLog("Db file found @" + FileManager.Instance.DbFile);
                    return StatusCode.FORHINDRING;
                }
            }
            catch (System.Exception e)
            {
                //File was not found but the applikation was not allowed to create a db file!

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
            //get the next id
            int? id = GetNextId(MessageTemplatesTableName);

            //if null, auto increment (if this is the first message template)
            string idString = id == null ? "null" : id.ToString();

            //create a command that adds a new message template
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

            //return the id of the newly added attachment (or null)
            return id;
        }

        /// <summary>
        /// Deletes the MessageTemplate with the input id, and all attachments related to it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StatusCode DeleteMessageTemplate(int id)
        {
            //create command to delete the message template with the input id
            string command = string.Format("delete from {0} where id = {1};"
                , MessageTemplatesTableName, id.ToString());

            StatusCode result = ExecuteSQLiteNonQuery(command);

            if (result == StatusCode.OK)
            {
                //if succesful, delete all of its attachments
                command = string.Format("delete from {0} where messageId = {1};"
                , AttachmentsTableName, id.ToString());

                result = ExecuteSQLiteNonQuery(command);
            }

            return result;
        }

        /// <summary>
        /// returns the next available id 
        /// </summary>
        /// <param name="tablename">the tablename</param>
        /// <returns></returns>
        public int? GetNextId(string tablename)
        {
            //select the highest id+1 from the table
            string cmdText = "SELECT MAX(id)+1 from " + tablename + ";";

            int? id = null;
            //open db connection
            DBConnect.Open();

            //convert string command to sqlite command
            SQLiteCommand sqliteCmd = new SQLiteCommand(cmdText, DBConnect);

            try
            {
                //execute command
                using (SQLiteDataReader dataReader = sqliteCmd.ExecuteReader())
                {
                    //read result
                    while (dataReader.Read())
                    {
                        //get the resut id
                        id = dataReader.GetInt32(dataReader.GetOrdinal("MAX(id)+1"));

                    }
                }
            }
            catch (Exception e)
            {
                //There are no elements in the table
                //return null which is fine

            }
            //close db connection
            DBConnect.Close();

            return id;
        }

        /// <summary>
        /// add an attachment to the db
        /// </summary>
        /// <param name="attachment">the attachment</param>
        /// <param name="messageTemplateId">the message template it should be related to</param>
        /// <returns></returns>
        public StatusCode AddAttachmentToDB(MessageAttachment attachment, int messageTemplateId)
        {
            StatusCode status = StatusCode.OK;

            //create command to insert attachment
            SQLiteCommand cmd = new SQLiteCommand(
                "insert into " + AttachmentsTableName + " values (null," +
              messageTemplateId +
              ", '" + attachment.FileName + "', @fileData)"
              , DBConnect);

            //Create @fileData parameter that converts the data into a "blob"
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

        /// <summary>
        /// Execute the input command without performing a query
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private StatusCode ExecuteSQLiteNonQuery(string command)
        {
            StatusCode returnCode = StatusCode.OK;

            //open db connection
            DBConnect.Open();

            //convert input to a sqlite command
            SQLiteCommand Command = new SQLiteCommand(command, DBConnect);

            try
            {
                //execute command
                Command.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                //if failed, print why
                Utility.WriteLog("Error in executing SQLiteNonQuery! Error messages: \n" + e.Message);
                Utility.WriteLog("SQLite command: " + command);

                returnCode = StatusCode.ERROR;
            }
            //close connection
            DBConnect.Close();

            //return status of the command execution
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

            //open db connection
            DBConnect.Open();

            //create a comman that gets all message templates
            SQLiteCommand Command = new SQLiteCommand("select * from "
            + MessageTemplatesTableName + ";", DBConnect);

            try
            {
                //execute command
                using (SQLiteDataReader dataReader = Command.ExecuteReader())
                {
                    //read result
                    while (dataReader.Read())//for each message template
                    {
                        //extract as message template
                        tmpMessage = ExtractMessageTemplateData(dataReader);

                        //add to list
                        listOfMessages.Add(tmpMessage);
                    }
                }
            }
            catch (Exception)
            {

                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af alle beskedskabeloner!");

            }

            //close db connection
            DBConnect.Close();

            //return list of message templates
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



        /// <summary>
        /// return a list containing all message templates where the title contains the input string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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

            //open db connection
            DBConnect.Open();

            //create command that gets all attachments related to the input messageTemplate id
            SQLiteCommand command = new SQLiteCommand("select * from "
            + AttachmentsTableName +
            " WHERE messageId = " + messageId + ";", DBConnect);

            MessageAttachment tmpAttachment = null;

            try
            {
                //execute command
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read()) //foreach attachment related to the message id
                    {
                        //Convert response to attachment
                        tmpAttachment = ExtractAttachmentData(dataReader);

                        if (tmpAttachment != null)
                        {
                            //add to list
                            attachments.Add(tmpAttachment);
                        }

                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af vedhæftede filer for besked med id " + messageId.ToString() + "!" +
                    "\n SQL kommando: " + command);

            }

            //close db connection
            DBConnect.Close();

            //return list of attachments
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

            //open the db connection
            DBConnect.Open();

            //create command
            SQLiteCommand Command = new SQLiteCommand("select * from "
            + MessageTemplatesTableName +
            " WHERE title LIKE '%" + textToContain + "%';", DBConnect);

            try
            {
                //execute command
                using (SQLiteDataReader dataReader = Command.ExecuteReader())
                {
                    //read the result
                    while (dataReader.Read())//foreach message template in result
                    {
                        //convert result to messagetemplate
                        tmpMessage = ExtractMessageTemplateData(dataReader);

                        //add to list
                        listOfMessages.Add(tmpMessage);
                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af alle beskedskabeloner!");

            }
            //close db connection
            DBConnect.Close();

            //return the list of msg templates, or an empty one if none was found
            return listOfMessages;
        }
        /// <summary>
        /// Returns the message template with the input id, if it exists
        /// </summary>
        /// <param name="id">unique identifier of the message template</param>
        /// <returns></returns>
        public MessageTemplate GetMessageTemplateFromId(int id)
        {

            MessageTemplate tmpMessage = null;

            //open the db connection
            DBConnect.Open();

            //build command 
            SQLiteCommand command = new SQLiteCommand("select * from "
            + MessageTemplatesTableName +
            " WHERE id = " + id + " " +
            "LIMIT 1;", DBConnect);


            try
            {
                //ececute command
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    //Read result
                    while (dataReader.Read())
                    {
                        //try to extract a message tempalte from the datareader
                        tmpMessage = ExtractMessageTemplateData(dataReader);

                        if (tmpMessage != null)
                        {
                            //success
                            break;
                        }

                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af beskedskabelon med id " + id.ToString() + "!" +
                    "\n SQL kommando: " + command);
            }

            //close db connection
            DBConnect.Close();

            //Return the message template or null if none was found
            return tmpMessage;
        }


        /// <summary>
        /// Stores and logs information about the recently sent message (or attempt)
        /// </summary>
        /// <param name="messageTemplateId">id of the message template</param>
        /// <param name="status">statuscode of the sent message</param>
        /// <param name="senderTuser">t user of the sender</param>
        /// <param name="ricipientCpr">cpr of the initially selected recipient</param>
        /// <param name="ricipientAdresse">the actual adress the message was sent to</param>
        /// <param name="title">the title of the sent message</param>
        /// <param name="text">the main text of the sent message</param>
        public void LogSentMessage(int? messageTemplateId, StatusCode status, string senderTuser, string ricipientCpr,
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
                title.Replace("'", ""),
                text.Replace("'", ""),
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
               + "timeStamp: " + timeStamp + "\n");

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
