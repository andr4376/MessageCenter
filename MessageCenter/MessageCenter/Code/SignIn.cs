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

        public List<Customer> MyCustomers
        {
            get
            {
                if (IsLoggedIn)
                {
                    return ApiManager.Instance.MakeRestCall<Customer>(
                        Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.GET_CUSTOMER_FROM_ADVISOR_TUSER_API_PARAMETERS)
                        + user.Tuser);
                }
                return null;
            }
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

        public StatusCode LogIn(string tUser, string passWord)
        {
            StatusCode returnCode = StatusCode.ERROR;

            try
            {
                this.user = ApiManager.Instance.MakeRestCall<Employee>
                (Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.GET_EMPLOYEE_FROM_CREDENTIALS_API_PARAMETERS)
                + tUser.ToUpper() + "/"
                + EncryptPassword(passWord))[0];

                if (User == null)
                {
                    returnCode = StatusCode.FORHINDRING;
                    Utility.WriteLog("Ingen medarbejder fundet med disse login oplysninger!");
                }
                else
                {
                    Utility.WriteLog("login: " + user.Tuser + " - " + user.FirstName + " " + user.LastName);
                    returnCode = StatusCode.OK;
                }
            }
            catch (Exception)
            {
                Utility.PrintWarningMessage("Api Exception! kunne lave kald for at finde loginbruger");                
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