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

namespace CTWebsite.Admin
{
    public partial class AdminProjects : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Button btnAddExampleData = (Button)AspUtilities.FindControlRecursive(this, "btnAddExampleData");
            Button btnRemoveExampleData = (Button)AspUtilities.FindControlRecursive(this, "btnRemoveExampleData");
            
            try
            {
                string query = "SELECT projectAbbreviation FROM PROJECTS WHERE projectAbbreviation = 'EXAMPLE'";
                string example = DatabaseUtilities.GetTextFromQuery(query);

                if (example != null)
                {
                    btnAddExampleData.Visible = false;
                    btnRemoveExampleData.Visible = true;
                }
            }
            catch (Exception)
            {
                btnAddExampleData.Visible = true;
                btnRemoveExampleData.Visible = false;
            }
        }

        protected void btnCreateNewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/CreateNewProject.aspx");
        }

        protected void btnAddExampleData_Click(object sender, EventArgs e)
        {
            DatabaseUtilities.ExecuteNoParameterStoredProcedure("sp_AddSampleData");
            Response.Redirect(Request.RawUrl);
        }

        protected void btnRemoveExampleData_Click(object sender, EventArgs e)
        {
            DatabaseUtilities.ExecuteNoParameterStoredProcedure("sp_RemoveSampleData");
            Response.Redirect(Request.RawUrl);
        }

        protected void btnResetCrystalTest_Click(object sender, EventArgs e)
        {
            DatabaseUtilities.ExecuteNoParameterStoredProcedure("sp_DANGER_RESET_CT_DB");
            Response.Redirect(Request.RawUrl);
        }
    }
} 