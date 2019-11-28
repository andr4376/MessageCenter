using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Word;

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
            //Make sure temp folder exist
            FileManager.Instance.CreateDirectoryIfNotExists(MessageHandler.Instance.GetTempFilesPath());

            Utility.WriteLog("Creating Temporary file for attachment with id:" + Id + " full filepath:" + FilePath);

            //Create a temporary file for this attachment
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

        /// <summary>
        /// Finds MessageVariables withing a word document and replaces them with customer / employee data
        /// </summary>
        private void ReplaceWordDocText()
        {
            Utility.WriteLog("Replacing all message variables within the attachment " + FileName + " (Word Doc)");
          
            //Create a new word file
            Microsoft.Office.Interop.Word.Application openedWordDoc = new Microsoft.Office.Interop.Word.Application();

            //Load this attachment's word document into the open file with writing access
            Microsoft.Office.Interop.Word.Document document = openedWordDoc.Documents.Open(FilePath, ReadOnly: false);
               
            //Activate the document to allow editing
            document.Activate();
                        
            foreach (KeyValuePair<MESSAGE_VARIABLES, string> variable in MessageHandler.GetMessageVariables)
            {
                //Fx. "[customerFullName]"
                string value = MessageHandler.Instance.GetValueFromMessageVariable(variable.Key);

                if (value == string.Empty)
                {
                    continue;
                }
                //Fx. Replace "[customerFullName]" with "Jane Doe" in the opened word Document
                WordDocReplaceText(openedWordDoc, MessageHandler.GetMessageVariable(variable.Key), value);

            }

            //Overwrite existing temp file
            document.SaveAs(filePath);

            //Close the word application
            openedWordDoc.Quit();

            Utility.WriteLog("All message variables within the attachment " + FileName + " (Word Doc) has been replaced");

        }

        public string GetWordDocAsHtml()
        {

            //TODO:
            if (FileType!="docx")
            {
                return string.Empty;
            }

            object documentFormat = 8;
            string randomName = DateTime.Now.Ticks.ToString();
            object htmlFilePath = FilePath.Replace("docx", "html");
            object fileSavePath = FilePath;

                    
            //Open the word document in background.
            _Application applicationclass = new Application();
            applicationclass.Documents.Open(ref fileSavePath);
            applicationclass.Visible = false;
            Document document = applicationclass.ActiveDocument;

            //Save the word document as HTML file.
            document.SaveAs(ref htmlFilePath, ref documentFormat);

            //Close the word document.
            document.Close();

            //Read the saved Html File.
            string wordHTML = System.IO.File.ReadAllText(htmlFilePath.ToString());

            string tempPath = "TempFiles/Besked om økonomiske vanskligheder nr.0_T210672";
            //Loop and replace the Image Path.
            foreach (Match match in Regex.Matches(wordHTML, "<v:imagedata.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase))
            {
                wordHTML = Regex.Replace(wordHTML, match.Groups[1].Value, tempPath + "/" + match.Groups[1].Value);
            }


            return wordHTML;

            
        }

        /// <summary>
        /// Finds and Replaces text with new text in the exact same format
        /// </summary>
        /// <param name="opnenedWordDocument">The word document to edit</param>
        /// <param name="textToReplace">The text to be replaced</param>
        /// <param name="newText">The text you wish to replace 'textToReplace' with</param>
        private void WordDocReplaceText(Microsoft.Office.Interop.Word.Application opnenedWordDocument, object textToReplace, object newText)
        {
            //all values are stored as local object variables, because the Execute method requires object references in its parameters
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
          

            opnenedWordDocument.Selection.Find.Execute(ref textToReplace, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref newText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }


    }
}
/*
 * fileData = System.IO.File.ReadAllBytes("D:\\skole\\Hovedopgave\\Project\\MessageCenter\\MessageCenter\\MessageCenter\\Images\\andreas.jpg");

            File.WriteAllBytes(FileManager.Instance.GetFilePath("testBillede.jpg"), fileData);
*/
