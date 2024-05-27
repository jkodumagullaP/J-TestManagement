using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using CTInfrastructure;

namespace CTWebsite.Admin
{
    public partial class AdminGroups : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            SqlDataSource sqlGroups = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlGroups");
            if (sqlGroups.SelectParameters["username"] == null)
            {
                Parameter userParam = new Parameter();
                userParam.Name = "username";
                userParam.DefaultValue = Utilities.AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                sqlGroups.SelectParameters.Add(userParam);
            }

            
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

                PopulateGroupDropdown();

                string groupAbbreviationQueryStringValue = Request.QueryString["groupAbbreviation"];

                if (!String.IsNullOrEmpty(groupAbbreviationQueryStringValue))
                {
                    txtSelectedGroup.Text = groupAbbreviationQueryStringValue;

                    fvGroups.ChangeMode(FormViewMode.Edit);
                    fvGroups.Caption = "Edit Group";
                    ShowFormView();
                    fvGroups.DataBind();
                }
                else
                {
                    HideFormView();
                }
            }
        }

        protected void ibtnAdd_Click(object sender, ImageClickEventArgs e)
        {
            if (!String.IsNullOrEmpty(ddlProjects.SelectedValue))
            {
                fvGroups.ChangeMode(FormViewMode.Insert);
                fvGroups.Caption = "Add New Group";
                ShowFormView();
                fvGroups.DataBind();
            }
            else
            {
                lblMessage.Text = "Please select a project first";
            }
        }

        protected void ibtnEdit_Click(object sender, ImageClickEventArgs e)
        {
            txtSelectedGroup.Text = ((ImageButton)sender).CommandArgument;

            //if (!String.IsNullOrEmpty(ddlGroupTest.SelectedValue))
            //{
                fvGroups.ChangeMode(FormViewMode.Edit);
                fvGroups.Caption = "Edit Group";
                ShowFormView();
                fvGroups.DataBind();
            //}
            //else
            //{
            //    lblMessage.Text = "Please select a group first";
            //}
        }

        protected void ibtnRemove_Click(object sender, ImageClickEventArgs e)
        {
            txtSelectedGroup.Text = ((ImageButton)sender).CommandArgument;
            
            //if (!String.IsNullOrEmpty(ddlGroupTest.SelectedValue))
            //{
            CTMethods.RemoveGroup(ddlProjects.SelectedValue, txtSelectedGroup.Text);
                lblMessage.Text = "Group Deleted";

                PopulateGroupDropdown();
            //}
            //else
            //{
            //    lblMessage.Text = "Please select a group first";
            //}
        }

        private void ShowFormView()
        {
            fvGroups.Visible = true;
            ddlProjects.Visible = false;
            ibtnAdd.Visible = false;
            lblMessage.Text = "";
        }

        private void HideFormView()
        {
            fvGroups.Visible = false;
            ddlProjects.Visible = true;
            ibtnAdd.Visible = true;
            lblMessage.Text = "";
        }

        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            PopulateGroupDropdown();
        }

        private void PopulateGroupDropdown()
        {

            lblMessage.Text = "";

            gvGroupTests.DataBind();
        }

        protected void fvGroups_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Group Inserted";
            PopulateGroupDropdown();
        }

        protected void fvGroups_ItemDeleted(object sender, FormViewDeletedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Group Deleted";

            PopulateGroupDropdown();
        }

        protected void fvGroups_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            HideFormView();
            lblMessage.Text = "Group Updated";
            PopulateGroupDropdown();
        }

        protected void fvGroups_DataBound(object sender, EventArgs e)
        {
            DropDownList viewsDdlProject = (DropDownList)AspUtilities.FindControlRecursive(fvGroups, "ddlProject");
            if (viewsDdlProject != null)
            {
                viewsDdlProject.SelectedValue = ddlProjects.SelectedValue;
            }

            CheckBox editChkSelected = (CheckBox)AspUtilities.FindControlRecursive(this, "editChkSelected");
            if (editChkSelected != null)
            {
                editChkSelected.Checked = CTMethods.IsPrivateGroup(viewsDdlProject.SelectedValue, txtSelectedGroup.Text);
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            HideFormView();
        }

        protected void InsertButton_Click(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(fvGroups, "ddlProject");
            TextBox txtGroupAbbreviation = (TextBox)AspUtilities.FindControlRecursive(fvGroups, "txtGroupAbbreviation");
            TextBox txtGroupTestName = (TextBox)AspUtilities.FindControlRecursive(fvGroups, "txtGroupTestName");
            CheckBox insertChkSelected = (CheckBox)AspUtilities.FindControlRecursive(fvGroups, "insertChkSelected");
            TextBox txtGroupTestDescription = (TextBox)AspUtilities.FindControlRecursive(fvGroups, "txtGroupTestDescription");

            //Validate required fields
            if (txtGroupAbbreviation.Text == "" || txtGroupAbbreviation.Text == null)
            {
                lblMessage.Text = " group abbreviaiton is required.";
                return;
            }

            if (txtGroupTestName.Text == "" || txtGroupTestName.Text == null)
            {
                lblMessage.Text = "A group name is required.";
                return;
            }

            //Validate duplications
            string errorMessage = CTMethods.ValidateGroupExists(ddlProject.SelectedValue, txtGroupAbbreviation.Text, txtGroupTestName.Text, txtGroupTestDescription.Text, false);

            if (errorMessage == null)
            {
                string testOwner = insertChkSelected.Checked ? "'" + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) + "'" : "null";

                string query = "INSERT INTO [GroupTests] ([projectAbbreviation], [groupTestAbbreviation], [groupTestName], [groupTestDescription], [personalGroupOwner]) VALUES ('" + ddlProject.SelectedValue + "', '" + txtGroupAbbreviation.Text + "', '" + txtGroupTestName.Text + "', '" + txtGroupTestDescription.Text + "', " + testOwner + ")";
                DatabaseUtilities.ExecuteQuery(query);

                gvGroupTests.DataBind();
                HideFormView();
            }
            else
            {
                lblMessage.Text = errorMessage;
            }

        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(fvGroups, "ddlProject");
            TextBox txtGroupAbbreviation = (TextBox)AspUtilities.FindControlRecursive(fvGroups, "txtGroupAbbreviation");
            TextBox txtGroupTestName = (TextBox)AspUtilities.FindControlRecursive(fvGroups, "txtGroupTestName");
            CheckBox editChkSelected = (CheckBox)AspUtilities.FindControlRecursive(fvGroups, "editChkSelected");
            TextBox txtGroupTestDescription = (TextBox)AspUtilities.FindControlRecursive(fvGroups, "txtGroupTestDescription");

            // Validate required fields

            if (txtGroupAbbreviation.Text == "" || txtGroupAbbreviation.Text == null)
            {
                lblMessage.Text = "A group abbreviation is required.";
                return;
            }

            if (txtGroupTestName.Text == "" || txtGroupTestName.Text == null)
            {
                lblMessage.Text = "A group name is required.";
                return;
            }

            string errorMessage = CTMethods.ValidateGroupExists(ddlProject.SelectedValue, txtGroupAbbreviation.Text, txtGroupTestName.Text, txtGroupTestDescription.Text, true);
            
            if (errorMessage == null)
            {
            string testOwner = editChkSelected.Checked ? "'" + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) + "'" : "null";
            string query = "update [GroupTests] set [groupTestName] = '" + txtGroupTestName.Text + "', [groupTestDescription] = '" + txtGroupTestDescription.Text + "', [personalGroupOwner] = " + testOwner + " where [projectAbbreviation] = '" + ddlProject.SelectedValue + "' AND [groupTestAbbreviation] = '" + txtGroupAbbreviation.Text + "'";

            DatabaseUtilities.ExecuteQuery(query);

            gvGroupTests.DataBind();
            HideFormView();
            }
            else
            {
                lblMessage.Text = errorMessage;
            }
        }
    }
} 