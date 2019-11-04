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

            tmpCustomers.Add(new Customer()
            {
                FirstName = "Andreas",
                LastName = "Kirkegaard Jensen",
                Birthday = "26/01/1994",
                Cpr = "2601941751",
                Advisor = Tusers.GetRandomTUser().Tuser,
                Department = "Spentrup",
                Email = "andr4376@gmail.com",
                PhoneNumber = "40965001"
            });

            tmpCustomers.Add(new Customer()
            {
                FirstName = "Louise",
                LastName = "Larsen",
                Birthday = "21/02/1992",
                Cpr = "2102921346",
                Advisor = Tusers.GetRandomTUser().Tuser,
                Department = "Grenaa",
                Email = "Louisemusen123@gmail.com",
                PhoneNumber = "23458978"
            });

            tmpCustomers.Add(new Customer()
            {
                FirstName = "Hansi",
                LastName = "Hinterseer",
                Birthday = "13/06/1954",
                Cpr = "1306541349",
                Advisor = Tusers.GetRandomTUser().Tuser,
                Department = "Hamburg",
                Email = "Hansi.official@hansi.de",
                PhoneNumber = "31649764"
            });

            tmpCustomers.Add(new Customer()
            {
                FirstName = "Test",
                LastName = "Testesen",
                Birthday = "02/02/1922",
                Cpr = "02022202",
                Advisor = Tusers.GetRandomTUser().Tuser,
                Department = "Teststrup",
                Email = "Test.Testesen@test.test",
                PhoneNumber = "87651232"
            });

            customers = tmpCustomers;
        }
    }
}