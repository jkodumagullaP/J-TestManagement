using CTInfrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;

namespace CTWebsite.FunctionalTesting
{
    public partial class TestDetails : System.Web.UI.Page
    {
        protected global::System.Web.UI.WebControls.Label lblResultInsertIndicator;
        //protected global::System.Web.UI.WebControls.DropDownList ddlEditProject;

        protected int gvResultHistoryStatusColumn
        {
            get
            {
                return AspUtilities.GetColumnIndexByDBName(gvResultHistory, "status");
            }
        }

        protected int gvResultHistoryReasonForStatusColumn
        {
            get
            {
                return AspUtilities.GetColumnIndexByDBName(gvResultHistory, "reasonForStatus");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];

            int tcid = Convert.ToInt32(tbTestCaseIdValue);


            lblTCID.Text = tbProjectAbbreviationValue + "-" + tbTestCaseIdValue;

            SqlDataSource ods = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlTestCaseGroups");
            
            if (ods.SelectParameters["username"] == null)
            {
                Parameter userParam = new Parameter();
                userParam.Name = "username";
                userParam.DefaultValue = AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods.SelectParameters.Add(userParam);
            }

            SqlDataSource ods2 = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlGroupTests");
            if (ods2.SelectParameters["username"] == null)
            {
                Parameter userParam2 = new Parameter();
                userParam2.Name = "username";
                userParam2.DefaultValue = AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods2.SelectParameters.Add(userParam2);
            }

            gvUpdateHistory.Caption = "Update History for " + tbProjectAbbreviationValue + "-" + tbTestCaseIdValue;
            gvResultHistory.Caption = "Result History for " + tbProjectAbbreviationValue + "-" + tbTestCaseIdValue;
            lblAutomationTestCaseProject.Text = tbProjectAbbreviationValue;
            lblAutomationTestCaseId.Text = tbTestCaseIdValue;


            if (!IsPostBack)
            {
                //Add items to the Automated dropdown list.
                ddlAutomated.Items.Add("");
                ddlAutomated.Items.Add("Yes");
                ddlAutomated.Items.Add("No");
                ddlAutomated.Items.Add("Future");

                refreshDatagrid();
                
         
                
                if (!string.IsNullOrEmpty(Request.QueryString["index"]))
                {
                    TabContainer1.ActiveTabIndex = int.Parse(Request.QueryString["index"]);
                }

                //Environment
                ddlEnvironments.DataBind();

                //If an environment is stored in a session variable, select it
                if (HttpContext.Current.Session["CurrentEnvironment"] != null)
                {
                    ddlEnvironments.SelectedValue = HttpContext.Current.Session["CurrentEnvironment"].ToString();
                }
                else
                {
                        // Select the project's default environment
                    string defaultEnvironment = CTMethods.GetDefaultEnvironment(tbProjectAbbreviationValue);
                    if (defaultEnvironment != null)
                    {
                        ddlEnvironments.SelectedValue = defaultEnvironment;
                    }
                }

                //If there is still nothing in environments. Put something there.
                if (ddlEnvironments.Items.Count == 0
                    || ddlEnvironments.Items[0].Value != "")
                {
                    ddlEnvironments.Items.Insert(0, "No Environment Available");
                    ddlEnvironments.Items[0].Value = "";
                }
                
                ddlEnvironments.DataBind();

                ddlBrowsers.DataBind();

                if (ddlBrowsers.Items.Count == 0
                    || ddlBrowsers.Items[0].Value != "")
                {
                    ddlBrowsers.Items.Insert(0, "All");
                    ddlBrowsers.Items[0].Value = "";
                }
                //ddlBrowsers.DataBind();
                FiltersChanged();
                PrePopulateFields();
                //Disable appropriate navigation buttons if the current id is null or at the beginning or the end

                if (tbTestCaseIdValue != null && Convert.ToInt32(tbTestCaseIdValue) != 0)
                {
                    int firstTCID = CTMethods.GetFirstLastPreviousNextIds(tbProjectAbbreviationValue, Convert.ToInt32(tbTestCaseIdValue), "First");
                    int backTCID = CTMethods.GetFirstLastPreviousNextIds(tbProjectAbbreviationValue, Convert.ToInt32(tbTestCaseIdValue), "Back");
                    int nextTCID = CTMethods.GetFirstLastPreviousNextIds(tbProjectAbbreviationValue, Convert.ToInt32(tbTestCaseIdValue), "Next");
                    int lastTCID = CTMethods.GetFirstLastPreviousNextIds(tbProjectAbbreviationValue, Convert.ToInt32(tbTestCaseIdValue), "Last");

                    btnFirst.Enabled = (firstTCID != 0);
                    btnBack.Enabled = (backTCID != 0);
                    btnNext.Enabled = (nextTCID != 0);
                    btnLast.Enabled = (lastTCID != 0);
                }
                else
                {
                    btnFirst.Enabled = false;
                    btnBack.Enabled = false;
                    btnNext.Enabled = false;
                    btnLast.Enabled = false;
                }
            }
        }

