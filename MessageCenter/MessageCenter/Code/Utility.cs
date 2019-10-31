using System;
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


    }

  
}