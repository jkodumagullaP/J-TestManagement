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
    public partial class AddNewRelease : System.Web.UI.UserControl
    {
        //Declare the event created to force a databind on the parent page (as a default EventHandler)
        public event EventHandler ReleaseReadyForDatabindEvent;
        
        // if you wanted to customize the parameters, you'd declare your own event handler like so (delegate is the same thing as eventhandler)
        //public delegate void NoArgsEventHandler();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (HttpContext.Current.Session["CurrentProject"] != null)
            {
                lblAddNewReleaseProject.Text = HttpContext.Current.Session["CurrentProject"].ToString();
            }
            else
            {
                // Get logged in user's default project
                string defaultProject = CTMethods.GetDefaultProject(HttpContext.Current.User.Identity);
                if (defaultProject != null)
                {
                    lblAddNewReleaseProject.Text = defaultProject;
                }
            }
        }

        protected void imgCalDateSelectionInsert_click(object sender, EventArgs e)
        {
            Calendar calDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calDateInsert");
            calDateInsert.Visible = !calDateInsert.Visible;
            Panel Panel2 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel2");
            ModalPopupExtender ModalPopupExtender2 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel2.Parent, "ModalPopupExtender2");
            ModalPopupExtender2.Show();

        }

        protected void calDateInsert_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtDateInsert = (TextBox)AspUtilities.FindControlRecursive(this, "txtDateInsert");
            Calendar calDateInsert = (Calendar)AspUtilities.FindControlRecursive(this, "calDateInsert");
            txtDateInsert.Text = calDateInsert.SelectedDate.ToString("d");
            calDateInsert.Visible = false;
            Panel Panel2 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel2");
            ModalPopupExtender ModalPopupExtender2 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel2.Parent, "ModalPopupExtender2");
            ModalPopupExtender2.Show();

        }

        protected void imgCalDateSelectionEdit_click(object sender, EventArgs e)
        {
            Calendar calDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calDateEdit");
            calDateEdit.Visible = !calDateEdit.Visible;
            Panel Panel2 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel2");
            ModalPopupExtender ModalPopupExtender2 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel2.Parent, "ModalPopupExtender2");
            ModalPopupExtender2.Show();

        }

        protected void calDateEdit_SelectionChanged(object sender, EventArgs e)
        {
            TextBox txtDateEdit = (TextBox)AspUtilities.FindControlRecursive(this, "txtDateEdit");
            Calendar calDateEdit = (Calendar)AspUtilities.FindControlRecursive(this, "calDateEdit");
            txtDateEdit.Text = calDateEdit.SelectedDate.ToString("d");
            calDateEdit.Visible = false;
            Panel Panel2 = (Panel)AspUtilities.FindControlRecursive(this.Parent, "Panel2");
            ModalPopupExtender ModalPopupExtender2 = (ModalPopupExtender)AspUtilities.FindControlRecursive(Panel2.Parent, "ModalPopupExtender2");
            ModalPopupExtender2.Show();
        }

        protected void btnAddNewRelease(object sender, EventArgs e)
        {

            // Check to make sure a release name exists
            if (txtRelease.Text == "" || txtRelease.Text == null)
            {
                lblMessage.Text = "A release name is required.";
                return;
            }

            //Validate release name
            string errorMessage = CTMethods.ValidateSprintExists(lblAddNewReleaseProject.Text, txtRelease.Text, txtbxEditDescription.Text, false);

            if (errorMessage == null)
            {
                DateTime releaseDate = DateTime.Parse(txtDateInsert.Text);
                String formattedDate = releaseDate.ToSqlString();

                string query = "INSERT INTO [Releases] ([projectAbbreviation], [release], [releaseDescription], [releaseDate]) VALUES ('" + lblAddNewReleaseProject.Text + "','" + txtRelease.Text + "','" + txtbxEditDescription.Text + "', '" + formattedDate + "')";

                DatabaseUtilities.ExecuteQuery(query);

            }
            else
            {
                lblMessage.Text = errorMessage;
            }
            
            
            
            //string releaseDate = txtDateInsert.Text;
            //string releaseDescription = txtbxEditDescription.Text;

            //// A using statement will make the contained object exist only for the duration of
            //// the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            //using (System.Data.SqlClient.SqlConnection Conn =
            //    new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            //{
            //    Conn.Open();

            //    using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
            //    {
            //        cmd.CommandText = "INSERT INTO [Releases] ([projectAbbreviation], [release], [releaseDescription], [releaseDate]) VALUES ('" + lblAddNewReleaseProject.Text + "','" + txtRelease.Text + "','" + releaseDescription + "'," + releaseDate + ")";
            //            cmd.CommandType = System.Data.CommandType.Text;
            //            cmd.ExecuteNonQuery();
            //    }
            //}

            // this "announces" the event, or "raises" the event. This shouts out "hey, anyone listening, this thingamajig happened!"
            ReleaseReadyForDatabindEvent(this, new EventArgs());
        }
    }
}
