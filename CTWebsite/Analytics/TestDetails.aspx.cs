using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;

namespace OSTMSWebsite.Analytics
{
    public partial class AnalyticsTestDetails : System.Web.UI.Page
    {
        protected global::System.Web.UI.WebControls.Label lblResultInsertIndicator;

        protected void Page_Load(object sender, EventArgs e)
        {
            SqlDataSource ods = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlTestCaseGroups");
            if (ods.SelectParameters["username"] == null)
            {
                Parameter userParam = new Parameter();
                userParam.Name = "username";
                userParam.DefaultValue = DatabaseUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods.SelectParameters.Add(userParam);
            }

            SqlDataSource ods2 = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlGroupTests");
            if (ods2.SelectParameters["username"] == null)
            {
                Parameter userParam2 = new Parameter();
                userParam2.Name = "username";
                userParam2.DefaultValue = DatabaseUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods2.SelectParameters.Add(userParam2);
            }

            DropDownList ddlSprints = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlSprints");

            if (ddlSprints.SelectedValue == null)
            {
                ddlSprints.SelectedValue = "";
            }
        }

        protected void ibtnRemove_OnClick(object sender, EventArgs e)
        {
            ImageButton ibtnRemove = (ImageButton)sender;

            List<string> split = ibtnRemove.CommandArgument.Split(new[] { '|' }).ToList();
            string projectAbbreviation = split[0];
            string testCaseId = split[1];
            string imageId = split[2];

            DatabaseUtilities.DeleteTestCaseScreenshot(
                projectAbbreviation,
                testCaseId,
                imageId);

            DataList dlTestCaseScreenshots = (DataList)AspUtilities.FindControlRecursive(this, "dlTestCaseScreenshots");
            dlTestCaseScreenshots.DataBind();
        }

        protected void UpdateButton_OnClick(object sender, EventArgs e)
        {
            Label lblProject = (Label)AspUtilities.FindControlRecursive(this, "lblProject");
            Label lblTestCaseId = (Label)AspUtilities.FindControlRecursive(this, "lblTestCaseId");
            TextBox txtbxEditDescription = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditDescription");
            TextBox txtbxEditExpectedResults = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditExpectedResults");
            TextBox txtbxEditNotes = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditNotes");
            TextBox txtbxEditUpdateStory = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditUpdateStory");
            Label MessageLabel = (Label)AspUtilities.FindControlRecursive(this, "MessageLabel");


            if (txtbxEditDescription.Text == "")
            {
                MessageLabel.Text = "Update FAILED, please enter a description";
                return;
            }
            else if (txtbxEditExpectedResults.Text == "")
            {
                MessageLabel.Text = "Update FAILED, please enter expected results";
                return;
            }
            else
            {
                MessageLabel.Text = "";
            }

           try
            {
                DatabaseUtilities.UpdateTestCase
                    (
                        DatabaseUtilities.RemovePrefixFromUserName(User.Identity.Name),
                        lblProject.Text,
                        Convert.ToInt32(lblTestCaseId.Text),
                        txtbxEditDescription.Text,
                        "NONE",
                        txtbxEditExpectedResults.Text,
                        txtbxEditNotes.Text,
                        String.IsNullOrEmpty(ddlSprints.SelectedValue) ? null : ddlSprints.SelectedValue,
                        txtbxEditUpdateStory.Text,
                        null,
                        null,
                        null,
                        null,
                        null
                    );

                Response.Redirect(Request.RawUrl);

            }
           catch (Exception ex)
           {
               MessageLabel.Text = "Update FAILED with error: \"" + ex.Message + "\"";
           }
        }



        protected void sqlTestCaseDetails_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            DbParameter p = e.Command.Parameters["@projectAbbreviation"];
            SqlParameter param = new SqlParameter("@user", DatabaseUtilities.RemovePrefixFromUserName(Page.User.Identity.Name));
            e.Command.Parameters.Add(param); 
        }

