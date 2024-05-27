using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using OSTMSInfrastructure;
using System.Data;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Web.UI.HtmlControls;

namespace OSTMSWebsite.Analytics
{
    public partial class TestCasesAnalytics : System.Web.UI.Page
    {
        #region Load/Init

        protected int[] gvTestCaseListStatusColumns
        {
            get
            {
                return new int[] { 5, 6, 7, 8, 9, 10, 11, 12 };
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
                userParam.DefaultValue = DatabaseUtilities.RemovePrefixFromUserName(User.Identity.Name);
                ods.SelectParameters.Add(userParam);
            }

            if (Page.IsPostBack)
            {
                HttpContext.Current.Session["Keyword"] = txtKeywordSearchTextBox.Text;
            }

            if (Page.IsPostBack == false)
            {
                ddlEnvironment.DataBind();

                if (HttpContext.Current.Session["CurrentEnvironment"] != null)
                {
                    ddlEnvironment.SelectedValue = HttpContext.Current.Session["CurrentEnvironment"].ToString();
                }
                else
                {
                    ddlEnvironment.SelectedValue = "QA";
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
                        string defaultProject = DatabaseUtilities.GetDefaultProject(HttpContext.Current.User.Identity);

                        if (defaultProject != null)
                        {
                            ddlProjects.SelectedValue = defaultProject;
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

            SetCheckBoxEnabledStatus();
        }

        #endregion

        #region Button Events

        #region Right Side Buttons
        protected void btnAddNewTestCase_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/Analytics/AddNewTestCase.aspx?project=" + tbProjectAbbreviationValue);
        }

        protected void btnUploadTestCases_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminTestCases.aspx");
        }

        protected void btnUploadTestResults_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminTestCases.aspx");
        }

        protected void lstbxBrowserSelection_DataBound(object sender, EventArgs e)
        {
            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

            for (int i = 0; i < lstbxBrowserSelection.Items.Count; i++)
            {
                lstbxBrowserSelection.Items[i].Selected = true;
            }
        }

        #endregion

        #region Left Side Drop Down Lists

        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            ddlProjectsValueChange();
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
            catch (Exception e)
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
            catch (Exception e)
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
            catch (Exception e)
            {
                HttpContext.Current.Session["CurrentSprint"] = null;
            }

            FiltersChanged();

            DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
            dropdown.Visible = false;

            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
            label.Visible = false;
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

            Response.Redirect("~/Analytics/TestCases.aspx");
        }

        protected void btnKeywordSearchSubmit_Click(object sender, ImageClickEventArgs e)
        {
            FiltersChanged();
        }

