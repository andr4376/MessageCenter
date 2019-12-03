﻿using MessageCenter.Code;
using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MessageCenter
{
    public partial class Messages : System.Web.UI.Page
    {
        private int messageTemplateIdInput;

        public string GetReceiverAdresse
        {
            get
            {
                if (MessageHandler.Instance.MsgTemplate != null)
                {
                    if (MessageHandler.Instance.Receiver != null)
                    {
                        return MessageHandler.Instance.MsgTemplate.MessageType == MessageType.MAIL ?
                            MessageHandler.Instance.Receiver.Email : MessageHandler.Instance.Receiver.PhoneNumber;
                    }

                }
                return "";
            }
        }
        public string GetTitle
        {
            get
            {
                if (MessageHandler.Instance.MsgTemplate != null)
                {
                    return MessageHandler.Instance.MsgTemplate.Title;
                }
                return "";
            }
        }

        public string GetText
        {
            get
            {
                if (MessageHandler.Instance.MsgTemplate != null)
                {
                    return MessageHandler.Instance.MsgTemplate.Text;
                }
                return "";
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {



            if (!Page.IsPostBack)
            {
                if (!MessageHandler.Instance.IsReady)
                {
                    Initialize();

                }

            }
            else
            {
                if (CheckIfDoubleClickCustomerListBox())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "closePickUserModal();", true);
                }


            }



        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            mailMessageBody.Visible = MessageHandler.Instance.IsReady &&
                MessageHandler.Instance.MsgTemplate.MessageType == MessageType.MAIL;

            smsMessageBody.Visible = MessageHandler.Instance.IsReady &&
                MessageHandler.Instance.MsgTemplate.MessageType == MessageType.SMS;

            if (MessageHandler.Instance.IsReady &&
                MessageHandler.Instance.Attachments != null &&
                MessageHandler.Instance.Attachments.Count > 0)
            {
                UpdateAttachmentsListbox(MessageHandler.Instance.Attachments);
            }




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

            /*
                        listBoxAttachments.DataSource = attachments;
                        listBoxAttachments.DataTextField = "FileName";
                        listBoxAttachments.DataValueField = "Id";
                        listBoxAttachments.DataBind();
                        */
        }

        private void Initialize()
        {

            //Add double click event to listbox, so user can double click instead of using the button
            listBoxCustomers.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(listBoxCustomers, "doubleClickCustomer"));



            //if user is logged in and has selected a message item
            if (Session["MessageTemplateId"] != null
                && Int32.TryParse(
                Session["MessageTemplateId"].ToString(),
                out messageTemplateIdInput)
                && SignIn.Instance.IsLoggedIn)
            {
                Utility.WriteLog("Message page was opened with the message template id:" + messageTemplateIdInput);


                //Setup messageHandlers's Sender and messagetype. Receiver is added later in "GetSelectedCustomer()"
                StatusCode status = SetupMessageHandler();

                // If message and or sender is NULL
                if (status == StatusCode.ERROR)
                {
                    Utility.WriteLog("MessageTemplate with id " + messageTemplateIdInput + " wan not found in the database");
                    Utility.PrintWarningMessage("Teknisk fejl - den valgte besked kunne ikke findes i databasen. Kontakt venligst teknisk support: "
                        + Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));
                    Response.Redirect("Default.aspx");
                    return;
                }
                if (status == StatusCode.OK)
                {
                    SetupCustomerPicker();

                }



            }
            else
            {
                //User did not navigate to this page using the user interface, so we send them back
                Response.Redirect("Default.aspx");
                Utility.WriteLog("redirecting to Default page");
                return;

            }



        }

        private StatusCode SetupMessageHandler()
        {
            //If messagehandler is already setup
            if (MessageHandler.Instance.IsReady == true)
            {
                return StatusCode.FORHINDRING;
            }

            MessageHandler.Instance.Sender = SignIn.Instance.User;
            MessageHandler.Instance.MsgTemplate = DatabaseManager.Instance.GetMessageTemplateFromId(messageTemplateIdInput);




            if (MessageHandler.Instance.MsgTemplate == null || MessageHandler.Instance.Sender == null)
            {

                return StatusCode.ERROR;
            }

            return StatusCode.OK;
        }

        private void SetupCustomerPicker()
        {

            StatusCode listboxStatus = UpdateCustomersListbox(SignIn.Instance.MyCustomers);

            if (listboxStatus == StatusCode.OK)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openPickUserModal();", true);

            }
            else
            {
                Utility.PrintWarningMessage(
                   "Der blev ikke fundet nogle kunder tilknyttet dit TUser - hvis dette er en fejl, kontakt support: " +
                   Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool CheckIfDoubleClickCustomerListBox()
        {
            //Check if doubleClick event on listbox
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "doubleClickCustomer")
            {
                if (GetSelectedCustomer() == StatusCode.OK)
                {
                    DisplayMessageData();

                    return true;
                }
            }
            return false;
        }

        private StatusCode UpdateCustomersListbox(List<Customer> customersToShow)
        {
            StatusCode status = StatusCode.OK;

            if (!(customersToShow.Count > 0))
            {
                status = StatusCode.FORHINDRING;

            }

            Dictionary<string, string> dictionaryVersionOfList =
                Utility.ConvertCustomerListToDictionary(customersToShow);

            listBoxCustomers.DataSource = dictionaryVersionOfList;
            listBoxCustomers.DataTextField = "Value";
            listBoxCustomers.DataValueField = "Key";
            listBoxCustomers.DataBind();


            return status;

        }



        private void DisplayMessageData()
        {
            //TODO:
            //activate elements
            //Fill with data
            //replace text with customer name / employee name ect.

            Utility.WriteLog(MessageHandler.Instance.ToString());

            MessageHandler.Instance.FillMessageWithData();


            //Refreshes page
            Response.Redirect(Request.RawUrl);
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        protected void btn_Submit_User_Click(object sender, EventArgs e)
        {
            if (GetSelectedCustomer() == StatusCode.OK)
            {
                DisplayMessageData();

            }

        }

        private StatusCode GetSelectedCustomer()
        {
            string selectedCustomer = listBoxCustomers.SelectedValue.ToString();

            if (selectedCustomer == string.Empty)
            {
                //User has double clicked inside the listbox, but not on an item
                return StatusCode.FORHINDRING;
            }

            Customer customer = SignIn.Instance.MyCustomers.Where(
                c => c.Cpr ==
               selectedCustomer)
                .ToList()[0];

            MessageHandler.Instance.Receiver = customer;

            if (customer == null)
            {
                Utility.PrintWarningMessage("Teknisk fejl ved udhentning af data for den valgte kunde - kontakt venligst teknisk support: "
                    + Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));
                return StatusCode.ERROR;
            }

            return StatusCode.OK;
        }

        protected void searchBtnCustomer_Click(object sender, EventArgs e)
        {
            string searchInput = customerCprInput.Text;

            List<Customer> customers = SignIn.Instance.MyCustomers;

            if (searchInput != "")
            {
                UpdateCustomersListbox(
                 customers.Where(x => x.Cpr.Contains(searchInput)).ToList<Customer>());
            }
            else
            {
                UpdateCustomersListbox(customers);
            }

        }

        protected void sendMailBtn_Click(object sender, EventArgs e)
        {

            //Update message title and text with the new, edited text
            MessageHandler.Instance.MsgTemplate.Title = titleTextBox.Text;

            switch (MessageHandler.Instance.MsgTemplate.MessageType)
            {
                case MessageType.MAIL:
                    MessageHandler.Instance.MsgTemplate.Text = messageTextTextBox.Text;
                    MessageHandler.Instance.Receiver.Email = customerMailInputText.Text;
                    MessageHandler.Instance.cCAdress = ccAdressInput.Text;


                    break;
                case MessageType.SMS:
                    MessageHandler.Instance.MsgTemplate.Text = smsContent.Text;
                    MessageHandler.Instance.Receiver.PhoneNumber = smsPhoneNumber.Text;
                    break;

                default:
                    Utility.WriteLog("ERROR! sendMailBtn_Click");
                    MessageHandler.Instance.MsgTemplate.Text = string.Empty;
                    break;
            }


            try
            {
                MessageHandler.Instance.SetupMessage();

            }
            catch (Exception exception)
            {
                Utility.WriteLog("Der opstod fejl ved at sende email - Fejlbesked: '" + exception.ToString() + "'");

                if (exception is System.FormatException || exception is System.ArgumentException)
                {
                    Utility.PrintWarningMessage("Email kunne ikke sendes! Valider venligt mail adresse information");
                }
                else
                {
                    Utility.PrintWarningMessage("Der opstod en ukendt fejl - kontakt venligt teknisk support: " +
                        Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));

                }

                return;
            }

            MessageHandler.Instance.SendMessage();

            Response.Redirect("Default.aspx");

        }

        public void ShowWordDocAsHtml(string html)
        {
            if (html == string.Empty)
            {
                return;
            }
            // attachmentsDiv.InnerHtml = html;

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


    }
}

//<asp:AsyncPostBackTrigger ControlID="UploadFileBtn" EventName="Click" />-

