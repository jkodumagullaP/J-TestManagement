using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using CTInfrastructure;
using CTWebsite.UserControls;
using OpenQA.Selenium.Chrome;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Threading;
using CTWebsite.WebDriver;
using CTWebsite.WebDriver.JsCommand;
using System.Threading.Tasks;
using CTWebsite.UI;
using System.Windows.Forms;
using System.IO;


//using CTWebsite.;

namespace CTWebsite.FunctionalTesting
{
    public partial class AddNewTestCase : System.Web.UI.Page, IView
    {
        public IWebDriver driver1;

        SwdMainPresenter presenter = new SwdMainPresenter();
        private System.Threading.ManualResetEvent startedEvent;

        protected global::System.Web.UI.WebControls.DropDownList ddlProject;
        protected global::System.Web.UI.WebControls.Label lblCreatedBy;
        protected global::System.Web.UI.WebControls.Label lblDateCreated;
        protected global::System.Web.UI.WebControls.Label MessageLabel;
        protected global::System.Web.UI.WebControls.TextBox txtCategory;

        public AddNewTestCase()
        {

            string versionText = string.Format("SWD Page Recorder {0} (Build: {1})", 0, 0);
            //this.Text = versionText;
            // MyLog.Write("Started: " + versionText);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

            presenter = Presenters.SwdMainPresenter;
        //    presenter.InitView(this);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Page.IsPostBack == false)
            {
                ddlProject = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlProject");

                //Initialize gridviews and dropdown boxes.
                if (ddlProject.Items.Count > 0)
                {
                    ddlProject.Items.Insert(0, "Select a Project");
                    ddlProject.Items[0].Value = "";
                    ddlProject.SelectedIndex = 0;
                }

                if (HttpContext.Current.Session["CurrentProject"] != null)
                {
                    ddlProject.SelectedValue = HttpContext.Current.Session["CurrentProject"].ToString();
                    SelectedProjectChanged();
                }
                else
                {
                    // Get logged in user's default project
                    string defaultProject = CTMethods.GetDefaultProject(HttpContext.Current.User.Identity);
                    if (defaultProject != null)
                    {
                        ddlProject.SelectedValue = defaultProject;
                        SelectedProjectChanged();
                    }
                }

                //Initialize gridviews and dropdown boxes.

                if (User.Identity.IsAuthenticated)
                {
                    System.Web.UI.WebControls.TextBox txtCategory = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtCategory");
                    System.Web.UI.WebControls.CheckBox chkActive = (System.Web.UI.WebControls.CheckBox)AspUtilities.FindControlRecursive(this, "chkActive");
                    System.Web.UI.WebControls.Label lblCreatedBy = (System.Web.UI.WebControls.Label)AspUtilities.FindControlRecursive(this, "lblCreatedBy");
                    System.Web.UI.WebControls.Label lblDateCreated = (System.Web.UI.WebControls.Label)AspUtilities.FindControlRecursive(this, "lblDateCreated");
                    System.Web.UI.WebControls.Label MessageLabel = (System.Web.UI.WebControls.Label)AspUtilities.FindControlRecursive(this, "MessageLabel");

                    lblCreatedBy.Text = AspUtilities.GetUserFullName(AspUtilities.RemovePrefixFromUserName(User.Identity.Name));
                    lblDateCreated.Text = DateTime.Now.ToString();
                    MessageLabel.Text = "";
                    txtCategory.Text = "Functional";
                    chkActive.Checked = true;

                }
                
                SetListsContentParams();

                HttpContext.Current.Session["screenshotDescriptionsAndUrls"] = new List<Pair>();

                RefreshScreenshotList();
            }

            // This line LISTENS to the event, a.k.a. subscribes to it, and says what method to call if it hears anything.
            AddNewGroup.ReadyForDatabindEvent += DoGroupDataBind;
            AddNewRelease.ReleaseReadyForDatabindEvent += DoReleaseDataBind;
            AddNewSprint.SprintReadyForDatabindEvent += DoSprintDataBind;
        }

        // This is the actual event handler. When it hears the event, it does something
        public void DoGroupDataBind(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.ListBox listBoxGroupTests = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxGroupTests");
            listBoxGroupTests.DataBind();
        }

        // This is the actual event handler. When it hears the event, it does something
        public void DoReleaseDataBind(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.ListBox listBoxReleases = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxReleases");
            listBoxReleases.DataBind();
        }

