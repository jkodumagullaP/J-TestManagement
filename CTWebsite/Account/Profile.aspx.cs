using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using CTInfrastructure;



namespace CTWebsite.Account
{
    public partial class Profile : System.Web.UI.Page
    {

        protected int gvTaskListTestDateColumn
        {
            get
            {
                return AspUtilities.GetColumnIndexByDBName(gvTaskList, "testDate");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {


            SqlDataSource ods3 = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlProfileData");
            if (ods3.SelectParameters["username"] == null)
            {
                Parameter userParam = new Parameter();
                userParam.Name = "username";
                userParam.DefaultValue = AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods3.SelectParameters.Add(userParam);
            }

            SqlDataSource ods = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlTaskList");
            if (ods.SelectParameters["username"] == null)
            {
                Parameter userParam = new Parameter();
                userParam.Name = "username";
                userParam.DefaultValue = AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods.SelectParameters.Add(userParam);
            }

            if (Page.IsPostBack == false)
            {

                //Initialize gridviews and dropdown boxes.
                if (ddlProjects.Items.Count > 0)
                {
                    ddlProjects.Items.Insert(0, "All Projects");
                    ddlProjects.Items[0].Value = "";
                    ddlProjects.SelectedIndex = 0;
                }


                
                // Get logged in user's default project
                string defaultProject = CTMethods.GetDefaultProject(HttpContext.Current.User.Identity);

                if (defaultProject != null)
                {
                    ddlProjects.SelectedValue = defaultProject;
                }

                //Environment

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
                    ddlEnvironment.Items.Insert(0, "No Environments Available");
                    ddlEnvironment.Items[0].Value = "";
                }


                ddlProjects.DataBind();
                ddlEnvironment.DataBind();
                //FiltersChanged();
                #region Set Browser Column Visibility

                System.Web.UI.WebControls.BoundField DataColumn;

                //Get the list of browser columns in the gridview
                int[] gridviewBrowserColumns = CTMethods.gvBrowserStatusColumns(gvTaskList);

                //Get the list of which ones of those should be visible
                int[] gridviewVisibleBrowserColumns = CTMethods.getVisibleBrowserColumns(gvTaskList, ddlProjects.SelectedValue);

                //Loop through the browser columns
                foreach (int column in gridviewBrowserColumns)
                {
                    DataColumn = gvTaskList.Columns[column] as System.Web.UI.WebControls.BoundField;

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

            // This line LISTENS to the event, a.k.a. subscribes to it, and says what method to call if it hears anything.
            AddNewGroup.ReadyForDatabindEvent += DoGroupDataBind;

            SqlDataSource ods2 = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlPersonalTestCaseGroups");
            if (ods2.SelectParameters["username"] == null)
            {
                Parameter userParam2 = new Parameter();
                userParam2.Name = "username";
                userParam2.DefaultValue = AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods2.SelectParameters.Add(userParam2);
            }



            RefreshLabels();

            FiltersChanged();
        }

        // This is the actual event handler. When it hears the event, it does something
        public void DoGroupDataBind(object sender, EventArgs e)
        {
            ListView lvPersonalTestCaseGroups = (ListView)AspUtilities.FindControlRecursive(this, "lvPersonalTestCaseGroups");
            lvPersonalTestCaseGroups.DataBind();
        }

        public void populateDefaultProjects()
        {
            string defaultProjectName = CTMethods.GetDefaultProjectName(HttpContext.Current.User.Identity);
            Label lblDefaultProject = (Label)AspUtilities.FindControlRecursive(this, "lblDefaultProject");
            if (lblDefaultProject != null)
            {
                lblDefaultProject.Text = defaultProjectName;
            }

            DropDownList ddlDefaultProjects = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlDefaultProjects");
            if (ddlDefaultProjects != null)
            {
                ddlDefaultProjects.SelectedValue = defaultProjectName;
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Account/ChangePassword.aspx");
        }

       
        
        protected void lvPersonalTestCaseGroups_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            /*  
             * CommandPieces[0] = projectAbbreviation
             * CommandPieces[1] = groupTestAbbreviation
            */
            if (e.CommandName == "RemovePersonalGroup")
            {
                string commandArgument = (e.CommandArgument ?? "").ToString();
                string[] commandPieces = commandArgument.Split(new[] { '|' });

                string projectAbbreviation = commandPieces[0];
                string groupTestAbbreviation = commandPieces[1];

                CTMethods.RemoveGroup(projectAbbreviation, groupTestAbbreviation);

                ListView lvPersonalTestCaseGroups = (ListView)AspUtilities.FindControlRecursive(this, "lvPersonalTestCaseGroups");
                lvPersonalTestCaseGroups.DataBind();
            }
        }

        protected void btnAddPersonalGroup_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminGroups.aspx?command=Insert");
        }

        protected void gvTaskList_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gvTaskList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            /*  
             * CommandPieces[0] = projectAbbreviation
             * CommandPieces[1] = testCaseID
            */
            if (e.CommandName == "Unassign")
            {
                string commandArgument = (e.CommandArgument ?? "").ToString();
                string[] commandPieces = commandArgument.Split(new[] { '|' });

                string projectAbbreviation = commandPieces[0];
                int testCaseID = Convert.ToInt32(commandPieces[1]);

                CTMethods.UnAssignSingleUser(Convert.ToInt32(testCaseID), projectAbbreviation, AspUtilities.GetUserGuid(AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name)));

                gvTaskList.DataBind();
            }

        }

