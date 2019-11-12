using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace MessageCenter.Code
{
    public class SignIn
    {
        private static SignIn instance;

        private  Employee user;

        public Employee User
        {
            get { return user; }
        }

        public static SignIn Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SignIn();
                }
                return instance;
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return User != null;
            }
        }

        public ReturnCode LogIn(string tUser, string passWord)
        {
            ReturnCode returnCode = ReturnCode.ERROR;

            try
            {
                this.user = ApiManager.Instance.MakeRestCall<Employee>
                (ApiManager.getEmployeeFromCredentials
                + tUser.ToUpper() + "/"
                + EncryptPassword(passWord))[0];

                if (User == null)
                {
                    returnCode = ReturnCode.FORHINDRING;
                    Utility.WriteLog("Ingen medarbejder fundet med disse login oplysninger!");
                }
                else
                {
                    Utility.WriteLog("login: " + user.Tuser + " - " + user.FirstName + " " + user.LastName);
                    returnCode = ReturnCode.OK;
                }
            }
            catch (Exception)
            {
                Utility.WriteWarningMessage("Api Exception! kunne lave kald for at finde loginbruger");                
            }            

            
            return returnCode;
        }

        /// <summary>
        /// Hashes the password using SHA (Secure Hash Algorithm)
        /// </summary>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static string EncryptPassword(string passWord)
        {
            byte[] encodedPassword = System.Text.Encoding.ASCII.GetBytes(passWord);
            encodedPassword = new System.Security.Cryptography.SHA256Managed().ComputeHash(encodedPassword);
                       
            return System.Convert.ToBase64String(encodedPassword).Replace('/', '_').Replace('+', 'q').Replace('=', 'a');
        }

        public override string ToString()
        {
            return this.user.FirstName + " " + this.user.LastName;
        }


    }
}