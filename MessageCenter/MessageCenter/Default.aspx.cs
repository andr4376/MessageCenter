using MessageCenter.Code;
using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MessageCenter
{
    public partial class _Default : Page
    {
       
        public bool ShowAdminInterface
        {
            get
            {
               return SignIn.Instance.IsAdmin;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            //Initializing
            if (!Page.IsPostBack)
            {
                Initialize();
              
            }
            CheckIfListboxDoubleClick();


        }

        /// <summary>
        /// Initialization of the front page. 
        /// </summary>
        private void Initialize()
        {
            Utility.WriteLog("Initializing Front Page");

            DataBind();

            MessageHandler.Reset();

            SetupListBox();

            listBoxMessageTemplates.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(listBoxMessageTemplates, "doubleClick"));


        }

        /// <summary>
        /// Populates listbox and sets up double click event
        /// </summary>
        private void SetupListBox()
        {
            if (PopulateMessageTemplatesListBox() != StatusCode.OK)
            {
                //ERROR
                Utility.WriteLog("Error in initializing front page - could not populate ListBox");

            }


        }

        private void CheckIfListboxDoubleClick()
        {
            //Check if doubleClick event on listbox
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "doubleClick")
            {
                GoToMessagePage();

            }
        }

        private StatusCode PopulateMessageTemplatesListBox()
        {
            List<MessageTemplate> messages = DatabaseManager.Instance.GetAllMessageTemplates();

            if (messages == null)
            {
                return StatusCode.ERROR;
            }

            UpdateListBox(messages);

            return StatusCode.OK;
        }

        protected void listBoxMessageTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            Utility.WriteLog(listBoxMessageTemplates.SelectedValue);
        }

        /// <summary>
        /// Proceeds to a message page for the selected ListBox MessageTemplate item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_proceedToMessagePage_Click(object sender, EventArgs e)
        {
            GoToMessagePage();

        }

        /// <summary>
        /// Searches for MessageTemplates where the title contains the text from the search input textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void searchBtn_Click(object sender, EventArgs e)
        {
            //Get search input
            string inputText = searchInput.Text;

            Utility.WriteLog("searching for input: " + inputText);

            if (inputText == "")
            {
                //Repopulate Listbox if the search input is blank
                PopulateMessageTemplatesListBox();
            }
            else
            {
                //Get all messagetemplates where the title contains the search input
                List<MessageTemplate> templates = DatabaseManager.Instance.GetMessagesTitleContainsText(inputText);


                UpdateListBox(templates);

            }


        }

        private void UpdateListBox(List<MessageTemplate> listOfMsgTemplates)
        {

            if (listBoxMessageTemplates.Items.Count > 0)
            {
                listBoxMessageTemplates.Items.Clear();
            }

            listBoxMessageTemplates.DataSource = listOfMsgTemplates;
            listBoxMessageTemplates.DataTextField = "Title";
            listBoxMessageTemplates.DataValueField = "Id";
            listBoxMessageTemplates.DataBind();


        }
        private void GoToMessagePage()
        {
            //Proceed if the user has picked a message item
            if (listBoxMessageTemplates.SelectedItem == null)
            {
                return;
            }
           

            //proceed if the has signed in, else, show user the login modal
            if (SignIn.Instance.User == null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

                return;
            }
            
            Utility.WriteLog("proceeding to message page for message Id: "+listBoxMessageTemplates.SelectedItem.Value);

           
            //Stores the Id of the selected messagetemplate
            Session["MessageTemplateId"] = listBoxMessageTemplates.SelectedItem.Value;

            //Opens the Messages page
            Response.Redirect("Messages.aspx");

        }

        protected void addNewMessageBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("NewMessage.aspx");
        }

        protected void removeMessageTemplate_Click(object sender, EventArgs e)
        {
            int messageId;

            //If user has selected a message template
            if (Int32.TryParse(listBoxMessageTemplates.SelectedValue, out messageId))
            {
                //DEMO skabeloner kan ikke slettes i sammenhæng med eksamensprojektet
                if (DatabaseManager.Instance.GetMessageTemplateFromId(messageId).Title.Contains("DEMO"))
                {
                    //slet ikke hvis det er en demo skabelon (EKSAMEN)
                    return;
                }
                ////Slet ovenstående efter eksamen

                //Delete the selected message Template, and all its attachments
                StatusCode deleteStatus=
                DatabaseManager.Instance.DeleteMessageTemplate(messageId);

                //Get all attachments and Update listbox               
                    UpdateListBox(
                        DatabaseManager.Instance.GetAllMessageTemplates());
;
            }

        }
    }
}