        protected void sqlTestCaseDetails_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            SqlParameter param = new SqlParameter("@user", DatabaseUtilities.RemovePrefixFromUserName(Page.User.Identity.Name));
            e.Command.Parameters.Add(param);
        }

        protected void sqlTestCaseDetails_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            Response.Redirect("~/Analytics/TestCases.aspx");
        }

        protected void btnAddEditResults_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/Analytics/AddEditResults.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&insert=Successful");
        }
        protected void btnViewResultHistory_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/Analytics/ResultHistory.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&insert=Successful");
        }
        protected void btnViewUpdateHistory_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/Analytics/UpdateHistory.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&insert=Successful");
        }
        protected void btnViewURLList_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/Analytics/TestURLDetails.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&insert=Successful");
        }

        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            String projectAbbreviation = Request.QueryString["project"];
            int testCaseID = Convert.ToInt32(Request.QueryString["testCase"]);
            String group = ddlGroupTests.SelectedValue;

            if (String.IsNullOrEmpty(group))
            {
                Label TopMessageLabel = (Label)AspUtilities.FindControlRecursive(this, "TopMessageLabel");
                TopMessageLabel.Text = "Please select a Group. You may need to create a Group for this Project first.";
                return;
            }
            else
            {
                TopMessageLabel.Text = "";
            }

            DatabaseUtilities.AddSingleGroupIfNeeded(testCaseID, projectAbbreviation, group);

            lvTestCaseGroups.DataBind();
        }

        protected void btnAddRelease_Click(object sender, EventArgs e)
        {
            String projectAbbreviation = Request.QueryString["project"];
            int testCaseID = Convert.ToInt32(Request.QueryString["testCase"]);
            String release = ddlReleases.SelectedValue;

            if (String.IsNullOrEmpty(release))
            {
                Label TopMessageLabel = (Label)AspUtilities.FindControlRecursive(this, "TopMessageLabel");
                TopMessageLabel.Text = "Please select a Release. You may need to create a Release for this Project first.";
                return;
            }
            else
            {
                TopMessageLabel.Text = "";
            }

            DatabaseUtilities.AddSingleReleaseIfNeeded(testCaseID, projectAbbreviation, release);

            lvTestCaseReleases.DataBind();
        }
        protected void btnAddSprint_Click(object sender, EventArgs e)
        {
            String projectAbbreviation = Request.QueryString["project"];
            int testCaseID = Convert.ToInt32(Request.QueryString["testCase"]);
            String sprint = ddlSprints.SelectedValue;

            if (String.IsNullOrEmpty(sprint))
            {
                Label TopMessageLabel = (Label)AspUtilities.FindControlRecursive(this, "TopMessageLabel");
                TopMessageLabel.Text = "Please select a Sprint. You may need to create a Sprint for this Project first.";
                return;
            }
            else
            {
                TopMessageLabel.Text = "";
            }

            DatabaseUtilities.AddSingleSprintIfNeeded(testCaseID, projectAbbreviation, sprint);

            lvSprints.DataBind();
        }

        protected void lvTestCaseGroups_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveFromGroup")
            {
                String projectAbbreviation = Request.QueryString["project"];
                int testCaseID = Convert.ToInt32(Request.QueryString["testCase"]);
                string groupTestAbbreviation = (e.CommandArgument ?? "").ToString();

                DatabaseUtilities.RemoveGroupForTestCase(projectAbbreviation, groupTestAbbreviation, testCaseID);

                lvTestCaseGroups.DataBind();
            }
        }

        protected void lvTestCaseReleases_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveFromRelease")
            {
                String projectAbbreviation = Request.QueryString["project"];
                int testCaseID = Convert.ToInt32(Request.QueryString["testCase"]);
                string release = (e.CommandArgument ?? "").ToString();

                DatabaseUtilities.RemoveReleaseForTestCase(projectAbbreviation, release, testCaseID);

                lvTestCaseReleases.DataBind();
            }
        }

        protected void lvSprints_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveFromSprint")
            {
                String projectAbbreviation = Request.QueryString["project"];
                int testCaseID = Convert.ToInt32(Request.QueryString["testCase"]);
                string sprint = (e.CommandArgument ?? "").ToString();

                DatabaseUtilities.RemoveSprintForTestCase(projectAbbreviation, sprint, testCaseID);

                lvSprints.DataBind();
            }
        }
    }
}