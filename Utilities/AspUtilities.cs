/*!
 * Crystal Test AspUtilities.cs
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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilities
{
    public class AspUtilities
    {
        /// <summary>
        /// Returns the first control found with the given ID
        /// </summary>
        /// <param name="rootControl">The parent control of the control that is being searched for</param>
        /// <param name="controlID">Control ID of the control being searched for.</param>
        /// <returns></returns>
        public static Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID)
                return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn = FindControlRecursive(controlToSearch, controlID);

                if (controlToReturn != null)
                    return controlToReturn;
            }

            return null;
        }






        /// <summary>
        /// Returns all controls found with the given ID
        /// </summary>
        /// <typeparam name="T">control</typeparam>
        /// <param name="rootControl">The parent control of the control that is being searched for</param>
        /// <param name="controlID">Control ID of the control being searched for</param>
        /// <returns>List</returns>
        public static List<T> FindMultipleControlsRecursive<T>(Control rootControl, string controlID) where T : Control
        {
            List<T> controls = new List<T>();

            if (rootControl.ID == controlID)
            {
                controls.Add((T)rootControl);
                return controls;
            }

            foreach (Control controlToSearch in rootControl.Controls)
            {
                controls.AddRange(FindMultipleControlsRecursive<T>(controlToSearch, controlID));
            }

            return controls;
        }


        /// <summary>
        /// Returns the Cell (DataControlFieldCell) of a Gridview row whose cell name is the same as a database field name.
        /// </summary>
        /// <param name="row">Gridview row</param>
        /// <param name="cellName">Database field name to search for</param>
        /// <returns>DataControlFieldCell</returns>
        static public DataControlFieldCell GetCellByName(GridViewRow row, String cellName)
        {
            foreach (DataControlFieldCell Cell in row.Cells)
            {
                if (Cell.ContainingField.ToString() == cellName)
                    return Cell;
            }
            return null;
        }

        /// <summary>
        /// Searches through columns of a gridview and returns the index of a column where the header text matches the value of the variable columnText.
        /// </summary>
        /// <param name="aGridView">The ID of the gridview in which to search.</param>
        /// <param name="ColumnText">The header text in which to search.</param>
        /// <returns>int</returns>

        static public int GetColumnIndexByHeaderText(GridView aGridView, String columnText)
        {
            TableCell Cell;
            for (int Index = 0; Index < aGridView.HeaderRow.Cells.Count; Index++)
            {
                Cell = aGridView.HeaderRow.Cells[Index];
                if (Cell.Text.ToString() == columnText)
                    return Index;
            }
            return -1;
        }

        /// <summary>
        /// Get Column Index by Database Field Name
        /// </summary>
        /// <param name="aGridView">Id of gridview</param>
        /// <param name="ColumnText">Database field name</param>
        /// <returns>int</returns>
        static public int GetColumnIndexByDBName(GridView aGridView, String ColumnText)
        {
            System.Web.UI.WebControls.BoundField DataColumn;

            for (int Index = 0; Index < aGridView.Columns.Count; Index++)
            {
                DataColumn = aGridView.Columns[Index] as System.Web.UI.WebControls.BoundField;

                if (DataColumn != null)
                {
                    if (DataColumn.DataField == ColumnText)
                        return Index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the username after trimming the domain prefix from the front of it.
        /// </summary>
        /// <param name="user">username</param>
        /// <returns>string</returns>
        public static string RemovePrefixFromUserName(string user)
        {
            user = DatabaseUtilities.MakeSQLSafe(user);

            if (user == null)
            {
                return null;
            }

            int substringIndex = user.IndexOf(@"\");

            if (substringIndex > -1)
            {
                return user.Substring(substringIndex + 1);
            }
            else
            {
                return user;
            }
        }

        /// <summary>
        /// Returns the first and last name of a user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>string</returns>
        public static string GetUserFullName(string userName)
        {

            string mySelectQuery = "SELECT userFirstName + ' ' + userLastName FROM aspnet_Users JOIN UserProfiles ON aspnet_Users.UserId = UserProfiles.UserId WHERE UserName='" + userName + "'";

            if (userName != "")
            {
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

                                return dr.GetString(0);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the GUID of a user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>string</returns>
        public static string GetUserGuid(string userName)
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
                        "Select UserId from aspnet_Users "
                        + " where UserName = '" + userName + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // If any rows matched, it passed.
                        if (dr.HasRows)
                        {
                            dr.Read();
                            return dr.GetGuid(0).ToString();
                        }
                    }
                }
            }

            return null;
        }

    }
}

