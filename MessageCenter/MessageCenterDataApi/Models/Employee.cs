using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MessageCenterDataApi.Models
{
    public class Employee
    {
        public string Tuser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }
        public string Cpr { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PassWord { get; set; }

        private static Random rnd;

        private static List<Employee> employees;

        public static List<Employee> GetEmployees
        {
            get
            {
                if (employees == null)
                {
                    GenerateEmployees();
                }
                return employees;
            }
        }

        /// <summary>
        /// returns a random TUser
        /// </summary>
        /// <returns></returns>
        public static Employee GetRandomTUser()
        {
            if (rnd == null) { rnd = new Random(); }

            int rndIndex = rnd.Next(0, GetEmployees.Count);

            Employee t = GetEmployees[rndIndex];

            System.Diagnostics.Debug.WriteLine(t.FirstName + " " + t.LastName + " was randomly chosen (index: " + rndIndex + ")");

            return t;
        }

        /// <summary>
        /// Generates Tusers (employees) with believable data
        /// </summary>
        private static void GenerateEmployees()
        {
            employees = new List<Employee>();

            employees.Add(
                new Employee()
                {
                    Tuser = "T210672",
                    FirstName = "Knud",
                    LastName = "Andersen",
                    Birthday = "26-06-1965",
                    Cpr = "2606651245",
                    Department = "Randers",
                    Email = "kna@sparkron.dk",
                    PhoneNumber = "26564578",
                    PassWord = "yFjFQqyHJbMmAYIsH5rZ4Y3S_6vpMyFLfWLnZEcv1J0a"

                });
            employees.Add(
                new Employee()
                {
                    Tuser = "T200454",
                    FirstName = "Svend-Erik",
                    LastName = "Hammershoej",
                    Birthday = "26-06-1987",
                    Cpr = "2606871245",
                    Department = "Spentrup",
                    Email = "Seh@sparkron.dk",
                    PhoneNumber = "20516498",
                    PassWord = "yFjFQqyHJbMmAYIsH5rZ4Y3S_6vpMyFLfWLnZEcv1J0a"


                });
            employees.Add(
               new Employee()
               {
                   Tuser = "T196543",
                   FirstName = "Bjarke",
                   LastName = "Hansemann",
                   Birthday = "22-12-1983",
                   Cpr = "2212831346",
                   Department = "Tronholmen",
                   Email = "Bhm@sparkron.dk",
                   PhoneNumber = "4051463125",
                   PassWord = "yFjFQqyHJbMmAYIsH5rZ4Y3S_6vpMyFLfWLnZEcv1J0a"


               });

            employees.Add(
              new Employee()
              {
                  Tuser = "T210345",
                  FirstName = "Bo",
                  LastName = "Risom Bitzer",
                  Birthday = "22-12-1976",
                  Cpr = "2212763164",
                  Department = "Processer & Effektivisering",
                  Email = "Bori@sparkron.dk",
                  PhoneNumber = "55443187",
                  PassWord = "yFjFQqyHJbMmAYIsH5rZ4Y3S_6vpMyFLfWLnZEcv1J0a"


              });
        }

        public static string GenerateCreateTableCommand(string tableName)
        {
            string command =
                        "Create table " + tableName + " " +
                      "(TUser varchar primary key," +
                      "FirstName varchar," +
                      "LastName varchar," +
                      "Birthday varchar," +
                      "Cpr varchar," +                      
                      "Department varchar," +
                      "Email varchar," +
                      "PhoneNumber varchar," +
                      "PassWord varchar)";

            return command;
        }
    }
}
