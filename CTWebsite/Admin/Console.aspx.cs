using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using System.IO;
using System.Configuration;

namespace CTWebsite.Admin
{
    public partial class Console : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String type = Request.QueryString["type"];
            String hostname = Request.QueryString["hostname"];

            string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];
            string TestEngineOutputPath = ConfigurationManager.AppSettings["TestEngineOutputPath"];

            if (type == "TestEngine")
            {
                pnlConsole.GroupingText = "Console for Test Engine";

                if (TestRunEnvironment == "LOCAL")
                {
                    using (System.IO.StreamReader myFile = new System.IO.StreamReader(new FileStream(TestEngineOutputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        txtServerConsole.Text = myFile.ReadToEnd();
                    }
                }
                else
                {

                    if (Constants.QAANetworkDomain.ToString() != "")
                    {
                        using (new Impersonator(Constants.QAANetworkDomain.ToString(), Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                        {
                            using (System.IO.StreamReader myFile = new System.IO.StreamReader(new FileStream(TestEngineOutputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                            {
                                txtServerConsole.Text = myFile.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        using (new Impersonator(Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                        {
                            using (System.IO.StreamReader myFile = new System.IO.StreamReader(new FileStream(TestEngineOutputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                            {
                                txtServerConsole.Text = myFile.ReadToEnd();
                            }
                        }
                    }
                }
            }
            else
            {
                pnlConsole.GroupingText = "Console for " + hostname;

                if (TestRunEnvironment == "LOCAL")
                {
                    using (System.IO.StreamReader myFile = new System.IO.StreamReader(new FileStream(@"\\" + hostname + @"\c$\Development\SeleniumServer\" + type + @"Output.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        txtServerConsole.Text = myFile.ReadToEnd();
                    }
                }
                else
                {
                    if (Constants.QAANetworkDomain.ToString() != "")
                    {
                        using (new Impersonator(Constants.QAANetworkDomain.ToString(), Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                        {
                            using (System.IO.StreamReader myFile = new System.IO.StreamReader(new FileStream(@"\\" + hostname + @"\c$\Development\SeleniumServer\" + type + @"Output.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                            {
                                txtServerConsole.Text = myFile.ReadToEnd();
                            }
                        }
                    }
                    else 
                    {
                        using (new Impersonator(Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                        {
                            using (System.IO.StreamReader myFile = new System.IO.StreamReader(new FileStream(@"\\" + hostname + @"\c$\Development\SeleniumServer\" + type + @"Output.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                            {
                                txtServerConsole.Text = myFile.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}