        private void refreshDatagrid()
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            
            SqlDataSource sqlChildAutomatedTestCases = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlChildAutomatedTestCases");
            int testCaseID;

            if (!String.IsNullOrEmpty(tbProjectAbbreviationValue)
                && !String.IsNullOrEmpty(tbTestCaseIdValue)
                && Int32.TryParse(tbTestCaseIdValue, out testCaseID))
            {
                sqlChildAutomatedTestCases.SelectCommand =
                    "select "
                    + " t.projectAbbreviation, "
                    + " childTestCaseId, "
                    + " testCaseDescription "
                    + " from AutoTestCaseMap a "
                    + " join TestCases t "
                    + " on t.projectAbbreviation = a.projectAbbreviation "
                    + " and t.testcaseid = a.childTestCaseId "
                    + " where t.projectAbbreviation = '" + tbProjectAbbreviationValue + "' and parentTestCaseId = " + tbTestCaseIdValue
                    + " order by childTestCaseId";
            }
            else
            {
                sqlChildAutomatedTestCases.SelectCommand = "";
            }

            sqlChildAutomatedTestCases.DataBind();

            ListView lvChildAutomatedTestCases = (ListView)AspUtilities.FindControlRecursive(this, "lvChildAutomatedTestCases");
            lvChildAutomatedTestCases.DataBind();
        }


        protected void FiltersChanged()
        {
            SqlDataSource sqlSingleTestCaseHistory = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlSingleTestCaseHistory");
            sqlSingleTestCaseHistory.SelectCommand = "SELECT "
            + "ProjectBrowserInfo.showBrowserColumn "
            + " ,TestResultsView.testRunId "
            + " ,TestResultsView.testCaseId "
            + " ,TestResultsView.projectAbbreviation "
            + " ,TestResultsView.environment "
            + " ,TestResultsView.browserAbbreviation "
            + " ,TestResultsView.status "
            + " ,TestResultsView.reasonForStatus "
            + " ,TestResultsView.reasonForStatusDetailed "
            + " ,TestResultsView.stepsToReproduce "
            + " ,TestResultsView.defectTicketNumber "
            + " ,TestResultsView.testDate "
            + " ,TestResultsView.testedBy "
            + " ,TestResultsView.testType "
            + " ,TestResultsView.rawReasonForStatus "
            + " ,TestResultsView.rawReasonForStatusDetailed "
            + " ,TestResultsView.rawStepsToReproduce "
            + " ,right('0' + rtrim(convert(char(2), elapsedSeconds / (60 * 60))), 2) + ':' + right('0' + rtrim(convert(char(2), (elapsedSeconds / 60) % 60)), 2) + ':' + right('0' + rtrim(convert(char(2), elapsedSeconds % 60)),2) as [elapsedSeconds] "
            + " ,dbo.UserProfiles.userFirstName + ' ' + dbo.UserProfiles.userLastName as TestedByFullName "
            + " , automationNode "
            + " FROM [TestResultsView] "
            + " INNER JOIN ProjectBrowserInfo "
            + " ON TestResultsView.browserAbbreviation = ProjectBrowserInfo.browserAbbreviation "
            + " LEFT JOIN dbo.aspnet_Users 	on dbo.TestResultsView.testedBy = aspnet_Users.UserName "
            + " LEFT JOIN dbo.UserProfiles on aspnet_Users.userid = UserProfiles.userid "
            + " WHERE ((ProjectBrowserInfo.projectAbbreviation = @projectAbbreviation) "
            + " AND (TestResultsView.testCaseId = @testCaseId) "
            + " AND (ProjectBrowserInfo.showBrowserColumn='1'))";
            
            if (ddlEnvironments.SelectedValue != "")
            {
                sqlSingleTestCaseHistory.SelectCommand += " and TestResultsView.environment = '" + ddlEnvironments.SelectedValue + "' ";
            }

            if (ddlBrowsers.SelectedValue != "")
            {
                sqlSingleTestCaseHistory.SelectCommand += " and ProjectBrowserInfo.browserAbbreviation = '" + ddlBrowsers.SelectedValue + "' ";
            }

            sqlSingleTestCaseHistory.SelectCommand += " order by testDate desc";

            sqlSingleTestCaseHistory.DataBind();

        }

