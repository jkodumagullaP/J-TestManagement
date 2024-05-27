using CTWebsite.UI;
using CTWebsite.WebDriver;
using CTWebsite.WebDriver.JsCommand;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Office.Interop.Excel;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Ionic.Zip;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;


using System.Web;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;
using TreeNode = System.Web.UI.WebControls.TreeNode;
using Spire.Xls;
using System.Data;
using Workbook = Spire.Xls.Workbook;
using Worksheet = Spire.Xls.Worksheet;
using Syncfusion.XlsIO;
using System.Drawing;
using System.Data.OleDb;
using System.Web.UI;
using TreeNodeCollection = System.Web.UI.WebControls.TreeNodeCollection;

namespace CTWebsite
{
    public partial class NewTest : System.Web.UI.Page
    {
        public string ProjectName { get; set; }
        public string Projectpath { get; set; }
        public string ProjectPath = null;
        public IWebDriver driver1;
        public IWebDriver newDriver;
        public static string spath = null;
        SwdMainPresenter presenter = new SwdMainPresenter();
        private System.Threading.ManualResetEvent startedEvent;

        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (!this.IsPostBack)
            {
                path = "";
                spath = "";
                //TreeView1.Nodes.Clear();
               // BindGridView();

                DirectoryInfo rootInfo = new DirectoryInfo("C:/TestAutomation/Projects/");
                this.PopulateTreeView(rootInfo, null);
                //foreach (TreeNode node in TreeView1.Nodes)
                //{
                //    if (node.Checked)
                //        path = path + "\\" + node.Text;

                //}

            }
        }

        public void UpdateVisualSearchResult(string xPathAttributeValue)
        {

            //System.Action action = () =>
            //{
            //    TextBox3.Text = xPathAttributeValue;
            //    //txtVisualSearchResult.Text = xPathAttributeValue;
            //};


            var action = (MethodInvoker)delegate
            {
                TextBox3.Text = xPathAttributeValue;
            };

            if (TextBox3.Enabled)
            {
                action();
            }
            else
            {
                action();
            }
        }



        private static Logger _logger = LogManager.GetCurrentClassLogger();


