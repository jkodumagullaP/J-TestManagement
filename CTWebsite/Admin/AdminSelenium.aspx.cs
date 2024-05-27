using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using System.Configuration;
using System.Management;
using CTInfrastructure;
using System.Threading;

namespace CTWebsite.Admin
{
    public partial class AdminSelenium : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

            SqlDataSource sqlSeleniumServers = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlSeleniumServers");
            sqlSeleniumServers.SelectCommand = "SELECT * FROM SeleniumServers WHERE (serverEnvironment= '" + TestRunEnvironment + "') AND (ServerRole = 'Node') ORDER BY serverName ASC";

            sqlSeleniumServers.DataBind();

            SqlDataSource sqlSeleniumHUBs = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlSeleniumHUBs");
            sqlSeleniumHUBs.SelectCommand = "SELECT * FROM SeleniumServers WHERE (serverEnvironment= '" + TestRunEnvironment + "') AND (ServerRole = 'HUB') ORDER BY serverName ASC";

            sqlSeleniumHUBs.DataBind();

            SqlDataSource sqlAutomationTestEngines = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlAutomationTestEngines");
            sqlAutomationTestEngines.SelectCommand = "SELECT * FROM SeleniumServers WHERE (serverEnvironment= '" + TestRunEnvironment + "') AND (ServerRole = 'AutomationTestEngine') ORDER BY serverName ASC";

