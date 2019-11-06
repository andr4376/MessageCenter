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

        public List<Customer> GetAllCustomers()
        {
            List<Customer> listOfCustomers = new List<Customer>();
            Customer tmpCustomer = null;

            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand("select * from "+customerTableName+";", DBConnect);


            using (SQLiteDataReader reader = Command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tmpCustomer = new Customer();
                    try
                    {
                        tmpCustomer.Id = reader.GetInt32(0);
                        tmpCustomer.FirstName = reader.GetString(1);
                        tmpCustomer.LastName = reader.GetString(2);
                        tmpCustomer.Birthday = reader.GetString(3);
                        tmpCustomer.Cpr = reader.GetString(4);
                        tmpCustomer.Advisor = reader.GetString(5);
                        tmpCustomer.Department = reader.GetString(6);
                        tmpCustomer.Email = reader.GetString(7);
                        tmpCustomer.PhoneNumber = reader.GetString(8);

                        listOfCustomers.Add(tmpCustomer);
                    }
                    catch (Exception)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af alle kunder!");
                        throw;
#endif
                    }
                  
                }
            }


            DBConnect.Close();
            return listOfCustomers;
        }

        public Customer GetCustomer(string cpr)
        {
            Customer tmpCustomer = null;

            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand("select * from " + customerTableName + " where Cpr = "+cpr+" LIMIT 1;", DBConnect);


            using (SQLiteDataReader reader = Command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tmpCustomer = new Customer();
                    try
                    {
                        tmpCustomer.Id = reader.GetInt32(0);
                        tmpCustomer.FirstName = reader.GetString(1);
                        tmpCustomer.LastName = reader.GetString(2);
                        tmpCustomer.Birthday = reader.GetString(3);
                        tmpCustomer.Cpr = reader.GetString(4);
                        tmpCustomer.Advisor = reader.GetString(5);
                        tmpCustomer.Department = reader.GetString(6);
                        tmpCustomer.Email = reader.GetString(7);
                        tmpCustomer.PhoneNumber = reader.GetString(8);

                    }
                    catch (Exception)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af kunde med cpr: "+cpr);
                        throw;
#endif
                    }

                }
            }


            DBConnect.Close();
            return tmpCustomer;
        }


        public static List<int> ReadFromDBInt(string command, string returnValue)
        {
            List<int> returnList = new List<int>();
            SQLiteConnection DBConnect = new SQLiteConnection("Data source = " + dbPath + "; Version = 3; ");
            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand(command, DBConnect);
            SQLiteDataReader highscoreReader = Command.ExecuteReader();
            while (highscoreReader.Read())
            {
                returnList.Add(Convert.ToInt32(highscoreReader[returnValue]));
            }
            DBConnect.Close();

            return returnList;
        }

        /// <summary>
        /// returns a list of strings from DB
        /// </summary>
        /// <param name="command"></param>
        public static List<string> ReadFromDBString(string command, string returnValue)
        {
            List<string> returnList = new List<string>();
            SQLiteConnection DBConnect = new SQLiteConnection("Data source = "+dbPath+"; Version = 3; ");
            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand(command, DBConnect);
            SQLiteDataReader highscoreReader = Command.ExecuteReader();
            while (highscoreReader.Read())
            {
                returnList.Add((string)highscoreReader[returnValue]);
            }
            DBConnect.Close();

            return returnList;
        }
    }
}