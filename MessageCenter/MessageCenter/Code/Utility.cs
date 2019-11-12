using System;
using System.Collections.Generic;
using System.Linq;


namespace MessageCenter.Code
{
    /// <summary>
    /// Returkoder, som returneres af mange metoder for at beskrive om metoden fuldførte sit job. 
    /// </summary>
    public enum ReturnCode {OK,FORHINDRING,ERROR};

    public static class Utility
    {
        public static void WriteWarningMessage(string message)
        {
            System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=\"\"JavaScript\"\">alert(\"" + message + "\")</SCRIPT>");           
        }

        public static Dictionary<string,string> ConvertTemplateListToDictionary(List<MessageTemplate> list)
        {
            Dictionary<string, string> messagesDictionary = new Dictionary<string, string>();

            foreach (MessageTemplate message in list)
            {
                messagesDictionary.Add(message.Id.ToString(), message.Title);

            }
            return messagesDictionary;
        }

        public static void WriteLog(string textToLog)
        {
            System.Diagnostics.Debug.WriteLine(textToLog);
        }

    }

  
}