        protected void FiltersChanged()
        {
            gvTestCaseList.Caption = "Project: " + ddlProjects.SelectedItem.Text;

            if (ddlReleases.SelectedValue != "")
            {
                gvTestCaseList.Caption += "<br/>" + "Release: " + ddlReleases.SelectedValue;
            }

            if (ddlGroupTest.SelectedIndex > 0)
            {
                gvTestCaseList.Caption += "<br/>" + "Group: " + ddlGroupTest.SelectedItem.Text;
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

            SqlDataSource sqlTestCasesByProject = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlTestCasesByProject");

            sqlTestCasesByProject.SelectCommand = "select " +
                "c.projectAbbreviation, c.testCaseId, c.testCaseDescription, " +
                "c.testdate, " +
                "Coalesce(Browser1TestResults.status, 'Not Started') as Browser1Status, " +
                "Coalesce(Browser2TestResults.status, 'Not Started') as Browser2Status, " +
                "Coalesce(Browser3TestResults.status, 'Not Started') as Browser3Status, " +
                "Coalesce(Browser4TestResults.status, 'Not Started') as Browser4Status, " +
                "Coalesce(Browser5TestResults.status, 'Not Started') as Browser5Status, " +
                "Coalesce(Browser6TestResults.status, 'Not Started') as Browser6Status, " +
                "Coalesce(Browser7TestResults.status, 'Not Started') as Browser7Status, " +
                "Coalesce(Browser8TestResults.status, 'Not Started') as Browser8Status " +
                "from Analytics_LatestTestRunIDPerBrowser c  " +
                "left outer join releaseTestCases rt " +
                "on c.projectAbbreviation = rt.projectAbbreviation and " +
                "c.testCaseId = rt.testCaseId " +
                "left outer join sprintTestCases s " +
                "on c.projectAbbreviation = s.projectAbbreviation and " +
                "c.testCaseId = s.testCaseId " +
                "left outer join groupTestCases g " +
                "on c.projectAbbreviation = g.projectAbbreviation and " +
                "c.testCaseId = g.testCaseId " +
                "left join TestResults Browser1TestResults " +
                    "on c.Browser1TestRunID = Browser1TestResults.testRunId " +
                "left join TestResults Browser2TestResults " +
                    "on c.Browser2TestRunID = Browser2TestResults.testRunId " +
                "left join TestResults Browser3TestResults " +
                    "on c.Browser3TestRunID = Browser3TestResults.testRunId " +
                "left join TestResults Browser4TestResults " +
                    "on c.Browser4TestRunID = Browser4TestResults.testRunId " +
                "left join TestResults Browser5TestResults " +
                    "on c.Browser5TestRunID = Browser5TestResults.testRunId " +
                "left join TestResults Browser6TestResults " +
                    "on c.Browser6TestRunID = Browser6TestResults.testRunId " +
                "left join TestResults Browser7TestResults " +
                    "on c.Browser7TestRunID = Browser7TestResults.testRunId " +
                "left join TestResults Browser8TestResults " +
                    "on c.Browser8TestRunID = Browser8TestResults.testRunId ";

            sqlTestCasesByProject.SelectCommand += GetFilterWithPrefixes();

            SetCheckBoxEnabledStatus();
        }

        public string GetFilterWithPrefixes()
        {
            string filter = " WHERE (c.[environment] = '" + DatabaseUtilities.MakeSQLSafe(ddlEnvironment.SelectedValue) + "'";
            filter += " or c.[environment] is null)";

            if (ddlProjects.SelectedValue != "")
            {
                filter += " AND (c.[projectAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "')";
            }

            if (ddlGroupTest.SelectedValue != "")
            {
                filter += " AND (g.[groupTestAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlGroupTest.SelectedValue) + "')";
            }

            if (ddlReleases.SelectedValue != "")
            {
                filter += " AND ([release] = '" + DatabaseUtilities.MakeSQLSafe(ddlReleases.SelectedValue) + "')";
            }

            if (ddlSprints.SelectedValue != "")
            {
                filter += " AND ([sprint] = '" + DatabaseUtilities.MakeSQLSafe(ddlSprints.SelectedValue) + "')";
            }

            TextBox txtKeywordSearchTextBox = (TextBox)AspUtilities.FindControlRecursive(this, "txtKeywordSearchTextBox");
            string searchText = txtKeywordSearchTextBox.Text;

            if (!String.IsNullOrEmpty(searchText))
            {
                List<string> splitSearchValues = searchText.Split(new[] { ' ' }).ToList();

                foreach (string splitSearchValue in splitSearchValues)
                {
                    filter += " AND (testCaseDescription like '%" + DatabaseUtilities.MakeSQLSafe(splitSearchValue) + "%')";
                }
            }

            return filter;
        }

        public string GetFilter()
        {
            string filter = " WHERE ([Environment] = '" + DatabaseUtilities.MakeSQLSafe(ddlEnvironment.SelectedValue) + "')";

            if (ddlProjects.SelectedValue != "")
            {
                filter += " AND (c.[projectAbbreviation] = '" + DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue) + "')";
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

            TextBox txtKeywordSearchTextBox = (TextBox)AspUtilities.FindControlRecursive(this, "txtKeywordSearchTextBox");
            string searchText = txtKeywordSearchTextBox.Text;

            if (!String.IsNullOrEmpty(searchText))
            {
                List<string> splitSearchValues = searchText.Split(new[] { ' ' }).ToList();

                foreach (string splitSearchValue in splitSearchValues)
                {
                    filter += " AND (testCaseDescription like '%" + DatabaseUtilities.MakeSQLSafe(splitSearchValue) + "%')";
                }
            }

            return filter;
        }

        #endregion

        #region Middle Buttons

        protected void AssignTestCase(object sender, ImageClickEventArgs e)
        {
            Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
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

        protected void AddToGroup(object sender, ImageClickEventArgs e)
        {
            if (ddlProjects.SelectedValue == "")
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Please select a project first";
                label.Visible = true;
            }
            else
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Visible = false;

                DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
                dropdown.Visible = true;

                SqlDataSource data = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlAddToaBunchOfThings");
                data.SelectCommand = "select groupTestAbbreviation, groupTestName from GroupTests where ([projectAbbreviation] = '" + ddlProjects.SelectedValue + "') and (personalGroupOwner is null or personalGroupOwner = '" + DatabaseUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) + "') order by groupTestName";

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

        protected void AddToRelease(object sender, ImageClickEventArgs e)
        {
            if (ddlProjects.SelectedValue == "")
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Please select a project first";
                label.Visible = true;
            }
            else
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
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

        protected void AddToSprint(object sender, ImageClickEventArgs e)
        {
            if (ddlProjects.SelectedValue == "")
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Please select a project first";
                label.Visible = true;
            }
            else
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
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

        protected void BulkDelete(object sender, ImageClickEventArgs e)
        {
            List<int> testCaseList = GetFilteredTestCaseIDs();

            foreach (int testCaseId in testCaseList)
            {
                DatabaseUtilities.RemoveTestCase(ddlProjects.SelectedValue, testCaseId.ToString(), User.Identity);
            }

            if (testCaseList.Count == 0)
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Please select one or more test cases first";
                label.Visible = true;
            }
            else
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = testCaseList.Count.ToString() + " " + (testCaseList.Count > 1 ? "Test Cases have" : "Test Case has") + " been deleted";
                label.Visible = true;

                FiltersChanged();
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

        protected void ddlAddToaBunchOfThings_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList dropdown = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlAddToaBunchOfThings");
            dropdown.Visible = false;

            List<int> testCaseList = GetFilteredTestCaseIDs();

            foreach (int testCaseId in testCaseList)
            {
                switch (HttpContext.Current.Session["TypeOfThingBeingAdded"].ToString())
                {
                    case "Group":
                        DatabaseUtilities.AddSingleGroupIfNeeded(Convert.ToInt32(testCaseId), ddlProjects.SelectedValue, dropdown.SelectedValue);
                        break;
                    case "Release":
                        DatabaseUtilities.AddSingleReleaseIfNeeded(Convert.ToInt32(testCaseId), ddlProjects.SelectedValue, dropdown.SelectedValue);
                        break;
                    case "Sprint":
                        DatabaseUtilities.AddSingleSprintIfNeeded(Convert.ToInt32(testCaseId), ddlProjects.SelectedValue, dropdown.SelectedValue);
                        break;
                    case "User":
                        DatabaseUtilities.AssignSingleUserIfNeeded(Convert.ToInt32(testCaseId), ddlProjects.SelectedValue, dropdown.SelectedValue);
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

        public List<int> GetFilteredTestCaseIDs()
        {
            CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");

            if (chk.Checked)
            {
                string filter = GetFilter();
                return DatabaseUtilities.GetIntListFromQuery("Select distinct TestCaseID from TestCaseFilterableView c " + filter);
            }
            else
            {
                List<int> returnList = new List<int>();

                for (int i = 0; i < gvTestCaseList.Rows.Count; i++)
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

        protected void AddToAutomatedQueue(object sender, ImageClickEventArgs e)
        {
            List<int> testCaseList = GetFilteredTestCaseIDs();

            DateTime startTime = DatabaseUtilities.GetEstimatedTimeCurrentlyQueuedAutomatedTestsWillFinish();
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
            DateTime endTime = DatabaseUtilities.GetEstimatedTimeCurrentlyQueuedAutomatedTestsWillFinish();

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

        protected void RemoveFromAutomationQueue(object sender, ImageClickEventArgs e)
        {
            List<int> testCaseList = GetFilteredTestCaseIDs();
            foreach (int testCaseId in testCaseList)
            {
                string commandArgument = GetCommandArgument(testCaseId);
                
                string[] commandPieces = commandArgument.Split(new[] { '|' });

                string projectAbbreviation = commandPieces[0];
                int testCaseID = Convert.ToInt32(commandPieces[1]);
                string environment = commandPieces[4];

                DatabaseUtilities.RemoveFromQueue(projectAbbreviation, testCaseID.ToString(), environment);
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

        #endregion

        #endregion

        #region Grid Events

        protected void sqlTestCasesByProject_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            Label lblRecordCount = (Label)AspUtilities.FindControlRecursive(this, "lblRecordCount");

            if (lblRecordCount != null)
            {
                string recordCount = e.AffectedRows.ToString();
                lblRecordCount.Text = "Test Cases Found: " + recordCount;
            }
        }

        protected void gvTestCaseList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string commandArgument = (e.CommandArgument ?? "").ToString();
            string[] commandPieces = commandArgument.Split(new[] { '|' });

            string projectAbbreviation = commandPieces[0];
            int testCaseID = Convert.ToInt32(commandPieces[1]);

            string project = DatabaseUtilities.MakeSQLSafe(ddlProjects.SelectedValue);
            List<string> selectedBrowsers = new List<string>();
            int[] selectedItemsIndexes = lstbxBrowserSelection.GetSelectedIndices();
            foreach (int selectedItem in selectedItemsIndexes)
            {
                selectedBrowsers.Add(lstbxBrowserSelection.Items[selectedItem].Value);
            }

            System.Security.Principal.IIdentity identity = HttpContext.Current.User.Identity;
            SeleniumTestBase.QueueTestCase(ddlEnvironment.SelectedValue, projectAbbreviation, testCaseID, DatabaseUtilities.RemovePrefixFromUserName(identity.Name), selectedBrowsers);


            //HttpContext.Current.Session["CurrentMessage"] = "Your automated test has been added to the testing queue. You will be emailed the results when your test is completed."; // The estimated time your tests will start is " + String.Format("{0:G}", startTime) + ", and the estimated time your tests will end is " + String.Format("{0:G}", endTime);

            //Response.Redirect(Request.RawUrl);
            //return;
            // Force a refresh of the screen (to see the new status in the GridView)
            FiltersChanged();
        }

        protected void gvTestCaseList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
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

        protected void gvTestCaseList_PageIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["PageIndex"] = gvTestCaseList.PageIndex;

            FiltersChanged();
        }

        protected void chkHeader_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)gvTestCaseList.HeaderRow.FindControl("chkHeader");

            for (int i = 0; i < gvTestCaseList.Rows.Count; i++)
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
                    for (int i = 0; i < gvTestCaseList.Rows.Count; i++)
                    {
                        CheckBox chkrow = (CheckBox)gvTestCaseList.Rows[i].FindControl("chkSelected");
                        chkrow.Enabled = !chk.Checked;
                    }
                }
            }
        }

        #endregion
     
        public string GetCommandArgument(int testCaseId)
        {
            string description;
            Type autoTestClass;
            int? autoMetaDataRow;
            string autoTestClassType;
            string automated;
            string reasonForNotAutomated;
            DatabaseUtilities.GetTestCaseAutomationParameters(ddlProjects.SelectedValue, testCaseId, out description, out autoTestClassType, out autoTestClass, out autoMetaDataRow, out automated, out reasonForNotAutomated);
            return String.Format("{0}|{1}|{2}|{3}|{4}", ddlProjects.SelectedValue, testCaseId, autoTestClassType, autoMetaDataRow, ddlEnvironment.SelectedValue);
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

            string environment = commandPieces[4];

            List<string> selectedBrowsers = new List<string>();
            int[] selectedItemsIndexes = lstbxBrowserSelection.GetSelectedIndices();
            foreach (int selectedItem in selectedItemsIndexes)
            {
                selectedBrowsers.Add(lstbxBrowserSelection.Items[selectedItem].Value);
            }

            System.Security.Principal.IIdentity identity = HttpContext.Current.User.Identity;

            SeleniumTestBase.QueueTestCase(environment, projectAbbreviation, testCaseID, DatabaseUtilities.RemovePrefixFromUserName(identity.Name), selectedBrowsers);

            return true;
        }

        protected void ExportToExcel(object sender, ImageClickEventArgs e)
        {
            List<int> testCaseList = GetFilteredTestCaseIDs();

            string testCases = String.Join(", ", testCaseList);

            if (testCaseList.Count == 0)
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Please select one or more test cases first";
                label.Visible = true;
            }
            else
            {
                Label label = (Label)AspUtilities.FindControlRecursive(this, "lblPleaseSelectAProject");
                label.Text = "Completed";
                label.Visible = true;

                FiltersChanged();

                string query = "select distinct projectAbbreviation, testCaseId, '' as groupTestAbbreviation, '' as release, '' as sprint, rawTestCaseDescription as testCaseDescription, rawTestCaseSteps as testCaseSteps, rawExpectedResults as expectedResults, '' as screenshots, '' as screenshotDescriptions, isnull(rawTestCaseNotes,'') as testCaseNotes, isnull(rawUpdateSprint,'') as updateSprint, isnull(rawUpdateStory,'') as updateStory, 'Y' as [isThisAnUpdate?]  from TestCaseFilterableView c "
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

                string unmappedFile = "~/Admin/ExportFiles/" + DatabaseUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) + "Export.xls";
                string file = Server.MapPath(unmappedFile);

                ExcelWriter.GenerateXLS(file, "TestCases", results);

                Response.Redirect(unmappedFile);
            }
        }
    }
}

