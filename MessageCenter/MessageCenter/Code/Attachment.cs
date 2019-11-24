using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{
    public class MessageAttachment
    {
        public int Id
        {
            get;
            private set;
        }

        public int MessageTemplateId
        {
            get;
            private set;
        }

        public string FileName
        {
            get;
            private set;
        }


        public byte[] FileData
        {
            get;
            private set;
        }

        string filePath;
        public string FilePath
        {
            get
            {
                return filePath = MessageHandler.Instance.GetTempFilesPath() + "\\" + FileName;
               
            }

        }

        public MessageAttachment(int _id, int _messageTemplateId, string _fileName, byte[] _fileData)
        {
            this.Id = _id;
            this.MessageTemplateId = _messageTemplateId;
            this.FileName = _fileName;
            this.FileData = _fileData;

        }

        public void CreateTempFile()
        {
            FileManager.Instance.CreateDirectoryIfNotExists(MessageHandler.Instance.GetTempFilesPath());

            Utility.WriteLog("Creating Temporary file for attachment with id:" + Id + " full filepath:" + FilePath);

            FileManager.Instance.CreateFile(FilePath, FileData);
        }

        public MessageAttachment(string filePath)
        {
            //TODO: uploaded files to bytes
            FileData = File.ReadAllBytes(filePath);

        }

        public static MessageAttachment GetTestAttachment()
        {
            return new MessageAttachment(1, 1, "testAttachment.jpg",
                System.IO.File.ReadAllBytes(FileManager.Instance.GetImageDirectory() + "andreas.jpg"));
        }

    }
}
/*
 * fileData = System.IO.File.ReadAllBytes("D:\\skole\\Hovedopgave\\Project\\MessageCenter\\MessageCenter\\MessageCenter\\Images\\andreas.jpg");

            File.WriteAllBytes(FileManager.Instance.GetFilePath("testBillede.jpg"), fileData);
*/
