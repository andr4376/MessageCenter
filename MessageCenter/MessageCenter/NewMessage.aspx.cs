using MessageCenter.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MessageCenter
{
    public partial class NewMessage : System.Web.UI.Page
    {


        /// <summary>
        /// Returns whether or not the msgTemplate is of type mail
        /// </summary>
        public bool IsMail
        {
            get
            {
                if (MessageHandler.Instance.MsgTemplate == null)
                {
                    return false;
                }
                return MessageHandler.Instance.MsgTemplate.MessageType == MessageType.MAIL;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //If user is not valid
            if (!SignIn.Instance.IsLoggedIn ||
                !SignIn.Instance.IsAdmin)
            {
                //got to front page
                Response.Redirect("Default.aspx");

            }

            //If this is the initial load
            if (!IsPostBack)
            {
                Initialize();

            }

            //Create the html table of message variables
            SetupVariablesTable();


        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //Show html elements if msg template is ready
            messageTemplateBody.Visible = MessageHandler.Instance.MsgTemplate != null;

            //show attachments if mail
            AttachmentsSection.Visible = IsMail;

        }

        /// <summary>
        /// Updates the list of attachments 
        /// </summary>
        /// <param name="attachments"></param>
        private void UpdateAttachmentsListbox(List<MessageAttachment> attachments)
        {
            if (listBoxAttachments.Items.Count > 0)
            {
                listBoxAttachments.Items.Clear();
            }

            for (int i = 0; i < attachments.Count; i++)
            {
                listBoxAttachments.Items.Add(
                    new ListItem(
                    attachments[i].FileName,
                    i.ToString()));/*The list item value is the item index, because i cannot ensure that all attachments have ids*/
            }
        }

        /// <summary>
        /// Code for initializing
        /// </summary>
        private void Initialize()
        {
            //Make sure the Messagehandler singleton is new
            MessageHandler.Reset();

            //opens the "select message type" modal
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openPickMessageTypeModal();", true);

        }

        /// <summary>
        /// Creates a html table of message variables with a  description 
        /// </summary>
        private void SetupVariablesTable()
        {
            //For each message variable
            foreach (KeyValuePair<MESSAGE_VARIABLES, string> variable in MessageHandler.GetMessageVariables)
            {
                string description = string.Empty;

                switch (variable.Key)
                {
                    case MESSAGE_VARIABLES.CUSTOMER_FULLNAME:
                        description = "Kundens fulde navn - F.eks \"Andreas Kirkegaard Jensen\"";
                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_FIRSTNAME:
                        description = "Kundens fornavn - F.eks \"Andreas\"";

                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_LASTNAME:
                        description = "Kundens efternavn - F.eks \"Kirkegaard Jensen\"";

                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_BIRTHDAY:
                        description = "Kundens fødselsdag- F.eks \"26-01-1994\"";

                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_PHONENUMBER:
                        description = "Kundens mobilnr. - F.eks \"40965001\"";

                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_EMAIL:
                        description = "Kundens email- F.eks \"andr4376@gmail.com\"";

                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_AGE:
                        description = "Kundens alder - F.eks \"25\"";

                        break;
                    case MESSAGE_VARIABLES.CUSTOMER_CPR:
                        description = "Kundens cprnr. - F.eks \"260194xxxx\"";

                        break;
                    case MESSAGE_VARIABLES.DEPARTMENT:
                        description = "Afsenderens / kundens afdeling - F.eks \"Spentrup\"";

                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_FULLNAME:
                        description = "Navnet på medarbejderen som afsender beskeden";

                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_FIRSTNAME:
                        description = "Fornavnet på medarbejderen som afsender beskeden";

                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_LASTNAME:
                        description = "Efternavnet på medarbejderen som afsender beskeden";

                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_PHONENUMBER:
                        description = "Mobil nummeret til medarbejderen som afsender beskeden";

                        break;
                    case MESSAGE_VARIABLES.EMPLOYEE_EMAIL:
                        description = "Email adresse til medarbejderen som afsender beskeden";
                        break;
                    default:
                        break;
                }

                //Create a new row
                TableRow row = new TableRow();

                //Create a cell for the variable name
                TableCell variableName = new TableCell();
                variableName.Text = variable.Value;
                row.Cells.Add(variableName);

                //Create a cell for the variable description
                TableCell variableDescription = new TableCell();
                variableDescription.Text = description;
                row.Cells.Add(variableDescription);

                //add row to table
                variablesTable.Rows.Add(row);
            }
        }

        /// <summary>
        /// Click event for the download attachment button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DownloadAttachmentBtn_Click(object sender, EventArgs e)
        {
            DownloadSelectedAttachment();
        }

        /// <summary>
        /// click event for removing a selected attachment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RemoveAttachmentButton_Click(object sender, EventArgs e)
        {
            //if an attachment is not selected
            if (listBoxAttachments.SelectedItem == null)
            {
                return;
            }
            //get file name
            string fileName = listBoxAttachments.SelectedItem.Text;

            //Remove selected attachment (and its files)                
            MessageHandler.Instance.RemoveAttachment(fileName);


            //update the listbox
            UpdateAttachmentsListbox(MessageHandler.Instance.Attachments);


        }
        /// <summary>
        /// click event that opens the "add attachment" modal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void openNewAttachmentModalBtn_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openAttachmentModal();", true);

        }

        /// <summary>
        /// Click event for uploading the selected file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UploadFileBtn_Click(object sender, EventArgs e)
        {
            MessageAttachment tmp;

            //if file is being uploaded
            if (AttachmentFileUpload.HasFile)
            {
                //convert the file to a MessageAttachment
                tmp = new MessageAttachment(
                    AttachmentFileUpload.FileName, AttachmentFileUpload.FileBytes);

                //add attachment to list and update listbox
                lock (MessageHandler.Instance.attachmentsKey)
                {
                    tmp = MessageHandler.Instance.AddAttachment(tmp);

                    UpdateAttachmentsListbox(MessageHandler.Instance.Attachments);
                }

                //create the temporary file
                StatusCode createFileStatus = tmp.CreateTempFile();

                //Close the "add attachment" modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "closeAttachmentModal();", true);
            }

        }

        /// <summary>
        /// Creates a file download of the selected attachment for the user
        /// </summary>
        private void DownloadSelectedAttachment()
        {

            //if a file is not selected
            if (listBoxAttachments.SelectedItem == null)
            {
                return;
            }

            int messageIndex;

            //Try to get the selected index
            if (!Int32.TryParse(listBoxAttachments.SelectedValue, out messageIndex))
            {
                Utility.PrintWarningMessage("Noget gik galt ved identificering af den valgte fil - kontakt venligst teknisk support: " +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));

                return;
            }

            //if file does not exist - return
            if (!(MessageHandler.Instance.Attachments[messageIndex] != null && File.Exists(
                MessageHandler.Instance.Attachments[messageIndex].FilePath)))
            {
                return;
            }

            Response.Clear();

            //Prepare to write the file to the user.
            Response.ContentType = "application/octet-stream";
            //set name of file
            Response.AppendHeader("content-disposition", "filename="
            + MessageHandler.Instance.Attachments[messageIndex].FileName);

            //send the file.
            Response.WriteFile(MessageHandler.Instance.Attachments[messageIndex].FilePath);

            //clear
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// Click event for saving the message template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CreateMessageBtn_Click(object sender, EventArgs e)
        {
            //remove "'" fro mthe title and text since sqlite doesnt like those. TODO: find better solution
            MessageHandler.Instance.MsgTemplate.Title = titleTextBox.Text.Replace("'", "");
            MessageHandler.Instance.MsgTemplate.Text = messageTextTextBox.Text.Replace("'", "");

            //if title and text is not filled out
            if (!MessageHandler.Instance.MsgTemplate.IsValid)
            {
                return;
            }
            if (MessageHandler.Instance.MsgTemplate.IsValid)
            {
                //Save the message template - and get the id
                int? id = DatabaseManager.Instance.AddMessageTemplate(MessageHandler.Instance.MsgTemplate);


                if (id == null) //table is empty and this is the only element
                {
                    id = DatabaseManager.Instance.GetNextId(
                        Configurations.GetConfigurationsValue(
                            CONFIGURATIONS_ATTRIBUTES.MESSAGE_TEMPLATE_TABLE_NAME));

                    if (id != null)
                        id -= 1;//set id to that of the newest added
                    else
                    {
                        Utility.WriteLog("Error - failed to get id of the newly added msg template");
                        return;
                    }


                }

                //Save all attachments with a reference to the message template created above
                foreach (MessageAttachment attachment in MessageHandler.Instance.Attachments)
                {
                    DatabaseManager.Instance.AddAttachmentToDB(attachment, ((int)id));
                }

            }
            //else tell user to fill title and text

            //delete temp files and reset messagehandler instance
            MessageHandler.Reset();
            //go to front page
            Response.Redirect("Default.aspx");


        }


        /// <summary>
        /// on click event for confirming choice of message type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void selectMsgTypeBtn_Click(object sender, EventArgs e)
        {
            int type;

            if (Int32.TryParse(selectMsgTypeDropDownList.SelectedValue, out type))
                MessageHandler.Instance.SetBlankMessage(type);


        }
    }
}