        private void PopulateTreeView(DirectoryInfo dirInfo, TreeNode treeNode)
        {
            foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {
                TreeNode directoryNode = new TreeNode
                {
                    Text = directory.Name,
                    Value = directory.FullName
                };

                if (treeNode == null)
                {
                    DropDownList1.Items.Add(directoryNode.Text);
                        //If Root Node, add to TreeView.
                     //   TreeView1.Nodes.Add(directoryNode);
                    
                }
                else
                {

              //If Child Node, add to Parent Node.
                    treeNode.ChildNodes.Add(directoryNode);
                
                }
                //Get all files in the Directory.
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        //Add each file as Child Node.
                        TreeNode fileNode = new TreeNode
                        {
                            Text = file.Name,
                            Value = file.FullName,
                            //Target = "_blank",
                            // NavigateUrl = (new Uri(Server.MapPath("~/"))).MakeRelativeUri(new Uri(file.FullName)).ToString()
                        };


                        if (fileNode.Text.Contains(".xls"))
                        {
                            directoryNode.ChildNodes.Add(fileNode);
                        }

                    }
              //  70s#ction98familyGrew49
                PopulateTreeView(directory, directoryNode);
            }
        }

        public static IWebDriver GetDriver()
        {

            SwdBrowser._driver = new RemoteWebDriver(new Uri("http://localhost:2222/wd/hub"), DesiredCapabilities.Chrome());
        //    SwdBrowser._driver = new ChromeDriver(@"C:\TestAutomation\exe");


            SwdBrowser._driver.Manage().Window.Maximize();

            return SwdBrowser._driver;
        }


        protected void Button1_Click(object sender, EventArgs e)
        {

            SwdBrowser._driver = GetDriver();
            //   System.setProperty("webdriver.chrome.driver", "C:\\TestAutomation\\exe\\chromedriver.exe");
            // WebDriverOptions browserOptions = new WebDriverOptions();

            // SwdBrowser._driver =
            //ConnetctToIWebDriver(browserOptions);
            //isRemote = true;
            //  driver = new IWebDriver(new Uri("http://localhost:4444/wd/hub"), DesiredCapabilities.Chrome());

            // var chromeOptions = new ChromeOptions();

            // var driver  = new RemoteWebDriver(new Uri("http://192.168.1.9:2222/wd/hub"), DesiredCapabilities.Chrome());

            //Thread.Sleep(10000);
            //String sessionId = ((RemoteWebDriver)SwdBrowser._driver)..getSessionId().toString();
            //string sessionId = ((RemoteWebDriver)SwdBrowser._driver).Capabilities.GetCapability("webdriver.remote.sessionid").ToString();

            SwdBrowser._driver.Navigate().GoToUrl("http://www.google.com");

            //  SwdBrowser._driver = newDriver;

            //ChromeOptions chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments(
            //    "start-maximized",
            //    "enable-automation",
            //    "--headless",
            //    "--no-sandbox", //this is the relevant other arguments came from solving other issues
            //    "--disable-infobars",
            //    "--disable-dev-shm-usage",
            //    "--disable-browser-side-navigation",
            //    "--disable-gpu",
            //    "--ignore-certificate-errors");
            //var capability = chromeOptions.ToCapabilities();
            //  capability = chromeOptions.ToCapabilities();

            // SetIWebDriver();
            //   //SetImplicitlyWait();
            //   Thread.Sleep(TimeSpan.FromSeconds(2));



            // string  baseURL = "http://demo.guru99.com/test/guru99home/";
            //string   nodeURL = "http://192.168.43.223:5566/wd/hub";
            //DesiredCapabilities capability = DesiredCapabilities.chrome();



            //var capabilities = new DesiredCapabilities();
            //capabilities.SetCapability("browserName", "chrome");

            // Replace 'http://localhost:4444/wd/hub' with the URL of your Selenium Grid hub
            // SwdBrowser._driver = new IWebDriver(new Uri("http://localhost:4444/wd/hub"), DesiredCapabilities.Chrome());

            // Navigate to a webpage
            //  SwdBrowser._driver.Navigate().GoToUrl("https://google.com");




            // Replace 'http://localhost:4444/wd/hub' with the URL of your Selenium Grid hub
            // = driver;

            // Now you have a RemoteWebDriver session with Chrome

            // Close the browser and end the session

            //var driver = new ChromeDriver();
            //  SwdBrowser._driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), DesiredCapabilities.Chrome()); // instead of this url you can put the url of your remote hub

            //Thread.Sleep(10000);
            //    var capabilities = new DesiredCapabilities();
            //  capabilities.SetCapability("app", "C:\\TestAutomation\\exe\\chromedriver.exe");
            //capabilities.SetCapability(CapabilityType.HandlesAlerts, "true");
            //capabilities.SetCapability(CapabilityType.IsJavaScriptEnabled, "true");
            //capabilities.SetCapability(CapabilityType.SupportsFindingByCss, "true");
            //capabilities.SetCapability(CapabilityType.BrowserName, "chrome");
            //capabilities.SetCapability(CapabilityType.Platform, new Platform(PlatformType.Windows));
            //  RemoteWebDriver driver89;

            // driver89 = new ChromeDriver(@"C:\TestAutomation\exe");
            //  driver89.Navigate().GoToUrl("https://www.google.com");

            //  SwdBrowser._driver = new IWebDriver(new Uri("http://localhost:4444/wd/hub"), chromeOptions.ToCapabilities()); // instead of this url you can put the url of your remote hub
            //            SwdBrowser._driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), capabilities, TimeSpan.FromMinutes(3));
            //// ChangeVisualSearchRunningState();

            //  var capabilities = new DesiredCapabilities();
            //capabilities.SetCapability(CapabilityType.BrowserName, "chrome");
            // SwdBrowser._driver = new IWebDriver(new URL("http://localhost:4444/wd/hub"), capabilities);

            //  SwdBrowser._driver = new IWebDriver(new Uri("http://127.0.0.1:4444/wd/hub"),DesiredCapabilities.Chrome(),TimeSpan.FromMinutes(3));
            ///ChromeOptions chromeOptions = new ChromeOptions();
            ////chromeOptions.setCapability("browserVersion", "74");
            //// chromeOptions.setCapability("platformName", "Windows 10");
            // var driver = new IWebDriver(new Uri("http://127.0.0.1:4444/wd/hub"), ChromeOptions.ToCapabilities());
            ////SwdBrowser._driver = driver;
            ////SwdBrowser._driver.get("http://www.google.com");


            //ChromeOptions chromeOptions = new ChromeOptions();
            //chromeOptions.setCapability("browserVersion", "74");
            //chromeOptions.setCapability("platformName", "Windows 10");
            //chromeOptions.setCapability = "win10"; // linux or win10
            //chromeOptions.BrowserVersion = "latest";


            //// Enable video recording
            //var gridlasticOptions = new Dictionary();
            //gridlasticOptions.Add("video", "true");
            //gridlasticOptions.Add("gridlasticUser", USERNAME);
            //gridlasticOptions.Add("gridlasticKey", ACCESS_KEY);
            //chromeOptions.AddAdditionalOption("gridlastic:options", gridlasticOptions);

            //RemoteWebDriver driver = new RemoteWebDriver(new Uri("https://YOUR_HUB_SUBDOMAIN.gridlastic.com/wd/hub/"), chromeOptions.ToCapabilities(), TimeSpan.FromSeconds(600));



        }

        public void DisableWebElementExplorerRunButton()
        {

            Button3.Enabled = false;


        }

        public void EnableWebElementExplorerRunButton()
        {

            Button3.Enabled = true;


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

            var action = new System.Action(() =>
            {
                Button3.Text = "Start";
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


        public void VisuaSearchStarted()
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

            var action = new System.Action(() =>
            {
                Button3.Text = "Stop";
            });

            // Capture the current synchronization context
            var syncContext = HttpContext.Current != null ? SynchronizationContext.Current : null;

            if (syncContext != null)
            {
                // Post the action to the synchronization context if required
                syncContext.Post(_ => action(), null);
                //  btnStartVisualSearch.Text = "Stop";
            }
            else
            {
                // Synchronization context is not available, directly execute the action
                action();
            }

        }
        public string str56 = null;
      //  public string strtag = null;
        private List<string> tags;
        public bool fg;
        public string[] selectors = null;

        public void createTC()
        {

            DataTable dt1 = new DataTable();

dt1.Columns.Clear();

            dt1.Columns.Add();

            dt1.Columns.Add("Tag");

            dt1.Columns.Add("Locator Type");

            dt1.Columns.Add("Locator Value");

          //  CheckBoxField chkBox = new CheckBoxField();

            //chkBox.DataField = "Chk";
            //chkBox.HeaderText = "";
            //dt1.Columns.Insert(4, chkBox);

            //System.Web.UI.WebControls.CheckBox chkbx = new System.Web.UI.WebControls.CheckBox();

            //dt1.Controls.Add(chkbx);



            int row = 2;//=HeaderNames.Count();

            int col = 0;
            ElementChecker ec = new ElementChecker(driver1);
            //  Scrape scrape = new Scrape();
            //scrape.ShowDialog();
            //   if (scrape.DialogResult == DialogResult.OK)
            // {
            tags = new List<string>()
                    {
                        "a",
                        "input",
                        "span",
                        "div",
                        "table",
                        "img",
                        "button",
                        "submit",
                        "select",
                        "mat-form-field",
                        "mat-card",
                        "mat-nav-list",
                        "mat-label",
                        "mat-select",
                        "mat-radio-button",
                        "mat-checkbox",
                        "mat-option"
                                            };            //scrape.GetCheckedItems();
                                                          //}

            //  foreach (var tag in tags)
            //{
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver1;

            var scriptString = "return document.querySelector";
            var selectorIndex = 0;
            var stopIndex = selectors.Length - 1;

            foreach (var selector in selectors)
            {
                var root = "('" + selector + "')";
                root += (selectorIndex != stopIndex && selectors.Length != 1) ? ".shadowRoot.querySelector" : null;
                selectorIndex++;
                scriptString += root;
            }

            //  scriptString = "return window.getComputedStyle(document.querySelector('.analyzer_search_inner.tooltipstered'),':after').getPropertyValue('content')";
            //  Thread.Sleep(3000);
            //  IJavaScriptExecutor js1 = (IJavaScriptExecutor)driver1;
            //  String content = (String)js.ExecuteScript(scriptString);

            //  //  var webElement = (IWebElement)js.ExecuteScript(scriptString);

            //MessageBox.Show(content);

            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> elms = driver1.FindElements(By.TagName("*"));

            foreach (var elm in elms)
            {

                try
                {
                    if (elm.TagName == "a" || elm.TagName == "mat-form-field" || elm.TagName == "span" || elm.TagName == "mat-card" || elm.TagName == "mat-nav-list" || elm.TagName == "mat-toolbar" || elm.TagName == "mat-label" || elm.TagName == "mat - option" || elm.TagName == "mat - select" || elm.TagName == "input" || elm.TagName == "div" || elm.TagName == "table" || elm.TagName == "img" || elm.TagName == "button" || elm.TagName == "submit" || elm.TagName == "select")
                    {

                        try
                        {
                            fg = elm.Enabled;
                        }
                        catch
                        {
                            fg = false;
                        }
                        try
                        {
                            strtag = elm.GetAttribute("type");
                        }
                        catch { }
                        try
                        {

                            if (strtag == "hidden")
                            {



                            }

                            else
                            {
                                UniqueData ud = ec.CheckUniqueness(new WrappedElement(driver1, elm));
                                if (ud.type != null)
                                {

                                    //if (ud.type == null)
                                    //{
                                    //    ud.type = "Element Type is null";
                                    //}
                                    //bool isElementDisplayed_Producer = driver1.FindElement(By.Id("Producer")).Displayed;
                                    //bool isElementDisplayed_Riskstate = driver1.FindElement(By.Id("RiskState")).Displayed;
                                    //bool isElementDisplayed_Company = driver1.FindElement(By.Id("Company")).Displayed;
                                    //bool isElementDisplayed_OrganizationState = driver1.FindElement(By.Id("OrganizationState")).Displayed;

                                    //if (isElementDisplayed_Producer == true) { name13 = "Producer"; }
                                    //if (isElementDisplayed_Riskstate == true) { name13 = "RiskState"; }
                                    //if (isElementDisplayed_Company == true) { name13 = "Company"; }
                                    //if (isElementDisplayed_OrganizationState == true) { name13 = "OrganizationState"; }

                                    //  var test56 = driver1.FindElement(By.Id("Producer"));
                                    // if (isElementDisplayed_Producer == true || isElementDisplayed_Riskstate == true || isElementDisplayed_Company == true || isElementDisplayed_OrganizationState == true)
                                    //{

                                    if (elm.Text != "")
                                    {
                                        //  dataGridView.Rows.Add(VariableName.Name(elm), elm.Text, VariableName.Locator(ud.type.ToString()), ud.value.ToString());
                                    }
                                    if (VariableName.Locator(ud.type.ToString()) == "Id")
                                    {
                                        //dataGridView1.Rows.;

                                        dt1.Rows.Add(VariableName.Name(elm), elm.GetAttribute("id"), VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);
                                    }

                                    else if (VariableName.Locator(ud.type.ToString()) == "Name")
                                    {
                                        dt1.Rows.Add(VariableName.Name(elm), elm.GetAttribute("name"), VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);
                                    }
                                    else if (VariableName.Locator(ud.type.ToString()) == "ClassName")
                                    {
                                        string name = elm.GetAttribute(nameof(name));

                                        dt1.Rows.Add(VariableName.Name(elm), elm.GetAttribute("class"), VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);

                                    }
                                    else if (VariableName.Locator(ud.type.ToString()) == "XPath")
                                    {
                                        string name = elm.GetAttribute(nameof(name));

                                        dt1.Rows.Add(VariableName.Name(elm), "Enter Logical Name ", VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);

                                    }
                                    else
                                    {
                                        //   if (elm.TagName != "select")
                                        // {
                                        string name = elm.GetAttribute(nameof(name));

                                        dt1.Rows.Add(VariableName.Name(elm), "Enter Logical Name ", VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);
                                        //}
                                    }

                                    col = col + 1;
                                    //}
                                }
                                row = row + 1;
                            }
                            // }
                        }
                        catch { };
                    }
                }
                catch { }
            }
            DataTable dt3 = (DataTable)dataGridView1.DataSource;

            dataGridView1.DataSource = dt1;

            //bool isElementDisplayed_Producer = driver1.FindElement(By.Id("Producer")).Displayed;
            //bool isElementDisplayed_Riskstate = driver1.FindElement(By.Id("RiskState")).Displayed;
            //bool isElementDisplayed_Company = driver1.FindElement(By.Id("Company")).Displayed;
            //bool isElementDisplayed_OrganizationState = driver1.FindElement(By.Id("OrganizationState")).Displayed;

            //if(isElementDisplayed_Producer == true) { name13 = "Producer"; }
            //if (isElementDisplayed_Riskstate == true) { name13 = "RiskState"; }
            //if (isElementDisplayed_Company == true) { name13 = "Company"; }
            //if (isElementDisplayed_OrganizationState == true) { name13 = "OrganizationState"; }

            ////  var test56 = driver1.FindElement(By.Id("Producer"));
            //if (isElementDisplayed_Producer == true || isElementDisplayed_Riskstate ==true || isElementDisplayed_Company==true || isElementDisplayed_OrganizationState ==true)
            //{

            //    driver1.FindElement(By.Id(name13)).Click();
            //    Thread.Sleep(1000);

            //    elms = driver1.FindElements(By.CssSelector("mat-option"));
            //    foreach (var elm in elms)
            //    {
            //        try
            //        {
            //            //if (elm.TagName == "a" || elm.TagName == "input" || elm.TagName == "span" || elm.TagName == "div" || elm.TagName == "table" || elm.TagName == "img" || elm.TagName == "button" || elm.TagName == "submit" || elm.TagName == "select")
            //            //{
            //            try
            //            {
            //                fg = elm.Enabled;
            //            }
            //            catch
            //            {
            //                fg = false;
            //            }
            //            try
            //            {
            //                strtag = elm.GetAttribute("type");
            //            }
            //            catch { }
            //            try
            //            {

            //                //if (strtag == "hidden")
            //                //{



            //                //}

            //                //else
            //                //{
            //                UniqueData ud = ec.CheckUniqueness(new WrappedElement(driver1, elm));

            //                // if (ud.value != null)
            //                //{
            //                //MessageBox.Show(elm.GetAttribute("innerText"));
            //                //MessageBox.Show(elm.GetAttribute("class"));
            //                //MessageBox.Show(VariableName.Name(elm));

            //                if (ud.type == null)
            //                {
            //                    ud.type = "Element Type is null";
            //                }


            //                if (elm.Text != "")
            //                {
            //                    //  dataGridView1.Rows.Add(VariableName.Name(elm), elm.Text, VariableName.Locator(ud.type.ToString()), ud.value.ToString());
            //                }
            //                if (VariableName.Locator(ud.type.ToString()) == "Id")
            //                {
            //                    dataGridView1.Rows.Add(VariableName.Name(elm), "Update Manually", VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);

            //                }

            //                else if (VariableName.Locator(ud.type.ToString()) == "Name")
            //                {

            //                    dataGridView1.Rows.Add(VariableName.Name(elm), "Update Manually", VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);

            //                }

            //                else if (VariableName.Locator(ud.type.ToString()) == "ClassName")
            //                {

            //                    dataGridView1.Rows.Add(VariableName.Name(elm), "Update Manually", VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);

            //                }
            //                else if (VariableName.Locator(ud.type.ToString()) == "XPath")
            //                {

            //                    dataGridView1.Rows.Add(VariableName.Name(elm), "Update Manually", VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);

            //                }
            //                else
            //                {

            //                    //   if (elm.TagName != "select")
            //                    // {
            //                    dataGridView1.Rows.Add(VariableName.Name(elm), "Update Manually", VariableName.Locator(ud.type.ToString()), ud.value.ToString(), elm.Enabled);

            //                    //}
            //                }

            //                col = col + 1;
            //                // }
            //                row = row + 1;
            //                // }
            //            }
            //            //   }
            //            catch { };
            //            //}
            //        }
            //        catch { }
            //    }
            //}
            DataTable dt2 = new DataTable();
            string str = "";
            dt2.Columns.Add("ExecuteFlag");
            dt2.Columns.Add("Action");

            DataGridViewComboBoxColumn Action = new DataGridViewComboBoxColumn();
            var list11 = new List<string>() { "I", "NC", "A", "V", "dbc", "Read", "Write" };
            Action.DataSource = list11;
            Action.HeaderText = "Action";
            Action.DataPropertyName = "Action";


            dt2.Columns.Add("LogicalName");
            dt2.Columns.Add("ControlName");
            dt2.Columns.Add("ControlType");
            dt2.Columns.Add("ControlID");
            dt2.Columns.Add("ImageType");
            dt2.Columns.Add("Index");
            dt2.Columns.Add("DynamicIndex");
            dt2.Columns.Add("ColumnName");
            dt2.Columns.Add("RowNo");
            dt2.Columns.Add("ColumnNo");

            dt2.Rows.Add("Y", "I", "Browser", "", "BrowserType", "", "", "", "", "", "", "");
            dt2.Rows.Add("Y", "I", "AppURL", "", "URL", "", "", "", "", "", "", "");
            dt2.Rows.Add("Y", "NC", "Wait", "5", "Wait", "", "", "", "", "", "", "");
            dt2.Rows.Add("Y", "I", "BrowserAuth", "", "BrowserAuth", "LinkText", "", "", "", "", "", "");
            dt2.Rows.Add("Y", "NC", "Wait", "5", "Wait", "", "", "", "", "", "", "");
            string str34 = null;
            foreach (DataGridViewRow row1 in dataGridView1.Rows)
            {
                // MessageBox.Show(row1.Cells[1].Value.ToString());
                //var cell = row1.Cells[1].Value;
                //if (cell == null)
                //{
                //    cell= "Value is empty";
                //    row1.Cells[1].Value = cell.ToString();
                //}
                //    if (cell == "")
                //    {
                //    cell = "Value is empty";
                //    row1.Cells[1].Value = cell;
                //}

                //MessageBox.Show(cell.ToString());

                var str123457 = row1.Cells[3].Value;
                //  var str12345 = row1.Cells[3].Value;

                if (str123457 != null)
                {
                    string str123 = row1.Cells[3].Value.ToString();



                    if (str123.Contains("option"))
                    { }
                    else
                    {

                        //  str34= row1.Cells[1].Value.ToString();
                        //str56 = "I";
                        if (row1.Cells[0].Value.ToString().Contains("WebEdit"))
                        {
                            str34 = "Enter text in :" + row1.Cells[1].Value.ToString();
                            str56 = "I";
                        }
                        else if (row1.Cells[0].Value.ToString().Contains("WebButton"))
                        {
                            str34 = "Click on Button :" + row1.Cells[1].Value.ToString();
                            str56 = "I";
                        }

                        else if (row1.Cells[0].Value.ToString().Contains("Radio"))
                        {
                            str34 = "Click on Radio :" + row1.Cells[1].Value.ToString();
                            str56 = "I";
                        }
                        else if (row1.Cells[0].Value.ToString().Contains("CheckBox"))
                        {
                            str34 = "Click on checkbox :" + row1.Cells[1].Value.ToString();
                            str56 = "I";
                        }
                        else if (row1.Cells[0].Value.ToString().Contains("WebLink"))
                        {
                            str34 = "Click on link :" + row1.Cells[1].Value.ToString();
                            str56 = "I";
                        }
                        else if (row1.Cells[0].Value.ToString().Contains("WebList"))
                        {
                            str34 = "Select item :" + row1.Cells[1].Value.ToString();
                            str56 = "I";
                        }
                        else if (row1.Cells[0].Value.ToString().Contains("WebElement"))
                        {

                            str34 = "Click on :" + row1.Cells[1].Value.ToString();
                            str56 = "I";
                        }
                        else if (row1.Cells[0].Value.ToString().Contains("span"))
                        {
                            str34 = "Click on  :" + row1.Cells[1].Value.ToString();
                            str56 = "I";
                        }

                        else if (row1.Cells[0].Value.ToString().Contains("mat"))
                        {
                            str34 = "Click on  :" + row1.Cells[1].Value.ToString();
                            str56 = "I";
                        }
                        else

                        {
                            //str34 = "Verify: " + row1.Cells[1].Value.ToString();
                            //str56 = "A";
                        }

                        string str1 = row1.Cells[3].Value.ToString();
                        string str2 = row1.Cells[0].Value.ToString();
                        string str3 = row1.Cells[2].Value.ToString();

                        if (fg = true)
                        {



                            if (str1 != null || str1 != "" || str2 != null || str2 != "" || str3 != null || str3 != "")
                            {
                                dt2.Rows.Add("Y", str56, str34, row1.Cells[3].Value, row1.Cells[0].Value, row1.Cells[2].Value);
                            }

                        }
                        else
                        {
                            if (str1 != null || str1 != "" || str2 != null || str2 != "" || str3 != null || str3 != "")
                            {
                                dt2.Rows.Add("Y", str56, str34, row1.Cells[3].Value, row1.Cells[0].Value, row1.Cells[2].Value);
                            }
                        }

                    }
                }
                //}
                // Insert into Temporary DataTable.
                // dt2.Rows.Add("Y", "I", row.Cells[2].Value, row.Cells[3].Value, str,"XPath");
                // Insert into DataBase.
                //   InsertInToDataBase(row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value);
                //}
            }

            DataTable dt = (DataTable)dataGridView2.DataSource;
            //     dt.Merge(dt2);
            dataGridView2.DataSource = dt2;

            // String str =     Microsoft.VisualBasic.Interaction.InputBox("Question?", "Title", "Default Text");
            // input = Interaction.InputBox("Prompt", "Enter Testcase name", "Default");


            Microsoft.Office.Interop.Excel.Application myexcelApplication2 = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook2;
            //  if (flag == "new")
            //{
            // myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestSuites\\Web\\" + textBox7.Text + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //  myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestRunner\\TestRunner.xls");//.Add();
            // @"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //}
            //else if (flag == "open")
            //{
            //  myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(Projectpath + "\\" + ProjectName + "\\Projects\\" + ProjectName + "\\TestSuites\\" + "Web" + "\\" + ProjectName + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            myexcelWorkbook2 = myexcelApplication2.Workbooks.Open("C:\\TestAutomation\\Projects\\" +DropDownList1.SelectedItem.Text + "\\Test Management\\" + "\\ConfigureTestSuites\\Configure_TestSuites.xls");//.Add();
                                                                                                                                                                                       //}
                                                                                                                                                                                       //  Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(@"C:\TestAutomation\Create Testcase.xls");//.Add();
            Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet2 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook2.Worksheets.get_Item(1);
            //      Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val1 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook1.Worksheets.get_Item(1);

            int cnt2;
            cnt2 = myexcelWorksheet2.Cells.Find("*", System.Reflection.Missing.Value,
                           System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                           Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows, Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                           false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

            myexcelWorksheet2.Cells[cnt2 + 1, 1].Value = "";
            myexcelWorksheet2.Cells[cnt2 + 1, 2].Value = "";
            myexcelWorksheet2.Cells[cnt2 + 1, 3].Value = input;
            myexcelWorksheet2.Cells[cnt2 + 1, 4].Value = ProjectName;
            myexcelWorksheet2.Cells[cnt2 + 1, 5].Value = input + ".xls";

            myexcelApplication2.DisplayAlerts = false;

            string ctsstr = "C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\"  + "ConfigureTestSuites" + "\\" + "Configure_TestSuites.xls";

            myexcelApplication2.ActiveWorkbook.SaveAs(ctsstr, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);

            myexcelApplication2.DisplayAlerts = true;


            myexcelWorkbook2.Close();
            myexcelApplication2.Quit();

            releaseObject(myexcelWorksheet2);
            //  releaseObject(myexcelWorksheet_val1);
            releaseObject(myexcelWorkbook2);
            releaseObject(myexcelApplication2);


            Microsoft.Office.Interop.Excel.Application myexcelApplication1 = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook1;
            //  if (flag == "new")
            //{                                             
            // myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestSuites\\Web\\" + textBox7.Text + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //  myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestRunner\\TestRunner.xls");//.Add();
            // @"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //}
            //else if (flag == "open")
            //{
            //  myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(Projectpath + "\\" + ProjectName + "\\Projects\\" + ProjectName + "\\TestSuites\\" + "Web" + "\\" + ProjectName + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            myexcelWorkbook1 = myexcelApplication1.Workbooks.Open("C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management" + "\\TestRunner\\TestRunner.xls");//.Add();
                                                                                                                                                                    //}
                                                                                                                                                                    //  Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(@"C:\TestAutomation\Create Testcase.xls");//.Add();
            Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet1 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook1.Worksheets.get_Item(1);
            //      Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val1 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook1.Worksheets.get_Item(1);

            int cnt1 = myexcelWorksheet1.Cells.Find("*", System.Reflection.Missing.Value,
                           System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                           Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows, Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                           false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

            myexcelWorksheet1.Cells[cnt1 + 1, 1].Value = "Jarus_QA_APS_Phoenix";
            myexcelWorksheet1.Cells[cnt1 + 1, 2].Value = "Web";
            myexcelWorksheet1.Cells[cnt1 + 1, 3].Value = "";
            myexcelWorksheet1.Cells[cnt1 + 1, 4].Value = input;
            myexcelWorksheet1.Cells[cnt1 + 1, 5].Value = input;
            myexcelWorksheet1.Cells[cnt1 + 1, 6].Value = "Y";
            myexcelWorksheet1.Cells[cnt1 + 1, 7].Value = "START";
            myexcelWorksheet1.Cells[cnt1 + 1, 8].Value = input;
            myexcelApplication1.DisplayAlerts = false;
            myexcelWorkbook1.SaveAs("C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\" + "TestRunner\\TestRunner.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            myexcelApplication1.DisplayAlerts = true;

            myexcelWorkbook1.Close();
            myexcelApplication1.Quit();

            releaseObject(myexcelWorksheet1);
            //  releaseObject(myexcelWorksheet_val1);
            releaseObject(myexcelWorkbook1);
            releaseObject(myexcelApplication1);



            //   Microsoft.VisualBasic.Interaction.InputBox("Question?", "Title", "Default Text");

            Microsoft.Office.Interop.Excel.Application myexcelApplication = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook = myexcelApplication.Workbooks.Open(@"C:\TestAutomation\Projects" + "\\" + DropDownList1.SelectedItem.Text + "\\" + "Create Testcase.xls");//.Add();
            Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(1);
            Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(2);

            myexcelWorksheet.Rows.Clear();

            myexcelWorksheet.Cells[1, 1] = "ExecuteFlag";
            //  }
            //  else if (item.Name != "")
            //  {
            myexcelWorksheet.Cells[1, 2] = "Action";
            myexcelWorksheet.Cells[1, 3] = "LogicalName";
            myexcelWorksheet.Cells[1, 4] = "ControlName";
            myexcelWorksheet.Cells[1, 5] = "ControlType";
            myexcelWorksheet.Cells[1, 6] = "ControlID";
            myexcelWorksheet.Cells[1, 7] = "ImageType";
            myexcelWorksheet.Cells[1, 8] = "Index";
            myexcelWorksheet.Cells[1, 9] = "DynamicIndex";
            myexcelWorksheet.Cells[1, 10] = "ColumnName";
            myexcelWorksheet.Cells[1, 11] = "RowNo";
            myexcelWorksheet.Cells[1, 12] = "ColumnNo";
            //  myexcelWorksheet.Cells[1, 6] = "XPATH";

            int cnt = 12;//=HeaderNames.Count();
            int cntc = 0;


            for (int rows = 0; rows < dataGridView2.Rows.Count - 1; rows++)
            {

                for (col = 0; col < dataGridView2.Rows[rows].Cells.Count; col++)
                {

                    myexcelWorksheet.Cells[rows + 2, col + 1] = dataGridView2.Rows[rows].Cells[col].Text.ToString();


                }


                // label20.Text = s1;
            }

            int u = 3;



            for (int rows = 0; rows < dataGridView2.Rows.Count - 1; rows++)
            {

                try
                {
                    if (myexcelWorksheet.Cells[rows + 2, 3].value != null)
                    {
                        myexcelWorksheet_val.Cells[1, u] = myexcelWorksheet.Cells[rows + 2, 3].value;

                        myexcelWorksheet_val.Cells[2, 1] = input;
                        myexcelWorksheet_val.Cells[2, 2] = input;
                        myexcelWorksheet_val.Cells[2, 3] = "Chrome";
                        myexcelWorksheet_val.Cells[2, 4] = "";
                        myexcelWorksheet_val.Cells[2, 5] = "";
                        str = myexcelWorksheet.Cells[rows + 2, 3].value.ToString();
                        if (strtag != "hidden")
                        {
                            if (str.Contains("Enter"))
                            {
                                if (str.Contains("Firm"))
                                {
                                    //   Mirage.Generators.CompanyAttribute  robj= new Mirage.Generators.CompanyAttribute();

                                    //  robj.ToString();
                                    var cn = new Bogus.DataSets.Company(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    //   myexcelWorksheet_val.Cells[2, u] = cn.CompanyName(1);
                                    //"Quantum";

                                }
                                if (str.Contains("First"))
                                {
                                    var fn = new Bogus.DataSets.Name(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    // myexcelWorksheet_val.Cells[2, u] = fn.FirstName();
                                    //  myexcelWorksheet_val.Cells[2, u ] = "Smith";
                                }
                                if (str.Contains("Last"))
                                {
                                    var ln = new Bogus.DataSets.Name(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    // myexcelWorksheet_val.Cells[2, u] = ln.LastName();

                                    // myexcelWorksheet_val.Cells[2, u] = "David";
                                }
                                if (str.Contains("Nick"))
                                {
                                    var ln = new Bogus.DataSets.Name(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    //myexcelWorksheet_val.Cells[2, u] = ln.LastName();

                                    // myexcelWorksheet_val.Cells[2, u ] = "Dave";
                                }
                                if (str.Contains("Address"))
                                {
                                    var ln = new Bogus.DataSets.Address(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    // myexcelWorksheet_val.Cells[2, u] = ln.StreetName();
                                    // myexcelWorksheet_val.Cells[2, u ] = "1400 American Lane";
                                }
                                if (str.Contains("Zipcode"))
                                {
                                    var ln = new Bogus.DataSets.Address(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    //myexcelWorksheet_val.Cells[2, u] = ln.ZipCode();
                                    // myexcelWorksheet_val.Cells[2, u ] = "60196";
                                }
                                if (str.Contains("Title"))
                                {


                                    //  myexcelWorksheet_val.Cells[2, u ] = "Mr.";
                                }
                                if (str.Contains("Ext"))
                                {
                                    //myexcelWorksheet_val.Cells[2, u] = "1234";
                                }
                                if (str.Contains("Phone"))
                                {
                                    var ln = new Bogus.DataSets.PhoneNumbers(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    // myexcelWorksheet_val.Cells[2, u] = ln.PhoneNumber();
                                    //  myexcelWorksheet_val.Cells[2, u ] = "9874561234";
                                }
                                if (str.Contains("Fax"))
                                {
                                    var ln = new Bogus.DataSets.PhoneNumbers(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    // myexcelWorksheet_val.Cells[2, u] = ln.PhoneNumber();
                                    //  myexcelWorksheet_val.Cells[2, u ] = "9874561235";
                                }
                                if (str.Contains("email"))
                                {
                                    //   var ln = new Bogus.DataSets.Lorem(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    // myexcelWorksheet_val.Cells[2, u] = ln
                                    // myexcelWorksheet_val.Cells[2, u] = "test@test.com";
                                }
                                if (str.Contains("Birth"))
                                {
                                    // var ln = new Bogus.DataSets.Date(locale: "en");

                                    // string fd = td.CompanyName(1);
                                    //myexcelWorksheet_val.Cells[2, u] = ln.();
                                    // myexcelWorksheet_val.Cells[2, u] = "12/12/1987";
                                }
                                if (str.Contains("estabilsh"))
                                {
                                    // myexcelWorksheet_val.Cells[2, u] = "12/12/2021";
                                }
                                if (str.Contains("Date"))
                                {
                                    // myexcelWorksheet_val.Cells[2, u] = "12/12/2021";
                                }
                                if (str.Contains("Attorney"))
                                {
                                    // myexcelWorksheet_val.Cells[2, u] = "1";
                                }
                            }
                            else if (str.Contains("Click"))
                            {
                                if (str.Contains("Webelement"))
                                {
                                    string[] arr = str.Split(':');

                                    myexcelWorksheet_val.Cells[2, u] = arr[1];
                                }
                                else
                                {

                                    // if (str.Contains("CheckBox") || str.Contains("Radio") || str.Contains("Link"))
                                    //{
                                    //  myexcelWorksheet_val.Cells[2, u] = "Y";
                                }
                                //}
                            }

                            u = u + 1;
                        }
                    }
                }

                catch (Exception e)
                {
                    // MessageBox.Show(e.Message);

                }

            }


            myexcelApplication.DisplayAlerts = false;
            //   if (flag == "new")
            // {
            // myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //   myexcelApplication.ActiveWorkbook.SaveAs(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestSuites\\Web\\" + textBox7.Text+"\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // @"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //}
            //else if (flag == "open")
            //{
            //  myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            myexcelApplication.ActiveWorkbook.SaveAs("C:\\TestAutomation\\Projects\\" +DropDownList1.SelectedItem.Text + "\\Test Management" + "\\TestSuites\\" + "Web"  + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //}
            myexcelApplication.DisplayAlerts = true;
            myexcelWorkbook.Close();
            myexcelApplication.Quit();

            releaseObject(myexcelWorksheet);
            releaseObject(myexcelWorksheet_val);
            releaseObject(myexcelWorkbook);
            releaseObject(myexcelApplication);

            //   MessageBox.Show("Test case " + input + " saved sucessfully");

            //  }
            //  else
            //{
            //  MessageBox.Show("Please select automation suite");
            //}


        }
        public BrowserPageFrame getCurrentlyChosenFrame()
        {

           

            BrowserPageFrame frame = null;
            object item = null;
            //var action = (MethodInvoker)delegate
            //{
            //    item = ListBox1.SelectedItem;
            //};
            ////this.ListBox1.Invoke((MethodInvoker)delegate ()
            ////{
            ////    item = ListBox1.SelectedItem;
            ////}
            ////);
            //if (item is BrowserPageFrame)
            //{
            //    frame = item as BrowserPageFrame;

            //}
            return null;

            //BrowserPageFrame frame = null;
            //object item = null;
            //int i = 1;
            ////this.ddlFrames.Invoke((MethodInvoker)delegate ()
            ////{
            //item = "Test" + i;// ddlFrames.SelectedItem;
            //                  //}
            //                  //);
            //                  //if (item is BrowserPageFrame)
            //                  //{
            //frame = item as BrowserPageFrame;

            //// }
            //i = i + 1;
            //return frame;
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

        public void DisableWebElementExplorerResultsField()
        {

            //txtVisualSearchResult.Enabled = false;


        }

        public void EnableWebElementExplorerResultsField()
        {

            // txtVisualSearchResult.Enabled = true;
            //  txtVisualSearchResult.Text=



        }
        const int VisualSearchQueryDelayMs = 777;

        public Thread visualSearchWorker = null;
        bool webElementExplorerStarted = false;
        bool webElementExplorerThreadPaused = false;
        public void VisualSearch_UpdateSearchResult()
        {
            //try
            System.Diagnostics.Process process = null;
            //{

            process = System.Diagnostics.Process.Start("cmd.exe");
            while (!process.HasExited)
            {
                //update UI
           //     Button3.Text = "Recording...";

                ProcessCommands();

            }
          //  Button3.Text = "Record";


            //    ProcessCommands();
            try { 
            //    // // MyLog.Write("VisualSearch_UpdateSearchResult: Started");
            while (webElementExplorerStarted == true)
            {
                    // ProcessCommands();

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

            }
        }
            finally
            {
                StopVisualSearch();
        //  txtVisualSearchResult.Text = str590;

        //// MyLog.Write("VisualSearch_UpdateSearchResult: Finished");
    }
  //  txtVisualSearchResult.Text = "praveen";

        }

        private void MsgBox(string sMessage)
        {
            string msg = "<script language=\"javascript\">";
            msg += "alert('" + sMessage + "');";
            msg += "</script>";
            Response.Write(msg);
        }


        public void ProcessCommands()
        {
           // MsgBox("Capture element");
            var command = SwdBrowser.GetNextCommand();
            if (command is GetXPathFromElement)
            {
                var getXPathCommand = command as GetXPathFromElement;
                UpdateVisualSearchResult(getXPathCommand.XPathValue);
                //txtVisualSearchResult.Enabled = true;
                //txtVisualSearchResult.Text = getXPathCommand.XPathValue;
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
                var element = new WebElementDefinition()
                {
                    Name = addElementCommand.ElementCodeName,
                    HtmlId = addElementCommand.ElementId,
                    Xpath = addElementCommand.ElementXPath,
                    HowToSearch = (emptyHtmlId) ? LocatorSearchMethod.XPath : LocatorSearchMethod.Id,
                    Locator = (emptyHtmlId) ? addElementCommand.ElementXPath : addElementCommand.ElementId,
                    CssSelector = addElementCommand.ElementCssSelector,
                    frame = simpleFrame,
                };



                FileStream fs = new FileStream("C:\\Dev\\Variables.txt", FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter("C:\\Dev\\Variables.txt", true, Encoding.ASCII);
                //"Tag Name:"+ "Element Name:"+ "Element Id:"+ "Xpath:" + "CSS:"
                string ln = element.HtmlTag;
                if (ln == null || ln == "")
                {
                    ln = "null";
                }
               
                string tag = element.HtmlTag;
                if (tag == null || tag == "")
                {
                    tag = "null";
                }
                
                string name = element.Name;
                if(name ==null || name == "")
                {
                    name = "null";
                }
               
                string id = element.HtmlId;
                if (id == null || id == "")
                {
                    id = "null";
                }
               
                string xpath = element.Xpath;
                if (xpath == null || xpath == "")
                {
                    xpath = "null";
                }
               
                string css = element.CssSelector;

                if (css == null || css == "")
                {
                    css = "null";
                }
                
                string NextLine = name + "^" +id + "^" +xpath + "^"  + css+ "^"+ tag  ;
                sw.Write(NextLine+System.Environment.NewLine);
                sw.Close();




                //  str590 = element.HtmlId;
                bool addNew = true;
               // txtVisualSearchResult.Text = element.HtmlTag;
                Presenters.SelectorsEditPresenter.UpdateWebElementWithAdditionalProperties(element);
                Presenters.PageObjectDefinitionPresenter.UpdatePageDefinition(element, addNew);
            }
        }

        public void StopVisualSearch()
        {
            VisualSearchStopped();
            webElementExplorerStarted = false;
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
        

        protected void btnStartVisualSearch_Click(object sender, EventArgs e)
        {
            ChangeVisualSearchRunningState();
         
         //   createTC();
          

        }



        protected void Button2_Click(object sender, EventArgs e)
        {
            ProcessCommands();
        }

        protected void fvTestCaseDetails_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {

        }

        protected void Button4_Click(object sender, EventArgs e)
        {

            var folder = "C:\\TestAutomation\\Projects\\" + TextBox3.Text;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string source_dir = Server.MapPath("~/bin/TestAutomation");
            string destination_dir = "C:\\TestAutomation\\Projects\\" + TextBox3.Text;

            // substring is to remove destination_dir absolute path (E:\).

            // Create subdirectory structure in destination    
            foreach (string dir in System.IO.Directory.GetDirectories(source_dir, "*", System.IO.SearchOption.AllDirectories))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(destination_dir, dir.Substring(source_dir.Length+1)));
                // Example:
                //     > C:\sources (and not C:\E:\sources)
            }

            foreach (string file_name in System.IO.Directory.GetFiles(source_dir, "*", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Copy(file_name, System.IO.Path.Combine(destination_dir, file_name.Substring(source_dir.Length+1)));
            }

            //TreeNode newNode = new TreeNode("New Node");
            //TreeView1.SelectedNode.ChildNodes.Add(newNode);

            //string Tranfiles, ProcessedFiles;

            ////Tranfiles = Server.MapPath(@"~\godurian\sth100\transfiles\" + Filename);

            //Tranfiles = Server.MapPath(@"~\transfiles\" + Filename);
            //if (File.Exists(Server.MapPath(@"~\transfiles\" + Filename)))
            //{
            //    File.Delete(Server.MapPath(@"~\transfiles\" + Filename));
            //}

            ////ProcessedFiles = Server.MapPath(@"~\godurian\sth100\ProcessedFiles");
            //ProcessedFiles = Server.MapPath(@"~\ProcessedFiles");

            //File.Move(Tranfiles, ProcessedFiles);
            string strDestination2 = "C:\\TestAutomation\\Projects\\"+ TextBox3.Text+ "\\Test Management\\Settings\\Settings.xls";

            Microsoft.Office.Interop.Excel.Application xlapp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook wb = default(Microsoft.Office.Interop.Excel.Workbook);

            wb = xlapp.Workbooks.Open(strDestination2);
            //FileName.ToString());

            wb.Worksheets["Config"].Cells.Replace("Sample", TextBox3.Text);
           // wb.Worksheets["Config"].Cells.Replace("C:\\TestAutomation", nightTextBox2.Text);
            wb.Save();
            wb.Close();
            xlapp.Quit();


          string  sFileName = "C:\\TestAutomation\\Projects\\" + TextBox3.Text  + "\\execute.bat";

            // System.Diagnostics.Process.Start(sFileName+"execute.bat", "\"1st\" \"2nd\"");

            string path123 = System.IO.Path.Combine(sFileName);
            if (File.Exists(path123))
            {
                File.Delete(path123);
                using (StreamWriter w = new StreamWriter(path123))
                {
                    w.WriteLine("@echo");
                    w.WriteLine("java -jar Jarus-Selenium_lat.jar" + " " + "\"" + "C:\\TestAutomation\\Projects\\" + TextBox3.Text + "\\" + "Test Management\\Settings\\settings.xls" + "\"");
                    w.Close();
                }
            }

            //StreamReader sr = new StreamReader(nightTextBox2.Text + "/" + nightTextBox1.Text + "/" + "execute1.bat");// path /to/file.txt");
            //StreamWriter sw = new StreamWriter(nightTextBox2.Text + "//" + nightTextBox1.Text + "//" + "execute.txt",true);
            //string sLine = sr.ReadLine();
            //for (; sLine != null; sLine = sr.ReadLine())
            //{
            //    if (sLine.Contains("jar"))
            //        {
            //        sLine ="{ " + sLine.Replace("ALPSPhoenix", nightTextBox1.Text)+"},"; ;
            //        sw.Write(sLine);
            //    }
            //    else
            //    {
            //        sw.Write(sLine);
            //    }

        //    Directory.Move(path1 + "\\settings\\Verification\\RB_IMM", path1 + "\\settings\\Verification\\" + nightTextBox1.Text);
            //}


        }

        protected void TextBox3_TextChanged(object sender, EventArgs e)
        {

        }
        public string input;

        public string flag = null;

    //   public string ProjectPath = null;
        protected void Button5_Click(object sender, EventArgs e)
        {
            flag = "TR";
            path = "C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\TestRunner\\TestRunner.xls";
            TestRunner(path);
          //string  FileName = "node.bat"; // It's a file name displayed on downloaded file on client side.

            //  response.ClearContent();
            //  response.Clear();
            //  response.ContentType = "*/*";
            //  response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ";");
            //  response.TransmitFile(Server.MapPath("~/bin/TestAutomation/selenium/node.bat"));
            //  response.Flush();
            //  response.End();

            //  //Thread.Sleep(50000);
            //  response.Redirect("C://TestAutomation//node.bat");
            //  System.Diagnostics.Process process = null;
            //  //{
            //  process = System.Diagnostics.Process.Start("C://Users//jagathpraveenk//Downloads//node.bat");
        }
        public string strtag = null;

        void ConvertToXlsx(string sourcefile, string destfile)
        {


            int i, j;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel._Worksheet xlWorkSheet;
            Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val;
            // "C:\\TestAutomation\\Projects\\" + TreeView1.SelectedNode.Text + "\\Test Management\\" + "ConfigureTestSuites\\Configure_TestSuites.xls");//.Add();
            //}

            object misValue = System.Reflection.Missing.Value;
            
            string[] lines, cells;
            lines = File.ReadAllLines(sourcefile);
            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.DisplayAlerts = false;
            //xlWorkBook = xlApp.Workbooks.Add();
            xlWorkBook = xlApp.Workbooks.Open(destfile);
            xlWorkSheet=(Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            //myexcelWorksheet_val = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            // xlWorkSheet = (Microsoft.Office.Interop.Excel._Worksheet)xlWorkBook.ActiveSheet;
            // xlWorkSheet.Cells[2,  1] = SwdBrowser._driver.Title;
            int rows = 6;// xlWorkSheet.UsedRange.Rows.Count;
            int cols = xlWorkSheet.Columns.Count;


            //int u = 3;
            //            for (i = 0; i < lines.Length; i++)

            for (i = 0; i < lines.Length; i++)
            {
               // int i = 1;
                cells = lines[i].Split(new Char[] { '^' });
                //for (j = 0; j < cells.Length; j++)

                xlWorkSheet.Cells[rows + 1, 1] = "Y";
                xlWorkSheet.Cells[rows + 1, 2] = "I";
                xlWorkSheet.Cells[rows + 1, 3] = cells[0];
                if (cells[1] != "null")
                {
                    xlWorkSheet.Cells[rows + 1, 4] = cells[1];
                    xlWorkSheet.Cells[rows + 1, 6] = "Id";
                }
                else if (cells[2] != "null")
                {
                    xlWorkSheet.Cells[rows + 1, 4] = cells[2];
                    xlWorkSheet.Cells[rows + 1, 6] = "XPath";

                }
                else if (cells[3] != "null")
                {
                    xlWorkSheet.Cells[rows + 1, 4] = cells[3];
                    xlWorkSheet.Cells[rows + 1, 6] = "CssSelector";

                }
                if (cells[0].Contains("text")|| cells[0].Contains("email")||cells[0].Contains("phone"))
                {
                    xlWorkSheet.Cells[rows + 1, 5] = "WebEdit";

                }
                else if(cells[0].Contains("button") || cells[0].Contains("submit"))
                {

                    xlWorkSheet.Cells[rows + 1, 5] = "WebButton";

                }
                else if (cells[0].Contains("select"))
                {

                    xlWorkSheet.Cells[rows + 1, 5] = "WebList";

                }
                else if (cells[0].Contains("checkbox"))
                {

                    xlWorkSheet.Cells[rows + 1, 5] = "CheckBox";

                }
                else if (cells[0].Contains("radio"))
                {

                    xlWorkSheet.Cells[rows + 1, 5] = "Radio";

                }
                else if (cells[0].Contains("link"))
                {

                    xlWorkSheet.Cells[rows + 1, 5] = "WebLink";

                }
                else 
                {

                    xlWorkSheet.Cells[rows + 1, 5] = "WebElement";

                }
                //for (j = 0; j < cols; j++)
                //    xlWorkSheet.Cells[rows + 1, j+1 ] = cells[j];
                rows = rows + 1;
            }
            input = SwdBrowser._driver.Title;

            xlWorkBook.SaveAs("C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\" + "TestSuites\\" + "Web" + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            releaseObject(xlApp);
            releaseObject(xlWorkBook);
            //  releaseObject(xlApp.ActiveWorkbook);
            releaseObject(xlWorkSheet);

           // i = i + 1;
        }

        void TransposeToXlsx(string sourcefile)
        {
            int i, j;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel._Worksheet xlWorkSheet;
            Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val;
            // "C:\\TestAutomation\\Projects\\" + TreeView1.SelectedNode.Text + "\\Test Management\\" + "ConfigureTestSuites\\Configure_TestSuites.xls");//.Add();
            //}

            object misValue = System.Reflection.Missing.Value;

            
            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.DisplayAlerts = false;
            //xlWorkBook = xlApp.Workbooks.Add();
            xlWorkBook = xlApp.Workbooks.Open(sourcefile);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            myexcelWorksheet_val = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            //   xlWorkSheet = (Microsoft.Office.Interop.Excel._Worksheet)xlWorkBook.ActiveSheet;
            // xlWorkSheet.Cells[2,  1] = SwdBrowser._driver.Title;
            int rows = xlWorkSheet.UsedRange.Rows.Count;
            int cols = xlWorkSheet.Columns.Count;
            input = SwdBrowser._driver.Title;


            int u = 5;
            //            for (i = 0; i < lines.Length; i++)

            //int rowss = xlWorkSheet.UsedRange.Rows.Count;
            int rowss = myexcelWorksheet_val.UsedRange.Rows.Count;

            //for (rowss = 0; rowss < dataGridView2.Rows.Count - 1; rowss++)

            for (int rowss567 = 0; rowss567 < rows; rowss567++)
            {

                try
                {
                    //if (myexcelWorksheet_val.Cells[rowss567 + 2, 3].value != null)
                    //{
                    myexcelWorksheet_val.Cells[1, u] = xlWorkSheet.Cells[rowss567 + 6, 3].value;

                    myexcelWorksheet_val.Cells[2, 1] = input;
                    myexcelWorksheet_val.Cells[2, 2] = input;
                    myexcelWorksheet_val.Cells[2, 3] = "Chrome";
                    myexcelWorksheet_val.Cells[2, 4] = "";
                    myexcelWorksheet_val.Cells[2, 5] = "";
                    //string str = xlWorkSheet.Cells[rowss567 + 2, 3].value.ToString();
                    // if (strtag != "hidden")
                    // {
                    //     if (str.Contains("Enter"))
                    //     {
                    //         if (str.Contains("Firm"))
                    //         {
                    //             //   Mirage.Generators.CompanyAttribute  robj= new Mirage.Generators.CompanyAttribute();

                    //             //  robj.ToString();
                    //             var cn = new Bogus.DataSets.Company(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             //   myexcelWorksheet_val.Cells[2, u] = cn.CompanyName(1);
                    //             //"Quantum";

                    //         }
                    //         if (str.Contains("First"))
                    //         {
                    //             var fn = new Bogus.DataSets.Name(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             // myexcelWorksheet_val.Cells[2, u] = fn.FirstName();
                    //             //  myexcelWorksheet_val.Cells[2, u ] = "Smith";
                    //         }
                    //         if (str.Contains("Last"))
                    //         {
                    //             var ln = new Bogus.DataSets.Name(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             // myexcelWorksheet_val.Cells[2, u] = ln.LastName();

                    //             // myexcelWorksheet_val.Cells[2, u] = "David";
                    //         }
                    //         if (str.Contains("Nick"))
                    //         {
                    //             var ln = new Bogus.DataSets.Name(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             //myexcelWorksheet_val.Cells[2, u] = ln.LastName();

                    //             // myexcelWorksheet_val.Cells[2, u ] = "Dave";
                    //         }
                    //         if (str.Contains("Address"))
                    //         {
                    //             var ln = new Bogus.DataSets.Address(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             // myexcelWorksheet_val.Cells[2, u] = ln.StreetName();
                    //             // myexcelWorksheet_val.Cells[2, u ] = "1400 American Lane";
                    //         }
                    //         if (str.Contains("Zipcode"))
                    //         {
                    //             var ln = new Bogus.DataSets.Address(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             //myexcelWorksheet_val.Cells[2, u] = ln.ZipCode();
                    //             // myexcelWorksheet_val.Cells[2, u ] = "60196";
                    //         }
                    //         if (str.Contains("Title"))
                    //         {


                    //             //  myexcelWorksheet_val.Cells[2, u ] = "Mr.";
                    //         }
                    //         if (str.Contains("Ext"))
                    //         {
                    //             //myexcelWorksheet_val.Cells[2, u] = "1234";
                    //         }
                    //         if (str.Contains("Phone"))
                    //         {
                    //             var ln = new Bogus.DataSets.PhoneNumbers(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             // myexcelWorksheet_val.Cells[2, u] = ln.PhoneNumber();
                    //             //  myexcelWorksheet_val.Cells[2, u ] = "9874561234";
                    //         }
                    //         if (str.Contains("Fax"))
                    //         {
                    //             var ln = new Bogus.DataSets.PhoneNumbers(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             // myexcelWorksheet_val.Cells[2, u] = ln.PhoneNumber();
                    //             //  myexcelWorksheet_val.Cells[2, u ] = "9874561235";
                    //         }
                    //         if (str.Contains("email"))
                    //         {
                    //             //   var ln = new Bogus.DataSets.Lorem(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             // myexcelWorksheet_val.Cells[2, u] = ln
                    //             // myexcelWorksheet_val.Cells[2, u] = "test@test.com";
                    //         }
                    //         if (str.Contains("Birth"))
                    //         {
                    //             // var ln = new Bogus.DataSets.Date(locale: "en");

                    //             // string fd = td.CompanyName(1);
                    //             //myexcelWorksheet_val.Cells[2, u] = ln.();
                    //             // myexcelWorksheet_val.Cells[2, u] = "12/12/1987";
                    //         }
                    //         if (str.Contains("estabilsh"))
                    //         {
                    //             // myexcelWorksheet_val.Cells[2, u] = "12/12/2021";
                    //         }
                    //         if (str.Contains("Date"))
                    //         {
                    //             // myexcelWorksheet_val.Cells[2, u] = "12/12/2021";
                    //         }
                    //         if (str.Contains("Attorney"))
                    //         {
                    //             // myexcelWorksheet_val.Cells[2, u] = "1";
                    //         }
                    //     }
                    //     else if (str.Contains("Click"))
                    //     {
                    //         if (str.Contains("Webelement"))
                    //         {
                    //             string[] arr = str.Split(':');

                    //             myexcelWorksheet_val.Cells[2, u] = arr[1];
                    //         }
                    //         else
                    //         {

                    //             // if (str.Contains("CheckBox") || str.Contains("Radio") || str.Contains("Link"))
                    //             //{
                    //             //  myexcelWorksheet_val.Cells[2, u] = "Y";
                    //         }
                    //         //}
                    //     }

                    u = u + 1;
                    // }
                    // }
                }

                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);

                }

            }


            xlWorkBook.SaveAs(sourcefile, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                //"C:\\TestAutomation\\Projects\\" + TreeView1.SelectedNode.Text + "\\Test Management\\" + "TestSuites\\" + "Web" + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);

            // xlWorkBook.SaveAs(destfile, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            releaseObject(xlApp);
            releaseObject(xlWorkBook);
            //  releaseObject(xlApp.ActiveWorkbook);
            releaseObject(xlWorkSheet);
            releaseObject(myexcelWorksheet_val);

            // i = i + 1;
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            string input = SwdBrowser._driver.Title;
            string sourcefile = "C:\\Dev\\Variables.txt";
            string destfile = "C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\TestSuites\\Test Case Template.xls";
            string destfile1 = "C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\TestSuites\\Web\\" + input + ".xls";

            ConvertToXlsx(sourcefile, destfile);


            TransposeToXlsx(destfile1);

            Microsoft.Office.Interop.Excel.Application myexcelApplication3 = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook3;
            //  if (flag == "new")
            //{
            // myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestSuites\\Web\\" + textBox7.Text + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //  myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestRunner\\TestRunner.xls");//.Add();
            // @"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //}
            //else if (flag == "open")
            //{
            //  myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(Projectpath + "\\" + ProjectName + "\\Projects\\" + ProjectName + "\\TestSuites\\" + "Web" + "\\" + ProjectName + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            myexcelWorkbook3 = myexcelApplication3.Workbooks.Open("C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\" + "ConfigureTestSuites\\Configure_TestSuites.xls");//.Add();
                                                                                                                                                                                                            //}
                                                                                                                                                                                                            //  Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(@"C:\TestAutomation\Create Testcase.xls");//.Add();
            Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet3 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook3.Worksheets.get_Item(1);
            //      Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val1 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook1.Worksheets.get_Item(1);

            int cnt3;
            cnt3 = myexcelWorksheet3.Cells.Find("*", System.Reflection.Missing.Value,
                           System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                           Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows, Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                           false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

            myexcelWorksheet3.Cells[cnt3 + 1, 1].Value = "";
            myexcelWorksheet3.Cells[cnt3 + 1, 2].Value = "";
            myexcelWorksheet3.Cells[cnt3 + 1, 3].Value = input;
            myexcelWorksheet3.Cells[cnt3 + 1, 4].Value = ProjectName;
            myexcelWorksheet3.Cells[cnt3 + 1, 5].Value = input + ".xls";

            myexcelApplication3.DisplayAlerts = false;
            string ctsstr = "C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\" + "Test Management" + "\\" + "ConfigureTestSuites" + "\\" + "Configure_TestSuites.xls";

            myexcelApplication3.ActiveWorkbook.SaveAs(ctsstr, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);

            myexcelApplication3.DisplayAlerts = true;


            myexcelWorkbook3.Close();
            myexcelApplication3.Quit();

            releaseObject(myexcelWorksheet3);
            //  releaseObject(myexcelWorksheet_val1);
            releaseObject(myexcelWorkbook3);
            releaseObject(myexcelApplication3);


            Microsoft.Office.Interop.Excel.Application myexcelApplication1 = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook1;
            //  if (flag == "new")
            //{                                             
            // myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestSuites\\Web\\" + textBox7.Text + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //  myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestRunner\\TestRunner.xls");//.Add();
            // @"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //}
            //else if (flag == "open")
            //{
            //  myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(Projectpath + "\\" + ProjectName + "\\Projects\\" + ProjectName + "\\TestSuites\\" + "Web" + "\\" + ProjectName + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            myexcelWorkbook1 = myexcelApplication1.Workbooks.Open("C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\" + "TestRunner\\TestRunner.xls");//.Add();
                                                                                                                                                                                         //}
                                                                                                                                                                                         //  Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(@"C:\TestAutomation\Create Testcase.xls");//.Add();
            Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet1 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook1.Worksheets.get_Item(1);
            //      Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val1 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook1.Worksheets.get_Item(1);

            int cnt1 = myexcelWorksheet1.Cells.Find("*", System.Reflection.Missing.Value,
                           System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                           Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows, Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                           false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

            myexcelWorksheet1.Cells[cnt1 + 1, 1].Value = "Jarus_QA_APS_Phoenix";
            myexcelWorksheet1.Cells[cnt1 + 1, 2].Value = "Web";
            myexcelWorksheet1.Cells[cnt1 + 1, 3].Value = "";
            myexcelWorksheet1.Cells[cnt1 + 1, 4].Value = input;
            myexcelWorksheet1.Cells[cnt1 + 1, 5].Value = input;
            myexcelWorksheet1.Cells[cnt1 + 1, 6].Value = "Y";
            myexcelWorksheet1.Cells[cnt1 + 1, 7].Value = "START";
            myexcelWorksheet1.Cells[cnt1 + 1, 8].Value = input;
            myexcelApplication1.DisplayAlerts = false;
            myexcelWorkbook1.SaveAs("C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\" + "TestRunner\\TestRunner.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            myexcelApplication1.DisplayAlerts = true;

            myexcelWorkbook1.Close();
            myexcelApplication1.Quit();

            releaseObject(myexcelWorksheet1);
            //  releaseObject(myexcelWorksheet_val1);
            releaseObject(myexcelWorkbook1);
            releaseObject(myexcelApplication1);




            //string str13 = null;
            //string str14 = null;
            //string str8;
            //string str97;
            //string str98;
            //string str7;











            //System.Data.DataTable dt2 = new DataTable();
            //dt2.Clear();
            //string str = "";
            //dt2.Columns.Add("ExecuteFlag");
            //dt2.Columns.Add("Action");

            //DataGridViewComboBoxColumn Action = new DataGridViewComboBoxColumn();
            //var list11 = new List<string>() { "I", "NC", "A", "V", "dbc", "Read", "Write" };
            //Action.DataSource = list11;
            //Action.HeaderText = "Action";
            //Action.DataPropertyName = "Action";


            //dt2.Columns.Add("LogicalName");
            //dt2.Columns.Add("ControlName");
            //dt2.Columns.Add("ControlType");
            //dt2.Columns.Add("ControlID");
            //dt2.Columns.Add("ImageType");
            //dt2.Columns.Add("Index");
            //dt2.Columns.Add("DynamicIndex");
            //dt2.Columns.Add("ColumnName");
            //dt2.Columns.Add("RowNo");
            //dt2.Columns.Add("ColumnNo");

            //dt2.Rows.Add("Y", "I", "Browser", "", "BrowserType", "", "", "", "", "", "", "");
            //dt2.Rows.Add("Y", "I", "AppURL", "", "URL", "", "", "", "", "", "", "");
            //dt2.Rows.Add("Y", "NC", "Wait", "5", "Wait", "", "", "", "", "", "", "");
            //dt2.Rows.Add("Y", "I", "BrowserAuth", "", "BrowserAuth", "LinkText", "", "", "", "", "", "");
            //dt2.Rows.Add("Y", "NC", "Wait", "5", "Wait", "", "", "", "", "", "", "");



            ////Microsoft.Office.Interop.Excel.Application myexcelApplication2 = new Microsoft.Office.Interop.Excel.Application();
            ////Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook2;
            ////  if (flag == "new")
            ////{
            //// myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //// myexcelApplication.ActiveWorkbook.SaveAs(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestSuites\\Web\\" + textBox7.Text + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            ////  myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestRunner\\TestRunner.xls");//.Add();
            //// @"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            ////}
            //input = SwdBrowser._driver.Title;

            //else if (flag == "open")
            //{
            //  myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(Projectpath + "\\" + ProjectName + "\\Projects\\" + ProjectName + "\\TestSuites\\" + "Web" + "\\" + ProjectName + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //myexcelWorkbook2 = myexcelApplication2.Workbooks.Open(Server.MapPath("C:\\TestAutomation\\Projects\\" + TreeView1.SelectedNode.Text + "\\Test Management\\" + "TestSuites\\" + "Web" + "\\" + input + ".xls"));
            //   Projectpath + "\\" + ProjectName + "\\Projects\\" + ProjectName + "\\ConfigureTestSuites\\Configure_TestSuites.xls");//.Add();
            //}
            //  Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook1 = myexcelApplication1.Workbooks.Open(@"C:\TestAutomation\Create Testcase.xls");//.Add();
            //Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet2 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook2.Worksheets.get_Item(1);
            //      Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val1 = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook1.Worksheets.get_Item(1);
            //  int cnt2;
            //myexcelWorksheet2.Cells.Clear();
            // cnt2 = myexcelWorksheet2.Cells.Find("*", System.Reflection.Missing.Value,
            //              System.Reflection.Missing.Value, System.Reflection.Missing.Value,
            //            Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious,
            //          false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;
            //Range excelRange = myexcelWorksheet2.UsedRange;

            //object[,] valueArray = (object[,])excelRange.get_Value(
            //XlRangeValueDataType.xlRangeValueDefault);
            //string logicalname = null;
            //string pysicalname = null;
            //string elementype = null;
            //string elementag = null;

            //int rowss = myexcelWorksheet2.UsedRange.Rows.Count;
            ////public string str10;
            //for (int row = 2; row <= rowss; row++)
            //// foreach (var row1 in myexcelWorksheet2.UsedRange.Rows)

            //{
            //    logicalname = myexcelWorksheet2.UsedRange[row, 1].Value;
            //    if (myexcelWorksheet2.UsedRange[row, 2].Value != null || myexcelWorksheet2.UsedRange[row, 2].Value != "")
            //        pysicalname = myexcelWorksheet2.UsedRange[row, 2].Value;
            //    else if (myexcelWorksheet2.UsedRange[row, 3].Value != null || myexcelWorksheet2.UsedRange[row, 3].Value != "")
            //        pysicalname = myexcelWorksheet2.UsedRange[row, 3].Value;
            //    else if (myexcelWorksheet2.UsedRange[row, 4].Value != null || myexcelWorksheet2.UsedRange[row, 4].Value != "")
            //        pysicalname = myexcelWorksheet2.UsedRange[row, 4].Value;

            //    if (myexcelWorksheet2.UsedRange[row, 1].Value != null)
            //    {
            //        elementype = myexcelWorksheet2.UsedRange[row, 1].Value;
            //        if (elementype.Contains("text") || elementype.Contains("textarea") || elementype.Contains("email") || elementype.Contains("phone"))
            //        {
            //            elementype = "WebEdit";

            //        }
            //        else if (elementype.Contains("button") || elementype.Contains("submit"))
            //        {
            //            elementype = "WebButton";
            //        }
            //        else if (elementype.Contains("checkbox"))
            //        {
            //            elementype = "CheckBox";
            //        }
            //        else if (elementype.Contains("radio"))
            //        {
            //            elementype = "Radio";
            //        }
            //        else if (elementype.Contains("select"))
            //        {
            //            elementype = "WebList";
            //        }
            //        else if (elementype.Contains("link"))
            //        {
            //            elementype = "WebLink";
            //        }
            //        else if (elementype.Contains("a") || elementype.Contains("href"))
            //        {
            //            elementype = "WebLink";
            //        }
            //        else
            //        {
            //            elementype = "WebElement";
            //        }
            //    }

            //    else if (myexcelWorksheet2.UsedRange[row, 2].Value != "null")
            //    {
            //        pysicalname = myexcelWorksheet2.UsedRange[row, 2].Value;
            //        elementag = "Id";
            //    }
            //    else if (myexcelWorksheet2.UsedRange[row, 3].Value != "null")
            //    {
            //        pysicalname = myexcelWorksheet2.UsedRange[row, 3].Value;
            //        elementag = "XPath";
            //    }
            //    else if (myexcelWorksheet2.UsedRange[row, 4].Value != "null")
            //    {
            //        pysicalname = myexcelWorksheet2.UsedRange[row, 4].Value;
            //        elementag = "CSS Selector";
            //    }

            //    dt2.Rows.Add("Y", "I", logicalname, pysicalname, elementype, elementag);
            //}
        
                //// dt2.Rows.Add("Y", str56, myexcelWorksheet2.UsedRange[row, col].Value.ToString(), row1.Cells[3].Value, row1.Cells[0].Value, row1.Cells[2].Value);
                //if (myexcelWorksheet2.UsedRange[row, 1].Value != null || myexcelWorksheet2.UsedRange[row, 2].Value != null || myexcelWorksheet2.UsedRange[row, 3].Value != null || myexcelWorksheet2.UsedRange[row, 4].Value != null)
                //{
                //    // MessageBox.Show(myexcelWorksheet2.UsedRange[row, 7].Value.ToString());
                //    //   MessageBox.Show(myexcelWorksheet2.UsedRange[row, 8].Value.ToString());
                //    //     MessageBox.Show(myexcelWorksheet2.UsedRange[row, 10].Value.ToString());
                //    //       MessageBox.Show(myexcelWorksheet2.UsedRange[row, 13].Value.ToString());
                //    if (myexcelWorksheet2.UsedRange[row, 1].Value == null)
                //    {
                //        str14 = "";
                //    }
                //    else
                //    {
                //        str14 = myexcelWorksheet2.UsedRange[row, 1].Value.ToString();
                //    }
                //    if (myexcelWorksheet2.UsedRange[row, 2].Value == null)
                //    {
                //        str8 = "";
                //    }
                //    else
                //    {
                //        str8 = myexcelWorksheet2.UsedRange[row, 2].Value.ToString();
                //    }
                //    if (myexcelWorksheet2.UsedRange[row, 3].Value == null)
                //    {
                //        str97 = "";
                //    }
                //    else
                //    {
                //        str97 = myexcelWorksheet2.UsedRange[row, 3].Value.ToString();
                //    }
                //    if (myexcelWorksheet2.UsedRange[row, 4].Value == null)
                //    {
                //        str98 = "";
                //    }
                //    else
                //    {
                //        str98 = myexcelWorksheet2.UsedRange[row, 4].Value.ToString();
                //    }


                //    if (myexcelWorksheet2.UsedRange[row, 1].Value == null)
                //    {
                //        str7 = "";
                //    }
                //    else
                //    {
                //        //string str45= myexcelWorksheet2.UsedRange[row, 33].Value.ToString();
                //        //if(str45)
                //        str7 = myexcelWorksheet2.UsedRange[row, 1].Value.ToString();
                //        if (str7.Contains("text") || str7.Contains("email"))
                //        {

                //            str7 = "Enter text in " + str7.Substring(3);
                //            str13 = "WebEdit";
                //        }
                //        else if (str7.Contains("button") || str7.Contains("submit"))
                //        {

                //            str7 = "Click on button" + str7.Substring(3);
                //            str13 = "WebButton";

                //        }

                //        else if (str7.Contains("checkbox"))
                //        {

                //            str7 = "Click on checkbox" + str7.Substring(3);
                //            str13 = "CheckBox";

                //        }

                //        else if (str7.Contains("radio") || str7.Contains("option"))
                //        {

                //            str7 = "Click on radio " + str7.Substring(3);
                //            str13 = "Radio";

                //        }
                //        else if (str7.Contains("select"))
                //        {

                //            str7 = "Select Item" + str7.Substring(3);
                //        }
                //        else if (str7.Contains("a"))
                //        {

                //            str7 = "Click on link" + str7.Substring(3);
                //            str13 = "WebLink";
                //        }
                //        else
                //        {
                //            str13 = "WebElement";

                //        }

                //    }

                //    //  string str10;
                //    //myexcelWorksheet2.UsedRange[row, col].Value.ToString()

                //}

                //else
                //{
                //    break;
                //}

            



            //}
            // Insert into Temporary DataTable.
            // dt2.Rows.Add("Y", "I", row.Cells[2].Value, row.Cells[3].Value, str,"XPath");
            // Insert into DataBase.
            //   InsertInToDataBase(row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value);
            //}


          //  DataTable dt = (DataTable)dataGridView2.DataSource;
            //     dt.Merge(dt2);
            //dataGridView2.DataSource = dt2;




            //   Microsoft.VisualBasic.Interaction.InputBox("Question?", "Title", "Default Text");

            //Microsoft.Office.Interop.Excel.Application myexcelApplication = new Microsoft.Office.Interop.Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook = myexcelApplication.Workbooks.Open("C:\\TestAutomation\\Projects\\" + TreeView1.SelectedNode.Text + "\\Test Management\\"  + "TestSuites\\Web\\" +input+ ".xls");//.Add();
            //Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(1);
            //Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(2);

            //myexcelWorksheet.Rows.Clear();

            //myexcelWorksheet.Cells[1, 1] = "ExecuteFlag";
            ////  }
            ////  else if (item.Name != "")
            ////  {
            //myexcelWorksheet.Cells[1, 2] = "Action";
            //myexcelWorksheet.Cells[1, 3] = "LogicalName";
            //myexcelWorksheet.Cells[1, 4] = "ControlName";
            //myexcelWorksheet.Cells[1, 5] = "ControlType";
            //myexcelWorksheet.Cells[1, 6] = "ControlID";
            //myexcelWorksheet.Cells[1, 7] = "ImageType";
            //myexcelWorksheet.Cells[1, 8] = "Index";
            //myexcelWorksheet.Cells[1, 9] = "DynamicIndex";
            //myexcelWorksheet.Cells[1, 10] = "ColumnName";
            //myexcelWorksheet.Cells[1, 11] = "RowNo";
            //myexcelWorksheet.Cells[1, 12] = "ColumnNo";
            ////  myexcelWorksheet.Cells[1, 6] = "XPATH";

            //int cnt = 12;//=HeaderNames.Count();
            //int cntc = 0;

            //// myexcelWorkbook.Worksheets.Add(dt2, "Structure");
            //int rownscnt = myexcelWorksheet.UsedRange.Rows.Count;

            //for (int rowsscnt = 0; rowsscnt < dt2.Rows.Count - 1; rowsscnt++)
            //{

            //    for (int col = 0; col < dt2.Columns.Count; col++)
            //    {

            //        myexcelWorksheet.Cells[rowss + 2, col + 1] = dt2.Rows[rowsscnt][col];//.ToString();


            //    }


            //    //    // label20.Text = s1;
            //    }

            //    int u = 3;
            //int rowss = myexcelWorksheet_val.UsedRange.Rows.Count;

            //for (rowss = 0; rowss < dataGridView2.Rows.Count - 1; rowss++)
            //    {

            //        try
            //        {
            //            if (myexcelWorksheet.Cells[rowss + 2, 3].value != null)
            //            {
            //                myexcelWorksheet_val.Cells[1, u] = myexcelWorksheet.Cells[rowss + 2, 3].value;

            //                myexcelWorksheet_val.Cells[2, 1] = input;
            //                myexcelWorksheet_val.Cells[2, 2] = input;
            //                myexcelWorksheet_val.Cells[2, 3] = "Chrome";
            //                myexcelWorksheet_val.Cells[2, 4] = "";
            //                myexcelWorksheet_val.Cells[2, 5] = "";
            //                str = myexcelWorksheet.Cells[rowss + 2, 3].value.ToString();
            //                if (strtag != "hidden")
            //                {
            //                    if (str.Contains("Enter"))
            //                    {
            //                        if (str.Contains("Firm"))
            //                        {
            //                            //   Mirage.Generators.CompanyAttribute  robj= new Mirage.Generators.CompanyAttribute();

            //                            //  robj.ToString();
            //                            var cn = new Bogus.DataSets.Company(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            //   myexcelWorksheet_val.Cells[2, u] = cn.CompanyName(1);
            //                            //"Quantum";

            //                        }
            //                        if (str.Contains("First"))
            //                        {
            //                            var fn = new Bogus.DataSets.Name(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            // myexcelWorksheet_val.Cells[2, u] = fn.FirstName();
            //                            //  myexcelWorksheet_val.Cells[2, u ] = "Smith";
            //                        }
            //                        if (str.Contains("Last"))
            //                        {
            //                            var ln = new Bogus.DataSets.Name(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            // myexcelWorksheet_val.Cells[2, u] = ln.LastName();

            //                            // myexcelWorksheet_val.Cells[2, u] = "David";
            //                        }
            //                        if (str.Contains("Nick"))
            //                        {
            //                            var ln = new Bogus.DataSets.Name(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            //myexcelWorksheet_val.Cells[2, u] = ln.LastName();

            //                            // myexcelWorksheet_val.Cells[2, u ] = "Dave";
            //                        }
            //                        if (str.Contains("Address"))
            //                        {
            //                            var ln = new Bogus.DataSets.Address(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            // myexcelWorksheet_val.Cells[2, u] = ln.StreetName();
            //                            // myexcelWorksheet_val.Cells[2, u ] = "1400 American Lane";
            //                        }
            //                        if (str.Contains("Zipcode"))
            //                        {
            //                            var ln = new Bogus.DataSets.Address(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            //myexcelWorksheet_val.Cells[2, u] = ln.ZipCode();
            //                            // myexcelWorksheet_val.Cells[2, u ] = "60196";
            //                        }
            //                        if (str.Contains("Title"))
            //                        {


            //                            //  myexcelWorksheet_val.Cells[2, u ] = "Mr.";
            //                        }
            //                        if (str.Contains("Ext"))
            //                        {
            //                            //myexcelWorksheet_val.Cells[2, u] = "1234";
            //                        }
            //                        if (str.Contains("Phone"))
            //                        {
            //                            var ln = new Bogus.DataSets.PhoneNumbers(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            // myexcelWorksheet_val.Cells[2, u] = ln.PhoneNumber();
            //                            //  myexcelWorksheet_val.Cells[2, u ] = "9874561234";
            //                        }
            //                        if (str.Contains("Fax"))
            //                        {
            //                            var ln = new Bogus.DataSets.PhoneNumbers(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            // myexcelWorksheet_val.Cells[2, u] = ln.PhoneNumber();
            //                            //  myexcelWorksheet_val.Cells[2, u ] = "9874561235";
            //                        }
            //                        if (str.Contains("email"))
            //                        {
            //                            //   var ln = new Bogus.DataSets.Lorem(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            // myexcelWorksheet_val.Cells[2, u] = ln
            //                            // myexcelWorksheet_val.Cells[2, u] = "test@test.com";
            //                        }
            //                        if (str.Contains("Birth"))
            //                        {
            //                            // var ln = new Bogus.DataSets.Date(locale: "en");

            //                            // string fd = td.CompanyName(1);
            //                            //myexcelWorksheet_val.Cells[2, u] = ln.();
            //                            // myexcelWorksheet_val.Cells[2, u] = "12/12/1987";
            //                        }
            //                        if (str.Contains("estabilsh"))
            //                        {
            //                            // myexcelWorksheet_val.Cells[2, u] = "12/12/2021";
            //                        }
            //                        if (str.Contains("Date"))
            //                        {
            //                            // myexcelWorksheet_val.Cells[2, u] = "12/12/2021";
            //                        }
            //                        if (str.Contains("Attorney"))
            //                        {
            //                            // myexcelWorksheet_val.Cells[2, u] = "1";
            //                        }
            //                    }
            //                    else if (str.Contains("Click"))
            //                    {
            //                        if (str.Contains("Webelement"))
            //                        {
            //                            string[] arr = str.Split(':');

            //                            myexcelWorksheet_val.Cells[2, u] = arr[1];
            //                        }
            //                        else
            //                        {

            //                            // if (str.Contains("CheckBox") || str.Contains("Radio") || str.Contains("Link"))
            //                            //{
            //                            //  myexcelWorksheet_val.Cells[2, u] = "Y";
            //                        }
            //                        //}
            //                    }

            //                    u = u + 1;
            //                }
            //            }
            //        }

            //        catch (Exception e1)
            //        {
            //            MessageBox.Show(e1.Message);

            //        }

            //    }


            //    myexcelApplication.DisplayAlerts = false;
            //    //   if (flag == "new")
            //    // {
            //    // myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //    //   myexcelApplication.ActiveWorkbook.SaveAs(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestSuites\\Web\\" + textBox7.Text+"\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //    // @"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //    //}
            //    //else if (flag == "open")
            //    //{
            //    //  myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //    myexcelApplication.ActiveWorkbook.SaveAs("C:\\TestAutomation\\Projects\\" + TreeView1.SelectedNode.Text + "\\Test Management\\" + "TestSuites\\" + "Web" + "\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //    //}
            //    myexcelApplication.DisplayAlerts = true;
            //    myexcelWorkbook.Close();
            //    myexcelApplication.Quit();

            //    releaseObject(myexcelWorksheet);
            //    releaseObject(myexcelWorksheet_val);
            //    releaseObject(myexcelWorkbook);
            //    releaseObject(myexcelApplication);

            //    //   MessageBox.Show("Test case " + input + " saved sucessfully");

            //    //  }
            //    //  else
            //    //{
            //    //  MessageBox.Show("Please select automation suite");
            //    //}



            //    //for (int col = 7; col <= 13; col++)
            //    //{
            //    //    if (valueArray[row, 7]!= null)
            //    //    {
            //    //        if (valueArray[row, col] != null)
            //    //            Debug.Print(valueArray[row, col].ToString());


            //    //    }
            //    //    else
            //    //    {
            //    //        break;
            //    //    }

            //    //myexcelApplication2.DisplayAlerts = true;
            //    //myexcelWorkbook2.Close();
            //    //myexcelApplication2.Quit();

            //    //releaseObject(myexcelWorksheet2);
            //    ////  releaseObject(myexcelWorksheet_val);
            //    //releaseObject(myexcelWorkbook2);
            //    //releaseObject(myexcelApplication2);

                MessageBox.Show("Testcase got created sucessfully.");

                //}
            

            //   myexcelWorkbook2 = myexcelApplication2.Workbooks.Open(Environment.CurrentDirectory + "\\Bin\\export.xlsx");

        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
               // MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        protected void StartZip(string strPath, string strFileName)
        {
            //MemoryStream ms = null;
            //Response.ContentType = "application/octet-stream";
            //strFileName = HttpUtility.UrlEncode(strFileName).Replace('+', ' ');
            //Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName + ".zip");
            //ms = new MemoryStream();
            //zos = new ZipOutputStream(ms);
            //strBaseDir = strPath + "\\";
            //addZipEntry(strBaseDir);
            //zos.Finish();
            //zos.Close();
            //Response.Clear();
            //Response.BinaryWrite(ms.ToArray());
            //Response.End();
        }

      //  ZipOutputStream zos;
        String strBaseDir;

      
        private static void SendFile(Socket client, String path, int FilesLeft)
        {
            byte[] filesLeft = BitConverter.GetBytes((Int64)FilesLeft);
            byte[] fileData = File.ReadAllBytes(path);
            byte[] fileLength = BitConverter.GetBytes((Int64)fileData.Length);
            byte[] fileMD5;

            using (MD5 md5Hash = MD5.Create())
            {
                fileMD5 = md5Hash.ComputeHash(fileData);
            }

            byte[] filepathdata = Encoding.Unicode.GetBytes(path);
            byte[] filepathLength = BitConverter.GetBytes((Int16)filepathdata.Length);

            byte[] byteData = filesLeft.Concat(fileLength).Concat(fileMD5).Concat(filepathLength).Concat(filepathdata).Concat(fileData).ToArray();

            Console.WriteLine("STEP 2 - File length: {0}", fileData.Length);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }
        private static ManualResetEvent connectDone =
    new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("STEP 3 - Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void SendAll(Socket client, String path)
        {
            String[] files =Directory. GetFiles(path);

            int i = 0;
            foreach (String file in files)
            {
                Console.WriteLine("STEP 1 - Files left: {0}", files.Length - i);
                SendFile(client, Path.GetFullPath(file), files.Length - i);
                sendDone.WaitOne();

                i++;
            }
        }
        System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;

        protected void Button7_Click(object sender, EventArgs e)
        {

            //   SendAll()



          
                string FileName = "Node Configuration.zip"; // It's a file name displayed on downloaded file on client side.

                response.ClearContent();
                response.Clear();
                response.ContentType = "*/*";
                response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ";");
                response.TransmitFile(Server.MapPath("~/bin/Node Configuration/Node Configuration.zip"));
                response.Flush();
                response.End();
            response.Redirect("C://Temp//Node Configuration.zip");

            //Thread.Sleep(50000);


            // Button7.Text = "Download Node";
        }
        // Thread.Sleep(50000);


        // Response.Redirect("~/bin/TestAutomation/selenium.zip");

        //StartZip(Server.MapPath("~/bin/TestAutomation/"), "selenium.zip");

        //CopyDirectory(Server.MapPath("~/bin/TestAutomation/selenium/"), "C:\\Selenium Server\\", true);

        //var folder = "C:\\Selenium Server\\";// + TreeView1.SelectedNode.Text + "\\" + TextBox3.Text;
        //if (!Directory.Exists(folder))
        //{
        //    Directory.CreateDirectory(folder);
        //}

        //   string source_dir = Server.MapPath("~/bin/TestAutomation/selenium");
        // string destination_dir =@"C:\\Selenium Server\\";

        //    // substring is to remove destination_dir absolute path (E:\).

        //    // Create subdirectory structure in destination    
        //    foreach (string dir in System.IO.Directory.GetDirectories(source_dir, "*", System.IO.SearchOption.AllDirectories))
        //    {
        //        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(destination_dir, dir.Substring(source_dir.Length + 1)));
        //        // Example:
        //        //     > C:\sources (and not C:\E:\sources)
        //    }

        //    foreach (string file_name in System.IO.Directory.GetFiles(source_dir, "*", System.IO.SearchOption.AllDirectories))
        //    {
        //        System.IO.File.Copy(file_name, System.IO.Path.Combine(destination_dir, file_name.Substring(source_dir.Length + 1)));
        //    }

        //    // var   _ip = HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? HttpContext.Current.Request.UserHostAddress;
        //    string Command = " java - jar selenium - server - standalone - 3.141.59.jar - role node - hub http://192.168.1.10:4444/grid/register -port 5555";
        //string str_Path = Server.MapPath("~/bin/TestAutomation/selenium/node.bat");

        //ProcessStartInfo processInfo = new ProcessStartInfo(str_Path);

        //processInfo.UseShellExecute = false;

        //Process batchProcess = new Process();

        //batchProcess.StartInfo = processInfo;

        //batchProcess.Start();





        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        protected void Execute_Click(object sender, EventArgs e)
        {

            
          //  ZipFile.Create(@"C:\TestAutomation");


        }


        //private static void AddFileToZip(string zipFilename, string fileToAdd)
        //{
        //    using (Package zip = System.IO.Packaging.Package.Open(zipFilename, FileMode.OpenOrCreate))
        //    {
        //        string destFilename = ".\\" + Path.GetFileName(fileToAdd);
        //        Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
        //        if (zip.PartExists(uri))
        //        {
        //            zip.DeletePart(uri);
        //        }
        //        PackagePart part = zip.CreatePart(uri, "", CompressionOption.Normal);
        //        using (FileStream fileStream = new FileStream(fileToAdd, FileMode.Open, FileAccess.Read))
        //        {
        //            using (Stream dest = part.GetStream())
        //            {
        //                CopyStream(fileStream, dest);
        //            }
        //        }
        //    }
        //}
        private const long BUFFER_SIZE = 4096;

        private static void CopyStream(System.IO.FileStream inputStream, System.IO.Stream outputStream)
        {
            long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bytesRead;
            }
        }
        private void DownloadFolderAsZip()
        {
            // Path to the folder you want to download
          

            // End the response
            Response.End();
        }

        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            string folderToZipPath = Server.MapPath("~/FolderToZip");
            string zipFilePath = Server.MapPath("~/Download/FolderToZip.zip");

            // Zip the folder
       //     ZipFolder(folderToZipPath, zipFilePath);

            // Clear the response
            Response.Clear();
            Response.ContentType = "application/zip";
            Response.AppendHeader("Content-Disposition", "attachment; filename=FolderToZip.zip");
            Response.TransmitFile(zipFilePath);
            Response.End();

        }
        private TreeNode FindRootNode(TreeNode treeNode)
        {
            while (treeNode.Parent != null)
            {
                treeNode = treeNode.Parent;
            }
            return treeNode;
        }

        //  public TreeNode RootTreeNode(TreeNode n) { while (n.Level > 0) { n = n.Parent; } return n; }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
           // Label6.Text = "Root Node: " + TreeView1.SelectedNode..Text;
          //  TreeNode selectedNode = TreeView1.SelectedNode.Parent;

            //TreeNode rootNode = FindRootNode(selectedNode);

         //   Console.Write(rootNode.Text);
            //Label6.Text = "Root Node: " + rootNode.Text;

            //   MessageBox.Show(TreeView1.Parent.Parent.Parent.Parent.ToString());

            //System.Diagnostics.Process.Start("C:\\TestAutomation\\Projects\\"+ rootNode.Text + "\\Test Management\\TestSuites\\Web\\"+TreeView1.SelectedNode.Text);
            ////Create a new workbook
            //Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            ////Load a file and imports its data
            //workbook.LoadFromFile(@"C:/TestAutomation/Projects/TestManagement/TestSuits/Web/"+TreeView1.SelectedNode.Text+".xls");
            ////Initialize worksheet
            //Spire.Xls.Worksheet sheet = workbook.Worksheets[0];
            //// get the data source that the grid is displaying data for
            //this.dataGridView1.DataSource = sheet.ExportDataTable();
        }

        protected void Downloadlocal_Click(object sender, EventArgs e)
        {
            flag = "CF";
            path = "C:\\TestAutomation\\Projects\\"+DropDownList1.SelectedItem.Text+ "\\Test Management\\ConfigureTestSuites\\Configure_TestSuites.xls";

            Web_Transaction_Input_Files(path);
            //string path ="C:\\TestAutomation\\Projects\\"+DropDownList1.SelectedItem.Text;//Location for inside Test Folder  
            //string[] Filenames = Directory.GetFiles(path);
            //using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            //{
            //    zip.AddFiles(Filenames, "Project");//Zip file inside filename  
            //    zip.Save(@"C:\TestAutomation\Projects\" + DropDownList1.SelectedItem.Text + ".zip");//location and name for creating zip file  

            //}

            //string FileName = DropDownList1.SelectedItem.Text + ".zip"; // It's a file name displayed on downloaded file on client side.

            //response.ClearContent();
            //response.Clear();
            //response.ContentType = "*/*";
            //response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ";");
            //response.TransmitFile(@"C:\TestAutomation\Projects\"+DropDownList1.SelectedItem.Text+".zip");
            //response.Flush();
            //response.End();
            //Button7.Text = "Redirect jar";
            //response.Redirect("C://TestAutomation//Projects//"+ DropDownList1.SelectedItem.Text+".zip");


      
}


        protected void Run_Click(object sender, EventArgs e)
        {


            string batname = "execute.bat";
            //  string theedit = "Testing one two three four\n\nfive six seven eight.";
            string theedit = "@echo off"+Environment.NewLine + "Jarus-Selenium_lat-remote_new2.jar " + "\"" + "C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\Settings\\settings.xls"+"\"";
            using (StreamWriter sw = File.CreateText(Server.MapPath("~/bin/TestAutomation/execute.bat")))
            {
                using (StringReader reader = new StringReader(theedit))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            sw.WriteLine(line);
                        }

                    } while (line != null);
                }
            }


            Process p = new Process();
            p.StartInfo.FileName = Server.MapPath("~/bin/TestAutomation/execute.bat");
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Server.MapPath("~/bin/TestAutomation/execute.bat"));// C://TestAutomation//Projects//" + TreeView1.SelectedNode.Text + "//execute.bat");
            p.StartInfo.UseShellExecute = false;// Server.MapPath("~/bin/TestAutomation")

            // Run the process and wait for it to complete
            p.Start();
            p.WaitForExit();

            //System.Diagnostics.Process process = null;
            ////{

            //process = System.Diagnostics.Process.Start("C://TestAutomation//Projects//" + TreeView1.SelectedNode.Text+"//execute.bat");

        }

        public void HandleOnGridViewRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (e.NewValues["CategoryName"] != null)
            {
                String newCategoryName = e.NewValues["CategoryName"].ToString();
                // process the data; 

            }
        }
        private Workbook workbook = new Workbook();


        protected void Extract_Click(object sender, EventArgs e)
        {
            string filePath = @"C:\TestAutomation\Projects\ALPSPHONEX_NEW_LATEST\Projects\ALPSPHONEX_NEW_LATEST\TestSuites\Web\ALPSPHONEX_NEW_LATEST\Create_Firm.xls";

            System.Diagnostics.Process.Start(@"C:\TestAutomation\Projects\ALPSPHONEX_NEW_LATEST\Projects\ALPSPHONEX_NEW_LATEST\TestSuites\Web\ALPSPHONEX_NEW_LATEST\Create_Firm.xls");

            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    Console.WriteLine(temp.GetString(b));
                }

                try
                {
                    // Try to write to the file.  
                    fs.Write(b, 0, b.Length);
                }
                catch (Exception e1)
                {
                    Console.WriteLine("Writing was disallowed, as expected: {0}", e.ToString());
                }
            }

            File.Open(@"C:\TestAutomation\Projects\ALPSPHONEX_NEW_LATEST\Projects\ALPSPHONEX_NEW_LATEST\TestSuites\Web\ALPSPHONEX_NEW_LATEST/Create_Firm.xls",FileMode.Open);
      //      //Create a new workbook
      ////    Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
      //      //Load a file and imports its data
      //      workbook.LoadFromFile(@"C:\TestAutomation\Projects\ALPSPHONEX_NEW_LATEST\Projects\ALPSPHONEX_NEW_LATEST\TestSuites\Web\ALPSPHONEX_NEW_LATEST/Create_Firm.xls");
      //      //Initialize worksheet
      //      Spire.Xls.Worksheet sheet = workbook.Worksheets[0];
      //      // get the data source that the grid is displaying data for
      //      this.GridView1.DataSource = sheet.ExportDataTable();
      //      GridView1.DataBind();
        }
        public string labelname ;
        //public string path
        //{
        //    get
        //    {
        //        var value = ViewState["path"];
        //        return null ;
        //    }
        //    set
        //    {
        //        ViewState["path"] = value;
        //    }
        //}

        private void BindGridView()
        {


            //string filePath = @"C:\TestAutomation\Projects\googlesearch\Test Management\TestSuites\Web\" + "Create_Firm.xls";
            //Response.ContentType = "Application/vnd.ms-excel";
            //Response.AppendHeader("content-disposition",
            //"attachment; filename=" + "Create_Firm.xls");
            //Response.TransmitFile(filePath);
            //Response.End();
           // string path = @"C:\TestAutomation\Projects\googlesearch\Test Management\TestSuites\Web\Create_Firm_sample.xls";
            //Coneection String by default empty  
            string ConStr = "";
            //Extantion of the file upload control saving into ext because   
            //there are two types of extation .xls and .xlsx of Excel   
            string ext = ".xls"; //Path.GetExtension(FileUpload1.FileName).ToLower();
                                 //getting the path of the file   
                                 //  path = Server.MapPath("~/bin/" + FileUpload1.FileName);
                                 //saving the file inside the MyFolder of the server  
                                 //  FileUpload1.SaveAs(path);
                                 // Label1.Text = FileUpload1.FileName + "\'s Dasta showing into the GridView";
                                 //labelname = FileUpload1.FileName;
                                 //checking that extantion is .xls or .xlsx  
            if (ext.Trim() == ".xls")
            {
                //connection string for that file which extantion is .xls  
                ConStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if (ext.Trim() == ".xlsx")
            {
                //connection string for that file which extantion is .xlsx  
                ConStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            }
            //making query  
            string query = "SELECT * FROM [Values$]";
            //Providing connection  
            OleDbConnection conn = new OleDbConnection(ConStr);
            //checking that connection state is closed or not if closed the   
            //open the connection  
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            //create command object  
            OleDbCommand cmd = new OleDbCommand(query, conn);
            // create a data adapter and get the data into dataadapter  
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();
            //fill the Excel data to data set  
            da.Fill(ds);
            //set data source of the grid view  
            gridDView.DataSource = ds.Tables[0];
            //binding the gridview  
            gridDView.DataBind();
            // FileUpload1.SaveAs(path);
            //close the connection  
            conn.Close();


            //using (SqlConnection con = new SqlConnection(connectionString))
            //{
            //    con.Open();
            //    string sql = "SELECT * FROM BoQ_Upload";
            //    SqlCommand cmd = new SqlCommand(sql, con);
            //    cmd.ExecuteNonQuery();

            //    SqlDataAdapter ad = new SqlDataAdapter(cmd);
            //    DataTable dt = new DataTable();
            //    ad.Fill(dt);
            //    con.Close();

            //    gridDView.DataSource = dt;
            //    gridDView.DataBind();
            //}
        }


        public void datadisplay(string path)
        {


            //string filePath = @"C:\TestAutomation\Projects\googlesearch\Test Management\TestSuites\Web\" + "Create_Firm.xls";
            //Response.ContentType = "Application/vnd.ms-excel";
            //Response.AppendHeader("content-disposition",
            //"attachment; filename=" + "Create_Firm.xls");
            //Response.TransmitFile(filePath);
            //Response.End();
            //Coneection String by default empty  
          //   path = @"C:\TestAutomation\Projects\googlesearch\Test Management\TestSuites\Web\Create_Firm_sample.xls";

            string ConStr = "";
            //Extantion of the file upload control saving into ext because   
            //there are two types of extation .xls and .xlsx of Excel   
            string ext = ".xls"; //Path.GetExtension(FileUpload1.FileName).ToLower();
            //getting the path of the file   
           //  path = Server.MapPath("~/bin/" + FileUpload1.FileName);
            //saving the file inside the MyFolder of the server  
          //  FileUpload1.SaveAs(path);
           // Label1.Text = FileUpload1.FileName + "\'s Dasta showing into the GridView";
            //labelname = FileUpload1.FileName;
            //checking that extantion is .xls or .xlsx  
            if (ext.Trim() == ".xls")
            {
                //connection string for that file which extantion is .xls  
                ConStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if (ext.Trim() == ".xlsx")
            {
                //connection string for that file which extantion is .xlsx  
                ConStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            }
            //making query  
            string query = "SELECT * FROM [Values$]";
            //Providing connection  
            OleDbConnection conn = new OleDbConnection(ConStr);
            //checking that connection state is closed or not if closed the   
            //open the connection  
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            //create command object  
            OleDbCommand cmd = new OleDbCommand(query, conn);
            // create a data adapter and get the data into dataadapter  
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();
            //fill the Excel data to data set  
            da.Fill(ds);
            //set data source of the grid view  
            gridDView.DataSource = ds.Tables[0];
           // GridView1.DataSource = ds.Tables[0];
            //binding the gridview  
            gridDView.DataBind();
           // GridView1.DataBind();
            // FileUpload1.SaveAs(path);
            //close the connection  
            conn.Close();
            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < procs.Length; i++)
            {
                if (procs[i].ProcessName == "EXCEL")
                {
                    procs[i].Kill();
                }
            }

        }

        public void TestRunner(string path)
        {


            //string filePath = @"C:\TestAutomation\Projects\googlesearch\Test Management\TestSuites\Web\" + "Create_Firm.xls";
            //Response.ContentType = "Application/vnd.ms-excel";
            //Response.AppendHeader("content-disposition",
            //"attachment; filename=" + "Create_Firm.xls");
            //Response.TransmitFile(filePath);
            //Response.End();
            //Coneection String by default empty  
            //   path = @"C:\TestAutomation\Projects\googlesearch\Test Management\TestSuites\Web\Create_Firm_sample.xls";

            string ConStr = "";
            //Extantion of the file upload control saving into ext because   
            //there are two types of extation .xls and .xlsx of Excel   
            string ext = ".xls"; //Path.GetExtension(FileUpload1.FileName).ToLower();
                                 //getting the path of the file   
                                 //  path = Server.MapPath("~/bin/" + FileUpload1.FileName);
                                 //saving the file inside the MyFolder of the server  
                                 //  FileUpload1.SaveAs(path);
                                 // Label1.Text = FileUpload1.FileName + "\'s Dasta showing into the GridView";
                                 //labelname = FileUpload1.FileName;
                                 //checking that extantion is .xls or .xlsx  
            if (ext.Trim() == ".xls")
            {
                //connection string for that file which extantion is .xls  
                ConStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if (ext.Trim() == ".xlsx")
            {
                //connection string for that file which extantion is .xlsx  
                ConStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            }
            //making query  
            string query = "SELECT * FROM [MainControlSheet$]";
            //Providing connection  
            OleDbConnection conn = new OleDbConnection(ConStr);
            //checking that connection state is closed or not if closed the   
            //open the connection  
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            //create command object  
            OleDbCommand cmd = new OleDbCommand(query, conn);
            // create a data adapter and get the data into dataadapter  
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();
            //fill the Excel data to data set  
            da.Fill(ds);
            //set data source of the grid view  
            gridDView.DataSource = ds.Tables[0];
            // GridView1.DataSource = ds.Tables[0];
            //binding the gridview  
            gridDView.DataBind();
            // GridView1.DataBind();
            // FileUpload1.SaveAs(path);
            //close the connection  
            conn.Close();
            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < procs.Length; i++)
            {
                if (procs[i].ProcessName == "EXCEL")
                {
                    procs[i].Kill();
                }
            }

        }
        public void Web_Transaction_Input_Files(string path)
        {


            //string filePath = @"C:\TestAutomation\Projects\googlesearch\Test Management\TestSuites\Web\" + "Create_Firm.xls";
            //Response.ContentType = "Application/vnd.ms-excel";
            //Response.AppendHeader("content-disposition",
            //"attachment; filename=" + "Create_Firm.xls");
            //Response.TransmitFile(filePath);
            //Response.End();
            //Coneection String by default empty  
            //   path = @"C:\TestAutomation\Projects\googlesearch\Test Management\TestSuites\Web\Create_Firm_sample.xls";

            string ConStr = "";
            //Extantion of the file upload control saving into ext because   
            //there are two types of extation .xls and .xlsx of Excel   
            string ext = ".xls"; //Path.GetExtension(FileUpload1.FileName).ToLower();
                                 //getting the path of the file   
                                 //  path = Server.MapPath("~/bin/" + FileUpload1.FileName);
                                 //saving the file inside the MyFolder of the server  
                                 //  FileUpload1.SaveAs(path);
                                 // Label1.Text = FileUpload1.FileName + "\'s Dasta showing into the GridView";
                                 //labelname = FileUpload1.FileName;
                                 //checking that extantion is .xls or .xlsx  
            if (ext.Trim() == ".xls")
            {
                //connection string for that file which extantion is .xls  
                ConStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if (ext.Trim() == ".xlsx")
            {
                //connection string for that file which extantion is .xlsx  
                ConStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            }
            //making query  
            string query = "SELECT * FROM [Web_Transaction_Input_Files$]";
            //Providing connection  
            OleDbConnection conn = new OleDbConnection(ConStr);
            //checking that connection state is closed or not if closed the   
            //open the connection  
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            //create command object  
            OleDbCommand cmd = new OleDbCommand(query, conn);
            // create a data adapter and get the data into dataadapter  
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();
            //fill the Excel data to data set  
            da.Fill(ds);
            //set data source of the grid view  
            gridDView.DataSource = ds.Tables[0];
            // GridView1.DataSource = ds.Tables[0];
            //binding the gridview  
            gridDView.DataBind();
            // GridView1.DataBind();
            // FileUpload1.SaveAs(path);
            //close the connection  
            conn.Close();
            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < procs.Length; i++)
            {
                if (procs[i].ProcessName == "EXCEL")
                {
                    procs[i].Kill();
                }
            }

        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < procs.Length; i++)
            {
                if (procs[i].ProcessName == "EXCEL")
                {
                    procs[i].Kill();
                }
            }
            //  TreeView1.SelectedNode.Checked = true;
            path = "C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\TestSuites\\Web\\" + ListBox1.SelectedItem.Text;
            datadisplay(path);
            //spath = "C:\\TestAutomation\\Projects" + path;

            //datadisplay(spath);            // GridView1.EditIndex = e.NewEditIndex;
            System.Diagnostics.Process[] procs1 = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < procs1.Length; i++)
            {
                if (procs1[i].ProcessName == "EXCEL")
                {
                    procs1[i].Kill();
                }
            }
            // node.Checked = true;

            // //TreeView treeView = new TreeView();
            // var selectedNode = TreeView1.SelectedNode;

            // for (int i =0; i<= 100;i++)
            // {
            //     if (selectedNode != null)
            //     {
            //         if (selectedNode.Parent == null)
            //         {

            //             path = "C:\\TestAutomation\\Projects\\" + selectedNode.Parent.Text + "\\Test Management\\TestSuites\\Web\\" + selectedNode.Text;
            //             break;
            //         }

            //     }
            // }

            // GridView1.EditIndex = 0;
            // string spath = "C:\\TestAutomation\\Projects" + path;

            // datadisplay(spath);
            //// FileUpload1.SaveAs(path);
            // GridView1.DataBind();

        }

        protected void Update_Click(object sender, EventArgs e)
        {
         

        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //for (int i = 0; i < GridView1.Rows.Count; i++)
            //{
               
            //        GridView1.EditIndex = GridView1.SelectedRow.RowIndex+1;
                 //   GridView1.Rows[GridView1.SelectedIndex+1].RowState = DataControlRowState.Edit;
                
          //  }
            //GridView1.DataBind();

        }
       
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {


           // GridView gv = (GridView)sender;
            // Change the row state
            GridView1.Rows[e.NewEditIndex].RowState = DataControlRowState.Edit;
            //// datadisplay();
            //GridView1.EditIndex = e.NewEditIndex;
            ////FileUpload1.SaveAs(path);
            //string spath = "C:\\TestAutomation\\Projects" + path;

            //datadisplay(spath);            // GridView1.EditIndex = e.NewEditIndex;
               GridView1.DataBind();

            //foreach (TreeNode node in TreeView1.Nodes)
            //{
            //    if (node.Checked)
            //        path = path + "\\" + node.Text;

            //}
        }
        public int rowcntgrid = 0;
        protected void Next_Click(object sender, EventArgs e)
        {
            GridView1.EditIndex = 2;
            //int o = 1;
            //foreach (GridViewRow row in GridView1.Rows)
            //{
            //    if (row.RowType == DataControlRowType.DataRow)
            //    {
            //        System.Web.UI.WebControls.CheckBox CheckRow = (row.FindControl("btnedit") as System.Web.UI.WebControls.CheckBox);
            //        if (CheckRow.Checked)
            //        {
            //            GridView1.EditIndex = o;// GridView1.Rows[o].RowIndex + 1;

            //        }
            //    }
            //    o = o + 1;
            //}
           //  GridView1.DataBind();


            //for (int i = 1; i < GridView1.Rows.Count; i++)
            //{
            //    System.Web.UI.WebControls.CheckBox checkBox = GridView1.Rows[i].Cells[0].FindControl("btnedit") as System.Web.UI.WebControls.CheckBox;
            //    if (checkBox.Checked)  // NOT working either
            //    {
            //        GridView1.EditIndex = GridView1.Rows[i].RowIndex+1;
            //        GridView1.DataBind();
            //    }
            //}
            string spath = "C:\\TestAutomation\\Projects" + path;

            datadisplay(spath);
            //// Label1.Text = GridView1.EditIndex.ToString();
            ////     GridView1.EditIndex =  1;//[1].Count;
              GridView1.DataBind();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            

        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            string spath = "C:\\TestAutomation\\Projects" + path;

            datadisplay(spath);
        }

        protected void btnedit_Click(object sender, EventArgs e)
        {
          ////  datadisplay();
          //  //FileUpload1.SaveAs(path);

            //  GridView1.EditIndex =GridView1.SelectedRow.RowIndex ;
            //  // GridView1.DataBind();
            //  //  Label1.Text = labelname;
            //  // GridView1.DataBind();
            //  string spath = "C:\\TestAutomation\\Projects" + path;

            //  datadisplay(spath);

            //for (int i = 1; i < GridView1.Rows.Count; i++)
            //{
            //    System.Web.UI.WebControls.CheckBox checkBox = (System.Web.UI.WebControls.CheckBox)GridView1.Rows[i].Cells[0].Controls[i];
            //    if (checkBox.Enabled == false)  // NOT working either
            //    {
            //        GridView1.EditIndex = GridView1.Rows[i].RowIndex;
            //        break;
            //    }
            //}

        }

        protected void TreeView1_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            if (e.Node.Checked)
            {
                path =path + "\\" + e.Node.Text;
            }

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            

        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            //int i = 1;
            //foreach (GridViewRow gvrow in GridView1.Rows)
            //{
            //    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)gvrow.FindControl("chkSelect");
            //    if (chk != null & chk.Checked)
            //    {
            //        GridView1.EditIndex = i;
            //        //To Fetch the row index
            //        //str += gvDetails.SelectedIndex.ToString();

            //        //To Fetch the value of Selected Row.

            //    }
            //}
            //i = i + 1;
            GridView1.DataBind();
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            int i = 1;
            foreach (GridViewRow gvrow in GridView1.Rows)
            {
                System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)gvrow.FindControl("chkSelect");
                if (chk != null & chk.Checked)
                {
                    GridView1.EditIndex = i;
                    //To Fetch the row index
                    //str += gvDetails.SelectedIndex.ToString();

                    //To Fetch the value of Selected Row.

                }
            }
            i = i + 1;
        }
        public static int k = 1;
        public static int p = 0;
      public static  DataTable dt = new DataTable();

        protected void gridDView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //if (g != 1)
            //{
            //    g = 1;
            //    savegridtoexcel();
            //}
            // dt = (DataTable)gridDView.DataSource;
            // savegridtoexcel();
            gridDView.EditIndex = e.NewEditIndex;

            // GridViewRow row = gridDView.Rows[e.RowIndex];

            //int col = gridDView.Columns.Count;

            //for (int rows = 0; rows < gridDView.Rows.Count; rows++)
            //{

            //    for (int cols = 0; cols < col; cols++)
            //    {
            //        griddata[rows][cols] = (gridDView.Rows[rows].Cells[cols].Controls[cols].ToString());
            //    }
            //}

            //int col1 = gridDView.Columns.Count;

            //for (int rows = 0; rows < gridDView.Rows.Count; rows++)
            //{

            //    for (int cols = 0; cols < col1; cols++)
            //    {
            //        gridDView.Rows[rows].Cells[cols].Text = griddata[rows][cols];
            //    }
            //}

            // BindGridView();
            // datadisplay(spath);
            //string spath = "C:\\TestAutomation\\Projects" + path
            //   dt = (DataTable)gridDView.DataSource;
            if (path.Contains("TestRunner"))
            {
                TestRunner(path);
            }
            else if (path.Contains("Config"))
            {
                Web_Transaction_Input_Files(path);
            }
            else
            {
                datadisplay(path);
            }

            // p = p + 1;
        }

        private void ExportGridView()
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

            // Render grid view control.
            gridDView.RenderControl(htw);

            // Write the rendered content to a file.
            string renderedGridView = sw.ToString();
            System.IO.File.WriteAllText(path, renderedGridView);
        }

       public void savegridtoexcel()
        {





        }


        private void AddNewRowToGrid()
        {
            int rowIndex = 0;

            //if (ViewState["CurrentTable"] != null)
            //{
                DataTable dtCurrentTable = (DataTable)gridDView.DataSource;
                    //ViewState["CurrentTable"];
                DataRow drCurrentRow = null;
                if (dtCurrentTable.Rows.Count > 0)
                {
                    //for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                    //{
                    //    //extract the TextBox values
                    //}
                    dtCurrentTable.Rows.Add(dtCurrentTable.Rows.Count);
                   // ViewState["CurrentTable"] = dtCurrentTable;

                    gridDView.DataSource = dtCurrentTable;
                   // gridDView.DataBind();
                }
           // }
            //else
            //{
            //    Response.Write("ViewState is null");
            //}
            //if (flag == "TR")
            //{
            //    TestRunner(path);
            //}
            //else if (flag == "CF")
            //{
            //    Web_Transaction_Input_Files(path);
            //}
            //else
            //{
            //    datadisplay(path);
            //}
            //Set Previous Data on Postbacks
            //  SetPreviousData();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //  AddNewRowToGrid();
            // dataGridView1.Rows.Add();//here on each click the new row will be added.

            // DataTable dt = new DataTable();
            // dt.Rows.Add();
            ////   dt = (DataTable) gridDView.DataSource;
            // gridDView.DataSource = dt;// gridDView.DataSource
            // gridDView.DataBind();
            // gridDView.Rows[;


            //string str = "C:\\TestAutomation\\Projects" + path;
            //DataTable dt = (DataTable)ViewState["Customers"];
            //dt.Rows.Add(txtName.Text.Trim(), txtCountry.Text.Trim());
            //ViewState["Customers"] = dt;
            //this.BindGrid();
            //txtName.Text = string.Empty;
            //txtCountry.Text = string.Empty;


            //Response.Clear();
            //Response.Buffer = true;
            //Response.ClearContent();
            //Response.ClearHeaders();
            //Response.Charset = "";
            //string FileName = str;// "Vithal" + DateTime.Now + ".xls";
            //StringWriter strwritter = new StringWriter();
            //HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.ContentType = "application/vnd.ms-excel";
            //Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            //gridDView.GridLines = GridLines.Both;
            //gridDView.HeaderStyle.Font.Bold = true;
            //gridDView.RenderControl(htmltextwrtter);
            //Response.Write(strwritter.ToString());
            //Response.End();

            //Microsoft.Office.Interop.Excel.Application myexcelApplication = new Microsoft.Office.Interop.Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook = myexcelApplication.Workbooks.Open(str);//.Add();
            //Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(2);
            // myexcelWorksheet.get_Item(2);
            //  Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(2);

            // myexcelWorksheet.Rows.Clear();

            //myexcelWorksheet.Cells[1, 1] = "ExecuteFlag";
            ////  }
            ////  else if (item.Name != "")
            ////  {
            //myexcelWorksheet.Cells[1, 2] = "Action";
            //myexcelWorksheet.Cells[1, 3] = "LogicalName";
            //myexcelWorksheet.Cells[1, 4] = "ControlName";
            //myexcelWorksheet.Cells[1, 5] = "ControlType";
            //myexcelWorksheet.Cells[1, 6] = "ControlID";
            //myexcelWorksheet.Cells[1, 7] = "ImageType";
            //myexcelWorksheet.Cells[1, 8] = "Index";
            //myexcelWorksheet.Cells[1, 9] = "DynamicIndex";
            //myexcelWorksheet.Cells[1, 10] = "ColumnName";
            //myexcelWorksheet.Cells[1, 11] = "RowNo";
            //myexcelWorksheet.Cells[1, 12] = "ColumnNo";
            //  myexcelWorksheet.Cells[1, 6] = "XPATH";

            //int cnt = 12;//=HeaderNames.Count();
            //int cntc = 0;
            //int u = 0;
            //for(int i=1;i<gridDView.Columns.Count;i++)
            //{

            //    dt.Columns.Add(gridDView.Columns[i].HeaderText);
            //}

            //for (int rows = 1; rows <= gridDView.Rows.Count; rows++)
            //{

            //    for (int col = 0; col <= gridDView.Rows[rows].Cells.Count; col++)
            //    {
            ////        if ((gridDView.Rows[rows].Cells[col + 1].ToString() != null) || (gridDView.Rows[rows].Cells[col + 1].ToString() != ""))
            //  //      {

            //            dt.Rows[rows][col] = gridDView.Rows[rows].Cells[col+1].ToString();// gridDView.Rows[rows].Cells[col].Text.ToString();
            //                                                                                               // myexcelWorksheet.Cells[rows + 2, col + 1] = dt.Rows[rows][col + 1].ToString();// gridDView.Rows[rows].Cells[col].Text.ToString();
            //    //    }
            //    }
            //    //   u = u + 1;

            //}

            //dataGridView2.DataSource = dt;
            // label20.Text = s1


            //   myexcelApplication.DisplayAlerts = false;
            //   if (flag == "new")
            // {
            // myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //   myexcelApplication.ActiveWorkbook.SaveAs(textBox10.Text + "\\" + textBox7.Text + "\\Projects" + textBox7.Text + "\\TestSuites\\Web\\" + textBox7.Text+"\\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // @"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //}
            //else if (flag == "open")
            //{
            //  myexcelApplication.ActiveWorkbook.SaveAs(@"C:\TestAutomation\Projects\ALPSPhoenix\TestSuites\Web\ALPSPhoenix\" + input + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //myexcelApplication.ActiveWorkbook.SaveAs(str, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            ////}
            //// myexcelApplication.DisplayAlerts = true;
            //myexcelWorkbook.Close();
            //myexcelApplication.Quit();

            //releaseObject(myexcelWorksheet);
            //// releaseObject(myexcelWorksheet_val);
            //releaseObject(myexcelWorkbook);
            //releaseObject(myexcelApplication);
            Server.Transfer(@"C:\TestAutomation\Projects\ALPSPHONEX_NEW_LATEST\Reports\ALPS-Phoenix-Test_Report_20240214_141240.html");

//            Page.ClientScript.RegisterStartupScript(
//this.GetType(), "OpenWindow", "window.open('file:///C:/TestAutomation/Projects/ALPSPHONEX_NEW_LATEST/Reports/ALPS-Phoenix-Test_Report_20240214_141240.html','_newtab');", true);

        }
        public static string path;

        protected void gridDView_DataBound(object sender, EventArgs e)
        {
            //foreach (TreeNode node in TreeView1.Nodes)
            //{
            //    if (node.Checked)
            //        path = path + "\\" + node.Text;

            //}
        }
        public string[] griddata = null;

        //protected void gridDView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{

        //    GridViewRow row = gridDView.Rows[e.RowIndex];

        //    int col = gridDView.Columns.Count;

        //    for (int rows = 0; rows<gridDView.Rows.Count;rows++)
        //    {

        //        for (int cols =0; cols<col;cols++)
        //        {
        //            griddata[rows][cols] =(gridDView.Rows[rows].Cells[cols].Controls[cols].ToString());
        //        }
        //    }

        //    int col1 = gridDView.Columns.Count;

        //    for (int rows = 0; rows < gridDView.Rows.Count; rows++)
        //    {

        //        for (int cols = 0; cols < col1; cols++)
        //        {
        //            gridDView.Rows[rows].Cells[cols].Text = griddata[rows][cols];
        //        }
        //    }

        //    gridDView.EditIndex = -1;
        //  //  gridDView.DataBind();
        //  //  datadisplay(spath);

        //}

        protected void Unnamed_Click(object sender, EventArgs e)
        {
         //   GridViewRow row = gridDView.Rows[e.RowIndex];

            int col = gridDView.Columns.Count;

            for (int rows = 0; rows < gridDView.Rows.Count; rows++)
            {

                for (int cols = 0; cols < col; cols++)
                {
                    gridDView.Rows[rows].Cells[cols].Text = (gridDView.Rows[rows].Cells[cols].Controls[cols].ToString());
                }
            }

            gridDView.EditIndex = -1;
            gridDView.DataBind();
        }

        protected void Unnamed_Click1(object sender, EventArgs e)
        {

        }

        protected void gridDView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gridDView_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            //int col = gridDView.Columns.Count;

            //for (int rows = 0; rows < gridDView.Rows.Count; rows++)
            //{

            //    for (int cols = 0; cols < col; cols++)
            //    {
            //        gridDView.Rows[rows].Cells[cols].Text = (gridDView.Rows[rows].Cells[cols].Controls[cols].ToString());
            //    }
            //}

           // gridDView.EditIndex = -1;
            //gridDView.DataBind();
        }
       public static int g = 1;
        public static string m = null;
        protected void gridDView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
           // k = 1;
            GridViewRow row = gridDView.Rows[e.RowIndex];
            g = row.RowIndex;
            string str = null;
             // str=  str+ ((System.Web.UI.WebControls.TextBox)row.Cells[k].Controls[0]).Text+";";
            //  bindGvEdit();
            //GridViewRow row = gridDView.Rows[e.RowIndex];

            int col = row.Cells.Count;
            //int rows = 0;
            int cols = 0;
            //  for (rows = 0; rows <= gridDView.Rows.Count; rows++)
            //{

            // for (cols = 1; cols <= col; cols++)
            //{
            for (int i = 1; i < row.Cells.Count; i++)
            {
               m = ((System.Web.UI.WebControls.TextBox)row.Cells[i].Controls[0]).Text;
            }
            //row.Cells[2].Text = ((System.Web.UI.WebControls.TextBox)row.Cells[2].Controls[0]).Text;
            //row.Cells[3].Text = ((System.Web.UI.WebControls.TextBox)row.Cells[3].Controls[0]).Text;

            //row.Cells[4].Text = ((System.Web.UI.WebControls.TextBox)row.Cells[4].Controls[0]).Text;
            Microsoft.Office.Interop.Excel.Application myexcelApplication = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook myexcelWorkbook = myexcelApplication.Workbooks.Open(path);//.Add();

            Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet = null;
            if (path.Contains("TestRunner")||path.Contains("Config"))
            {
                myexcelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(1);
            }
            else
            {
                myexcelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(2);

            }
            for (int i = 1; i < row.Cells.Count; i++)
            {
                myexcelWorksheet.Cells[g+2,i] = ((System.Web.UI.WebControls.TextBox)row.Cells[i].Controls[0]).Text;
            }
            myexcelWorksheet.Cells[g+3, 1] = myexcelWorksheet.Cells[g+2, 1];

            // myexcelWorksheet.Cells[myexcelWorksheet.UsedRange.Count+1, 1] = myexcelWorksheet.Cells[myexcelWorksheet.UsedRange.Count, 1];
            myexcelApplication.DisplayAlerts = false;

            // myexcelWorksheet.get_Item(2);
          
            //Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(2);
            myexcelApplication.ActiveWorkbook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            // myexcelApplication.ActiveWorkbook.SaveAs(str, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            myexcelApplication.DisplayAlerts = true;

            myexcelWorkbook.Close();
            myexcelApplication.Quit();

            releaseObject(myexcelWorksheet);
            //  releaseObject(myexcelWorksheet_val1);
            releaseObject(myexcelWorkbook);
            releaseObject(myexcelApplication);
            //myexcelWorksheet.Cells[myexcelWorksheet.UsedRange.Count + 1, 1] = myexcelWorksheet.Cells[myexcelWorksheet.UsedRange.Count, 1];
            //myexcelApplication.DisplayAlerts = false;

            //// myexcelWorksheet.get_Item(2);

            ////Microsoft.Office.Interop.Excel.Worksheet myexcelWorksheet_val = (Microsoft.Office.Interop.Excel.Worksheet)myexcelWorkbook.Worksheets.get_Item(2);
            //myexcelApplication.ActiveWorkbook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //// myexcelApplication.ActiveWorkbook.SaveAs(str, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            //myexcelApplication.DisplayAlerts = true;
            ////      col = col - 1;
            //// }
            ////   row.Cells[]

            gridDView.EditIndex = -1;
            if (path.Contains("TestRunner"))
            {
                TestRunner(path);
            }
            else if(path.Contains("Config"))
            {
                Web_Transaction_Input_Files(path);
            }
            else {
                datadisplay(path);
            }
            

           // datadisplay(path);
            flag =null;
          //  savegridtoexcel();
          //    dt = (DataTable)gridDView.DataSource;
          //    for(int f=0; f<=row.Cells.Count;f++)
          //{
          //    row.Cells[f+1].Text = griddata[f];
          //    gridDView.EditIndex = -1;

            //}
            //}
            //  gridDView.DataBind();
            //     datadisplay(spath);

            // k = k + 1;
            //  savegridtoexcel();

            //datadisplay(spath);
            //  gridDView.EditIndex = -1;
            //   gridDView.DataBind();
            //  datadisplay(spath);
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DirectoryInfo dinfo = new DirectoryInfo("C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\TestSuites\\Web");
            FileInfo[] Files = dinfo.GetFiles("*.xls");

            foreach (FileInfo file in Files)
            {
                ListBox1.Items.Add(file.Name);
            }

            //var folder = "C:\\TestAutomation\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\TestSuites\\Web";
            //var txtFiles = Directory.GetFiles(folder, "*.xls");

            //ListBox1.Items.AddRange(txtFiles);

        }

        protected void Button1_Click1(object sender, EventArgs e)
        {
            DirectoryInfo dinfo = new DirectoryInfo("C:\\TestAutomation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\TestSuites\\Web");
            FileInfo[] Files = dinfo.GetFiles("*.xls");

            foreach (FileInfo file in Files)
            {
                ListBox1.Items.Add(file.Name);
            }

        }

        protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           //path = "C:\\Test Automation\\Projects\\" + DropDownList1.SelectedItem.Text + "\\Test Management\\TestSuites\\Web\\" + ListBox1.SelectedItem.Text;
           // datadisplay(path);
        }
    }
        }