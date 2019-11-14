using MessageCenter.Code;
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
            if (Int32.TryParse(
                Session["MessageTemplateId"].ToString(),
                out messageTemplateIdInput)
                && SignIn.Instance.IsLoggedIn)
            {
                Utility.WriteLog("Message page was opened with the message template id:" + messageTemplateIdInput);
                               
                

            }
            else
            {
                //User did not navigate to this page using the user interface, so we send them back
                Response.Redirect("Default.aspx");
                Utility.WriteLog("redirecting to Default page");


            }








        }
    }
}