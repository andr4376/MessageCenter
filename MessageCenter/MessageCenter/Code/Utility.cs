using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MessageCenter.Code
{
    /// <summary>
    /// Returkoder, som returneres af mange metoder for at beskrive om metoden fuldførte sit job. 
    /// </summary>
    public enum StatusCode { OK, FORHINDRING, ERROR };

    public static class Utility
    {
        /// <summary>
        /// Calls the JS function "alert" and prints the input text to the userinterface in a message box
        /// </summary>
        /// <param name="message">The warning message to be printed</param>
        public static void PrintWarningMessage(string message)
        {
            System.Web.HttpContext.Current.Response.Write
                ("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"" + message + "\")</SCRIPT>");
        }


        /// <summary>
        /// Converts a list of customers into a dictionary to be displayed in a listbox
        /// </summary>
        /// <param name="list">the list of customers to convert.</param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertCustomerListToDictionary(List<Customer> list)
        {
            Dictionary<string, string> customerDictionary = new Dictionary<string, string>();

            foreach (Customer customer in list)
            {
                customerDictionary.Add(
                    customer.Cpr, //key
                    customer.Cpr + " - " + customer.FirstName + " " + customer.LastName);//value

            }
            return customerDictionary;
        }


        public static void WriteLog(string textToLog)
        {
            System.Diagnostics.Debug.WriteLine(textToLog);

        }
    }





}