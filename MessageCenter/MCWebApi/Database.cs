
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace MessageCenterWebApi
{
    public class Database
    {/*
        static SQLiteConnection sqliteConn = new SQLiteConnection("Data Source=C:\\MessageCenter\\Database\\Messages.db");

        private static List<MessageTemplate> messageTemplates;

        public static List<MessageTemplate> MessageTemplates
        {
            get
            {
                if (messageTemplates == null)
                {
                    InitializeMessageList();
                }

                return messageTemplates;
            }
            set
            {
                messageTemplates = value;
            }
        }

        public static void CreateDatabase()
        {
            if (!System.IO.File.Exists("C:\\MessageCenter\\Database\\Highscore.db"))
            {
                System.IO.Directory.CreateDirectory("C:\\MessageCenter\\Database");
                SQLiteConnection.CreateFile("C:\\MessageCenter\\Database\\Messages.db");
            }
            CreateTables();
        }
        static void CreateTables()
        {
            //Highscore table created

            sqliteConn.Open();
            string CreateTableHighscore = "CREATE TABLE IF NOT EXISTS Messages" +
                "(ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "" + "user" + " varchar(50)," +
                "" + "score" + " INTEGER," +
                "DateTime DATETIME NOT NULL DEFAULT (datetime(CURRENT_TIMESTAMP, 'localtime')))";

            SQLiteCommand commandHighscore = new SQLiteCommand(CreateTableHighscore, sqliteConn);
            commandHighscore.ExecuteNonQuery();

            sqliteConn.Close();
        }

        public static void InitializeMessageList()
        {


            CreateDatabase();


            //Dummy list - normally read from database file
            messageTemplates = new List<MessageTemplate>();
            messageTemplates.Add(new MessageTemplate() { ID = 1, Name = "Charlie", Race = "Collie" });
            messageTemplates.Add(new MessageTemplate() { ID = 2, Name = "Basse", Race = "Labrador" });
            messageTemplates.Add(new MessageTemplate() { ID = 3, Name = "Bongo", Race = "Brun Labrador" });

        }
        */
    }
}