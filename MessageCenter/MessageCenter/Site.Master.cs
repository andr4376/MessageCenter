using MessageCenter.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MessageCenter
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            loginLink.Visible = SignIn.Instance.User == null ? true : false;

            SuccesfulLoginHandler();

            MessageSentStatus();


        }

        private void MessageSentStatus()
        {
            if (Session["messageStatusDescription"] != null)
            {
                KeyValuePair<StatusCode, string> messageStatus =
                 (KeyValuePair<StatusCode, string>)Session["messageStatusDescription"];

                statusModalLabel.InnerText = messageStatus.Key.ToString();
                statusModalText.Text = messageStatus.Value;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openstatusModal();", true);

                Session["messageStatusDescription"] = null;
            }
        }

        private void SuccesfulLoginHandler()
        {
            if (Session["NewLogin"] != null)
            {
                if (SignIn.Instance.IsLoggedIn)
                {
                    //Show welcome message
                    loginStatusText.Text = "Velkommen " + SignIn.Instance.ToString();

                    //hide login input
                    loginPasswordInput.Visible = false;
                    loginTuserInput.Visible = false;
                    btn_login.Visible = false;
                    btn_login.UseSubmitBehavior = false;
                    //

                    //Reopen modal to display welcome message
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

                    //Close modal automatically after a delay (some users thought that the welcome message means the app is loading something)
                    int msDelay = 3000;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeModal", "closeModal(" + msDelay + ");", true);

                }

                Session["NewLogin"] = null;
            }
        }




        protected void btn_login_Click(object sender, EventArgs e)
        {
            Utility.WriteLog("Attempting to log in");



            if (SignIn.Instance.IsLoggedIn)
            {
                //If user is somehow already logged in, return
                return;
            }

            //disable button to prevent multiple login attempts at once
            btn_login.Enabled = false;

            //Login
            StatusCode loginStatus =
                SignIn.Instance.LogIn(loginTuserInput.Text, loginPasswordInput.Text);

            //If succesful, hide login button
            loginLink.Visible = SignIn.Instance.User == null ? true : false;

            switch (loginStatus)
            {

                //SUCCESS
                case StatusCode.OK:
                    //Next postback / pageload will change the ui and welcome the user
                    Session["NewLogin"] = SignIn.Instance.ToString();
                    Utility.WriteLog("Login success");
                    //Trigger a page reload 
                    Response.Redirect(Request.RawUrl);

                    break;

                //INVALID CREDENTIALS
                case StatusCode.FORHINDRING:
                    btn_login.Enabled = true;
                    loginStatusText.Text = "Forkert TUser eller kode!\n (prøv evt. testbrugeren: \"T210672\"+\"andr4376\")";
                    break;

                //TECHNICAL ERROR
                case StatusCode.ERROR:
                    btn_login.Enabled = true;
                    loginStatusText.Text = "API Fejl ved hentning af login info, kontakt venligst it-support! " +
                        Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.SUPPORT_EMAIL);
                    break;
            }

            //Reopen the login modal in case of invalid credentials / error
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        }

    }
    /*<!--Help box-->
        <div class="panel-group col-md-2 help-box">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapse1">Collapsible list group</a>
                </h4>
            </div>
            <div id="collapse1" class="panel-collapse collapse">
                <ul class="list-group">
                    <li class="list-group-item">1. Log ind via navigationsbaren (Bruger: "T196543" PassWord: "andr4376")</li>
                    <li class="list-group-item">Two</li>
                    <li class="list-group-item">Three</li>
                </ul>
                <div class="panel-footer">Footer</div>
            </div>
        </div>
    </div>
        <!--Help box END-->*/

}

