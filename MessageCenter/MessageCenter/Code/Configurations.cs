using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace MessageCenter.Code
{

  public  enum CONFIGURATION_NAME {SUPPORT_EMAIL,
        DB_FILE_NAME,
        MESSAGE_TEMPLATE_TABLE_NAME,
        ATTACHMENTS_TABLE_NAME,
        API_URL }

    public static class Configurations
    {
        private static XmlDocument configurationsFile;

        private static Dictionary<CONFIGURATION_NAME, string> configDictionary;


        private static void SetupConfigurations()
        {
            configurationsFile = new XmlDocument();
            string tmp = FileManager.Instance.GetFilePath("MessageCenterConfigurations.xml");
            configurationsFile.Load(tmp);

            configDictionary = new Dictionary<CONFIGURATION_NAME, string>()
            {
                {CONFIGURATION_NAME.API_URL,"apiUrl" },
                {CONFIGURATION_NAME.ATTACHMENTS_TABLE_NAME,"attachmentsTableName" },
                {CONFIGURATION_NAME.DB_FILE_NAME,"dbFileName" },
                {CONFIGURATION_NAME.MESSAGE_TEMPLATE_TABLE_NAME,"messageTemplateTableName" },
                {CONFIGURATION_NAME.SUPPORT_EMAIL,"supportEmail" }
            };
            
    
        }

        public static string GetConfigurationsValue(CONFIGURATION_NAME configurationType)
        {
            if (configurationsFile == null)
            {
                SetupConfigurations();

            }

            XmlNodeList xmlNodes = configurationsFile.DocumentElement.SelectNodes("/configurations/" + configDictionary[configurationType]);
            foreach (XmlNode xmlNode in xmlNodes)
            {
                return xmlNode.InnerText;
            }

            Utility.PrintWarningMessage("Fejl ved indlæsning af app konfigurationer - kontakt venligt teknisk support");
            Utility.WriteLog("the xml node '" + configDictionary[configurationType] + "' does not exist!");

            return string.Empty;
        }
    }
}