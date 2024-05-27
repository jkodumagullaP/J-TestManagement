using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class DatabaseUtilities
    {
        public static bool AreAnyAutomatedTestsRunning()
        {
            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "select * from TestResults where testType = 'Automated' and status = 'In Progress'";
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        bool wasTestCaseFound = dr.Read();

                        return wasTestCaseFound;
                    }
                }
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

        public static void UpdateTestCase(
            string projectAbbreviation,
            int testCaseId,
            string environment,
            string browserAbbreviation,
            string status,
            string reasonForStatus)
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
                        "UPDATE TestResults\n" +
                        "set status = '" + status + "' " +
                        ", reasonForStatus =  '" + reasonForStatus.Replace('\'', ' ') + "' " +
                        "where projectAbbreviation = '" + projectAbbreviation + "' " +
                        "and testCaseId =" + testCaseId + " " +
                        "and environment = '" + environment + "' " +
                        "and browserAbbreviation = '" + browserAbbreviation + "' " +
                        "and status =  '" + Utilities.Constants.ProcessStatus.IN_PROGRESS + "' "; // Only update the running row... TODO: make this smarter.

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