        protected void gvTaskList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                int[] gvTaskListStatusColumns = CTMethods.gvBrowserStatusColumns(gvTaskList);

                foreach (int gvTaskListStatusColumn in gvTaskListStatusColumns)
                {
                    if (e.Row.Cells[gvTaskListStatusColumn].Text == "Pass")
                    {
                        string testDateString = e.Row.Cells[gvTaskListTestDateColumn].Text;

                        DateTime testDate;

                        bool parsedSuccessfully = DateTime.TryParse(testDateString, out testDate);

                        if (parsedSuccessfully)
                        {
                            TimeSpan timeSinceTested = DateTime.Now.Subtract(testDate);
                            TimeSpan testSpanDays = new TimeSpan(7, 0, 0, 0);

                            if (timeSinceTested < testSpanDays)
                            {
                                e.Row.CssClass = "testSpanRow";
                            }
                        }
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusPass";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "Pass";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTaskListStatusColumn].Text == "Fail")
                    {
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusFail";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "Fail";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTaskListStatusColumn].Text == "Test Case Needs Updated")
                    {
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusTestCaseNeedsUpdated";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "Test Case Needs Updated";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTaskListStatusColumn].Text == "Automated Test Needs Updated")
                    {
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusAutomatedTestNeedsUpdated";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "Automated Test Needs Updated";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTaskListStatusColumn].Text == "Not Started")
                    {
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusNotStarted";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "Not Started";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTaskListStatusColumn].Text == "Not Implemented")
                    {
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusNotImplemented";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "Not Implemented";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTaskListStatusColumn].Text == "Not Applicable")
                    {
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusNotApplicable";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "Not Applicable";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTaskListStatusColumn].Text == "In Progress")
                    {
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusInProgress";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "In Progress";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTaskListStatusColumn].Text == "In Queue")
                    {
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusInQueue";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "In Queue";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }
                    else if (e.Row.Cells[gvTaskListStatusColumn].Text == "Retest")
                    {
                        e.Row.Cells[gvTaskListStatusColumn].CssClass = "statusRetest";
                        e.Row.Cells[gvTaskListStatusColumn].ToolTip = "Retest";
                        e.Row.Cells[gvTaskListStatusColumn].Text = "";
                    }

                    if (e.Row.Cells[gvTaskListStatusColumn].Text == "Pass")
                    {
                    }
                }
            }
        }




        public void RefreshLabels()
        {
            userManualCasesWritten.Text = ManualCasesWrittenByUser().ToString();
            userAutomatedCasesWritten.Text = AutomatedCasesWrittenByUser().ToString();

            userManualCasesExecuted.Text = ManualCasesExecutedByUser().ToString();
            userAutomatedCasesExecuted.Text = AutomatedCasesExecutedByUser().ToString();

            userManualDefectsDiscovered.Text = ManualDefectsDiscoveredByUser().ToString();
            userAutomatedDefectsDiscovered.Text = AutomatedDefectsDiscoveredByUser().ToString();
        }

        public int ManualCasesWrittenByUser()
        {
            string query = " select count(*) from "
            + " ( "
            + " SELECT "
            + " distinct TestCases.projectabbreviation, TestCases.testcaseid "
            + " FROM TestCases"
            + " left JOIN dbo.GroupTestCases ON dbo.GroupTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.GroupTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.ReleaseTestCases ON dbo.ReleaseTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.ReleaseTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.SprintTestCases ON dbo.SprintTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.SprintTestCases.testCaseId = dbo.TestCases.testCaseId "
            + LabelQuery_TestCaseLevelFilter("dateCreated")
            + " and createdBy = '" + AspUtilities.RemovePrefixFromUserName(User.Identity.Name) + "'"
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int AutomatedCasesWrittenByUser()
        {
            string query = " select count(*) from "
            + " ( "
            + " SELECT "
            + " distinct TestCases.projectabbreviation, TestCases.testcaseid "
            + " FROM TestCases"
            + " left JOIN dbo.GroupTestCases ON dbo.GroupTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.GroupTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.ReleaseTestCases ON dbo.ReleaseTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.ReleaseTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.SprintTestCases ON dbo.SprintTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.SprintTestCases.testCaseId = dbo.TestCases.testCaseId "
            + LabelQuery_TestCaseLevelFilter("dateAutomatedTestCreated")
            + " and automatedTestCreatedBy = '" + AspUtilities.RemovePrefixFromUserName(User.Identity.Name) + "'"
            + " and autotestclass is not null "
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int ManualCasesExecutedByUser()
        {
            string query = " select count(*) from "
            + " ( "
            + " SELECT "
            + " distinct testRunID "
            + " FROM TestResults "
            + " join TestCases on TestCases.ProjectAbbreviation = TestResults.ProjectAbbreviation and TestCases.TestCaseID = TestResults.TestCaseID "
            + " left JOIN dbo.GroupTestCases ON dbo.GroupTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.GroupTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.ReleaseTestCases ON dbo.ReleaseTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.ReleaseTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.SprintTestCases ON dbo.SprintTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.SprintTestCases.testCaseId = dbo.TestCases.testCaseId "
            + LabelQuery_TestCaseLevelFilter("testDate")
            + " and testtype = 'Manual' "
            + " and testedBy = '" + AspUtilities.RemovePrefixFromUserName(User.Identity.Name) + "'"
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int AutomatedCasesExecutedByUser()
        {
            string query = " select count(*) from "
            + " ( "
            + " SELECT "
            + " distinct testRunID "
            + " FROM TestResults "
            + " join TestCases on TestCases.ProjectAbbreviation = TestResults.ProjectAbbreviation and TestCases.TestCaseID = TestResults.TestCaseID "
            + " left JOIN dbo.GroupTestCases ON dbo.GroupTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.GroupTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.ReleaseTestCases ON dbo.ReleaseTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.ReleaseTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.SprintTestCases ON dbo.SprintTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.SprintTestCases.testCaseId = dbo.TestCases.testCaseId "
            + LabelQuery_TestCaseLevelFilter("testDate")
            + " and testtype = 'Automated' "
            + " and testedBy = '" + AspUtilities.RemovePrefixFromUserName(User.Identity.Name) + "'"
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int ManualDefectsDiscoveredByUser()
        {
            string query = " select count(*) from "
            + " ( "
            + " SELECT "
            + " distinct defectTicketNumber "
            + " FROM TestResults "
            + " join TestCases on TestCases.ProjectAbbreviation = TestResults.ProjectAbbreviation and TestCases.TestCaseID = TestResults.TestCaseID "
            + " left JOIN dbo.GroupTestCases ON dbo.GroupTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.GroupTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.ReleaseTestCases ON dbo.ReleaseTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.ReleaseTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.SprintTestCases ON dbo.SprintTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.SprintTestCases.testCaseId = dbo.TestCases.testCaseId "
            + LabelQuery_TestCaseLevelFilter("testDate")
            + " and testedBy = '" + AspUtilities.RemovePrefixFromUserName(User.Identity.Name) + "'"
            + " and testtype = 'Manual' "
            + " ) innerQuery "
            + " where defectTicketNumber is not null and defectTicketNumber != ''";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int AutomatedDefectsDiscoveredByUser()
        {
            string query = " select count(*) from "
            + " ( "
            + " SELECT "
            + " distinct defectTicketNumber "
            + " FROM TestResults "
            + " join TestCases on TestCases.ProjectAbbreviation = TestResults.ProjectAbbreviation and TestCases.TestCaseID = TestResults.TestCaseID "
            + " left JOIN dbo.GroupTestCases ON dbo.GroupTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.GroupTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.ReleaseTestCases ON dbo.ReleaseTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.ReleaseTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.SprintTestCases ON dbo.SprintTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.SprintTestCases.testCaseId = dbo.TestCases.testCaseId "
            + LabelQuery_TestCaseLevelFilter("testDate")
            + " and testedBy = '" + AspUtilities.RemovePrefixFromUserName(User.Identity.Name) + "'"
            + " and testtype = 'Automated' "
            + " ) innerQuery "
            + " where defectTicketNumber is not null and defectTicketNumber != ''";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public string LabelQuery_TestCaseLevelFilter(string fieldDateAppliesTo)
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            return " where " + TrimDateToMidnight(fieldDateAppliesTo) + "  >= '" + startDate.ToSqlString() + "' and " + TrimDateToMidnight(fieldDateAppliesTo) + "  <= '" + endDate.ToSqlString() + "' ";
        }

        public string TrimDateToMidnight(string dateFieldName)
        {
            return "DATEADD(dd, DATEDIFF(dd,0," + dateFieldName + "), 0)";
        }

        protected void ddlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentEnvironment"] = ddlEnvironment.SelectedValue;
            FiltersChanged();
        }

        protected void FiltersChanged()
        {
            DropDownList ddlEnvironment = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlEnvironment");
            gvTaskList.Caption = "Project: " + ddlProjects.SelectedItem.Text;

            gvTaskList.Caption += "<br/>" + "Environment: " + ddlEnvironment.SelectedValue;



            SqlDataSource sqlTaskList = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlTaskList");

            sqlTaskList.SelectCommand =
                "select distinct "
              + "    LatestTestRunIDPerBrowser.environment, "
              + "    LatestTestRunIDPerBrowser.projectAbbreviation, "
              + "    LatestTestRunIDPerBrowser.testCaseId, "
              + "    LatestTestRunIDPerBrowser.testCaseDescription, "
              + "    autoTestClass, "
              + "    autoMetaDataTable, "
              + "    autoMetaDataRow, "
              + "    testCategory, "
              + "    LatestTestRunIDPerBrowser.testDate, "
              + "    Coalesce(Browser1TestResults.status, 'Not Started') as Browser1Status, "
              + "    Coalesce(Browser2TestResults.status, 'Not Started') as Browser2Status, "
              + "    Coalesce(Browser3TestResults.status, 'Not Started') as Browser3Status, "
              + "    Coalesce(Browser4TestResults.status, 'Not Started') as Browser4Status, "
              + "    Coalesce(Browser5TestResults.status, 'Not Started') as Browser5Status, "
              + "    Coalesce(Browser6TestResults.status, 'Not Started') as Browser6Status, "
              + "    Coalesce(Browser7TestResults.status, 'Not Started') as Browser7Status, "
              + "    Coalesce(Browser8TestResults.status, 'Not Started') as Browser8Status, "
              + "    dateAssigned "
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
              + "left JOIN dbo.GroupTestCases "
              + "    ON dbo.GroupTestCases.projectAbbreviation = LatestTestRunIDPerBrowser.projectAbbreviation "
              + "    AND dbo.GroupTestCases.testCaseId = LatestTestRunIDPerBrowser.testCaseId "
              + "left JOIN dbo.ReleaseTestCases "
              + "    ON dbo.ReleaseTestCases.projectAbbreviation = LatestTestRunIDPerBrowser.projectAbbreviation "
              + "    AND dbo.ReleaseTestCases.testCaseId = LatestTestRunIDPerBrowser.testCaseId "
              + "left JOIN dbo.SprintTestCases "
              + "    ON dbo.SprintTestCases.projectAbbreviation = LatestTestRunIDPerBrowser.projectAbbreviation "
              + "    AND dbo.SprintTestCases.testCaseId = LatestTestRunIDPerBrowser.testCaseId "
              + " join dbo.aspnet_Users "
              + "   on aspnet_Users.UserName = '" + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) + "' "
              + " join dbo.TestCaseAssignments "
              + "   ON TestCaseAssignments.ProjectAbbreviation = LatestTestRunIDPerBrowser.ProjectAbbreviation "
              + "   AND TestCaseAssignments.TestCaseID = LatestTestRunIDPerBrowser.TestCaseID "
              + "   and TestCaseAssignments.UserID = aspnet_Users.UserID ";

            sqlTaskList.SelectCommand += GetFilter();


            sqlTaskList.SelectCommand +=
              " group by "
            + "    LatestTestRunIDPerBrowser.environment, "
            + "    LatestTestRunIDPerBrowser.projectAbbreviation, "
            + "    LatestTestRunIDPerBrowser.testCaseId, "
            + "    LatestTestRunIDPerBrowser.testCaseDescription, "
            + "    autoTestClass, "
            + "    autoMetaDataTable, "
            + "    autoMetaDataRow, "
            + "    testCategory, "
            + "    LatestTestRunIDPerBrowser.testDate, "
            + "    Browser1TestResults.status, "
            + "    Browser2TestResults.status, "
            + "    Browser3TestResults.status, "
            + "    Browser4TestResults.status, "
            + "    Browser5TestResults.status, "
            + "    Browser6TestResults.status, "
            + "    Browser7TestResults.status, "
            + "    Browser8TestResults.status, "
            + "    dateAssigned ";

            sqlTaskList.SelectCommand += " order by dateAssigned desc";

        }
        
        public string GetFilter()
        {
            DropDownList ddlEnvironment = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlEnvironment");
            string filter = " WHERE (LatestTestRunIDPerBrowser.[Environment] = '" + DatabaseUtilities.MakeSQLSafe(ddlEnvironment.SelectedValue) + "')";
            filter += " AND (LatestTestRunIDPerBrowser.[projectAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' OR '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' = '')";

            return filter;
        }

        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {

            //Environment
            ddlEnvironment.DataBind();

            //If an environment is stored in a session variable, select it
            if (HttpContext.Current.Session["CurrentEnvironment"] != null)
            {
                try
                {
                    ddlEnvironment.SelectedValue = HttpContext.Current.Session["CurrentEnvironment"].ToString();
                }
                catch
                {
                    HttpContext.Current.Session["CurrentEnvironment"] = null;
                }
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
                ddlEnvironment.Items.Insert(0, "Select an Environment");
                ddlEnvironment.Items[0].Value = "";
            }

            FiltersChanged();
        }

        protected void fvProfileData_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            
            if (e.CommandName == "EditPersonalData")
            {
                Label ErrorMessage2 = (Label)AspUtilities.FindControlRecursive(this, "ErrorMessage2");
                Label lblUsername = (Label)AspUtilities.FindControlRecursive(fvProfileData, "lblUsername");
                TextBox txtFirstName = (TextBox)AspUtilities.FindControlRecursive(fvProfileData, "txtFirstName");
                TextBox txtLastName = (TextBox)AspUtilities.FindControlRecursive(fvProfileData, "txtLastName");
                TextBox txtSupervisorFirstName = (TextBox)AspUtilities.FindControlRecursive(fvProfileData, "txtSupervisorFirstName");
                TextBox txtSupervisorLastName = (TextBox)AspUtilities.FindControlRecursive(fvProfileData, "txtSupervisorLastName");
                TextBox txtSupervisorEmail = (TextBox)AspUtilities.FindControlRecursive(fvProfileData, "txtSupervisorEmail");
                DropDownList ddlDefaultProjects = (DropDownList)AspUtilities.FindControlRecursive(fvProfileData, "ddlDefaultProjects");
                CTMethods.UpdateUserProfiles(lblUsername.Text, txtFirstName.Text, txtLastName.Text, txtSupervisorFirstName.Text, txtSupervisorLastName.Text, txtSupervisorEmail.Text, ddlDefaultProjects.SelectedValue);
                Label lblEmail = (Label)AspUtilities.FindControlRecursive(fvProfileData, "lblEmail");
                CTMethods.UpdateUserMembership(lblUsername.Text, lblEmail.Text);

                fvProfileData.ChangeMode(FormViewMode.ReadOnly);
            }
        }

        protected void fvProfileData_DataBinding(object sender, EventArgs e)
        {
        }
    }
}
