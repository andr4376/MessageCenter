using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;

namespace MessageCenter.Code
{
    /// <summary>
    /// A singleton class that handles files and directories
    /// </summary>
    public class FileManager
    {
        private static FileManager instance;

        private string appDataPath;

        private string applicationPath;



        /// <summary>
        /// Returns the path to the database file
        /// </summary>
        public string DbFile
        {
            get
            {
                return GetFilePath(Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.DB_FILE_NAME));
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
            // applicationPath = HttpContext.Current.Request.PhysicalApplicationPath;

            applicationPath = Path.GetFullPath(HttpContext.Current.Server.MapPath("~"));
            appDataPath = Path.GetFullPath(HttpContext.Current.Server.MapPath("~/App_Data"));


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
            return Path.Combine(applicationPath, "Images\\");
        }

        /// <summary>
        /// Deletes all directories which name contains the input tuser 
        /// </summary>
        /// <param name="tuser"></param>
        public void DeleteAllDirectoriesContainingTUser(string tuser)
        {
            //get all the directories
            IEnumerable<string> directoriesContainingTUser = Directory.EnumerateDirectories
                (appDataPath + ("\\TempFiles\\"), "*", SearchOption.AllDirectories).
                Where(x => x.Contains(tuser));

            //Delete all found directories
            foreach (string directory in directoriesContainingTUser)
            {
                DeleteDirectory(directory);
            }
        }

        /// <summary>
        /// returns a generated directory name based on the current message and the signed in tUser
        /// </summary>
        /// <param name="message"></param>
        /// <param name="employeeTUser"></param>
        /// <returns></returns>
        public string GetTempDirectory(MessageTemplate message, string employeeTUser)
        {
            string directoryPath = appDataPath + ("\\TempFiles\\" + message.PathName + "_" + employeeTUser);

            return directoryPath;
        }

        public void DeleteDirectory(string path)
        {
            lock (MessageHandler.attachmentsKey)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }

        }



        public void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

        }

        public StatusCode CreateFile(string name, byte[] data)
        {
            try
            {
                File.WriteAllBytes(name, data);
            }
            catch (Exception e)
            {
                Utility.WriteLog("ERROR in 'FileManager.CreateFile': " + e.ToString());
                Utility.PrintWarningMessage("Der er opstået en fejl ved oprettelse af midlertidlige filer - kontakt venligst teknisk support: " +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));

                return StatusCode.ERROR;
            }
            return StatusCode.OK;
        }

        /// <summary>
        /// Deletes the file with the given path
        /// </summary>
        /// <param name="filePath"></param>
        public void DeleteFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            //Alternate parameterized thread
            Thread fileDeleteThread = new Thread(
                () => DeleteFileWhenNotInUse(filePath)
                )
            { IsBackground = true };
            fileDeleteThread.Start();

        }

        /// <summary>
        /// Attempts to delete a file untill succesful or failed due to another process still using the file after repeated attempts
        /// </summary>
        /// <param name="filePath">File to be deleted</param>
        private void DeleteFileWhenNotInUse(string filePath)
        {
            bool fileIsDeleted = false;

            //Incremented every time the cycle the file is still in use
            //or when File.Delete fails - (it hasn't failed yet, it's "just in case")
            byte attemps = 0;
            byte maxAttempts = 50;
            int attemptIntervalMilliSeconds = 100;

            Utility.WriteLog("Deleting file: " + filePath);

            while (!fileIsDeleted || attemps > maxAttempts)
            {
                //If file is occupied by another process
                if (FileIsInUse(new FileInfo(filePath)))
                {
                    //Wait and try again
                    Thread.Sleep(attemptIntervalMilliSeconds);
                    attemps++;
                    continue;

                }
                //File is not occupied
                try
                {
                    //Try to delete 
                    File.Delete(filePath);

                    Utility.WriteLog(filePath + " succesfully deleted after " + attemps + " attempts");

                    //Escape while loop next cycle
                    fileIsDeleted = true;

                }
                catch (Exception e)
                {
                    attemps++;
                    Utility.WriteLog("ERROR at FileManager.DeleteFile: " + e.ToString());

                }

            }
            if (attemps > maxAttempts)
            {
                Utility.WriteLog("ERROR at FileManager.DeleteFile. The file deleting thread gave up trying after " + attemps + " attempts!");
                return;
            }
        }

        /// <summary>
        /// Returns whether or not the given file is currently in use by another process
        /// </summary>
        /// <param name="file">the filepath</param>
        /// <returns></returns>
        private  bool FileIsInUse(FileInfo file)
        {
            try
            {
                //try opening the file
                using (FileStream fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    //Success
                    fileStream.Close();
                }
            }
            catch (IOException)
            {
                //File is in use
                return true;
            }

            //file is not in use
            return false;
        }


    }
}
