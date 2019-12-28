using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;

namespace MessageCenter.Code
{
    public class SignIn
    {

        private  Employee user;



        public Employee User
        {
            get { return user; }
            set { user = value;
               
            }
        }

        public List<Customer> MyCustomers
        {
            get
            {
                if (IsLoggedIn)
                {
                    return new ApiCaller().GetDataFromApi<Customer>(
                        Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.GET_CUSTOMER_FROM_ADVISOR_TUSER_API_PARAMETERS)
                        + User.Tuser);
                }
                return null;
            }
        }

        public static SignIn Instance
        {
            get
            {              
                HttpSessionState session = HttpContext.Current.Session;
                if (session["SignIn"] == null)
                {
                    session["SignIn"] = new SignIn();
                }
                return (SignIn)session["SignIn"];
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return User != null;
            }
        }
        public bool IsAdmin
        {
            get
            {
                if (User == null)
                {
                    return false;
                }
                return Configurations.TUserIsAdmin(User.Tuser);
            }
        }

        private SignIn()
        {

        }

        /// <summary>
        /// Attempts to log in using the input credentials and returns the status of the attempt - OK:success, Forhindring:Wrong credentials, Error:API exception
        /// </summary>
        /// <param name="tUser"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public StatusCode LogIn(string tUser, string passWord)
        {
            StatusCode returnCode = StatusCode.ERROR;

            try
            {
                User = new ApiCaller().GetDataFromApi<Employee>
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
                    Utility.WriteLog("login: " + User.Tuser + " - " + User.FirstName + " " + User.LastName);
                    returnCode = StatusCode.OK;

                    //Clean up previous temp files, in case this user had any unfinished messages.
                    FileManager.Instance.DeleteAllDirectoriesContainingTUser(User.Tuser);
                }
            }
            catch (Exception)
            {
                Utility.PrintWarningMessage("Teknisk Fejl ved login forsøg! Kontakt venligst teknisk support på: "+
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));
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
            if (this.User == null)
            {
                return null;
            }
            return this.User.FirstName + " " + this.User.LastName;
        }

     
    }

   
}