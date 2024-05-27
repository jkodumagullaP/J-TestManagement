using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using CTInfrastructure;

namespace CTWebsite.FunctionalTesting
{
    public partial class AddEditResults : System.Web.UI.Page
    {

        protected global::System.Web.UI.WebControls.Label tbProjectAbbreviation;
        protected global::System.Web.UI.WebControls.Label tbTestCaseId;
        protected global::System.Web.UI.WebControls.Label tbLoggedInUser;
        protected global::System.Web.UI.WebControls.Label tbLoggedInUserRaw;
        protected global::System.Web.UI.WebControls.Label tbTestDate;
        protected global::System.Web.UI.WebControls.DropDownList ddlEnvironment;
        protected global::System.Web.UI.WebControls.DropDownList ddlBrowser;
        protected global::System.Web.UI.WebControls.DropDownList ddlStatus;
        protected global::System.Web.UI.WebControls.TextBox tbTestType;
        protected global::System.Web.UI.WebControls.Label MessageLabel;

        protected void Page_Load(object sender, EventArgs e)
        {
            FormView fvTestResultDetails = (FormView)AspUtilities.FindControlRecursive(this, "fvTestResultDetails");

            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["testRunId"]))
                {
                    if (fvTestResultDetails.CurrentMode != FormViewMode.ReadOnly)
                    {
                        fvTestResultDetails.ChangeMode(FormViewMode.ReadOnly);
                    }
                }
                else
                {
                    fvTestResultDetails.ChangeMode(FormViewMode.Insert);
                    lblCommands.Visible = false;
                    btnAddScreenshot.Visible = false;
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                tbTestType = (TextBox)AspUtilities.FindControlRecursive(this, "tbTestType");
                tbProjectAbbreviation = (Label)AspUtilities.FindControlRecursive(this, "tbProjectAbbreviation");
                tbTestCaseId = (Label)AspUtilities.FindControlRecursive(this, "tbTestCaseId");
                tbLoggedInUser = (Label)AspUtilities.FindControlRecursive(this, "tbLoggedInUser");
                tbLoggedInUserRaw = (Label)AspUtilities.FindControlRecursive(this, "tbLoggedInUserRaw");
                tbTestDate = (Label)AspUtilities.FindControlRecursive(this, "tbTestDate");
                ddlEnvironment = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlEnvironment");
                ddlBrowser = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlBrowser");
                MessageLabel = (Label)AspUtilities.FindControlRecursive(this, "MessageLabel");


                tbLoggedInUserRaw.Text = AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                tbLoggedInUser.Text = AspUtilities.GetUserFullName(AspUtilities.RemovePrefixFromUserName(User.Identity.Name));
                //tbTestDate.Text = DateTime.Now.ToString();
                String tbProjectAbbreviationValue = Request.QueryString["project"];
                String tbTestCaseIdValue = Request.QueryString["testCase"];
                tbProjectAbbreviation.Text = tbProjectAbbreviationValue;
                tbTestCaseId.Text = tbTestCaseIdValue;
                tbTestType.Text = "Manual";
                MessageLabel.Text = "";
                String environment = Request.QueryString["environment"];

                if (!IsPostBack && ddlEnvironment != null)
                {
                    if (HttpContext.Current.Session["CurrentEnvironment"] != null)
                    {
                        ddlEnvironment.SelectedValue = HttpContext.Current.Session["CurrentEnvironment"].ToString();
                    }
                    else
                    {
                        ddlEnvironment.SelectedValue = environment;
                    }
                }
            }
   
        }

        protected void fvTestResultDetails_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
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
                    Response.Redirect("~/FunctionalTesting/TestDetails.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&index=1&update=Successful");

                }
                else
                {
                    MessageLabel.Text = "An error occurred during the update operation. This test result has not been recorded.";

                    // Use the KeepInInsertMode property to remain in insert mode
                    // when an error occurs during the insert operation.
                    e.KeepInEditMode = true;
                }
            }
            else
            {
                // Insert the code to handle the exception.
                MessageLabel.Text = e.Exception.Message;

                // Use the ExceptionHandled property to indicate that the 
                // exception has already been handled.
                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        protected void fvTestResultDetails_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
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
                    Response.Redirect("~/FunctionalTesting/TestDetails.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&index=1&insert=Successful");

                }
                else
                {
                    MessageLabel.Text = "An error occurred during the insert operation. This test result has not been recorded.";

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

        protected void fvTestResultDetails_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                String tbProjectAbbreviationValue = Request.QueryString["project"];
                String tbTestCaseIdValue = Request.QueryString["testCase"];
                Response.Redirect("~/FunctionalTesting/TestDetails.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&index=1&insert=Cancelled");
            }
        }

        protected void InsertButton_OnClick(object sender, EventArgs e)
        {
            DropDownList ddlEnvironment = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlEnvironment");
            DropDownList ddlBrowser = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlBrowser");
            DropDownList ddlStatus = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlStatus");

            Label tbProjectAbbreviation = (Label)AspUtilities.FindControlRecursive(this, "tbProjectAbbreviation");
            Label tbTestCaseId = (Label)AspUtilities.FindControlRecursive(this, "tbTestCaseId");
            TextBox tbReasonForStatus = (TextBox)AspUtilities.FindControlRecursive(this, "tbReasonForStatus");
            TextBox tbReasonForStatusDetailed = (TextBox)AspUtilities.FindControlRecursive(this, "tbReasonForStatusDetailed");

            TextBox tbStepsToReproduce = (TextBox)AspUtilities.FindControlRecursive(this, "tbStepsToReproduce");
            TextBox tbdefectTicketNumber = (TextBox)AspUtilities.FindControlRecursive(this, "tbdefectTicketNumber");
            
            Label MessageLabel = (Label)AspUtilities.FindControlRecursive(this, "MessageLabel");


            if (ddlEnvironment.SelectedValue == "" || ddlEnvironment.SelectedValue == "0")
            {
                MessageLabel.Text = "Insert FAILED, please enter an Environment";
                return;
            }
            else if (ddlBrowser.SelectedValue == "" || ddlBrowser.SelectedValue == "0")
            {
                MessageLabel.Text = "Insert FAILED, please enter a Browser";
                return;
            }
            else if (ddlStatus.SelectedValue == "" || ddlStatus.SelectedValue == "0")
            {
                MessageLabel.Text = "Insert FAILED, please enter a Status";
                return;
            }
            else
            {
                MessageLabel.Text = "";
            }

            try
            {
                SeleniumTestBase.InsertTestResult(
                tbProjectAbbreviation.Text,
                Convert.ToInt32(tbTestCaseId.Text),
                ddlEnvironment.SelectedValue,
                ddlBrowser.SelectedValue,
                ddlStatus.SelectedValue,
                tbReasonForStatus.Text,
                tbReasonForStatusDetailed.Text,
                tbStepsToReproduce.Text,
                tbdefectTicketNumber.Text,
                AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                "Manual"
                );

                Response.Redirect("~/FunctionalTesting/TestDetails.aspx?project=" + tbProjectAbbreviation.Text + "&testCase=" + tbTestCaseId.Text + "&index=1&insert=Successful");

            }
            catch (Exception ex)
            {
                MessageLabel.Text = "Insert FAILED with error: \"" + ex.Message + "\"";
            }
        }

        protected void UpdateButton_OnClick(object sender, EventArgs e)
        {
            Label lblEnvironment = (Label)AspUtilities.FindControlRecursive(this, "lblEnvironment");
            Label lblBrowserAbbreviation = (Label)AspUtilities.FindControlRecursive(this, "lblBrowserAbbreviation");
            DropDownList ddlStatus = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlStatus");

            Label tbProjectAbbreviation = (Label)AspUtilities.FindControlRecursive(this, "tbProjectAbbreviation");
            Label tbTestCaseId = (Label)AspUtilities.FindControlRecursive(this, "tbTestCaseId");
            TextBox tbReasonForStatus = (TextBox)AspUtilities.FindControlRecursive(this, "tbReasonForStatus");
            TextBox tbReasonForStatusDetailed = (TextBox)AspUtilities.FindControlRecursive(this, "tbReasonForStatusDetailed");
            TextBox tbStepsToReproduce = (TextBox)AspUtilities.FindControlRecursive(this, "tbStepsToReproduce");
            TextBox tbdefectTicketNumber = (TextBox)AspUtilities.FindControlRecursive(this, "tbdefectTicketNumber");

            Label MessageLabel = (Label)AspUtilities.FindControlRecursive(this, "MessageLabel");


            if (lblEnvironment.Text == "" || lblEnvironment.Text == "0")
            {
                MessageLabel.Text = "Update FAILED, please enter an Environment";
                return;
            }
            else if (lblBrowserAbbreviation.Text == "" || lblBrowserAbbreviation.Text == "0")
            {
                MessageLabel.Text = "Update FAILED, please enter a Browser";
                return;
            }
            else if (ddlStatus.SelectedValue == "" || ddlStatus.SelectedValue == "0")
            {
                MessageLabel.Text = "Update FAILED, please enter a Status";
                return;
            }
            else
            {
                MessageLabel.Text = "";
            }

            try
            {
                SeleniumTestBase.UpdateTestResultsWithTestRunID
                (
                    tbProjectAbbreviation.Text,
                    Convert.ToInt32(tbTestCaseId.Text),
                    lblEnvironment.Text,
                    lblBrowserAbbreviation.Text,
                    ddlStatus.SelectedValue,
                    tbReasonForStatus.Text,
                    tbReasonForStatusDetailed.Text,
                    tbStepsToReproduce.Text,
                    tbdefectTicketNumber.Text,
                    Convert.ToInt32(Request.QueryString["testRunId"])
                );

                Response.Redirect("~/FunctionalTesting/TestDetails.aspx?project=" + tbProjectAbbreviation.Text + "&testCase=" + tbTestCaseId.Text + "&index=1&update=Successful");

            }
            catch (Exception ex)
            {
                MessageLabel.Text = "Update FAILED with error: \"" + ex.Message + "\"";
            }
        }
        
        protected void imgTestResultScreenshots_OnClick(object sender, EventArgs e)
        {
            ImageButton imgTestResultScreenshots = (ImageButton)sender;
            Response.Redirect(imgTestResultScreenshots.ImageUrl);
        }
        
        protected void btnAddScreenshot_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            String tbTestRunId = Request.QueryString["testRunId"];
            Response.Redirect("~/FunctionalTesting/AddNewScreenshot.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&testRunId=" + tbTestRunId + "&type=TestResult");
        }

        protected void btnEditResult_Click(object sender, EventArgs e)
        {
            // Change this page to edit mode.
            
            sqlTestResultDetails.SelectCommand = "Select TestResults.*, dbo.UserProfiles.userFirstName + ' ' + dbo.UserProfiles.userLastName as TestedByFullName from [TestResults] left join dbo.aspnet_Users 	on dbo.TestResults.testedBy = aspnet_Users.UserName left join dbo.UserProfiles on aspnet_Users.userid = UserProfiles.userid where testrunid = @testRunID";

            fvTestResultDetails.ChangeMode(FormViewMode.Edit);
        }

        protected void ibtnRemove_OnClick(object sender, EventArgs e)
        {
            ImageButton ibtnRemove = (ImageButton)sender;
            /*  
             * CommandPieces[0] = projectAbbreviation
             * CommandPieces[1] = testCaseID
             * CommandPieces[2] = testRunId
             * CommandPieces[3] = imageId
            */

            List<string> split = ibtnRemove.CommandArgument.Split(new[] { '|' }).ToList();
            string projectAbbreviation = split[0];
            string testCaseId = split[1];
            string testRunId = split[2];
            string imageId = split[3];

            CTMethods.DeleteTestResultScreenshot(
                projectAbbreviation,
                testCaseId,
                testRunId,
                imageId);

            DataList dlTestResultScreenshots = (DataList)AspUtilities.FindControlRecursive(this, "dlTestResultScreenshots");
            dlTestResultScreenshots.DataBind();
        }
    }
}