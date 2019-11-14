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
            SetupListBox();

        }

        /// <summary>
        /// Populates listbox and sets up double click event
        /// </summary>
        private void SetupListBox()
        {
            if (PopulateMessageTemplatesListBox() != ReturnCode.OK)
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
            listBoxMessageTemplates.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(listBoxMessageTemplates, "doubleClick"));
        }

        private ReturnCode PopulateMessageTemplatesListBox()
        {
           Dictionary<string,string> listBoxMessageDictionary = Utility.ConvertTemplateListToDictionary(DatabaseManager.Instance.GetMessageTemplates());

            if (listBoxMessageDictionary == null)
            {
                return ReturnCode.ERROR;
            }

            UpdateListBox(listBoxMessageDictionary);

            return ReturnCode.OK;
        }

        protected void listBoxMessageTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            Utility.WriteLog(listBoxMessageTemplates.SelectedValue);
        }

        protected void btn_proceedToMessagePage_Click(object sender, EventArgs e)
        {
            GoToMessagePage();


        }


        protected void searchBtn_Click(object sender, EventArgs e)
        {
            string inputText = searchInput.Text;
            Utility.WriteLog("searching for input: " + inputText);

            if (inputText == "")
            {
                PopulateMessageTemplatesListBox();
            }
            else
            {
                List<MessageTemplate> templates = DatabaseManager.Instance.GetMessagesContainingText(inputText);

                Dictionary<string,string> listBoxMessageDictionary = Utility.ConvertTemplateListToDictionary(templates);

                UpdateListBox(listBoxMessageDictionary);

            }


        }

        private void UpdateListBox(Dictionary<string,string> newMessagesInput)
        {

            if (listBoxMessageTemplates.Items.Count > 0)
            {
                listBoxMessageTemplates.Items.Clear();
            }

            listBoxMessageTemplates.DataSource = newMessagesInput;
            listBoxMessageTemplates.DataTextField = "Value";
            listBoxMessageTemplates.DataValueField = "Key";
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

            //TODO: continue if element exists in db
            /*
            if (DatabaseManager.Instance.get)
            {

            }
            */
            Utility.WriteLog("proceeding to message page for message Id: "+listBoxMessageTemplates.SelectedItem.Value);

            Session["MessageTemplateId"] = listBoxMessageTemplates.SelectedItem.Value;
            Response.Redirect("Messages.aspx");

        }
    }
}








