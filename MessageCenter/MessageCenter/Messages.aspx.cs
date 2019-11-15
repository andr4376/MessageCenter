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
    public partial class Messages : System.Web.UI.Page
    {
        private int messageTemplateIdInput;

        /// <summary>
        /// The message that is to be sent to the customer
        /// </summary>
        private MessageTemplate messageTemplate;

        /// <summary>
        /// The receiver of the message
        /// </summary>
        private Customer customer;

      


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Initialize();

            }
           

        }

        private void Initialize()
        {
            //if user is logged in and has selected a message item
            if (Session["MessageTemplateId"] != null
                && Int32.TryParse(
                Session["MessageTemplateId"].ToString(),
                out messageTemplateIdInput)
                && SignIn.Instance.IsLoggedIn)
            {
                Utility.WriteLog("Message page was opened with the message template id:" + messageTemplateIdInput);

                messageTemplate = DatabaseManager.Instance.GetMessageTemplateFromId(messageTemplateIdInput);

                if (messageTemplate == null)
                {
                    Utility.WriteLog("MessageTemplate with id " + messageTemplateIdInput + " wan not found in the database");
                    Utility.PrintWarningMessage("Teknisk fejl - den valgte besked kunne ikke findes i databasen. Kontakt venligst teknisk support: " + DatabaseManager.supportEmail);
                    Response.Redirect("Default.aspx");
                    return;
                }
                SetupCustomerPicker();





            }
            else
            {
                //User did not navigate to this page using the user interface, so we send them back
                Response.Redirect("Default.aspx");
                Utility.WriteLog("redirecting to Default page");


            }

            
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
                   "Der blev ikke fundet nogle kunder tilknyttet dit TUser - hvis dette er en fejl, kontakt support: " + DatabaseManager.supportEmail);
            }
        }

        private StatusCode UpdateCustomersListbox(List<Customer> customersToShow)
        {
            StatusCode status = StatusCode.OK;

            if (!(customersToShow.Count>0))
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
        }

        protected void btn_Submit_User_Click(object sender, EventArgs e)
        {

        }

        protected void searchBtnCustomer_Click(object sender, EventArgs e)
        {

        }
    }
}