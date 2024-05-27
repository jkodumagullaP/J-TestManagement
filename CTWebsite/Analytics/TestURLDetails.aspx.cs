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
    public partial class AnalyticsTestURLDetails : System.Web.UI.Page
    {
        protected global::System.Web.UI.WebControls.Label lblResultInsertIndicator;

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlNewURL.Visible = false;

            if (Request.QueryString.Count > 0)
            {
                lblURLHeader.Text = "URL List Details " + Request.QueryString["project"].ToString() + "-" + Request.QueryString["testCase"].ToString();
                lblExpectedResultsHeader.Text = "Expected Results " + Request.QueryString["project"].ToString() + "-" + Request.QueryString["testCase"].ToString();
                
                if (!IsPostBack)
                {
                    ddlEnvironment.DataSource = DatabaseUtilities.GetEnvironments();
                    ddlEnvironment.DataBind();
                    ddlEnvironment.SelectedValue = HttpContext.Current.Session["CurrentEnvironment"].ToString();
                    FiltersChanged();
                }
            }
        }


        #region URL Grid Events

        protected void gvURLList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
      
            }
        }

        protected void gvURLList_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvURLList.EditIndex = e.NewEditIndex;
            FiltersChanged();
        }

        protected void gvURLList_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvURLList.EditIndex = -1;
            FiltersChanged();
        }

        protected void gvURLList_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Label environment = (Label)gvURLList.Rows[e.RowIndex].Cells[2].FindControl("lblEnvironments");
            TextBox url = (TextBox)gvURLList.Rows[e.RowIndex].Cells[3].FindControl("txtURL");
            TextBox omni = (TextBox)gvURLList.Rows[e.RowIndex].Cells[4].FindControl("txtOmniture");

            string errorMessage = ValidateURLs(url.Text, omni.Text, true);

            if (errorMessage != "")
            {
                TopMessageLabel.Text = errorMessage;
            }
            else
            {
                try
                {
                    DatabaseUtilities.UpdateAnalyticsURL(Request.QueryString["project"].ToString(),
                        Convert.ToInt32(Request.QueryString["testCase"].ToString()),
                        environment.Text,
                        url.Text,
                        omni.Text);

                    gvURLList.EditIndex = -1;
                    FiltersChanged();
                    TopMessageLabel.Text = "";
                }
                catch (Exception ex)
                {
                    TopMessageLabel.Text = "Error updating the record:  " + ex.Message;
                }
            }
        }

        protected void gvURLList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                DatabaseUtilities.DeleteAnalyticsURL(Request.QueryString["project"].ToString(),
                        Convert.ToInt32(Request.QueryString["testCase"].ToString()),
                        e.Values[0].ToString());

                Response.Redirect("~/Analytics/TestURLDetails.aspx?project=" + Request.QueryString["project"] + "&testCase=" + Request.QueryString["testCase"]);
            }
            catch (Exception ex)
            {
                TopMessageLabel.Text = "Error deleting the record:  " + ex.Message;
            }
        }

        #endregion

        #region Expected Results Grid Events

        protected void gvExpectedResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }

        protected void gvExpectedResults_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvExpectedResults.EditIndex = e.NewEditIndex;
            FiltersChanged();
        }

        protected void gvExpectedResults_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvExpectedResults.EditIndex = -1;
            FiltersChanged();
        }

        protected void gvExpectedResults_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Label oldField = (Label)gvExpectedResults.Rows[e.RowIndex].Cells[5].FindControl("lblOldFieldName");
            TextBox fieldName = (TextBox)gvExpectedResults.Rows[e.RowIndex].Cells[2].FindControl("txtFieldName");
            TextBox searchString = (TextBox)gvExpectedResults.Rows[e.RowIndex].Cells[3].FindControl("txtSearchString");
            TextBox expectedValue = (TextBox)gvExpectedResults.Rows[e.RowIndex].Cells[4].FindControl("txtExpectedValue");

            string errorMessage = ValidateResults(fieldName.Text, searchString.Text, expectedValue.Text, true);

            if (errorMessage != "")
            {
                TopMessageLabel.Text = errorMessage;
            }
            else
            {
                try
                {
                    DatabaseUtilities.UpdateAnalyticsExpectedResult(Request.QueryString["project"].ToString(),
                        Convert.ToInt32(Request.QueryString["testCase"].ToString()),
                        ddlEnvironment.SelectedValue,
                        oldField.Text,
                        fieldName.Text,
                        searchString.Text,
                        expectedValue.Text);

                    gvExpectedResults.EditIndex = -1;
                    FiltersChanged();
                    TopMessageLabel.Text = "";
                }
                catch (Exception ex)
                {
                    TopMessageLabel.Text = "Error updating the record:  " + ex.Message;
                }
            }
        }

        protected void gvExpectedResults_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                DatabaseUtilities.DeleteAnalyticsExpectedResult(Request.QueryString["project"].ToString(),
                        Convert.ToInt32(Request.QueryString["testCase"].ToString()),
                        ddlEnvironment.SelectedValue,
                        e.Values[0].ToString());

                Response.Redirect("~/Analytics/TestURLDetails.aspx?project=" + Request.QueryString["project"] + "&testCase=" + Request.QueryString["testCase"]);
            }
            catch (Exception ex)
            {
                TopMessageLabel.Text = "Error deleting the record:  " + ex.Message;
            }
        }

        #endregion

        #region Add URL Section
        protected void btnAddURL_Click(object sender, EventArgs e)
        {
            pnlNewURL.Visible = true;
            txtURL.Focus();
            btnAddURL.Visible = false;
        }

        protected void lnkSubmit_Click(object sender, EventArgs e)
        {
            string errorMessage = ValidateURLs(txtURL.Text, txtOmnitureString.Text, false);

            if (errorMessage != "")
            {
                TopMessageLabel.Text = errorMessage;
                pnlNewURL.Visible = true;
                return;
            }

            try
            {
                DatabaseUtilities.InsertAnalyticsURL(Request.QueryString["project"].ToString(),
                        Convert.ToInt32(Request.QueryString["testCase"].ToString()),
                        ddlEnvironment.SelectedValue,
                        txtURL.Text,
                        txtOmnitureString.Text);

                Response.Redirect("~/Analytics/TestURLDetails.aspx?project=" + Request.QueryString["project"] + "&testCase=" + Request.QueryString["testCase"]);
            }
            catch (Exception ex)
            {
                TopMessageLabel.Text = "Error inserting record: " + ex.Message;
            }
        }

        protected void lnkCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Analytics/TestURLDetails.aspx?project=" + Request.QueryString["project"] + "&testCase=" + Request.QueryString["testCase"]);
        }
        #endregion

        #region Add Expected Results Section
        protected void btnAddExpectedResult_Click(object sender, EventArgs e)
        {
            pnlNewExpectedResult.Visible = true;
            txtFieldName.Focus();
            btnAddExpectedResult.Visible = false;
        }

        protected void lnkSubmitResult_Click(object sender, EventArgs e)
        {
            string errorMessage = ValidateResults(txtFieldName.Text, txtSearchString.Text, txtExpectedValue.Text, false);

            if (errorMessage != "")
            {
                TopMessageLabel.Text = errorMessage;
                pnlNewExpectedResult.Visible = true;
                return;
            }

            try
            {
                DatabaseUtilities.InsertAnalyticsExpectedResult(Request.QueryString["project"].ToString(),
                        Convert.ToInt32(Request.QueryString["testCase"].ToString()),
                        ddlEnvironment.SelectedValue,
                        txtFieldName.Text,
                        txtSearchString.Text,
                        txtExpectedValue.Text);

                Response.Redirect("~/Analytics/TestURLDetails.aspx?project=" + Request.QueryString["project"] + "&testCase=" + Request.QueryString["testCase"]);
            }
            catch (Exception ex)
            {
                TopMessageLabel.Text = "Error inserting record: " + ex.Message;
            }
        }

        protected void lnkCancelResult_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Analytics/TestURLDetails.aspx?project=" + Request.QueryString["project"] + "&testCase=" + Request.QueryString["testCase"]);
        }
        #endregion

        #region Right Side Buttons

        protected void btnAddEditResults_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/FunctionalTesting/AddEditResults-API.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&insert=Successful");
        }
        protected void btnViewResultHistory_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/FunctionalTesting/ResultHistory-API.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&insert=Successful");
        }
        protected void btnViewUpdateHistory_Click(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            Response.Redirect("~/FunctionalTesting/UpdateHistory-API.aspx?project=" + tbProjectAbbreviationValue + "&testCase=" + tbTestCaseIdValue + "&insert=Successful");
        }

        #endregion

        public string ValidateURLs(string url, string omniture, bool edit)
        {
            string errorMessage = "";

            if (url == "")
            {
                errorMessage = "Must have a URL to add!";
            }

            if (omniture == "")
            {
                errorMessage = "Must have an Omniture String to add!";
            }

            return errorMessage;
        }

        public string ValidateResults(string fieldName, string searchString, string expectedResult, bool edit)
        {
            string errorMessage = "";

            if (fieldName == "")
            {
                errorMessage = "Must have a Field Name to add!";
            }
            else
            {
                foreach (GridViewRow dr in gvExpectedResults.Rows)
                {
                    if (dr.RowType == DataControlRowType.DataRow)
                    {
                        Label lbl = (Label)dr.Cells[3].FindControl("lblFieldName");
                        if (lbl != null && lbl.Text == fieldName)
                        {
                            errorMessage = "Field " + fieldName + " already exists!";
                        }
                    }
                }
            }

            if (searchString == "")
            {
                errorMessage = "Must have a Search String to add!";
            }

            if (expectedResult == "")
            {
                errorMessage = "Must supply an Expected Value!";
            }

            return errorMessage;
        }

        protected void ddlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentEnvironment"] = ddlEnvironment.SelectedValue;
            FiltersChanged();
        }

        protected void FiltersChanged()
        {
            gvURLList.DataSource = DatabaseUtilities.GetAnalyticsURL(Request.QueryString["project"].ToString(),
                Convert.ToInt32(Request.QueryString["testCase"].ToString()),
                ddlEnvironment.SelectedValue);
            gvURLList.DataBind();

            gvExpectedResults.DataSource = DatabaseUtilities.GetAnalyticsExpectedResults(Convert.ToInt32(Request.QueryString["testCase"].ToString()),
                ddlEnvironment.SelectedValue,
                Request.QueryString["project"].ToString());
            gvExpectedResults.DataBind();

            if (gvURLList.Rows.Count > 0)
            {
                btnAddURL.Visible = false;
            }
            else
            {
                btnAddURL.Visible = true;
            }
        }
    }
}