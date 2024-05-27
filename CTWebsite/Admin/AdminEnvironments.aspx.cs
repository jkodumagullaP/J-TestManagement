using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Data.OleDb;
using Utilities;
using CTInfrastructure;

namespace CTWebsite.Admin
{
    public partial class AdminEnvironments: System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                ddlProjects.DataBind();

                //Initialize gridviews and dropdown boxes.
                if (ddlProjects.Items.Count > 0)
                {
                    ddlProjects.Items.Insert(0, "Select a Project");
                    ddlProjects.Items[0].Value = "";
                    ddlProjects.SelectedIndex = 0;
                }

                string projectQueryStringValue = Request.QueryString["project"];

                if (!String.IsNullOrEmpty(projectQueryStringValue))
                {
                    ddlProjects.SelectedValue = projectQueryStringValue;
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
                        }
                    }
                }

                HideFormView();
            }
        }

        protected void ibtnAdd_Click(object sender, ImageClickEventArgs e)
        {
            if (!String.IsNullOrEmpty(ddlProjects.SelectedValue))
            {
                fvEnvironments.ChangeMode(FormViewMode.Insert);
                fvEnvironments.Caption = "Add New Environment";
                ShowFormView();
                fvEnvironments.DataBind();
            }
            else
            {
                lblMessage.Text = "Please select a project first";
            }
        }

        protected void ibtnEdit_Click(object sender, ImageClickEventArgs e)
        {
            txtSelectedEnvironment.Text = ((ImageButton)sender).CommandArgument;
                fvEnvironments.ChangeMode(FormViewMode.Edit);
                fvEnvironments.Caption = "Edit Environment";
                ShowFormView();
                fvEnvironments.DataBind();
        }

        protected void ibtnRemove_Click(object sender, ImageClickEventArgs e)
        {
            txtSelectedEnvironment.Text = ((ImageButton)sender).CommandArgument;
            CTMethods.RemoveEnvironment(ddlProjects.SelectedValue, txtSelectedEnvironment.Text);
                lblMessage.Text = "Environment Deleted";
                PopulateEnvironmentDropdown();
        }

        private void ShowFormView()
        {
            fvEnvironments.Visible = true;
            ddlProjects.Visible = false;
            ibtnAdd.Visible = false;
            lblMessage.Text = "";
        }

        private void HideFormView()
        {
            fvEnvironments.Visible = false;
            ddlProjects.Visible = true;
            ibtnAdd.Visible = true;
            lblMessage.Text = "";
        }

        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            PopulateEnvironmentDropdown();
        }

        private void PopulateEnvironmentDropdown()
        {
            gvEnvironmentList.DataBind();
            lblMessage.Text = "";
        }

        protected void fvEnvironments_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Environment Inserted";
            PopulateEnvironmentDropdown();
        }

        protected void fvEnvironments_ItemDeleted(object sender, FormViewDeletedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Environment Deleted";
            PopulateEnvironmentDropdown();
        }

        protected void fvEnvironments_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Environment Updated";
            PopulateEnvironmentDropdown();
        }

        protected void fvEnvironments_DataBound(object sender, EventArgs e)
        {
            DropDownList viewsDdlProject = (DropDownList)AspUtilities.FindControlRecursive(fvEnvironments, "ddlProject");
            if (viewsDdlProject != null)
            {
                viewsDdlProject.SelectedValue = ddlProjects.SelectedValue;
            }
        }
        protected void InsertButton_Click(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(fvEnvironments, "ddlProject");
            TextBox txtEnvironment = (TextBox)AspUtilities.FindControlRecursive(fvEnvironments, "txtEnvironment");
            TextBox txtBaseURL = (TextBox)AspUtilities.FindControlRecursive(fvEnvironments, "txtBaseURL");
            TextBox txtBaseAdminURL = (TextBox)AspUtilities.FindControlRecursive(fvEnvironments, "txtBaseAdminURL");
            CheckBox chkDefaultEnvironment = (CheckBox)AspUtilities.FindControlRecursive(fvEnvironments, "chkDefaultEnvironment");

            // Validate required fields
            if (txtEnvironment.Text == "" || txtEnvironment.Text == null)
            {
                lblMessage.Text = "An environment name is required.";
                return;
            }

            if (txtBaseURL.Text == "" || txtBaseURL.Text == null)
            {
                lblMessage.Text = "A Base URL is required.";
                return;
            }

            //Validate the new environment
            string environmentErrorMessage = CTMethods.ValidateEnvironmentExists(txtEnvironment.Text, false);

            //Exists in Environments table
            if (environmentErrorMessage != null)
            {
                string environmentProjectErrorMessage = CTMethods.ValidateProjectEnvironmentExists(ddlProjects.SelectedValue, txtEnvironment.Text, false);

                //Exists in ProjectEnvironmentInfo table AND ProjectEnvironmentInfo table (Duplicate)
                if (environmentProjectErrorMessage != null)
                {
                    lblMessage.Text = environmentProjectErrorMessage;
                    return;
                }
                
                //Exists in Environment table only (This is ok. Just add to ProjectEnvironmentInfo table)
                else
                {
                    //If default environment is checked, clear previous default
                    if (chkDefaultEnvironment.Checked)
                    {
                        CTMethods.RemoveAllDefaultEnvironments(ddlProject.SelectedValue);
                    }

                    string query = "INSERT INTO [ProjectEnvironmentInfo] ([projectAbbreviation], [environment], [baseURL], [baseAdminURL], [defaultEnvironment]) VALUES ('" + ddlProject.SelectedValue + "', '" + txtEnvironment.Text + "', '" + txtBaseURL.Text + "', '" + txtBaseAdminURL.Text + "', '" + chkDefaultEnvironment.Checked + "')";
                    DatabaseUtilities.ExecuteQuery(query);
                    
                    gvEnvironmentList.DataBind();
                    HideFormView();
                }
            }
            
            // Does not exist in Environments table
            else
            {
                string environmentProjectErrorMessage = CTMethods.ValidateProjectEnvironmentExists(ddlProjects.SelectedValue, txtEnvironment.Text, false);

                //Exists in ProjectEnvironmentInfo table but not Environments table (This should never happen, but if it does, fix it. Add to environments and report the duplicate
                if (environmentProjectErrorMessage != null)
                {
                    string query = "INSERT INTO [Environments] ([environment]) VALUES ('" + txtEnvironment.Text + "')";
                    DatabaseUtilities.ExecuteQuery(query);

                    lblMessage.Text = environmentProjectErrorMessage + "<br/> This is a rare occurance where somehow this environment belongs to a project without existing in the Environments table. This should never happen. Fixing it now by adding it to the environments table. You will still need to change the environment name as it is still a duplicate.";
                    return;
                }
                
                //Environment does not exist in either table. Add to both
                else
                {
                    //If default environment is checked, clear previous default
                    if (chkDefaultEnvironment.Checked)
                    {
                        CTMethods.RemoveAllDefaultEnvironments(ddlProject.SelectedValue);
                    }

                    string environmentQuery = "INSERT INTO [Environments] ([environment]) VALUES ('" + txtEnvironment.Text + "')";
                    DatabaseUtilities.ExecuteQuery(environmentQuery);
                    
                    string projectEnvironmentQuery = "INSERT INTO [ProjectEnvironmentInfo] ([projectAbbreviation], [environment], [baseURL], [baseAdminURL], [defaultEnvironment]) VALUES ('" + ddlProject.SelectedValue + "', '" + txtEnvironment.Text + "', '" + txtBaseURL.Text + "', '" + txtBaseAdminURL.Text + "', '" + chkDefaultEnvironment.Checked + "')";
                    DatabaseUtilities.ExecuteQuery(projectEnvironmentQuery);

                    gvEnvironmentList.DataBind();
                    HideFormView();
                }
            }
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(fvEnvironments, "ddlProject");
            TextBox txtEnvironment = (TextBox)AspUtilities.FindControlRecursive(fvEnvironments, "txtEnvironment");
            TextBox txtBaseURL = (TextBox)AspUtilities.FindControlRecursive(fvEnvironments, "txtBaseURL");
            TextBox txtBaseAdminURL = (TextBox)AspUtilities.FindControlRecursive(fvEnvironments, "txtBaseAdminURL");
            CheckBox chkDefaultEnvironment = (CheckBox)AspUtilities.FindControlRecursive(fvEnvironments, "chkDefaultEnvironment");

            // Check to make sure a baseURL exists

            if (txtEnvironment.Text == "" || txtEnvironment.Text == null)
            {
                lblMessage.Text = "An environment name is required.";
                return;
            }

            if (txtBaseURL.Text == "" || txtBaseURL.Text == null)
            {
                lblMessage.Text = "A Base URL is required.";
                return;
            }

            if (chkDefaultEnvironment.Checked)
            {
                CTMethods.RemoveAllDefaultEnvironments(ddlProject.SelectedValue);
            }


            string query = "Update [ProjectEnvironmentInfo] set  baseURL = '" + txtBaseURL.Text + "', baseAdminURL = '" + txtBaseAdminURL.Text + "', defaultEnvironment = '" + chkDefaultEnvironment.Checked + "' where [projectAbbreviation] = '" + ddlProject.SelectedValue + "' and [environment] = '" + txtEnvironment.Text + "'";
            
            DatabaseUtilities.ExecuteQuery(query);
            gvEnvironmentList.DataBind();
            HideFormView();
        }

        protected void chkDefaultEnvironment_CheckedChanged(object sender, EventArgs e)
        {
            //Get row on where CheckedChanged event was called
            GridViewRow Row = ((GridViewRow)((Control)sender).Parent.Parent);
            
            string projectAbbreviation = Row.Cells[1].Text;
            string environment = Row.Cells[2].Text;            
            
            CTMethods.RemoveAllDefaultEnvironments(projectAbbreviation);
            CTMethods.AddDefaultEnvironments(projectAbbreviation, environment);
            gvEnvironmentList.DataBind();

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            HideFormView();
        }

    }
}