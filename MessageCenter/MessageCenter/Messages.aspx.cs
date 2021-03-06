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

        public string GetRecipientAdresse
        {
            get
            {
                if (MessageHandler.Instance.MsgTemplate != null)
                {
                    if (MessageHandler.Instance.Recipient != null)
                    {
                        return MessageHandler.Instance.MsgTemplate.MessageType == MessageType.MAIL ?
                            MessageHandler.Instance.Recipient.Email : MessageHandler.Instance.Recipient.PhoneNumber;
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
            if (!Page.IsPostBack) //If this is the first time this page is loaded on this page visit
            {
                if (!MessageHandler.Instance.IsReady)
                {
                    Initialize();
                }
            }
            else
            {
                CheckIfDoubleClickCustomerListBox();




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

        /// <summary>
        /// Update the listbox so its content matches that of the list of attachments
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
                    attachments[i].FileName, //Text = Display file name
                    i.ToString()) // value = index of the element
                    );
                /*The list item value is the item index, because i cannot ensure that all attachments have ids*/
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
            listBoxAttachments.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(listBoxCustomers, "doubleClickAttachment"));



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

            //The sender of the message is the employee who's signed in
            MessageHandler.Instance.Sender = SignIn.Instance.User;

            //Get the selected message template from db
            MessageHandler.Instance.MsgTemplate =
                DatabaseManager.Instance.GetMessageTemplateFromId(messageTemplateIdInput);

            if (MessageHandler.Instance.MsgTemplate == null
                || MessageHandler.Instance.Sender == null)
            {
                //Message template does not exist or user is not signed in
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
        /// Allows user to double click in order to select customer
        /// </summary>
        private void CheckIfDoubleClickCustomerListBox()
        {
            //Check if doubleClick event on listbox
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "doubleClickCustomer")
            {
                PickCustomer();
            }
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
            listBoxCustomers.DataTextField = "Value"; //cpr+firstname+lastname
            listBoxCustomers.DataValueField = "Key"; //cpr
            listBoxCustomers.DataBind();


            return status;

        }


        /// <summary>
        /// Makes sure message template content is editted, and then displays it to the screen
        /// </summary>
        private void EditAndDisplayMessageData()
        {

            //Prints message template, sender and receiver to output
            Utility.WriteLog(MessageHandler.Instance.ToString());

            //Inserts data into message text / attachments (Fx. replaces [customerFullName] with "Keld Hansen")
            MessageHandler.Instance.FillMessageWithData();

            //Refreshes page
            Response.Redirect(Request.RawUrl);

        }
        protected void Page_Init(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        protected void btn_Submit_Customer_Click(object sender, EventArgs e)
        {
            //

            PickCustomer();




        }

        private void PickCustomer()
        {
            //Get the customer and switch the result
            switch (GetSelectedCustomer())
            {
                case StatusCode.OK:
                    EditAndDisplayMessageData();
                    break;

                //User did not select a customer
                case StatusCode.FORHINDRING:
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openPickUserModal();", true);
                    break;

                //Just in case
                case StatusCode.ERROR:
                    Utility.WriteLog("Error in btn_Submit_Customer_Click - something went wrong in GetSelectedCustomer()");
                    Response.Redirect("Default.aspx");
                    break;

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

            //Just in case
            if ((selectedCustomer.Length != 10 &&//not cpr
                selectedCustomer.Length != 8)) //not cvr
            {
                Utility.WriteLog("ERROR: GetSelectedCustomer was called where the selected customer's identifier was " + selectedCustomer +
                    " - invalid (not cpr/cvr)");
                return StatusCode.ERROR;

            }

            //Get customer from api
            Customer customer = new ApiCaller().GetDataFromApi<Customer>(
                Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.GET_CUSTOMER_FROM_CPR_API_PARAMETERS)
               + selectedCustomer)[0];


            if (customer == null)
            {
                //ERROR
                Utility.PrintWarningMessage("Teknisk fejl ved udhentning af data for den valgte kunde - kontakt venligst teknisk support: "
                    + Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));
                return StatusCode.ERROR;
            }

            //Set Recipient to the selected customer.
            MessageHandler.Instance.Recipient = customer;

            return StatusCode.OK;
        }

        /// <summary>
        /// Filter customer listbox based on search input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void searchBtnCustomer_Click(object sender, EventArgs e)
        {
            //Get search input
            string searchInput = customerCprInput.Text.ToUpper();

            //Get a list of the User's customers
            List<Customer> customers = SignIn.Instance.MyCustomers;

            List<Customer> searchResults;

            if (searchInput != "")
            {
                //First search by CPR first
                searchResults = customers.Where(x => x.Cpr.Contains(searchInput))
                    .ToList<Customer>();

                //If no results
                if (searchResults.Count == 0)
                {
                    // try searching by name
                    searchResults = customers.Where(x => x.FullName.ToUpper().Contains(searchInput))
                    .ToList<Customer>();
                }

                //Update customer listbox with the resulsts
                UpdateCustomersListbox(searchResults);
                return;
            }
            else
            {
                //if blank search input: show all
                UpdateCustomersListbox(customers);
            }

        }

        protected void sendMailBtn_Click(object sender, EventArgs e)
        {
            //In case user has managed to access the page in an unintented way
            if (!MessageHandler.Instance.IsReady)
            {
                Response.Redirect("Default.aspx");
                return;
            }



            //Update message title and text with the new, edited text
            MessageHandler.Instance.MsgTemplate.Title = titleTextBox.Text;

            //Get neccessary infomation based on Message Template's message type
            switch (MessageHandler.Instance.MsgTemplate.MessageType)
            {
                case MessageType.MAIL:
                    MessageHandler.Instance.MsgTemplate.Text = messageTextTextBox.Text;
                    MessageHandler.Instance.Recipient.Email = customerMailInputText.Text;
                    MessageHandler.Instance.cCAdress = ccAdressInput.Text;
                    Utility.WriteLog("Preparing to send Mail");

                    break;
                case MessageType.SMS:
                    MessageHandler.Instance.MsgTemplate.Text = smsContent.Text;
                    MessageHandler.Instance.Recipient.PhoneNumber = smsPhoneNumber.Text;
                    Utility.WriteLog("Preparing to send Sms");

                    break;

                default:
                    Utility.WriteLog("ERROR! sendMailBtn_Click");
                    MessageHandler.Instance.MsgTemplate.Text = string.Empty;
                    break;
            }


            try
            {
                Utility.WriteLog("Setting up the message...");

                //Create the Mail or Sms based on the info above
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

            Utility.WriteLog("Sending the message!");


            //Send the message (adds attachments if Mail)
            KeyValuePair<StatusCode, string> messageSentReport =
                MessageHandler.Instance.SendMessage();

            Utility.WriteLog("Result: " + messageSentReport.Key + " - Description: " + messageSentReport.Value);

            switch (messageSentReport.Key)
            {
                //if message is succesfully sent
                case StatusCode.OK:

                    Session["messageStatusDescription"] =
                        new KeyValuePair<StatusCode, string>(StatusCode.OK, "Besked blev sendt!");
                    Response.Redirect("Default.aspx");//go to home page
                    break;

                //if complications occured
                case StatusCode.FORHINDRING: // Fx. Complications while editing attachments
                    messageStatus.Text = messageSentReport.Value;
                    //Site.Master.cs will make a popup descriping what went wrong in its Load_Page
                    Session["messageStatusDescription"] =
                        messageSentReport;
                    Response.Redirect(Request.RawUrl); //refresh page to trigger Master's popup
                    break;

                case StatusCode.ERROR:
                    //Site.Master.cs will make a popup descriping what went wrong in its Load_Page
                    Session["messageStatusDescription"] =
                        messageSentReport;
                    Response.Redirect("Default.aspx"); //go to home page
                    break;

                default:
                    break;
            }




        }


        /// <summary>
        /// On click event for download attachment button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DownloadAttachmentBtn_Click(object sender, EventArgs e)
        {
            lock (MessageHandler.Instance.attachmentsKey)
            {
                DownloadSelectedAttachment();
            }

        }


        /// <summary>
        /// Remove the selected attachment from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RemoveAttachmentButton_Click(object sender, EventArgs e)
        {


            if (listBoxAttachments.SelectedItem == null)
            {
                return;
            }

            lock (MessageHandler.Instance.attachmentsKey)
            {
                //In case user has managed to access the page in an unintented way
                if (!MessageHandler.Instance.IsReady)
                {
                    Response.Redirect("Default.aspx");
                    return;
                }

                if (listBoxAttachments.SelectedItem == null)
                {
                    return;
                }

                string fileName = listBoxAttachments.SelectedItem.Text;

                //Remove selected attachment (and its files)                
                MessageHandler.Instance.RemoveAttachment(fileName);

                //refresh attachments listbox
                UpdateAttachmentsListbox(MessageHandler.Instance.Attachments);

            }

        }

        /// <summary>
        /// Button click event that opens a modal containging "add attachment" functionality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void openNewAttachmentModalBtn_Click(object sender, EventArgs e)
        {
            //In case user has managed to access the page in an unintented way
            if (!MessageHandler.Instance.IsReady)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            //calls the JS function "openAttachmentModal()" from Messages.aspx on the upcomming postback
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openAttachmentModal();", true);
        }

        /// <summary>
        /// Adds the user's uploaded file to attachments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UploadFileBtn_Click(object sender, EventArgs e)
        {
            MessageAttachment tmp;

            //In case user has managed to access the page in an unintented way
            if (!MessageHandler.Instance.IsReady)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            //if the FileUpload Control has a file
            if (AttachmentFileUpload.HasFile)
            {
                //Create a new attachment object with the file name and the file data
                tmp = new MessageAttachment(
                    AttachmentFileUpload.FileName, AttachmentFileUpload.FileBytes);

                lock (MessageHandler.Instance.attachmentsKey)
                {
                    //Add attachment to the list of attachments for the current message
                    tmp = MessageHandler.Instance.AddAttachment(tmp);

                    Utility.WriteLog("User uploaded attachment \"" + tmp.FileName + "\"");


                    //Refresh Listbox to show all attachments
                    UpdateAttachmentsListbox(MessageHandler.Instance.Attachments);
                }

                //Create temporary file in app data 
                StatusCode createFileStatus = tmp.CreateTempFile();

                //Close the "upload file" modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "closeAttachmentModal();", true);

            }

        }

        /// <summary>
        /// Finds the selected attachment File, and sends a download to the user's client
        /// </summary>
        private void DownloadSelectedAttachment()
        {

            //In case user has managed to access the page in an unintented way
            if (!MessageHandler.Instance.IsReady)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (listBoxAttachments.SelectedItem == null)
            {
                return;
            }

            int messageIndex;

            //try to get selected file's list index
            if (!Int32.TryParse(listBoxAttachments.SelectedValue, out messageIndex))
            {
                //If failed
                Utility.PrintWarningMessage("Noget gik galt ved identificering af den valgte fil - kontakt venligst teknisk support: " +
                    Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL));

                return;
            }

            Response.Clear();

            //Tell the page we're about to stream file data
            Response.ContentType = "application/octet-stream";

            //Set name of file
            Response.AppendHeader("content-disposition", "filename="
            + MessageHandler.Instance.Attachments[messageIndex].FileName);

            //Send the selected attachment's file 
            Response.WriteFile(MessageHandler.Instance.Attachments[messageIndex].FilePath);

            //Clear
            Response.Flush();
            Response.End();
        }



    }
}



