using CTInfrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Utilities;

namespace CTWebsite.FunctionalTesting
{
    public partial class TestCases : System.Web.UI.Page
    {

        protected int gvTestCasesTestButtonColumn
        {
            get
            {
                return AspUtilities.GetColumnIndexByHeaderText(gvTestCaseList, "Test");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ImageButton btnKeywordSearchSubmit = (ImageButton)AspUtilities.FindControlRecursive(this, "btnKeywordSearchSubmit");
            this.Page.Form.DefaultButton = btnKeywordSearchSubmit.UniqueID;

            SqlDataSource ods = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlGroupTests");
            if (ods.SelectParameters["username"] == null)
            {
                Parameter userParam = new Parameter();
                userParam.Name = "username";
                userParam.DefaultValue = AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods.SelectParameters.Add(userParam);
            }
            #region (Page.IsPostBack)
            if (Page.IsPostBack)
            {
                HttpContext.Current.Session["Keyword"] = txtKeywordSearchTextBox.Text;
                if (ddlProjects.SelectedValue != hdnProject.Value)
                {
                    //Reset page number to the first page.
                    int page = 0;
                    HttpContext.Current.Session["PageIndex"] = page;
                    gvTestCaseList.PageIndex = page;
                    hdnProject.Value = ddlProjects.SelectedValue;
                }
            }
            #endregion
            #region Page.IsPostBack == false
            if (Page.IsPostBack == false)
            {
                // Get the project's default environment
                ddlEnvironment.DataBind();
                
                if (HttpContext.Current.Session["CurrentEnvironment"] != null)
                {
                    ddlEnvironment.SelectedValue = HttpContext.Current.Session["CurrentEnvironment"].ToString();
                }
                else
                {
                    if (ddlProjects.SelectedValue == "" || ddlProjects.SelectedValue == null)
                    {
                        string defaultEnvironment = CTMethods.GetDefaultEnvironment(ddlProjects.SelectedValue);
                        if (defaultEnvironment != null)
                        {
                            ddlEnvironment.SelectedValue = defaultEnvironment;
                        }
                    }
                }
                ddlProjects.DataBind();

                //Initialize gridviews and dropdown boxes.
                if (ddlProjects.Items.Count > 0)
                {
                    ddlProjects.Items.Insert(0, "Select a Project");
                    ddlProjects.Items[0].Value = "";
                    ddlProjects.SelectedIndex = 0;
                }

                string tbProjectAbbreviationQueryStringValue = Request.QueryString["project"];

                if (HttpContext.Current.Session["ManualOrAutomated"] != null)
                {
                    ddlManualOrAutomated.SelectedValue = HttpContext.Current.Session["ManualOrAutomated"].ToString();
                }
                else
                {
                    ddlManualOrAutomated.SelectedValue = "All";
                }

                HtmlGenericControl BrowserSelectionHeader = (HtmlGenericControl)AspUtilities.FindControlRecursive(this, "BrowserSelectionHeader");
                BrowserSelectionHeader.Visible = (ddlManualOrAutomated.SelectedValue == "Automated");

                ListBox lstbxBrowserSelection = (ListBox)AspUtilities.FindControlRecursive(this, "lstbxBrowserSelection");
                lstbxBrowserSelection.Visible = (ddlManualOrAutomated.SelectedValue == "Automated");
                
                //If a project was given in the query string, choose it now
                if (tbProjectAbbreviationQueryStringValue != null)
                {
                    ddlProjects.SelectedValue = tbProjectAbbreviationQueryStringValue;
                }
                else
                {
                    if (HttpContext.Current.Session["CurrentProject"] != null)
                    {
                        ddlProjects.SelectedValue = HttpContext.Current.Session["CurrentProject"].ToString();
                    }
                    else
                    {
                        // Get logged in user's default project
                        string defaultProject = CTMethods.GetDefaultProject(HttpContext.Current.User.Identity);

                        if (defaultProject != null)
                        {
                            ddlProjects.SelectedValue = defaultProject;
                            hdnProject.Value = defaultProject;
                        }
                    }
                }

                if (HttpContext.Current.Session["Keyword"] != null)
                {
                    txtKeywordSearchTextBox.Text = HttpContext.Current.Session["Keyword"].ToString() ?? "";
                }

                if (HttpContext.Current.Session["PageIndex"] != null)
                {
                    int page;
                    if (Int32.TryParse(HttpContext.Current.Session["PageIndex"].ToString(), out page))
                    {
                        gvTestCaseList.PageIndex = page;
                    }
                }

                ddlProjectsValueChange();
                ddlAddToaBunchOfThings_SetValidation();


                if (HttpContext.Current.Session["CurrentMessage"] != null)
                {
                    Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                    label.Text = HttpContext.Current.Session["CurrentMessage"].ToString();
                    label.Visible = true;

                    HttpContext.Current.Session["CurrentMessage"] = null;
                }

            }
#endregion
            SetCTTestCaseMessage(ddlProjects.SelectedValue);
            SetCheckBoxEnabledStatus();
            
            #region Set Browser Column Visibility
            
            System.Web.UI.WebControls.BoundField DataColumn;

            //Get the list of browser columns in the gridview
            int[] gridviewBrowserColumns = CTMethods.gvBrowserStatusColumns(gvTestCaseList);
            
            //Get the list of which ones of those should be visible
            int[] gridviewVisibleBrowserColumns = CTMethods.getVisibleBrowserColumns(gvTestCaseList, ddlProjects.SelectedValue);

            //Loop through the browser columns
            foreach (int column in gridviewBrowserColumns)
            {
                DataColumn = gvTestCaseList.Columns[column] as System.Web.UI.WebControls.BoundField;

                //If the current column exists in the visible browser list
                if (gridviewVisibleBrowserColumns.Contains(column))
                {
                    //Make it visible
                    DataColumn.Visible = true;
                }
                else
                {
                    //Otherwise hide it
                    DataColumn.Visible = false;
                }
            }
             #endregion
        }

        private void SetCTTestCaseMessage(string projectAbbreviation)
        {
            lblCrystalTestCases.Text = "";
            if (projectAbbreviation.Equals("CT"))
            {
                string query = "SELECT * FROM TestCases WHERE projectAbbreviation = 'CT'";
                DataTable dt = DatabaseUtilities.GetDataTable(query);
                if (dt.Rows.Count > 0)
                {
                    lblCrystalTestCases.Text = "";
                }
                else
                {
                    lblCrystalTestCases.Text = "The test cases for Crystal Test are at C:\\Development\\Test Cases and Plans\\Test Cases\\Crystal Test. You can upload them on the Admin-TestCases page.";
                }
            }
        }
        protected void ddlAddToaBunchOfThings_SetValidation()
        {
            // Note: You have to use the ClientID or javascript won't be able to look the element up by ID.
            DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
            if (HttpContext.Current.Session["TypeOfThingBeingAdded"] != null)
            {
                switch (HttpContext.Current.Session["TypeOfThingBeingAdded"].ToString())
                {
                    case "Group":
                        dropdown.Attributes.Add("Onchange", "if(!confirm('Are you sure you want to add the selected test cases to this group?')){document.getElementById('"
                            + ddlAddToaBunchOfThings.ClientID + "').selectedIndex = 0;return;}");
                        break;
                    case "Release":
                        dropdown.Attributes.Add("Onchange", "if(!confirm('Are you sure you want to add the selected test cases to this release?')){document.getElementById('"
                            + ddlAddToaBunchOfThings.ClientID + "').selectedIndex = 0;return;}");
                        break;
                    case "Sprint":
                        dropdown.Attributes.Add("Onchange", "if(!confirm('Are you sure you want to add the selected test cases to this sprint?')){document.getElementById('"
                            + ddlAddToaBunchOfThings.ClientID + "').selectedIndex = 0;return;}");
                        break;
                    case "User":
                        dropdown.Attributes.Add("Onchange", "if(!confirm('Are you sure you want to assign the selected test cases to this user?')){document.getElementById('"
                            + ddlAddToaBunchOfThings.ClientID + "').selectedIndex = 0;return;}");
                        break;
                }
            }
        }

        protected void btnAddNewTestCase_Click(object sender, EventArgs e)
        {
            string tbProjectAbbreviationValue = ddlProjects.SelectedValue;
            Response.Redirect("~/FunctionalTesting/AddNewTestCase.aspx?project=" + tbProjectAbbreviationValue);
        }

        protected void btnUploadTestResults_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminTestCases.aspx");
        }

