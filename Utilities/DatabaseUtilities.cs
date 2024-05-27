/*!
 * Crystal Test DatabaseUtilities.cs
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
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Web.UI.WebControls;

namespace Utilities
{
    public static class DatabaseUtilities
    {

        /// <summary>
        /// Protects from SQL Injection
        /// </summary>
        /// <param name="text">Enter a string of text</param>
        /// <returns>Returns the safe string of text to enter into SQL</returns>
        public static string MakeSQLSafe(string text)
        {
            if (text == null)
            {
                return null;
            }

            text = text.Replace("'", "''").Replace("''''", "''");

            return text;
        }

        
        /// <summary>
        /// Executes a query and returs the data table
        /// </summary>
        /// <param name="query">Pass in an SQL query</param>
        /// <returns>DataTable</returns>
        public static DataTable GetDataTable(string query)
        {
            //query = DatabaseUtilities.MakeSQLSafe(query);

            DataTable returnValue = new DataTable();

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
                        //dr.Read()
                        returnValue.Load(dr);
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Executes a query and returs an integer
        /// </summary>
        /// <param name="query">Pass in an SQL query</param>
        /// <returns>int</returns>
        public static int GetIntFromQuery(string query)
        {
            //query = DatabaseUtilities.MakeSQLSafe(query);

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

                        return dr.GetInt32(0);
                    }
                }
            }
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query">Pass in an SQL query</param>
        /// <returns></returns>
        public static void ExecuteQuery(string query)
        {
            //query = DatabaseUtilities.MakeSQLSafe(query);

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

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes a query and returs a string
        /// </summary>
        /// <param name="query">Pass in an SQL query</param>
        /// <returns>string</returns>
        public static string GetTextFromQuery(string query)
        {
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

                        return dr.GetString(0);
                    }
                }
            }
        }


        public static void ExecuteNoParameterStoredProcedure(string procedureName)
        {
            using (var conn = new SqlConnection(Constants.QAAConnectionString))
            using (var command = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                conn.Open();
                command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Executes a query and returs a string
        /// </summary>
        /// <param name="query">Pass in an SQL query</param>
        /// <returns>string</returns>
        public static Guid GetGuidFromQuery(string query)
        {
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = query;

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();

                        return dr.GetGuid(0);
                    }
                }
            }
        }

        /// <summary>
        /// Executes a query and returs a list of integers
        /// </summary>
        /// <param name="query">Pass in an SQL query</param>
        /// <returns>List[]</returns>
        public static List<int> GetIntListFromQuery(string query)
        {

            List<int> returnList = new List<int>();

            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = query;

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            returnList.Add(dr.GetInt32(0));
                        }
                    }
                }
            }

            return returnList;
        }
        /// <summary>
        /// Executes a query and returs a list of strings
        /// </summary>
        /// <param name="query">Pass in an SQL query</param>
        /// <returns>List[]</returns>
        public static List<string> GetStringListFromQuery(string query)
        {

            List<string> returnList = new List<string>();

            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = query;

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            returnList.Add(dr.GetString(0));
                        }
                    }
                }
            }

            return returnList;
        }


        public static Dictionary<string, string> GetTableDictionary(string projectAbbreviation, int testCaseId, string query)
        {
            DataTable dt = GetDataTable(query);

            //Put all the field names and their values into a dictionary
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (DataColumn column in dt.Columns)
            {
                //Get the name of the column
                string columnName = column.ColumnName;

                foreach (DataRow row in dt.Rows)
                {
                    //Get value for that field
                    String columnValue = row[columnName].ToString();

                    //Add field name and value to dictionary
                    dictionary.Add(columnName, columnValue);
                }
            }
            return dictionary;
        }
        
        public static Dictionary<string, string> GetTableDictionary(string tableName, string fieldNameForLookup, int rowIndex)
        {
            string query = "select * from " + tableName + " where " + fieldNameForLookup + " = " + rowIndex;
            DataTable dt = GetDataTable(query);

            //Put all the field names and their values into a dictionary
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (DataColumn column in dt.Columns)
            {
                //Get the name of the column
                string columnName = column.ColumnName;

                foreach (DataRow row in dt.Rows)
                {
                    //Get value for that field
                    String columnValue = row[columnName].ToString();

                    //Add field name and value to dictionary
                    dictionary.Add(columnName, columnValue);
                }
            }
            return dictionary;
        }

        public static Dictionary<string, string> GetTableDictionary(string projectAbbreviation, string tableName, string fieldNameForLookup, int rowIndex)
        {
            string query = "select * from " + tableName + " where projectAbbreviation = '" + projectAbbreviation + "' AND " + fieldNameForLookup + " = " + rowIndex;
            DataTable dt = GetDataTable(query);

            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (DataColumn column in dt.Columns)
            {
                string columnName = column.ColumnName;

                foreach (DataRow row in dt.Rows)
                {
                    String columnValue = row[columnName].ToString();

                    dictionary.Add(columnName, columnValue);
                }
            }
            return dictionary;
        }

        public static DateTime DateParse(string date)
        {
            date = date.Trim();
            if (!string.IsNullOrEmpty(date))
                return DateTime.Parse(date, new System.Globalization.CultureInfo("en-GB"));
            return new DateTime();
        }
    }
}
