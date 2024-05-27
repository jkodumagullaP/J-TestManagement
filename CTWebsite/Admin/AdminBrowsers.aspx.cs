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
    public partial class AdminBrowsers : System.Web.UI.Page
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
                gvProjectSupportedBrowsers.DataBind();
            }
        }


        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
        }


        protected void gvProjectSupportedBrowsers_RowCommand(object sender, GridViewCommandEventArgs e) 
        {
            /*  
             * CommandPieces[0] = projectAbbreviation
             * CommandPieces[1] = groupTestAbbreviation
             * CommandPieces[2] = show

            */
            if (e.CommandName == "Toggle")
            {
                string commandArgument = (e.CommandArgument ?? "").ToString();
                string[] commandPieces = commandArgument.Split(new[] { '|' });

                string projectAbbreviation = commandPieces[0];
                string browserAbbreviation = commandPieces[1];
                bool showBrowserColumn = Convert.ToBoolean(commandPieces[2]);

                CTMethods.ToggleProjectSupportedBrowser(projectAbbreviation, browserAbbreviation, showBrowserColumn);

                gvProjectSupportedBrowsers.DataBind();
            }
        }
        
        protected void gvProjectSupportedBrowsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton imgbtnBrowserToggle = (ImageButton)AspUtilities.FindControlRecursive(e.Row, "imgbtnBrowserToggle");

                if (imgbtnBrowserToggle.CommandName == "Toggle")
                {
                    string commandArgument = (imgbtnBrowserToggle.CommandArgument ?? "").ToString();
                    string[] commandPieces = commandArgument.Split(new[] { '|' });

                    string projectAbbreviation = commandPieces[0];
                    string browserAbbreviation = commandPieces[1];
                    bool showBrowserColumn = Convert.ToBoolean(commandPieces[2]);


                    if (showBrowserColumn)
                    {
                        imgbtnBrowserToggle.ImageUrl = "~/Images/onbutton.png";
                    }
                    else
                    {
                        imgbtnBrowserToggle.ImageUrl = "~/Images/offbutton.png";
                    }
                }
            }
        }
    }
} 