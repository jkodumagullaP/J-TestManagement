using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;

namespace OSTMSWebsite.Analytics
{
    public partial class AddNewTestCase : System.Web.UI.Page
    {

        protected global::System.Web.UI.WebControls.DropDownList ddlProject;
        protected global::System.Web.UI.WebControls.Label lblCreatedBy;
        protected global::System.Web.UI.WebControls.Label lblDateCreated;
        protected global::System.Web.UI.WebControls.Label MessageLabel;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                //listBoxScreenshots.Attributes.Add("disabled", "true");

                ddlProject = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlProject");
                //ddlProject.DataBind();

                //Initialize gridviews and dropdown boxes.
                if (ddlProject.Items.Count > 0)
                {
                    ddlProject.Items.Insert(0, "Select a Project");
                    ddlProject.Items[0].Value = "";
                    ddlProject.SelectedIndex = 0;
                }

                if (HttpContext.Current.Session["CurrentProject"] != null)
                {
                    ddlProject.SelectedValue = HttpContext.Current.Session["CurrentProject"].ToString();

                    SelectedProjectChanged();
                }
                else
                {
                    // Get logged in user's default project
                    string defaultProject = DatabaseUtilities.GetDefaultProject(HttpContext.Current.User.Identity);

                    if (defaultProject != null)
                    {
                        ddlProject.SelectedValue = defaultProject;

                        SelectedProjectChanged();
                    }
                }

                DropDownList ddlSprints = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlSprints");

                //sqlSprints.DataBind();

                //Initialize gridviews and dropdown boxes.
                if (ddlSprints.Items.Count == 0 || ddlSprints.Items[0].Value != "")
                {
                    ddlSprints.Items.Insert(0, "Select a Sprint");
                    ddlSprints.Items[0].Value = "";
                    ddlSprints.SelectedIndex = 0;
                }

                if (User.Identity.IsAuthenticated)
                {
                    lblCreatedBy = (Label)AspUtilities.FindControlRecursive(this, "lblCreatedBy");
                    lblDateCreated = (Label)AspUtilities.FindControlRecursive(this, "lblDateCreated");
                    MessageLabel = (Label)AspUtilities.FindControlRecursive(this, "MessageLabel");

                    lblCreatedBy.Text = DatabaseUtilities.GetUserFullName(DatabaseUtilities.RemovePrefixFromUserName(User.Identity.Name));
                    lblDateCreated.Text = DateTime.Now.ToString();
                    MessageLabel.Text = "";
                }


                SetLeftColumnContentParams();
            }

            //ddlProject = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlProject");

