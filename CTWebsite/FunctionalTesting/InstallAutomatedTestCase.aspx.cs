using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Utilities;
using CTInfrastructure;

namespace CTWebsite.FunctionalTesting
{
    public partial class InstallAutomatedTestCase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                ddlProjects.DataBind();

                //Initialize gridviews and dropdown boxes.
                if (ddlProjects.Items.Count > 0)
                {
                    ddlProjects.Items.Insert(0, "Select a Project");
                    ddlProjects.Items[0].Value = "";
                    ddlProjects.SelectedIndex = 0;
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
                }

                if (!IsPostBack)
                {
                    //Add items to the Automated dropdown list.
                    ddlAutomated.Items.Add("");
                    ddlAutomated.Items.Add("Yes");
                    ddlAutomated.Items.Add("No");
                    ddlAutomated.Items.Add("Future");
                }

                refreshDatagrid();
            }
        }

        protected void lvChildAutomatedTestCases_DataBound(object sender, EventArgs e)
        {
            //DropDownList viewsDdlProject = (DropDownList)AspUtilities.FindControlRecursive(fvSprints, "ddlProject");
            //if (viewsDdlProject != null)
            //{
            //    viewsDdlProject.SelectedValue = ddlProjects.SelectedValue;
            //}
        }

        protected void btnRemoveChild_OnClick(object sender, EventArgs e)
        {
            string commandArgument = ((ImageButton)sender).CommandArgument;

            CTMethods.RemoveAutomatedTestMap(
                ddlProjects.SelectedValue,
                Int32.Parse(txtTestCaseID.Text),
                Int32.Parse(commandArgument));

            CTMethods.RemoveChildCase(ddlProjects.SelectedValue, Int32.Parse(commandArgument));

            refreshDatagrid();
        }

        //Install Automated Test
        protected void Button1_Click(object sender, EventArgs e)
        {
            string error = CTMethods.ValidateTestCaseIDForResults(ddlProjects.SelectedValue, txtTestCaseID.Text);
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

                string autoMetaDataTable = txtautoMetaDataTable.Text;

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
                                ddlProjects.SelectedValue,
                                Int32.Parse(txtTestCaseID.Text),
                                Int32.Parse(ChildID.Text));
                            
                            //Update Child Case to blank only if NOT already set to "yes"
                            if (TestCaseIsNotAutomated(Int32.Parse(ChildID.Text)))
                            {
                                if (!TestCaseIsChild(Int32.Parse(ChildID.Text)))
                                {
                                    CTMethods.UpdateTestCaseForAutomation(
                                            AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                                            ddlProjects.SelectedValue,
                                            Int32.Parse(ChildID.Text),
                                            "",
                                            "",
                                            null,
                                            "",
                                            "Test removed from automation because it was a child of ID " + txtTestCaseID.Text + " which was uninstalled on " + DateTime.Now + " by " + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name));
                                }
                                else
                                {
                                    CTMethods.UpdateTestCaseForAutomation(
                                            AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                                            ddlProjects.SelectedValue,
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
                ddlProjects.SelectedValue,
                Int32.Parse(txtTestCaseID.Text),
                txtAutoTestClass.Text,
                txtautoMetaDataTable.Text,
                autoMetaDataRow,
                ddlAutomated.SelectedValue,
                txtAutomationReason.Text);

                lblStatus.Text = "Updated Successfuly";
                lblMessage.Text = "";
                txtTestCaseID.Text = "";
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
        }

        // Add Child test cases to automated parent test
        protected void Button2_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtChildTestCaseID.Text))
            {
                List<String> childTestCaseIDs = txtChildTestCaseID.Text.Split(new[] { ',', ' ' }).ToList();

                bool updatesMade = false;

                foreach (String childTestCaseID in childTestCaseIDs)
                {
                    if (!String.IsNullOrEmpty(childTestCaseID))
                    {
                        string error = CTMethods.ValidateTestCaseIDForResults(ddlProjects.SelectedValue, txtTestCaseID.Text)
                            ?? CTMethods.ValidateTestCaseIDForResults(ddlProjects.SelectedValue, childTestCaseID);
                        string automationCheck = ValidateAutomationFields(true);

                        if (error == null && automationCheck == "")
                        {
                            string query = "select "
                                + " childTestCaseId "
                                + " from AutoTestCaseMap a "
                                + " join TestCases t "
                                + " on t.projectAbbreviation = a.projectAbbreviation "
                                + " and t.testcaseid = a.parentTestCaseId "
                                + " where a.projectAbbreviation = '" + ddlProjects.SelectedValue + "' "
                                + " and parentTestCaseId = " + txtTestCaseID.Text;

                            List<int> mappedTestCaseChildren = DatabaseUtilities.GetIntListFromQuery(query);

                            if (mappedTestCaseChildren.Contains(Int32.Parse(childTestCaseID)))
                            {
                                lblMessage.Text = "Test case " + childTestCaseID + " already exists";
                                refreshDatagrid();
                                return;
                            }

                            CTMethods.AddAutomatedTestMap(
                                ddlProjects.SelectedValue,
                                Int32.Parse(txtTestCaseID.Text),
                                Int32.Parse(childTestCaseID),
                                AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name));

                            CTMethods.AddChildCase(ddlProjects.SelectedValue, Int32.Parse(childTestCaseID));

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

        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            txtChildTestCaseID.Text = "";
            lblMessage.Text = "";
            PrePopulateFields();
            refreshDatagrid();
        }

        protected void txtTestCaseID_TextChanged(object sender, EventArgs e)
        {
            txtChildTestCaseID.Text = "";
            lblMessage.Text = "";
            PrePopulateFields();
            refreshDatagrid();
        }

        protected bool TestCaseIsNotAutomated(int testCaseId)
        {
            SqlConnection conn = new SqlConnection(Constants.QAAConnectionString);
            string sql = "select automated from TestCases " +
                        "where projectAbbreviation = '" + ddlProjects.SelectedValue + "' " +
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

        protected bool TestCaseIsChild(Int32 testCaseId)
        {
            SqlConnection conn = new SqlConnection(Constants.QAAConnectionString);
            conn.Open();
            string sql = "select * from AutoTestCaseMap " +
                    "where projectAbbreviation = '" + ddlProjects.SelectedValue + "' " +
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

        protected void PrePopulateFields()
        {
            string error = CTMethods.ValidateTestCaseIDForResults(ddlProjects.SelectedValue, txtTestCaseID.Text);

            if (error == null)
            {
                Type autoTestClass;
                string description;
                string autoMetaDataTable;
                int? autoMetaDataRow;
                string autoTestClassType;
                string automated;
                string reasonForNotAutomated;
                AutomatedTestBase.GetTestCaseAutomationParameters(ddlProjects.SelectedValue, Int32.Parse(txtTestCaseID.Text), out description, out autoTestClassType, out autoTestClass, out autoMetaDataTable, out autoMetaDataRow, out automated, out reasonForNotAutomated);

                lblDescription.Text = description;
                txtAutoTestClass.Text = autoTestClassType;
                txtautoMetaDataTable.Text = autoMetaDataTable;
                txtautoMetaDataRow.Text = autoMetaDataRow.HasValue ? autoMetaDataRow.Value.ToString() : "";

                if (TestCaseIsChild(Int32.Parse(txtTestCaseID.Text)))
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

        private void refreshDatagrid()
        {
            SqlDataSource sqlChildAutomatedTestCases = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlChildAutomatedTestCases");
            int testCaseID;

            if (!String.IsNullOrEmpty(ddlProjects.SelectedValue) 
                && !String.IsNullOrEmpty(txtTestCaseID.Text)
                && Int32.TryParse(txtTestCaseID.Text, out testCaseID))
            {
                sqlChildAutomatedTestCases.SelectCommand =
                    "select "
                    +" t.projectAbbreviation, "
                    +" childTestCaseId, "
                    +" testCaseDescription "
                    +" from AutoTestCaseMap a "
                    +" join TestCases t "
                    +" on t.projectAbbreviation = a.projectAbbreviation "
                    +" and t.testcaseid = a.childTestCaseId "
                    +" where t.projectAbbreviation = '" + ddlProjects.SelectedValue + "' and parentTestCaseId = " + testCaseID
                    +" order by childTestCaseId";
            }
            else
            {
                sqlChildAutomatedTestCases.SelectCommand = "";
            }

            sqlChildAutomatedTestCases.DataBind();

            ListView lvChildAutomatedTestCases = (ListView)AspUtilities.FindControlRecursive(this, "lvChildAutomatedTestCases");
            lvChildAutomatedTestCases.DataBind();
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
    }
}