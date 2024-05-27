/*!
 * Crystal Test CTMethods.cs
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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Web.UI.WebControls;
using Utilities;

namespace CTInfrastructure
{
    public static class CTMethods
    {


        // ---- GetColumnIndexByDBName ----------------------------------
        //
        // pass in a GridView
        // returns an array of browser columns
        static public int[] gvBrowserStatusColumns(GridView aGridView)
        {
            return new int[] 
                    { 
                        AspUtilities.GetColumnIndexByDBName(aGridView, "Browser1Status"), 
                        AspUtilities.GetColumnIndexByDBName(aGridView, "Browser2Status"), 
                        AspUtilities.GetColumnIndexByDBName(aGridView, "Browser3Status"), 
                        AspUtilities.GetColumnIndexByDBName(aGridView, "Browser4Status"), 
                        AspUtilities.GetColumnIndexByDBName(aGridView, "Browser5Status"), 
                        AspUtilities.GetColumnIndexByDBName(aGridView, "Browser6Status"), 
                        AspUtilities.GetColumnIndexByDBName(aGridView, "Browser7Status"),
                        AspUtilities.GetColumnIndexByDBName(aGridView, "Browser8Status"),
                    };
        }

        static public int[] gvNonBrowserStatusColumns(GridView aGridView)
        {
            int[] gridviewStatusColumns = gvBrowserStatusColumns(aGridView);
            List<int> gvNonBrowserColumns = new List<int>();

            //Loop through the columns of the gridview
            for (int Index = 0; Index < aGridView.Columns.Count; Index++)
            {
                //See if the column exists in gridviewStatusColumns
                bool exists = gridviewStatusColumns.Contains(Index);
                // If not, add to gvNonBrowserColumns list.
                if (!exists)
                {
                    gvNonBrowserColumns.Add(Index);
                }
            }
            int[] columnArray = gvNonBrowserColumns.ToArray();
            return columnArray;
        }


        // ---- getVisibleBrowserColumns ----------------------------------
        //
        // pass in a GridView
        // returns an array of gridview columns that should be visible 

        static public int[] getVisibleBrowserColumns(GridView aGridView, String projectAbbreviation)
        {
            System.Web.UI.WebControls.BoundField DataColumn;

            //Get the Browser Visibility datatable
            string query = "SELECT ProjectBrowserInfo.browserAbbreviation, "
                + "ProjectBrowserInfo.showBrowserColumn "
                + "FROM ProjectBrowserInfo "
                + "JOIN Browsers ON Browsers.browserAbbreviation = ProjectBrowserInfo.browserAbbreviation "
                + "WHERE ProjectBrowserInfo.projectAbbreviation = '" + projectAbbreviation + "' "
                + "AND ProjectBrowserInfo.showBrowserColumn = 'True'";

            DataTable browserVisibility = DatabaseUtilities.GetDataTable(query);

            //Determine which columns in the gridview are browser status columns
            int[] gridviewStatusColumns = gvBrowserStatusColumns(aGridView);
            List<int> gridviewVisibleBrowserColumns = new List<int>();


            foreach (DataRow row in browserVisibility.Rows)
            {
                //Store the data in variables
                String browserAbbreviation = row["browserAbbreviation"].ToString();
                bool showBrowserColumn = Convert.ToBoolean(row["showBrowserColumn"]);

                //Loop through the browser columns to determine the matching column ID
                foreach (int gridviewStatusColumn in gridviewStatusColumns)
                {
                    try
                    {
                        DataColumn = aGridView.Columns[gridviewStatusColumn] as System.Web.UI.WebControls.BoundField;
                        //If the column exists for the browserAbbreviation
                        if (DataColumn.FooterText == browserAbbreviation)
                        {
                            if (showBrowserColumn == true)
                            {
                                gridviewVisibleBrowserColumns.Add(gridviewStatusColumn);
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                    catch
                    {
                        throw new Exception("The Browser list in the database does not match the browser columns in gridview " + aGridView.ID + "on " + aGridView.Page + " page. All columns in the Browsers table and ProjectBrowserInfo need to have a corresponding browser column in the gridview, whether it will be visible or not. Please check the Browsers to make sure a record exists for all browsers. Then check the ProjectBrowserInfo table to make sure a record exists for all browsers for your specific project. Then check the TestCases.aspx page to make sure a column has been created in the gridview for all browsers.");
                    }

                }
            }
            int[] columnArray = gridviewVisibleBrowserColumns.ToArray();
            return columnArray;
        }
        
        //returns a dictionary with active, and needs updating statuses
        public static Dictionary<string, string> GetTestCaseStatuses(string projectAbbreviation, int testCaseId)
        {
            string query = "Select [active], [testCaseOutdated], [testScriptOutdated] FROM TestCases WHERE projectAbbreviation = '" + projectAbbreviation + "' AND testCaseId = " + testCaseId;

            Dictionary<string, string> dictionary = DatabaseUtilities.GetTableDictionary(projectAbbreviation, testCaseId, query);
            return dictionary;
        }
        
        public static bool IsPrivateGroup(string projectAbbreviation, string groupAbbreviation)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            groupAbbreviation = DatabaseUtilities.MakeSQLSafe(groupAbbreviation);

            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "select * from GroupTests "
                    + " where projectAbbreviation = '" + projectAbbreviation + "'"
                    + " and groupTestAbbreviation = '" + groupAbbreviation + "' and personalGroupOwner is not null";
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        return dr.HasRows;
                    }
                }
            }
        }

        public static string GetDefaultEnvironment(string projectAbbreviation)
        {
            string environment;
            string query = "SELECT environment FROM ProjectEnvironmentInfo WHERE projectAbbreviation = '" + projectAbbreviation + "' AND defaultEnvironment = 'True'";
            DataTable defaultEnvironmentTable = DatabaseUtilities.GetDataTable(query);

            if (defaultEnvironmentTable.Rows.Count != 0)
            {
                //Grab the first row and return the environment
                environment = defaultEnvironmentTable.Rows[0]["environment"].ToString();
                return environment;
            }
            else
            {
                return null;
            }
        }


        public static string GetDefaultProject(IIdentity identity)
        {

            string mySelectQuery = "SELECT defaultProjectAbbreviation FROM aspnet_Users JOIN UserProfiles ON aspnet_Users.UserId = UserProfiles.UserId WHERE UserName='" + AspUtilities.RemovePrefixFromUserName(identity.Name) + "'";

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
                        if (dr.HasRows)
                        {
                            dr.Read();

                            if (!dr.IsDBNull(0))
                            {
                                return dr.GetString(0);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }


        public static string GetEnvironmentsCommandText(string project)
        {
            return "Select e.environment as environment from ProjectEnvironmentXref x " +
                "join Environments e on x.environment = e.environment " +
                "where x.projectAbbreviation = '" + project + "' " +
                "order by e.sortorder";
        }

        public static string GetDefaultProjectName(IIdentity identity)
        {

            string mySelectQuery = "SELECT projectName FROM aspnet_Users JOIN UserProfiles ON aspnet_Users.UserId = UserProfiles.UserId join projects on defaultProjectAbbreviation = ProjectAbbreviation WHERE UserName = '" + AspUtilities.RemovePrefixFromUserName(identity.Name) + "'";

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
                        if (dr.HasRows)
                        {
                            dr.Read();

                            if (!dr.IsDBNull(0))
                            {
                                return dr.GetString(0);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }


        public static string GetTestCaseStatus(
                    string projectAbbreviation,
                    int testCaseId,
                    string environment)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            environment = DatabaseUtilities.MakeSQLSafe(environment);

            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                    cmd.CommandText =
                        "Select status from TestCaseWithStatus " +
                        "where projectAbbreviation = '" + projectAbbreviation + "' " +
                        "and testCaseId =" + testCaseId + " " +
                        "and environment = '" + environment + "' ";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        bool wasTestCaseFound = dr.Read();

                        if (wasTestCaseFound)
                        {
                            return dr["status"].ToString();
                        }
                        else
                        {
                            return "ERROR";
                        }
                    }
                }
            }
        }



        public static int GetFirstLastPreviousNextIds(string projectAbbreviation, int currentTestCaseId, string returnType)
        {
            DataTable returnValue = new DataTable();
            int firstTestCaseId = 0;
            int previousTestCaseId = 0;
            int nextTestCaseId = 0;
            int lastTestCaseId = 0;

            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);

            string spName;
            spName = "sp_ReturnFirstLastNextPreviousTCIDs";

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "exec "
                        + spName
                        + " '" + projectAbbreviation // @projectAbbreviation
                        + "'," + currentTestCaseId; // @currentTestCaseId

                    cmd.CommandType = System.Data.CommandType.Text;

                    // execute the command
                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        bool wasRowFound = dr.Read();

                        if (wasRowFound)
                        {
                            firstTestCaseId = Convert.ToInt32(dr["firstTestCaseId"]);
                            previousTestCaseId = Convert.ToInt32(dr["previousTestCaseId"]);
                            nextTestCaseId = Convert.ToInt32(dr["nextTestCaseId"]);
                            lastTestCaseId = Convert.ToInt32(dr["lastTestCaseId"]);

                            switch (returnType)
                            {
                                case "First":
                                    return firstTestCaseId;

                                case "Back":
                                    return previousTestCaseId;

                                case "Next":
                                    return nextTestCaseId;

                                case "Last":
                                    return lastTestCaseId;

                                default:
                                    return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
        }


        public static void UpdateSingleTestCaseScreenshot(int testCaseID, string projectAbbreviation, string imageURL, string description)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            imageURL = DatabaseUtilities.MakeSQLSafe(imageURL);
            description = DatabaseUtilities.MakeSQLSafe(description);

            string mySelectQuery = "update [TestCaseScreenshots]"
            + " set description = '" + description + "'  where "
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

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int GetNextTestCaseID(string projectAbbreviation)
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
                        "select max(testCaseID) + 1 "
                        + " from "
                        + " ( "
                        + " select 0 as testCaseID "
                        + " union "
                        + " select testCaseID from testcases where projectabbreviation = '" + projectAbbreviation + "' "
                        + " union "
                        + " select testCaseID from testcaseshistory where projectabbreviation = '" + projectAbbreviation + "' "
                        + " ) a ";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();
                        return dr.GetInt32(0);
                    }
                }
            }
        }


        public static int InsertTestCase(
            string projectAbbreviation,
            string testCaseDescription,
            bool active,
            bool testCaseOutdated,
            bool testScriptOutdated,
            string testCaseSteps,
            string expectedResults,
            string testCaseNotes,
            string updatedBy,
            string autoTestClass,
            string autoMetaDataTable,
            int? autoMetaDataRow,
            string testCategory,
            string automated,
            string reasonForNotAutomated
            )
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            testCaseDescription = DatabaseUtilities.MakeSQLSafe(testCaseDescription);
            testCaseSteps = DatabaseUtilities.MakeSQLSafe(testCaseSteps);
            expectedResults = DatabaseUtilities.MakeSQLSafe(expectedResults);
            testCaseNotes = DatabaseUtilities.MakeSQLSafe(testCaseNotes);
            updatedBy = DatabaseUtilities.MakeSQLSafe(updatedBy);
            autoTestClass = DatabaseUtilities.MakeSQLSafe(autoTestClass);
            autoMetaDataTable = DatabaseUtilities.MakeSQLSafe(autoMetaDataTable);
            testCategory = DatabaseUtilities.MakeSQLSafe(testCategory);
            automated = DatabaseUtilities.MakeSQLSafe(automated);
            reasonForNotAutomated = DatabaseUtilities.MakeSQLSafe(reasonForNotAutomated);

            int testCaseId = GetNextTestCaseID(projectAbbreviation);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO [TestCases]"
                        + "   ([projectAbbreviation]"
                        + "   ,[testCaseId]"
                        + "   ,[testCaseDescription]"
                        + "   ,[active]"
                        + "   ,[testCaseOutdated]"
                        + "   ,[testScriptOutdated]"
                        + "   ,[testCaseSteps]"
                        + "   ,[expectedResults]"
                        + "   ,[testCaseNotes]"
                        + "   ,[dateLastUpdated]"
                        + "   ,[updatedBy]"
                        + "   ,[autoTestClass]"
                        + "   ,[autoMetaDataTable]"
                        + "   ,[autoMetaDataRow]"
                        + "   ,[dateCreated]"
                        + "   ,[createdBy]"
                        + "   ,[testCategory]"
                        + "   ,[automated]"
                        + "   ,[reasonForNotAutomated]"
                        + "   ,[dateAutomatedTestCreated]"
                        + "   ,[automatedTestCreatedBy])"
                        + "VALUES"
                        + "   ('" + projectAbbreviation + "'" // <projectAbbreviation, varchar(20),>
                        + "   ," + testCaseId // <testCaseId, int,>
                        + "   ,'" + testCaseDescription + "'" // <testCaseDescription, varchar,>
                        + "   ,'" + active + "'" // <active, bit,>
                        + "   ,'" + testCaseOutdated + "'" // <testCaseOutdated, bit,>
                        + "   ,'" + testScriptOutdated + "'" // <testScriptOutdated, bit,>
                        + "   ,'" + testCaseSteps + "'" // <testCaseSteps, varchar,>
                        + "   ,'" + expectedResults + "'" // <expectedResults, varchar,>
                        + "   ,'" + testCaseNotes + "'" // <testCaseNotes, varchar,>
                        + "   , getdate()"  // <dateLastUpdated, datetime,>
                        + "   ,'" + updatedBy + "'" // <updatedBy, nvarchar(256),>
                        + "   ," + (String.IsNullOrEmpty(autoTestClass) ? "null" : ("'" + autoTestClass + "'")) // <autoTestClass, varchar,>
                        + "   ," + (String.IsNullOrEmpty(autoMetaDataTable) ? "null" : ("'" + autoMetaDataTable + "'")) // <autoMetaDataTable, varchar,>
                        + "   ," + (autoMetaDataRow.HasValue ? autoMetaDataRow.Value.ToString() : "null") // <autoMetaDataRow, int,>)";
                        + "   , getdate()"  // <dateCreated, datetime,>
                        + "   ,'" + updatedBy + "'" // createdBy = updatedBy
                        + "   ," + (String.IsNullOrEmpty(testCategory) ? "null" : ("'" + testCategory + "'"))
                        + "   ," + (String.IsNullOrEmpty(automated) ? "null" : ("'" + automated + "'"))
                        + "   ," + (String.IsNullOrEmpty(reasonForNotAutomated) ? "null" : ("'" + reasonForNotAutomated + "'"))
                        + "   ," + (String.IsNullOrEmpty(autoTestClass) ? "null" : "getdate()")
                        + "   ," + (String.IsNullOrEmpty(autoTestClass) ? "null" : ("'" + updatedBy + "'"))
                        + "   )";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }

            return testCaseId;
        }


        public static int InsertProjectBrowserDefault(
                string projectAbbreviation,
                string browserAbbreviation,
                bool showBrowserColumn)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            browserAbbreviation = DatabaseUtilities.MakeSQLSafe(browserAbbreviation);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO [dbo].[ProjectBrowserInfo]"
                        + " ([projectAbbreviation]"
                        + " ,[browserAbbreviation]"
                        + " ,[showBrowserColumn])"
                        + " VALUES"
                        + " ( '" + projectAbbreviation + "'"
                        + ", '" + browserAbbreviation + "'"
                        + ", '" + showBrowserColumn + "')";

                    cmd.CommandType = System.Data.CommandType.Text;

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static string CheckIfProjectExists(
                string projectAbbreviation,
                string projectName
                )
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            projectName = DatabaseUtilities.MakeSQLSafe(projectName);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "select * from [dbo].[Projects]"
                                    + " where [projectAbbreviation]"
                                    + " = '" + projectAbbreviation + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            return "A project with abbreviation \"" + projectAbbreviation + "\" already exists";
                        }
                    }
                }

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "select * from [dbo].[Projects]"
                                    + " where [projectName]"
                                    + " = '" + projectName + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            return "A project named \"" + projectName + "\" already exists";
                        }
                    }
                }

                return null;
            }
        }

        public static void DeleteTestCaseScreenshot(
                string projectAbbreviation,
                string testCaseID,
                string imageId
                )
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            testCaseID = DatabaseUtilities.MakeSQLSafe(testCaseID);
            imageId = DatabaseUtilities.MakeSQLSafe(imageId);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "Delete from [dbo].[TestCaseScreenshots]"
                                    + " where projectAbbreviation = '" + projectAbbreviation + "'"
                                    + " and testCaseID = '" + testCaseID + "'"
                                    + " and imageId = '" + imageId + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteTestResultScreenshot(
                string projectAbbreviation,
                string testCaseID,
                string testRunID,
                string imageId
                )
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            testCaseID = DatabaseUtilities.MakeSQLSafe(testCaseID);
            testRunID = DatabaseUtilities.MakeSQLSafe(testRunID);
            imageId = DatabaseUtilities.MakeSQLSafe(imageId);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "Delete from [dbo].[TestResultScreenshots]"
                                    + " where projectAbbreviation = '" + projectAbbreviation + "'"
                                    + " and testCaseID = '" + testCaseID + "'"
                                    + " and testRunID = '" + testRunID + "'"
                                    + " and imageId = '" + imageId + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertProject(
                string projectAbbreviation,
                string projectName
                )
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            projectName = DatabaseUtilities.MakeSQLSafe(projectName);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [dbo].[Projects]"
                                    + " ([projectAbbreviation]"
                                    + " ,[projectName])"
                                    + " VALUES"
                                    + " ('" + projectAbbreviation + "'"
                                    + " ,'" + projectName + "')";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateTestCaseForAutomation(
            string user,
            string projectAbbreviation,
            int testCaseId,
            string autoTestClass,
            string autoMetaDataTable,
            int? autoMetaDataRow,
            string automated,
            string reasonForNotAutomated)
        {
            user = DatabaseUtilities.MakeSQLSafe(user);
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            autoTestClass = DatabaseUtilities.MakeSQLSafe(autoTestClass);
            automated = DatabaseUtilities.MakeSQLSafe(automated);
            reasonForNotAutomated = DatabaseUtilities.MakeSQLSafe(reasonForNotAutomated);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "exec [sp_UpdateTestCaseWithHistoryAutomation] "
                        + " '" + user + "'"
                        + ", '" + projectAbbreviation + "'"
                        + ", '" + testCaseId + "'"
                        + "   ," + (String.IsNullOrEmpty(autoTestClass) ? "null" : ("'" + autoTestClass + "'"))
                        + "   ," + (String.IsNullOrEmpty(autoMetaDataTable) ? "null" : ("'" + autoMetaDataTable + "'"))
                        + "   ," + (!autoMetaDataRow.HasValue ? "null" : ("'" + autoMetaDataRow + "'"))
                        + ", '" + automated + "'"
                        + ", '" + reasonForNotAutomated + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void partialUpdateTestCase(
            string user,
            string projectAbbreviation,
            int testCaseId,
            string testCaseDescription,
            bool active,
            bool chkTestCaseOutdated,
            bool chkTestScriptOutdated,
            string testCaseSteps,
            string expectedResults,
            string rawTestCaseNotes,
            string newTestCategory)
        {
            string autoTestClass;
            string autoMetaDataTable;
            int? autoMetaDataRow;
            string testCategory;
            string automated;
            string reasonForNotAutomated;

            string query = "select autoTestClass, autoMetaDataTable, autoMetaDataRow, testCategory, automated, reasonForNotAutomated "
                + " from dbo.TestCases "
                + " where projectAbbreviation = '" + projectAbbreviation + "' "
                + " and testCaseId = '" + testCaseId + "'";

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

                        autoTestClass = dr["autoTestClass"].ToString();
                        autoMetaDataTable = dr["autoMetaDataTable"].ToString();
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

                        if (newTestCategory == null)
                        {
                            testCategory = dr["testCategory"].ToString();
                        }
                        else
                        {
                            testCategory = newTestCategory;
                        }
                        automated = dr["automated"].ToString();
                        reasonForNotAutomated = dr["reasonForNotAutomated"].ToString();
                    }
                }
            }

            UpdateTestCase(
            user,
            projectAbbreviation,
            testCaseId,
            testCaseDescription,
            active,
            chkTestCaseOutdated,
            chkTestScriptOutdated,
            testCaseSteps,
            expectedResults,
            rawTestCaseNotes,
            autoTestClass,
            autoMetaDataTable,
            autoMetaDataRow,
            testCategory,
            automated,
            reasonForNotAutomated);
        }

        public static void UpdateTestCase(
            string user,
            string projectAbbreviation,
            int testCaseId,
            string testCaseDescription,
            bool active,
            bool testCaseOutdated,
            bool testScriptOutdated,
            string testCaseSteps,
            string expectedResults,
            string rawTestCaseNotes,
            string autoTestClass,
            string autoMetaDataTable,
            int? autoMetaDataRow,
            string testCategory,
            string automated,
            string reasonForNotAutomated)
        {
            user = DatabaseUtilities.MakeSQLSafe(user);
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            testCaseDescription = DatabaseUtilities.MakeSQLSafe(testCaseDescription);
            testCaseSteps = DatabaseUtilities.MakeSQLSafe(testCaseSteps);
            expectedResults = DatabaseUtilities.MakeSQLSafe(expectedResults);
            rawTestCaseNotes = DatabaseUtilities.MakeSQLSafe(rawTestCaseNotes);
            autoTestClass = DatabaseUtilities.MakeSQLSafe(autoTestClass);
            testCategory = DatabaseUtilities.MakeSQLSafe(testCategory);
            automated = DatabaseUtilities.MakeSQLSafe(automated);
            reasonForNotAutomated = DatabaseUtilities.MakeSQLSafe(reasonForNotAutomated);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "exec sp_UpdateTestCaseWithHistory "
                        + " '" + user + "'" // @user
                        + ", '" + projectAbbreviation + "'" // @projectAbbreviation
                        + ", '" + testCaseId + "'" // @testCaseId
                        + ", '" + testCaseDescription + "'" // @rawTestCaseDescription
                        + ", '" + active + "'" // @rawActive
                        + ", '" + testCaseOutdated + "'" // @rawTestCaseOutdated
                        + ", '" + testScriptOutdated + "'" // @rawTestScriptOutdated
                        + ", '" + testCaseSteps + "'" // @rawTestCaseSteps
                        + ", '" + expectedResults + "'" // @rawExpectedResults
                        + ", '" + rawTestCaseNotes + "'" // @rawTestCaseNotes
                        + ", '" + testCategory + "'"; // @testCategory"
                    //+", '" + autoMetaDataTable + "'"; // @autoMetaDataTable"
                    //+", " + (autoMetaDataRow.HasValue ? autoMetaDataRow.Value.ToString() : "null") // @autoMetaDataRow";
                    //+ ", '" + testCategory + "'" // @testCategory"
                    //+ ", '" + automated + "'" // @automated"
                    //+ ", '" + reasonForNotAutomated + "'"; // @reasonForNotAutomated"

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static void RestoreTestCase(
            string user,
            string projectAbbreviation,
            int testCaseId)
        {
            user = DatabaseUtilities.MakeSQLSafe(user);
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            string spName;
            spName = "sp_RestoreTestCaseFromHistory";

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {

                    cmd.CommandText =
                        "exec " + spName
                        + " '" + user + "'" // @user
                        + ", '" + projectAbbreviation + "'" // @projectAbbreviation
                        + ", " + testCaseId; // @testCaseId"

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void SetupProjectBrowsers(string projectAbbreviation)
        {

            //Get the Browser table
            DataTable browsers = DatabaseUtilities.GetDataTable("select browserAbbreviation, showBrowserColumn from Browsers");

            foreach (DataRow row in browsers.Rows)
            {
                //Store the data in variables
                String browserAbbreviation = row["browserAbbreviation"].ToString();
                bool showBrowserColumn = Convert.ToBoolean(row["showBrowserColumn"]);

                projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
                browserAbbreviation = DatabaseUtilities.MakeSQLSafe(browserAbbreviation);

                CTMethods.InsertProjectBrowserDefault(projectAbbreviation, browserAbbreviation, showBrowserColumn);

            }
        }


        public static void AddReleases(int testCaseID,
            string projectAbbreviation,
            string releases)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            releases = DatabaseUtilities.MakeSQLSafe(releases);

            if (String.IsNullOrEmpty(releases))
            {
                return;
            }

            List<string> splitReleases = releases.Split(new[] { '|' }).ToList();
            foreach (string singleRelease in splitReleases)
            {
                AddSingleReleaseIfNeeded(testCaseID, projectAbbreviation, singleRelease);
            }

        }

        public static void AddSingleReleaseIfNeeded(int testCaseID,
            string projectAbbreviation,
            string singleRelease)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            singleRelease = DatabaseUtilities.MakeSQLSafe(singleRelease);
            string releaseTable;
            releaseTable = "ReleaseTestCases";

            string mySelectQuery = "SELECT * from " + releaseTable + " where "
                + " projectAbbreviation = '" + projectAbbreviation + "'"
                + " and testCaseId = " + testCaseID
                + " and release = '" + singleRelease + "'";

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
                            AddSingleRelease(testCaseID, projectAbbreviation, singleRelease);
                        }
                        // Otherwise it's already in the database
                    }
                }
            }
        }

        public static void AddSingleRelease(int testCaseID,
            string projectAbbreviation,
            string singleRelease)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            singleRelease = DatabaseUtilities.MakeSQLSafe(singleRelease);
            string releaseTable;
            releaseTable = "ReleaseTestCases";

            string mySelectQuery = "INSERT INTO " + releaseTable
            + " ([projectAbbreviation] "
            + " ,[testCaseId] "
            + " ,[release]) "
            + " VALUES "
            + " ('" + projectAbbreviation
            + "', " + testCaseID
            + ", '" + singleRelease + "')";

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

        public static void AddGroups(int testCaseID,
            string projectAbbreviation,
            string groups)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            groups = DatabaseUtilities.MakeSQLSafe(groups);

            if (String.IsNullOrEmpty(groups))
            {
                return;
            }

            List<string> splitGroups = groups.Split(new[] { '|' }).ToList();
            foreach (string singleGroup in splitGroups)
            {
                AddSingleGroupIfNeeded(testCaseID, projectAbbreviation, singleGroup);
            }

        }

        public static void AddSingleGroupIfNeeded(int testCaseID,
            string projectAbbreviation,
            string singleGroup)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            singleGroup = DatabaseUtilities.MakeSQLSafe(singleGroup);

            string mySelectQuery = "SELECT * from GroupTestCases where "
                + " projectAbbreviation = '" + projectAbbreviation + "'"
                + " and testCaseId = " + testCaseID
                + " and groupTestAbbreviation = '" + singleGroup + "'";

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
                            AddSingleGroup(testCaseID, projectAbbreviation, singleGroup);
                        }
                        // Otherwise it's already in the database
                    }
                }
            }
        }

        public static void AddSingleGroup(int testCaseID,
            string projectAbbreviation,
            string singleGroup)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            singleGroup = DatabaseUtilities.MakeSQLSafe(singleGroup);

            string mySelectQuery = "INSERT INTO [GroupTestCases]"
            + " ([projectAbbreviation] "
            + " ,[testCaseId] "
            + " ,[groupTestAbbreviation]) "
            + " VALUES "
            + " ('" + projectAbbreviation
            + "', " + testCaseID
            + ", '" + singleGroup + "')";

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

        public static void AddSprints(int testCaseID, string projectAbbreviation, string sprints)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            sprints = DatabaseUtilities.MakeSQLSafe(sprints);

            if (String.IsNullOrEmpty(sprints))
            {
                return;
            }

            List<string> splitSprints = sprints.Split(new[] { '|' }).ToList();
            foreach (string singleSprint in splitSprints)
            {
                AddSingleSprintIfNeeded(testCaseID, projectAbbreviation, singleSprint);
            }

        }

        public static void AddSingleSprintIfNeeded(int testCaseID, string projectAbbreviation, string singleSprint)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            singleSprint = DatabaseUtilities.MakeSQLSafe(singleSprint);
            string sprintTable;
            sprintTable = "SprintTestCases";

            string mySelectQuery = "SELECT * from " + sprintTable + " where "
                + " projectAbbreviation = '" + projectAbbreviation + "'"
                + " and testCaseId = " + testCaseID
                + " and sprint = '" + singleSprint + "'";

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
                            AddSingleSprint(testCaseID, projectAbbreviation, singleSprint);
                        }
                        // Otherwise it's already in the database
                    }
                }
            }
        }

        
        
        public static void AddSingleSprint(int testCaseID, string projectAbbreviation, string singleSprint)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            singleSprint = DatabaseUtilities.MakeSQLSafe(singleSprint);
            string sprintTable;
            sprintTable = "SprintTestCases";

            string mySelectQuery = "INSERT INTO " + sprintTable
            + " ([projectAbbreviation] "
            + " ,[testCaseId] "
            + " ,[sprint]) "
            + " VALUES "
            + " ('" + projectAbbreviation
            + "', " + testCaseID
            + ", '" + singleSprint + "')";

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

        public static string ValidateGroups(string projectAbbreviation, string groupTestAbbreviations)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            groupTestAbbreviations = DatabaseUtilities.MakeSQLSafe(groupTestAbbreviations);

            if (String.IsNullOrEmpty(groupTestAbbreviations))
            {
                return null;
            }

            List<string> splitGroupTestAbbreviations = groupTestAbbreviations.Split(new[] { '|' }).ToList();
            foreach (string singleGroupTestAbbreviation in splitGroupTestAbbreviations)
            {
                bool valid = ValidateProjectAndSingleGroup(projectAbbreviation, singleGroupTestAbbreviation);
                if (!valid)
                {
                    return "The group " + projectAbbreviation + " - " + singleGroupTestAbbreviation + " must be created in Crystal Test before uploading.";
                }
            }
            return null;
        }

        public static string ValidateReleases(string projectAbbreviation, string releases)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            releases = DatabaseUtilities.MakeSQLSafe(releases);

            if (String.IsNullOrEmpty(releases))
            {
                return null;
            }

            List<string> splitReleases = releases.Split(new[] { '|' }).ToList();
            foreach (string singleRelease in splitReleases)
            {
                bool valid = ValidateProjectAndSingleRelease(projectAbbreviation, singleRelease);
                if (!valid)
                {
                    return "The release " + projectAbbreviation + " - " + singleRelease + " must be created in Crystal Test before uploading.";
                }
            }
            return null;
        }

        public static string ValidateSprints(string projectAbbreviation, string sprints)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            sprints = DatabaseUtilities.MakeSQLSafe(sprints);

            if (String.IsNullOrEmpty(sprints))
            {
                return null;
            }

            List<string> splitSprints = sprints.Split(new[] { '|' }).ToList();
            foreach (string singleSprint in splitSprints)
            {
                bool valid = ValidateProjectAndSingleSprint(projectAbbreviation, singleSprint);
                if (!valid)
                {
                    return "The sprint " + projectAbbreviation + " - " + singleSprint + " must be created in Crystal Test before uploading.";
                }
            }
            return null;
        }

        public static string ValidateTestCaseID(string projectAbbreviation, string testCaseId, string isThisAnUpdate)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            testCaseId = DatabaseUtilities.MakeSQLSafe(testCaseId);
            isThisAnUpdate = DatabaseUtilities.MakeSQLSafe(isThisAnUpdate);

            if (String.IsNullOrEmpty(isThisAnUpdate) || isThisAnUpdate.ToUpper() == "N" || isThisAnUpdate == "0")
            {
                isThisAnUpdate = "NO";
            }

            if (isThisAnUpdate.ToUpper() == "Y" || isThisAnUpdate == "1")
            {
                isThisAnUpdate = "YES";
            }

            isThisAnUpdate = isThisAnUpdate.ToUpper();

            if (isThisAnUpdate == "NO")
            {
                if (String.IsNullOrEmpty(testCaseId))
                {
                    return null;
                }
                else
                {
                    return "Invalid testCaseId: A value of " + testCaseId + " was given in the testCaseId field, but there was no value in the isThisAnUpdate? field. (Only updates can specify a testCaseID)";
                }
            }
            else if (isThisAnUpdate == "YES")
            {
                // Else, it's an update

                int testCaseInt;
                bool parsed = Int32.TryParse(testCaseId, out testCaseInt);
                if (!parsed || testCaseInt <= 0)
                {
                    return "Invalid testCaseId (not a whole number): " + testCaseId;
                }

                using (System.Data.SqlClient.SqlConnection Conn =
                    new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
                {
                    Conn.Open();

                    using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                    {
                        // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                        cmd.CommandText =
                            "Select * from TestCases "
                            + "where testCaseId = " + testCaseInt
                            + "and projectAbbreviation = '" + projectAbbreviation + "'";

                        cmd.CommandType = System.Data.CommandType.Text;

                        using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (!dr.HasRows)
                            {
                                return "Invalid testCaseId: Test case " + projectAbbreviation + "-" + testCaseId + " does not exist but it is set as an update on the upload sheet.";
                            }
                        }
                    }
                }
            }
            else
            {
                return "Invalid 'isThisAnUpdate' value (must be yes, no, y, n, 1, 0, or blank): " + isThisAnUpdate;
            }

            return null;
        }

        public static string ValidateTestCaseIDForResults(string projectAbbreviation, string testCaseId)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            testCaseId = DatabaseUtilities.MakeSQLSafe(testCaseId);

            int testCaseInt;
            bool parsed = Int32.TryParse(testCaseId, out testCaseInt);
            if (!parsed || testCaseInt <= 0)
            {
                return "Invalid testCaseId (not a whole number): " + testCaseId;
            }

            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                    cmd.CommandText =
                        "Select * from TestCases "
                        + " where testCaseId = " + testCaseInt
                        + " and projectAbbreviation = '" + projectAbbreviation + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // If any rows matched, it passed.
                        if (!dr.HasRows)
                        {
                            return "Invalid testCaseId: The test case " + projectAbbreviation + "-" + testCaseId + " does not exist.";
                        }
                    }
                }
            }

            return null;
        }

        public static string ValidateEnvironmentExists(string environment, bool shouldExist)
        {
            environment = DatabaseUtilities.MakeSQLSafe(environment);
            
            string query = "Select * from Environments " + " WHERE environment = '" + environment + "'";

            if (environment.Contains("|") ||
                    environment.Contains("'") ||
                    environment.Contains(",") ||
                    environment.Contains("\""))
            {
                return "Fields cannot contain characters like \"|\", a comma or any quotes.  Please remove them.";
            }

            DataTable validationDataTable = DatabaseUtilities.GetDataTable(query);
            if (shouldExist)
            {
                // If any rows matched, it passed.
                if (validationDataTable.Rows.Count == 0)
                {
                    return "Invalid environment (doesn't exist): <b>" + environment + "</b>";
                }
            }

            if (!shouldExist)
            {
                // If no rows matched, it passed.
                if (validationDataTable.Rows.Count > 0)
                {
                    return "Duplicate Entry: <b>" + environment + "</b>";
                }
            }
            return null;
        }

        public static string ValidateProjectEnvironmentExists(string projectAbbreviation, string environment, bool shouldExist)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            environment = DatabaseUtilities.MakeSQLSafe(environment);

            string query = "Select * from ProjectEnvironmentInfo " + " WHERE projectAbbreviation = '" + projectAbbreviation + "' AND environment = '" + environment + "'";

            if (environment.Contains("|") ||
                    environment.Contains("'") ||
                    environment.Contains(",") ||
                    environment.Contains("\""))
            {
                return "Fields cannot contain characters like \"|\", a comma or any quotes.  Please remove them.";
            }

            DataTable validationDataTable = DatabaseUtilities.GetDataTable(query);
            if (shouldExist)
            {
                // If any rows matched, it passed.
                if (validationDataTable.Rows.Count == 0)
                {
                    return "Invalid projectEnvironment (doesn't exist):  <b>" + environment + "</b>";
                }
            }

            if (!shouldExist)
            {
                // If no rows matched, it passed.
                if (validationDataTable.Rows.Count > 0)
                {
                    return "Duplicate Entry: <b>" + environment + "</b>";
                }
            }
            return null;
        }

        public static string ValidateSprintExists(string projectAbbreviation, string sprintName, string txtbxEditDescription, bool shouldExist)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            sprintName = DatabaseUtilities.MakeSQLSafe(sprintName);
            
            string query = "Select * from Sprints " + " WHERE projectAbbreviation = '" + projectAbbreviation + "' AND sprint = '" + sprintName + "'";

            DataTable validationDataTable = DatabaseUtilities.GetDataTable(query);

            if (sprintName.Contains("|") ||
                    sprintName.Contains("'") ||
                    sprintName.Contains(",") ||
                    sprintName.Contains("\"") ||
                    txtbxEditDescription.Contains("|") ||
                    txtbxEditDescription.Contains("'") ||
                    txtbxEditDescription.Contains(",") ||
                    txtbxEditDescription.Contains("\""))
            {
                return "Fields cannot contain characters like \"|\", a comma or any quotes.  Please remove them.";
            }

            if (shouldExist)
            {
                // If any rows matched, it passed.
                if (validationDataTable.Rows.Count == 0)
                {
                    return "Invalid sprint (doesn't exist): <b>" + sprintName + "</b>";
                }
            }

            if (!shouldExist)
            {
                // If no rows matched, it passed.
                if (validationDataTable.Rows.Count > 0)
                {
                    return "Duplicate Sprint: <b>" + sprintName + "</b>";
                }
            }
            return null;
        }

        public static string ValidateGroupExists(string projectAbbreviation, string groupAbbreviation, string groupName, string groupDescription, bool shouldExist)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            groupAbbreviation = DatabaseUtilities.MakeSQLSafe(groupAbbreviation);


            if (groupAbbreviation.Contains("|") ||
                    groupAbbreviation.Contains("'") ||
                    groupAbbreviation.Contains(",") ||
                    groupAbbreviation.Contains("\"") ||
                    groupDescription.Contains("|") ||
                    groupDescription.Contains("'") ||
                    groupDescription.Contains(",") ||
                    groupDescription.Contains("\"") ||
                    groupName.Contains("|") ||
                    groupName.Contains("'") ||
                    groupName.Contains(",") ||
                    groupName.Contains("\""))
            {
                return "Fields cannot contain characters like \"|\", a comma or any quotes.  Please remove them.";
            }

            string query = "Select * from GroupTests " + " WHERE projectAbbreviation = '" + projectAbbreviation + "' AND groupTestAbbreviation = '" + groupAbbreviation + "'";
            
            DataTable validationDataTable = DatabaseUtilities.GetDataTable(query);

            if (shouldExist)
            {
                // If any rows matched, it passed.
                if (validationDataTable.Rows.Count == 0)
                {
                    return "Invalid group (doesn't exist): <b>" + groupAbbreviation + "</b>";
                }
            }

            if (!shouldExist)
            {
                // If no rows matched, it passed.
                if (validationDataTable.Rows.Count > 0)
                {
                    return "Duplicate Group: <b>" + groupAbbreviation + "</b>";
                }

                //Check for duplicate groupTestName
                string query2 = "Select * from GroupTests " + " WHERE projectAbbreviation = '" + projectAbbreviation + "' AND groupTestName = '" + groupName + "'";
                DataTable validationDataTable2 = DatabaseUtilities.GetDataTable(query2);
                
                if (validationDataTable2.Rows.Count > 0)
                {
                    return "Duplicate Group Name: <b>" + groupName + "</b>";
                }
            }
            return null;
        }

        public static string ValidateReleaseExists(string projectAbbreviation, string releaseName, string txtbxEditDescription, bool shouldExist)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            releaseName = DatabaseUtilities.MakeSQLSafe(releaseName);
            
            string query = "Select * from Releases " + " WHERE projectAbbreviation = '" + projectAbbreviation + "' AND release = '" + releaseName + "'";

            if (releaseName.Contains("|") ||
                    releaseName.Contains("'") ||
                    releaseName.Contains(",") ||
                    releaseName.Contains("\"") ||
                    txtbxEditDescription.Contains("|") ||
                    txtbxEditDescription.Contains("'") ||
                    txtbxEditDescription.Contains(",") ||
                    txtbxEditDescription.Contains("\""))
            {
                return "Fields cannot contain characters like \"|\", a comma or any quotes.  Please remove them.";
            }

            DataTable validationDataTable = DatabaseUtilities.GetDataTable(query);

            if (shouldExist)
            {
                // If any rows matched, it passed.
                if (validationDataTable.Rows.Count == 0)
                {
                    return "Invalid release (doesn't exist): <b>" + releaseName + "</b>";
                }
            }

            if (!shouldExist)
            {
                // If no rows matched, it passed.
                if (validationDataTable.Rows.Count > 0)
                {
                    return "Duplicate Release: <b>" + releaseName + "</b>";
                }
            }
            return null;
        }

        public static string ValidateProjectExists(string projectAbbreviation, bool shouldExist)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            string query = "Select * from Projects " + " where projectAbbreviation = '" + projectAbbreviation + "'";

            if (projectAbbreviation.Contains("|") ||
                    projectAbbreviation.Contains("'") ||
                    projectAbbreviation.Contains(",") ||
                    projectAbbreviation.Contains("\""))
            {
                return "Fields cannot contain characters like \"|\", a comma or any quotes.  Please remove them.";
            }
            
            DataTable validationDataTable = DatabaseUtilities.GetDataTable(query);

            if (shouldExist)
            {
                // If any rows matched, it passed.
                if (validationDataTable.Rows.Count == 0)
                {
                    return "Invalid project (doesn't exist): <b>" + projectAbbreviation + "</b>";
                }
            }

            if (!shouldExist)
            {
                // If no rows matched, it passed.
                if (validationDataTable.Rows.Count > 0)
                {
                    return "Duplicate Project: <b>" + projectAbbreviation + "</b>";
                }
            }
            return null;
        }

        public static string ValidateBrowserAbbreviation(string browserAbbreviation)
        {
            browserAbbreviation = DatabaseUtilities.MakeSQLSafe(browserAbbreviation);

            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                    cmd.CommandText =
                        "Select * from Browsers "
                        + " where browserAbbreviation = '" + browserAbbreviation + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // If any rows matched, it passed.
                        if (!dr.HasRows)
                        {
                            return "Invalid browserAbbreviation (doesn't exist): " + browserAbbreviation;
                        }
                    }
                }
            }

            return null;
        }

        public static string ValidateStatus(string status)
        {
            status = DatabaseUtilities.MakeSQLSafe(status);

            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                    cmd.CommandText =
                        "Select * from Statuses "
                        + " where status = '" + status + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // If any rows matched, it passed.
                        if (!dr.HasRows)
                        {
                            return "Invalid status (doesn't exist): " + status;
                        }
                    }
                }
            }

            return null;
        }

        public static void RemoveAllDefaultEnvironments(string projectAbbreviation)
        {
            string query = "UPDATE [dbo].[ProjectEnvironmentInfo] SET [defaultEnvironment] = 'False' WHERE [projectAbbreviation] = '" + projectAbbreviation + "' AND [defaultEnvironment] = 'True'";
            DatabaseUtilities.ExecuteQuery(query);
        }

        public static void AddDefaultEnvironments(string projectAbbreviation, string environment)
        {
            string query = "UPDATE [dbo].[ProjectEnvironmentInfo] SET [defaultEnvironment] = 'True' WHERE [projectAbbreviation] = '" + projectAbbreviation + "' AND [environment] = '" + environment + "'";
            DatabaseUtilities.ExecuteQuery(query);
        }

        public static bool ValidateProjectAndSingleGroup(string projectAbbreviation, string groupTestAbbreviation)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            groupTestAbbreviation = DatabaseUtilities.MakeSQLSafe(groupTestAbbreviation);

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
                        "Select * from GroupTests "
                        + "where projectAbbreviation = '" + projectAbbreviation + "' "
                        + "and groupTestAbbreviation = '" + groupTestAbbreviation + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // If any rows matched, it passed.
                        return dr.HasRows;
                    }
                }
            }
        }

        public static bool ValidateProjectAndSingleRelease(string projectAbbreviation, string release)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            release = DatabaseUtilities.MakeSQLSafe(release);

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
                        "Select * from Releases "
                        + "where projectAbbreviation = '" + projectAbbreviation + "' "
                        + "and release = '" + release + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // If any rows matched, it passed.
                        return dr.HasRows;
                    }
                }
            }
        }

        public static bool ValidateProjectAndSingleSprint(string projectAbbreviation, string sprint)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            sprint = DatabaseUtilities.MakeSQLSafe(sprint);

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
                        "Select * from Sprints "
                        + "where projectAbbreviation = '" + projectAbbreviation + "' "
                        + "and sprint = '" + sprint + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // If any rows matched, it passed.
                        return dr.HasRows;
                    }
                }
            }
        }

        public static void ValidateNewSprint(string projectAbbreviation, string sprintName)
        {
 
        }

        public static void RemoveReleaseForTestCase(string projectAbbreviation, string release, int testCaseID)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            release = DatabaseUtilities.MakeSQLSafe(release);
            string releaseTable;
            releaseTable = "ReleaseTestCases";

            string mySelectQuery = "DELETE FROM " + releaseTable
            + " where projectAbbreviation = '" + projectAbbreviation + "'"
            + " and release = '" + release + "'"
            + " and testCaseId = " + testCaseID;

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

        public static void RemoveGroupForTestCase(string projectAbbreviation, string groupTestAbbreviation, int testCaseID)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            groupTestAbbreviation = DatabaseUtilities.MakeSQLSafe(groupTestAbbreviation);

            string mySelectQuery = "DELETE FROM [GroupTestCases]"
            + " where projectAbbreviation = '" + projectAbbreviation + "'"
            + " and groupTestAbbreviation = '" + groupTestAbbreviation + "'"
            + " and testCaseId = " + testCaseID;

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

        public static void RemoveSprintForTestCase(string projectAbbreviation, string sprint, int testCaseID)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            sprint = DatabaseUtilities.MakeSQLSafe(sprint);
            string sprintTable;
            sprintTable = "SprintTestCases";

            string mySelectQuery = "DELETE FROM " + sprintTable
            + " where projectAbbreviation = '" + projectAbbreviation + "'"
            + " and sprint = '" + sprint + "'"
            + " and testCaseId = " + testCaseID;

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

        public static void RemoveTestCase(string projectAbbreviation, string testCaseIsString, IIdentity identity)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            testCaseIsString = DatabaseUtilities.MakeSQLSafe(testCaseIsString);

            int testCaseID = Convert.ToInt32(testCaseIsString);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "exec sp_DeleteTestCaseWithHistory "
                        + " '" + Utilities.AspUtilities.RemovePrefixFromUserName(identity.Name) + "'" // @user
                        + ", '" + projectAbbreviation + "'" // @projectAbbreviation
                        + ", " + testCaseID; // @testCaseId

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void RemoveFromQueue(string projectAbbreviation, string testCaseIsString, string environment)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            testCaseIsString = DatabaseUtilities.MakeSQLSafe(testCaseIsString);

            int testCaseID = Convert.ToInt32(testCaseIsString);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "Delete from [dbo].[TestResults]"
                                    + " where projectAbbreviation = '" + projectAbbreviation + "'"
                                    + " and testCaseId = '" + testCaseID + "'"
                                    + " and environment = '" + environment + "'"
                                    + " and status = '" + Constants.ProcessStatus.IN_QUEUE + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static void RemoveGroup(string projectAbbreviation, string groupTestAbbreviation)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            groupTestAbbreviation = DatabaseUtilities.MakeSQLSafe(groupTestAbbreviation);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from GroupTestCasesHistory "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and groupTestAbbreviation = '" + groupTestAbbreviation + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from GroupTestCases "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and groupTestAbbreviation = '" + groupTestAbbreviation + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from GroupTests "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and groupTestAbbreviation = '" + groupTestAbbreviation + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void RemoveEnvironment(string projectAbbreviation, string environment)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            environment = DatabaseUtilities.MakeSQLSafe(environment);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from ProjectEnvironmentInfo "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and environment = '" + environment + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void RemoveRelease(string projectAbbreviation, string release)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            release = DatabaseUtilities.MakeSQLSafe(release);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from ReleaseTestCasesHistory "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and release = '" + release + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from ReleaseTestCases "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and release = '" + release + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from Releases "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and release = '" + release + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void RemoveSprint(string projectAbbreviation, string sprint)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            sprint = DatabaseUtilities.MakeSQLSafe(sprint);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from SprintTestCasesHistory "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and sprint = '" + sprint + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from SprintTestCases "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and sprint = '" + sprint + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "Delete from Sprints "
                        + " where projectAbbreviation = '" + projectAbbreviation + "'"
                        + " and sprint = '" + sprint + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static void UpdateUserProfiles(
            string UserName,
            string userFirstName,
            string userLastName,
            string userSupervisorFirstName,
            string userSupervisorLastName,
            string userSupervisorEmail,
            string defaultProjectAbbreviation)
        {
            UserName = DatabaseUtilities.MakeSQLSafe(UserName);
            userFirstName = DatabaseUtilities.MakeSQLSafe(userFirstName);
            userLastName = DatabaseUtilities.MakeSQLSafe(userLastName);
            userSupervisorFirstName = DatabaseUtilities.MakeSQLSafe(userSupervisorFirstName);
            userSupervisorLastName = DatabaseUtilities.MakeSQLSafe(userSupervisorLastName);
            userSupervisorEmail = DatabaseUtilities.MakeSQLSafe(userSupervisorEmail);
            defaultProjectAbbreviation = DatabaseUtilities.MakeSQLSafe(defaultProjectAbbreviation);


            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "UPDATE UserProfiles " +
                        " set userFirstName = '" + userFirstName + "', " +
                        " userLastName = '" + userLastName + "', " +
                        " userSupervisorFirstName = '" + userSupervisorFirstName + "', " +
                        " userSupervisorLastName = '" + userSupervisorLastName + "', " +
                        " userSupervisorEmail = '" + userSupervisorEmail + "', " +
                        " defaultProjectAbbreviation = '" + defaultProjectAbbreviation + "' " +
                        " FROM aspnet_Users " +
                        " JOIN UserProfiles ON aspnet_Users.UserId = UserProfiles.UserId " +
                        " WHERE UserName='" + UserName + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateUserMembership(string UserName, string userEmail)
        {
            UserName = DatabaseUtilities.MakeSQLSafe(UserName);
            UserName = DatabaseUtilities.MakeSQLSafe(userEmail);

            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "UPDATE aspnet_Membership " +
                        " set email = '" + userEmail + "' " +
                        " FROM aspnet_Users " +
                        " JOIN aspnet_Membership ON aspnet_Users.UserId = aspnet_Membership.UserId " +
                        " WHERE UserName='" + UserName + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AssignSingleUserIfNeeded(int testCaseID, string projectAbbreviation, string userId)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            userId = DatabaseUtilities.MakeSQLSafe(userId);

            string mySelectQuery = "SELECT * from TestCaseAssignments where "
                + " projectAbbreviation = '" + projectAbbreviation + "'"
                + " and testCaseId = " + testCaseID
                + " and userId = '" + userId + "'";

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
                            AssignSingleUser(testCaseID, projectAbbreviation, userId);
                        }
                        // Otherwise it's already in the database
                    }
                }
            }
        }


        public static void AssignSingleUser(int testCaseID, string projectAbbreviation, string userId)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            userId = DatabaseUtilities.MakeSQLSafe(userId);

            string mySelectQuery = "INSERT INTO [TestCaseAssignments]"
            + " ([projectAbbreviation] "
            + " ,[testCaseId] "
            + " ,[userId]"
            + ",dateAssigned) "
            + " VALUES "
            + " ('" + projectAbbreviation
            + "', " + testCaseID
            + ", '" + userId + "'"
            + ",getdate())";

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

        public static void UnAssignSingleUser(int testCaseID, string projectAbbreviation, string userId)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            userId = DatabaseUtilities.MakeSQLSafe(userId);

            string mySelectQuery = "delete from [TestCaseAssignments] "
            + " where projectAbbreviation = '" + projectAbbreviation + "' "
            + " and testCaseId = " + testCaseID + " "
            + " and userId = '" + userId + "' ";

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

        public static string VerifyUserCreatedProperlyElseRemove(string userName)
        {
            userName = DatabaseUtilities.MakeSQLSafe(userName);

            string error = null;

            try
            {

                using (System.Data.SqlClient.SqlConnection Conn =
                    new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
                {
                    Conn.Open();

                    using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                    {
                        // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                        cmd.CommandText =
                            "select * "
                            + " from aspnet_Users "
                            + " join aspnet_Membership on aspnet_Users.UserId = aspnet_Membership.UserId "
                            + " join UserProfiles on aspnet_Users.UserId = UserProfiles.UserId "
                            + " where userName = '" + userName + "'";

                        cmd.CommandType = System.Data.CommandType.Text;

                        using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                        {
                            // If any rows matched, it passed.
                            if (!dr.HasRows)
                            {
                                error = "User not created properly, please try again";
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                error = "Unexpected error: " + e.Message;
            }

            if (!String.IsNullOrEmpty(error))
            {
                RemoveUser(userName);
            }

            return error;
        }

        public static void RemoveUser(string userName)
        {
            userName = DatabaseUtilities.MakeSQLSafe(userName);

            using (System.Data.SqlClient.SqlConnection Conn =
            new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    // The view TestCaseWithStatus aggregates the status of all browsers to give a single status
                    cmd.CommandText =
                        "delete UserProfiles "
                        + " from aspnet_Users "
                        + " join UserProfiles on aspnet_Users.UserId = UserProfiles.UserId "
                        + " where userName = '" + userName + "'; "

                        + " delete aspnet_Membership "
                        + " from aspnet_Users "
                        + " join aspnet_Membership on aspnet_Users.UserId = aspnet_Membership.UserId "
                        + " where userName = '" + userName + "'; "

                        + " delete aspnet_Users "
                        + " from aspnet_Users "
                        + " where userName = '" + userName + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddTestCaseScreenshots(int testCaseID, string projectAbbreviation, string screenshots, string screenshotDescriptions)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            screenshots = DatabaseUtilities.MakeSQLSafe(screenshots);

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
                AddSingleTestCaseScreenshotIfNeeded(testCaseID, projectAbbreviation, imageURL, imageDescription);
            }

        }

        public static bool AddSingleTestCaseScreenshotIfNeeded(int testCaseID, string projectAbbreviation, string imageURL, string description)
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
                            AddSingleTestCaseScreenshot(testCaseID, projectAbbreviation, imageURL, description);
                            return true;
                        }
                        else
                        {
                            // Otherwise it's already in the database
                            UpdateSingleTestCaseScreenshot(testCaseID, projectAbbreviation, imageURL, description);
                            return false;
                        }
                    }
                }
            }
        }

        public static void RemoveAutomatedTestMap(
            string projectAbbreviation,
            int parentTestCaseId,
            int childTestCaseId)
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
                        "delete from [AutoTestCaseMap] "
                        + " where projectAbbreviation = '" + projectAbbreviation + "' "
                        + " and parentTestCaseId = " + parentTestCaseId + " "
                        + " and childTestCaseId = " + childTestCaseId + " ";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddAutomatedTestMap(
            string projectAbbreviation,
            int parentTestCaseId,
            int childTestCaseId,
            string userID)
        {
            RemoveAutomatedTestMap(
                projectAbbreviation,
                parentTestCaseId,
                childTestCaseId);

            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO [AutoTestCaseMap] "
                        + "           ([projectAbbreviation] "
                        + "           ,[parentTestCaseId] "
                        + "           ,[childTestCaseId] "
                        + "           ,[childTestCaseInstalledBy] "
                        + "           ,[childTestCaseInstalledDate]) "
                        + "     VALUES "
                        + "           ('" + projectAbbreviation + "' "
                        + "           ," + parentTestCaseId + " "
                        + "           ," + childTestCaseId + " "
                        + "           ,'" + userID + "' "
                        + "           ,GETDATE()) ";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Adding Updates for Automated field for Child cases here.
        public static void RemoveChildCase(string project, int testCaseId)
        {
            SqlConnection conn = new SqlConnection(Constants.QAAConnectionString);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE [TestCases] " +
                        "SET AUTOMATED = null WHERE " +
                        "projectAbbreviation = '" + project + "' AND " +
                        "testCaseId = " + testCaseId;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static void AddChildCase(string project, int testCaseId)
        {
            SqlConnection conn = new SqlConnection(Constants.QAAConnectionString);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE [TestCases] " +
                        "SET AUTOMATED = 'Child' WHERE " +
                        "projectAbbreviation = '" + project + "' AND " +
                        "testCaseId = " + testCaseId;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static void ToggleProjectSupportedBrowser(string projectAbbreviation, string browserAbbreviation, bool showBrowserColumn)
        {

            SqlConnection conn = new SqlConnection(Constants.QAAConnectionString);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE [ProjectBrowserInfo] " +
                        "SET showBrowserColumn = '" + !showBrowserColumn + "' WHERE " +
                        "projectAbbreviation = '" + projectAbbreviation + "' AND " +
                        "browserAbbreviation = '" + browserAbbreviation + "'";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static bool IsAdmin(string userName)
        {
            string userID = "";
            string roleID = "";

            SqlConnection Conn = new SqlConnection(Constants.QAAConnectionString);
            Conn.Open();

            SqlCommand cmd1 = Conn.CreateCommand();
            cmd1.CommandText = "select UserID from aspnet_Users " +
                        "where UserName = '" + userName + "'";
            cmd1.CommandType = CommandType.Text;
            SqlDataReader dr1 = cmd1.ExecuteReader();

            dr1.Read();
            if (dr1.HasRows)
            {
                userID = dr1[0].ToString();
            }
            else
            {
                return false;
            }
            dr1.Close();

            SqlCommand cmd2 = Conn.CreateCommand();
            cmd2.CommandText = "select RoleId from aspnet_Roles " +
                        "where RoleName = 'Admin'";
            cmd2.CommandType = CommandType.Text;
            SqlDataReader dr2 = cmd2.ExecuteReader();

            dr2.Read();
            if (dr2.HasRows)
            {
                roleID = dr2[0].ToString();
            }
            else
            {
                return false;
            }
            dr2.Close();

            SqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "select * from aspnet_UsersInRoles " +
                        "where UserId = '" + userID + "' " +
                        "and RoleId = '" + roleID + "'";

            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static public string[] GetVisibleBrowserAbbreviations(string projectAbbreviation)
        {

            string query = "SELECT ProjectBrowserInfo.browserAbbreviation, "
                + "ProjectBrowserInfo.showBrowserColumn "
                + "FROM ProjectBrowserInfo "
                + "JOIN Browsers ON Browsers.browserAbbreviation = ProjectBrowserInfo.browserAbbreviation "
                + "WHERE ProjectBrowserInfo.projectAbbreviation = '" + projectAbbreviation + "' "
                + "AND ProjectBrowserInfo.showBrowserColumn = 'True'";

            List<string> visbleBrowserAbbreviations = new List<string>();

            DataTable showBrowserRow = DatabaseUtilities.GetDataTable(query);

            foreach (DataRow row in showBrowserRow.Rows)
            {
                String browserAbbreviation = row["browserAbbreviation"].ToString();

                visbleBrowserAbbreviations.Add(browserAbbreviation);
            }

            string[] stringArray = visbleBrowserAbbreviations.ToArray();
            return stringArray;
        }

        public static void AddSingleTestCaseScreenshot(int testCaseID, string projectAbbreviation, string imageURL, string description)
        {
            projectAbbreviation = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
            imageURL = DatabaseUtilities.MakeSQLSafe(imageURL);
            description = DatabaseUtilities.MakeSQLSafe(description);

            string mySelectQuery = "INSERT INTO [TestCaseScreenshots]"
            + " ([projectAbbreviation] "
            + " ,[testCaseId] "
            + " ,[imageURL] "
            + " ,description) "
            + " VALUES "
            + " ('" + projectAbbreviation
            + "', " + testCaseID
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


        public static int CountUsers()
        {
            int numberOfUsers = DatabaseUtilities.GetIntFromQuery("SELECT COUNT(*) FROM aspnet_Users");
            return numberOfUsers;
        }

        public static void MakeUserAdmin(Guid userGuid)
        {
            {
                Guid roleId = DatabaseUtilities.GetGuidFromQuery("SELECT RoleId FROM aspnet_Roles WHERE RoleName = 'Admin'");
                string query = "INSERT INTO aspnet_UsersInRoles (UserId, RoleId) VALUES (CAST('" + userGuid + "' AS UNIQUEIDENTIFIER), CAST('" + roleId + "' AS UNIQUEIDENTIFIER))";
                DatabaseUtilities.ExecuteQuery(query);
                
            }
        }


    }
}