            //SqlDataSource sqlSprints = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlSprints");
            //sqlSprints.SelectCommand = "SELECT [sprint] FROM [Sprints] where projectAbbreviation = '" + ddlProject.SelectedValue + "' ORDER BY [Sprint] DESC";

            
        }

        public void SetLeftColumnContentParams()
        {
            // Releases
            SqlDataSource sqlReleases = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlReleases");

            if (sqlReleases.SelectParameters["projectAbbreviation"] == null)
            {
                Parameter projectAbbreviationParam = new Parameter();
                projectAbbreviationParam.Name = "projectAbbreviation";
                sqlReleases.SelectParameters.Add(projectAbbreviationParam);
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }
            else
            {
                Parameter projectAbbreviationParam = sqlReleases.SelectParameters["projectAbbreviation"];
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }

            ListBox listBoxReleases = (ListBox)AspUtilities.FindControlRecursive(this, "listBoxReleases");
            listBoxReleases.DataBind();



            // Sprints
            SqlDataSource sqlSprints2 = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlSprints2");

            if (sqlSprints2.SelectParameters["projectAbbreviation"] == null)
            {
                Parameter projectAbbreviationParam = new Parameter();
                projectAbbreviationParam.Name = "projectAbbreviation";
                sqlSprints2.SelectParameters.Add(projectAbbreviationParam);
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }
            else
            {
                Parameter projectAbbreviationParam = sqlSprints2.SelectParameters["projectAbbreviation"];
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }

            ListBox listBoxSprints = (ListBox)AspUtilities.FindControlRecursive(this, "listBoxSprints");
            listBoxSprints.DataBind();
        }

        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            SelectedProjectChanged();
        }

        private void SelectedProjectChanged()
        {
            ddlProject = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlProject");

            SqlDataSource sqlSprints = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlSprints");
            sqlSprints.SelectCommand = "SELECT [sprint] FROM [Sprints] where projectAbbreviation = '" + ddlProject.SelectedValue + "' ORDER BY [Sprint] DESC";

            DropDownList ddlSprints = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlSprints");
            ddlSprints.DataBind();

            //fvTestCaseDetails.DataBind();

            //ddlSprints.DataBind();
            if (ddlSprints.Items.Count == 0 || ddlSprints.Items[0].Value != "")
            {
                ddlSprints.Items.Insert(0, "Select a Sprint");
                ddlSprints.Items[0].Value = "";
                ddlSprints.SelectedIndex = 0;
            }

            SetLeftColumnContentParams();
        }

        protected void ddlSprints_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            SqlDataSource sqlAddTestCase = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlAddTestCase");

            DropDownList ddlSprints = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlSprints");

            sqlAddTestCase.InsertCommand = "INSERT INTO [TestCases] ([testCaseId], [projectAbbreviation], [testCaseDescription], [testCaseSteps], [expectedResults], [testCaseNotes], [dateLastUpdated], [updatedBy], [updateSprint], [updateStory], [dateCreated]) VALUES (@testCaseId, @projectAbbreviation, @testCaseDescription, @testCaseSteps, @expectedResults, @testCaseNotes, @dateLastUpdated, @updatedBy, '" + ddlSprints.SelectedValue + "', @updateStory, @dateCreated)";
            */
        }

        protected void ddlSprints_DataBound(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            FormView frmV = (FormView)ddl.NamingContainer;
            if (frmV.DataItem != null)
            {
                string strCity = ddl.SelectedValue.ToString();
                DropDownList ddlSubdivision = (DropDownList)frmV.FindControl("ddlSprints");
                ddl.ClearSelection();

                ListItem lm = ddl.Items.FindByValue(strCity);
                if (lm == null)
                { 
                    ddl.SelectedIndex = 0; 
                }
                else
                { 
                    lm.Selected = true; 
                }
            }
        }

        protected void CancelButton_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/FunctionalTesting/TestCases.aspx");
        }

        protected void InsertButton_OnClick(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlProject");
            TextBox txtbxEditDescription = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditDescription");
            TextBox txtbxEditTestCaseSteps = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditTestCaseSteps");
            TextBox txtbxEditExpectedResults = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditExpectedResults");
            TextBox txtbxEditNotes = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditNotes");
            DropDownList ddlSprints = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlSprints");
            TextBox txtbxEditUpdateStory = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditUpdateStory");
            Label MessageLabel = (Label)AspUtilities.FindControlRecursive(this, "MessageLabel");

            if (ddlProject.SelectedValue == "")
            {
                MessageLabel.Text = "Insert FAILED, please select a project";
                return;
            }

            if (txtbxEditDescription.Text == "")
            {
                MessageLabel.Text = "Insert FAILED, please enter a description";
                return;
            }

            if (txtbxEditTestCaseSteps.Text == "")
            {
                MessageLabel.Text = "Insert FAILED, please enter test case steps";
                return;
            }

            if (txtbxEditExpectedResults.Text == "")
            {
                txtbxEditExpectedResults.Text = "NONE";
            }

            try
            {
                int testCaseId = DatabaseUtilities.InsertTestCase
                    (
                        ddlProject.SelectedValue,
                        txtbxEditDescription.Text,
                        txtbxEditTestCaseSteps.Text,
                        txtbxEditExpectedResults.Text,
                        txtbxEditNotes.Text,
                        DatabaseUtilities.RemovePrefixFromUserName(User.Identity.Name),
                        String.IsNullOrEmpty(ddlSprints.SelectedValue) ? null : ddlSprints.SelectedValue,
                        txtbxEditUpdateStory.Text,
                        "",
                        null,
                        "Analytics",
                        null,
                        null
                    );

                MessageLabel.Text = "Inserted TestCaseId " + testCaseId + " Successfully";

                txtbxEditDescription.Text = "";
                txtbxEditTestCaseSteps.Text = "";
                txtbxEditExpectedResults.Text = "";
                txtbxEditUpdateStory.Text = "";
                txtbxEditNotes.Text = "";
                ddlSprints.SelectedIndex = 0;

                // Releases
                ListBox listBoxReleases = (ListBox)AspUtilities.FindControlRecursive(this, "listBoxReleases");
                ListItemCollection listItemCollectionReleases = listBoxReleases.Items;
                foreach (ListItem listItem in listItemCollectionReleases)
                {
                    if (listItem.Selected)
                    {
                        DatabaseUtilities.AddSingleRelease(testCaseId, ddlProject.SelectedValue, listItem.Value);
                    }
                }

                // Sprints
                ListBox listBoxSprints = (ListBox)AspUtilities.FindControlRecursive(this, "listBoxSprints");
                ListItemCollection listItemCollectionSprints = listBoxSprints.Items;
                foreach (ListItem listItem in listItemCollectionSprints)
                {
                    if (listItem.Selected)
                    {
                        DatabaseUtilities.AddSingleSprint(testCaseId, ddlProject.SelectedValue, listItem.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = "Insert FAILED with error: \"" + ex.Message + "\"";
            }
        }

        protected void fvTestCaseDetails_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            // Use the Exception property to determine whether an exception
            // occurred during the insert operation.
            if (e.Exception == null)
            {
                // Use the AffectedRows property to determine whether the
                // record was inserted. Sometimes an error might occur that 
                // does not raise an exception, but prevents the insert
                // operation from completing.
                if (e.AffectedRows == 1)
                {
                    //MessageLabel.Text = "Record inserted successfully.";
                    //e.KeepInInsertMode = true;
                    //Response.Redirect("~/Admin/AdminTestCases.aspx?insert=Successful");
                    Response.Redirect("~/FunctionalTesting/TestCases.aspx");

                }
                else
                {
                    MessageLabel.Text = "An error occurred during the insert operation. This test case has not been added.";

                    // Use the KeepInInsertMode property to remain in insert mode
                    // when an error occurs during the insert operation.
                    e.KeepInInsertMode = true;
                }
            }
            else
            {
                // Insert the code to handle the exception.
                MessageLabel.Text = e.Exception.Message;

                // Use the ExceptionHandled property to indicate that the 
                // exception has already been handled.
                e.ExceptionHandled = true;
                e.KeepInInsertMode = true;
            }
        }

        

        
    }
}