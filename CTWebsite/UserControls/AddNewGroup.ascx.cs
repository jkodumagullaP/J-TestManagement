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
    public partial class AddNewGroup : System.Web.UI.UserControl
    {
        //Declare the event created to force a databind on the parent page (as a default EventHandler)
        public event EventHandler ReadyForDatabindEvent;
        
        // if you wanted to customize the parameters, you'd declare your own event handler like so (delegate is the same thing as eventhandler)
        //public delegate void NoArgsEventHandler();
        
        protected void Page_Load(object sender, EventArgs e)
        {

            CheckBox chkPersonalGroup = (CheckBox)AspUtilities.FindControlRecursive(this, "chkPersonalGroup");
            if (HttpContext.Current.Session["CurrentProject"] != null)
            {
                lblAddNewGroupProject.Text = HttpContext.Current.Session["CurrentProject"].ToString();
            }
            else
            {
                // Get logged in user's default project
                string defaultProject = CTMethods.GetDefaultProject(HttpContext.Current.User.Identity);
                if (defaultProject != null)
                {
                    lblAddNewGroupProject.Text = defaultProject;
                }
            }
        }

        protected void btnAddNewGroup(object sender, EventArgs e)
        {
            
            String testOwner = null;

            CheckBox chkPersonalGroup = (CheckBox)AspUtilities.FindControlRecursive(this, "chkPersonalGroup");
            if (chkPersonalGroup != null)
            {
                testOwner = chkPersonalGroup.Checked ? "'" + AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name) + "'" : null;
            }
            
            string projectAbbreviation = lblAddNewGroupProject.Text;
            string groupTestAbbreviation = txtGroupTestAbbreviation.Text;
            string groupTestName = txtGroupTestName.Text;
            string groupTestDescription = txtGroupTestDescription.Text;

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [GroupTests] ([projectAbbreviation], [groupTestAbbreviation], [groupTestName], [groupTestDescription], [personalGroupOwner]) VALUES ('" + projectAbbreviation + "','" + groupTestAbbreviation + "','" + groupTestName + "','" + groupTestDescription + "'," + (testOwner != null ?  testOwner : "null")+")";


                    
                    
                    cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                }
            }

            // this "announces" the event, or "raises" the event. This shouts out "hey, anyone listening, this thingamajig happened!"
            ReadyForDatabindEvent(this, new EventArgs());
        }
    }
}
