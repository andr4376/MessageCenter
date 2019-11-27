using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop;


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

        /// <summary>
        /// Returns the type of the file fx. 'docx', 'png', ect.
        /// </summary>
        public string FileType
        {
            get
            {
                return this.FileName.Split('.')[1];
            }
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
            return new MessageAttachment(2, 1, "testDocument.docx",
                System.IO.File.ReadAllBytes(FileManager.Instance.GetFilePath("Developement\\testDocument.docx")));
        }

        public void InsertData()
        {
            
            switch (this.FileType)
            {
                case "docx":
                    ReplaceWordDocText();
                    break;
                default:
                    Utility.WriteLog("File type " + this.FileType + " is not supported for data insertion");
                    break;
            }

        }

        private void ReplaceWordDocText()
        {
            //TODO:
            //Create a new microsoft word file

            Microsoft.Office.Interop.Word.Application fileOpen = new Microsoft.Office.Interop.Word.Application();

            //Open a already existing word file into the new document created
            Microsoft.Office.Interop.Word.Document document = fileOpen.Documents.Open(FilePath, ReadOnly: false);

           
           //fileOpen.Visible = true;

            document.Activate();

            foreach (KeyValuePair<MESSAGE_VARIABLES, string> variable in MessageHandler.GetMessageVariables)
            {
                string value = MessageHandler.Instance.GetValueFromMessageVariable(variable.Key);

                if (value == string.Empty)
                {
                    continue;
                }
               
                FindAndReplace(fileOpen, MessageHandler.GetMessageVariable(variable.Key), value);

            }


            document.SaveAs(filePath);
            //Close the file out
            fileOpen.Quit();

        }
        //Method to find and replace the text in the word document. Replaces all instances of it
        private void FindAndReplace(Microsoft.Office.Interop.Word.Application fileOpen, object findText, object replaceWithText)
        {
            object matchCase = false;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;
            //execute find and replace
            fileOpen.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }


    }
}
/*
 * fileData = System.IO.File.ReadAllBytes("D:\\skole\\Hovedopgave\\Project\\MessageCenter\\MessageCenter\\MessageCenter\\Images\\andreas.jpg");

            File.WriteAllBytes(FileManager.Instance.GetFilePath("testBillede.jpg"), fileData);
*/
