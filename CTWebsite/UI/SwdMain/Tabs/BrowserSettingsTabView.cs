using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using CTWebsite.WebDriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

//using FormKeys = System.Windows.Forms.Keys;
using CTWebsite.ConfigurationManagement.Profiles;
using CTWebsite.UI;

namespace CTWebsite.UI
{
    public partial class BrowserSettingsTabView //: UserControl, IView
    {
        public BrowserSettingsTabPresenter Presenter {get; private set;}
        //private Control[] driverControls;
                
        public BrowserSettingsTabView()
        {
            InitializeComponent();
          //  Presenter = Presenters.BrowserSettingsTabPresenter;
            Presenter.InitWithView(this);
                        
            HandleRemoteDriverSettingsEnabledStatus();

            //driverControls = new Control[] { chkUseRemoteHub, grpRemoteConnection, ddlBrowserToStart };

            SetDesiredCapsAvailability(false);
            Presenter.InitDesiredCapabilities();


        }

        private void SetDesiredCapsAvailability(bool enabled)
        {
           // grpDesiredCaps.DoInvokeAction( () => grpDesiredCaps.Enabled = enabled);
        }

        private void btnStartWebDriver_Click(object sender, EventArgs e)
        {
            //var isRemoteDriver = chkUseRemoteHub.Checked;
            //var startSeleniumServerIfNotStarted = chkAutomaticallyStartServer.Checked;
            //var shouldMaximizeBrowserWindow = chkMaximizeBrowserWindow.Checked;

            //Profile selectedProfile = ddlBrowserToStart.SelectedItem as Profile;


            var browserOptions = new WebDriverOptions()
            {
                //BrowserProfile = selectedProfile,
                //IsRemote = isRemoteDriver,
                //RemoteUrl = txtRemoteHubUrl.Text,
            };


           // Presenter.StartNewBrowser(browserOptions, startSeleniumServerIfNotStarted, shouldMaximizeBrowserWindow); 
        }

        private void HandleRemoteDriverSettingsEnabledStatus()
        {
            //grpRemoteConnection.DoInvokeAction(
            //        () => grpRemoteConnection.Enabled = chkUseRemoteHub.Checked); 

            //ChangeBrowsersList(chkUseRemoteHub.Checked);
        }

        private void ChangeBrowsersList(bool showIWebDriverProfiles)
        {

            //Profile selectedItem = ddlBrowserToStart.SelectedItem as Profile;
            //Profile previousValue = null;

            //if (selectedItem != null)
            //{
            //    previousValue = selectedItem;
            //}

            //ddlBrowserToStart.Items.Clear();

            //Profile[] addedItems = null;
            //if (showIWebDriverProfiles)
            //{
            //    addedItems = Presenter.GetIWebDriverProfiles();
            //}
            //else
            //{
            //    addedItems = Presenter.GetLocalWebdriverProfiles();
            //}
            //ddlBrowserToStart.Items.AddRange(addedItems);

            //if (ddlBrowserToStart.Items.Count > 0)
            //{
            //    int index = TryFindPreviousIndex(previousValue, addedItems);
            //    index = index >= 0 ? index : 0;
            //    ddlBrowserToStart.SelectedIndex = index;
            //}
        }

        private int TryFindPreviousIndex(Profile previousValue, Profile[] addedItems)
        {
            int index = -1;
            if (previousValue == null) return index;
            if (addedItems == null) return index;

            for (var i = 0; i < addedItems.Length; i++)
            {
                if (previousValue.DisplayName != null && addedItems[i].DisplayName == previousValue.DisplayName)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private void chkUseRemoteHub_CheckedChanged(object sender, EventArgs e)
        {
            HandleRemoteDriverSettingsEnabledStatus();
        }



        private void SetControlsState(string startButtonCaption, bool isEnabled)
        {
            //btnStartWebDriver.DoInvokeAction(() => btnStartWebDriver.Text = startButtonCaption);

            //foreach (var control in driverControls)
            //{
            //    btnStartWebDriver.DoInvokeAction(() => control.Enabled = isEnabled);
            //}
            //HandleRemoteDriverSettingsEnabledStatus();
        }

        public void SetUseRemoteHubConnection(bool useRemoteHubConnection)
        {
            //chkUseRemoteHub.DoInvokeAction(() => chkUseRemoteHub.Checked = useRemoteHubConnection);
        }

        public void SetMaximizeBrowserWindow(bool maximizeBrowserWindow)
        {
            //ch//MaximizeBrowserWindow.DoInvokeAction(() => chkMaximizeBrowserWindow.Checked = maximizeBrowserWindow);
        }

        public void DriverIsStopping()
        {
            SetControlsState("Start", true);
            SetDesiredCapsAvailability(false);
        }

        public void DriverWasStarted()
        {
            SetControlsState("Stop", false);
            SetDesiredCapsAvailability(true);
        }

        public void SetRemoteHubUrl(string remoteHubUrl)
        {
           // txtRemoteHubUrl.DoInvokeAction(() => txtRemoteHubUrl.Text = remoteHubUrl);
        }

        public void SetRunSeleniumServerBatch(bool batchAutorun)
        {
           // chkAutomaticallyStartServer.DoInvokeAction(() => chkAutomaticallyStartServer.Checked = batchAutorun);
        }

        public void DisableDriverStartButton()
        {
           // btnStartWebDriver.DoInvokeAction( () =>  btnStartWebDriver.Enabled = false);
        }

        public void EnableDriverStartButton()
        {
           // btnStartWebDriver.DoInvokeAction(() => btnStartWebDriver.Enabled = true);
        }

        public void SetStatus(string status)
        {

            //lblWebDriverStatus.DoInvokeAction(() => lblWebDriverStatus.Text = status);
        }

        private void btnLoadCapabilities_Click(object sender, EventArgs e)
        {
            Presenter.LoadCapabilities();
        }

        private void btnTestRemoteHub_Click(object sender, EventArgs e)
        {
           // Presenter.TestRemoteHub(txtRemoteHubUrl.Text);
        }

        public void SetTestResult(string result, bool isOk)
        {
            //lblRemoteHubStatus.Text = result;
            //lblRemoteHubStatus.ForeColor = (isOk) ? Color.Green : Color.Red;
        }

        public void SetBrowserStartupSettings(WebDriverOptions browserOptions)
        {
            Action action = new Action(() =>
            {
              //  chkUseRemoteHub.Checked = browserOptions.IsRemote;

                //var index = ddlBrowserToStart.Items.IndexOf(browserOptions.BrowserProfile.DisplayName);

                //ddlBrowserToStart.SelectedIndex = index;

                //txtRemoteHubUrl.Text = browserOptions.RemoteUrl;
            });

            //if (this.InvokeRequired)
            //{
            //    this.Invoke(action);
            //}
            //else
            //{
            //    action();
            //}

        }

        public void ClickOnStartButton()
        {
         //   btnStartWebDriver.DoInvokeAction(() => btnStartWebDriver.PerformClick());
        }

        //private void lnkSeleniumDownloadPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    System.Diagnostics.Process.Start(@"http://docs.seleniumhq.org/download/");
        //}

        //public void DisableMaximizeBrowserChackBox()
        //{
        //    chkMaximizeBrowserWindow.DoInvokeAction(() =>
        //    {
        //        chkMaximizeBrowserWindow.Enabled = false;
        //    });
            
        //}

        //public void EnableMaximizeBrowserChackBox()
        //{
        //    chkMaximizeBrowserWindow.DoInvokeAction(() =>
        //    {
        //        chkMaximizeBrowserWindow.Enabled = true;
        //    });
        //}
    }
}
