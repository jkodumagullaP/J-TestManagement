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
    public partial class AdminSprints : System.Web.UI.Page
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

        protected void imgStartDateSelectionInsert_click(object sender, EventArgs e)
        {
            Calendar calStartDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calStartDateInsert");
            calStartDateInsert.Visible = !calStartDateInsert.Visible;
        }

        protected void calStartDateInsert_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtStartDateInsert = (TextBox)AspUtilities.FindControlRecursive(this, "txtStartDateInsert");
            Calendar calStartDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calStartDateInsert");
            txtStartDateInsert.Text = calStartDateInsert.SelectedDate.ToString("d");
            calStartDateInsert.Visible = false;
        }

        protected void imgEndDateSelectionInsert_click(object sender, EventArgs e)
        {
            Calendar calEndDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calEndDateInsert");
            calEndDateInsert.Visible = !calEndDateInsert.Visible;
        }

        protected void calEndDateInsert_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtEndDateInsert = (TextBox)AspUtilities.FindControlRecursive(this, "txtEndDateInsert");
            Calendar calEndDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calEndDateInsert");
            txtEndDateInsert.Text = calEndDateInsert.SelectedDate.ToString("d");
            calEndDateInsert.Visible = false;

        }

        protected void imgStartDateSelectionEdit_click(object sender, EventArgs e)
        {
            Calendar calStartDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calStartDateEdit");
            calStartDateEdit.Visible = !calStartDateEdit.Visible;
        }

        protected void calStartDateEdit_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtStartDateEdit = (TextBox)AspUtilities.FindControlRecursive(this, "txtStartDateEdit");
            Calendar calStartDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calStartDateEdit");
            txtStartDateEdit.Text = calStartDateEdit.SelectedDate.ToString("d");
            calStartDateEdit.Visible = false;
        }

        protected void imgEndDateSelectionEdit_click(object sender, EventArgs e)
        {
            Calendar calEndDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calEndDateEdit");
            calEndDateEdit.Visible = !calEndDateEdit.Visible;
        }

        protected void calEndDateEdit_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtEndDateEdit = (TextBox)AspUtilities.FindControlRecursive(this, "txtEndDateEdit");
            Calendar calEndDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calEndDateEdit");
            txtEndDateEdit.Text = calEndDateEdit.SelectedDate.ToString("d");
            calEndDateEdit.Visible = false;

        }

        protected void ibtnAdd_Click(object sender, ImageClickEventArgs e)
        {
            if (!String.IsNullOrEmpty(ddlProjects.SelectedValue))
            {
                fvSprints.ChangeMode(FormViewMode.Insert);
                fvSprints.Caption = "Add New Sprint";
                ShowFormView();
                fvSprints.DataBind();
            }
            else
            {
                lblMessage.Text = "Please select a project first";
            }
        }

        protected void ibtnEdit_Click(object sender, ImageClickEventArgs e)
        {
            txtSelectedSprint.Text = ((ImageButton)sender).CommandArgument;
            //if (!String.IsNullOrEmpty(ddlSprints.SelectedValue))
            //{
            fvSprints.ChangeMode(FormViewMode.Edit);
            fvSprints.Caption = "Edit Sprint";
            ShowFormView();
            fvSprints.DataBind();

            //}
            //else
            //{
            //    lblMessage.Text = "Please select a sprint first";
            //}
        }

        protected void ibtnRemove_Click(object sender, ImageClickEventArgs e)
        {
            txtSelectedSprint.Text = ((ImageButton)sender).CommandArgument;
            //if (!String.IsNullOrEmpty(ddlSprints.SelectedValue))
            //{
            CTMethods.RemoveSprint(ddlProjects.SelectedValue, txtSelectedSprint.Text);
            lblMessage.Text = "Sprint Deleted";
            PopulateSprintDropdown();
            //}
            //else
            //{
            //    lblMessage.Text = "Please select a sprint first";
            //}
        }

        private void ShowFormView()
        {
            fvSprints.Visible = true;
            ddlProjects.Visible = false;
            ibtnAdd.Visible = false;
            lblMessage.Text = "";
        }

        private void HideFormView()
        {
            fvSprints.Visible = false;
            ddlProjects.Visible = true;
            ibtnAdd.Visible = true;
            lblMessage.Text = "";
        }

        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            PopulateSprintDropdown();
        }

        private void PopulateSprintDropdown()
        {
            lblMessage.Text = "";
            gvSprintList.DataBind();
        }

        protected void fvSprints_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Sprint Inserted";
            PopulateSprintDropdown();
        }

        protected void fvSprints_ItemDeleted(object sender, FormViewDeletedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Sprint Deleted";
            PopulateSprintDropdown();
        }

        protected void fvSprints_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Sprint Updated";
            PopulateSprintDropdown();
        }

        protected void fvSprints_DataBound(object sender, EventArgs e)
        {
            DropDownList viewsDdlProject = (DropDownList)AspUtilities.FindControlRecursive(fvSprints, "ddlProject");
            if (viewsDdlProject != null)
            {
                viewsDdlProject.SelectedValue = ddlProjects.SelectedValue;
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            HideFormView();
        }

        protected void fvSprints_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            TextBox txtStartDateEdit = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtStartDateEdit");
            TextBox txtEndDateEdit = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtEndDateEdit");

            DateTime startDate;
            bool startDateParsedSuccessfully = DateTime.TryParse(txtStartDateEdit.Text, out startDate);

            DateTime endDate;
            bool endDateParsedSuccessfully = DateTime.TryParse(txtEndDateEdit.Text, out endDate);

            lblMessage.Text = "";

            if (startDateParsedSuccessfully && endDateParsedSuccessfully)
            {
                if (endDate < startDate)
                {
                    lblMessage.Text = "Start date must be on or after end date";
                    e.Cancel = true;
                }
            }
        }

        protected void InsertButton_Click(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(fvSprints, "ddlProject");
            TextBox txtSprint = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtSprint");
            TextBox txtStartDateInsert = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtStartDateInsert");
            TextBox txtEndDateInsert = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtEndDateInsert");
            TextBox txtbxEditDescription = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtbxEditDescription");


            // Check to make sure a sprint name exists
            if (txtSprint.Text == "" || txtSprint.Text == null)
            {
                lblMessage.Text = "A sprint name is required.";
                return;
            }


            //Validate sprint name
            string errorMessage = CTMethods.ValidateSprintExists(ddlProject.SelectedValue, txtSprint.Text, txtbxEditDescription.Text, false);

            if (errorMessage == null)
            {

                DateTime start = DateTime.Parse(txtStartDateInsert.Text);
                String formattedStart = start.ToSqlString();

                DateTime end = DateTime.Parse(txtEndDateInsert.Text);
                String formattedEnd = end.ToSqlString();

                string query = "INSERT INTO [Sprints] ([projectAbbreviation], [sprint], [sprintDescription], [sprintStartDate], [sprintEndDate]) VALUES ('" + ddlProject.SelectedValue + "', '" + txtSprint.Text + "', '" + txtbxEditDescription.Text + "', '" + formattedStart + "', '" + formattedEnd + "')";

                DatabaseUtilities.ExecuteQuery(query);
                gvSprintList.DataBind();
                HideFormView();
            }
            else
            {
                lblMessage.Text = errorMessage;
            }

        }

        
        protected void EditButton_Click(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(fvSprints, "ddlProject");
            TextBox txtSprint = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtSprint");
            TextBox txtStartDateEdit = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtStartDateEdit");
            TextBox txtEndDateEdit = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtEndDateEdit");
            TextBox txtbxEditDescription = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtbxEditDescription");


            // Check to make sure a sprint name exists
            if (txtSprint.Text == "" || txtSprint.Text == null)
            {
                lblMessage.Text = "A sprint name is required.";
                return;
            }

            DateTime start = DateTime.Parse(txtStartDateEdit.Text);
            String formattedStart = start.ToSqlString();

            DateTime end = DateTime.Parse(txtEndDateEdit.Text);
            String formattedEnd = end.ToSqlString();

            string query = "UPDATE Sprints set sprintDescription = '" + txtbxEditDescription.Text + "', sprintStartDate = '" + formattedStart + "', sprintEndDate = '" + formattedEnd + "' WHERE projectAbbreviation = '" + ddlProject.SelectedValue + "' and [sprint] = '" + txtSprint.Text + "'";
                
                
            DatabaseUtilities.ExecuteQuery(query);
            gvSprintList.DataBind();
            HideFormView();

        }
    
    protected void fvSprints_ItemInserting(object sender, FormViewInsertEventArgs e)
        {
            TextBox txtStartDateInsert = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtStartDateInsert");
            TextBox txtEndDateInsert = (TextBox)AspUtilities.FindControlRecursive(fvSprints, "txtEndDateInsert");

            DateTime startDate;
            bool startDateParsedSuccessfully = DateTime.TryParse(txtStartDateInsert.Text, out startDate);

            DateTime endDate;
            bool endDateParsedSuccessfully = DateTime.TryParse(txtEndDateInsert.Text, out endDate);

            lblMessage.Text = "";

            if (startDateParsedSuccessfully && endDateParsedSuccessfully)
            {
                if (endDate < startDate)
                {
                    lblMessage.Text = "Start date must be on or after end date";
                    e.Cancel = true;
                }
            }
        }
    }
}   