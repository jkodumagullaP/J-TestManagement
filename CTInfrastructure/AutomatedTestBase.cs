/*!
 * Crystal Test AutomatedTestBase.cs
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Threading;
using Utilities;

namespace CTInfrastructure
{
    public abstract class AutomatedTestBase
    {
        private const int MaxNonLocalSimultaneousTests = 2;

        protected static int MaxSimultaneousTests
        {
            get 
            {
                string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

                if (TestRunEnvironment == "LOCAL")
                {
                    return 1;
                }
                else if (TestRunEnvironment == "STAGE")
                {
                    return 1;
                }
                else
                {
                    return MaxNonLocalSimultaneousTests;
                }
            }
        }

        protected string browserAbbreviation;

        protected string browserName;

        protected string browserVersion;

        protected string autoMetaDataTable;

        protected int? autoMetaDataRow;

        protected string environment;

        protected string projectAbbreviation;

        protected int testCaseId;

        public static void QueueTestCase(string environment, string projectAbbreviation, int testCaseId, string testedBy, List<string> browsers)
        {
            // Only queue if it's not already queued or running
            string currentTestStatus = CTMethods.GetTestCaseStatus(projectAbbreviation, testCaseId, environment);

            if (currentTestStatus != Utilities.Constants.ProcessStatus.IN_PROGRESS
                && currentTestStatus != Utilities.Constants.ProcessStatus.IN_QUEUE)
            {
                string description;
                Type autoTestClass;
                string autoMetaDataTable;
                int? autoMetaDataRow;
                string autoTestClassType;
                string automated;
                string reasonForNotAutomated;
                GetTestCaseAutomationParameters(projectAbbreviation, testCaseId, out description, out autoTestClassType, out autoTestClass, out autoMetaDataTable, out autoMetaDataRow, out automated, out reasonForNotAutomated);

                ConstructorInfo ctor = autoTestClass.GetConstructor(System.Type.EmptyTypes);
                if (ctor != null)
                {
                    AutomatedTestBase instance = (AutomatedTestBase)ctor.Invoke(null);
                    instance.TestSpecific_QueueTestCase(environment, projectAbbreviation, testCaseId, testedBy, browsers);
                }
            }
        }


        // "Synchronized" methods can't be accessed from 2 threads at the same time, to ensure we don't overshoot our max running test count.
        public static string CheckForRunnableQueuedTests()
        {
            // This lock is used in two places, and is intended to protect the integrity of the code that makes sure all iexplore 
            // instances close on a node when a test is done running, and before another test can start using that node.
            lock (typeof(AutomatedTestBase))
            {
                string returnValue = "";

                string projectAbbreviation;
                int testCaseId;
                string environment;

                GetNextQueuedTest(
                     out projectAbbreviation,
                     out testCaseId,
                     out environment);

                if (GetInProgressTestCount() < MaxSimultaneousTests
                    && projectAbbreviation != null)
                {
                    RunQueuedTests(projectAbbreviation, testCaseId, environment);

                }

                returnValue += "<" + GetInProgressTestCount() + " in progress out of " + MaxSimultaneousTests + " max>";

                return returnValue;
            }
        }

        public static int GetInProgressTestCount()
        {
            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    //cmd.CommandText = "select count(*) as [count] from TestCaseWithStatus where status = 'In Progress'";
                    cmd.CommandText = "select count(*) from (select distinct [testCaseId], environment, projectAbbreviation from testresults where status = 'In Progress') innerQuery";
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();

                        int result = dr.GetInt32(0);

                        return result;
                    }
                }
            }
        }

        private static void RunQueuedTests(string projectAbbreviation,
                    int testCaseId,
                    string environment)
        {
            string description;
            Type autoTestClass;
            string autoMetaDataTable;
            int? autoMetaDataRow;
            string autoTestClassType;
            string automated;
            string reasonForNotAutomated;
            GetTestCaseAutomationParameters(projectAbbreviation, testCaseId, out description, out autoTestClassType, out autoTestClass, out autoMetaDataTable, out autoMetaDataRow, out automated, out reasonForNotAutomated);

            List<string> browserAbbreviations = GetTestCaseBrowsers(projectAbbreviation, testCaseId, environment);

            foreach (string browserAbbreviation in browserAbbreviations)
            {
                MethodInfo RunQueuedTestMethod = typeof(AutomatedTestBase).GetMethod("RunQueuedTest", BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo RunQueuedTestGenericMethod = RunQueuedTestMethod.MakeGenericMethod(autoTestClass);

                SeleniumTestBase.UpdateTestResultsOnLatestAutomatedRow(
                    projectAbbreviation,
                    testCaseId,
                    environment,
                    browserAbbreviation,
                    Utilities.Constants.ProcessStatus.IN_PROGRESS,
                    "",
                    "",
                    "",
                    "");

                RunQueuedTestGenericMethod.Invoke(
                    null,
                    new object[] {projectAbbreviation,
                    testCaseId,
                    environment,
                    browserAbbreviation,
                    autoTestClass,
                    autoMetaDataTable,
                    autoMetaDataRow});
            }
        }  

        protected static void RunQueuedTest<TestType>(string projectAbbreviation,
                    int testCaseId,
                    string environment,
                    string browserAbbreviation,
                    Type autoTestClass,
                    string autoMetaDataTable,
                    int? autoMetaDataRow
                    ) where TestType : AutomatedTestBase, new()
        {
            TestType test = null;
            try
            {
                test = new TestType();
                test.browserName = Utilities.Constants.GetSeleniumCompatibleBrowserName(browserAbbreviation);
                test.browserVersion = Utilities.Constants.GetSeleniumCompatibleBrowserVersion(browserAbbreviation);
                test.browserAbbreviation = browserAbbreviation;
                test.autoMetaDataTable = autoMetaDataTable;
                test.autoMetaDataRow = autoMetaDataRow;
                test.environment = environment;
                test.projectAbbreviation = projectAbbreviation;
                test.testCaseId = testCaseId;

                Thread testThread = new Thread(new ThreadStart(test.RunAutomatedTest));
                testThread.Start();
            }
            catch (Exception ex)
            {
                // This should never happen.... but just in case.
                string error = ex.Message;
                string errorDetailed = ex.Message + "\n" + ex.StackTrace;

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;

                    error = error + "\n" + "\n" + "Inner Exception:" + ex.Message;
                    errorDetailed = errorDetailed + "\n" + "\n" + "Inner Exception:" + ex.Message + "\n" + ex.StackTrace;
                }

                string screenshotNetworkPath = "";
                if (test != null)
                {
                    screenshotNetworkPath = test.GenerateScreenshot();
                }

                SeleniumTestBase.UpdateTestResultsOnLatestAutomatedRow(
                projectAbbreviation,
                testCaseId,
                environment,
                browserAbbreviation,
                Utilities.Constants.ProcessStatus.FAIL,
                error,
                errorDetailed,
                "",
                screenshotNetworkPath);


            }
        }

        public static void UpdateTestResultsOnLatestAutomatedRow(
        string projectAbbreviation,
        int testCaseId,
        string environment,
        string browserAbbreviation,
        string status,
        string reasonForStatus,
        string reasonForStatusDetailed,
        string defectTicketNumber,
        string screenshotNetworkPath)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine("\n" + now.ToShortDateString() + " " + now.ToLongTimeString() + " Updated status: " + status + " for " + projectAbbreviation + " - " + testCaseId + " - " + environment + " - " + browserAbbreviation);

            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            environment = DatabaseUtilities.MakeSQLSafe(environment);
            browserAbbreviation = DatabaseUtilities.MakeSQLSafe(browserAbbreviation);
            status = DatabaseUtilities.MakeSQLSafe(status);
            reasonForStatus = DatabaseUtilities.MakeSQLSafe(reasonForStatus);
            reasonForStatusDetailed = DatabaseUtilities.MakeSQLSafe(reasonForStatusDetailed);
            defectTicketNumber = DatabaseUtilities.MakeSQLSafe(defectTicketNumber);

            // Save a screenshot, if any was provided
            if (!String.IsNullOrEmpty(screenshotNetworkPath))
            {
                using (System.Data.SqlClient.SqlConnection Conn = new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
                {
                    Conn.Open();

                    using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                    {
                        cmd.CommandText =
                            "select MAX(testrunid) testRunID from TestResults " +
                            "where projectAbbreviation = '" + projectAbbreviation + "' " +
                            "and testCaseId =" + testCaseId + " " +
                            "and environment = '" + environment + "' " +
                            "and browserAbbreviation = '" + browserAbbreviation + "' " +
                            "and testType = 'Automated'";

                        cmd.CommandType = System.Data.CommandType.Text;

                        int testRunID = Convert.ToInt32(cmd.ExecuteScalar());

                        AddSingleTestResultScreenshot(testCaseId, projectAbbreviation, testRunID, screenshotNetworkPath, "status");
                    }
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
                        ", stepsToReproduce = isnull(stepsToReproduce, '') " +
                        ", defectTicketNumber =  '" + defectTicketNumber + "' " +
                        ", testDate =  GetDate() " +
                        (status == "In Progress" ? ", startDate =  GetDate() " : "") +
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

            // Send out an email if necessary
            if (status == Utilities.Constants.ProcessStatus.FAIL || status == Utilities.Constants.ProcessStatus.PASS)
            {
                // A using statement will make the contained object exist only for the duration of
                // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
                using (System.Data.SqlClient.SqlConnection Conn =
                    new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
                {
                    Conn.Open();

                    using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                    {
                        cmd.CommandText =
                            "select testedBy " +
                            " , right('0' + rtrim(convert(char(2), elapsedSeconds / (60 * 60))), 2) + ':' + right('0' + rtrim(convert(char(2), (elapsedSeconds / 60) % 60)), 2) + ':' + right('0' + rtrim(convert(char(2), elapsedSeconds % 60)),2) as [elapsedSeconds] " +

                            "from TestResults join " +
                            // This inner query just gets the latest row so we'll update the right one.
                            "(select MAX(testrunid) testRunID from TestResults " +
                            "where projectAbbreviation = '" + projectAbbreviation + "' " +
                            "and testCaseId =" + testCaseId + " " +
                            "and environment = '" + environment + "' " +
                            "and browserAbbreviation = '" + browserAbbreviation + "' " +
                            "and testType = 'Automated') as InnerSearchResult " +

                            "on TestResults.testrunid = InnerSearchResult.testrunid";

                        cmd.CommandType = System.Data.CommandType.Text;

                        using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();

                            string testedBy = dr.GetString(0);
                            string elapsedSeconds = dr.GetString(1);

                            EmailResults(testedBy, projectAbbreviation, testCaseId, environment, browserAbbreviation, status, reasonForStatus, reasonForStatusDetailed, elapsedSeconds);
                        }
                    }
                }
            }
        }

        public static void AddSingleTestResultScreenshot(int testCaseID, string projectAbbreviation, int testRunID, string imageURL, string description)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            imageURL = DatabaseUtilities.MakeSQLSafe(imageURL);
            description = DatabaseUtilities.MakeSQLSafe(description);

            string mySelectQuery = "INSERT INTO [TestResultScreenshots]"
            + " ([projectAbbreviation] "
            + " ,[testCaseId] "
            + " ,[testRunId] "
            + " ,[imageURL] "
            + " ,description) "
            + " VALUES "
            + " ('" + projectAbbreviation
            + "', " + testCaseID
            + ", " + testRunID
            + ", '" + imageURL + "'"
            + ", '" + description + "')";

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

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static string GetUserEmail(string userName)
        {
            string query = "SELECT Email FROM aspnet_Users JOIN aspnet_Membership ON aspnet_Users.UserId = aspnet_Membership.UserId WHERE UserName='" + userName + "'";

            string userEmail = DatabaseUtilities.GetTextFromQuery(query);

            if (userEmail != null && userEmail != "")
            {
                return userEmail;
            }
            else
            {
                return null;
            }
        }

        public static void EmailResults(
            string userName,
            string projectAbbreviation,
            int testCaseID,
            string environment,
            string browserAbbreviation,
            string status,
            string reasonForStatus,
            string reasonForStatusDetailed,
            string elapsedSeconds)
        {
            string from = Constants.FROMEMAILADDRESS.ToString();
            string to = GetUserEmail(userName);
            string subject = projectAbbreviation + "-" + testCaseID + " : " + status + " : " + environment + " : " + browserAbbreviation;
            string body =

                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"> \n\n"
                + "<html xmlns=\"http://www.w3.org/1999/xhtml\">\n\n"
                + "<head id=\"Head1\" runat=\"server\">\n\n"
                + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n\n"
                + "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>\n\n"
                + "<title>Test Case: " + projectAbbreviation + "-" + testCaseID + " Results: " + status + " (in " + environment + ", " + browserAbbreviation + ")</title>\n\n"
                + "<style type=\"text/css\" media=\"all\">\n\n"
                + "<!--"
                + " .fieldName { font-weight:bold; font-size: 10pt; font-family: Verdana, Geneva, Tahoma, sans-serif; align: center; color: #FFFFFF; background:#5D7FC0; border:1px solid #404040;}"
                + " .fieldValue { font-size: 10pt; font-family: Verdana, Geneva, Tahoma, sans-serif; align: center; color: #404040; background:#D5E1F0; border:1px solid #404040;}"
                + " .title {font-size:36px; text-align:center;}"
                + "wrapperTable {margin:0px; padding:6px; width:500px; box-shadow: 10px 10px 5px #888888;}"
                + "dataTable {margin:0px; padding:6px; width:100%; box-shadow: 10px 10px 5px #888888;}"
                + "-->"
                + "</style>\n\n"
                + "</head>"
                + "<body>"
                + "<table class=\"wrapperTable\">"
                + "<tr>"
                + "<td>"
                + "<img src=\"http://content.screencast.com/users/jacquelinewalton/folders/Snagit/media/0be14987-452a-4f5c-b6fb-64afc26556bc/04.01.2014-14.09.png\" alt=\"Crystal Test\" width=\"150\" height=\"175\" />"
                + "</td>"
                + "<td class=\"title\">"
                + "Automated Results <br/>" + projectAbbreviation + "-" + testCaseID
                + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td colspan=\"2\">"
                + "<table class=\"dataTable\">"
                + "<tr>"
                + "<td class=\"fieldName\"> Project </td>"
                + "<td class=\"fieldValue\">" + projectAbbreviation + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td class=\"fieldName\"> Test Case ID </td>"
                + "<td class=\"fieldValue\">" + testCaseID + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td class=\"fieldName\"> Environment </td>"
                + "<td class=\"fieldValue\">" + environment + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td class=\"fieldName\"> Browser </td>"
                + "<td class=\"fieldValue\">" + browserAbbreviation + "</td>"
                + "</tr>"
                + "</tr>"
                + "<tr>"
                + "<td class=\"fieldName\"> Final Status </td>"
                + "<td class=\"fieldValue\">" + status + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td class=\"fieldName\"> Reason For Status </td>"
                + "<td class=\"fieldValue\">" + reasonForStatus + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td class=\"fieldName\"> Detailed Reason For Status </td>"
                + "<td class=\"fieldValue\">" + reasonForStatusDetailed + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td class=\"fieldName\"> Test Time (HH:MM:SS) </td>"
                + "<td class=\"fieldValue\">" + elapsedSeconds + "</td>"
                + "</tr>"
                + "</table>"
                + "</td>"
                + "</tr>"
                + "</table>"
                + "</body>"
                + "</html>";



            int maxEmailAttempts = 3;
            for (int emailAttempts = 0; emailAttempts < maxEmailAttempts; emailAttempts++)
            {
                try
                {
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(from, to, subject, body);
                    message.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();

                    smtp.Send(message);
                    break;
                }
                catch (Exception e)
                {
                    if (emailAttempts == maxEmailAttempts - 1)
                    {
                        throw new Exception("Error sending email to report test completion.\nMessage intended to be sent to " + to + ":\n\n" + body, e);
                    }
                }
            }
        }

        public static void QuickEmail(string fromEmailAddress, string toEmailAddress, string subject, string body)
        {
            int maxEmailAttempts = 3;
            for (int emailAttempts = 0; emailAttempts < maxEmailAttempts; emailAttempts++)
            {
                try
                {
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(fromEmailAddress, toEmailAddress, subject, body);

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();

                    smtp.Send(message);
                    break;
                }
                catch (Exception e)
                {
                    if (emailAttempts == maxEmailAttempts - 1)
                    {
                        throw new Exception("Error sending email to report test completion.\nMessage intended to be sent to " + toEmailAddress + ":\n\n" + body, e);
                    }
                }
            }
        }

        public static List<string> GetTestCaseBrowsers(
            string projectAbbreviation,
            int testCaseId,
            string environment)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            environment = DatabaseUtilities.MakeSQLSafe(environment);

            List<string> returnList = new List<string>();

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                    cmd.CommandText =
                        "Select browserAbbreviation from TestResults " +

                        "join " +

                        // This inner query just gets the latest row.
                        " (select MAX(testrunid) testRunID from TestResults " +
                        " where projectAbbreviation = '" + projectAbbreviation + "' " +
                        " and testCaseId =" + testCaseId + " " +
                        " and environment = '" + environment + "' " +
                        " and testtype = 'Automated' " +
                        " group by browserAbbreviation) as InnerSearchResult " +

                        " on TestResults.testrunid = InnerSearchResult.testrunid" +
                        " where status = '" + Constants.ProcessStatus.IN_QUEUE + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            returnList.Add(dr["browserAbbreviation"].ToString());
                        }
                    }
                }
            }

            return returnList;
        }

        public static void GetTestCaseAutomationParameters(
                    string projectAbbreviation,
                    int testCaseId,
                    out string description,
                    out string autoTestClassType,
                    out Type autoTestClass,
                    out string autoMetaDataTable,
                    out int? autoMetaDataRow,
                    out string automated,
                    out string reasonForNotAutomated
            )
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                    cmd.CommandText =
                        "Select testCaseDescription, autoTestClass, autoMetaDataTable, autoMetaDataRow, automated, reasonForNotAutomated " +
                        "from TestCases " +
                        "where projectAbbreviation = '" + projectAbbreviation + "' " +
                        "and testCaseId =" + testCaseId + " ";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        bool wasTestCaseFound = dr.Read();

                        if (wasTestCaseFound)
                        {
                            description = dr["testCaseDescription"].ToString();
                            string autoTestClassString = dr["autoTestClass"].ToString();
                            autoTestClassType = autoTestClassString;
                            autoTestClass = Type.GetType(autoTestClassString);
                            autoMetaDataTable = dr["autoMetaDataTable"].ToString();
                            automated = dr["automated"].ToString();
                            reasonForNotAutomated = dr["reasonForNotAutomated"].ToString();

                            if (autoTestClass == null && !String.IsNullOrEmpty(autoTestClassString))
                            {
                                throw new Exception("Unable to parse the type: \"" + autoTestClassString + "\"");
                            }

                            int possibleautoMetaDataRow = -1;
                            bool parsedSuccessfully = Int32.TryParse((dr["autoMetaDataRow"] ?? "").ToString(), out possibleautoMetaDataRow);

                            if (parsedSuccessfully)
                            {
                                autoMetaDataRow = possibleautoMetaDataRow;
                            }
                            else
                            {
                                autoMetaDataRow = null;
                            }
                        }
                        else
                        {

                            throw new Exception("No results returned from query: \"" + cmd.CommandText + "\"");
                        }
                    }
                }
            }
        }

        public static void GetNextQueuedTest(
                    out string projectAbbreviation,
                    out int testCaseId,
                    out string environment)
        {
            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                    cmd.CommandText =
                        "select top 1 projectAbbreviation, testCaseId, environment from testresults where status = 'In Queue'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        bool wasTestCaseFound = dr.Read();

                        if (wasTestCaseFound)
                        {
                            projectAbbreviation = dr["projectAbbreviation"].ToString();
                            testCaseId = Convert.ToInt32(dr["testCaseId"].ToString());
                            environment = dr["environment"].ToString();
                        }
                        else
                        {
                            projectAbbreviation = null;
                            testCaseId = -1;
                            environment = null;
                        }
                    }
                }
            }
        }

        protected virtual string GenerateScreenshot()
        {
            // This logic is test-specific, and should be overridden.
            return "";
        }

        protected void RunAutomatedTest()
        {
            TestSpecific_RunAutomatedTest();

            CheckForRunnableQueuedTests();
            
        }

        protected abstract void TestSpecific_QueueTestCase(string environment, string projectAbbreviation, int testCaseId, string testedBy, List<string> browsers);

        protected abstract void TestSpecific_RunAutomatedTest();
    }
}






