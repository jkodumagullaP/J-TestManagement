using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using CTInfrastructure;

namespace CTWebsite.Admin
{
    public partial class CreateNewProject : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreateNewProject_Click(object sender, EventArgs e)
        {
            // Verify that project name is no more than 100 characters
            if (txtProjectName.Text.Length < 1 || txtProjectName.Text.Length > 100)
            {
                MessageLabel.Text = "Project name must be at least 1 character long and no more than 100 characters long";
                return;
            }
            // Verify that project abbreviation is no more than 20 characters
            if (txtProjectAbbreviation.Text.Length < 1 || txtProjectAbbreviation.Text.Length > 20)
            {
                MessageLabel.Text = "Project abbreviation must be at least 1 character long and no more than 20 characters long";
                return;
            }
            // Verify that neither already exist

            string alreadyExistsMessage = CTMethods.CheckIfProjectExists(txtProjectAbbreviation.Text, txtProjectName.Text);

            if (!String.IsNullOrEmpty(alreadyExistsMessage))
            {
                MessageLabel.Text = alreadyExistsMessage;
                return;
            }
            
            // Add to database
            try
            {
                CTMethods.InsertProject(txtProjectAbbreviation.Text, txtProjectName.Text);
            }
            catch (Exception ex)
            {
                MessageLabel.Text = "Error while creating this project: \"" + ex.Message + "\"";
                return;
            }
            try
            {
                CTMethods.SetupProjectBrowsers(txtProjectAbbreviation.Text);
            }
            catch (Exception ex)
            {
                MessageLabel.Text = "Error while trying to setup browsers for this project: \"" + ex.Message + "\"";
                return;
            }

            // Redirect User back to AdminProject.aspx
            Response.Redirect("~/Admin/AdminProjects.aspx");

            // Put any error messages in label id="MessageLabel"
        }
    }
}