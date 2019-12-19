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
            if (!SignIn.Instance.IsLoggedIn ||
                !SignIn.Instance.IsAdmin)
            {
                Response.Redirect("Default.aspx");

            }

            if (!IsPostBack)
            {
                Initialize();

            }

            SetupVariablesTable();


        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            messageTemplateBody.Visible = MessageHandler.Instance.MsgTemplate != null;
            AttachmentsSection.Visible = IsMail;

        }

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

        private void Initialize()
        {
            MessageHandler.Reset();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openPickMessageTypeModal();", true);

        }

        private void SetupVariablesTable()
        {

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

                TableRow row = new TableRow();

                TableCell variableName = new TableCell();
                variableName.Text = variable.Value;
                row.Cells.Add(variableName);

                TableCell variableDescription = new TableCell();
                variableDescription.Text = description;
                row.Cells.Add(variableDescription);

                variablesTable.Rows.Add(row);
            }
        }

        protected void DownloadAttachmentBtn_Click(object sender, EventArgs e)
        {
            DownloadSelectedAttachment();
        }

        protected void RemoveAttachmentButton_Click(object sender, EventArgs e)
        {
            if (listBoxAttachments.SelectedItem == null)
            {
                return;
            }

            int messageIndex;

            if (!Int32.TryParse(listBoxAttachments.SelectedValue, out messageIndex))
            {
                Utility.PrintWarningMessage("Noget gik galt ved identificering af den valgte fil - kontakt venligst teknisk support: " +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));

                return;
            }

            MessageHandler.Instance.RemoveAttachment(messageIndex);



            UpdateAttachmentsListbox(MessageHandler.Instance.Attachments);


        }

        protected void openNewAttachmentModalBtn_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openAttachmentModal();", true);

        }

        protected void UploadFileBtn_Click(object sender, EventArgs e)
        {



            MessageAttachment tmp;
            if (AttachmentFileUpload.HasFile)
            {

                tmp = new MessageAttachment(
                    AttachmentFileUpload.FileName, AttachmentFileUpload.FileBytes);

                StatusCode createFileStatus = tmp.CreateTempFile();

                if (createFileStatus == StatusCode.OK)
                {
                    lock (MessageHandler.attachmentsKey)
                    {
                        MessageHandler.Instance.Attachments.Add(tmp);

                        UpdateAttachmentsListbox(MessageHandler.Instance.Attachments);
                    }
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "closeAttachmentModal();", true);
            }

        }

        private void DownloadSelectedAttachment()
        {

            if (listBoxAttachments.SelectedItem == null)
            {
                return;
            }

            int messageIndex;

            if (!Int32.TryParse(listBoxAttachments.SelectedValue, out messageIndex))
            {
                Utility.PrintWarningMessage("Noget gik galt ved identificering af den valgte fil - kontakt venligst teknisk support: " +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));

                return;
            }

            if (!(MessageHandler.Instance.Attachments[messageIndex] != null && File.Exists(
                MessageHandler.Instance.Attachments[messageIndex].FilePath)))
            {
                return;
            }

            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("content-disposition", "filename="
            + MessageHandler.Instance.Attachments[messageIndex].FileName);
            Response.WriteFile(MessageHandler.Instance.Attachments[messageIndex].FilePath);
            Response.Flush();
            Response.End();
        }


        protected void CreateMessageBtn_Click(object sender, EventArgs e)
        {

            MessageHandler.Instance.MsgTemplate.Title = titleTextBox.Text.Replace("'","");

            MessageHandler.Instance.MsgTemplate.Text = messageTextTextBox.Text.Replace("'", "");

            if (!MessageHandler.Instance.MsgTemplate.IsValid)
            {
                return;
            }
            if (MessageHandler.Instance.MsgTemplate.IsValid)
            {
                //Save the message template
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



        protected void selectMsgTypeBtn_Click(object sender, EventArgs e)
        {
            int type;

            if (Int32.TryParse(selectMsgTypeDropDownList.SelectedValue, out type))
                MessageHandler.Instance.SetBlankMessage(type);


        }
    }
}