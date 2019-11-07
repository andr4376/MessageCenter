using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    public class SignIn
    {



        /// <summary>
        /// Hashes the password using SHA (Secure Hash Algorithm)
        /// </summary>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static string EncryptPassword(string passWord)
        {
            byte[] encodedPassword = System.Text.Encoding.ASCII.GetBytes(passWord);
            encodedPassword = new System.Security.Cryptography.SHA256Managed().ComputeHash(encodedPassword);
            return System.Convert.ToBase64String(encodedPassword);
        }
    }
}