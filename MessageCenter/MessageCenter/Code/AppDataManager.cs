using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    public  class AppDataManager
    {
        private static AppDataManager instance;

        public string appDataPath;

        /// <summary>
        /// A
        /// </summary>
        private string dbFile;

        public string DbFile
        {
            get
            {
                return dbFile;
            }
        }


        public static AppDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppDataManager();
                }
                return instance;
            }
        }

        private AppDataManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            appDataPath = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data");

            dbFile = appDataPath+ "\\" + DatabaseManager.dbFileName;

        }


    }
}