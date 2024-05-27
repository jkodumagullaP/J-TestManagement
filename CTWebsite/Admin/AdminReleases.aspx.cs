using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using CTInfrastructure;

namespace CTWebsite.Admin
{
    public partial class AdminReleases : System.Web.UI.Page
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

        protected void imgCalDateSelectionInsert_click(object sender, EventArgs e)
        {
            Calendar calDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calDateInsert");
            calDateInsert.Visible = !calDateInsert.Visible;
        }

        protected void calDateInsert_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtDateInsert = (TextBox)AspUtilities.FindControlRecursive(this, "txtDateInsert");
            Calendar calDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calDateInsert");
            txtDateInsert.Text = calDateInsert.SelectedDate.ToString("d");
            calDateInsert.Visible = false;
        }

        protected void imgCalDateSelectionEdit_click(object sender, EventArgs e)
        {
            Calendar calDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calDateEdit");
            calDateEdit.Visible = !calDateEdit.Visible;
        }

        protected void calDateEdit_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtDateEdit = (TextBox)AspUtilities.FindControlRecursive(this, "txtDateEdit");
            Calendar calDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calDateEdit");
            txtDateEdit.Text = calDateEdit.SelectedDate.ToString("d");
            calDateEdit.Visible = false;
        }

        protected void ibtnAdd_Click(object sender, ImageClickEventArgs e)
        {
            if (!String.IsNullOrEmpty(ddlProjects.SelectedValue))
            {
                fvReleases.ChangeMode(FormViewMode.Insert);
                fvReleases.Caption = "Add New Release";
                ShowFormView();
                fvReleases.DataBind();
            }
            else
            {
                lblMessage.Text = "Please select a project first";
            }
        }

        protected void ibtnEdit_Click(object sender, ImageClickEventArgs e)
        {
            txtSelectedRelease.Text = ((ImageButton)sender).CommandArgument;
            //if (!String.IsNullOrEmpty(ddlReleases.SelectedValue))
            //{
                fvReleases.ChangeMode(FormViewMode.Edit);
                fvReleases.Caption = "Edit Release";
                ShowFormView();
                fvReleases.DataBind();
            //}
            //else
            //{
            //    lblMessage.Text = "Please select a release first";
            //}
        }

        protected void ibtnRemove_Click(object sender, ImageClickEventArgs e)
        {
            txtSelectedRelease.Text = ((ImageButton)sender).CommandArgument;
            //if (!String.IsNullOrEmpty(ddlReleases.SelectedValue))
            //{
            CTMethods.RemoveRelease(ddlProjects.SelectedValue, txtSelectedRelease.Text);
                lblMessage.Text = "Release Deleted";
                PopulateReleaseDropdown();
            //}
            //else
            //{
            //    lblMessage.Text = "Please select a release first";
            //}
        }

        protected void InsertButton_Click(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(fvReleases, "ddlProject");
            TextBox txtRelease = (TextBox)AspUtilities.FindControlRecursive(fvReleases, "txtRelease");
            TextBox txtDateInsert = (TextBox)AspUtilities.FindControlRecursive(fvReleases, "txtDateInsert");
            TextBox txtbxEditDescription = (TextBox)AspUtilities.FindControlRecursive(fvReleases, "txtbxEditDescription");

            if (txtRelease.Text == "" || txtRelease.Text == null)
            {
                lblMessage.Text = "A release name is required.";
                return;
            }


            //Validate release name
            string errorMessage = CTMethods.ValidateReleaseExists(ddlProject.SelectedValue, txtRelease.Text, txtbxEditDescription.Text, false);

            if (errorMessage == null)
            {
                DateTime dateInsert = DateTime.Parse(txtDateInsert.Text);
                String formattedDateInsert = dateInsert.ToSqlString();

                //sqlDate = DatabaseUtilities.DateParse(txtDateInsert)
                string query = "INSERT INTO [Releases] ([projectAbbreviation], [release], [releaseDescription], [releaseDate]) VALUES ('" + ddlProject.SelectedValue + "', '" + txtRelease.Text + "', '" + txtbxEditDescription.Text + "', '" + formattedDateInsert + "')";
                DatabaseUtilities.ExecuteQuery(query);

                gvReleaseList.DataBind();
                HideFormView();
            }
            else
            {
                lblMessage.Text = errorMessage;
            }
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(fvReleases, "ddlProject");
            TextBox txtRelease = (TextBox)AspUtilities.FindControlRecursive(fvReleases, "txtRelease");
            TextBox txtDateEdit = (TextBox)AspUtilities.FindControlRecursive(fvReleases, "txtDateEdit");
            TextBox txtbxEditDescription = (TextBox)AspUtilities.FindControlRecursive(fvReleases, "txtbxEditDescription");


            // Check to make sure a sprint name exists
            if (txtRelease.Text == "" || txtRelease.Text == null)
            {
                lblMessage.Text = "A sprint name is required.";
                return;
            }

            DateTime start = DateTime.Parse(txtDateEdit.Text);
            String formattedDate = start.ToSqlString();

            string query = "UPDATE Releases set releaseDescription = '" + txtbxEditDescription.Text + "', releaseDate = '" + formattedDate + "' WHERE projectAbbreviation = '" + ddlProject.SelectedValue + "' and [release] = '" + txtRelease.Text + "'";


            DatabaseUtilities.ExecuteQuery(query);
            gvReleaseList.DataBind();
            HideFormView();

        }

        private void ShowFormView()
        {
            fvReleases.Visible = true;
            ddlProjects.Visible = false;
            ibtnAdd.Visible = false;
            lblMessage.Text = "";
        }

        private void HideFormView()
        {
            fvReleases.Visible = false;
            ddlProjects.Visible = true;
            ibtnAdd.Visible = true;
            lblMessage.Text = "";
        }

        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            PopulateReleaseDropdown();
        }

        private void PopulateReleaseDropdown()
        {
            gvReleaseList.DataBind();
            lblMessage.Text = "";
        }

        protected void fvReleases_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Release Inserted";
            PopulateReleaseDropdown();
        }

        protected void fvReleases_ItemDeleted(object sender, FormViewDeletedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Release Deleted";
            PopulateReleaseDropdown();
        }

        protected void fvReleases_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Release Updated";
            PopulateReleaseDropdown();
        }

        protected void fvReleases_DataBound(object sender, EventArgs e)
        {
            DropDownList viewsDdlProject = (DropDownList)AspUtilities.FindControlRecursive(fvReleases, "ddlProject");
            if (viewsDdlProject != null)
            {
                viewsDdlProject.SelectedValue = ddlProjects.SelectedValue;
            }

            //TextBox txtProject = (TextBox)AspUtilities.FindControlRecursive(fvReleases, "txtProject");
            //if (txtProject != null)
            //{
            //    txtProject.Text = ddlProjects.SelectedValue;
            //}
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            HideFormView();
        }
    }
}