            sqlAutomationTestEngines.DataBind();
        }

        public void RefreshNodeStatuses()
        {
            List<Label> lblServerStatuses = AspUtilities.FindMultipleControlsRecursive<Label>(this, "lblServerStatus");

            foreach (Label lblServerStatus in lblServerStatuses)
            {
                int seleniumServerID = Convert.ToInt32(lblServerStatus.ToolTip);

                string hostName = DatabaseUtilities.GetTextFromQuery("Select ServerName from SeleniumServers where SeleniumServerID = " + seleniumServerID);

                lblServerStatus.Text = CTTestGridMethods.GetNodeStatus(hostName);

                if (lblServerStatus.Text == "OFFLINE")
                {
                    lblMessage.Text = "The process of rebooting the Selenium Grid has been started, you will have to refesh until all the nodes show as Running. Also note that as long as any node is offline, this screen will take an unusually long time to refresh.";
                }
            }
        }

        public void RefreshHUBStatus()
        {
            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

            SqlDataSource sqlSeleniumHUBs = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlSeleniumHUBs");
            string hubName = DatabaseUtilities.GetTextFromQuery("SELECT servername FROM SeleniumServers WHERE (serverEnvironment= '" + TestRunEnvironment + "') AND (ServerRole = 'HUB')");

            Label lblHubHostName = (Label)AspUtilities.FindControlRecursive(this, "lblHubHostName");
            if (lblHubHostName != null)
            {
                lblHubHostName.Text = hubName;
            }

            Label lblHubStatus = (Label)AspUtilities.FindControlRecursive(this, "lblHubStatus");
            if (lblHubHostName != null)
            {
                lblHubStatus.Text = CTTestGridMethods.GetHubStatus(hubName);

                if (lblHubStatus.Text == "OFFLINE")
                {
                    lblMessage.Text = "The process of rebooting the Selenium Grid has been started, you will have to refesh until all the nodes show as Running. Also note that as long as any node is offline, this screen will take an unusually long time to refresh.";
                }
            }
        }

        public void RefreshAutomationTestEngineStatus()
        {
            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

            SqlDataSource sqlAutomationTestEngines = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlAutomationTestEngines");
            string engineName = DatabaseUtilities.GetTextFromQuery("SELECT servername FROM SeleniumServers WHERE (serverEnvironment= '" + TestRunEnvironment + "') AND (ServerRole = 'AutomationTestEngine')");

            Label lblAutomationTestEngineName = (Label)AspUtilities.FindControlRecursive(this, "lblAutomationTestEngineName");
            if (lblAutomationTestEngineName != null)
            {
                lblAutomationTestEngineName.Text = engineName;
            }

            Label lblAutomationTestEngineStatus = (Label)AspUtilities.FindControlRecursive(this, "lblAutomationTestEngineStatus");
            if (lblAutomationTestEngineName != null)
            {
                lblAutomationTestEngineStatus.Text = CTTestGridMethods.GetEngineStatus(engineName);

                if (lblAutomationTestEngineStatus.Text == "OFFLINE")
                {
                    lblMessage.Text = "The process of rebooting the Selenium Grid has been started, you will have to refesh until all the nodes show as Running. Also note that as long as any node is offline, this screen will take an unusually long time to refresh.";
                }
            }
        }


        protected void imgViewHubConsole_OnClick(object sender, ImageClickEventArgs e)
        {
            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];
            string hubURL = ConfigurationManager.AppSettings["hubURL"];

            if (TestRunEnvironment != "LOCAL")
            {
                Response.Redirect("http://" + hubURL + "/grid/console");
            }
            else
            {
                Response.Redirect("http://localhost:4444/grid/console");
            }
        }

        protected void imgViewAutomationTestEngineConsole_OnClick(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/Admin/Console.aspx?type=TestEngine");
        }
        
        protected void imgViewNodeConsole_OnClick(object sender, EventArgs e)
        {
            int seleniumServerID = Convert.ToInt32(((ImageButton)sender).CommandArgument);

            string hostName = DatabaseUtilities.GetTextFromQuery("Select ServerName from SeleniumServers where SeleniumServerID = " + seleniumServerID);

            Response.Redirect("~/Admin/Console.aspx?type=Node&hostname=" + hostName);
        }

        
        protected void imgServerStart_OnClick(object sender, EventArgs e)
        {
            int seleniumServerID = Convert.ToInt32(((ImageButton)sender).CommandArgument);

            string hostName = DatabaseUtilities.GetTextFromQuery("Select ServerName from SeleniumServers where SeleniumServerID = " + seleniumServerID);

            // Sleep for 5 seconds before starting, so the user can't stop-then-start too quickly
            Thread.Sleep(5000);

            CTTestGridMethods.StartNode(hostName);

            lblMessage.Text = hostName + " has been started.";

            RefreshNodeStatuses();
        }
        
        protected void imgServerStop_OnClick(object sender, EventArgs e)
        {
            int seleniumServerID = Convert.ToInt32(((ImageButton)sender).CommandArgument);

            DatabaseUtilities.ExecuteQuery("Update TestResults set status = 'Retest', reasonForStatus='The Selenium Node was shut down during the test. This test will need to be restarted.' where seleniumserverid = " + seleniumServerID + " and status in ('In Progress', 'In Queue')");
            
            string hostName = DatabaseUtilities.GetTextFromQuery("Select ServerName from SeleniumServers where SeleniumServerID = " + seleniumServerID);

            CTTestGridMethods.StopNode(hostName);

            lblMessage.Text = hostName + " has been stopped.";

            RefreshNodeStatuses();
        }

        protected void imgServerRefresh_OnClick(object sender, EventArgs e)
        {
            int seleniumServerID = Convert.ToInt32(((ImageButton)sender).CommandArgument);

            DatabaseUtilities.ExecuteQuery("Update TestResults set status = 'Retest', reasonForStatus='The Selenium Node was shut down during the test. This test will need to be restarted.' where seleniumserverid = " + seleniumServerID + " and status in ('In Progress', 'In Queue')");

            string hostName = DatabaseUtilities.GetTextFromQuery("Select ServerName from SeleniumServers where SeleniumServerID = " + seleniumServerID);

            CTTestGridMethods.StopNode(hostName);

            // Sleep for 10 seconds before restarting
            Thread.Sleep(10000);

            CTTestGridMethods.StartNode(hostName);

            lblMessage.Text = hostName + " has been refreshed.";

            RefreshNodeStatuses();
        }


        protected void imgEngineStart_OnClick(object sender, EventArgs e)
        {
            int seleniumServerID = Convert.ToInt32(((ImageButton)sender).CommandArgument);

            string hostName = DatabaseUtilities.GetTextFromQuery("Select ServerName from SeleniumServers where SeleniumServerID = " + seleniumServerID);

            CTTestGridMethods.StartEngine(hostName);

            Response.Redirect(Request.RawUrl);
            return;
        }

        protected void imgEngineStop_OnClick(object sender, EventArgs e)
        {
            int seleniumServerID = Convert.ToInt32(((ImageButton)sender).CommandArgument);

            string hostName = DatabaseUtilities.GetTextFromQuery("Select ServerName from SeleniumServers where SeleniumServerID = " + seleniumServerID);

            CTTestGridMethods.StopEngine(hostName);

            Response.Redirect(Request.RawUrl);
            return;
        }

        protected void imgEngineRefresh_OnClick(object sender, EventArgs e)
        {
            int seleniumServerID = Convert.ToInt32(((ImageButton)sender).CommandArgument);

            string hostName = DatabaseUtilities.GetTextFromQuery("Select ServerName from SeleniumServers where SeleniumServerID = " + seleniumServerID);

            CTTestGridMethods.StopEngine(hostName);

            CTTestGridMethods.StartEngine(hostName);

            Response.Redirect(Request.RawUrl);
            return;
        }

        protected void AutomationTestEngine_DataBound(object sender, EventArgs e)
        {
            RefreshAutomationTestEngineStatus();
        }

        protected void ListView1_DataBound(object sender, EventArgs e)
        {
            RefreshHUBStatus();
        }

        protected void lvSeleniumServers_DataBound(object sender, EventArgs e)
        {
            RefreshNodeStatuses();
        }

        protected void btnResetWebsite_Click(object sender, EventArgs e)
        {
            DatabaseUtilities.ExecuteQuery("Update TestResults set status = 'Retest', reasonForStatus='The QAA test website was restarted during the test. This test will need to be restarted.' where status in ('In Progress', 'In Queue')");

            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

            if (TestRunEnvironment == "LOCAL")
            {
                HttpRuntime.UnloadAppDomain();
            }
            else
            {
                if (Constants.QAANetworkDomain.ToString() != "")
                {
                    using (new Impersonator(Constants.QAANetworkDomain.ToString(), Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                    {
                        HttpRuntime.UnloadAppDomain();
                    }
                }
                else
                {
                    using (new Impersonator(Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                    {
                        HttpRuntime.UnloadAppDomain();
                    }
                }
            }
        }

        protected void btnResetSeleniumGrid_Click(object sender, EventArgs e)
        {
            ResetSeleniumGrid(false);
        }

        protected void btnResetSeleniumGridWithReboot_Click(object sender, EventArgs e)
        {
            ResetSeleniumGrid(true);
        }

        protected void ResetSeleniumGrid(bool withReboot)
        {
            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

            string hubName = DatabaseUtilities.GetTextFromQuery("SELECT servername FROM SeleniumServers WHERE (serverEnvironment= '" + TestRunEnvironment + "') AND (ServerRole = 'HUB')");
            List<string> nodeNames = DatabaseUtilities.GetStringListFromQuery("SELECT ServerName FROM SeleniumServers WHERE (serverEnvironment= '" + TestRunEnvironment + "') AND (ServerRole = 'Node') ORDER BY serverName ASC");

            DatabaseUtilities.ExecuteQuery("Update TestResults set status = 'Retest', reasonForStatus='The Selenium Grid was shut down during the test. This test will need to be restarted.' where status in ('In Progress', 'In Queue')");

            // Stop all the nodes (this includes stopping the browsers)
            foreach (string nodeName in nodeNames)
            {
                CTTestGridMethods.StopNode(nodeName);
            }

            // Stop any stray Java.exe instances that didn't get shut down with the services
            foreach (string nodeName in nodeNames)
            {
                CTTestGridMethods.TerminateJava(nodeName);
            }

            // Stop the HUB
            CTTestGridMethods.StopHUB(hubName);

            // Stop any stray Java.exe instances that didn't get shut down with the services
            CTTestGridMethods.TerminateJava(hubName);


            // Reboot the HUB completely
            if (withReboot)
            //if (withReboot && (hubName.ToLower() != "localhost"))
            {
                CTTestGridMethods.RebootComputer(hubName);

                // Sleep for 90 seconds to let the hub reboot (or at least get a decent head start on the nodes)
                Thread.Sleep(90 * 1000);

                foreach (string nodeName in nodeNames)
                {
                    CTTestGridMethods.RebootComputer(nodeName);
                }

                // Sleep for 3 minutes to let the nodes reboot
                //Thread.Sleep(3 * 60 * 1000);

                Response.Redirect(Request.RawUrl);
                return;
            }
            else
            {
                CTTestGridMethods.StartHUB(hubName);

                // Sleep for 5 seconds before restarting the nodes
                Thread.Sleep(5 * 1000);

                foreach (string nodeName in nodeNames)
                {
                    CTTestGridMethods.StartNode(nodeName);
                }

                lblMessage.Text = "The entire Selenium Grid has been refreshed.";
                
                RefreshHUBStatus();
                RefreshNodeStatuses();
            }
        }

    }
}