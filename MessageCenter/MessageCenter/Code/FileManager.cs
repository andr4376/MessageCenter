using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace MessageCenter.Code
{
    /// <summary>
    /// A singleton class that handles files and directories
    /// </summary>
    public  class FileManager
    {
        private static FileManager instance;

        public string appDataPath;


        /// <summary>
        /// The filepath to the database file
        /// </summary>
        private string dbFile = string.Empty;

        /// <summary>
        /// Returns the path to the database file
        /// </summary>
        public string DbFile
        {
            get
            {
                if (dbFile==string.Empty)
                {
                    dbFile = GetFilePath(Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.DB_FILE_NAME));                    
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

        /// <summary>
        /// Returns the full file path relative to the App_Data folder
        /// </summary>
        /// <param name="filename">The file you wish to get the full path from - ex. 'database.db'</param>
        /// <returns></returns>
        public string GetFilePath(string filename)
        {
            return appDataPath + "\\" + filename;
        }

        public string GetImageDirectory()
        {
            return Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Images\\");
        }

       


    }
}