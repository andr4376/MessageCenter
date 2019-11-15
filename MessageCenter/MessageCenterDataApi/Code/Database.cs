using MessageCenterDataApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MessageCenterDataApi.Code
{
    public class Database
    {
        private static Database instance;

        private static string dbPath = "Database.db";

        private static string customerTableName = "Customers";
        private static string employeeTableName = "Employees";
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
            string appdatafolder = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data");
            dbPath = appdatafolder + "\\" + dbPath;

            DBConnect = new SQLiteConnection("Data source = " + dbPath + "; Version = 3; ");

            SetupDbFile();



        }

        private void SetupDbFile()
        {
            if (!(File.Exists(dbPath)))
            {
                SQLiteConnection.CreateFile(dbPath);
                System.Diagnostics.Debug.WriteLine("Db file created @" + dbPath);
                CreateTables();

                PopulateDBWithTestCustomers();
                PopulateDBWithTestEmployees();

            }
        }

        private void CreateTables()
        {

            ExecuteSQLiteNonQuery(Customer.GenerateCreateTableCommand(customerTableName));

            ExecuteSQLiteNonQuery(Employee.GenerateCreateTableCommand(employeeTableName));

        }

        private void PopulateDBWithTestCustomers()
        {
            foreach (Customer customer in Customer.GetCustomers)
            {
                string command =
               "insert into " + customerTableName + " values (null," +
               "'" + customer.FirstName + "'," +
               "'" + customer.LastName + "'," +
               "'" + customer.Birthday + "'," +
               "'" + customer.Cpr + "'," +
               "'" + customer.Advisor + "'," +
               "'" + customer.Department + "'," +
               "'" + customer.Email + "'," +
               "'" + customer.PhoneNumber + "')";

                ExecuteSQLiteNonQuery(command);
            }

        }

        private void PopulateDBWithTestEmployees()
        {
            foreach (Employee employee in Employee.GetEmployees)
            {
                string command =
               "insert into " + employeeTableName + " values (" +
               "'" + employee.Tuser + "'," +
               "'" + employee.FirstName + "'," +
               "'" + employee.LastName + "'," +
               "'" + employee.Birthday + "'," +
               "'" + employee.Cpr + "'," +
               "'" + employee.Department + "'," +
               "'" + employee.Email + "'," +
               "'" + employee.PhoneNumber + "'," +
               "'" + employee.PassWord + "')";

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
            SQLiteCommand Command = new SQLiteCommand("select * from " + customerTableName + ";", DBConnect);

            try
            {
                using (SQLiteDataReader dataReader = Command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        tmpCustomer = new Customer();

                        tmpCustomer = ExtractCustomerData(dataReader);

                        listOfCustomers.Add(tmpCustomer);


                    }
                }
            }
            catch (Exception)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af alle kunder!");
                throw;
#endif
            }

            DBConnect.Close();
            return listOfCustomers;
        }
        public List<Employee> GetAllEmployees()
        {
            List<Employee> listOfEmployees = new List<Employee>();
            Employee tmpEmployee = null;

            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand("select * from " + employeeTableName + ";", DBConnect);

            try
            {
                using (SQLiteDataReader dataReader = Command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        tmpEmployee = new Employee();

                        tmpEmployee = ExtractEmployeeData(dataReader);

                        listOfEmployees.Add(tmpEmployee);


                    }
                }
            }
            catch (Exception)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af alle medarbejdere!");
                throw;
#endif
            }

            DBConnect.Close();
            return listOfEmployees;
        }

        public Customer GetCustomer(string cpr)
        {
            Customer tmpCustomer = null;

            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand("select * from " + customerTableName + " where Cpr = '" + cpr + "' LIMIT 1;", DBConnect);


            using (SQLiteDataReader dataReader = Command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    try
                    {
                        tmpCustomer = ExtractCustomerData(dataReader);

                    }
                    catch (Exception)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af kunde med cpr: " + cpr);
                        throw;
#endif
                    }

                }
            }


            DBConnect.Close();
            return tmpCustomer;
        }
        public List<Customer> GetCustomersFromAdvisor(string tuser)
        {

            List<Customer> listOfCustomers = new List<Customer>();
            Customer tmpCustomer = null;

            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand("select * from " + customerTableName + " where Advisor = '" + tuser.ToUpper() + "'" +
                "ORDER BY firstName ASC, lastName ASC;", DBConnect);


            using (SQLiteDataReader dataReader = Command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    try
                    {
                        tmpCustomer = ExtractCustomerData(dataReader);
                        listOfCustomers.Add(tmpCustomer);
                    }
                    catch (Exception)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af kunder med rådgiveren: " + tuser);
                        throw;
#endif
                    }

                }
            }


            DBConnect.Close();
            return listOfCustomers;
        }
        public Employee GetEmployee(string tuser)
        {
            Employee tmpEmployee = null;


            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand("select * from " + employeeTableName + " where TUser = '" + tuser + "' LIMIT 1;", DBConnect);


            using (SQLiteDataReader dataReader = Command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    try
                    {
                        tmpEmployee = ExtractEmployeeData(dataReader);

                    }
                    catch (Exception)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af medarbejder med cpr: " + tuser);
                        throw;
#endif
                    }

                }
            }


            DBConnect.Close();
            return tmpEmployee;
        }
        public Employee GetEmployee(string tuser, string password)
        {
            Employee tmpEmployee = null;


            DBConnect.Open();
            SQLiteCommand Command = new SQLiteCommand("select * from " + employeeTableName + " where TUser = '" + tuser + "'" +
                "AND password= '"+password+"' LIMIT 1;", DBConnect);


            using (SQLiteDataReader dataReader = Command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    try
                    {
                        tmpEmployee = ExtractEmployeeData(dataReader);

                    }
                    catch (Exception)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("ERROR! fejl ved udhentning af medarbejder med cpr: " + tuser);
                        throw;
#endif
                    }

                }
            }


            DBConnect.Close();
            return tmpEmployee;
        }
        private Customer ExtractCustomerData(SQLiteDataReader dataReader)
        {
            Customer tmpCustomer = new Customer();

            tmpCustomer.Id = dataReader.GetInt32(0);
            tmpCustomer.FirstName = dataReader.GetString(1);
            tmpCustomer.LastName = dataReader.GetString(2);
            tmpCustomer.Birthday = dataReader.GetString(3);
            tmpCustomer.Cpr = dataReader.GetString(4);
            tmpCustomer.Advisor = dataReader.GetString(5);
            tmpCustomer.Department = dataReader.GetString(6);
            tmpCustomer.Email = dataReader.GetString(7);
            tmpCustomer.PhoneNumber = dataReader.GetString(8);

            return tmpCustomer;
        }

        private Employee ExtractEmployeeData(SQLiteDataReader dataReader)
        {
            Employee tmpEmployee = new Employee();

            tmpEmployee.Tuser = dataReader.GetString(0);
            tmpEmployee.FirstName = dataReader.GetString(1);
            tmpEmployee.LastName = dataReader.GetString(2);
            tmpEmployee.Birthday = dataReader.GetString(3);
            tmpEmployee.Cpr = dataReader.GetString(4);
            tmpEmployee.Department = dataReader.GetString(5);
            tmpEmployee.Email = dataReader.GetString(6);
            tmpEmployee.PhoneNumber = dataReader.GetString(7);
            tmpEmployee.PassWord = dataReader.GetString(8);

            return tmpEmployee;
        }

    }
}