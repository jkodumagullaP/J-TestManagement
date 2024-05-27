using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;

namespace OSTMSWebsite.Analytics
{
    public partial class ResultHistory : System.Web.UI.Page
    {

        protected int gvResultHistoryStatusColumn
        {
            get
            {
                return 5;
            }
        }

        protected int gvResultHistoryReasonForStatusColumn
        {
            get
            {
                return 3;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                String tbProjectAbbreviationValue = Request.QueryString["project"];
                String tbTestCaseIdValue = Request.QueryString["testCase"];
                gvResultHistory.Caption = "Result History for " + tbProjectAbbreviationValue + "-" + tbTestCaseIdValue;

                ddlEnvironments.DataBind();

                if (ddlEnvironments.Items.Count == 0
                    || ddlEnvironments.Items[0].Value != "")
                {
                    ddlEnvironments.Items.Insert(0, "All");
                    ddlEnvironments.Items[0].Value = "";
                }

                if (HttpContext.Current.Session["CurrentEnvironment"] != null)
                {
                    ddlEnvironments.SelectedValue = HttpContext.Current.Session["CurrentEnvironment"].ToString();
                }
                else
                {
                    ddlEnvironments.SelectedValue = "QA";
                }

                ddlBrowsers.DataBind();

                if (ddlBrowsers.Items.Count == 0
                    || ddlBrowsers.Items[0].Value != "")
                {
                    ddlBrowsers.Items.Insert(0, "All");
                    ddlBrowsers.Items[0].Value = "";
                }

                FiltersChanged();
            }


        }

        protected void FiltersChanged()
        {
            SqlDataSource sqlSingleTestCaseHistory = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlSingleTestCaseHistory");
            sqlSingleTestCaseHistory.SelectCommand = "SELECT  " // TestResultsView.*,

            + " [testRunId] "
            + " ,[testCaseId] "
            + " ,[projectAbbreviation] "
            + " ,[environment] "
            + " ,[browserAbbreviation] "
            + " ,[status] "
            + " ,reasonForStatus "
            + " ,reasonForStatusDetailed "
            + " ,stepsToReproduce "
            + " ,[defectTicketNumber] "
            + " ,[testDate] "
            + " ,[testedBy] "
            + " ,[testType] "
            + " ,rawReasonForStatus "
            + " ,rawReasonForStatusDetailed "
            + " ,rawStepsToReproduce "
            + " , right('0' + rtrim(convert(char(2), elapsedSeconds / (60 * 60))), 2) + ':' + right('0' + rtrim(convert(char(2), (elapsedSeconds / 60) % 60)), 2) + ':' + right('0' + rtrim(convert(char(2), elapsedSeconds % 60)),2) as [elapsedSeconds], "
            + " dbo.UserProfiles.userFirstName + ' ' + dbo.UserProfiles.userLastName as TestedByFullName, automationNode FROM [TestResultsView] left join dbo.aspnet_Users 	on dbo.TestResultsView.testedBy = aspnet_Users.UserName left join dbo.UserProfiles on aspnet_Users.userid = UserProfiles.userid WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([testCaseId] = @testCaseId)) ";

            if (ddlEnvironments.SelectedValue != "")
            {
                sqlSingleTestCaseHistory.SelectCommand += " and environment = '" + ddlEnvironments.SelectedValue + "' ";
            }

            if (ddlBrowsers.SelectedValue != "")
            {
                sqlSingleTestCaseHistory.SelectCommand += " and browserAbbreviation = '" + ddlBrowsers.SelectedValue + "' ";
            }

            sqlSingleTestCaseHistory.SelectCommand += " order by testDate desc";

            sqlSingleTestCaseHistory.DataBind();
        }
        
        protected void gvResultHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // DO NOT CHANGE THE COLUMN ORDER
                if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "Pass")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusPass";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "Pass";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
                }
                else if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "Fail")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusFail";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "Fail";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
                }
                else if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "Test Case Needs Updated")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusTestCaseNeedsUpdated";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "test Case Needs Updated";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
                }
                else if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "Automated Test Needs Updated")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusAutomatedTestNeedsUpdated";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "Automated Case Needs Updated";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
                }
                else if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "Not Started")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusNotStarted";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "Not Started";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
                }
                else if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "Not Implemented")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusNotImplemented";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "Not Implemented";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
                }
                else if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "Not Applicable")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusNotApplicable";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "Not Applicable";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
                }
                else if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "In Progress")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusInProgress";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "In Progress";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
                }
                else if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "In Queue")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusInQueue";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "In Queue";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
                }
                else if (e.Row.Cells[gvResultHistoryStatusColumn].Text == "Retest")
                {
                    e.Row.Cells[gvResultHistoryStatusColumn].CssClass = "statusRetest";
                    e.Row.Cells[gvResultHistoryStatusColumn].ToolTip = "Retest";
                    e.Row.Cells[gvResultHistoryStatusColumn].Text = "";
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
    }
}