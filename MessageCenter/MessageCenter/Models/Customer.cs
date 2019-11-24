using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Models
{
    public class Customer : IModel
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

        public int Age
        {
            get
            {
                DateTime birthDate = Convert.ToDateTime(Birthday);

                return (int)(DateTime.Now - birthDate).TotalDays / 365;                   
            }
        }


        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

    }
}