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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (PopulateMessagePrefabListBox() != 0)
                {
                    //ERROR
                }
            }


                if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "doubleClick")
                {
                    GoToMessagePage();

                }
                listBoxMessageTemplates.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(listBoxMessageTemplates, "doubleClick"));

           
        }

        private int PopulateMessagePrefabListBox()
        {
            Dictionary<string, string> listOfMessages = DatabaseManager.Instance.GetMessageTemplatesDictionaryTitleId();

            if (listOfMessages == null)
            {
                return 9; //error
            }

            if (listBoxMessageTemplates.Items.Count > 0)
            {
                listBoxMessageTemplates.Items.Clear();

            }


            listBoxMessageTemplates.DataSource = listOfMessages;
            listBoxMessageTemplates.DataTextField = "Value";
            listBoxMessageTemplates.DataValueField = "Key";
            listBoxMessageTemplates.DataBind();
         

            return 0;
        }

        protected void listBoxMessageTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(listBoxMessageTemplates.SelectedValue);
        }

        protected void btn_proceedToMessagePage_Click(object sender, EventArgs e)
        {
            GoToMessagePage();
            
        }
        

              protected void searchBtn_Click(object sender, EventArgs e)
        {

         
            System.Diagnostics.Debug.WriteLine("searching for input:");
            System.Diagnostics.Debug.WriteLine(searchInput.Text);

        }
        private void GoToMessagePage()
        {
            System.Diagnostics.Debug.WriteLine("proceeding to message page");

            System.Diagnostics.Debug.WriteLine(listBoxMessageTemplates.SelectedItem.Value);
            System.Diagnostics.Debug.WriteLine(listBoxMessageTemplates.SelectedItem.Text);
        }
    }
}








