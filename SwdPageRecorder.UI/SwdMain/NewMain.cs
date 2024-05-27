using System;
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
using OpenQA.Selenium.Chrome;

namespace CTWebsite.UI.SwdMain
{
    public partial class NewMain : Form, IView
    {
        public IWebDriver driver1;

        SwdMainPresenter presenter = new SwdMainPresenter();
        private System.Threading.ManualResetEvent startedEvent;

        //private System.Threading.ManualResetEvent startedEvent;

        public NewMain()
        {
            InitializeComponent();
            string versionText = string.Format("SWD Page Recorder {0} (Build: {1})", Build.WebDriverVersion, Build.Version);
            this.Text = versionText;
           // MyLog.Write("Started: " + versionText);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

            presenter = Presenters.SwdMainPresenter;
            presenter.InitView(this);
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
        public bool wasBrowserStarted = false;
        // = new SwdBrowser();

        public void btnStartVisualSearch_Click(
            object sender, EventArgs e)
        {
            //   Presenter.StartNewBrowser(browserOptions, startSeleniumServerIfNotStarted, shouldMaximizeBrowserWindow);

            presenter.ChangeVisualSearchRunningState();


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

        public void SetDriverDependingControlsEnabled(bool shouldControlBeEnabled)
        {
            txtBrowserUrl.DoInvokeAction(() => txtBrowserUrl.Enabled = shouldControlBeEnabled);
            //btnBrowser_Go.DoInvokeAction(() => btnBrowser_Go.Enabled = shouldControlBeEnabled);
            //btnTakePageScreenshot.DoInvokeAction(() => btnTakePageScreenshot.Enabled = shouldControlBeEnabled);
            //btnOpenScreenshotFolder.DoInvokeAction(() => btnOpenScreenshotFolder.Enabled = shouldControlBeEnabled);
            //grpVisualSearch.DoInvokeAction(() => grpVisualSearch.Enabled = shouldControlBeEnabled);
            //grpSwitchTo.DoInvokeAction(() => grpSwitchTo.Enabled = shouldControlBeEnabled);
        }

        public BrowserPageFrame getCurrentlyChosenFrame()
        {
            BrowserPageFrame frame = null;
            object item = null;
            this.ddlFrames.Invoke((MethodInvoker)delegate ()
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


        private void txtBrowserUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == FormKeys.Enter)
            {
                presenter.SetBrowserUrl(txtBrowserUrl.Text);
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            SwdBrowser._driver = new ChromeDriver(@"C:\TestAutomation\exe");

        }

        private void NewMain_Load(object sender, EventArgs e)
        {

        }
    }
}