        protected void btnUploadTestCases_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminTestCases.aspx");
        }

        protected void btnCreateExcelTestSheet_Click(object sender, EventArgs e)
        {

        }

        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            ddlProjectsValueChange();
        }

        protected void ddlProjectsValueChange()
        {
            gvTestCaseList.DataBind();


            ddlGroupTest.DataBind();

            if (ddlGroupTest.Items.Count == 0
                || ddlGroupTest.Items[0].Value != "")
            {
                ddlGroupTest.Items.Insert(0, "Select a Group Test");
                ddlGroupTest.Items[0].Value = "";
            }


            ddlReleases.DataBind();

            if (ddlReleases.Items.Count == 0
                || ddlReleases.Items[0].Value != "")
            {
                ddlReleases.Items.Insert(0, "Select a Release");
                ddlReleases.Items[0].Value = "";
            }


            ddlSprints.DataBind();

            if (ddlSprints.Items.Count == 0
                || ddlSprints.Items[0].Value != "")
            {
                ddlSprints.Items.Insert(0, "Select a Sprint");
                ddlSprints.Items[0].Value = "";
            }

            lstbxBrowserSelection.DataBind();
            string tblGroupQueryStringValue = Request.QueryString["group"];

            // If this is the initial page load, and a group was given in the query string, choose it now.
            if (Page.IsPostBack == false && tblGroupQueryStringValue != null)
            {
                ddlGroupTest.SelectedValue = tblGroupQueryStringValue;
            }

            try
            {
                ddlGroupTest.SelectedValue = HttpContext.Current.Session["CurrentGroup"].ToString();
            }
            catch(Exception)
            {
                HttpContext.Current.Session["CurrentGroup"] = null;
            }

            string tblReleaseQueryStringValue = Request.QueryString["release"];

            // If this is the initial page load, and a group was given in the query string, choose it now.
            if (Page.IsPostBack == false && tblReleaseQueryStringValue != null)
            {
                ddlReleases.SelectedValue = tblReleaseQueryStringValue;
            }

            try
            {
                ddlReleases.SelectedValue = HttpContext.Current.Session["CurrentRelease"].ToString();
            }
            catch (Exception)
            {
                HttpContext.Current.Session["CurrentRelease"] = null;
            }

            string tblSprintQueryStringValue = Request.QueryString["sprint"];

            // If this is the initial page load, and a group was given in the query string, choose it now.
            if (Page.IsPostBack == false && tblSprintQueryStringValue != null)
            {
                ddlSprints.SelectedValue = tblSprintQueryStringValue;
            }

            try
            {
                ddlSprints.SelectedValue = HttpContext.Current.Session["CurrentSprint"].ToString();
            }
            catch (Exception)
            {
                HttpContext.Current.Session["CurrentSprint"] = null;
            }

            //Environment
            ddlEnvironment.DataBind();

            //If an environment is stored in a session variable, select it
            if (HttpContext.Current.Session["CurrentEnvironment"] != null)
            {
                ddlEnvironment.SelectedValue = HttpContext.Current.Session["CurrentEnvironment"].ToString();
            }
            else
            {
                //If there is a value in the projects dropdown, use it to find the default environment
                if (ddlProjects.SelectedValue != "" && ddlProjects.SelectedValue != null)
                {
                    // Select the project's default environment
                    string defaultEnvironment = CTMethods.GetDefaultEnvironment(ddlProjects.SelectedValue);
                    if (defaultEnvironment != null)
                    {
                        ddlEnvironment.SelectedValue = defaultEnvironment;
                    }
                }
            }

            //If there is still nothing in environments. Put something there.
            if (ddlEnvironment.Items.Count == 0
                || ddlEnvironment.Items[0].Value != "")
            {
                ddlEnvironment.Items.Insert(0, "No Environment Available");
                ddlEnvironment.Items[0].Value = "";
            }

            ddlEnvironment.DataBind();

            FiltersChanged();

            DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
            dropdown.Visible = false;

            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
            label.Visible = false;
        }

        protected void gvTestCaseList_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "RunTest")
            {
                if (ddlManualOrAutomated.SelectedValue == "Automated")
                {
                    string commandArgument = (e.CommandArgument ?? "").ToString();

                    DateTime startTime = SeleniumTestBase.GetEstimatedTimeCurrentlyQueuedAutomatedTestsWillFinish();
                    RunAutomatedTest(commandArgument);
                    DateTime endTime = SeleniumTestBase.GetEstimatedTimeCurrentlyQueuedAutomatedTestsWillFinish();

                    HttpContext.Current.Session["CurrentMessage"] = "Your automated test has been added to the testing queue. You will be emailed the results when your test is completed."; // The estimated time your tests will start is " + String.Format("{0:G}", startTime) + ", and the estimated time your tests will end is " + String.Format("{0:G}", endTime);
                    
                    Response.Redirect(Request.RawUrl);
                    return;
                }
                else // if (ddlManualOrAutomated.SelectedValue == "Manual" || ddlManualOrAutomated.SelectedValue == "All")
                {
                    string commandArgument = (e.CommandArgument ?? "").ToString();
                    string[] commandPieces = commandArgument.Split(new[] { '|' });

                    string projectAbbreviation = commandPieces[0];
                    int testCaseID = Convert.ToInt32(commandPieces[1]);
                    string environment = ddlEnvironment.SelectedValue;

                    //string url = (string)e.CommandArgument;
                    string url = "~/FunctionalTesting/AddEditResults.aspx?project=" + projectAbbreviation + "&testCase=" + testCaseID + "&environment=" + environment;
                    Response.Redirect(url);
                    return;
                }
            }
            // Force a refresh of the screen (to see the new status in the GridView)
            FiltersChanged();
        }

        /// <returns>True if a test was kicked off, or false if no test was kicked off (because this was a child of an automated test, not an actual automated test)</returns>
        private bool RunAutomatedTest(string commandArgument)
        {
            string[] commandPieces = commandArgument.Split(new[] { '|' });

            string projectAbbreviation = commandPieces[0];
            int testCaseID = Convert.ToInt32(commandPieces[1]);

            if (commandPieces[2] == "")
            {
                // Automated child only, can't be kicked off by itself
                return false;
            }

            string autoTestClass = commandPieces[2];

            string environment = commandPieces[5];

            List<string> selectedBrowsers = new List<string>();
            int[] selectedItemsIndexes = lstbxBrowserSelection.GetSelectedIndices();
            foreach (int selectedItem in selectedItemsIndexes)
            {
                selectedBrowsers.Add(lstbxBrowserSelection.Items[selectedItem].Value);
            }

            System.Security.Principal.IIdentity identity = HttpContext.Current.User.Identity;

            SeleniumTestBase.QueueTestCase(environment, projectAbbreviation, testCaseID, AspUtilities.RemovePrefixFromUserName(identity.Name), selectedBrowsers);

            return true;
        }

        protected void gvTestCaseList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            /*  
                * CommandPieces[0] = projectAbbreviation
                * CommandPieces[1] = testCaseId
                * CommandPieces[2] = autoTestClass
                * CommandPieces[3] = autoMetaDataTable
                * CommandPieces[4] = autoMetaDataRow
                * CommandPieces[5] = environment
                * CommandPieces[6] = childTestCaseId
                * CommandPieces[7] = active
                * CommandPieces[8] = testCaseOutdated
                * CommandPieces[9] = testScriptOutdated
            */
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // If the view is automated, hide all test cases that do not have a value in the autoTestClass field
                if (ddlManualOrAutomated.SelectedValue == "Automated")
                {
                    TableCell tableCell = e.Row.Cells[gvTestCasesTestButtonColumn];
                    Button button = (Button)tableCell.Controls[1]; // Test Button (The only control within the tableCell)
                    string commandArgument = (button.CommandArgument ?? "").ToString();
                    string[] commandPieces = commandArgument.Split(new[] { '|' });
                    
                    //Hide test button if necessary
                    if (commandPieces[2] == "")
                    {
                        button.Visible = false;
                    }
                    if (Convert.ToBoolean(commandPieces[9]) == true)
                    {
                        button.Visible = false;
                        e.Row.CssClass = "NeedsAttention";
                    }

                    if (Convert.ToBoolean(commandPieces[7]) == false)
                    {
                        button.Visible = false;
                        e.Row.CssClass = "Inactive";
                    }

                }

                // If the view is manual or all, hide all test cases that do not have a value in the autoTestClass field
                if (ddlManualOrAutomated.SelectedValue != "Automated")
                {
                    TableCell tableCell = e.Row.Cells[gvTestCasesTestButtonColumn];
                    Button button = (Button)tableCell.Controls[1]; // Test Button (The only control within the tableCell)
                    string commandArgument = (button.CommandArgument ?? "").ToString();
                    string[] commandPieces = commandArgument.Split(new[] { '|' });
                    if (Convert.ToBoolean(commandPieces[8]) == true)
                    {
                        button.Visible = false;
                        e.Row.CssClass = "NeedsAttention";
                    }

                    if (Convert.ToBoolean(commandPieces[7]) == false)
                    {
                        button.Visible = false;
                        e.Row.CssClass = "Inactive";
                    }

                }

                // Display the icon depending on test status
                // Icons are defined in site.css
                int[] gvTestCaseListStatusColumns = CTMethods.gvBrowserStatusColumns(gvTestCaseList);
                
                foreach (int gvTestCaseListStatusColumn in gvTestCaseListStatusColumns)
                {
                    if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "Pass")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusPass";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "Pass";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "Fail")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusFail";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "Fail";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "Test Case Needs Updated")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusTestCaseNeedsUpdated";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "Test Case Needs Updated";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "Automated Test Needs Updated")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusAutomatedTestNeedsUpdated";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "Automated Test Needs Updated";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "Not Started")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusNotStarted";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "Not Started";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "Not Implemented")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusNotImplemented";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "Not Implemented";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "Not Applicable")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusNotApplicable";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "Not Applicable";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "In Progress")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusInProgress";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "In Progress";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "In Queue")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusInQueue";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "In Queue";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTestCaseListStatusColumn].Text == "Retest")
                    {
                        e.Row.Cells[gvTestCaseListStatusColumn].CssClass = "statusRetest";
                        e.Row.Cells[gvTestCaseListStatusColumn].ToolTip = "Retest";
                        e.Row.Cells[gvTestCaseListStatusColumn].Text = "";
                    }
                }
            }
        }

        // Stores the selected Release in the session variable 'CurrentRelease'
        protected void ddlReleases_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentRelease"] = ddlReleases.SelectedValue;
            FiltersChanged();
        }

        // Stores the selected Sprint in the session variable 'CurrentSprint'
        protected void ddlSprints_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentSprint"] = ddlSprints.SelectedValue;
            FiltersChanged();
        }

        // Stores the selected Environment in the session variable 'CurrentEnvironment'
        protected void ddlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentEnvironment"] = ddlEnvironment.SelectedValue;
            FiltersChanged();
        }

        // Stores the selected Group in the session variable 'CurrentGroup'
        protected void ddlGroupTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentGroup"] = ddlGroupTest.SelectedValue;
            FiltersChanged();
        }

        // Stores the selected View in the session variable 'ManualOrAutomated' and hides the Browser Selection box if View is not 'Automated' 
        protected void ddlManualOrAutomated_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["ManualOrAutomated"] = ((DropDownList)sender).SelectedValue;
            FiltersChanged();

            HtmlGenericControl BrowserSelectionHeader = (HtmlGenericControl)AspUtilities.FindControlRecursive(this, "BrowserSelectionHeader");
            BrowserSelectionHeader.Visible = (ddlManualOrAutomated.SelectedValue == "Automated");

            ListBox lstbxBrowserSelection = (ListBox)AspUtilities.FindControlRecursive(this, "lstbxBrowserSelection");
            lstbxBrowserSelection.Visible = (ddlManualOrAutomated.SelectedValue == "Automated");
        }



        protected void FiltersChanged()
        {
            #region Test Case List Caption Assignments
            gvTestCaseList.Caption = "Project: " + ddlProjects.SelectedItem.Text;
            
            if (ddlGroupTest.SelectedIndex > 0)
            {
                gvTestCaseList.Caption += "<br/>" + "Group: " + ddlGroupTest.SelectedItem.Text;
            }

            if (ddlReleases.SelectedValue != "")
            {
                gvTestCaseList.Caption += "<br/>" + "Release: " + ddlReleases.SelectedValue;
            }

            if (ddlSprints.SelectedValue != "")
            {
                gvTestCaseList.Caption += "<br/>" + "Sprint: " + ddlSprints.SelectedValue;
            }

            TextBox txtKeywordSearchTextBox = (TextBox)AspUtilities.FindControlRecursive(this, "txtKeywordSearchTextBox");
            string searchText = txtKeywordSearchTextBox.Text;

            if (!String.IsNullOrEmpty(searchText))
            {
                gvTestCaseList.Caption += "<br/>" + "Keywords: " + searchText;
            }

            gvTestCaseList.Caption += "<br/>" + "Environment: " + ddlEnvironment.SelectedValue;
            #endregion

            #region Test Case and Result Query
            SqlDataSource sqlTestCasesByProject = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlTestCasesByProject");

            sqlTestCasesByProject.SelectCommand =
                "select distinct "
              + "    LatestTestRunIDPerBrowser.environment, "
              + "    LatestTestRunIDPerBrowser.projectAbbreviation, "
              + "    LatestTestRunIDPerBrowser.testCaseId, "
              + "    LatestTestRunIDPerBrowser.testCaseDescription, "
              + "    LatestTestRunIDPerBrowser.active, "
              + "    LatestTestRunIDPerBrowser.testCaseOutdated, "
              + "    LatestTestRunIDPerBrowser.testScriptOutdated, "
              + "    autoTestClass, "
              + "    autoMetaDataTable, "
              + "    autoMetaDataRow, "
              + "    LatestTestRunIDPerBrowser.testDate, "
              + "    Coalesce(Browser1TestResults.status, 'Not Started') as Browser1Status, "
              + "    Coalesce(Browser2TestResults.status, 'Not Started') as Browser2Status, "
              + "    Coalesce(Browser3TestResults.status, 'Not Started') as Browser3Status, "
              + "    Coalesce(Browser4TestResults.status, 'Not Started') as Browser4Status, "
              + "    Coalesce(Browser5TestResults.status, 'Not Started') as Browser5Status, "
              + "    Coalesce(Browser6TestResults.status, 'Not Started') as Browser6Status, "
              + "    Coalesce(Browser7TestResults.status, 'Not Started') as Browser7Status, "
              + "    Coalesce(Browser8TestResults.status, 'Not Started') as Browser8Status, "
              + "    Childtestcases.childTestCaseId "
              + "from LatestTestRunIDPerBrowser "
              + "left join TestResults Browser1TestResults "
              + "    on LatestTestRunIDPerBrowser.Browser1TestRunID = Browser1TestResults.testRunId "
              + "left join TestResults Browser2TestResults "
              + "    on LatestTestRunIDPerBrowser.Browser2TestRunID = Browser2TestResults.testRunId "
              + "left join TestResults Browser3TestResults "
              + "    on LatestTestRunIDPerBrowser.Browser3TestRunID = Browser3TestResults.testRunId "
              + "left join TestResults Browser4TestResults "
              + "    on LatestTestRunIDPerBrowser.Browser4TestRunID = Browser4TestResults.testRunId "
              + "left join TestResults Browser5TestResults "
              + "    on LatestTestRunIDPerBrowser.Browser5TestRunID = Browser5TestResults.testRunId "
              + "left join TestResults Browser6TestResults "
              + "    on LatestTestRunIDPerBrowser.Browser6TestRunID = Browser6TestResults.testRunId "
              + "left join TestResults Browser7TestResults "
              + "    on LatestTestRunIDPerBrowser.Browser7TestRunID = Browser7TestResults.testRunId "
              + "left join TestResults Browser8TestResults "
              + "    on LatestTestRunIDPerBrowser.Browser8TestRunID = Browser8TestResults.testRunId "
              + "left join (select distinct childTestCaseId from dbo.AutoTestCaseMap) Childtestcases "
              + "    on LatestTestRunIDPerBrowser.testCaseId = Childtestcases.childTestCaseId "
              + "left JOIN dbo.GroupTestCases "
              + "    ON dbo.GroupTestCases.projectAbbreviation = LatestTestRunIDPerBrowser.projectAbbreviation "
              + "    AND dbo.GroupTestCases.testCaseId = LatestTestRunIDPerBrowser.testCaseId "
              + "left JOIN dbo.ReleaseTestCases "
              + "    ON dbo.ReleaseTestCases.projectAbbreviation = LatestTestRunIDPerBrowser.projectAbbreviation "
              + "    AND dbo.ReleaseTestCases.testCaseId = LatestTestRunIDPerBrowser.testCaseId "
              + "left JOIN dbo.SprintTestCases "
              + "    ON dbo.SprintTestCases.projectAbbreviation = LatestTestRunIDPerBrowser.projectAbbreviation "
              + "    AND dbo.SprintTestCases.testCaseId = LatestTestRunIDPerBrowser.testCaseId ";

            sqlTestCasesByProject.SelectCommand += GetFilterWithPrefixes();


            sqlTestCasesByProject.SelectCommand +=
                " group by "
              + "    LatestTestRunIDPerBrowser.environment, "
              + "    LatestTestRunIDPerBrowser.projectAbbreviation, "
              + "    LatestTestRunIDPerBrowser.testCaseId, "
              + "    LatestTestRunIDPerBrowser.testCaseDescription, "
              + "    LatestTestRunIDPerBrowser.active, "
              + "    LatestTestRunIDPerBrowser.testCaseOutdated, "
              + "    LatestTestRunIDPerBrowser.testScriptOutdated, "
              + "    autoTestClass, "
              + "    autoMetaDataTable, "
              + "    autoMetaDataRow, "
              + "    LatestTestRunIDPerBrowser.testDate, "
              + "    Browser1TestResults.status, "
              + "    Browser2TestResults.status, "
              + "    Browser3TestResults.status, "
              + "    Browser4TestResults.status, "
              + "    Browser5TestResults.status, "
              + "    Browser6TestResults.status, "
              + "    Browser7TestResults.status, "
              + "    Browser8TestResults.status, "
              + "    Childtestcases.childTestCaseId ";

              //Console.WriteLine();
            #endregion

            #region Set Automated-Only Control Visibility
              if (ddlManualOrAutomated.SelectedValue == "Automated")
            {
                AspUtilities.FindControlRecursive(this, "btnAddToAutomatedQueue").Visible = true;
                AspUtilities.FindControlRecursive(this, "btnRemoveFromAutomatedQueue").Visible = true;

                foreach (DataControlField column in gvTestCaseList.Columns)
                {
                    if (column != null && (column is BoundField) && String.Compare(((BoundField)column).DataField, "AverageSeconds", true) == 0)
                    {
                        column.Visible = true;
                    }
                }
            }
            else
            {
                AspUtilities.FindControlRecursive(this, "btnAddToAutomatedQueue").Visible = false;
                AspUtilities.FindControlRecursive(this, "btnRemoveFromAutomatedQueue").Visible = false;
                foreach (DataControlField column in gvTestCaseList.Columns)
                {
                    if (column != null && (column is BoundField) && String.Compare(((BoundField)column).DataField, "AverageSeconds", true) == 0)
                    {
                        column.Visible = false;
                    }
                }
            }
            #endregion

            #region PopulateChart
            //AjaxControlToolkit.PieChartValue temp = new AjaxControlToolkit.PieChartValue();
            //temp.Category = "CatA";
            //temp.Data = 1;
            decimal countPass = 0;
            decimal countFail = 0;
            decimal countOther = 0;

            System.Web.UI.WebControls.BoundField DataColumn;
            DataTable chartInfo = DatabaseUtilities.GetDataTable(sqlTestCasesByProject.SelectCommand);
            foreach (DataRow row in chartInfo.Rows)
            {
                //Get the list of which browser columns are visible
                //We should only calculate results for these columns (despite hidden column results)
                int[] gridviewVisibleBrowserColumns = CTMethods.getVisibleBrowserColumns(gvTestCaseList, ddlProjects.SelectedValue);

                // Loop through all visible columns
                // Populate the chart based on visible columns
                foreach (int gridviewVisibleBrowserColumn in gridviewVisibleBrowserColumns)
                { 
                    DataColumn = gvTestCaseList.Columns[gridviewVisibleBrowserColumn] as System.Web.UI.WebControls.BoundField;
                    String browserColumn = DataColumn.DataField;
                    String browserStatus = row[browserColumn].ToString();
                    CountBrowserStatuses(browserStatus, ref countPass, ref countFail, ref countOther);
                }

            }

            //Apply Data to Bar Chart
            decimal[] tempPass = new decimal[1];
            tempPass[0] = countPass;

            decimal[] tempFail = new decimal[1];
            tempFail[0] = countFail;

            decimal[] tempOther = new decimal[1];
            tempOther[0] = countOther;

            
            BarChart1.Series.Add
                (
                new AjaxControlToolkit.BarChartSeries
                {
                    Name = "Pass",
                    BarColor = "#00FF00",
                    Data = tempPass
                }
                    );


            BarChart1.Series.Add
                (
                new AjaxControlToolkit.BarChartSeries
                {
                    Name = "Fail",
                    BarColor = "#FF0000",
                    Data = tempFail
                }
                    );

            BarChart1.Series.Add
                (
                new AjaxControlToolkit.BarChartSeries
                {
                    Name = "Other",
                    BarColor = "#D1D1D1",
                    Data = tempOther
                }
                    );

            decimal totaltested = countPass + countFail;
            decimal totalstatuses = countPass + countFail + countOther;

            if (totaltested != 0 && totalstatuses != 0)
            {
                string passedPercentage = string.Format("{0:0%}", (decimal)countPass / totalstatuses);
                string passedTestedPercentage = string.Format("{0:0%}", (decimal)countPass / totaltested);
                string testedPercentage = string.Format("{0:0%}", (decimal)totaltested / totalstatuses);
                PercentOfTotalTestCasesPassed.Text = passedPercentage;
                PercentOfTotalTested.Text = passedTestedPercentage;
                PercentOfTotalTestCasesTested.Text = testedPercentage;
            }
            else {
                PercentOfTotalTestCasesPassed.Text = string.Format("{0:0%}",0);
                PercentOfTotalTested.Text = string.Format("{0:0%}", 0);
                PercentOfTotalTestCasesTested.Text = string.Format("{0:0%}", 0);
            }


            #endregion

            if (ddlProjects.SelectedValue == "CT")
            {
                lblCrystalTestCases.Text = "If the test cases for Crystal Test have not already been uploaded, you can find them at C:\\Development\\Test Cases and Plans\\Test Cases\\Crystal Test. You can upload them on the Admin Test Cases page";
            }
            else 
            {
                lblCrystalTestCases.Text = "";
            }

            SetCheckBoxEnabledStatus();
        }

        public void CountBrowserStatuses(String browserStatus, ref decimal countPass, ref decimal countFail, ref decimal countOther)
        {
            if (browserStatus.Equals("Pass"))
            {
                countPass++;
            }
            else if (browserStatus.Equals("Fail"))
            {
                countFail++;
            }
            else
            {
                countOther++;
            }
        }

        public string GetCommandArgument(int testCaseId)
        {
            string description;
            Type autoTestClass;
            string autoMetaDataTable;
            int? autoMetaDataRow;
            string autoTestClassType;
            string automated;
            string reasonForNotAutomated;
            AutomatedTestBase.GetTestCaseAutomationParameters(ddlProjects.SelectedValue, testCaseId, out description, out autoTestClassType, out autoTestClass, out autoMetaDataTable, out autoMetaDataRow, out automated, out reasonForNotAutomated);
            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}", ddlProjects.SelectedValue, testCaseId, autoTestClassType, autoMetaDataTable, autoMetaDataRow, ddlEnvironment.SelectedValue);
        }

        public List<int> GetFilteredTestCaseIDs()
        {
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
                if (chk.Checked)
                {
                    string filter = GetFilter();

                    string sql = "Select distinct TestCaseID from TestCaseFilterableView " 
                        + "left join (select distinct childTestCaseId from dbo.AutoTestCaseMap) Childtestcases "
                        + "    on testCaseId = childTestCaseId "
                        + filter;
                    return DatabaseUtilities.GetIntListFromQuery(sql);
                }
                else
                {
                    List<int> returnList = new List<int>();

                    for (int i = 0;i < gvTestCaseList.Rows.Count;i++)
                    {
                        GridViewRow gridViewRow = gvTestCaseList.Rows[i];

                        string testCaseID = gridViewRow.Cells[2].Text;

                        if (((CheckBox)gridViewRow.FindControl("chkSelected")).Checked)
                        {
                            returnList.Add(Convert.ToInt32(testCaseID));
                        }
                    }

                    return returnList;
                }
            }
            catch
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Please select one or more test cases first";
                label.Visible = true;
                return null;
            }

        }


        // Build SQL query WHERE clause based on drop down filters
        public string GetFilter()
        {
            string filter = " WHERE ([Environment] = '" + DatabaseUtilities.MakeSQLSafe(ddlEnvironment.SelectedValue) + "')";

            if (ddlProjects.SelectedValue != "")
            {
                filter += " AND ([projectAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "')";
            }

            if (ddlGroupTest.SelectedValue != "")
            {
                filter += " AND ([groupTestAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlGroupTest.SelectedValue) + "')";
            }

            if (ddlReleases.SelectedValue != "")
            {
                filter += " AND ([release] = '" + DatabaseUtilities.MakeSQLSafe(ddlReleases.SelectedValue) + "')";
            }

            if (ddlSprints.SelectedValue != "")
            {
                filter += " AND ([sprint] = '" + DatabaseUtilities.MakeSQLSafe(ddlSprints.SelectedValue) + "')";
            }

            if (ddlManualOrAutomated.SelectedValue == "Automated")
            {
                filter += " AND ((([autoTestClass] is not null) AND ([autoTestClass] <> '')) OR (childTestCaseId is not null))";
            }

            if (ddlManualOrAutomated.SelectedValue == "Manual")
            {
                filter += " AND (([autoTestClass] is null OR [autoTestClass] = '') AND (childTestCaseId is null))";
            }


            TextBox txtKeywordSearchTextBox = (TextBox)AspUtilities.FindControlRecursive(this, "txtKeywordSearchTextBox");
            string searchText = txtKeywordSearchTextBox.Text;

            if (!String.IsNullOrEmpty(searchText))
            {
                List<string> splitSearchValues = searchText.Split(new[] { ' ' }).ToList();

                foreach (string splitSearchValue in splitSearchValues)
                {
                    filter += " AND (testCaseDescription like '%" + DatabaseUtilities.MakeSQLSafe(splitSearchValue) + "%')";
                    filter += " OR (LatestTestRunIDPerBrowser.testCaseId = " + DatabaseUtilities.MakeSQLSafe(splitSearchValue) + "))";
                }
            }

            return filter;
        }

        // Build SQL query WHERE clause based on drop down filters and results of the latest Test Run ID Per Browser
        public string GetFilterWithPrefixes()
        {
            string filter = " WHERE (LatestTestRunIDPerBrowser.[Environment] = '" + DatabaseUtilities.MakeSQLSafe(ddlEnvironment.SelectedValue) + "')" +
                    " AND (LatestTestRunIDPerBrowser.[testCategory] = 'Functional')";

            if (ddlProjects.SelectedValue != "")
            {
                filter += " AND (LatestTestRunIDPerBrowser.[projectAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "')";
            }

            if (ddlGroupTest.SelectedValue != "")
            {
                filter += " AND (GroupTestCases.[groupTestAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlGroupTest.SelectedValue) + "')";
            }

            if (ddlReleases.SelectedValue != "")
            {
                filter += " AND (ReleaseTestCases.[release] = '" + DatabaseUtilities.MakeSQLSafe(ddlReleases.SelectedValue) + "')";
            }

            if (ddlSprints.SelectedValue != "")
            {
                filter += " AND (SprintTestCases.[sprint] = '" + DatabaseUtilities.MakeSQLSafe(ddlSprints.SelectedValue) + "')";
            }

            if (ddlManualOrAutomated.SelectedValue == "Automated")
            {
                filter += " AND (((LatestTestRunIDPerBrowser.[autoTestClass] is not null) AND (LatestTestRunIDPerBrowser.[autoTestClass] <> '')) OR (Childtestcases.childTestCaseId is not null))";
            }

            if (ddlManualOrAutomated.SelectedValue == "Manual")
            {
                filter += " AND ((LatestTestRunIDPerBrowser.[autoTestClass] is null OR LatestTestRunIDPerBrowser.[autoTestClass] = '') AND (Childtestcases.childTestCaseId is null))";
            }

            TextBox txtKeywordSearchTextBox = (TextBox)AspUtilities.FindControlRecursive(this, "txtKeywordSearchTextBox");
            string searchText = txtKeywordSearchTextBox.Text;

            if (!String.IsNullOrEmpty(searchText))
            {
                List<string> splitSearchValues = searchText.Split(new[] { ' ' }).ToList();

                foreach (string splitSearchValue in splitSearchValues)
                {
                    // Check to see if the search string is an integer
                    int i = 0;
                    bool isInteger = int.TryParse(splitSearchValue, out i); //i now = 108
                    if (isInteger)
                    {
                        filter += " AND ((LatestTestRunIDPerBrowser.testCaseDescription like '%" + DatabaseUtilities.MakeSQLSafe(splitSearchValue) + "%')";
                        filter += " OR (LatestTestRunIDPerBrowser.testCaseId = " + i.ToString() + "))";
                    }
                    else
                    {
                        filter += " AND (LatestTestRunIDPerBrowser.testCaseDescription like '%" + DatabaseUtilities.MakeSQLSafe(splitSearchValue) + "%')";
                    }
                }
            }

            return filter;
        }

        // Stores the selected page index in the session variable 'PageIndex'
        protected void gvTestCaseList_PageIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["PageIndex"] = gvTestCaseList.PageIndex;
            
            FiltersChanged();
        }


        #region Icon Menu Actions
        // Assign selected test cases to a user
        protected void AssignTestCase(object sender, ImageClickEventArgs e)
        {
            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }
            label.Visible = false;

            DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
            dropdown.Visible = true;

            SqlDataSource data = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlAddToaBunchOfThings");
            data.SelectCommand = "select UserID, userFirstName + ' ' + userLastName as UserFullName from userprofiles";

            dropdown.DataTextField = "UserFullName";
            dropdown.DataValueField = "UserID";

            data.DataBind();
            dropdown.DataBind();

            if (dropdown.Items.Count == 0
                || dropdown.Items[0].Value != "")
            {
                dropdown.Items.Insert(0, "Select a User");
                dropdown.Items[0].Value = "";
                dropdown.SelectedIndex = 0;
            }

            HttpContext.Current.Session["TypeOfThingBeingAdded"] = "User";
            ddlAddToaBunchOfThings_SetValidation();
        }
        
        // Export selected test cases to an Excel sheet
        protected void ExportToExcel(object sender, ImageClickEventArgs e)
        {
            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }

            List<int> testCaseList = GetFilteredTestCaseIDs();


            string testCases = String.Join(", ", testCaseList);         


            if (testCaseList.Count == 0)
            {
                label.Text = "Please select one or more test cases first";
                label.Visible = true;
            }
            else
            {
                label.Text = "Completed";
                label.Visible = true;

                FiltersChanged();

                string query = "select distinct projectAbbreviation, testCaseId, '' as groupTestAbbreviation, '' as release, '' as sprint, rawTestCaseDescription as testCaseDescription, active, testCaseOutdated, testScriptOutdated, rawTestCaseSteps as testCaseSteps, rawExpectedResults as expectedResults, '' as screenshots, '' as screenshotDescriptions, isnull(rawTestCaseNotes,'') as testCaseNotes, 'Y' as [isThisAnUpdate?], isnull(testCategory,'') as testCategory, isnull(autoTestClass,'') as autoTestClass, autoMetaDataTable, isnull( CONVERT(varchar(3), autoMetaDataRow) ,'') as autoMetaDataRow, isnull(automated,'') as automated, isnull(reasonForNotAutomated,'') as reasonForNotAutomated  from TestCaseFilterableView "
                    + "left join (select distinct childTestCaseId from dbo.AutoTestCaseMap) Childtestcases "
                    + "    on testCaseId = childTestCaseId "
                    + GetFilter()
                    + " and TestCaseID in (" + testCases + ")";

                

                DataTable results = DatabaseUtilities.GetDataTable(query);

                results.Columns["groupTestAbbreviation"].ReadOnly = false;
                results.Columns["groupTestAbbreviation"].MaxLength = 500;
                results.Columns["release"].ReadOnly = false;
                results.Columns["release"].MaxLength = 500;
                results.Columns["sprint"].ReadOnly = false;
                results.Columns["sprint"].MaxLength = 500;
                results.Columns["screenshots"].ReadOnly = false;
                results.Columns["screenshots"].MaxLength = 5000;
                results.Columns["screenshotDescriptions"].ReadOnly = false;
                results.Columns["screenshotDescriptions"].MaxLength = 5000;

                foreach (DataRow dataRow in results.Rows)
                {
                    if (dataRow["groupTestAbbreviation"] != null && dataRow["groupTestAbbreviation"].ToString() == "0")
                    {
                        dataRow["groupTestAbbreviation"] = "";
                    }

                    string groupsQuery = "Select groupTestAbbreviation from GroupTestCases where projectAbbreviation = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' and testCaseId = " + dataRow["testCaseId"].ToString();
                    dataRow["groupTestAbbreviation"] = String.Join("|", DatabaseUtilities.GetStringListFromQuery(groupsQuery)) ?? "";

                    string releaseQuery = "Select release from ReleaseTestCases where projectAbbreviation = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' and testCaseId = " + dataRow["testCaseId"].ToString();
                    dataRow["release"] = String.Join("|", DatabaseUtilities.GetStringListFromQuery(releaseQuery)) ?? "";

                    string sprintQuery = "Select sprint from SprintTestCases where projectAbbreviation = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' and testCaseId = " + dataRow["testCaseId"].ToString();
                    dataRow["sprint"] = String.Join("|", DatabaseUtilities.GetStringListFromQuery(sprintQuery)) ?? "";

                    string screenshotQuery = "Select imageURL from TestCaseScreenshots where projectAbbreviation = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' and testCaseId = " + dataRow["testCaseId"].ToString() + " order by imageID";
                    dataRow["screenshots"] = String.Join("|", DatabaseUtilities.GetStringListFromQuery(screenshotQuery)) ?? "";

                    string screenshotDescriptionsQuery = "Select description from TestCaseScreenshots where projectAbbreviation = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' and testCaseId = " + dataRow["testCaseId"].ToString() + " order by imageID";
                    dataRow["screenshotDescriptions"] = String.Join("|", DatabaseUtilities.GetStringListFromQuery(screenshotDescriptionsQuery)) ?? "";
                }

                string unmappedFile = "~/Admin/ExportFiles/" + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) + "Export.xls";
                string file = Server.MapPath(unmappedFile);

                ExcelWriter.GenerateXLS(file, "TestCases", results);

                Response.Redirect(unmappedFile);
            }
        }

        // Add selected test cases to a Group
        protected void AddToGroup(object sender, ImageClickEventArgs e)
        {
            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }
            
            if (ddlProjects.SelectedValue == "")
            {
                label.Text = "Please select a project first";
                label.Visible = true;
            }
            else
            {
                label.Visible = false;

                DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
                dropdown.Visible = true;

                SqlDataSource data = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlAddToaBunchOfThings");
                data.SelectCommand = "select groupTestAbbreviation, groupTestName from GroupTests where ([projectAbbreviation] = '" + ddlProjects.SelectedValue + "') and (personalGroupOwner is null or personalGroupOwner = '" + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) + "') order by groupTestName";

                dropdown.DataTextField = "groupTestName";
                dropdown.DataValueField = "groupTestAbbreviation";

                data.DataBind();
                dropdown.DataBind();

                if (dropdown.Items.Count == 0
                    || dropdown.Items[0].Value != "")
                {
                    dropdown.Items.Insert(0, "Select a Group Test");
                    dropdown.Items[0].Value = "";
                    dropdown.SelectedIndex = 0;
                }

                HttpContext.Current.Session["TypeOfThingBeingAdded"] = "Group";
                ddlAddToaBunchOfThings_SetValidation();
            }
        }

        // Add selected test cases to a Release
        protected void AddToRelease(object sender, ImageClickEventArgs e)
        {
            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }

            if (ddlProjects.SelectedValue == "")
            {
                label.Text = "Please select a project first";
                label.Visible = true;
            }
            else
            {
                label.Visible = false;

                DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
                dropdown.Visible = true;

                SqlDataSource data = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlAddToaBunchOfThings");
                data.SelectCommand = "select release from Releases WHERE ([projectAbbreviation] = '" + ddlProjects.SelectedValue + "') order by release";

                dropdown.DataTextField = "release";
                dropdown.DataValueField = "release";

                data.DataBind();
                dropdown.DataBind();

                if (dropdown.Items.Count == 0
                    || dropdown.Items[0].Value != "")
                {
                    dropdown.Items.Insert(0, "Select a Release");
                    dropdown.Items[0].Value = "";
                    dropdown.SelectedIndex = 0;
                }

                HttpContext.Current.Session["TypeOfThingBeingAdded"] = "Release";
                ddlAddToaBunchOfThings_SetValidation();
            }
        }

        // Add selected test cases to a Sprint
        protected void AddToSprint(object sender, ImageClickEventArgs e)
        {
            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }

            if (ddlProjects.SelectedValue == "")
            {
                label.Text = "Please select a project first";
                label.Visible = true;
            }
            else
            {
                label.Visible = false;

                DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
                dropdown.Visible = true;

                SqlDataSource data = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlAddToaBunchOfThings");
                data.SelectCommand = "select sprint from Sprints WHERE ([projectAbbreviation] = '" + ddlProjects.SelectedValue + "') order by sprint";

                dropdown.DataTextField = "sprint";
                dropdown.DataValueField = "sprint";

                data.DataBind();
                dropdown.DataBind();

                if (dropdown.Items.Count == 0
                    || dropdown.Items[0].Value != "")
                {
                    dropdown.Items.Insert(0, "Select a Sprint");
                    dropdown.Items[0].Value = "";
                    dropdown.SelectedIndex = 0;
                }

                HttpContext.Current.Session["TypeOfThingBeingAdded"] = "Sprint";
                ddlAddToaBunchOfThings_SetValidation();
            }
        }

        // Enter a passing result for each browser for each selected test case
        protected void MassPass(object sender, ImageClickEventArgs e)
        {
            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }
            List<int> testCaseList = GetFilteredTestCaseIDs();
            List<string> browserList = new List<string>();
            foreach (DataControlField col in gvTestCaseList.Columns)
            {
                if (col.Visible == true && col.FooterText != "")
                {
                    browserList.Add(col.FooterText);
                }
            }

            //Process each browser across all tests.
            foreach (int testCaseId in testCaseList)
            {

                Dictionary<string, string> dictionary = CTMethods.GetTestCaseStatuses(ddlProjects.SelectedValue, testCaseId);

                //Parse the dictionary list
                bool active = Convert.ToBoolean(dictionary["active"]);
                bool tcOutdated = Convert.ToBoolean(dictionary["testCaseOutdated"]);
                bool tsOutdated = Convert.ToBoolean(dictionary["testScriptOutdated"]);

                if (active == false || tcOutdated == true || tsOutdated == true)
                {
                    continue;
                }
                
                foreach (string browserAbbreviation in browserList)
                {
                    try
                    {
                        SeleniumTestBase.InsertTestResult(
                        ddlProjects.SelectedValue,
                        testCaseId,
                        ddlEnvironment.SelectedValue,
                        browserAbbreviation,
                        "Pass",
                        "Test passed via pressing the Mass Pass button on the Test Cases page by" +
                                " " + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) +
                                " at " + DateTime.Now,
                        "Test passed via pressing the Mass Pass button on the Test Cases page by" +
                                " " + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) +
                                " at " + DateTime.Now,
                        "",
                        "",
                        AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                        "Manual"
                        );
                    }
                    catch (Exception ex)
                    {
                        label.Text = "Insert FAILED for test" +
                                " " + testCaseId + " in browser " + browserAbbreviation + 
                                " with error: \"" + ex.Message + "\"";
                        label.Visible = true;
                    }
                }
            }

            if (testCaseList.Count == 0)
            {
                label.Text = "Please select one or more test cases first";
                label.Visible = true;
            }
            else
            {
                label.Text = testCaseList.Count.ToString() + " tests have been automatically passed!";
                label.Visible = true;
                HttpContext.Current.Session["CurrentMessage"] = testCaseList.Count.ToString() + " tests passed across " + browserList.Count.ToString() + " browsers";
                Response.Redirect(Request.RawUrl);
            }
        }

        // Add an 'In Queue' test result for each selected test case
        // Automation Test Engine will process results with InQueue status, 
        // execute the tests, and update the result status with the test result.
        protected void AddToAutomatedQueue(object sender, ImageClickEventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }

            List<int> testCaseList = GetFilteredTestCaseIDs();

            DateTime startTime = SeleniumTestBase.GetEstimatedTimeCurrentlyQueuedAutomatedTestsWillFinish();
            int numberOfTestsKickedOff = 0;
            foreach (int testCaseId in testCaseList)
            {
                string commandArgument = GetCommandArgument(testCaseId);
                bool results = RunAutomatedTest(commandArgument);
                if (results)
                {
                    numberOfTestsKickedOff++;
                }
            }
            DateTime endTime = SeleniumTestBase.GetEstimatedTimeCurrentlyQueuedAutomatedTestsWillFinish();

            if (testCaseList.Count == 0)
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Please select one or more test cases first";
                label.Visible = true;
            }
            else
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = testCaseList.Count.ToString() + " " + (testCaseList.Count > 1 ? "Test Cases have" : "Test Case has") + " been started or queued.";
                label.Visible = true;

                //FiltersChanged();

                HttpContext.Current.Session["CurrentMessage"] = "Your " + numberOfTestsKickedOff + " tests have been added to the testing queue. You will be emailed the results when your tests are completed."; // The estimated time your tests will start is " + String.Format("{0:G}", startTime) + ", and the estimated time your tests will end is " + String.Format("{0:G}", endTime);
                
                Response.Redirect(Request.RawUrl);
            }
        }

        // Remove all In Queue entries for selected test cases.
        // Test cases already in progress will continue and can not be stopped.
        protected void RemoveFromAutomationQueue(object sender, ImageClickEventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }

            List<int> testCaseList = GetFilteredTestCaseIDs();
            foreach (int testCaseId in testCaseList)
            {
                /*  
                 * CommandPieces[0] = projectAbbreviation
                 * CommandPieces[1] = testCaseId
                 * CommandPieces[2] = autoTestClass
                 * CommandPieces[3] = autoMetaDataTable
                 * CommandPieces[4] = autoMetaDataRow
                 * CommandPieces[5] = environment
                 * CommandPieces[6] = childTestCaseId
                */
                string commandArgument = GetCommandArgument(testCaseId);
                string[] commandPieces = commandArgument.Split(new[] { '|' });

                string projectAbbreviation = commandPieces[0];
                int testCaseID = Convert.ToInt32(commandPieces[1]);
                string environment = commandPieces[5];

                CTMethods.RemoveFromQueue(projectAbbreviation, testCaseID.ToString(), environment);
            }
            
            if (testCaseList.Count == 0)
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Please select one or more test cases that are in queue first";
                label.Visible = true;
            }
            else
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = testCaseList.Count.ToString() + " " + (testCaseList.Count > 1 ? "Test Cases have" : "Test Case has") + " been removed from queue.";
                label.Visible = true;

                //FiltersChanged();

                HttpContext.Current.Session["CurrentMessage"] = "Your " + testCaseList.Count + " tests have been removed from the testing queue.";

                Response.Redirect(Request.RawUrl);
            }
        }

        // Delete selected test cases
        protected void BulkDelete(object sender, ImageClickEventArgs e)
        {
            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }

            List<int> testCaseList = GetFilteredTestCaseIDs();

            foreach (int testCaseId in testCaseList)
            {
                CTMethods.RemoveTestCase(ddlProjects.SelectedValue, testCaseId.ToString(), User.Identity);
            }


            if (testCaseList.Count == 0)
            {
                label.Text = "Please select one or more test cases first";
                label.Visible = true;
            }
            else
            {
                label.Text = testCaseList.Count.ToString() + " " + (testCaseList.Count > 1 ? "Test Cases have" : "Test Case has") + " been deleted";
                label.Visible = true;

                FiltersChanged();
            }
        }

        protected void chkHeader_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");

            for (int i = 0;i < gvTestCaseList.Rows.Count;i++)
            {
                CheckBox chkrow = (CheckBox)gvTestCaseList.Rows[i].FindControl("chkSelected");
                chkrow.Checked = chk.Checked;
            }

            SetCheckBoxEnabledStatus();
        }

        private void SetCheckBoxEnabledStatus()
        {
            if (gvTestCaseList.HeaderRow != null)
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");

                if (chk != null)
                {
                    for (int i = 0;i < gvTestCaseList.Rows.Count;i++)
                    {
                        CheckBox chkrow = (CheckBox)gvTestCaseList.Rows[i].FindControl("chkSelected");
                        chkrow.Enabled = !chk.Checked;
                    }
                }
            }
        }

        protected void ddlAddToaBunchOfThings_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");
            }
            catch
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "There are no test cases loaded in the test case list.";
                label.Visible = true;
                return;
            }

            DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
            dropdown.Visible = false;


            List<int> testCaseList = GetFilteredTestCaseIDs();

            foreach (int testCaseId in testCaseList)
            {
                switch (HttpContext.Current.Session["TypeOfThingBeingAdded"].ToString())
                {
                    case "Group":
                        CTMethods.AddSingleGroupIfNeeded(Convert.ToInt32(testCaseId), ddlProjects.SelectedValue, dropdown.SelectedValue);
                        break;
                    case "Release":
                        CTMethods.AddSingleReleaseIfNeeded(Convert.ToInt32(testCaseId), ddlProjects.SelectedValue, dropdown.SelectedValue);
                        break;
                    case "Sprint":
                        CTMethods.AddSingleSprintIfNeeded(Convert.ToInt32(testCaseId), ddlProjects.SelectedValue, dropdown.SelectedValue);
                        break;
                    case "User":
                        CTMethods.AssignSingleUserIfNeeded(Convert.ToInt32(testCaseId), ddlProjects.SelectedValue, dropdown.SelectedValue);
                        break;
                }
            }


            if (testCaseList.Count == 0)
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Please select one or more test cases first";
                label.Visible = true;                
            }
            else
            {
                if (HttpContext.Current.Session["TypeOfThingBeingAdded"].ToString() == "User")
                {
                    Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                    label.Text = testCaseList.Count.ToString() + " " + (testCaseList.Count > 1 ? "Test Cases have" : "Test Case has") + " been assigned to '" + dropdown.SelectedItem.Text + "'";
                    label.Visible = true;
                }
                else
                {
                    Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                    label.Text = testCaseList.Count.ToString() + " " + (testCaseList.Count > 1 ? "Test Cases have" : "Test Case has") + " been added to the '" + dropdown.SelectedItem.Text + "' " + HttpContext.Current.Session["TypeOfThingBeingAdded"].ToString();
                    label.Visible = true;
                }
            }
        }
        #endregion


        // Reset all filter drop downs back to default values
        protected void btnReset_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = null;
            HttpContext.Current.Session["CurrentEnvironment"] = null;
            HttpContext.Current.Session["ManualOrAutomated"] = null;
            HttpContext.Current.Session["TypeOfThingBeingAdded"] = null;

            HttpContext.Current.Session["CurrentSprint"] = null;
            HttpContext.Current.Session["CurrentRelease"] = null;
            HttpContext.Current.Session["CurrentGroup"] = null;
            HttpContext.Current.Session["Keyword"] = null;

            HttpContext.Current.Session["StartDate"] = null;
            HttpContext.Current.Session["EndDate"] = null;

            HttpContext.Current.Session["PageIndex"] = null;

            Response.Redirect("~/FunctionalTesting/TestCases.aspx");
        }

        protected void btnKeywordSearchSubmit_Click(object sender, ImageClickEventArgs e)
        {
            FiltersChanged();
        }

        protected void sqlTestCasesByProject_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {

            int NumberOfBrowsers = 0;
            int NumberOfTotalTests = 0;
            Label lblRecordCount = (Label)AspUtilities.FindControlRecursive(this, "lblRecordCount");
            Label lblNumberOfTestCasesNumbers = (Label)AspUtilities.FindControlRecursive(this, "lblNumberOfTestCasesNumbers");
            Label lblNumberOfBrowsersNumbers = (Label)AspUtilities.FindControlRecursive(this, "lblNumberOfBrowsersNumbers");
            Label lblNumberOfTotalTestsNumbers = (Label)AspUtilities.FindControlRecursive(this, "lblNumberOfTotalTestsNumbers");

            if (lblRecordCount != null)
            {
                string recordCount = e.AffectedRows.ToString();
                lblRecordCount.Text = "Test Cases Found: " + recordCount;
                NumberOfBrowsers = CTMethods.getVisibleBrowserColumns(gvTestCaseList, ddlProjects.SelectedValue).Length;

                lblNumberOfTestCasesNumbers.Text = recordCount;
                lblNumberOfBrowsersNumbers.Text = NumberOfBrowsers.ToString();
                NumberOfTotalTests = e.AffectedRows * NumberOfBrowsers;
                lblNumberOfTotalTestsNumbers.Text = NumberOfTotalTests.ToString();


            }

        }

        protected void lstbxBrowserSelection_DataBound(object sender, EventArgs e)
        {
            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];
            
            for (int i = 0;i < lstbxBrowserSelection.Items.Count;i++)
            {
                    lstbxBrowserSelection.Items[i].Selected = true;
            }
        }
    }
}

