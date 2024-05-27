using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using Utilities;
using CTInfrastructure;

namespace CTWebsite.Account
{
    public partial class Register : System.Web.UI.Page
    {
        protected global::System.Web.UI.WebControls.CreateUserWizard RegisterUser;
        protected void Page_Load(object sender, EventArgs e)
        {

            RegisterUser.ContinueDestinationPageUrl = Request.QueryString["ReturnUrl"];


            TextBox UserNameTextBox = (TextBox)RegisterUserWizardStep.ContentTemplateContainer.FindControl("UserName");
            if (UserNameTextBox != null)
            {
                if (!string.IsNullOrEmpty(AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name)))
                {
                    // If we already have a user name, hang on to it
                    UserNameTextBox.Enabled = false;
                    UserNameTextBox.ReadOnly = true;
                    UserNameTextBox.Text = AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name);
                }
                else
                {
                    string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

                    if (TestRunEnvironment == "LOCAL")
                    {
                        Label localUsername = (Label)RegisterUserWizardStep.ContentTemplateContainer.FindControl("localUsername");
                        localUsername.Visible = true;

                        //TextBox Password = (TextBox)RegisterUserWizardStep.ContentTemplateContainer.FindControl("Password");
                        //Password.Visible = true;
                        //Password.Text = "";

                        //Label PasswordLabel = (Label)RegisterUserWizardStep.ContentTemplateContainer.FindControl("PasswordLabel");
                        //PasswordLabel.Visible = true;

                        //TextBox ConfirmPassword = (TextBox)RegisterUserWizardStep.ContentTemplateContainer.FindControl("ConfirmPassword");
                        //ConfirmPassword.Visible = true;
                        //ConfirmPassword.Text = "";

                        //Label ConfirmPasswordLabel = (Label)RegisterUserWizardStep.ContentTemplateContainer.FindControl("ConfirmPasswordLabel");
                        //ConfirmPasswordLabel.Visible = true;
                    }
                    else
                    {
                        throw new Exception("***THIS IS AN EXPECTED EXCEPTION***\n"
                                          + "***THIS IS AN EXPECTED EXCEPTION***\n"
                                          + "***THIS IS AN EXPECTED EXCEPTION***\n"
                                          + "\n"
                                          + "The register page cannot be used unless you are using windows authentication to establish your identity. \n"
                                          + "\n"
                                          + "***THIS IS AN EXPECTED EXCEPTION***\n"
                                          + "***THIS IS AN EXPECTED EXCEPTION***\n"
                                          + "***THIS IS AN EXPECTED EXCEPTION***\n");
                    }
                }
            }
        }

        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
            
            Literal ErrorMessage2 = (Literal)AspUtilities.FindControlRecursive(this, "ErrorMessage2");

            TextBox UserNameTextBox = (TextBox)RegisterUserWizardStep.ContentTemplateContainer.FindControl("UserName");
            SqlDataSource DataSource = (SqlDataSource)RegisterUserWizardStep.ContentTemplateContainer.FindControl("InsertExtraInfo");

            MembershipUser User = Membership.GetUser(UserNameTextBox.Text);
            object UserGUID = User.ProviderUserKey;

            DataSource.InsertParameters.Add("UserId", UserGUID.ToString());

            try
            {
                DataSource.Insert();
            }
            catch (Exception)
            {
                // This will be handled by VerifyUserCreatedProperlyElseRemove
            }

            FormsAuthentication.SetAuthCookie(RegisterUser.UserName, false /* createPersistentCookie */);

            //If first user, make admin
            int userCount = CTMethods.CountUsers();
            Guid userID = DatabaseUtilities.GetGuidFromQuery("SELECT UserId FROM aspnet_Users WHERE UserName = '" + User.UserName + "'");
            if (userCount == 1)
            {
                CTMethods.MakeUserAdmin(userID);
            }

            // Last resort error handling
            string error = CTMethods.VerifyUserCreatedProperlyElseRemove(User.UserName);

            if (!String.IsNullOrEmpty(error))
            {
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                
                ErrorMessage2.Text = "";

                string continueUrl = RegisterUser.ContinueDestinationPageUrl;
                if (String.IsNullOrEmpty(continueUrl))
                {
                    continueUrl = "~/";
                }
                Response.Redirect(continueUrl);
            }
        }

        protected void RegisterUser_CreateUserError(object sender, CreateUserErrorEventArgs e)
        {
            Literal ErrorMessage2 = (Literal)AspUtilities.FindControlRecursive(this, "ErrorMessage2");
            ErrorMessage2.Text = e.CreateUserError.ToString();

            TextBox UserNameTextBox = (TextBox)RegisterUserWizardStep.ContentTemplateContainer.FindControl("UserName");
            CTMethods.RemoveUser(UserNameTextBox.Text);
        }

        protected void RegisterUser_CreatingUser(object sender, LoginCancelEventArgs e)
        {
            Literal ErrorMessage2 = (Literal)AspUtilities.FindControlRecursive(this, "ErrorMessage2");

            TextBox UserName = (TextBox)AspUtilities.FindControlRecursive(this, "UserName");
            TextBox FirstName = (TextBox)AspUtilities.FindControlRecursive(this, "FirstName");
            TextBox LastName = (TextBox)AspUtilities.FindControlRecursive(this, "LastName");
            //TextBox Password = (TextBox)AspUtilities.FindControlRecursive(this, "Password");
            //TextBox ConfirmPassword = (TextBox)AspUtilities.FindControlRecursive(this, "ConfirmPassword");
            TextBox Email = (TextBox)AspUtilities.FindControlRecursive(this, "Email");
            TextBox SupervisorFirstName = (TextBox)AspUtilities.FindControlRecursive(this, "SupervisorFirstName");
            TextBox SupervisorLastName = (TextBox)AspUtilities.FindControlRecursive(this, "SupervisorLastName");
            TextBox SupervisorEmail = (TextBox)AspUtilities.FindControlRecursive(this, "SupervisorEmail");
            //TextBox Question = (TextBox)AspUtilities.FindControlRecursive(this, "Question");
            //TextBox Answer = (TextBox)AspUtilities.FindControlRecursive(this, "Answer");

            DropDownList ddlProjects = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlProjects");

            ErrorMessage2.Text = "";

            if (String.IsNullOrEmpty(UserName.Text))
            {
                e.Cancel = true;
                ErrorMessage2.Text += "<br>User Name is required.";
            }
            
            if (AspUtilities.GetUserGuid(UserName.Text) != null)
            {
                e.Cancel = true;
                ErrorMessage2.Text += "<br>User Name is already taken.";
            }

            if (String.IsNullOrEmpty(FirstName.Text))
            {
                e.Cancel = true;
                ErrorMessage2.Text += "<br>First Name is required.";
            }

            if (String.IsNullOrEmpty(LastName.Text))
            {
                e.Cancel = true;
                ErrorMessage2.Text += "<br>Last Name is required.";
            }

            //if (String.IsNullOrEmpty(Password.Text))
            //{
            //    e.Cancel = true;
            //    ErrorMessage2.Text += "<br>Password is required.";
            //}

            //if (String.IsNullOrEmpty(ConfirmPassword.Text))
            //{
            //    e.Cancel = true;
            //    ErrorMessage2.Text += "<br>ConfirmPassword is required.";
            //}

            //if (Password.Text != ConfirmPassword.Text)
            //{
            //    e.Cancel = true;
            //    ErrorMessage2.Text += "<br>Password must match ConfirmPassword.";
            //}

            if (String.IsNullOrEmpty(Email.Text))
            {
                e.Cancel = true;
                ErrorMessage2.Text += "<br>E-mail is required.";
            }

            if (String.IsNullOrEmpty(ddlProjects.SelectedValue))
            {
                e.Cancel = true;
                ErrorMessage2.Text += "<br>Default Project is required.";
            }

            //if (String.IsNullOrEmpty(Question.Text))
            //{
            //    e.Cancel = true;
            //    ErrorMessage2.Text += "<br>Security Question is required.";
            //}

            //if (String.IsNullOrEmpty(Answer.Text))
            //{
            //    e.Cancel = true;
            //    ErrorMessage2.Text += "<br>Security Answer is required.";
            //}
        }

    }
}