        // This is the actual event handler. When it hears the event, it does something
        public void DoSprintDataBind(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.ListBox listBoxSprints = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxSprints");
            listBoxSprints.DataBind();
        }

        public void SetListsContentParams()
        {
            // Groups
            SqlDataSource sqlGroupTests = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlGroupTests");
            if (sqlGroupTests.SelectParameters["username"] == null)
            {
                Parameter userParam2 = new Parameter();
                userParam2.Name = "username";
                userParam2.DefaultValue = AspUtilities.RemovePrefixFromUserName(User.Identity.Name);
                sqlGroupTests.SelectParameters.Add(userParam2);
            }

            if (sqlGroupTests.SelectParameters["projectAbbreviation"] == null)
            {
                Parameter projectAbbreviationParam = new Parameter();
                projectAbbreviationParam.Name = "projectAbbreviation";
                sqlGroupTests.SelectParameters.Add(projectAbbreviationParam);
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }
            else
            {
                Parameter projectAbbreviationParam = sqlGroupTests.SelectParameters["projectAbbreviation"];
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }

            System.Web.UI.WebControls.ListBox listBoxGroupTests = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxGroupTests");
            listBoxGroupTests.DataBind();


            // Releases
            SqlDataSource sqlReleases = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlReleases");

            if (sqlReleases.SelectParameters["projectAbbreviation"] == null)
            {
                Parameter projectAbbreviationParam = new Parameter();
                projectAbbreviationParam.Name = "projectAbbreviation";
                sqlReleases.SelectParameters.Add(projectAbbreviationParam);
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }
            else
            {
                Parameter projectAbbreviationParam = sqlReleases.SelectParameters["projectAbbreviation"];
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }

            System.Web.UI.WebControls.ListBox listBoxReleases = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxReleases");
            listBoxReleases.DataBind();



            // Sprints
            SqlDataSource sqlSprints2 = (SqlDataSource)AspUtilities.FindControlRecursive(this, "sqlSprints2");

            if (sqlSprints2.SelectParameters["projectAbbreviation"] == null)
            {
                Parameter projectAbbreviationParam = new Parameter();
                projectAbbreviationParam.Name = "projectAbbreviation";
                sqlSprints2.SelectParameters.Add(projectAbbreviationParam);
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }
            else
            {
                Parameter projectAbbreviationParam = sqlSprints2.SelectParameters["projectAbbreviation"];
                projectAbbreviationParam.DefaultValue = ddlProject.SelectedValue;
            }

            System.Web.UI.WebControls.ListBox listBoxSprints = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxSprints");
            listBoxSprints.DataBind();
        }

        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
            SelectedProjectChanged();
        }

        private void SelectedProjectChanged()
        {
            ddlProject = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlProject");


            SetListsContentParams();
        }


