/*!
 * Crystal Test SeleniumTestBase.cs
 * 
 *     https://crystaltest.codeplex.com
 *
 * Distributed in whole under the terms of the Apache 2.0 License
 *
 *     Copyright 2014, Pixeltrix
 *
 * Licensed under the GNU General Public License version 2 (GPLv2) (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     https://crystaltest.codeplex.com/license
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *     Date: Fri Mar 28 2014 09:33:33 -0500
 *     
 * Includes Selenium
 * http://docs.seleniumhq.org/
 * Released under the Apache 2.0 License.
 */

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Utilities;

namespace CTInfrastructure
{
    public abstract class SeleniumTestBase : AutomatedTestBase
    {
        private const int TIMES_TO_RETRY_FROM_SCRATCH = 0;

        protected IWebDriver driver;

        protected override void TestSpecific_QueueTestCase(string environment, string projectAbbreviation, int testCaseId, string testedBy, List<string> browsers)
        {
            foreach (string browser in browsers)
            {
                StartTestCase(
                    projectAbbreviation,
                    testCaseId,
                    environment,
                    browser,
                    Utilities.Constants.ProcessStatus.IN_QUEUE,
                    testedBy,
                    Utilities.Constants.TestType.AUTOMATION);
            }

        }

        public static void StartTestCase(
            string projectAbbreviation,
            int testCaseId,
            string environment,
            string browserAbbreviation,
            string status,
            string testedBy,
            string testType)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            environment = DatabaseUtilities.MakeSQLSafe(environment);
            browserAbbreviation = DatabaseUtilities.MakeSQLSafe(browserAbbreviation);
            status = DatabaseUtilities.MakeSQLSafe(status);
            status = DatabaseUtilities.MakeSQLSafe(status);
            testedBy = DatabaseUtilities.MakeSQLSafe(testedBy);
            testType = DatabaseUtilities.MakeSQLSafe(testType);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO TestResults" +
                        "(testCaseId" +
                        ",projectAbbreviation" +
                        ",environment" +
                        ",browserAbbreviation" +
                        ",status" +
                        ",testDate" +
                        ",testedBy" +
                        ",testType)" +
                        "VALUES" +
                        "(" + testCaseId.ToString() +
                        ", '" + projectAbbreviation + "' " +
                        ", '" + environment + "' " +
                        ", '" + browserAbbreviation + "' " +
                        ", '" + status + "' " +
                        ",GETDATE()" +
                        ", '" + testedBy + "' " +
                        ", '" + testType + "' )";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected override void TestSpecific_RunAutomatedTest()
        {
            bool testPassed = false;

            for (int index = 0;index < TIMES_TO_RETRY_FROM_SCRATCH;index++)
            {
                if (!testPassed)
                {
                    testPassed = RunTestOnce(true);
                }
            }

            if (!testPassed)
            {
                RunTestOnce(false);
            }
        }      

