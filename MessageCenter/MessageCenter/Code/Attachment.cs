using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Word;
using DocumentFormat.OpenXml.Packaging;

namespace MessageCenter.Code
{
    public class MessageAttachment
    {
        /// <summary>
        /// The unique id of the attachment
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// The id of the message template it should be attached to
        /// </summary>
        public int MessageTemplateId
        {
            get;
            private set;
        }

        /// <summary>
        /// The file name and type as it was uploaded
        /// </summary>
        public string FileName
        {
            get;
             set;
        }

        /// <summary>
        /// the data of the file
        /// </summary>
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
                string[] fileParts = this.FileName.Split('.');
                return fileParts[fileParts.Length-1];
            }
        }



        /// <summary>
        /// The path to this file in the temp folder
        /// </summary>
        public string FilePath
        {
            get
            {
                return MessageHandler.Instance.GetTempFilesPath() + "\\" + FileName;

            }

        }

        /// <summary>
        /// Returns the path to this file in the temp folder - used in threaded methods that cant access singleton normally
        /// </summary>
        /// <param name="msgHandler"></param>
        /// <returns></returns>
        public string GetFilePath(MessageHandler msgHandler)
        {
            return msgHandler.GetTempFilesPath() + "\\" + FileName;

        }

        public MessageAttachment(string _fileName, byte[] _fileData)
        {
            this.FileName = _fileName;
            this.FileData = _fileData;


        }
        public MessageAttachment(int _id, int _messageTemplateId, string _fileName, byte[] _fileData) : this(_fileName, _fileData)
        {
            this.Id = _id;
            this.MessageTemplateId = _messageTemplateId;


        }

        /// <summary>
        /// Creates the temporary file in the temp folder
        /// </summary>
        /// <returns>a status code describing the mothod's success</returns>
        public StatusCode CreateTempFile()
        {
            //Make sure temp folder exist
            FileManager.Instance.CreateDirectoryIfNotExists(MessageHandler.Instance.GetTempFilesPath());

            Utility.WriteLog("Creating Temporary file for attachment with id:" + Id + " full filepath:" + FilePath);

            //Create a temporary file for this attachment
            return FileManager.Instance.CreateFile(FilePath, FileData);
        }

        //FOR TESTING
        public static List<MessageAttachment> GetTestAttachment()
        {
            return new List<MessageAttachment>(){

                new MessageAttachment(1, 1, "andreas.jpg",
                System.IO.File.ReadAllBytes(FileManager.Instance.GetImageDirectory()+"andreas.jpg")),

                new MessageAttachment(2, 1, "testDocument.docx",
                System.IO.File.ReadAllBytes(FileManager.Instance.GetFilePath("Developement\\testDocument.docx")))
            };
        }

        /// <summary>
        /// Inserts data from the messagehandler into this attachments content if possible
        /// </summary>
        /// <param name="messageHandler"></param>
        public void EditAttachment(MessageHandler messageHandler)
        {
            switch (this.FileType)
            {
                case "docx":
                    ReplaceWordDocText(messageHandler);

                    break;
                default:
                    Utility.WriteLog("File type " + this.FileType + " is not supported for data insertion");
                    break;
            }

        }

        /// <summary>
        /// Finds MessageVariables withing a word document and replaces them with customer / employee data
        /// </summary>
        private void ReplaceWordDocText(MessageHandler messageHandler)
        {
            Utility.WriteLog("Replacing all message variables within the attachment " + FileName);

            //Create a new word file
            Microsoft.Office.Interop.Word.Application openedWordDoc = new Microsoft.Office.Interop.Word.Application();

            //Load this attachment's word document into the open file with writing access
            Microsoft.Office.Interop.Word.Document document = openedWordDoc.Documents.Open(GetFilePath(messageHandler), ReadOnly: false);

            //Activate the document to allow editing
            document.Activate();

            try
            {
                foreach (KeyValuePair<MESSAGE_VARIABLES, string> variable in MessageHandler.GetMessageVariables)
                {
                    //Fx. "[customerFullName] => Andreas Kirkegaard Jensen"
                    string value = messageHandler.GetValueFromMessageVariable(variable.Key);

                    if (value == string.Empty)//just in case
                    {
                        continue;
                    }
                    //Fx. Replace "[customerFullName]" with "Jane Doe" in the opened word Document
                    WordDocReplaceText(openedWordDoc, MessageHandler.GetMessageVariable(variable.Key), value);

                }
            }
            catch (Exception e )
            {
                //If something goes wrong while editting the word document:
                //Close the document, so the file is not in use by this process
                document.Close();
                openedWordDoc.Quit();

                Utility.WriteLog("Error in ReplaceWordDocText: "+e.ToString());

                throw e;
            }

            //Overwrite existing temp file
            document.SaveAs(GetFilePath(messageHandler));


            //Close the word application
            document.Close();
            openedWordDoc.Quit();

            Utility.WriteLog("All message variables within the attachment " + FileName + " (Word Doc) has been replaced");

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
            //These settings will replace the text and keep the same format
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

        /// <summary>
        /// Converts a docx file to pdf
        /// </summary>
        /// <returns></returns>
        public StatusCode ConvertDocToPDF()
        {
            StatusCode convertStatus = StatusCode.OK;

            if (this.FileType == "docx")//if Word document
            {
                try
                {
                    Application wordApplication = new Application();//new word application
                    Document wordDocument = wordApplication.Documents.Open(this.FilePath); //open the document

                    //change file name to have .pdf instead of .docx
                    this.FileName = this.FileName.Replace(FileType, "pdf"); 

                    //save the open word document as PDF
                    wordDocument.SaveAs2(FilePath, WdSaveFormat.wdFormatPDF);

                    //Close document
                    wordDocument.Close();

                    //exit word application
                    wordApplication.Quit();
                }
                catch (Exception e)
                {

                    convertStatus = StatusCode.FORHINDRING;
                    //The machine does not have word installed
                    Utility.WriteLog("ERROR- could not convert word doc to PDF - Perhaps server does not support Word functionality - message:" + e.ToString());
                }
                

            }
            return convertStatus;
        }

        /// <summary>
        /// Removes the temporary file for this attachment
        /// </summary>
        public void RemoveTempFile()
        {
            FileManager.Instance.DeleteFile(FilePath);
        }


        /// <summary>
        /// Not in use - not done
        /// </summary>
        private void EditWordDocSDKOpenXML()
        {
            //SERVER SIDE WORD PROCESSING using SDK Open XML 2.5
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(FilePath, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    //The xml content of the word document
                    docText = sr.ReadToEnd();
                }

                foreach (KeyValuePair<MESSAGE_VARIABLES, string> variable in MessageHandler.GetMessageVariables)
                {
                    //Fx. "[customerFullName] => Andreas Kirkegaard Jensen"
                    string value = MessageHandler.Instance.GetValueFromMessageVariable(variable.Key);

                    if (value == string.Empty)
                    {
                        continue;
                    }


                    Regex regexText = new Regex(variable.Value.Replace("[", "\\[").Replace("]", "\\]"));

                    docText = regexText.Replace(docText, value);

                    /*
                     This did not work.
                     The Problem:

                    I'm reading the raw xml data of the word file, and i cannot guarentee that 
                    each word / paragraph is a complete string.

                                Ideally it would be structured like this:

                    Kære [customerFirstName]...                   
                 
                                    But Word tends to split up the words like this:
                                           (each line represents a "string")
                      kære [  
                     customerFirstName  
                      ]
                 
                                    and sometimes like this:
                    kæ
                    re [cus
                    tomer   
                    FirstNa 
                    
                    m   
                    e]   

                      Word does this because each "string" is wrapped in xml - fx size, font, styl
                      but also spell checking instructions, which makes it incredibly inconsistent
                     */

                }

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }

        }
    }
}
/*
 * fileData = System.IO.File.ReadAllBytes("D:\\skole\\Hovedopgave\\Project\\MessageCenter\\MessageCenter\\MessageCenter\\Images\\andreas.jpg");

            File.WriteAllBytes(FileManager.Instance.GetFilePath("testBillede.jpg"), fileData);
*/



/*
public string GetWordDocAsHtml()
{
//TODO:
if (FileType != "docx")
{
    return string.Empty;
}

object documentFormat = 8;
string randomName = DateTime.Now.Ticks.ToString();
object htmlFilePath = FilePath.Replace("docx", "html");
object fileSavePath = FilePath;


//Open the word document in background.
Application applicationclass = new Application();
applicationclass.Documents.Open(ref fileSavePath);
applicationclass.Visible = false;
Document document = applicationclass.ActiveDocument;

//Save the word document as HTML file.
document.SaveAs(ref htmlFilePath, ref documentFormat);

//Close the word document.
document.Close();

//Read the saved Html File.
string wordHTML = System.IO.File.ReadAllText(htmlFilePath.ToString());

string tempPath = MessageHandler.Instance.GetTempFilesPath();
//Loop and replace the Image Path.
foreach (Match match in Regex.Matches(wordHTML, "<v:imagedata.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase))
{
    wordHTML = Regex.Replace(wordHTML, match.Groups[1].Value, tempPath + "/" + match.Groups[1].Value);
}


return wordHTML;




}
*/
