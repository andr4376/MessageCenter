using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenterDataApi.Code
{
    /// <summary>
    /// A class for generating test people to populate the db
    /// </summary>
    public class PersonGenerator
    {
        private static string[] firsNames=
        {
            "Kurt",
            "Bent",
            "Anders",
            "Erik",
            "André",
            "Birgitte",
            "Bjarke",
            "Bo",
            "Asbjørn",
            "Clause",
            "Søren",
            "Kjeld",
            "Brian",
            "Michael",
            "Heidi",
            "Stefania",
            "Mike",
            "Jens",
            "Lars",
            "Henriette",
            "Sofie",
            "Louise",
            "Bjørn",
            "Klaus",
            "Tobias"
        };

        private static string[] lastNames1 =
        {
            "Bitzer",
            "Høholdt",
            "Kirkegaard",
            "Kjær",
            "Bjerrum",
            "Bak",
            "Bertelsen",
            "Damgaard",
            "Dalsgaard",
            "Gade",
            "Holm",
            "Hedegaard",
            "Hald",
            "Juhl",
            "Johnson",
            "Iversen",
            "Bjerre"
        };
        private static string[] lastNames2 =
        {
            "Jensen",
            "Hansen",
            "Eriksen",
            "Christiansen",
            "Kristensen",
            "Hinterseer",
            "Klausen",
            "Hougaard",
            "Johannesen",
            "Jørgensen"
        };

        private static Random rnd = new Random();

        public static string[] GetName()
        {
            string[] name = new string[3];

            name[0] = firsNames[rnd.Next(0, firsNames.Length)];
            name[1] = lastNames1[rnd.Next(0, lastNames1.Length)];

            if (rnd.Next(0,3)!=0)
            {
                name[2] = lastNames2[rnd.Next(0, lastNames2.Length)];

            }
            else
            {
                name[2] = "";
            }

            return name;
        }


        public static string[] GetRandomBirthdayAndCpr()
        {
            DateTime startDate = new DateTime(1950, 1, 1);
            int range = (DateTime.Today - startDate).Days;
            DateTime RandomDate= startDate.AddDays(rnd.Next(range));

            string[] data = new string[2];
            data[0] = RandomDate.ToString("dd/MM/yyyy");
            data[1] = RandomDate.ToString("ddMMyy") + rnd.Next(1000, 9999 + 1).ToString();

            return data;
        }
    }
}