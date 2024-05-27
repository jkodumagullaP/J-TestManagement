using CTInfrastructure;
using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;

namespace CTWebsite
{
    public partial class ThreeColumn : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];
         //   AppVers.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
           // EnvName.Text = TestRunEnvironment;
            Constants.LocalRootPath = Server.MapPath("~").TrimEnd(new[] { '/', '\\' });
            Constants.NetworkRootPath = ResolveClientUrl("~").TrimEnd(new[] { '/', '\\' });

            String User = AspUtilities.GetUserFullName(AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name));
            Label HeadLoginFullName = (Label)AspUtilities.FindControlRecursive(this, "HeadLoginFullName");

            if (User != null)
            {
                HeadLoginFullName.Text = AspUtilities.GetUserFullName(AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name));
            }
            else
            {
                if (!Request.RawUrl.Contains("Register.aspx")
                 && !Request.RawUrl.Contains("Login.aspx"))
                {
                    if (String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)) // We're logged in but not registered
                    {
                        Response.Redirect("~/Account/Login.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.RawUrl));
                        return;
                    }

                    // Logged in, but username not in the database
                    Response.Redirect("~/Account/Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.RawUrl));
                    return;
                }

            }

            if (!CTMethods.IsAdmin(AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name)))
            {
                //If not an admin, disable Admin tab!
                AdminButton.Visible = false;
            }

            //Display Company Info in Help dropdown
            DataTable CompanyInfo = DatabaseUtilities.GetDataTable("SELECT * FROM CompanyInfo");

            if (CompanyInfo.Rows.Count > 0)
            {
                foreach (DataRow row in CompanyInfo.Rows)
                {
                    //Store the data in variables
                    String CreatectDocumentationURL = row["ctDocumentationURL"].ToString();
                    String CreateCTBugURL = row["CTProjectCreateBugURL"].ToString();
                    String ContactEmail = row["ContactEmail"].ToString();

                    ctDocumentationURL.NavigateUrl = CreatectDocumentationURL;
                    ctCreateBug.NavigateUrl = CreateCTBugURL;
                    ctContact.NavigateUrl = "mailto:" + ContactEmail;
                }
            }

        }
            
        protected void ibtnReportBug_Click(object sender, ImageClickEventArgs e)
        {
            String URL = DatabaseUtilities.GetTextFromQuery("SELECT CTProjectCreateBugURL FROM CompanyInfo Where CompanyID = 1");
            Response.Redirect(URL);
        }
        

        protected void ibtnWiki_Click(object sender, ImageClickEventArgs e)
        {
            String URL = DatabaseUtilities.GetTextFromQuery("SELECT ctDocumentationURL FROM CompanyInfo Where CompanyID = 1");
            Response.Redirect(URL);
        }
    }
}
