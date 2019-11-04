using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenterDataApi.Models
{
    public class Tusers
    {
        public string Tuser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }
        public string Cpr { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        private static Random rnd;

        private static List<Tusers> tUsers;

        public static List<Tusers> GetTusers
        {
            get
            {
                if (tUsers == null)
                {
                    GenerateTusers();
                }
                return tUsers;
            }
        }

        /// <summary>
        /// returns a random TUser
        /// </summary>
        /// <returns></returns>
        public static Tusers GetRandomTUser()
        {
            if (rnd == null) { rnd = new Random(); }

            int rndIndex = rnd.Next(0, GetTusers.Count);

            Tusers t = GetTusers[rndIndex];

            System.Diagnostics.Debug.WriteLine(t.FirstName + " " + t.LastName + " was randomly chosen (index: " + rndIndex + ")");

            return t;
        }

        /// <summary>
        /// Generates Tusers (employees) with believable data
        /// </summary>
        private static void GenerateTusers()
        {
            tUsers = new List<Tusers>();

            tUsers.Add(
                new Tusers()
                {
                    Tuser = "T210672",
                    FirstName = "Knud",
                    LastName = "Andersen",
                    Birthday = "26/06/1965",
                    Cpr = "2606651245",
                    Department = "Randers",
                    Email = "kna@sparkron.dk",
                    PhoneNumber = "26564578"

                });
            tUsers.Add(
                new Tusers()
                {
                    Tuser = "T200454",
                    FirstName = "Svend-Erik",
                    LastName = "Hammershoej",
                    Birthday = "26/06/1987",
                    Cpr = "2606871245",
                    Department = "Spentrup",
                    Email = "Seh@sparkron.dk",
                    PhoneNumber = "20516498"

                });
            tUsers.Add(
               new Tusers()
               {
                   Tuser = "T196543",
                   FirstName = "Bjarke",
                   LastName = "Hansemann",
                   Birthday = "22/12/1983",
                   Cpr = "2212831346",
                   Department = "Tronholmen",
                   Email = "Bhm@sparkron.dk",
                   PhoneNumber = "4051463125"

               });

            tUsers.Add(
              new Tusers()
              {
                  Tuser = "T210345",
                  FirstName = "Bo",
                  LastName = "Risom Bitzer",
                  Birthday = "22/12/1976",
                  Cpr = "2212763164",
                  Department = "Processer & Effektivisering",
                  Email = "Bori@sparkron.dk",
                  PhoneNumber = "55443187"

              });
        }
    }
}