using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using System.Configuration;

namespace CTWebsite.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected global::System.Web.UI.WebControls.HyperLink RegisterHyperLink;
        protected void Page_Load(object sender, EventArgs e)
        {
            bool firstUser = CheckForFirstUser();
            if (firstUser)
            {
                Response.Redirect("Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]));
            }
            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

            if (TestRunEnvironment == "LOCAL")
            {
                Label localUsername = (Label)AspUtilities.FindControlRecursive(this, "PasswordLabel");
                localUsername.Text = "Password (always \"localtest\" on local):";
            }

            RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
        }

        private bool CheckForFirstUser()
        {

            int numberOfUsers = DatabaseUtilities.GetIntFromQuery("SELECT COUNT(*) FROM aspnet_Users");
            if (numberOfUsers > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