        protected void NavigationButtons_OnClientClick(object sender, CommandEventArgs e)
        { 
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            int tcid = CTMethods.GetFirstLastPreviousNextIds(tbProjectAbbreviationValue, Convert.ToInt32(tbTestCaseIdValue), e.CommandName);
            int index = TabContainer1.ActiveTabIndex;
            Response.Redirect("~/FunctionalTesting/TestDetails.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tcid.ToString() + "&index=" + index);
        }
        protected void gvResultHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    int i = 0;
                    i = gvResultHistoryStatusColumn;
                    if (e.Row.Cells[i].Text == "Pass")
                    {
                        e.Row.Cells[i].CssClass = "statusPass";
                        e.Row.Cells[i].ToolTip = "Pass";
                        e.Row.Cells[i].Text = "";
                    }
                    else if (e.Row.Cells[i].Text == "Fail")
                    {
                        e.Row.Cells[i].CssClass = "statusFail";
                        e.Row.Cells[i].ToolTip = "Fail";
                        e.Row.Cells[i].Text = "";
                    }
                    else if (e.Row.Cells[i].Text == "Test Case Needs Updated")
                    {
                        e.Row.Cells[i].CssClass = "statusTestCaseNeedsUpdated";
                        e.Row.Cells[i].ToolTip = "test Case Needs Updated";
                        e.Row.Cells[i].Text = "";
                    }
                    else if (e.Row.Cells[i].Text == "Automated Test Needs Updated")
                    {
                        e.Row.Cells[i].CssClass = "statusAutomatedTestNeedsUpdated";
                        e.Row.Cells[i].ToolTip = "Automated Case Needs Updated";
                        e.Row.Cells[i].Text = "";
                    }
                    else if (e.Row.Cells[i].Text == "Not Started")
                    {
                        e.Row.Cells[i].CssClass = "statusNotStarted";
                        e.Row.Cells[i].ToolTip = "Not Started";
                        e.Row.Cells[i].Text = "";
                    }
                    else if (e.Row.Cells[i].Text == "Not Implemented")
                    {
                        e.Row.Cells[i].CssClass = "statusNotImplemented";
                        e.Row.Cells[i].ToolTip = "Not Implemented";
                        e.Row.Cells[i].Text = "";
                    }
                    else if (e.Row.Cells[i].Text == "Not Applicable")
                    {
                        e.Row.Cells[i].CssClass = "statusNotApplicable";
                        e.Row.Cells[i].ToolTip = "Not Applicable";
                        e.Row.Cells[i].Text = "";
                    }
                    else if (e.Row.Cells[i].Text == "In Progress")
                    {
                        e.Row.Cells[i].CssClass = "statusInProgress";
                        e.Row.Cells[i].ToolTip = "In Progress";
                        e.Row.Cells[i].Text = "";
                    }
                    else if (e.Row.Cells[i].Text == "In Queue")
                    {
                        e.Row.Cells[i].CssClass = "statusInQueue";
                        e.Row.Cells[i].ToolTip = "In Queue";
                        e.Row.Cells[i].Text = "";
                    }
                    else if (e.Row.Cells[i].Text == "Retest")
                    {
                        e.Row.Cells[i].CssClass = "statusRetest";
                        e.Row.Cells[i].ToolTip = "Retest";
                        e.Row.Cells[i].Text = "";
                    }
                    gvResultHistory.Attributes.Add("style", "word-break:keep-all;word-wrap:normal");
                
            }
        }
        
        protected void ddlEnvironments_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentEnvironment"] = ddlEnvironments.SelectedValue;
            FiltersChanged();
        }
        protected void ddlBrowsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltersChanged();
        }

        protected void gvResultHistory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
            string CommandArgument = (e.CommandArgument ?? "").ToString();
            string CommandName = (e.CommandName ?? "").ToString();
            string CommandSource = (e.CommandSource ?? "").ToString();
            FiltersChanged();
        }

        protected void gvResultHistory_PageIndexChanged(object sender, EventArgs e)
        {
            FiltersChanged();
        }

        protected void ibtnRemove_OnClick(object sender, EventArgs e)
        {
            ImageButton ibtnRemove = (ImageButton)sender;

            List<string> split = ibtnRemove.CommandArgument.Split(new[] { '|' }).ToList();
            string projectAbbreviation = split[0];
            string testCaseId = split[1];
            string imageId = split[2];

            CTMethods.DeleteTestCaseScreenshot(
                projectAbbreviation,
                testCaseId,
                imageId);

            DataList dlTestCaseScreenshots = (DataList)AspUtilities.FindControlRecursive(this, "dlTestCaseScreenshots");
            dlTestCaseScreenshots.DataBind();
        }

        protected void chkActive_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkActive = (CheckBox)AspUtilities.FindControlRecursive(this, "chkActive");
            String projectAbbreviation = Request.QueryString["project"];
            String tcid = Request.QueryString["testCase"];

            string query = "Update [TestCases] set  active = '" + chkActive.Checked + "' where [projectAbbreviation] = '" + projectAbbreviation + "' and [testCaseId] = '" + tcid + "'";
            DatabaseUtilities.ExecuteQuery(query);
        }

        protected void chkTestCaseOutdated_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkTestCaseOutdated = (CheckBox)AspUtilities.FindControlRecursive(this, "chkTestCaseOutdated");
            String projectAbbreviation = Request.QueryString["project"];
            String tcid = Request.QueryString["testCase"];

            string query = "Update [TestCases] set  testCaseOutdated = '" + chkTestCaseOutdated.Checked + "' where [projectAbbreviation] = '" + projectAbbreviation + "' and [testCaseId] = '" + tcid + "'";
            DatabaseUtilities.ExecuteQuery(query);
        }

        protected void chkTestScriptOutdated_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkTestScriptOutdated = (CheckBox)AspUtilities.FindControlRecursive(this, "chkTestScriptOutdated");
            String projectAbbreviation = Request.QueryString["project"];
            String tcid = Request.QueryString["testCase"];

            string query = "Update [TestCases] set  testScriptOutdated = '" + chkTestScriptOutdated.Checked + "' where [projectAbbreviation] = '" + projectAbbreviation + "' and [testCaseId] = '" + tcid + "'";
            DatabaseUtilities.ExecuteQuery(query);
        }







        protected void UpdateButton_OnClick(object sender, EventArgs e)
        {
            Label lblProject = (Label)AspUtilities.FindControlRecursive(this, "lblProject");
            Label lblTestCaseId = (Label)AspUtilities.FindControlRecursive(this, "lblTestCaseId");
            TextBox txtbxEditDescription = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditDescription");
            CheckBox chkActive = (CheckBox)AspUtilities.FindControlRecursive(this, "chkActive");
            CheckBox chkTestCaseOutdated = (CheckBox)AspUtilities.FindControlRecursive(this, "chkTestCaseOutdated");
            CheckBox chkTestScriptOutdated = (CheckBox)AspUtilities.FindControlRecursive(this, "chkTestScriptOutdated");
            TextBox txtbxEditTestCaseSteps = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditTestCaseSteps");
            TextBox txtbxEditExpectedResults = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditExpectedResults");
            TextBox txtbxEditNotes = (TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditNotes");
            Label MessageLabel = (Label)AspUtilities.FindControlRecursive(this, "MessageLabel");
            TextBox txtbxTestCategory = (TextBox)AspUtilities.FindControlRecursive(this, "txtCategory");


            if (txtbxEditDescription.Text == "")
            {
                MessageLabel.Text = "Update FAILED, please enter a description";
                return;
            }
            else if (txtbxEditTestCaseSteps.Text == "")
            {
                MessageLabel.Text = "Update FAILED, please enter test case steps";
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
                CTMethods.partialUpdateTestCase
                    (
                        AspUtilities.RemovePrefixFromUserName(User.Identity.Name),
                        lblProject.Text,
                        Convert.ToInt32(lblTestCaseId.Text),
                        txtbxEditDescription.Text,
                        chkActive.Checked,
                        chkTestCaseOutdated.Checked,
                        chkTestScriptOutdated.Checked,
                        txtbxEditTestCaseSteps.Text,
                        txtbxEditExpectedResults.Text,
                        txtbxEditNotes.Text,
                        txtbxTestCategory.Text
                    );

                Response.Redirect(Request.RawUrl);

            }
           catch (Exception ex)
           {
               MessageLabel.Text = "Update FAILED with error: \"" + ex.Message + "\"";
           }
        }

        protected void fvTestCaseDetails_PreRender(object sender, EventArgs e)
        {
            if (fvTestCaseDetails.CurrentMode == FormViewMode.Edit)
            {
                TextBox Category = (TextBox)AspUtilities.FindControlRecursive(this, "txtCategory");
                if (User.IsInRole("Admin") && Category != null)
                {
                    Category.Enabled = true;
                }
            }
        }

        protected void sqlTestCaseDetails_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            DbParameter p = e.Command.Parameters["@projectAbbreviation"];
            SqlParameter param = new SqlParameter("@user", AspUtilities.RemovePrefixFromUserName(Page.User.Identity.Name));
            e.Command.Parameters.Add(param);


            

        }

        protected void sqlTestCaseDetails_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            SqlParameter param = new SqlParameter("@user", AspUtilities.RemovePrefixFromUserName(Page.User.Identity.Name));
            e.Command.Parameters.Add(param);
        }

        protected void sqlTestCaseDetails_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            Response.Redirect("~/FunctionalTesting/TestCases.aspx");
        }

        protected void btnAddEditResults_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/FunctionalTesting/AddEditResults.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&insert=Successful");
        }
        protected void btnViewResultHistory_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/FunctionalTesting/TestDetails.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&index=1&insert=Successful");
        }
        protected void btnViewUpdateHistory_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/FunctionalTesting/TestDetails.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&index=4&insert=Successful");
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

            CTMethods.AddSingleGroupIfNeeded(testCaseID, projectAbbreviation, group);

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

            CTMethods.AddSingleReleaseIfNeeded(testCaseID, projectAbbreviation, release);

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

            CTMethods.AddSingleSprintIfNeeded(testCaseID, projectAbbreviation, sprint);

            lvSprints.DataBind();
        }

        protected void lvTestCaseGroups_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveFromGroup")
            {
                String projectAbbreviation = Request.QueryString["project"];
                int testCaseID = Convert.ToInt32(Request.QueryString["testCase"]);
                string groupTestAbbreviation = (e.CommandArgument ?? "").ToString();

                CTMethods.RemoveGroupForTestCase(projectAbbreviation, groupTestAbbreviation, testCaseID);

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

                CTMethods.RemoveReleaseForTestCase(projectAbbreviation, release, testCaseID);

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

                CTMethods.RemoveSprintForTestCase(projectAbbreviation, sprint, testCaseID);

                lvSprints.DataBind();
            }
        }

        protected void imgTestCaseScreenshots_OnClick(object sender, EventArgs e)
        {
            ImageButton imgTestCaseScreenshots = (ImageButton)sender;
            Response.Redirect(imgTestCaseScreenshots.ImageUrl);
        }

        protected void btnAddScreenshot_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/FunctionalTesting/AddNewScreenshot.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&type=TestCase");
        }
       
        protected void gvDefectTickets_PageIndexChanged(object sender, EventArgs e)
        {
            FiltersChanged();
        }

        protected void PrePopulateFields()
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            
            //Populate the test case description
            int tcid = Convert.ToInt32(tbTestCaseIdValue);
            if (tcid > 0)
            {
                Dictionary<string, string> tcDictionary = new Dictionary<string,string>();
                tcDictionary = DatabaseUtilities.GetTableDictionary(tbProjectAbbreviationValue, "TestCases", "testCaseId", tcid);
                string lblResultTCDescription = tcDictionary["testCaseDescription"];
                lblResultTestCaseDescription.Text = lblResultTCDescription;
            }

            string error = CTMethods.ValidateTestCaseIDForResults(tbProjectAbbreviationValue, tbTestCaseIdValue);

            if (error == null)
            {
                Type autoTestClass;
                string description;
                string autoMetaDataTable;
                int? autoMetaDataRow;
                string autoTestClassType;
                string automated;
                string reasonForNotAutomated;

                AutomatedTestBase.GetTestCaseAutomationParameters(tbProjectAbbreviationValue, Int32.Parse(tbTestCaseIdValue), out description, out autoTestClassType, out autoTestClass, out autoMetaDataTable, out autoMetaDataRow, out automated, out reasonForNotAutomated);
                lblDescription.Text = description;
                txtAutoTestClass.Text = autoTestClassType;
                txtautoMetaDataTable.Text = autoMetaDataTable;
                txtautoMetaDataRow.Text = autoMetaDataRow.HasValue ? autoMetaDataRow.Value.ToString() : "";

                if (TestCaseIsChild(tbProjectAbbreviationValue, Int32.Parse(tbTestCaseIdValue)))
                {
                    //Change Dropdown Options
                    ddlAutomated.Items.Clear();
                    ddlAutomated.Items.Add("Child");
                    ddlAutomated.Items.Add("Yes");
                    ddlAutomated.SelectedValue = automated;
                }
                else
                {
                    //Change Dropdown Options
                    ddlAutomated.Items.Clear();
                    ddlAutomated.Items.Add("");
                    ddlAutomated.Items.Add("Yes");
                    ddlAutomated.Items.Add("No");
                    ddlAutomated.Items.Add("Future");
                    ddlAutomated.SelectedValue = automated;
                }
                txtAutomationReason.Text = reasonForNotAutomated;
            }
            else
            {
                txtAutoTestClass.Text = "";
                txtautoMetaDataTable.Text = "";
                txtautoMetaDataRow.Text = "";
            }
        }

        protected bool TestCaseIsChild(String projectAbbreviation, int testCaseId)
        {
            SqlConnection conn = new SqlConnection(Constants.QAAConnectionString);
            conn.Open();
            string sql = "select * from AutoTestCaseMap " +
                    "where projectAbbreviation = '" + projectAbbreviation + "' " +
                    "and childTestCaseId = " + testCaseId;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void btnInstallAutoTestCase_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];

            string error = CTMethods.ValidateTestCaseIDForResults(tbProjectAbbreviationValue, tbTestCaseIdValue);
            string automationCheck = ValidateAutomationFields(false);

            if (error == null && automationCheck == "")
            {
                if (!String.IsNullOrEmpty(txtAutoTestClass.Text))
                {
                    try
                    {
                        Type autoTestClass = Type.GetType(txtAutoTestClass.Text);
                    }
                    catch
                    {
                        lblStatus.Text = "Invalid AutoTestClass: \"" + txtAutoTestClass.Text + "\"";
                    }
                }

                int nonNullableautoMetaDataRow;
                int? autoMetaDataRow;
                bool autoMetaDataRowParsedSuccessfully = Int32.TryParse(txtautoMetaDataRow.Text, out nonNullableautoMetaDataRow);
                if (autoMetaDataRowParsedSuccessfully)
                {
                    autoMetaDataRow = nonNullableautoMetaDataRow;
                }
                else
                {
                    autoMetaDataRow = null;
                }

                if (ddlAutomated.SelectedValue != "Yes" && ddlAutomated.SelectedValue != "Child")
                {
                    foreach (ListViewDataItem lvRow in lvChildAutomatedTestCases.Items)
                    {
                        //Remove Children.
                        Label ChildID = (Label)lvRow.FindControl("lblChildTestCaseId");
                        if (ChildID != null)
                        {
                            //Remove Child from AutoMap
                            CTMethods.RemoveAutomatedTestMap(
                                tbProjectAbbreviationValue,
                                Int32.Parse(tbTestCaseIdValue),
                                Int32.Parse(ChildID.Text));
                            //Update Child Case to blank only if NOT already set to "yes"
                            if (TestCaseIsNotAutomated(Int32.Parse(ChildID.Text)))
                            {
                                if (!TestCaseIsChild(tbProjectAbbreviationValue, Int32.Parse(ChildID.Text)))
                                {
                                    CTMethods.UpdateTestCaseForAutomation(
                                            AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                                            tbProjectAbbreviationValue,
                                            Int32.Parse(ChildID.Text),
                                            "",
                                            "",
                                            null,
                                            "",
                                            "Test removed from automation because it was a child of ID " + tbTestCaseIdValue + " which was uninstalled on " + DateTime.Now + " by " + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name));
                                }
                                else
                                {
                                    CTMethods.UpdateTestCaseForAutomation(
                                            AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                                            tbProjectAbbreviationValue,
                                            Int32.Parse(ChildID.Text),
                                            "",
                                            "",
                                            null,
                                            "Child",
                                            "");
                                }
                            }
                        }
                    }
                }


                CTMethods.UpdateTestCaseForAutomation(
                AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                tbProjectAbbreviationValue,
                Int32.Parse(tbTestCaseIdValue),
                txtAutoTestClass.Text,
                txtautoMetaDataTable.Text,
                autoMetaDataRow,
                ddlAutomated.SelectedValue,
                txtAutomationReason.Text);

                lblStatus.Text = "Updated Successfuly";
                lblMessage.Text = "";
                tbTestCaseIdValue = "";
                lblDescription.Text = "";
                txtAutoTestClass.Text = "";
                txtautoMetaDataTable.Text = "";
                txtautoMetaDataRow.Text = "";
                txtChildTestCaseID.Text = "";
                txtAutomationReason.Text = "";
                ddlAutomated.Items.Clear();
                ddlAutomated.Items.Add("");
                ddlAutomated.Items.Add("Yes");
                ddlAutomated.Items.Add("No");
                ddlAutomated.Items.Add("Future");
                ddlAutomated.SelectedValue = "";
                refreshDatagrid();
            }
            else
            {
                lblMessage.Text = error;
                if (lblMessage.Text == "")
                {
                    lblMessage.Text = automationCheck;
                }
                else
                {
                    lblMessage.Text = lblMessage.Text + "\n" + automationCheck;
                }
            }
            PrePopulateFields();
        }

        // Add Child test cases to automated parent test
        protected void btnAddChild_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            if (!String.IsNullOrEmpty(txtChildTestCaseID.Text))
            {
                List<String> childTestCaseIDs = txtChildTestCaseID.Text.Split(new[] { ',', ' ' }).ToList();

                bool updatesMade = false;

                foreach (String childTestCaseID in childTestCaseIDs)
                {
                    if (!String.IsNullOrEmpty(childTestCaseID))
                    {
                        string error = CTMethods.ValidateTestCaseIDForResults(tbProjectAbbreviationValue, tbTestCaseIdValue)
                            ?? CTMethods.ValidateTestCaseIDForResults(tbProjectAbbreviationValue, childTestCaseID);
                        string automationCheck = ValidateAutomationFields(true);

                        if (error == null && automationCheck == "")
                        {
                            string query = "select "
                                + " childTestCaseId "
                                + " from AutoTestCaseMap a "
                                + " join TestCases t "
                                + " on t.projectAbbreviation = a.projectAbbreviation "
                                + " and t.testcaseid = a.parentTestCaseId "
                                + " where a.projectAbbreviation = '" + tbProjectAbbreviationValue + "' "
                                + " and parentTestCaseId = " + tbTestCaseIdValue;

                            List<int> mappedTestCaseChildren = DatabaseUtilities.GetIntListFromQuery(query);

                            if (mappedTestCaseChildren.Contains(Int32.Parse(childTestCaseID)))
                            {
                                lblMessage.Text = "Test case " + childTestCaseID + " already exists";
                                refreshDatagrid();
                                return;
                            }

                            CTMethods.AddAutomatedTestMap(
                                tbProjectAbbreviationValue,
                                Int32.Parse(tbTestCaseIdValue),
                                Int32.Parse(childTestCaseID),
                                AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name));

                            CTMethods.AddChildCase(tbProjectAbbreviationValue, Int32.Parse(childTestCaseID));

                            updatesMade = true;

                            txtChildTestCaseID.Text = "";
                        }
                        else
                        {
                            lblMessage.Text = error;
                            if (lblMessage.Text == "")
                            {
                                lblMessage.Text = automationCheck;
                            }
                            else
                            {
                                lblMessage.Text = lblMessage.Text + "\n" + automationCheck;
                            }
                            refreshDatagrid();
                            return;
                        }
                    }
                }

                if (updatesMade == true)
                {
                    lblMessage.Text = "Update successful";
                }

                refreshDatagrid();
            }
        }

        protected string ValidateAutomationFields(bool childTest)
        {
            switch (ddlAutomated.SelectedValue)
            {
                case "":
                    if (!childTest)
                    {
                        return "You cannot automate a test with a blank Automated Field!";
                    }
                    return "";

                case "No":
                    if (txtAutomationReason.Text == "")
                    {
                        return "If Automated is No, you must provide a Reason!";
                    }
                    else if (txtAutoTestClass.Text != "")
                    {
                        return "If Automated is No, AutoTestClass must be blank!";
                    }

                    else if (txtautoMetaDataTable.Text != "")
                    {
                        return "If Automated is No, autoMetaDataTable must be blank!";
                    }

                    else if (txtautoMetaDataRow.Text != "")
                    {
                        return "If Automated is No, autoMetaDataRow must be blank!";
                    }
                    return "";

                case "Yes":
                    if (txtAutoTestClass.Text == "")
                    {
                        return "If Automated is Yes, AutoTestClass must be filled in!";
                    }
                    else
                    {
                        if (Type.GetType(txtAutoTestClass.Text) == null)
                        {
                            return "Automated Test Class needs to be in a format of 'Full Namespace.Class', 'Assembly Name'";
                        }
                    }
                    if (txtautoMetaDataTable.Text == "")
                    {
                        return "If Automated is Yes, autoMetaDataTable must be filled in!";
                    }

                    if (txtautoMetaDataRow.Text == "")
                    {
                        return "If Automated is Yes, autoMetaDataRow must be filled in!";
                    }
                    return "";

                case "Future":

                    if (txtAutoTestClass.Text != "")
                    {
                        return "If Automated is Future, AutoTestClass must be blank!";
                    }
                    else if (txtautoMetaDataTable.Text != "")
                    {
                        return "If Automated is Future, autoMetaDataTable must be blank!";
                    }
                    else if (txtautoMetaDataRow.Text != "")
                    {
                        return "If Automated is Future, autoMetaDataRow must be blank!";
                    }
                    return "";

                case "Child":
                    if (txtAutoTestClass.Text != "")
                    {
                        return "If Automated is Child, AutoTestClass must be blank!";
                    }
                    else if (txtautoMetaDataTable.Text != "")
                    {
                        return "If Automated is Child, autoMetaDataTable must be blank!";
                    }
                    else if (txtautoMetaDataRow.Text != "")
                    {
                        return "If Automated is Child, autoMetaDataRow must be blank!";
                    }
                    return "";
            }
            return "";
        }

        protected bool TestCaseIsNotAutomated(int testCaseId)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            SqlConnection conn = new SqlConnection(Constants.QAAConnectionString);
            string sql = "select automated from TestCases " +
                        "where projectAbbreviation = '" + tbProjectAbbreviationValue + "' " +
                        "and TestCaseId = " + testCaseId;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.HasRows)
            {
                conn.Close();
                return false;
            }
            else
            {
                dr.Read();
                if (dr[0].ToString() == "Yes" || dr[0].ToString() == "")
                {
                    conn.Close();
                    return false;
                }
                else
                {
                    conn.Close();
                    return true;
                }
            }
        }

        protected void btnRemoveChild_OnClick(object sender, EventArgs e)
        {
            string commandArgument = ((ImageButton)sender).CommandArgument;
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];

            CTMethods.RemoveAutomatedTestMap(
                tbProjectAbbreviationValue,
                Int32.Parse(tbTestCaseIdValue),
                Int32.Parse(commandArgument));

            CTMethods.RemoveChildCase(tbProjectAbbreviationValue, Int32.Parse(commandArgument));

            refreshDatagrid();
        }

        protected void lvChildAutomatedTestCases_DataBound(object sender, EventArgs e)
        {
        }




    }
}