using MessageCenter.Code;
using System;
using System.Collections.Generic;
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
            Utility.WriteLog(IsMail.ToString());
            if (!IsPostBack)
            {
                Initialize();
            }

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

            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("content-disposition", "filename="
            + MessageHandler.Instance.Attachments[messageIndex].FilePath);
            Response.WriteFile(MessageHandler.Instance.Attachments[messageIndex].FilePath);
            Response.Flush();
            Response.End();
        }


        protected void CreateMessageBtn_Click(object sender, EventArgs e)
        {

        }



        protected void selectMsgTypeBtn_Click(object sender, EventArgs e)
        {
            int type;

            if (Int32.TryParse(selectMsgTypeDropDownList.SelectedValue, out type))
                MessageHandler.Instance.SetBlankMessage(type);


        }
    }
}