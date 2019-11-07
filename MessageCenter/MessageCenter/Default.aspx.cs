using MessageCenter.Code;
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
        private static Dictionary<string, string> listBoxMessageDictionary;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!Page.IsPostBack)
            {
                if (PopulateMessageTemplatesListBox() != ReturnCode.OK)
                {
                    //ERROR
                }
            }

            //Add doubleClick event
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "doubleClick")
            {
                GoToMessagePage();

            }
            listBoxMessageTemplates.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(listBoxMessageTemplates, "doubleClick"));


        }

        private ReturnCode PopulateMessageTemplatesListBox()
        {
           listBoxMessageDictionary = Utility.ConvertTemplateListToDictionary(DatabaseManager.Instance.GetMessageTemplates());

            if (listBoxMessageDictionary == null)
            {
                return ReturnCode.ERROR;
            }          

            UpdateListBox();

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

                listBoxMessageDictionary = Utility.ConvertTemplateListToDictionary(templates);

                UpdateListBox();
               
            }


        }

        private void UpdateListBox()
        {

            if (listBoxMessageTemplates.Items.Count > 0)
            {
                listBoxMessageTemplates.Items.Clear();
            }

            listBoxMessageTemplates.DataSource = listBoxMessageDictionary;
            listBoxMessageTemplates.DataTextField = "Value";
            listBoxMessageTemplates.DataValueField = "Key";
            listBoxMessageTemplates.DataBind();

          
        }
        private void GoToMessagePage()
        {
            if (listBoxMessageTemplates.SelectedItem == null)
            {
                return;
            }
            Utility.WriteLog("proceeding to message page");
          
            Utility.WriteLog(listBoxMessageTemplates.SelectedItem.Value);
            Utility.WriteLog(listBoxMessageTemplates.SelectedItem.Text);
        }
    }
}








