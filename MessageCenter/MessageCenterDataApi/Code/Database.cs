using MessageCenterDataApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;

namespace MessageCenterDataApi.Code
{
    public class Database
    {
        private static Database instance;

        private static string dbPath = "C:\\MessageCenterDataApi\\Database\\Database.db";

        private static string customerTableName = "Customers";

        private SQLiteConnection DBConnect;

        public static Database Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Database();
                }
                return instance;

            }
        }

        private Database()
        {
            Initialize();




        }

        private void Initialize()
        {
            DBConnect = new SQLiteConnection("Data source = " + dbPath + "; Version = 3; ");
            SetupDbFile();


        }

        private void SetupDbFile()
        {


            if (!(Directory.Exists("C:\\MessageCenterDataApi")))
            {
                Directory.CreateDirectory("C:\\MessageCenter");
            }

            if (!(Directory.Exists("C:\\MessageCenterDataApi\\Database")))
            {
                Directory.CreateDirectory("C:\\MessageCenterDataApi\\Database");
                System.Diagnostics.Debug.WriteLine("Db directories created");

            }
            if (!(File.Exists(dbPath)))
            {
                SQLiteConnection.CreateFile(dbPath);

                System.Diagnostics.Debug.WriteLine("Db file created @" + dbPath);

                CreateTables();

            }

        }

        private void CreateTables()
        {
            string sqlCommand = "DROP TABLE IF EXISTS "+customerTableName;

            ExecuteSQLiteNonQuery(sqlCommand);

            sqlCommand = "Create table " + customerTableName + " " +
                "(Id integer primary key," +
                "FirstName varchar," +
                "LastName varchar," +
                "Birthday varchar," +
                "Cpr varchar," +
                "Advisor varchar," +
                "Department varchar," +
                "Email varchar," +
                "PhoneNumber varchar)";

            ExecuteSQLiteNonQuery(sqlCommand);

            PopulateDBWithTestCustomers();
        }

        private void PopulateDBWithTestCustomers()
        {
            foreach (Customer customer in Customer.GetCustomers)
            {
                string command =
               "insert into " + customerTableName + " values (null," +
               "'"+customer.FirstName+"'," +
               "'" + customer.LastName + "'," +
               "'" + customer.Birthday + "'," +
               "'" + customer.Cpr + "'," +
               "'" + customer.Advisor+ "'," +
               "'" + customer.Department + "'," +
               "'" + customer.Email + "'," +
               "'" + customer.PhoneNumber + "')";

                ExecuteSQLiteNonQuery(command);
            }
           
        }

        public void ExecuteSQLiteNonQuery(string command)
        {
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

            }
            DBConnect.Close();
        }

    }
}