        private bool RunTestOnce(bool ignoreErrors) // returns true if it passed, false if it failed
        {
            String finalStatus = Utilities.Constants.ProcessStatus.FAIL;
            String error = "";
            String errorDetailed = "";

            string nodeHost = null;
            int port = -1;

            // This lock is used in two places, and is intended to protect the integrity of the code that makes sure all iexplore 
            // instances close on a node when a test is done running, and before another test can start using that node.
            try
            {

                string hub = ConfigurationManager.AppSettings["hubURL"];
                String base_url = String.Format("http://{0}/wd/hub/", hub);
                            
                int maxDriverInstantiationAttempts = 3;
                for (int driverInstantiationAttempts = 0;driverInstantiationAttempts < maxDriverInstantiationAttempts;driverInstantiationAttempts++)
                {
                    try
                    {
                        if (browserName.ToLower() == "chrome")
                        {
                            ChromeOptions options = new ChromeOptions();
                            options.AddArguments("--no-sandbox");
                            //options.AddArguments("--url-base=" + base_url);

                            //driver = new CustomChromeDriver("c:\\Development\\SeleniumServer", options);
                            driver = new CustomRemoteDriver(new Uri(base_url), (DesiredCapabilities)options.ToCapabilities());
                        }
                        else
                        {
                            DesiredCapabilities desiredCapabilities = new DesiredCapabilities(browserName, browserVersion, new Platform(PlatformType.Windows));

                            desiredCapabilities.SetCapability("ignoreProtectedModeSettings", true);

                            driver = new CustomRemoteDriver(new Uri(base_url), desiredCapabilities);
                        }
                        break;
                    }
                    catch (Exception e)
                    {
                        if (driverInstantiationAttempts == maxDriverInstantiationAttempts - 1)
                        {
                            throw new Exception("Unable to instantiate the remote web driver. Please check the node or hub.", e);
                        }
                    }
                }

                string sessionId = ((ICustomRemoteDriver)driver).GetSessionId().ToString();

                List<String> hubParts = hub.Split(new[] { ':' }).ToList();

                string hubHost = hubParts[0];
                int hubIP = Convert.ToInt32(hubParts[1]);

                CTTestGridMethods.GetNodeHostNameAndPort(hubHost, hubIP, sessionId, out nodeHost, out port);

                SaveNodeInfoInTestResults(
                    projectAbbreviation,
                    testCaseId,
                    environment,
                    browserAbbreviation,
                    nodeHost,
                    port);

                TestSpecific_RunSeleniumTest();

                finalStatus = Utilities.Constants.ProcessStatus.PASS;

                // Pass any mapped test cases
                PassMappedTestCases();

                return true;
            }
            catch (Exception ex)
            {
                if (!ignoreErrors)
                {
                    error = ex.Message;
                    errorDetailed = ex.Message + "\n" + ex.StackTrace;

                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;

                        error = error + "\n" + "\n" + "Inner Exception:" + ex.Message;
                        errorDetailed = errorDetailed + "\n" + "\n" + "Inner Exception:" + ex.Message + "\n" + ex.StackTrace;
                    }

                    if (error.Contains("No connection could be made because the target machine actively refused it"))
                    {
                        error = "The Selenium WebGrid is down. Please check the Selenium Hub and Nodes on the AdminSelenium page.";
                        errorDetailed = "The Selenium WebGrid is down. Please check the Selenium Hub and Nodes on the AdminSelenium page.\n\n" + errorDetailed;
                    }
                }

                return false;
            }
            finally
            {
                lock (typeof(AutomatedTestBase))
                {
                    string screenshotNetworkPath = "";

                    try
                    {
                        screenshotNetworkPath = GenerateScreenshot();
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        UpdateTestResultsOnLatestAutomatedRow(
                            projectAbbreviation,
                            testCaseId,
                            environment,
                            browserAbbreviation,
                            finalStatus,
                            error,
                            errorDetailed,
                            "",
                            screenshotNetworkPath);
                    }
                    catch (Exception)
                    {

                    }

                    // Ensure all driver windows close and that the driver quits
                    try
                    {
                        try
                        {
                            System.Collections.ObjectModel.ReadOnlyCollection<String> windowHandles = driver.WindowHandles;
                            for (int i = windowHandles.Count - 1;i >= 0;i--)
                            {
                                try
                                {
                                    driver.SwitchTo().Window(windowHandles[i]);
                                    Thread.Sleep(500);
                                    // This disables Javascript popup window that Selenium cannot interact with
                                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                    js.ExecuteScript("window.onbeforeunload=null");
                                    Thread.Sleep(500);

                                    driver.Close();
                                    Thread.Sleep(1000);
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                        catch (Exception)
                        {

                        }

                        driver.Quit();
                    }
                    catch (Exception)
                    {

                    }

                    Thread.Sleep(1000);

                    // Ensure that any and all browser windows of this tests's type were fully closed
                    if (nodeHost != null) // if no nodeHost, then it probably never even started
                    {
                        string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

                        // For a local run, don't kill firefox. For any other combination of environment and browser,
                        // kill all browser instances of that type on that server to avoid "zombie" processes that don't die
                        if (TestRunEnvironment != "LOCAL" || browserName.ToLower() != "firefox")
                        {
                            CTTestGridMethods.ResetSpecificBrowserProcessesOnNode(nodeHost, browserName.ToLower());
                        }
                    }
                }
            }
        }

        public void PassMappedTestCases()
        {
            string query = "select "
                    + " childTestCaseId "
                    + " from AutoTestCaseMap a "
                    + " join TestCases t "
                    + " on t.projectAbbreviation = a.projectAbbreviation "
                    + " and t.testcaseid = a.parentTestCaseId "
                    + " where a.projectAbbreviation = '" + projectAbbreviation + "' "
                    + " and parentTestCaseId = " + testCaseId;

            List<int> mappedTestCaseChildren = DatabaseUtilities.GetIntListFromQuery(query);

            foreach (int mappedTestCaseChild in mappedTestCaseChildren)
            {
                InsertTestResult
                (
                    projectAbbreviation,
                    mappedTestCaseChild,
                    environment,
                    browserAbbreviation,
                    Utilities.Constants.ProcessStatus.PASS,
                    "Automatic pass due to test case " + testCaseId + " passing",
                    "Automatic pass due to test case " + testCaseId + " passing",
                    "",
                    "",
                    "",
                    "Automated Child"
                );
            }
        }

        protected virtual void TestSpecific_RunSeleniumTest()
        {
            // This gets overridden by the subclass
        }

        protected override string GenerateScreenshot()
        {
            try
            {

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff") + (int)(new Random().NextDouble() * 100);

                string TestResultScreenshotPath = ConfigurationManager.AppSettings["TestResultScreenshotPath"];

                string absolutePath = TestResultScreenshotPath + timestamp + ".png";
                string networkPath = "../Admin/UploadedTestResultScreenshots/" + timestamp + ".png";

                Response screenshotResponse = ((ICustomRemoteDriver)driver).PublicExecute(DriverCommand.Screenshot, null);
                string base64 = screenshotResponse.Value.ToString();
                
                Screenshot ss = new Screenshot(base64);
                string screenshot = ss.AsBase64EncodedString;
                byte[] screenshotAsByteArray = ss.AsByteArray;

                // Save the screenshot
                ss.SaveAsFile(absolutePath, System.Drawing.Imaging.ImageFormat.Png);

                return networkPath;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static int InsertTestResult(
                string projectAbbreviation,
                int testCaseId,
                string environment,
                string browserAbbreviation,
                string status,
                string reasonForStatus,
                string reasonForStatusDetailed,
                string stepsToReproduce,
                string defectTicketNumber,
                string user,
                string testType
                )
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            environment = DatabaseUtilities.MakeSQLSafe(environment);
            browserAbbreviation = DatabaseUtilities.MakeSQLSafe(browserAbbreviation);
            status = DatabaseUtilities.MakeSQLSafe(status);
            reasonForStatus = DatabaseUtilities.MakeSQLSafe(reasonForStatus);
            reasonForStatusDetailed = DatabaseUtilities.MakeSQLSafe(reasonForStatusDetailed);
            stepsToReproduce = DatabaseUtilities.MakeSQLSafe(stepsToReproduce);
            defectTicketNumber = DatabaseUtilities.MakeSQLSafe(defectTicketNumber);
            user = DatabaseUtilities.MakeSQLSafe(user);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO [dbo].[TestResults]"
           + " ([projectAbbreviation]"
           + " ,[testCaseId]"
           + " ,[environment]"
           + " ,[browserAbbreviation]"
           + " ,[status]"
           + " ,[reasonForStatus]"
           + " ,[reasonForStatusDetailed]"
           + " ,[stepsToReproduce]"
           + " ,[defectTicketNumber]"
           + " ,[testDate]"
           + " ,[testedBy]"
           + " ,[testType])"
     + " VALUES"
           + " ( '" + projectAbbreviation + "'"
           + ", " + testCaseId
           + ", '" + environment + "'"
           + ", '" + browserAbbreviation + "'"
           + ", '" + status + "'"
           + ", '" + reasonForStatus + "'"
           + ", '" + reasonForStatusDetailed + "'"
           + ", '" + stepsToReproduce + "'"
           + ", '" + defectTicketNumber + "'"
           + ", GetDate()"
           + ", '" + user + "'"
           + ", '" + testType + "');select @@identity";

                    cmd.CommandType = System.Data.CommandType.Text;

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void SaveNodeInfoInTestResults(
            string projectAbbreviation,
            int testCaseId,
            string environment,
            string browserAbbreviation,
            string ipAddress,
            int port)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            environment = DatabaseUtilities.MakeSQLSafe(environment);
            browserAbbreviation = DatabaseUtilities.MakeSQLSafe(browserAbbreviation);

            IPAddress localhostIPv4Address = Dns.GetHostAddresses(Dns.GetHostName()).Where(address => address.AddressFamily == AddressFamily.InterNetwork).First();

            string hostnameOrIPAddess = ipAddress;

            if (localhostIPv4Address.ToString() == ipAddress)
            {
                hostnameOrIPAddess = "localhost";
            }

            int seleniumServerID;

            try
            {
                seleniumServerID = DatabaseUtilities.GetIntFromQuery("select top 1 SeleniumServerID from SeleniumServers where (servername = '" + hostnameOrIPAddess + "' or serverip = '" + hostnameOrIPAddess + "') and ServerRole = 'Node'");
            }
            catch (Exception)
            {
                throw new Exception("Unrecognized node at " + ipAddress + ", please add this node to the database before using it.");
            }

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "UPDATE TestResults " +
                        "set SeleniumServerID = " + seleniumServerID + " " +
                        " From TestResults join " +

                        // This inner query just gets the latest row so we'll update the right one.
                        "(select MAX(testrunid) testRunID from TestResults " +
                        "where projectAbbreviation = '" + projectAbbreviation + "' " +
                        "and testCaseId =" + testCaseId + " " +
                        "and environment = '" + environment + "' " +
                        "and browserAbbreviation = '" + browserAbbreviation + "' " +
                        "and testType = 'Automated') as InnerSearchResult " +

                        "on TestResults.testrunid = InnerSearchResult.testrunid";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DateTime GetEstimatedTimeCurrentlyQueuedAutomatedTestsWillFinish()
        {
            //string query = "select isnull(sum(AverageSeconds), 0) from TestCaseWithStatus where status in ('In Progress', 'In Queue')"
            string query = "select DATEADD(ss,isnull(sum(AverageSeconds), 0),GETDATE()) from TestCaseWithStatus where status in ('In Progress', 'In Queue')";

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                    cmd.CommandText = query;

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();

                        return dr.GetDateTime(0);
                    }
                }
            }
        }

        public static void UpdateStepsToReproduceOnLatestAutomatedRow(
            string projectAbbreviation,
            int testCaseId,
            string environment,
            string browserAbbreviation,
            string stepsToReproduce)
        {
            stepsToReproduce = DatabaseUtilities.MakeSQLSafe(stepsToReproduce);


            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "UPDATE TestResults " +
                        "set stepsToReproduce = '" + stepsToReproduce + "' " +
                        " From TestResults join " +

                        // This inner query just gets the latest row so we'll update the right one.
                        "(select MAX(testrunid) testRunID from TestResults " +
                        "where projectAbbreviation = '" + projectAbbreviation + "' " +
                        "and testCaseId =" + testCaseId + " " +
                        "and environment = '" + environment + "' " +
                        "and browserAbbreviation = '" + browserAbbreviation + "' " +
                        "and testType = 'Automated') as InnerSearchResult " +

                        "on TestResults.testrunid = InnerSearchResult.testrunid " +
                        "where status != 'Retest'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateTestResultsWithTestRunID(
            string projectAbbreviation,
            int testCaseId,
            string environment,
            string browserAbbreviation,
            string status,
            string reasonForStatus,
            string reasonForStatusDetailed,
            string stepsToReproduce,
            string defectTicketNumber,
            int testRunId)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            environment = DatabaseUtilities.MakeSQLSafe(environment);
            browserAbbreviation = DatabaseUtilities.MakeSQLSafe(browserAbbreviation);
            status = DatabaseUtilities.MakeSQLSafe(status);
            reasonForStatus = DatabaseUtilities.MakeSQLSafe(reasonForStatus);
            reasonForStatusDetailed = DatabaseUtilities.MakeSQLSafe(reasonForStatusDetailed);
            stepsToReproduce = DatabaseUtilities.MakeSQLSafe(stepsToReproduce);
            defectTicketNumber = DatabaseUtilities.MakeSQLSafe(defectTicketNumber);



            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "insert into TestResultsHistory " +
                        " select 'UPDATE', * " +
                        " From TestResults " +
                        "where TestResults.testrunid = " + testRunId.ToString();

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "UPDATE TestResults " +
                        "set status = '" + status + "' " +
                        ", reasonForStatus =  '" + reasonForStatus + "' " +
                        ", reasonForStatusDetailed =  '" + reasonForStatusDetailed + "' " +
                        ", stepsToReproduce =  '" + stepsToReproduce + "' " +
                        ", defectTicketNumber =  '" + defectTicketNumber + "' " +
                        " From TestResults " +
                        "where TestResults.testrunid = " + testRunId.ToString();

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static bool AddSingleTestResultScreenshotIfNeeded(int testCaseID, string projectAbbreviation, int testRunID, string imageURL, string description)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            imageURL = DatabaseUtilities.MakeSQLSafe(imageURL);
            description = DatabaseUtilities.MakeSQLSafe(description);

            string mySelectQuery = "SELECT * from TestCaseScreenshots where "
                + " projectAbbreviation = '" + projectAbbreviation + "'"
                + " and testCaseId = " + testCaseID
                + " and imageURL = '" + imageURL + "'";

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = mySelectQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                        {
                            // Need to add this row
                            AddSingleTestResultScreenshot(testCaseID, projectAbbreviation, testRunID, imageURL, description);
                            return true;
                        }
                        else
                        {
                            // Otherwise it's already in the database
                            return false;
                        }
                    }
                }
            }
        }

        public static void AddTestResultScreenshots(int testCaseID, string projectAbbreviation, int testRunID, string screenshots, string screenshotDescriptions)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            screenshots = DatabaseUtilities.MakeSQLSafe(screenshots);
            screenshotDescriptions = DatabaseUtilities.MakeSQLSafe(screenshotDescriptions);

            if (String.IsNullOrEmpty(screenshots))
            {
                return;
            }

            List<string> splitScreenshots = screenshots.Split(new[] { '|' }).ToList();
            List<string> splitScreenshotDescriptions = screenshotDescriptions.Split(new[] { '|' }).ToList();

            for (int i = 0; i < splitScreenshots.Count; i++)
            {
                string imageURL = splitScreenshots[i];
                string imageDescription = String.IsNullOrEmpty(screenshotDescriptions) ? "" : splitScreenshotDescriptions[i];
                AddSingleTestResultScreenshotIfNeeded(testCaseID, projectAbbreviation, testRunID, imageURL, imageDescription);
            }

        }

    }
}
