using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace MessageCenter.Code
{
    public  class FileManager
    {
        private static FileManager instance;

        public string appDataPath;


        /// <summary>
        /// The filepath to the database file
        /// </summary>
        private string dbFile = string.Empty;

        public string DbFile
        {
            get
            {
                if (dbFile==string.Empty)
                {
                    dbFile = GetFilePath(Configurations.GetConfigurationsValue(CONFIGURATION_NAME.DB_FILE_NAME));
                    
                }

                return dbFile;
            }
        }


        public static FileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileManager();
                }
                return instance;
            }
        }

        private FileManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            appDataPath = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data");           


        }

        public string GetFilePath(string filename)
        {
            return appDataPath + "\\" + filename;
        }

       


    }
}