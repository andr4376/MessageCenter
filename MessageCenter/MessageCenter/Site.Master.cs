﻿using MessageCenter.Code;
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

            SuccesfulLoginHandler();

            MessageSentStatus();


        }

        private void MessageSentStatus()
        {
            if (Session["messageSentDescription"] != null)
            {
                KeyValuePair<StatusCode,string> messageStatus =
                 (KeyValuePair<StatusCode, string>)Session["messageSentDescription"];

                statusModalLabel.InnerText = messageStatus.Key.ToString();
                statusModalText.Text = messageStatus.Value;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openstatusModal();", true);

                Session["messageSentDescription"] = null;
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
                }

                Session["NewLogin"] = null;
            }
        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            Utility.WriteLog("Attempting to log in");

            if (SignIn.Instance.IsLoggedIn)
            {
                return;
            }
            btn_login.Enabled = false;

            StatusCode loginStatus = SignIn.Instance.LogIn(loginTuserInput.Text, loginPasswordInput.Text);

            loginLink.Visible = SignIn.Instance.User == null ? true : false;

            switch (loginStatus)
            {
                case StatusCode.OK:

                    //Next postback / pageload will change the ui and welcome the user
                    Session["NewLogin"] = SignIn.Instance.ToString();
                    Utility.WriteLog("Login success");
                    //Trigger a page reload 
                    Response.Redirect(Request.RawUrl);

                    break;
                case StatusCode.FORHINDRING:
                    btn_login.Enabled = true;
                    loginStatusText.Text = "Forkert TUser eller kode!";
                    break;
                case StatusCode.ERROR:
                    btn_login.Enabled = true;
                    loginStatusText.Text = "Fejl ved hentning af login info, kontakt venligst it-support!";
                    break;
            }


            //Reopen the login modal in case of invalid credentials / error
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);



        }

    }

}