        protected void CancelButton_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/FunctionalTesting/TestCases.aspx");
        }

        protected void InsertButton_OnClick(object sender, EventArgs e)
        {
            DropDownList ddlProject = (DropDownList)AspUtilities.FindControlRecursive(this, "ddlProject");
            System.Web.UI.WebControls.TextBox txtbxEditDescription = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditDescription");
            System.Web.UI.WebControls.CheckBox chkActive = (System.Web.UI.WebControls.CheckBox)AspUtilities.FindControlRecursive(this, "chkActive");
            System.Web.UI.WebControls.TextBox txtbxEditTestCaseSteps = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditTestCaseSteps");
            System.Web.UI.WebControls.TextBox txtbxEditExpectedResults = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditExpectedResults");
            System.Web.UI.WebControls.TextBox txtbxEditNotes = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtbxEditNotes");
            System.Web.UI.WebControls.Label MessageLabel = (System.Web.UI.WebControls.Label)AspUtilities.FindControlRecursive(this, "MessageLabel");

            if (ddlProject.SelectedValue == "")
            {
                MessageLabel.Text = "Insert FAILED, please select a project";
                return;
            }

            if (txtbxEditDescription.Text == "")
            {
                MessageLabel.Text = "Insert FAILED, please enter a description";
                return;
            }

            if (txtbxEditTestCaseSteps.Text == "")
            {
                MessageLabel.Text = "Insert FAILED, please enter test case steps";
                return;
            }

            if (txtbxEditExpectedResults.Text == "")
            {
                MessageLabel.Text = "Insert FAILED, please enter expected results";
                return;
            }

            try
            {
                bool active = chkActive.Checked;
                
                int testCaseId = CTMethods.InsertTestCase
                    (
                        ddlProject.SelectedValue,
                        txtbxEditDescription.Text,
                        active,
                        false,
                        false,
                        txtbxEditTestCaseSteps.Text,
                        txtbxEditExpectedResults.Text,
                        txtbxEditNotes.Text,
                        AspUtilities.RemovePrefixFromUserName(User.Identity.Name),
                        "",
                        "",
                        null,
                        "Functional",
                        null,
                        null
                    );

                MessageLabel.Text = "Inserted TestCaseId " + testCaseId + " Successfully";

                txtbxEditDescription.Text = "";
                txtbxEditTestCaseSteps.Text = "";
                txtbxEditExpectedResults.Text = "";
                txtbxEditNotes.Text = "";

                // Groups
                System.Web.UI.WebControls.ListBox listBoxGroupTests = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxGroupTests");
                ListItemCollection listItemCollectionGroups = listBoxGroupTests.Items;
                foreach (ListItem listItem in listItemCollectionGroups)
                {
                    if (listItem.Selected)
                    {
                        CTMethods.AddSingleGroup(testCaseId, ddlProject.SelectedValue, listItem.Value);
                    }
                }

                // Releases
                System.Web.UI.WebControls.ListBox listBoxReleases = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxReleases");
                ListItemCollection listItemCollectionReleases = listBoxReleases.Items;
                foreach (ListItem listItem in listItemCollectionReleases)
                {
                    if (listItem.Selected)
                    {
                        CTMethods.AddSingleRelease(testCaseId, ddlProject.SelectedValue, listItem.Value);
                    }
                }

                // Releases
                System.Web.UI.WebControls.ListBox listBoxSprints = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxSprints");
                ListItemCollection listItemCollectionSprints = listBoxSprints.Items;
                foreach (ListItem listItem in listItemCollectionSprints)
                {
                    if (listItem.Selected)
                    {
                        CTMethods.AddSingleSprint(testCaseId, ddlProject.SelectedValue, listItem.Value);
                    }
                }

                //Screenshots
                if (HttpContext.Current.Session["screenshotDescriptionsAndUrls"] == null)
                {
                    HttpContext.Current.Session["screenshotDescriptionsAndUrls"] = new List<Pair>();
                }

                List<Pair> screenshotDescriptionsAndUrls = (List<Pair>)HttpContext.Current.Session["screenshotDescriptionsAndUrls"];

                foreach (Pair screenshotDescriptionsAndUrl in screenshotDescriptionsAndUrls)
                {
                    AddScreenshotForReal(screenshotDescriptionsAndUrl.Second.ToString(), screenshotDescriptionsAndUrl.First.ToString(), ddlProject.SelectedValue, testCaseId);
                }

                HttpContext.Current.Session["screenshotDescriptionsAndUrls"] = new List<Pair>();

                RefreshScreenshotList();
            }
            catch (Exception ex)
            {
                MessageLabel.Text = "Insert FAILED with error: \"" + ex.Message + "\"";
            }
        }

        protected void fvTestCaseDetails_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            // Use the Exception property to determine whether an exception
            // occurred during the insert operation.
            if (e.Exception == null)
            {
                // Use the AffectedRows property to determine whether the
                // record was inserted. Sometimes an error might occur that 
                // does not raise an exception, but prevents the insert
                // operation from completing.
                if (e.AffectedRows == 1)
                {
                    //MessageLabel.Text = "Record inserted successfully.";
                    //e.KeepInInsertMode = true;
                    //Response.Redirect("~/Admin/AdminTestCases.aspx?insert=Successful");
                    Response.Redirect("~/FunctionalTesting/TestCases.aspx");

                }
                else
                {
                    MessageLabel.Text = "An error occurred during the insert operation. This test case has not been added.";

                    // Use the KeepInInsertMode property to remain in insert mode
                    // when an error occurs during the insert operation.
                    e.KeepInInsertMode = true;
                }
            }
            else
            {
                // Insert the code to handle the exception.
                MessageLabel.Text = e.Exception.Message;

                // Use the ExceptionHandled property to indicate that the 
                // exception has already been handled.
                e.ExceptionHandled = true;
                e.KeepInInsertMode = true;
            }
        }

        protected void btnUploadScreenshot_Click(object sender, EventArgs e)
        {
            FileUpload imgUploadScreenshot = (FileUpload)AspUtilities.FindControlRecursive(this, "imgUploadScreenshot");
            System.Web.UI.WebControls.TextBox txtUploadImageDescription = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtUploadImageDescription");
            System.Web.UI.WebControls.Label lblStatus = (System.Web.UI.WebControls.Label)AspUtilities.FindControlRecursive(this, "lblStatus");
            
            String TestCaseScreenshotPath = ConfigurationManager.AppSettings["TestCaseScreenshotPath"];
            String TestResultScreenshotPath = ConfigurationManager.AppSettings["TestResultScreenshotPath"];

            if (imgUploadScreenshot.HasFile)
            {
                string rawURL = "~/Admin/UploadedTestCaseScreenshots/" + imgUploadScreenshot.FileName.ToString();

                string uploadedFileLocalPath = Server.MapPath(rawURL);
                string uploadedFilePath = ResolveClientUrl(rawURL);

                imgUploadScreenshot.SaveAs(uploadedFileLocalPath);

                AddScreenshot(uploadedFilePath, txtUploadImageDescription.Text);
            }
            else
            {
                lblStatus.Text = "Please select a file first";
                return;
            }
        }

        protected void btnAddImageUrl_Click(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.TextBox txtImageUrl = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtImageUrl");
            System.Web.UI.WebControls.TextBox txtImageUrlDescription = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtImageUrlDescription");
            AddScreenshot(txtImageUrl.Text, txtImageUrlDescription.Text);
        }

        protected void deleteScreenshotButton_Click(object sender, EventArgs e)
        {

            System.Web.UI.WebControls.ListBox listBoxScreenshots = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxScreenshots");

            if (HttpContext.Current.Session["screenshotDescriptionsAndUrls"] == null)
            {
                HttpContext.Current.Session["screenshotDescriptionsAndUrls"] = new List<Pair>();
            }

            List<Pair> screenshotDescriptionsAndUrls = (List<Pair>)HttpContext.Current.Session["screenshotDescriptionsAndUrls"];

            int index = listBoxScreenshots.SelectedIndex;

            if (index < 0)
            {
                return;
            }

            screenshotDescriptionsAndUrls.RemoveAt(index);

            HttpContext.Current.Session["screenshotDescriptionsAndUrls"] = screenshotDescriptionsAndUrls;

            RefreshScreenshotList();
        }

        private void AddScreenshot(string imageURL, string description)
        {
            System.Web.UI.WebControls.Label lblStatus = (System.Web.UI.WebControls.Label)AspUtilities.FindControlRecursive(this, "lblStatus");
            System.Web.UI.WebControls.TextBox txtImageUrl = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtImageUrl");
            System.Web.UI.WebControls.TextBox txtImageUrlDescription = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtImageUrlDescription");
            System.Web.UI.WebControls.TextBox txtUploadImageDescription = (System.Web.UI.WebControls.TextBox)AspUtilities.FindControlRecursive(this, "txtUploadImageDescription");
            
            if (String.IsNullOrEmpty(description))
            {
                lblStatus.Text = "Description is required";
                return;
            }

            if (!imageURL.ToLower().EndsWith(".jpg")
                 && !imageURL.ToLower().EndsWith(".gif")
                 && !imageURL.ToLower().EndsWith(".png"))
            {
                lblStatus.Text = "Screenshot URLs must end with .jpg, .png, or .gif";
                return;
            }

            lblStatus.Text = "";

            if (HttpContext.Current.Session["screenshotDescriptionsAndUrls"] == null)
            {
                HttpContext.Current.Session["screenshotDescriptionsAndUrls"] = new List<Pair>();
            }

            List<Pair> screenshotDescriptionsAndUrls = (List<Pair>)HttpContext.Current.Session["screenshotDescriptionsAndUrls"];

            screenshotDescriptionsAndUrls.Add(new Pair(description, imageURL));

            RefreshScreenshotList();

            txtImageUrl.Text = "";
            txtImageUrlDescription.Text = "";
            txtUploadImageDescription.Text = "";
        }

        private void RefreshScreenshotList()
        {
            System.Web.UI.WebControls.ListBox listBoxScreenshots = (System.Web.UI.WebControls.ListBox)AspUtilities.FindControlRecursive(this, "listBoxScreenshots");
            listBoxScreenshots.Items.Clear();

            if (HttpContext.Current.Session["screenshotDescriptionsAndUrls"] == null)
            {
                HttpContext.Current.Session["screenshotDescriptionsAndUrls"] = new List<Pair>();
            }

            List<Pair> screenshotDescriptionsAndUrls = (List<Pair>)HttpContext.Current.Session["screenshotDescriptionsAndUrls"];

            foreach (Pair screenshotDescriptionsAndUrl in screenshotDescriptionsAndUrls)
            {
                ListItem newScreenshot = new ListItem();
                newScreenshot.Text = screenshotDescriptionsAndUrl.First.ToString();
                newScreenshot.Value = screenshotDescriptionsAndUrl.Second.ToString();

                listBoxScreenshots.Items.Add(newScreenshot);
            }
        }

        private void AddScreenshotForReal(string imageURL, string description, string project, int testCaseId)
        {
            System.Web.UI.WebControls.Label lblStatus = (System.Web.UI.WebControls.Label)AspUtilities.FindControlRecursive(this, "lblStatus");

            try
            {
                bool created = CTMethods.AddSingleTestCaseScreenshotIfNeeded(testCaseId, project, imageURL, description);

                if (!created)
                {
                    lblStatus.Text = "This is a duplicate of an existing screenshot URL on this test case";
                    return;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Unexpected error: " + ex.Message;
                return;
            }
        }

        private void CheckChanged(string checkboxName)
        {
            System.Web.UI.WebControls.CheckBox chkSelected = (System.Web.UI.WebControls.CheckBox)AspUtilities.FindControlRecursive(this, checkboxName);
            if (chkSelected != null)
            {
                string testOwner = chkSelected.Checked ? "'" + AspUtilities.RemovePrefixFromUserName (HttpContext.Current.User.Identity.Name) + "'" : "null";

            }
        }
       // public IWebDriver _driver;
        protected void StartBrowser_Click(object sender, EventArgs e)
        {
            SwdBrowser._driver = new ChromeDriver(@"C:\TestAutomation\exe");

        }
      // private CTWebsite.UI.SwdMainPresenter presenter;//= new SwdPageRecorder.UI.SwdMainPresenter();
        protected void btnStartVisualSearch_Click(object sender, EventArgs e)
        {
            ChangeVisualSearchRunningState();

           // var command = SwdBrowser.GetNextCommand();
          //  txtVisualSearchResult.Text = getXPathCommand.XPathValue;
            //if (command is GetXPathFromElement)
            //{

            //    var getXPathCommand = command as GetXPathFromElement;
            ////    UpdateVisualSearchResult(getXPathCommand.XPathValue);
            //    txtVisualSearchResult.Text = getXPathCommand.XPathValue;
            //}

        }
        public void ChangeVisualSearchRunningState()
        {
            DisableWebElementExplorerRunButton();

            try
            {
                if (webElementExplorerStarted)
                {
                    StopVisualSearch();
                    DisableWebElementExplorerResultsField();
                }
                else
                {
                    StartVisualSearch();
                    EnableWebElementExplorerResultsField();
                }
            }
            finally
            {
                EnableWebElementExplorerRunButton();
            }
        }
        public List<string> str567 = null;
        public  void ProcessCommands()
        {
            var command = SwdBrowser.GetNextCommand();
            if (command is GetXPathFromElement)
            {
                var getXPathCommand = command as GetXPathFromElement;
                UpdateVisualSearchResult(getXPathCommand.XPathValue);
            }
            else if (command is AddElement)
            {
                var addElementCommand = command as AddElement;

                SimpleFrame simpleFrame;
                BrowserPageFrame browserPageFrame = getCurrentlyChosenFrame();
                if (browserPageFrame != null)
                {
                    List<string> childs = new List<string>();
                    string parentTitle;
                    if (browserPageFrame.ParentFrame != null)
                    {
                        parentTitle = browserPageFrame.ParentFrame.GetTitle();
                    }
                    else
                    {
                        parentTitle = "none";
                    }
                    if (browserPageFrame.ChildFrames != null)
                    {
                        foreach (BrowserPageFrame b in browserPageFrame.ChildFrames)
                        {
                            childs.Add(b.GetTitle());
                        }
                    }

                    simpleFrame = new SimpleFrame(browserPageFrame.Index, browserPageFrame.LocatorNameOrId, browserPageFrame.GetTitle(), parentTitle, childs);
                }
                else
                {
                    simpleFrame = new SimpleFrame(-1, "noFrameChosen", "noFrameChosen", "noFrameChosen", null);
                }

                bool emptyHtmlId = String.IsNullOrEmpty(addElementCommand.ElementId);
                //txtVisualSearchResult.Text = addElementCommand.ElementXPath;
              var   element = new WebElementDefinition()
                {
                    Name = addElementCommand.ElementCodeName,
                    HtmlId = addElementCommand.ElementId,
                  Xpath = addElementCommand.ElementXPath,
                    HowToSearch = (emptyHtmlId) ? LocatorSearchMethod.XPath : LocatorSearchMethod.Id,
                    Locator = (emptyHtmlId) ? addElementCommand.ElementXPath : addElementCommand.ElementId,
                    CssSelector = addElementCommand.ElementCssSelector,
                    frame = simpleFrame,
                };

                FileStream fs = new FileStream("D:\\Variables.txt", FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter("D:\\Variables.txt", true, Encoding.ASCII);
                string NextLine = element.HtmlTag + element.Name + element.HtmlId + element.Xpath + element.CssSelector;
                sw.Write(NextLine);
                sw.Close();
                //  str590 = element.HtmlId;
                bool addNew = true;
                Presenters.SelectorsEditPresenter.UpdateWebElementWithAdditionalProperties(element);
                Presenters.PageObjectDefinitionPresenter.UpdatePageDefinition(element, addNew);
            }
        }
        public string str590;
        public void Messagebox(string xMessage)
        {
            Response.Write("<script>alert('" + xMessage + "')</script>");
        }
        public void VisualSearch_UpdateSearchResult()
        {
            try
            {

              // // MyLog.Write("VisualSearch_UpdateSearchResult: Started");
                while (webElementExplorerStarted == true)
                {
                    if (!webElementExplorerThreadPaused)
                    {
                        try
                        {
                            if (!CTWebsite.WebDriver.SwdBrowser.IsVisualSearchScriptInjected())
                            {
                                //// MyLog.Write("VisualSearch_UpdateSearchResult: Found the Visual search is not injected. Injecting");
                                CTWebsite.WebDriver.SwdBrowser.InjectVisualSearch();
                            }

                            if (!webElementExplorerThreadPaused)
                            {
                              ProcessCommands();
                            }
                        }
                        catch (Exception e)
                        {
                            StopVisualSearch();
                            //MyLog.Error("Visual search stopped:");
                            //MyLog.Exception(e);
                        }
                    }
                    Thread.Sleep(VisualSearchQueryDelayMs);
                    var command = SwdBrowser.GetNextCommand();
                    if (command is GetXPathFromElement)
                    {
                        var getXPathCommand = command as GetXPathFromElement;
                        UpdateVisualSearchResult(getXPathCommand.XPathValue);
                    }
                }
               //txtVisualSearchResult.Text = "test1";
            }
            finally
            {
                StopVisualSearch();
             //   txtVisualSearchResult.Text = str590;

                //// MyLog.Write("VisualSearch_UpdateSearchResult: Finished");
            }
           // txtVisualSearchResult.Text = "praveen";

        }
        public void StopVisualSearch()
        {
            VisualSearchStopped();
            webElementExplorerStarted = false;
        }
        const int VisualSearchQueryDelayMs = 777;

        public Thread visualSearchWorker = null;
        bool webElementExplorerStarted = false;
        bool webElementExplorerThreadPaused = false;
        //  public  static void InjectVisualSearch();
   //  priviate   SwdPageRecorder.WebDriver.SwdBrowser re;

        //= new SwdPageRecorder.WebDriver.SwdBrowser();
        public void StartVisualSearch()
        {
            SwdBrowser.InjectVisualSearch();
            if (visualSearchWorker != null)
            {
                visualSearchWorker.Abort();
                visualSearchWorker = null;
            }

            webElementExplorerStarted = true;

            visualSearchWorker = new Thread(VisualSearch_UpdateSearchResult);
            visualSearchWorker.IsBackground = true;
            visualSearchWorker.Start();

            while (!visualSearchWorker.IsAlive)
            {
              // Application.DoEvents();
                System.Threading.Thread.Sleep(1);
            }

            VisuaSearchStarted();

        }


        public void VisualSearchStopped()
        {
            // btnStartVisualSearch.Text = "Start";
          //  MethodInvoker action = delegate;
         //   {
             //   btnStartVisualSearch.Text = "Start";
            // };



            //Action action = () =>
            //{
            //    btnStartVisualSearch.Text = "Start";
            //};

            //// Check if invoking is required (this isn't typical in ASP.NET)
            //if (this.InvokeRequired)
            //{
            //    // If invoking is required, execute the action on the UI thread
            //    btnStartVisualSearch.Text = "Start";
            //}
            //else
            //{
            //    // If invoking is not required, execute the action directly
            //    action();
            //}

            var action = new Action(() =>
            {
                //btnStartVisualSearch.Text = "Start";
            });

            // Capture the current synchronization context
            var syncContext = HttpContext.Current != null ? SynchronizationContext.Current : null;

            if (syncContext != null)
            {
                // Post the action to the synchronization context if required
                syncContext.Post(_ => action(), null);
                //btnStartVisualSearch.Text = "Start";
            }
            else
            {
                // Synchronization context is not available, directly execute the action
                action();
            }


        }


        public  void  VisuaSearchStarted()
        {

            //Action action = () =>
            //{
            //    btnStartVisualSearch.Text = "Stop";
            //};

            //// Check if invoking is required (this isn't typical in ASP.NET)
            //if (this.InvokeRequired)
            //{
            //    // If invoking is required, execute the action on the UI thread
            //   btnStartVisualSearch.Text = "Stop";
            //}
            //else
            //{
            //    // If invoking is not required, execute the action directly
            //    action();
            //}

            var action = new Action(() =>
            {
                //btnStartVisualSearch.Text = "Stop";
            });

            // Capture the current synchronization context
            var syncContext = HttpContext.Current != null ? SynchronizationContext.Current : null;

            if (syncContext != null)
            {
                // Post the action to the synchronization context getCurrentlyChosenFrameif required
               syncContext.Post(_ => action(), null);
              //  btnStartVisualSearch.Text = "Stop";
            }
            else
            {
                // Synchronization context is not available, directly execute the action
                action();
            }

        }


        public void ShowGlobalLoading()
        {
            //pnlLoadingBar.Visible = true;

            //var action = (MethodInvoker)delegate
            //{
            //    pnlLoadingBar.Visible = true;
            //};

            //if (pnlLoadingBar.InvokeRequired)
            //{
            //  pnlLoadingBar.(action);
            //}
            //else
            //{
            //    action();
            //}

        //    txtVisualSearchResult.Text = "test";
        }

        public void DisableWebElementExplorerRunButton()
        {
            //btnStartVisualSearch.DoInvokeAction(() =>
            //{
                //btnStartVisualSearch.Enabled = false;

           // });
        }

        public void EnableWebElementExplorerRunButton()
        {
            //btnStartVisualSearch.DoInvokeAction(() =>
            //{
           //     btnStartVisualSearch.Enabled = true;

            //});
        }

        public void DisableWebElementExplorerResultsField()
        {
            //txtVisualSearchResult.DoInvokeAction(() =>
            //{
             //   txtVisualSearchResult.Enabled = false;

           // });
        }

        public void EnableWebElementExplorerResultsField()
        {
          //if  (txtVisualSearchResult.Visible)
          //  {
          //      txtVisualSearchResult.Enabled = true;

          //  }

        }

        public void HideGlobalLoading()
        {
            //pnlLoadingBar.Visible = false;
            //var action = (MethodInvoker)delegate
            //{
            //    pnlLoadingBar.Visible = false;
            //};

            //if (pnlLoadingBar.InvokeRequired)
            //{
            //    pnlLoadingBar.Invoke(action);
            //}
            //else
            //{
            //    action();
            //}
        }

        public void SetInitialRefreshMessageForSwitchToControls()
        {
            //ddlFrames.Enabled = false;
            //ddlWindows.Enabled = false;

            //ddlWindows.Text = "Press Refresh button";
            //ddlFrames.Text = "... please";
        }
        public void EnableSwitchToControls()
        {
            //ddlFrames.Enabled = true;
            //ddlWindows.Enabled = true;
        }

        public void DisableSwitchToControls()
        {
            //ddlFrames.Enabled = false;
            //ddlWindows.Enabled = false;
        }

        public void SetDriverDependingControlsEnabled(bool shouldControlBeEnabled)
        {
          //  txtBrowserUrl.DoInvokeAction(() => txtBrowserUrl.Enabled = shouldControlBeEnabled);

            //  txtBrowserUrl.DoInvokeAction(() => 
          //  txtBrowserUrl.Enabled = shouldControlBeEnabled;
            //btnBrowser_Go.DoInvokeAction(() => 
           // btnBrowser_Go.Enabled = shouldControlBeEnabled);
            //btnTakePageScreenshot.DoInvokeAction(() => btnTakePageScreenshot.Enabled = shouldControlBeEnabled);
            //btnOpenScreenshotFolder.DoInvokeAction(() => btnOpenScreenshotFolder.Enabled = shouldControlBeEnabled);
            //grpVisualSearch.DoInvokeAction(() => grpVisualSearch.Enabled = shouldControlBeEnabled);
            //grpSwitchTo.DoInvokeAction(() => grpSwitchTo.Enabled = shouldControlBeEnabled);
        }

        public BrowserPageFrame getCurrentlyChosenFrame()
        {
            BrowserPageFrame frame = null;
            object item = null;
            //this.ddlFrames.Invoke((MethodInvoker)delegate ()
            //{
            item = null;// ddlFrames.SelectedItem;
            //}
            //);
            if (item is BrowserPageFrame)
            {
                frame = item as BrowserPageFrame;

            }
            return frame;
        }

        public void UpdateBrowserWindowsList(CTWebsite.WebDriver.BrowserWindow[] currentWindows, string currentWindowHandle)
        {
            //ddlWindows.DoInvokeAction(() =>
            //{
            //    ddlWindows.Items.Clear();
            //   // ddlWindows.Items.AddRange(currentWindows);

            //   // ddlWindows.SelectedItem = currentWindows.First(win => (win.WindowHandle == currentWindowHandle));
            //});
        }
        public void UpdateVisualSearchResult(string xPathAttributeValue)
        {
            var action = new Action(() =>
            {
                //txtVisualSearchResult.Text = xPathAttributeValue;
            });

            // Capture the current synchronization context
            var syncContext = HttpContext.Current != null ? SynchronizationContext.Current : null;

            if (syncContext != null)
            {
                // Post the action to the synchronization context if required
                syncContext.Post(_ => action(), null);
               // txtVisualSearchResult.Text = xPathAttributeValue;
            }
            else
            {
                // Synchronization context is not available, directly execute the action
                action();
            }

        }


       
        public void UpdateVisualSearchResult123(string xPathAttributeValue)
        {
            //txtVisualSearchResult.Text = xPathAttributeValue;


            //var action = (MethodInvoker)delegate
            //{
            //    txtVisualSearchResult.Text = xPathAttributeValue;
            //};

            //if (txtVisualSearchResult.InvokeRequired)
            //{
            //    txtVisualSearchResult.Invoke(action);
            //}
            //else
            //{
            //action();
            //}
        }

        public void SetUrlText(string browserUrl)
        {
            //txtBrowserUrl.DoInvokeAction(() => 
           // txtBrowserUrl.DoInvokeAction(() => txtBrowserUrl.Text = browserUrl);
        }
        public bool wasBrowserStarted = false;

        public bool InvokeRequired { get; private set; }

        // = new SwdBrowser();


        public void UpdatePageFramesList(BrowserPageFrame[] currentPageFrames)
        {
            //ddlFrames.Items.Clear();

            //ddlFrames.DoInvokeAction(() =>
            //{
            //    ddlFrames.Items.Clear();
            //    //ddlFrames.Items.AddRange(currentPageFrames);

            //    //ddlFrames.SelectedItem = currentPageFrames.First();
            //});

        }

        protected void txtVisualSearchResult_TextChanged(object sender, EventArgs e)
        {

        }

        public static string ReadJavaScriptFromFile(string filePath)
        {
            string contents = File.ReadAllText(filePath);
            return contents;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //IWebElement currentElement = SwdBrowser._driver.SwitchTo().ActiveElement();
            //txtVisualSearchResult.Text = currentElement.TagName;


            IJavaScriptExecutor js = (IJavaScriptExecutor)SwdBrowser._driver;
            string javaScript = ReadJavaScriptFromFile(System.IO.Path.Combine("JavaScript", @"D:\New Download\swd-recorder-3.13.0.2018.06.28\swd-recorder-3.13.0.2018.06.28\SwdPageRecorder\SwdPageRecorder.WebDriver\JavaScript\ElementSearch.js"));

            //  string str = "return elem === document.activeElement && (!document.hasFocus || document.hasFocus()) && !!(elem.type || elem.href || ~elem.tabIndex);";

            //+ '"';
            //+":focus" + ")";
            IWebElement currentElement = (IWebElement) js.ExecuteScript(javaScript);

            //txtVisualSearchResult.Text = currentElement.TagName;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}