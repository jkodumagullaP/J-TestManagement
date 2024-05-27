using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using CTInfrastructure;
using Utilities;

namespace CTWebsite.Account
{
    public partial class RetrievePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PasswordRecovery PWRecovery = (PasswordRecovery)AspUtilities.FindControlRecursive(this, "PWRecovery");
            if (IsPostBack)
            {
                Literal personname = (Literal)AspUtilities.FindControlRecursive(this, "personname");

                //Get their real name from the UserProfiles table using their username
                if (personname != null && Request.Cookies["usernameCookie"] != null)
                {
                    personname.Text = Request.Cookies["usernameCookie"].Value;
                }
            }
        }
        protected void validateUserEmail(object sender, LoginCancelEventArgs e)
        {
            TextBox txtEmail = (TextBox)AspUtilities.FindControlRecursive(this, "txtEmail");

            Literal ErrorLiteral = (Literal)AspUtilities.FindControlRecursive(this, "ErrorLiteral");
            PasswordRecovery PWRecovery = (PasswordRecovery)AspUtilities.FindControlRecursive(this, "PWRecovery");

            MembershipUser User = Membership.GetUser(PWRecovery.UserName);

            if (User != null) // The username exists
            {
                if (User.Email.Equals(txtEmail.Text)) // Their email matches
                {

                    //ProfileCommon newProfile = Profile.GetProfile(PWRecovery.UserName);

                    HttpCookie appCookie = new HttpCookie("usernameCookie");
                    appCookie.Value = AspUtilities.GetUserFullName(PWRecovery.UserName);
                    appCookie.Expires = DateTime.Now.AddMinutes(3);
                    Response.Cookies.Add(appCookie);
                }
                else
                {
                    e.Cancel = true;
                    ErrorLiteral.Text = "This email address does not match the stored email address for the username you provided.";
                }
            }
            else
            {
                e.Cancel = true;
                ErrorLiteral.Text = "Username does not exist.";
            }

        }
    }
}