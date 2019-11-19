using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{

    public enum MessageType { MAIL, SMS };

    public class MessageTemplate
    {

        public int? Id
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }

        public string Text
        {
            get;
            set;
        }

        public MessageType MessageType
        {
            get;
            private set;
        }
        public int MessageTypeId
        {
            get
            {
                return (int)this.MessageType;
            }
            private set
            {
                MessageType = (MessageType)value;
            }
        }

        /// <summary>
        /// Returns a string that is safe for path naming
        /// </summary>
        public string PathName
        {
            get
            {
                string pathName;

                pathName = Title;

                foreach (char forbiddenChar in System.IO.Path.GetInvalidPathChars())
                {
                    pathName = pathName.Replace(forbiddenChar, '_');
                }

                return pathName;
            }
        }

        /// <summary>
        /// returns a string that is safe for file namings - Remember to append with file type Fx: FilenName + .pdf
        /// </summary>
        public string FileName
        {
            get
            {
                string fileName;

                fileName = Title;

                foreach (char forbiddenChar in System.IO.Path.GetInvalidFileNameChars())
                {
                   fileName= fileName.Replace(forbiddenChar, '_');
                }

                return fileName;
            }
        }

        public MessageTemplate(string title, string text, int messageType)
        {
            this.Title = title;
            this.Text = text;
            this.MessageTypeId = messageType;

        }
        public MessageTemplate(int id, string title, string text, int messageType):this(title,text,messageType)
        {
            this.Id = id;

        }


    }
}