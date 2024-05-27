using AjaxControlToolkit;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using CTInfrastructure;

namespace CTWebsite.UserControls
{
    public partial class AddNewSprint : System.Web.UI.UserControl
    {
        //Declare the event created to force a databind on the parent page (as a default EventHandler)
        public event EventHandler SprintReadyForDatabindEvent;
        
        // if you wanted to customize the parameters, you'd declare your own event handler like so (delegate is the same thing as eventhandler)
        //public delegate void NoArgsEventHandler();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (HttpContext.Current.Session["CurrentProject"] != null)
            {
                lblAddNewSprintProject.Text = HttpContext.Current.Session["CurrentProject"].ToString();
            }
            else
            {
                // Get logged in user's default project
                string defaultProject = CTMethods.GetDefaultProject(HttpContext.Current.User.Identity);
                if (defaultProject != null)
                {
                    lblAddNewSprintProject.Text = defaultProject;
                }
            }
        }

        protected void imgStartDateSelectionInsert_click(object sender, EventArgs e)
        {
            Calendar calStartDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calStartDateInsert");
            calStartDateInsert.Visible = !calStartDateInsert.Visible;
            Panel Panel3 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel3");
            ModalPopupExtender ModalPopupExtender3 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel3.Parent, "ModalPopupExtender3");
            ModalPopupExtender3.Show();
        }

        protected void calStartDateInsert_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtStartDateInsert = (TextBox)AspUtilities.FindControlRecursive(this, "txtStartDateInsert");
            Calendar calStartDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calStartDateInsert");
            txtStartDateInsert.Text = calStartDateInsert.SelectedDate.ToString("d");
            calStartDateInsert.Visible = false;
            Panel Panel3 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel3");
            ModalPopupExtender ModalPopupExtender3 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel3.Parent, "ModalPopupExtender3");
            ModalPopupExtender3.Show();
        }

        protected void imgEndDateSelectionInsert_click(object sender, EventArgs e)
        {
            Calendar calEndDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calEndDateInsert");
            calEndDateInsert.Visible = !calEndDateInsert.Visible;
            Panel Panel3 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel3");
            ModalPopupExtender ModalPopupExtender3 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel3.Parent, "ModalPopupExtender3");
            ModalPopupExtender3.Show();
        }

        protected void calEndDateInsert_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtEndDateInsert = (TextBox)AspUtilities.FindControlRecursive(this, "txtEndDateInsert");
            Calendar calEndDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calEndDateInsert");
            txtEndDateInsert.Text = calEndDateInsert.SelectedDate.ToString("d");
            calEndDateInsert.Visible = false;
            Panel Panel3 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel3");
            ModalPopupExtender ModalPopupExtender3 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel3.Parent, "ModalPopupExtender3");
            ModalPopupExtender3.Show();
        }

        protected void imgStartDateSelectionEdit_click(object sender, EventArgs e)
        {
            Calendar calStartDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calStartDateEdit");
            calStartDateEdit.Visible = !calStartDateEdit.Visible;
            Panel Panel3 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel3");
            ModalPopupExtender ModalPopupExtender3 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel3.Parent, "ModalPopupExtender3");
            ModalPopupExtender3.Show();
        }

        protected void calStartDateEdit_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtStartDateEdit = (TextBox)AspUtilities.FindControlRecursive(this, "txtStartDateEdit");
            Calendar calStartDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calStartDateEdit");
            txtStartDateEdit.Text = calStartDateEdit.SelectedDate.ToString("d");
            calStartDateEdit.Visible = false;
            Panel Panel3 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel3");
            ModalPopupExtender ModalPopupExtender3 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel3.Parent, "ModalPopupExtender3");
            ModalPopupExtender3.Show();
        }

        protected void imgEndDateSelectionEdit_click(object sender, EventArgs e)
        {
            Calendar calEndDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calEndDateEdit");
            calEndDateEdit.Visible = !calEndDateEdit.Visible;
            Panel Panel3 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel3");
            ModalPopupExtender ModalPopupExtender3 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel3.Parent, "ModalPopupExtender3");
            ModalPopupExtender3.Show();
        }

        protected void calEndDateEdit_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtEndDateEdit = (TextBox)AspUtilities.FindControlRecursive(this, "txtEndDateEdit");
            Calendar calEndDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calEndDateEdit");
            txtEndDateEdit.Text = calEndDateEdit.SelectedDate.ToString("d");
            calEndDateEdit.Visible = false;
            Panel Panel3 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel3");
            ModalPopupExtender ModalPopupExtender3 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel3.Parent, "ModalPopupExtender3");
            ModalPopupExtender3.Show();
        }

        protected void btnAddNewSprint(object sender, EventArgs e)
        {
            
           // Check to make sure a sprint name exists
            if (txtSprint.Text == "" || txtSprint.Text == null)
            {
                lblMessage.Text = "A sprint name is required.";
                return;
            }

            //Validate sprint name
            string errorMessage = CTMethods.ValidateSprintExists(lblAddNewSprintProject.Text, txtSprint.Text, txtbxEditDescription.Text, false);

            if (errorMessage == null)
            {
                DateTime sprintStartDate = DateTime.Parse(txtStartDateInsert.Text);
                String formattedStart = sprintStartDate.ToSqlString();

                DateTime sprintEndDate = DateTime.Parse(txtEndDateInsert.Text);
                String formattedEnd = sprintEndDate.ToSqlString();

                string query = "INSERT INTO [Sprints] ([projectAbbreviation], [sprint], [sprintDescription], [sprintStartDate], [sprintEndDate]) VALUES ('" + lblAddNewSprintProject.Text + "', '" + txtSprint.Text + "', '" + txtbxEditDescription.Text + "', '" + formattedStart + "', '" + formattedEnd + "')";

                DatabaseUtilities.ExecuteQuery(query);

            }
            else
            {
                lblMessage.Text = errorMessage;
            }

            // this "announces" the event, or "raises" the event. This shouts out "hey, anyone listening, this thingamajig happened!"
            SprintReadyForDatabindEvent(this, new EventArgs());
        }
    }
}
