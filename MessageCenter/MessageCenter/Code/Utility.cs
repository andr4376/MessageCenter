using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MessageCenter.Code
{
    /// <summary>
    /// Returkoder, som returneres af mange metoder for at beskrive om metoden fuldførte sit job. 
    /// </summary>
    public enum StatusCode {OK,FORHINDRING,ERROR};

    public static class Utility
    {
        /// <summary>
        /// Calls the JS function "alert" and prints the input text to the userinterface in a message box
        /// </summary>
        /// <param name="message">The warning message to be printed</param>
        public static void PrintWarningMessage(string message)
        {
            System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"" + message + "\")</SCRIPT>");           
        }

        /// <summary>
        /// Converts a list of MessageTemplates into a Dictionary containing "ID" and "Title"
        /// </summary>
        /// <param name="list">The list of MessageTemplates to be converted</param>
        /// <returns></returns>
        public static Dictionary<string,string> ConvertTemplateListToDictionary(List<MessageTemplate> list)
        {
            Dictionary<string, string> messagesDictionary = new Dictionary<string, string>();

            foreach (MessageTemplate message in list)
            {
                messagesDictionary.Add(message.Id.ToString(), message.Title);

            }
            return messagesDictionary;
        }

        /// <summary>
        /// Converts a list of customers into a dictionary to be displayed in a listbox
        /// </summary>
        /// <param name="list">the list of customers to convert.</param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertCustomerListToDictionary(List<Customer> list)
        {
            Dictionary<string, string> messagesDictionary = new Dictionary<string, string>();

            foreach (Customer customer in list)
            {
                messagesDictionary.Add(customer.Cpr, customer.Cpr+" - "+customer.FirstName+" "+customer.LastName);

            }
            return messagesDictionary;
        }


        public static void WriteLog(string textToLog)
        {
            System.Diagnostics.Debug.WriteLine(textToLog);
        }

    }

  
}