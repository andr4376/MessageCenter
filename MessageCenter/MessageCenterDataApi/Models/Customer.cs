using MessageCenterDataApi.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenterDataApi.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }
        public string Cpr { get; set; }
        public string Advisor { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        private static List<Customer> customers;

        public static List<Customer> GetCustomers
        {
            get
            {
                if (customers == null)
                {
                    GemerateTestCustomers();
                }
                return customers;
            }
        }

        public static void GemerateTestCustomers()
        {
            List<Customer> tmpCustomers = new List<Customer>();

            Employee customersAdvisor = Employee.GetRandomTUser();

            tmpCustomers.Add(new Customer()
            {
                FirstName = "Andreas",
                LastName = "Kirkegaard Jensen",
                Birthday = "26-01-1994",
                Cpr = "2601941751",
                Advisor = "T210672",
                Department = customersAdvisor.Department,
                Email = "andr4376@gmail.com",
                PhoneNumber = "40965001"
            });


            tmpCustomers.Add(new Customer()
            {
                FirstName = "Louise",
                LastName = "Larsen",
                Birthday = "21-02-1992",
                Cpr = "2102921346",
                Advisor = "T210672",
                Department = customersAdvisor.Department,
                Email = "Louisemusen123@gmail.com",
                PhoneNumber = "23458978"
            });

            customersAdvisor = Employee.GetRandomTUser();
            tmpCustomers.Add(new Customer()
            {
                FirstName = "Hansi",
                LastName = "Hinterseer",
                Birthday = "13-06-1954",
                Cpr = "1306541349",
                Advisor = customersAdvisor.Tuser,
                Department = customersAdvisor.Department,
                Email = "Hansi.official@hansi.de",
                PhoneNumber = "31649764"
            });

            customersAdvisor = Employee.GetRandomTUser();
            tmpCustomers.Add(new Customer()
            {
                FirstName = "Test",
                LastName = "Testesen",
                Birthday = "02-02-1922",
                Cpr = "02022202",
                Advisor = customersAdvisor.Tuser,
                Department = customersAdvisor.Department,
                Email = "Test.Testesen@test.test",
                PhoneNumber = "87651232"
            });

            for (int i = 0; i < 150; i++)
            {
                customersAdvisor = Employee.GetRandomTUser();

                string[] names = PersonGenerator.GetName();
                string[] bDayAndCpr = PersonGenerator.GetRandomBirthdayAndCpr();
                tmpCustomers.Add(new Customer()
                {
                    FirstName = names[0],
                    LastName = names[1]+" "+ names[2],
                    Birthday = bDayAndCpr[0],
                    Cpr = bDayAndCpr[1],
                    Advisor = customersAdvisor.Tuser,
                    Department = customersAdvisor.Department,
                    Email = "andr4376@gmail.com",
                    PhoneNumber = "40965001"
                });
            }

            customers = tmpCustomers;
        }

        public static string GenerateCreateTableCommand(string tableName)
        {
            string command =
                        "Create table " + tableName + " " +
                      "(Id integer primary key," +
                      "FirstName varchar," +
                      "LastName varchar," +
                      "Birthday varchar," +
                      "Cpr varchar," +
                      "Advisor varchar," +
                      "Department varchar," +
                      "Email varchar," +
                      "PhoneNumber varchar)";

            return command;
        }
    }
}