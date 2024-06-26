﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CTWebsite.WebDriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using FormKeys = System.Windows.Forms.Keys;
using CTWebsite.UI.SwdMain.Popups;


namespace CTWebsite.UI
{
    public partial class SwdMainView : Form, IView
    {
        private SwdMainPresenter presenter = null;
        private System.Threading.ManualResetEvent startedEvent;
        
        public SwdMainView()
        {
            InitializeComponent();
            string versionText = string.Format("SWD Page Recorder {0} (Build: {1})", Build.WebDriverVersion, Build.Version);
            this.Text = versionText;
           // MyLog.Write("Started: " + versionText);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

            presenter = Presenters.SwdMainPresenter;
            //presenter.InitView(this);
        }

        public SwdMainView(System.Threading.ManualResetEvent startedEvent) : this()
        {
            this.startedEvent = startedEvent;
        }


        private void txtBrowserUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == FormKeys.Enter)
            {
                presenter.SetBrowserUrl(txtBrowserUrl.Text);
            }
        }


        private void btnStartVisualSearch_Click(object sender, EventArgs e)
        {
            presenter.ChangeVisualSearchRunningState();
        }



        public void UpdateVisualSearchResult(string xPathAttributeValue)
        {

            var action = (MethodInvoker)delegate
            {
                txtVisualSearchResult.Text = xPathAttributeValue;
            };

            if (txtVisualSearchResult.InvokeRequired)
            {
                txtVisualSearchResult.Invoke(action);
            }
            else
            {
                action();
            }
        }

        public void SetUrlText(string browserUrl)
        {
            txtBrowserUrl.DoInvokeAction(() => txtBrowserUrl.Text = browserUrl);
        }

        public BrowserPageFrame getCurrentlyChosenFrame()
        {
            BrowserPageFrame frame = null;
            object item = null;
            this.ddlFrames.Invoke((MethodInvoker)delegate()
            {
                item = ddlFrames.SelectedItem;
            }
            );
            if (item is BrowserPageFrame)
            {
                frame = item as BrowserPageFrame;

            }
            return frame;
        }

        private void btnBrowser_Go_Click(object sender, EventArgs e)
        {
            presenter.SetBrowserUrl(txtBrowserUrl.Text);
        }


        public void VisualSearchStopped()
        {
            var action = (MethodInvoker)delegate
            {
                btnStartVisualSearch.Text = "Start";
            };

            if (btnStartVisualSearch.InvokeRequired)
            {
                btnStartVisualSearch.Invoke(action);
            }
            else
            {
                action();
            }
        }

        public void VisuaSearchStarted()
        {
            var action = (MethodInvoker)delegate
            {
                btnStartVisualSearch.Text = "Stop";
            };

            if (btnStartVisualSearch.InvokeRequired)
            {
                btnStartVisualSearch.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/dzharii/swd-recorder");
        }



        public void ShowGlobalLoading()
        {

            var action = (MethodInvoker)delegate
            {
                pnlLoadingBar.Visible = true;
            };

            if (pnlLoadingBar.InvokeRequired)
            {
                pnlLoadingBar.Invoke(action);
            }
            else
            {
                action();
            }
        }

        public void HideGlobalLoading()
        {
            var action = (MethodInvoker)delegate
            {
                pnlLoadingBar.Visible = false;
            };

            if (pnlLoadingBar.InvokeRequired)
            {
                pnlLoadingBar.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void SwdMainView_Shown(object sender, EventArgs e)
        {
            if (startedEvent != null) startedEvent.Set();
        }

        public void SetDriverDependingControlsEnabled(bool shouldControlBeEnabled)
        {
            txtBrowserUrl.DoInvokeAction(   () => txtBrowserUrl.Enabled = shouldControlBeEnabled);
            btnBrowser_Go.DoInvokeAction(   () =>  btnBrowser_Go.Enabled = shouldControlBeEnabled);
            btnTakePageScreenshot.DoInvokeAction(() => btnTakePageScreenshot.Enabled = shouldControlBeEnabled);
            btnOpenScreenshotFolder.DoInvokeAction(() => btnOpenScreenshotFolder.Enabled = shouldControlBeEnabled);
            grpVisualSearch.DoInvokeAction( () =>  grpVisualSearch.Enabled = shouldControlBeEnabled);
            grpSwitchTo.DoInvokeAction(     () => grpSwitchTo.Enabled = shouldControlBeEnabled);
        }

        public void UpdateBrowserWindowsList(BrowserWindow[] currentWindows, string currentWindowHandle)
        {
            ddlWindows.DoInvokeAction(() =>
            {
                ddlWindows.Items.Clear();
                ddlWindows.Items.AddRange(currentWindows);

                ddlWindows.SelectedItem = currentWindows.First(win => (win.WindowHandle == currentWindowHandle));
            });
        }

        public void UpdatePageFramesList(BrowserPageFrame[] currentPageFrames)
        {
            ddlFrames.DoInvokeAction(() =>
            {
                ddlFrames.Items.Clear();
                ddlFrames.Items.AddRange(currentPageFrames);

                ddlFrames.SelectedItem = currentPageFrames.First();
            });

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            presenter.RefreshSwitchToList();
        }

        public void SetInitialRefreshMessageForSwitchToControls()
        {
            ddlFrames.Enabled = false;
            ddlWindows.Enabled = false;

            ddlWindows.Text = "Press Refresh button";
            ddlFrames.Text = "... please";
        }

        public void EnableSwitchToControls()
        {
            ddlFrames.Enabled = true;
            ddlWindows.Enabled = true;
        }

        public void DisableSwitchToControls()
        {
            ddlFrames.Enabled = false;
            ddlWindows.Enabled = false;
        }

        private void ddlFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            BrowserPageFrame frame = ddlFrames.SelectedItem as BrowserPageFrame;
            presenter.SwitchToFrame(frame);
        }

        private void ddlWindows_SelectedIndexChanged(object sender, EventArgs e)
        {
            BrowserWindow window = ddlWindows.SelectedItem as BrowserWindow;
            presenter.SwitchToWindow(window);
        }

        public void DisableWebElementExplorerRunButton()
        {
            btnStartVisualSearch.DoInvokeAction(() =>
            {
                btnStartVisualSearch.Enabled = false;

            });
        }

        public void EnableWebElementExplorerRunButton()
        {
            btnStartVisualSearch.DoInvokeAction(() =>
            {
                btnStartVisualSearch.Enabled = true;

            });
        }

        public void DisableWebElementExplorerResultsField()
        {
            txtVisualSearchResult.DoInvokeAction(() =>
            {
                txtVisualSearchResult.Enabled = false;

            });
        }

        public void EnableWebElementExplorerResultsField()
        {
            txtVisualSearchResult.DoInvokeAction(() =>
            {
                txtVisualSearchResult.Enabled = true;

            });

        }

        private void btnSwitchToFrameCode_Click(object sender, EventArgs e)
        {
            BrowserPageFrame frame = ddlFrames.SelectedItem as BrowserPageFrame;
            presenter.OpenSwitchToFrameCodeHelperPopup(frame);
        }

        private async void btnTakePageScreenshot_Click(object sender, EventArgs e)
        {
            btnTakePageScreenshot.Enabled = false;
            var oldBackground = btnTakePageScreenshot.BackColor;
            btnTakePageScreenshot.BackColor = Color.DarkGray;
            try
            {
                presenter.DisplayLoadingIndicator(true);
                await presenter.TakeAndSaveScreenshot();
            }
            finally 
            {
                btnTakePageScreenshot.Enabled = true;
                btnTakePageScreenshot.BackColor = oldBackground;
                presenter.DisplayLoadingIndicator(false);
            }
        }

        private void btnOpenScreenshotFolder_Click(object sender, EventArgs e)
        {
            presenter.OpenScreenshotsFolder();
        }

        private void btnOpenBrowserPreview_Click(object sender, EventArgs e)
        {
            var browserScreenView = new BrowserScreenView();
            browserScreenView.Show();
        }

    }
}
