using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace MessageCenter.Code
{

    public enum CONFIGURATIONS_ATTRIBUTES
    {
        SUPPORT_EMAIL,
        DB_FILE_NAME,
        MESSAGE_TEMPLATE_TABLE_NAME,
        ATTACHMENTS_TABLE_NAME,
        MESSAGE_LOG_TABLE_NAME,
        API_URL,
        GET_ALL_EMPLOYEES_API_PARAMETERS,
        GET_ALL_CUSTOMERS_API_PARAMETERS,
        GET_EMPLOYEE_FROM_TUSER_API_PARAMETERS,
        GET_CUSTOMER_FROM_CPR_API_PARAMETERS,
        GET_EMPLOYEE_FROM_CREDENTIALS_API_PARAMETERS,
        GET_CUSTOMER_FROM_ADVISOR_TUSER_API_PARAMETERS
    }

    /// <summary>
    /// A static class that handles communication with the configurations file
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// The xml file containing the stored app configurations
        /// </summary>
        private static XmlDocument configurationsFile;

        /// <summary>
        /// A Dictionary that translates the CONFIGURATIONS_ATTRIBUTES values into the name of the XML attribute     
        /// </summary>
        private static Dictionary<CONFIGURATIONS_ATTRIBUTES, string> configDictionary;

        private static readonly string xmlConfigFileName = "MessageCenterConfigurations.xml";

        /// <summary>
        /// Loads stored configurations, or creates default configurations, if none are found.
        /// </summary>
        private static void SetupConfigurations()
        {
            configurationsFile = new XmlDocument();

            //get path to config file 
            string configXmlPath = FileManager.Instance.GetFilePath(xmlConfigFileName);


            if (!File.Exists(configXmlPath)) //if config file does not exist
            {
                CreateDefaultConfigurations(configXmlPath); //create config file
            }
            configurationsFile.Load(configXmlPath); //load configurations

            //the values are the actual names of the xml nodes
            configDictionary = new Dictionary<CONFIGURATIONS_ATTRIBUTES, string>()
            {
                {CONFIGURATIONS_ATTRIBUTES.API_URL,"apiUrl" },
                {CONFIGURATIONS_ATTRIBUTES.ATTACHMENTS_TABLE_NAME,"attachmentsTableName" },
                {CONFIGURATIONS_ATTRIBUTES.DB_FILE_NAME,"dbFileName" },
                {CONFIGURATIONS_ATTRIBUTES.MESSAGE_TEMPLATE_TABLE_NAME,"messageTemplateTableName" },
                {CONFIGURATIONS_ATTRIBUTES.MESSAGE_LOG_TABLE_NAME,"messageLogTableName" },
                {CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL,"supportEmail" },
                {CONFIGURATIONS_ATTRIBUTES.GET_ALL_CUSTOMERS_API_PARAMETERS,"getAllCustomerParameters" },
                {CONFIGURATIONS_ATTRIBUTES.GET_ALL_EMPLOYEES_API_PARAMETERS,"getAllEmployeesParameters" },
                {CONFIGURATIONS_ATTRIBUTES.GET_CUSTOMER_FROM_CPR_API_PARAMETERS,"getCustomerFromCprParameters" },
                {CONFIGURATIONS_ATTRIBUTES.GET_CUSTOMER_FROM_ADVISOR_TUSER_API_PARAMETERS,"getCustomerFromAdvisor" },
                {CONFIGURATIONS_ATTRIBUTES.GET_EMPLOYEE_FROM_TUSER_API_PARAMETERS,"getEmployeeFromTUserParameters" },
                {CONFIGURATIONS_ATTRIBUTES.GET_EMPLOYEE_FROM_CREDENTIALS_API_PARAMETERS,"getEmployeeFromCredentials" }
            };


        }

        /// <summary>
        /// Creates and inserts the default configurations for the app in case the stored one gets deleted
        /// </summary>
        /// <param name="configXmlPath">The file path for the configurations file</param>
        private static void CreateDefaultConfigurations(string configXmlPath)
        {
            string defaultConfigurations =
                "<configurations>\n" +
                  "<supportEmail>andr4376@gmail.com</supportEmail>\n" +
                  "<dbFileName>database.db</dbFileName>\n" +
                  "<messageTemplateTableName>MessageTemplates</messageTemplateTableName>\n" +
                  "<messageLogTableName>MessageLog</messageLogTableName>" +
                  "<attachmentsTableName>Attachments</attachmentsTableName>\n" +
                  "<apiUrl>https://messagecenterdataapi.azurewebsites.net/api/</apiUrl>\n" +

                  //Api parametres
                  "<getAllEmployeesParameters>employees/</getAllEmployeesParameters>\n" +
                  "<getAllCustomerParameters>customers/</getAllCustomerParameters>\n" +
                  "<getEmployeeFromTUserParameters>employees/tuser/</getEmployeeFromTUserParameters>\n" +
                  "<getCustomerFromCprParameters>customers/cpr/</getCustomerFromCprParameters>\n" +
                  "<getEmployeeFromCredentials>employees/login/</getEmployeeFromCredentials>\n" +
                  "<getCustomerFromAdvisor>customers/advisor/</getCustomerFromAdvisor>\n" +
                //Admins
                "<admins>" +
                     "<Tuser>T210672</Tuser>" +
                     "<Tuser>T200454</Tuser>" +
                "</admins>" +
                "</configurations>\n";

            //Create and write to file
            File.WriteAllText(configXmlPath, defaultConfigurations);


        }


        /// <summary>
        /// Returns a string value that is stored in the configuration XML file based on the input enum 
        /// </summary>
        /// <param name="configurationType">The configuration attribute you wish to a value from</param>
        /// <returns></returns>
        public static string GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES configurationType)
        {
            //If this is the first time we read from the config file
            if (configurationsFile == null)
                SetupConfigurations();

            //Translate enum to attribute name (xml node text)
            string configurationNodeName =
                configDictionary[configurationType];

            //Get the xml attribute
            XmlNodeList xmlNodes = configurationsFile.DocumentElement.
                SelectNodes("/configurations/" + configurationNodeName);

            //Return the first value it finds, if any exists
            foreach (XmlNode xmlNode in xmlNodes)
            {
                return xmlNode.InnerText;
            }

            //ERROR! - No attribute found for the input
            Utility.PrintWarningMessage("Fejl ved indlæsning af app konfigurationer - kontakt venligt teknisk support");
            Utility.WriteLog("the xml node '" + configDictionary[configurationType] + "' does not exist!");
#if DEBUG
            throw new Exception("GetConfigurationsValue was called with an unsupported ConfigurationsType: " + configurationsFile);
#endif
            return string.Empty;
        }

        public static bool TUserIsAdmin(string tUser)
        {
            //Get the xml attribute
            XmlNodeList listOfAdminTUsers = configurationsFile.DocumentElement.SelectNodes("/configurations/admins/Tuser");

            //for each admin
            foreach (XmlNode adminTUser in listOfAdminTUsers)
            {
                if (tUser.ToUpper() == adminTUser.InnerText.ToUpper())
                {
                    return true;
                } 
            }

            return false;
        }
    }
}