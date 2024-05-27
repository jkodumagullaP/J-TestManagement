using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.DataVisualization.Charting;
using Utilities;
using CTInfrastructure;
using System.Web.UI.HtmlControls;


namespace CTWebsite
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Add username parameter to groups query to grab personal groups
            SqlDataSource ods = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlGroupTests");
            if (ods.SelectParameters["username"] == null)
            {
                Parameter userParam = new Parameter();
                userParam.Name = "username";
                userParam.DefaultValue = AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods.SelectParameters.Add(userParam);
            }

            //Set default project and environment
            DropDownList ddlProjects = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlProjects");
            DropDownList ddlEnvironment = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlEnvironment");

            if (Page.IsPostBack == false)
            {
                ddlUsers.DataBind();

                if (ddlUsers.Items.Count > 0)
                {
                    ddlUsers.SelectedIndex = 0;
                }

                ddlBrowsers.DataBind();

                if (ddlBrowsers.Items.Count > 0)
                {
                    ddlBrowsers.SelectedIndex = 0;
                }

                //Initialize gridviews and dropdown boxes.
                
                ddlProjects.DataBind();
                if (ddlProjects.Items.Count > 0)
                {
                    ddlProjects.Items.Insert(0, "All Projects");
                    ddlProjects.Items[0].Value = "";
                    ddlProjects.SelectedIndex = 0;
                }

                if (HttpContext.Current.Session["ManualOrAutomated"] != null)
                {
                    ddlManualOrAutomated.SelectedValue = HttpContext.Current.Session["ManualOrAutomated"].ToString();
                }
                else
                {
                    ddlManualOrAutomated.SelectedValue = "All";
                }

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
                    }
                    else
                    {
                        ddlProjects.SelectedValue = "";
                    }
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
                    ddlEnvironment.Items.Insert(0, "Select an Environment");
                    ddlEnvironment.Items[0].Value = "";
                }

                if (HttpContext.Current.Session["StartDate"] != null)
                {
                    txtStartDate.Text = HttpContext.Current.Session["StartDate"].ToString() ?? "";
                }

                if (HttpContext.Current.Session["EndDate"] != null)
                {
                    txtEndDate.Text = HttpContext.Current.Session["EndDate"].ToString() ?? "";
                }

                ddlProjectsValueChange();

                populateAutomatedMetricsTable();
            }

            FiltersChanged();
        }

        protected void ddlManualOrAutomated_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["ManualOrAutomated"] = ((DropDownList)sender).SelectedValue;
            FiltersChanged();

        }

        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            ddlProjectsValueChange();

        }


        protected void ddlProjectsValueChange()
        {
            //gvFailingTestCasesByProject.DataBind();

            ddlGroupTest.DataBind();

            if (ddlGroupTest.Items.Count == 0
                || ddlGroupTest.Items[0].Value != "")
            {
                ddlGroupTest.Items.Insert(0, "Select a Group Test");
                ddlGroupTest.Items[0].Value = "";
            }

            try
            {
                ddlGroupTest.SelectedValue = HttpContext.Current.Session["CurrentGroup"].ToString();
            }
            catch (Exception)
            {
                HttpContext.Current.Session["CurrentGroup"] = null;
            }

            ddlReleases.DataBind();

            if (ddlReleases.Items.Count == 0
                || ddlReleases.Items[0].Value != "")
            {
                ddlReleases.Items.Insert(0, "Select a Release");
                ddlReleases.Items[0].Value = "";
            }

            try
            {
                ddlReleases.SelectedValue = HttpContext.Current.Session["CurrentRelease"].ToString();
            }
            catch (Exception)
            {
                HttpContext.Current.Session["CurrentRelease"] = null;
            }

            ddlSprints.DataBind();

            if (ddlSprints.Items.Count == 0
                || ddlSprints.Items[0].Value != "")
            {
                ddlSprints.Items.Insert(0, "Select a Sprint");
                ddlSprints.Items[0].Value = "";
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
                ddlEnvironment.Items.Insert(0, "Select an Environment");
                ddlEnvironment.Items[0].Value = "";
            }
            
            populateAutomatedMetricsTable();

            FiltersChanged();
        }

        protected void ddlReleases_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentRelease"] = ddlReleases.SelectedValue;
            FiltersChanged();
        }

        protected void ddlSprints_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentSprint"] = ddlSprints.SelectedValue;
            FiltersChanged();
        }

        protected void ddlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentEnvironment"] = ddlEnvironment.SelectedValue;
            FiltersChanged();
        }

        protected void ddlGroupTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentGroup"] = ddlGroupTest.SelectedValue;
            FiltersChanged();
        }

        protected void FiltersChanged()
        {
            Label lblDashboardTitle = (Label)AspUtilities.FindControlRecursive(this, "lblDashboardTitle");
            lblDashboardTitle.Text = "Project: " + ddlProjects.SelectedItem.Text;
            
            if (ddlGroupTest.SelectedIndex > 0)
            {
                lblDashboardTitle.Text += "<br/>" + "Group: " + ddlGroupTest.SelectedItem.Text; 
            }

            if (ddlReleases.SelectedValue != "")
            {
                lblDashboardTitle.Text += "<br/>" + "Release: " + ddlReleases.SelectedValue; 
            }

            if (ddlSprints.SelectedValue != "")
            {
                lblDashboardTitle.Text += "<br/>" + "Sprint: " + ddlSprints.SelectedValue;
            }
            
            lblDashboardTitle.Text += "<br/>" + "Environment: " + ddlEnvironment.SelectedValue; 
            RefreshGraph();

            if (AreDatesValid())
            {
                if (!String.IsNullOrEmpty(txtStartDate.Text))
                {
                    lblDashboardTitle.Text += "<br/>" + "On or after " + txtStartDate.Text;
                }

                if (!String.IsNullOrEmpty(txtEndDate.Text))
                {
                    lblDashboardTitle.Text += "<br/>" + "On or before " + txtEndDate.Text;
                }
            }


            RefreshLabels();


            SqlDataSource sqlInProgress = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlInProgress");

            sqlInProgress.SelectCommand = " SELECT * FROM TestCaseWithStatus "
                            + " WHERE (status = 'In Progress') "
                            + " ORDER BY testDate DESC";

            SqlDataSource sqlInQueue = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlInQueue");

            sqlInQueue.SelectCommand = " SELECT * FROM TestCaseWithStatus "
                            + " WHERE (status = 'In Queue') "
                            + " ORDER BY testDate DESC";

            SqlDataSource sqlNeedsUpdatedAuto = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlNeedsUpdatedAuto");

            sqlNeedsUpdatedAuto.SelectCommand = " SELECT * FROM TestCases "
                            + " WHERE ([projectAbbreviation] = '" + ddlProjects.SelectedValue + "') AND (testScriptOutdated = 'True') "
                            + " ORDER BY testCaseId ASC";

            SqlDataSource sqlTestCaseNeedsUpdated = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlTestCaseNeedsUpdated");

            sqlTestCaseNeedsUpdated.SelectCommand = " SELECT * FROM TestCases "
                            + " WHERE ([projectAbbreviation] = '" + ddlProjects.SelectedValue + "') AND (testCaseOutdated = 'true') "
                            + " ORDER BY testCaseId ASC";

            lvInProgress.DataBind();
            lvInQueue.DataBind();
            lvNeedsUpdatedAuto.DataBind();
            lvTestCaseNeedsUpdated.DataBind();
        }

        public void RefreshGraph()
        {

            #region Set default values first.
            Chrome_Pass.Text = "0";
            Chrome_Fail.Text = "0";
            Chrome_Other.Text = "0";
            Chrome_Total.Text = "0";
            Firefox_Pass.Text = "0";
            Firefox_Fail.Text = "0";
            Firefox_Other.Text = "0";
            Firefox_Total.Text = "0";
            IE8_Pass.Text = "0";
            IE8_Fail.Text = "0";
            IE8_Other.Text = "0";
            IE8_Total.Text = "0";
            IE9_Pass.Text = "0";
            IE9_Fail.Text = "0";
            IE9_Other.Text = "0";
            IE9_Total.Text = "0";
            IE10_Pass.Text = "0";
            IE10_Fail.Text = "0";
            IE10_Other.Text = "0";
            IE10_Total.Text = "0";
            IE11_Pass.Text = "0";
            IE11_Fail.Text = "0";
            IE11_Other.Text = "0";
            IE11_Total.Text = "0";
            MACSAF_Pass.Text = "0";
            MACSAF_Fail.Text = "0";
            MACSAF_Other.Text = "0";
            MACSAF_Total.Text = "0";
            WINSAF_Pass.Text = "0";
            WINSAF_Fail.Text = "0";
            WINSAF_Other.Text = "0";
            WINSAF_Total.Text = "0";
            Totals_Pass.Text = "0";
            Totals_Fail.Text = "0";
            Totals_Other.Text = "0";
            Totals_Total.Text = "0";
            #endregion

            #region Create Totals for Pass/Fail
            string sql;
            string whereSQL = "";
            string fromSQL = "";
            string[] visibleBrowsers2 = CTMethods.GetVisibleBrowserAbbreviations(ddlProjects.SelectedValue);
            string visibleBrowserListString = String.Join("','", visibleBrowsers2);
            string visbleBrowsersForQuery = "('" + visibleBrowserListString + "')";
            SqlConnection conn = new SqlConnection(Constants.QAAConnectionString);
            conn.Open();

            sql = "select TestResults.projectAbbreviation, TestResults.browserAbbreviation, " +
                        "TestResults.status, COUNT(RunID) as TestCount from TestResults " +
                        "join " +
                        "(select max(testRunId) as RunID, projectAbbreviation, environment, " +
                        "browserAbbreviation, testCaseId " +
                        "from TestResults " +
                        "where status in ('Pass', 'Fail') " +
                        "and browserAbbreviation in " + visbleBrowsersForQuery + " " +
                        GetTestResultFilter(false) +
                        "group by projectAbbreviation, environment, browserAbbreviation, testcaseID " +
                        ") LastTestResults " +
                        "on testRunID = LastTestResults.RunID ";

            fromSQL = "join TestCases " +
                        "on TestResults.projectAbbreviation = TestCases.projectAbbreviation " +
                        "and TestResults.testCaseId = TestCases.testCaseId ";

            whereSQL = "where TestResults.projectAbbreviation = '" + ddlProjects.SelectedValue + "' " +
                        "and TestResults.environment = '" + ddlEnvironment.SelectedValue + "' ";
            #endregion

            #region Apply filters if needed.
            if (ddlSprints.SelectedValue != "")
            {
                fromSQL += "left outer join SprintTestCases " +
                        "on TestResults.projectAbbreviation = SprintTestCases.projectAbbreviation " +
                        "and TestResults.testCaseId = SprintTestCases.testCaseId ";
                whereSQL += "and SprintTestCases.sprint = '" + ddlSprints.SelectedValue + "' ";
            }
            if (ddlGroupTest.SelectedValue != "")
            {
                fromSQL += "left outer join GroupTestCases " +
                        "on TestResults.projectAbbreviation = GroupTestCases.projectAbbreviation " +
                        "and TestResults.testCaseId = GroupTestCases.testCaseId ";
                whereSQL += "and GroupTestCases.groupTestAbbreviation = '" + ddlGroupTest.SelectedValue + "' ";
            }
            if (ddlReleases.SelectedValue != "")
            {
                fromSQL += "left outer join ReleaseTestCases " +
                        "on TestResults.projectAbbreviation = ReleaseTestCases.projectAbbreviation " +
                        "and TestResults.testCaseId = ReleaseTestCases.testCaseId ";
                whereSQL += "and ReleaseTestCases.release = '" + ddlReleases.SelectedValue + "' ";
            }

            fromSQL += " left join (select distinct Projectabbreviation, childtestcaseid from dbo.AutoTestCaseMap) Childtestcases on TestCases.testcaseid = childtestcaseid and TestCases.Projectabbreviation = Childtestcases.Projectabbreviation ";

            if (ddlManualOrAutomated.SelectedValue == "Automated")
            {
                whereSQL += " AND (((TestCases.[autoTestClass] is not null) AND (TestCases.[autoTestClass] <> '')) OR (Childtestcases.childTestCaseId is not null))";
            }

            if (ddlManualOrAutomated.SelectedValue == "Manual")
            {
                whereSQL += " AND ((TestCases.[autoTestClass] is null OR TestCases.[autoTestClass] = '') AND (Childtestcases.childTestCaseId is null))";
            }
            sql = sql + fromSQL + whereSQL +
                    "group by TestResults.projectAbbreviation, TestResults.browserAbbreviation, TestResults.status " +
                    "order by TestResults.projectAbbreviation, TestResults.browserAbbreviation, TestResults.status";

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.Fill(ds);
            #endregion

            #region Set values of cells from the dataset.
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                switch (dr[1].ToString().ToUpper())
                {
                    case "CHROME":
                        if (dr[2].ToString() == "Pass")
                        {
                            Chrome_Pass.Text = dr[3].ToString();
                        }
                        else
                        {
                            Chrome_Fail.Text = dr[3].ToString();
                        }
                        break;
                    case "FF":
                        if (dr[2].ToString() == "Pass")
                        {
                            Firefox_Pass.Text = dr[3].ToString();
                        }
                        else
                        {
                            Firefox_Fail.Text = dr[3].ToString();
                        }
                        break;
                    case "IE8":
                        if (dr[2].ToString() == "Pass")
                        {
                            IE8_Pass.Text = dr[3].ToString();
                        }
                        else
                        {
                            IE8_Fail.Text = dr[3].ToString();
                        }
                        break;

                    case "IE9":
                        if (dr[2].ToString() == "Pass")
                        {
                            IE9_Pass.Text = dr[3].ToString();
                        }
                        else
                        {
                            IE9_Fail.Text = dr[3].ToString();
                        }
                        break;
                    case "IE10":
                        if (dr[2].ToString() == "Pass")
                        {
                            IE10_Pass.Text = dr[3].ToString();
                        }
                        else
                        {
                            IE10_Fail.Text = dr[3].ToString();
                        }
                        break;
                    case "IE11":
                        if (dr[2].ToString() == "Pass")
                        {
                            IE11_Pass.Text = dr[3].ToString();
                        }
                        else
                        {
                            IE11_Fail.Text = dr[3].ToString();
                        }
                        break;
                    case "MACSAF":
                        if (dr[2].ToString() == "Pass")
                        {
                            MACSAF_Pass.Text = dr[3].ToString();
                        }
                        else
                        {
                            MACSAF_Fail.Text = dr[3].ToString();
                        }
                        break;

                    case "WINSAF":
                        if (dr[2].ToString() == "Pass")
                        {
                            WINSAF_Pass.Text = dr[3].ToString();
                        }
                        else
                        {
                            WINSAF_Fail.Text = dr[3].ToString();
                        }
                        break;


                }
            }

            //Calculate Total Test Cases
            sql = "select count(*) from TestCases ";
            fromSQL = " ";

            whereSQL = "where TestCases.projectAbbreviation = '" + ddlProjects.SelectedValue + "' ";
            #endregion

            #region Apply filters if needed
            if (ddlSprints.SelectedValue != "")
            {
                fromSQL += "left outer join SprintTestCases " +
                        "on TestCases.projectAbbreviation = SprintTestCases.projectAbbreviation " +
                        "and TestCases.testCaseId = SprintTestCases.testCaseId ";
                whereSQL += "and SprintTestCases.sprint = '" + ddlSprints.SelectedValue + "' ";
            }
            if (ddlGroupTest.SelectedValue != "")
            {
                fromSQL += "left outer join GroupTestCases " +
                        "on TestCases.projectAbbreviation = GroupTestCases.projectAbbreviation " +
                        "and TestCases.testCaseId = GroupTestCases.testCaseId ";
                whereSQL += "and GroupTestCases.groupTestAbbreviation = '" + ddlGroupTest.SelectedValue + "' ";
            }
            if (ddlReleases.SelectedValue != "")
            {
                fromSQL += "left outer join ReleaseTestCases " +
                        "on TestCases.projectAbbreviation = ReleaseTestCases.projectAbbreviation " +
                        "and TestCases.testCaseId = ReleaseTestCases.testCaseId ";
                whereSQL += "and ReleaseTestCases.release = '" + ddlReleases.SelectedValue + "' ";
            }

            fromSQL += " left join (select distinct Projectabbreviation, childtestcaseid from dbo.AutoTestCaseMap) Childtestcases on TestCases.testcaseid = childtestcaseid and TestCases.Projectabbreviation = Childtestcases.Projectabbreviation ";

            if (ddlManualOrAutomated.SelectedValue == "Automated")
            {
                whereSQL += " AND (((TestCases.[autoTestClass] is not null) AND (TestCases.[autoTestClass] <> '')) OR (Childtestcases.childTestCaseId is not null))";
            }

            if (ddlManualOrAutomated.SelectedValue == "Manual")
            {
                whereSQL += " AND ((TestCases.[autoTestClass] is null OR TestCases.[autoTestClass] = '') AND (Childtestcases.childTestCaseId is null))";
            }


            sql = sql + fromSQL + whereSQL;

            SqlCommand cmd2 = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            #endregion

            #region Assign values to Totals row
            SqlDataReader dr2 = cmd.ExecuteReader();
            dr2.Read();
            if (dr2.HasRows)
            {
                Chrome_Total.Text = dr2[0].ToString();
                Firefox_Total.Text = dr2[0].ToString();
                IE8_Total.Text = dr2[0].ToString();
                IE9_Total.Text = dr2[0].ToString();
                IE10_Total.Text = dr2[0].ToString();
                IE11_Total.Text = dr2[0].ToString();
                MACSAF_Total.Text = dr2[0].ToString();
                WINSAF_Total.Text = dr2[0].ToString();
                Totals_Total.Text = dr2[0].ToString();
            }
            conn.Close();
            #endregion

            #region Calculate and fill Others row

            foreach (string browser in visibleBrowsers2)
            {
                switch (browser)
                {
                    case "CHROME":
                        Chrome_Other.Text = (Convert.ToInt32(Chrome_Total.Text) - Convert.ToInt32(Chrome_Pass.Text) -
                                Convert.ToInt32(Chrome_Fail.Text)).ToString();
                        break;

                    case "FF":
                        Firefox_Other.Text = (Convert.ToInt32(Firefox_Total.Text) - Convert.ToInt32(Firefox_Pass.Text) -
                                Convert.ToInt32(Firefox_Fail.Text)).ToString();
                        break;

                    case "IE8":
                        IE8_Other.Text = (Convert.ToInt32(IE8_Total.Text) - Convert.ToInt32(IE8_Pass.Text) -
                                Convert.ToInt32(IE8_Fail.Text)).ToString();
                        break;

                    case "IE9":
                        IE9_Other.Text = (Convert.ToInt32(IE9_Total.Text) - Convert.ToInt32(IE9_Pass.Text) -
                                Convert.ToInt32(IE9_Fail.Text)).ToString();
                        break;

                    case "IE10":
                        IE10_Other.Text = (Convert.ToInt32(IE10_Total.Text) - Convert.ToInt32(IE10_Pass.Text) -
                                Convert.ToInt32(IE10_Fail.Text)).ToString();
                        break;

                    case "IE11":
                        IE11_Other.Text = (Convert.ToInt32(IE11_Total.Text) - Convert.ToInt32(IE11_Pass.Text) -
                                Convert.ToInt32(IE11_Fail.Text)).ToString();
                        break;

                    case "MACSAF":
                        MACSAF_Other.Text = (Convert.ToInt32(MACSAF_Total.Text) - Convert.ToInt32(MACSAF_Pass.Text) -
                        Convert.ToInt32(MACSAF_Fail.Text)).ToString();
                        break;

                    case "WINSAF":
                        WINSAF_Other.Text = (Convert.ToInt32(WINSAF_Total.Text) - Convert.ToInt32(WINSAF_Pass.Text) -
                                Convert.ToInt32(WINSAF_Fail.Text)).ToString();
                        break;
                    default:
                        throw new Exception("Encountered an invalid browser type in visible browser list while trying to populate the Test Result Metrics table.");
                }
            }
            #endregion

            #region  Fill Totals column
            Totals_Pass.Text = (Convert.ToInt32(Chrome_Pass.Text) + Convert.ToInt32(Firefox_Pass.Text) +
                    Convert.ToInt32(IE8_Pass.Text) + Convert.ToInt32(IE9_Pass.Text) +
                    Convert.ToInt32(IE10_Pass.Text) + Convert.ToInt32(IE11_Pass.Text) +
                    Convert.ToInt32(MACSAF_Pass.Text) + Convert.ToInt32(WINSAF_Pass.Text)).ToString();
            Totals_Fail.Text = (Convert.ToInt32(Chrome_Fail.Text) + Convert.ToInt32(Firefox_Fail.Text) +
                    Convert.ToInt32(IE8_Fail.Text) + Convert.ToInt32(IE9_Fail.Text) +
                    Convert.ToInt32(IE10_Fail.Text) + Convert.ToInt32(IE11_Fail.Text) +
                    Convert.ToInt32(MACSAF_Fail.Text) + Convert.ToInt32(WINSAF_Fail.Text)).ToString();
            Totals_Other.Text = (Convert.ToInt32(Chrome_Other.Text) + Convert.ToInt32(Firefox_Other.Text) +
                    Convert.ToInt32(IE8_Other.Text) + Convert.ToInt32(IE9_Other.Text) +
                    Convert.ToInt32(IE10_Other.Text) + Convert.ToInt32(IE11_Other.Text) +
                    Convert.ToInt32(MACSAF_Other.Text) + Convert.ToInt32(WINSAF_Other.Text)).ToString();

            int totalother = 0;
            foreach (string browser in visibleBrowsers2)
            {
                switch (browser)
                {
                    case "CHROME":
                        totalother = Convert.ToInt32(Chrome_Total.Text);
                        break;

                    case "FF":
                        totalother += Convert.ToInt32(Firefox_Total.Text);
                        break;

                    case "IE8":
                        totalother += Convert.ToInt32(IE8_Total.Text);
                        break;

                    case "IE9":
                        totalother += Convert.ToInt32(IE9_Total.Text);
                        break;

                    case "IE10":
                        totalother += Convert.ToInt32(IE10_Total.Text);
                        break;

                    case "IE11":
                        totalother += Convert.ToInt32(IE11_Total.Text);
                        break;

                    case "MACSAF":
                        totalother += Convert.ToInt32(MACSAF_Total.Text);
                        break;

                    case "WINSAF":
                        totalother += Convert.ToInt32(WINSAF_Total.Text);
                        break;

                    default:
                        throw new Exception("Encountered an invalid browser type in visible browser list while trying to populate the Test Result Metrics table.");
                }
            }
            totalother = totalother - Convert.ToInt32(Totals_Pass.Text) - Convert.ToInt32(Totals_Fail.Text);
            Totals_Other.Text = totalother.ToString();

            int totaltotal = Convert.ToInt32(Totals_Pass.Text) + Convert.ToInt32(Totals_Fail.Text) + Convert.ToInt32(Totals_Other.Text);
            Totals_Total.Text = totaltotal.ToString();
            
            #endregion

            #region Set Browser Visbility for Test Results Table

            //Get a list of browser abbreviations that should be visible
            string[] visibleBrowsers = CTMethods.GetVisibleBrowserAbbreviations(ddlProjects.SelectedValue);

            //Loop through the rows of the metrics table
            foreach (TableRow row in TestResultMetrics.Rows)
            {
                String id = row.ID.ToString();
                string browserAbbreviation = id.Split('_').Last();

                if (!visibleBrowsers.Contains(browserAbbreviation) && !browserAbbreviation.Equals("Header") && !browserAbbreviation.Equals("Totals"))
                {
                    row.Visible = false;
                }
            }
            #endregion

        }
            public void RefreshLabels()
        {
            projectManualCasesWritten.Text = ManualCasesWrittenByProject().ToString();
            projectAutomatedCasesWritten.Text = AutomatedCasesWrittenByProject().ToString();

            userManualCasesWritten.Text = ManualCasesWrittenByUser().ToString();
            userAutomatedCasesWritten.Text = AutomatedCasesWrittenByUser().ToString();

            projectManualCasesExecuted.Text = ManualCasesExecutedByProject().ToString();
            projectAutomatedCasesExecuted.Text = AutomatedCasesExecutedByProject().ToString();

            userManualCasesExecuted.Text = ManualCasesExecutedByUser().ToString();
            userAutomatedCasesExecuted.Text = AutomatedCasesExecutedByUser().ToString();

            browserManualCasesExecuted.Text = ManualCasesExecutedByBrowser().ToString();
            browserAutomatedCasesExecuted.Text = AutomatedCasesExecutedByBrowser().ToString();

            projectManualDefectsDiscovered.Text = ManualDefectsDiscoveredByProject().ToString();
            projectAutomatedDefectsDiscovered.Text = AutomatedDefectsDiscoveredByProject().ToString();

            userManualDefectsDiscovered.Text = ManualDefectsDiscoveredByUser().ToString();
            userAutomatedDefectsDiscovered.Text = AutomatedDefectsDiscoveredByUser().ToString();

            browserManualDefectsDiscovered.Text = ManualDefectsDiscoveredByBrowser().ToString();
            browserAutomatedDefectsDiscovered.Text = AutomatedDefectsDiscoveredByBrowser().ToString();
        }

        public int ManualCasesWrittenByProject()
        {
            string query = " select count(*) from "
            + " ( "
            + " SELECT "
            + " distinct TestCases.projectabbreviation, TestCases.testcaseid "
            + " FROM TestCases"
            + " left JOIN dbo.GroupTestCases ON dbo.GroupTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.GroupTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.ReleaseTestCases ON dbo.ReleaseTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.ReleaseTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.SprintTestCases ON dbo.SprintTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.SprintTestCases.testCaseId = dbo.TestCases.testCaseId "
            + GetTestCaseLevelFilterForLabels("dateCreated")
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int AutomatedCasesWrittenByProject()
        {
            string query = " select count(*) from "
            + " ( "
            + " SELECT "
            + " distinct TestCases.projectabbreviation, TestCases.testcaseid "
            + " FROM TestCases"
            + " left JOIN dbo.GroupTestCases ON dbo.GroupTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.GroupTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.ReleaseTestCases ON dbo.ReleaseTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.ReleaseTestCases.testCaseId = dbo.TestCases.testCaseId "
            + " left JOIN dbo.SprintTestCases ON dbo.SprintTestCases.projectAbbreviation = dbo.TestCases.projectAbbreviation AND dbo.SprintTestCases.testCaseId = dbo.TestCases.testCaseId "
            + GetTestCaseLevelFilterForLabels("dateAutomatedTestCreated")
            + " and autotestclass is not null "
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int ManualCasesExecutedByProject()
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and TestResults.testtype = 'Manual' "
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int AutomatedCasesExecutedByProject()
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and TestResults.testtype = 'Automated' "
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int ManualDefectsDiscoveredByProject()
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and TestResults.testtype = 'Manual' "
            + " ) innerQuery "
            + " where defectTicketNumber is not null and defectTicketNumber != ''";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int AutomatedDefectsDiscoveredByProject()
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and TestResults.testtype = 'Automated' "
            + " ) innerQuery "
            + " where defectTicketNumber is not null and defectTicketNumber != ''";

            return DatabaseUtilities.GetIntFromQuery(query);
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
            + GetTestCaseLevelFilterForLabels("dateCreated")
            + " and createdBy = '" + ddlUsers.SelectedValue + "'"
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
            + GetTestCaseLevelFilterForLabels("dateAutomatedTestCreated")
            + " and createdBy = '" + ddlUsers.SelectedValue + "'"
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and TestResults.testtype = 'Manual' "
            + " and testedBy = '" + ddlUsers.SelectedValue + "'"
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and TestResults.testtype = 'Automated' "
            + " and testedBy = '" + ddlUsers.SelectedValue + "'"
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and testedBy = '" + ddlUsers.SelectedValue + "'"
            + " and TestResults.testtype = 'Manual' "
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and testedBy = '" + ddlUsers.SelectedValue + "'"
            + " and TestResults.testtype = 'Automated' "
            + " ) innerQuery "
            + " where defectTicketNumber is not null and defectTicketNumber != ''";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int ManualCasesExecutedByBrowser()
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and TestResults.testtype = 'Manual' "
            + " and browserAbbreviation = '" + ddlBrowsers.SelectedValue + "'"
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int AutomatedCasesExecutedByBrowser()
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and TestResults.testtype = 'Automated' "
            + " and browserAbbreviation = '" + ddlBrowsers.SelectedValue + "'"
            + " ) innerQuery ";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int ManualDefectsDiscoveredByBrowser()
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and browserAbbreviation = '" + ddlBrowsers.SelectedValue + "'"
            + " and TestResults.testtype = 'Manual' "
            + " ) innerQuery "
            + " where defectTicketNumber is not null and defectTicketNumber != ''";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public int AutomatedDefectsDiscoveredByBrowser()
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
            + GetTestResultLevelFilterForLabels("testDate")
            + " and browserAbbreviation = '" + ddlBrowsers.SelectedValue + "'"
            + " and TestResults.testtype = 'Automated' "
            + " ) innerQuery "
            + " where defectTicketNumber is not null and defectTicketNumber != ''";

            return DatabaseUtilities.GetIntFromQuery(query);
        }

        public string GetTestResultLevelFilterForLabels(string fieldDateAppliesTo)
        {
            string filter = GetTestCaseLevelFilterForLabels(fieldDateAppliesTo);

            filter += " AND ([environment] = '" + DatabaseUtilities.MakeSQLSafe(ddlEnvironment.SelectedValue) + "')";

            return filter;
        }

        public string GetTestCaseLevelFilterForLabels(string fieldDateAppliesTo)
        {
            string filter;

            filter = " WHERE (TestCases.[projectAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' OR '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' = '')";

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


            if (AreDatesValid())
            {
                DateTime startDate;
                bool startDateParsedSuccessfully = DateTime.TryParse(txtStartDate.Text, out startDate);

                DateTime endDate;
                bool endDateParsedSuccessfully = DateTime.TryParse(txtEndDate.Text, out endDate);

                if (!startDateParsedSuccessfully)
                {
                    txtStartDate.Text = "";
                }

                if (!endDateParsedSuccessfully)
                {
                    txtEndDate.Text = "";
                }

                if (startDateParsedSuccessfully && endDateParsedSuccessfully)
                {
                    filter += " and " + TrimDateToMidnight(fieldDateAppliesTo) + "  >= '" + startDate.ToSqlString() + "' and " + TrimDateToMidnight(fieldDateAppliesTo) + "  <= '" + endDate.ToSqlString() + "' ";
                }

                if (startDateParsedSuccessfully)
                {
                    filter += " and " + TrimDateToMidnight(fieldDateAppliesTo) + "  >= '" + startDate.ToSqlString() + "' ";
                }

                if (endDateParsedSuccessfully)
                {
                    filter += " and " + TrimDateToMidnight(fieldDateAppliesTo) + "  <= '" + endDate.ToSqlString() + "' ";
                }
            }


            return filter;
        }

        public string TrimDateToMidnight(string dateFieldName)
        {
            return "DATEADD(dd, DATEDIFF(dd,0," + dateFieldName + "), 0)";
        }

        public string GetTestCaseFilter(bool usePrefixes)
        {
            string filter;

            if (usePrefixes)
            {
                filter = " WHERE (Environments.[Environment] = '" + DatabaseUtilities.MakeSQLSafe(ddlEnvironment.SelectedValue) + "')";
            }
            else
            {
                filter = " WHERE ([Environment] = '" + DatabaseUtilities.MakeSQLSafe(ddlEnvironment.SelectedValue) + "')";
            }  
            

            if (ddlProjects.SelectedValue != "")
            {
                if (usePrefixes)
                {
                    filter += " AND (TestCases.[projectAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' OR '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' = '')";
                }
                else
                {
                    filter += " AND ([projectAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' OR '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "' = '')";
                }                
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
            return filter;
        }

        public string GetTestResultFilter(bool useWhere)
        {
            if (AreDatesValid())
            {
                DateTime startDate;
                bool startDateParsedSuccessfully = DateTime.TryParse(txtStartDate.Text, out startDate);

                DateTime endDate;
                bool endDateParsedSuccessfully = DateTime.TryParse(txtEndDate.Text, out endDate);

                if (!startDateParsedSuccessfully)
                {
                    txtStartDate.Text = "";
                }

                if (!endDateParsedSuccessfully)
                {
                    txtEndDate.Text = "";
                }

                if (startDateParsedSuccessfully && endDateParsedSuccessfully)
                {
                    return " " + (useWhere ? "where" : "and") + " " + TrimDateToMidnight("testDate") + " >= '" + startDate.ToSqlString() + "' and " + TrimDateToMidnight("testDate") + " <= '" + endDate.ToSqlString() + "' ";
                }

                if (startDateParsedSuccessfully)
                {
                    return " " + (useWhere ? "where" : "and") + " " + TrimDateToMidnight("testDate") + " >= '" + startDate.ToSqlString() + "' ";
                }

                if (endDateParsedSuccessfully)
                {
                    return " " + (useWhere ? "where" : "and") + " " + TrimDateToMidnight("testDate") + " <= '" + endDate.ToSqlString() + "' ";
                }
            }

            return "";
        }

        protected void gvFailingTestCasesByProject_PageIndexChanged(object sender, EventArgs e)
        {
            FiltersChanged();
        }

        
        protected void ddlProject_DataBound(object sender, EventArgs e)
        {
            string defaultProject;
            if (HttpContext.Current.Session["CurrentProject"] != null)
            {
                defaultProject = HttpContext.Current.Session["CurrentProject"].ToString();
            }
            else
            {
                // Get logged in user's default project
                defaultProject = CTMethods.GetDefaultProject(HttpContext.Current.User.Identity);
            }

            //Initialize gridviews and dropdown boxes.
            if (ddlProjects.Items.Count > 0)
            {
                ddlProjects.Items.Insert(0, "All Projects");
                ddlProjects.Items[0].Value = "";
                ddlProjects.SelectedIndex = 0;
            }

            if (defaultProject != null)
            {
                ddlProjects.SelectedValue = defaultProject;
            }

            ddlProjectsValueChange();
        }

        protected void ddlEnvironment_DataBound(object sender, EventArgs e)
        {

            //Initialize gridviews and dropdown boxes.
            if (ddlEnvironment.Items.Count > 0)
            {
                ddlEnvironment.SelectedValue = "QA";
            }
            
            ddlEnvironmentValueChange();
         
        }

        protected void ddlEnvironmentValueChange()
        {
            if (ddlProjects.SelectedValue == "")
            {
                //gvFailingTestCasesByProject.Visible = false;
            }
        }
        
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

            Response.Redirect("~/Default.aspx");
        }


        protected void gvFailingTestCasesByProject_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RunTest")
            {
                string commandArgument = (e.CommandArgument ?? "").ToString();
                string[] commandPieces = commandArgument.Split(new[] { '|' });

                string projectAbbreviation = commandPieces[0];
                int testCaseID = Convert.ToInt32(commandPieces[1]);

                //string url = (string)e.CommandArgument;
                string url = "~/FunctionalTesting/AddEditResults.aspx?project=" + projectAbbreviation + "&testCase=" + testCaseID;
                Response.Redirect(url);
                return;
            }
            FiltersChanged();
        }
       
        protected void imgStartDateSelection_click(object sender, EventArgs e)
        {
            Calendar calStartDate = (Calendar)AspUtilities.FindControlRecursive(this, "calStartDate");
            calStartDate.Visible = !calStartDate.Visible;
        }

        protected void calStartDate_SelectionChanged(object sender, EventArgs e)
        {
            txtStartDate.Text = calStartDate.SelectedDate.ToString("d");

            calStartDate.Visible = false;

            AreDatesValid();

            HttpContext.Current.Session["StartDate"] = txtStartDate.Text;

            FiltersChanged();
        }

        protected bool AreDatesValid()
        {
            DateTime startDate;
            bool startDateParsedSuccessfully = DateTime.TryParse(txtStartDate.Text, out startDate);

            DateTime endDate;
            bool endDateParsedSuccessfully = DateTime.TryParse(txtEndDate.Text, out endDate);

            lblCalendarError.Visible = false;

            if (startDateParsedSuccessfully && endDateParsedSuccessfully)
            {
                if (endDate < startDate)
                {
                    lblCalendarError.Visible = true;
                    return false;
                }
            }

            return true;
        }

        protected void imgEndDateSelection_click(object sender, EventArgs e)
        {
            Calendar calEndDate = (Calendar)AspUtilities.FindControlRecursive(this, "calEndDate");
            calEndDate.Visible = !calEndDate.Visible;
        }

        protected void calEndDate_SelectionChanged(object sender, EventArgs e)
        {
            txtEndDate.Text = calEndDate.SelectedDate.ToString("d");

            calEndDate.Visible = false;

            AreDatesValid();

            HttpContext.Current.Session["EndDate"] = txtEndDate.Text;

            FiltersChanged();
        }

        private void populateAutomatedMetricsTable()
        {

            String project = ddlProjects.SelectedValue;
            int Automated_Future = populateSingleAutomatedMetric("Select Count (*) FROM TestCases WHERE projectAbbreviation = '" + project + "' AND automated = 'Future'", "Automated_Future");
            int Automated_No = populateSingleAutomatedMetric("Select Count (*) FROM TestCases WHERE projectAbbreviation = '" + project + "' AND automated = 'No'", "Automated_No");
            int Automated_Yes = populateSingleAutomatedMetric("Select Count (*) FROM TestCases WHERE projectAbbreviation = '" + project + "' AND automated = 'Yes'", "Automated_Yes");
            int Automated_Child = populateSingleAutomatedMetric("Select Count (*) FROM TestCases WHERE projectAbbreviation = '" + project + "' AND automated = 'Child'", "Automated_Child");
            int Automated_Blank = populateSingleAutomatedMetric("Select Count (*) FROM TestCases WHERE (projectAbbreviation = '" + project + "') AND (automated IS Null or automated not in ('Future', 'No', 'Yes', 'Child'))", "Automated_Blank");
            int Automated_Total = populateSingleAutomatedMetric("Select Count (*) FROM TestCases WHERE (projectAbbreviation = '" + project + "') AND (automated in ('Yes', 'Child'))", "Automated_Total");
            int Manual_Total = populateSingleAutomatedMetric("Select Count (*) FROM TestCases WHERE (projectAbbreviation = '" + project + "') AND (automated IS Null or automated not in ('Yes', 'Child'))", "Manual_Total");
            int TestCases_Total = populateSingleAutomatedMetric("Select Count (*) FROM TestCases WHERE (projectAbbreviation = '" + project + "')", "TestCases_Total");

            int Current_Percentage;
            if (TestCases_Total != 0)
            {
               Current_Percentage = (Automated_Total * 100) / TestCases_Total;
            }
            else
            {
                Current_Percentage = 0;
            }
            Label Current_Percentage_Label = (Label)AspUtilities.FindControlRecursive(this, "Current_Percentage");
            Current_Percentage_Label.Text = Current_Percentage.ToString() + "%";


            int Possible_Coverage;
            if (TestCases_Total != 0)
            {
                Possible_Coverage = ((Automated_Future + Automated_Yes + Automated_Child) * 100) / TestCases_Total;
            }
            else
            {
                Possible_Coverage = 0;
            }
            Label Possible_Coverage_Label = (Label)AspUtilities.FindControlRecursive(this, "Possible_Coverage");
            Possible_Coverage_Label.Text = Possible_Coverage.ToString() + "%";



            int Possible_Completed;
            if ((Automated_Future + Automated_Yes + Automated_Child) != 0)
            {
                Possible_Completed = ((Automated_Yes + Automated_Child) * 100) / (Automated_Future + Automated_Yes + Automated_Child);
            }
            else
            {
                Possible_Completed = 0;
            }
            Label Possible_Completed_Label = (Label)AspUtilities.FindControlRecursive(this, "Possible_Completed");
            Possible_Completed_Label.Text = Possible_Completed.ToString() + "%";

        
        }

        private int populateSingleAutomatedMetric(String sqlQuery, String labelName)
        {
            int i = DatabaseUtilities.GetIntFromQuery(sqlQuery);
            Label label = (Label)AspUtilities.FindControlRecursive(this, labelName);
            label.Text = i.ToString();
            return i;
        }
    }
}

