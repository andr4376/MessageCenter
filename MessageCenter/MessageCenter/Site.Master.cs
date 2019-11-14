using MessageCenter.Code;
using System;
using System.Collections.Generic;
using System.Linq;
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

        

        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            Utility.WriteLog("Attempting to log in");

            if (SignIn.Instance.IsLoggedIn)
            {
                return;
            }

            ReturnCode loginStatus = SignIn.Instance.LogIn(loginTuserInput.Text, loginPasswordInput.Text);

            loginLink.Visible = SignIn.Instance.User == null ? true : false;

            switch (loginStatus)
            {
                case ReturnCode.OK:
                    loginStatusText.Text = "Velkommen " + SignIn.Instance.ToString();
                    loginPasswordInput.Visible = false;
                    loginTuserInput.Visible = false;
                    btn_login.Visible = false;
                    btn_login.UseSubmitBehavior = false;
                    Utility.WriteLog("Login success");

                    break;
                case ReturnCode.FORHINDRING:
                    loginStatusText.Text = "Forkert TUser eller kode!";
                    break;
                case ReturnCode.ERROR:
                    loginStatusText.Text = "Fejl ved hentning af login info, kontakt venligst it-support!";
                    break;                
            }           

            

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

        }

    }

}

