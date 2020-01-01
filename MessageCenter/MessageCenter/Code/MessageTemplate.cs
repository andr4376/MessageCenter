using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageCenter.Code
{

    public enum MessageType { MAIL, SMS };

    /// <summary>
    /// An object containing the title and main text for a message. 
    /// </summary>
    public class MessageTemplate
    {
        /// <summary>
        /// Returns the messagetemplates id as a nullable int 
        /// </summary>
        public int? Id //it has no id when we make new messagetemplates (therefor nullable)
        {
            get;
            private set;
        }

        /// <summary>
        /// the title of the message
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// The main text of the message.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Get / private set message type
        /// </summary>
        public MessageType MessageType
        {
            get;
            private set;
        }

        /// <summary>
        /// Get / private set messagetype by enum index
        /// </summary>
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
        /// The initial title of the message template. used for creating directories since the "title"'s value might change
        /// </summary>
        private readonly string initialTitle;

        /// <summary>
        /// Returns a string that is safe for path naming
        /// </summary>
        public string PathName
        {
            get
            {
                string pathName;

                pathName = initialTitle;
                //replace all dangerous chars with '_'
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

                fileName = initialTitle;
                //replace all dangerous chars with '_'
                foreach (char forbiddenChar in System.IO.Path.GetInvalidFileNameChars())
                {
                    fileName = fileName.Replace(forbiddenChar, '_');
                }

                return fileName;
            }
        }

        /// <summary>
        /// Returns whether or not both the title and the main text of the message is not empty
        /// </summary>
        public bool IsValid
        { get
            {
                return (Title != string.Empty && Text != string.Empty);
            }
        }

        /// <summary>
        /// Message Template constructor
        /// </summary>
        /// <param name="title">title of the message</param>
        /// <param name="text">content of the message</param>
        /// <param name="messageType">type of the message by index</param>
        public MessageTemplate(string title, string text, int messageType)
        {
            this.Title = title;
            this.Text = text;
            this.MessageTypeId = messageType;

            initialTitle = title;

        }
        /// <summary>
        /// Message Template constructor
        /// </summary>
        /// <param name="title">title of the message</param>
        /// <param name="text">content of the message</param>
        /// <param name="messageType">type of the message by index</param>
        /// <param name="id">the message templates id</param>
        public MessageTemplate(int id, string title, string text, int messageType) : this(title, text, messageType)
        {
            this.Id = id;

        }

        /// <summary>
        /// Used for creating a new messageTemplate
        /// </summary>
        /// <param name="type"></param>
        public MessageTemplate(int type):this(string.Empty,string.Empty, type)
        {
            initialTitle = "_NEW_MESSAGE_"; //used for temp directory for the message template's attachments
        }
    }
}