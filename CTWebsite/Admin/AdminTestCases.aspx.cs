using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Data.OleDb;
using Utilities;
using CTInfrastructure;

namespace CTWebsite.Admin
{
    public partial class AdminTestCases : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                ddlProjects.DataBind();
                //Initialize gridviews and dropdown boxes.
                if (ddlProjects.Items.Count > 0)
                {
                    ddlProjects.Items.Insert(0, "Select a Project");
                    ddlProjects.Items[0].Value = "";
                    ddlProjects.SelectedIndex = 0;
                }

                if (HttpContext.Current.Session["CurrentProject"] != null)
                {
                    ddlProjects.SelectedValue = HttpContext.Current.Session["CurrentProject"].ToString();
                }
                else
                {
                    // Get logged in user's default project
                    string defaultProject = CTMethods.GetDefaultProject(HttpContext.Current.User.Identity);

                    if (defaultProject != null)
                    {
                        ddlProjects.SelectedValue = defaultProject;
                    }
                }
                gvDeletedTestCases.DataBind();
            }
            
            // Create a list of hyperlinks to all files in the Files folder.
            DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Files")); 
            int i = 0;
            foreach (FileInfo fi in di.GetFiles())
            {
                HyperLink HL = new HyperLink();
                HL.ID = "HyperLink" + i++;
                HL.Text = fi.Name;
                HL.NavigateUrl = "downloading.aspx?file=" + fi.Name;


                Panel pnlRightColumn = (Panel)AspUtilities.FindControlRecursive(this, "pnlRightColumn");

                pnlRightColumn.Controls.Add(HL);
                pnlRightColumn.Controls.Add(new LiteralControl("<br/><br/>"));
            }

        }


        public System.Data.DataTable xlsInsert(string path, string tabName, string firstColumnName) //  TestCases, projectAbbreviation
        {
            string strcon = string.Empty;
            if (Path.GetExtension(path).ToLower().Equals(".xls")// || Path.GetExtension(path).ToLower().Equals(".xlsx")
                )
            {
                strcon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                                + path +
                                //";Extended Properties=\"Excel 8.0;HDR=NO;\"";
                 ";Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1\"";
            }
            else
            {
                strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                              + path +
                              //";Extended Properties=\"Excel 12.0;HDR=NO;\"";
                 ";Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1\"";
            }
            string strSelect = "Select * from [" + tabName + "$]";
            DataTable exDT = new DataTable();
            using (OleDbConnection excelCon = new OleDbConnection(strcon))
            {
                try
                {
                    excelCon.Open();
                    using (OleDbDataAdapter exDA = 
            new OleDbDataAdapter(strSelect, excelCon))
                    {
                        exDA.Fill(exDT);
                    }
                }
                catch (OleDbException oledb)
                {
                    if (oledb.Message.Contains("TestResults$"))
                    {
                        throw new Exception("Excel file did not contain a sheet named \"TestResults\"");
                    }
                    else if (oledb.Message.Contains("TestCases$"))
                    {
                        throw new Exception("Excel file did not contain a sheet named \"TestCases\"");
                    }
                    else
                    {
                        throw new Exception(oledb.Message.ToString());
                    }
                }
                finally
                {
                    excelCon.Close();
                }

                exDT.AcceptChanges();  // refresh rows changes
                if (exDT.Rows.Count == 0)
                {
                    throw new Exception("File uploaded has no record found.");
                }
                return exDT;
            }
        }

        protected void btnUploadTc_Click(object sender, EventArgs e)
        {
            string errorMessage;

            try
            {
                if (xlsTcUpload.HasFile)
                {
                    string extension = Path.GetExtension(xlsTcUpload.FileName.ToString());
                    if (extension.Trim().ToLower() == ".xls" || extension.Trim().ToLower() == ".xlsx")
                    {
                        // Save excel file into Server sub dir
                        // to catch excel file downloading permission
                        string uploadedFile = Server.MapPath("~/Admin/UploadedTestCases/" + // UploadedTestResults
                            xlsTcUpload.FileName.ToString());

                        xlsTcUpload.SaveAs(uploadedFile);


                        DataTable results = xlsInsert(uploadedFile, "TestCases", "projectAbbreviation");


                        for (int i = results.Rows.Count - 1;i >= 0 ;i--)
                        {
                            // Check if first column is empty
                            // If empty then delete such record
                            if (results.Rows[i][0].ToString() == string.Empty
                                && results.Rows[i][1].ToString() == string.Empty
                                && results.Rows[i][5].ToString() == string.Empty
                                && results.Rows[i][6].ToString() == string.Empty
                                && results.Rows[i][7].ToString() == string.Empty)
                            {
                                //results.Rows[i].Delete();
                                results.Rows.RemoveAt(i);
                            }
                        }



                        errorMessage = ValidateTestCaseColumns(results.Rows[0]);

                        // Remove the header row
                        results.Rows.RemoveAt(0);

                        if (errorMessage != null)
                        {
                            this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n" + errorMessage;
                            return;
                        }

                        for (int index = 0; index < results.Rows.Count; index++)
                        {
                            DataRow row = results.Rows[index];

                            string rowErrorMessage = ValidateTestCase(row);

                            if (rowErrorMessage != null)
                            {
                                // Make the value non-null, and add an endline if there's already an error message.
                                errorMessage = errorMessage == null ? "" : errorMessage + "<br>\n";

                                // Add the error to the overall error message
                                errorMessage = errorMessage + "Validation error on row #" + (index + 2) + ": "+ rowErrorMessage;
                            }
                        }

                        if (errorMessage != null)
                        {
                            this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n" + errorMessage;
                            return;
                        }

                        for (int index = 0; index < results.Rows.Count; index++)
                        {
                            DataRow row = results.Rows[index];

                            errorMessage = LoadTestCase(row);

                            if (errorMessage != null)
                            {
                                this.lblMessage.Text = "FILE ***PARTIALLY*** LOADED, error on row #" + (index + 2) + ": " + errorMessage;
                                return;
                            }
                        }

                        /* Delete upload Excel
                        Uncomment the line below if we do not want to store the test case excel files
                        I reccommend keeping them for a backup. */

                        //File.Delete(uploadedFile);

                        // SUCCESS
                        this.lblMessage.Text = "File has successfully uploaded";
                    }
                    else
                    {
                        this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n Invalid file extension '" + extension + "'";
                    }
                }
                else
                {
                    this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n Please select file to upload.";
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n An error occurred: " + ex.Message;
            }

        }

        string LoadTestCase(DataRow row)
        {
            try
            {
                string projectAbbreviation = row[TestCaseFileColumns.projectAbbreviation].ToString().Replace("'", "");
                string groupTestAbbreviation = row[TestCaseFileColumns.groupTestAbbreviation].ToString().Replace("'", "");
                string release = row[TestCaseFileColumns.release].ToString().Replace("'", "");
                string sprint = row[TestCaseFileColumns.sprint].ToString().Replace("'", "");
                string testCaseDescription = row[TestCaseFileColumns.testCaseDescription].ToString().Replace("'", "");
                bool active = bool.Parse(row[TestCaseFileColumns.active].ToString().Replace("'", ""));
                bool testCaseOutdated = bool.Parse(row[TestCaseFileColumns.testCaseOutdated].ToString().Replace("'", ""));
                bool testScriptOutdated = bool.Parse(row[TestCaseFileColumns.testScriptOutdated].ToString().Replace("'", ""));
                string testCaseSteps = row[TestCaseFileColumns.testCaseSteps].ToString().Replace("'", "");
                string expectedResults = row[TestCaseFileColumns.expectedResults].ToString().Replace("'", "");
                string screenshots = row[TestCaseFileColumns.screenshots].ToString().Replace("'", "");
                string screenshotDescriptions = row[TestCaseFileColumns.screenshotDescriptions].ToString().Replace("'", "");
                string testCaseNotes = row[TestCaseFileColumns.testCaseNotes].ToString().Replace("'", "");
                string isThisAnUpdate = row[TestCaseFileColumns.isThisAnUpdate].ToString();
                string testCategory = row[TestCaseFileColumns.testCategory].ToString().Replace("'", "");
                string autoTestClass = row[TestCaseFileColumns.autoTestClass].ToString().Replace("'", "");
                string autoMetaDataTable = row[TestCaseFileColumns.autoMetaDataTable].ToString().Replace("'", "");
                string autoMetaDataRowString = row[TestCaseFileColumns.autoMetaDataRow].ToString().Replace("'", "");
                string automated = row[TestCaseFileColumns.automated].ToString().Replace("'", "");
                string reasonForNotAutomated = row[TestCaseFileColumns.reasonForNotAutomated].ToString().Replace("'", "");

                if (String.IsNullOrEmpty(isThisAnUpdate) || isThisAnUpdate.ToUpper() == "N" || isThisAnUpdate == "0")
                {
                    isThisAnUpdate = "NO";
                }

                if (isThisAnUpdate.ToUpper() == "Y" || isThisAnUpdate == "1")
                {
                    isThisAnUpdate = "YES";
                }

                int testCaseId;

                int nonNullableautoMetaDataRow;
                int? autoMetaDataRow;
                bool autoMetaDataRowParsedSuccessfully = Int32.TryParse(autoMetaDataRowString, out nonNullableautoMetaDataRow);
                if (autoMetaDataRowParsedSuccessfully)
                {
                    autoMetaDataRow = nonNullableautoMetaDataRow;
                }
                else
                {
                    autoMetaDataRow = null;
                }

                if (isThisAnUpdate.ToUpper() == "NO")
                {
                    testCaseId = CTMethods.InsertTestCase(
                    projectAbbreviation,
                    testCaseDescription,
                    active,
                    testCaseOutdated,
                    testScriptOutdated,
                    testCaseSteps,
                    expectedResults,
                    testCaseNotes,
                    AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                    autoTestClass,
                    autoMetaDataTable,
                    autoMetaDataRow,
                    testCategory,
                    automated,
                    reasonForNotAutomated
                    );
                }
                else
                {
                    testCaseId = Convert.ToInt32(row[TestCaseFileColumns.testCaseId].ToString());

                    CTMethods.UpdateTestCase(
                            AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                            projectAbbreviation,
                            testCaseId,
                            testCaseDescription,
                            active,
                            testCaseOutdated,
                            testScriptOutdated,
                            testCaseSteps,
                            expectedResults,
                            testCaseNotes,
                            autoTestClass,
                            autoMetaDataTable,
                            autoMetaDataRow,
                            testCategory,
                            automated,
                            reasonForNotAutomated);

                    CTMethods.UpdateTestCaseForAutomation(
                            AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                            projectAbbreviation,
                            testCaseId,
                            autoTestClass,
                            autoMetaDataTable,
                            autoMetaDataRow,
                            automated,
                            reasonForNotAutomated);
                }

                CTMethods.AddReleases(testCaseId, projectAbbreviation, release);
                CTMethods.AddGroups(testCaseId, projectAbbreviation, groupTestAbbreviation);
                CTMethods.AddSprints(testCaseId, projectAbbreviation, sprint);

                CTMethods.AddTestCaseScreenshots(testCaseId, projectAbbreviation, screenshots, screenshotDescriptions);

                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        string LoadTestResult(DataRow row)
        {
            try
            {
                string projectAbbreviation = row[TestResultFileColumns.projectAbbreviation].ToString().Replace("'", "");
                int testCaseId = Convert.ToInt32(row[TestResultFileColumns.testCaseId].ToString());
                string environment = row[TestResultFileColumns.environment].ToString().Replace("'", "");
                string browserAbbreviation = row[TestResultFileColumns.browserAbbreviation].ToString().Replace("'", "");
                string status = row[TestResultFileColumns.status].ToString().Replace("'", "");
                string reasonForStatus = row[TestResultFileColumns.reasonForStatus].ToString().Replace("'", "");
                string reasonForStatusDetailed = row[TestResultFileColumns.reasonForStatusDetailed].ToString().Replace("'", "");
                string stepsToReproduce = row[TestResultFileColumns.stepsToReproduce].ToString().Replace("'", "");
                string defectTicketNumber = row[TestResultFileColumns.defectTicketNumber].ToString().Replace("'", "");
                string screenshots = row[TestResultFileColumns.screenshots].ToString().Replace("'", "");
                string screenshotDescriptions = row[TestResultFileColumns.screenshotDescriptions].ToString().Replace("'", "");

                int testRunID = SeleniumTestBase.InsertTestResult(
                projectAbbreviation,
                testCaseId,
                environment,
                browserAbbreviation,
                status,
                reasonForStatus,
                reasonForStatusDetailed,
                stepsToReproduce,
                defectTicketNumber,
                AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name),
                "Manual"
                );

                SeleniumTestBase.AddTestResultScreenshots(testCaseId, projectAbbreviation, testRunID, screenshots, screenshotDescriptions);

                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        protected void btnUploadTr_Click(object sender, EventArgs e)
        {
            string errorMessage;

            try
            {
                if (xlsTrUpload.HasFile)
                {
                    string extension = Path.GetExtension(xlsTrUpload.FileName.ToString());
                    if (extension.Trim().ToLower() == ".xls" || extension.Trim().ToLower() == ".xlsx")
                    {
                        // Save excel file into Server sub dir
                        // to catch excel file downloading permission
                        string uploadedFile = Server.MapPath("~/Admin/UploadedTestResults/" + // UploadedTestResults
                            xlsTrUpload.FileName.ToString());

                        xlsTrUpload.SaveAs(uploadedFile);


                        DataTable results = xlsInsert(uploadedFile, "TestResults", "projectAbbreviation");

                        for (int i = results.Rows.Count - 1;i >= 0;i--)
                        {
                            // Check if first column is empty
                            // If empty then delete such record
                            if (results.Rows[i][0].ToString() == string.Empty
                                && results.Rows[i][1].ToString() == string.Empty
                                && results.Rows[i][2].ToString() == string.Empty
                                && results.Rows[i][3].ToString() == string.Empty
                                && results.Rows[i][4].ToString() == string.Empty)
                            {
                                //results.Rows[i].Delete();
                                results.Rows.RemoveAt(i);
                            }
                        }

                        errorMessage = ValidateTestResultColumns(results.Rows[0]);

                        // Remove the header rows
                        //results.Rows.RemoveAt(9);
                        //results.Rows.RemoveAt(8);
                        //results.Rows.RemoveAt(7);
                        //results.Rows.RemoveAt(6);
                        //results.Rows.RemoveAt(5);
                        //results.Rows.RemoveAt(4);
                        //results.Rows.RemoveAt(3);
                        //results.Rows.RemoveAt(2);
                        //results.Rows.RemoveAt(1);
                        results.Rows.RemoveAt(0);

                        if (errorMessage != null)
                        {
                            this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n " + errorMessage;
                            return;
                        }

                        for (int index = 0;index < results.Rows.Count;index++)
                        {
                            DataRow row = results.Rows[index];

                            string rowErrorMessage = ValidateTestResult(row);

                            if (rowErrorMessage != null)
                            {
                                // Make the value non-null, and add an endline if there's already an error message.
                                errorMessage = errorMessage == null ? "" : errorMessage + "<br>\n";

                                // Add the error to the overall error message
                                errorMessage = errorMessage + "Validation error on row #" + (index + 2) + ": " + rowErrorMessage;
                            }
                        }

                        if (errorMessage != null)
                        {
                            this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n " + errorMessage;
                            return;
                        }

                        for (int index = 0;index < results.Rows.Count;index++)
                        {
                            DataRow row = results.Rows[index];

                            errorMessage = LoadTestResult(row);

                            if (errorMessage != null)
                            {
                                this.lblMessage.Text = "FILE ***PARTIALLY*** LOADED, error on row #" + (index + 2) + ": " + errorMessage;
                                return;
                            }
                        }

                        /* Delete upload Excel
                        Uncomment the line below if we do not want to store the test result excel files
                        I reccommend keeping them for a backup. */

                        //File.Delete(uploadedFile);

                        // SUCCESS
                        this.lblMessage.Text = "File has successfully uploaded";
                    }
                    else
                    {
                        this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n Invalid file extension '" + extension + "'";
                    }
                }
                else
                {
                    this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n Please select file to upload.";
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = "<b>FILE NOT LOADED:</b><br>\n An error occurred: " + ex.Message;
            }

        }

        string ValidateTestCaseColumns(DataRow headerRow)
        {
            if (headerRow.ItemArray.Count() != 21)
            {
                return "Data Error: There should be 21 columns.";
            }
            else if (headerRow[TestCaseFileColumns.projectAbbreviation].ToString() != "projectAbbreviation")
            {
                return "Data Error: Column A should be named 'projectAbbreviation'";
            }
            else if (headerRow[TestCaseFileColumns.testCaseId].ToString() != "testCaseId")
            {
                return "Data Error: Column B should be named 'testCaseId'";
            }
            else if (headerRow[TestCaseFileColumns.groupTestAbbreviation].ToString() != "groupTestAbbreviation")
            {
                return "Data Error: Column C should be named 'groupTestAbbreviation'";
            }
            else if (headerRow[TestCaseFileColumns.release].ToString() != "release")
            {
                return "Data Error: Column D should be named 'release'";
            }
            else if (headerRow[TestCaseFileColumns.sprint].ToString() != "sprint")
            {
                return "Data Error: Column E should be named 'sprint'";
            }
            else if (headerRow[TestCaseFileColumns.testCaseDescription].ToString() != "testCaseDescription")
            {
                return "Data Error: Column F should be named 'testCaseDescription'";
            }
            else if (headerRow[TestCaseFileColumns.active].ToString() != "active")
            {
                return "Data Error: Column G should be named 'active'";
            }
            else if (headerRow[TestCaseFileColumns.testCaseOutdated].ToString() != "testCaseOutdated")
            {
                return "Data Error: Column H should be named 'testCaseOutdated'";
            }
            else if (headerRow[TestCaseFileColumns.testScriptOutdated].ToString() != "testScriptOutdated")
            {
                return "Data Error: Column I should be named 'testScriptOutdated'";
            }
            else if (headerRow[TestCaseFileColumns.testCaseSteps].ToString() != "testCaseSteps")
            {
                return "Data Error: Column J should be named 'testCaseSteps'";
            }
            else if (headerRow[TestCaseFileColumns.expectedResults].ToString() != "expectedResults")
            {
                return "Data Error: Column K should be named 'expectedResults'";
            }
            else if (headerRow[TestCaseFileColumns.screenshots].ToString() != "screenshots")
            {
                return "Data Error: Column L should be named 'screenshots'";
            }
            else if (headerRow[TestCaseFileColumns.screenshotDescriptions].ToString() != "screenshotDescriptions")
            {
                return "Data Error: Column M should be named 'screenshotDescriptions'";
            }
            else if (headerRow[TestCaseFileColumns.testCaseNotes].ToString() != "testCaseNotes")
            {
                return "Data Error: Column N should be named 'testCaseNotes'";
            }
            else if (headerRow[TestCaseFileColumns.isThisAnUpdate].ToString() != "isThisAnUpdate?")
            {
                return "Data Error: Column O should be named 'isThisAnUpdate?'";
            }
            else if (headerRow[TestCaseFileColumns.testCategory].ToString() != "testCategory")
            {
                return "Data Error: Column P should be named 'testCategory'";
            }
            else if (headerRow[TestCaseFileColumns.autoTestClass].ToString() != "autoTestClass")
            {
                return "Data Error: Column Q should be named 'autoTestClass'";
            }
            else if (headerRow[TestCaseFileColumns.autoMetaDataTable].ToString() != "autoMetaDataTable")
            {
                return "Data Error: Column R should be named 'autoMetaDataTable'";
            }
            else if (headerRow[TestCaseFileColumns.autoMetaDataRow].ToString() != "autoMetaDataRow")
            {
                return "Data Error: Column S should be named 'autoMetaDataRow'";
            }
            else if (headerRow[TestCaseFileColumns.automated].ToString() != "automated")
            {
                return "Data Error: Column T should be named 'automated'";
            }
            else if (headerRow[TestCaseFileColumns.reasonForNotAutomated].ToString() != "reasonForNotAutomated")
            {
                return "Data Error: Column U should be named 'reasonForNotAutomated'";
            }

            return null;
        }

        string ValidateTestCase(DataRow row)
        {
            string projectAbbreviation = row[TestCaseFileColumns.projectAbbreviation].ToString().Replace("'", "");
            if (String.IsNullOrEmpty(projectAbbreviation))
            {
                return "Project Abbreviation is required";
            }
            
            string groupTestAbbreviation = row[TestCaseFileColumns.groupTestAbbreviation].ToString().Replace("'", "");

            string error = CTMethods.ValidateGroups(projectAbbreviation, groupTestAbbreviation);
            if (error != null)
                return error;


            string release = row[TestCaseFileColumns.release].ToString().Replace("'", "");

            error = CTMethods.ValidateReleases(projectAbbreviation, release);
            if (error != null)
                return error;


            string testCaseId = row[TestCaseFileColumns.testCaseId].ToString();
            string isThisAnUpdate = row[TestCaseFileColumns.isThisAnUpdate].ToString();

            error = CTMethods.ValidateTestCaseID(projectAbbreviation, testCaseId, isThisAnUpdate);
            if (error != null)
                return error;


            string sprint = row[TestCaseFileColumns.sprint].ToString().Replace("'", "");

            error = CTMethods.ValidateSprints(projectAbbreviation, sprint);
            if (error != null)
                return error;


            string screenshots = row[TestCaseFileColumns.screenshots].ToString().Replace("'", "");
            string screenshotDescriptions = row[TestCaseFileColumns.screenshotDescriptions].ToString().Replace("'", "");

            if (!String.IsNullOrEmpty(screenshots))
            {
                List<string> splitScreenshots = screenshots.Split(new[] { '|' }).ToList();
                List<string> splitScreenshotDescriptions = screenshotDescriptions.Split(new[] { '|' }).ToList();

                if (splitScreenshotDescriptions.Count != splitScreenshots.Count && !String.IsNullOrEmpty(screenshotDescriptions))
                {
                    return "The number of screenshots must match the number of screenshot descriptions";
                }

                foreach (string imageURL in splitScreenshots)
                {
                    if (!imageURL.ToLower().EndsWith(".jpg")
                     && !imageURL.ToLower().EndsWith(".gif")
                     && !imageURL.ToLower().EndsWith(".png"))
                    {
                        return "Screenshot URLs must end with .jpg, .png, or .gif";
                    }
                }
            }


            if (String.IsNullOrEmpty(row[TestCaseFileColumns.testCaseDescription].ToString().Replace("'", "").Trim()))
            {
                return "Description is required";
            }

            if (String.IsNullOrEmpty(row[TestCaseFileColumns.testCaseSteps].ToString().Replace("'", "").Trim()))
            {
                return "Test Case Steps are required";
            }

            if (String.IsNullOrEmpty(row[TestCaseFileColumns.expectedResults].ToString().Replace("'", "").Trim()))
            {
                return "ExpectedResults is required";
            }

            if (String.IsNullOrEmpty(row[TestCaseFileColumns.testCategory].ToString().Replace("'", "").Trim()))
            {
                return "Test Category is required";
            }

            return null;
        }

        string ValidateTestResultColumns(DataRow headerRow)
        {
            							

            if (headerRow.ItemArray.Count() != 11)
            {
                return "Data Error: There should be 11 columns.";
            }
            else if (headerRow[TestResultFileColumns.projectAbbreviation].ToString() != "projectAbbreviation")
            {
                return "Data Error: Column A should be named 'projectAbbreviation'";
            }
            else if (headerRow[TestResultFileColumns.testCaseId].ToString() != "testCaseId")
            {
                return "Data Error: Column B should be named 'testCaseId'";
            }
            else if (headerRow[TestResultFileColumns.environment].ToString() != "environment")
            {
                return "Data Error: Column C should be named 'environment'";
            }
            else if (headerRow[TestResultFileColumns.browserAbbreviation].ToString() != "browserAbbreviation")
            {
                return "Data Error: Column D should be named 'browserAbbreviation'";
            }
            else if (headerRow[TestResultFileColumns.status].ToString() != "status")
            {
                return "Data Error: Column E should be named 'status'";
            }
            else if (headerRow[TestResultFileColumns.reasonForStatus].ToString() != "reasonForStatus")
            {
                return "Data Error: Column F should be named 'reasonForStatus'";
            }
            else if (headerRow[TestResultFileColumns.reasonForStatusDetailed].ToString() != "reasonForStatusDetailed")
            {
                return "Data Error: Column G should be named 'reasonForStatusDetailed'";
            }
            else if (headerRow[TestResultFileColumns.stepsToReproduce].ToString() != "stepsToReproduce")
            {
                return "Data Error: Column H should be named 'stepsToReproduce'";
            }
            else if (headerRow[TestResultFileColumns.defectTicketNumber].ToString() != "defectTicketNumber")
            {
                return "Data Error: Column I should be named 'defectTicketNumber'";
            }
            else if (headerRow[TestResultFileColumns.screenshots].ToString() != "screenshots")
            {
                return "Data Error: Column J should be named 'screenshots'";
            }
            else if (headerRow[TestResultFileColumns.screenshotDescriptions].ToString() != "screenshotDescriptions")
            {
                return "Data Error: Column J should be named 'screenshotDescriptions'";
            }

            return null;
        }

        string ValidateTestResult(DataRow row)
        {
            string projectAbbreviation = row[TestResultFileColumns.projectAbbreviation].ToString().Replace("'", "");
            string testCaseId = row[TestResultFileColumns.testCaseId].ToString();

            if (String.IsNullOrEmpty(row[TestResultFileColumns.projectAbbreviation].ToString().Replace("'", "").Trim()))
            {
                return "Project Abbreviation is required";
            }

            if (String.IsNullOrEmpty(row[TestResultFileColumns.testCaseId].ToString().Replace("'", "").Trim()))
            {
                return "Test Case ID is required";
            }

            Dictionary<string, string> dictionary = CTMethods.GetTestCaseStatuses(projectAbbreviation, Convert.ToInt32(testCaseId));
            //Parse the dictionary list
            bool active = Convert.ToBoolean(dictionary["active"]);
            bool tcOutdated = Convert.ToBoolean(dictionary["testCaseOutdated"]);
            bool tsOutdated = Convert.ToBoolean(dictionary["testScriptOutdated"]);

            if (active == false)
            {
                return "Test Case #" + testCaseId + " is inactive. You can not enter results for an inactive test case.";
            }

            if (tcOutdated == true)
            {
                return "Test Case #" + testCaseId + " is outdated. You can not enter results for an outdated test case. Please update the test case and uncheck the outdated checkbox in the test case details. Then retry your upload.";
            }

            if (String.IsNullOrEmpty(row[TestResultFileColumns.environment].ToString().Replace("'", "").Trim()))
            {
                return "Environment is missing";
            }


            if (String.IsNullOrEmpty(row[TestResultFileColumns.environment].ToString().Replace("'", "").Trim()))
            {
                return "Environment is required";
            }

            if (String.IsNullOrEmpty(row[TestResultFileColumns.browserAbbreviation].ToString().Replace("'", "").Trim()))
            {
                return "Browser Abbreviation is required";
            }

            if (String.IsNullOrEmpty(row[TestResultFileColumns.status].ToString().Replace("'", "").Trim()))
            {
                return "Status is required";
            }

            string error = CTMethods.ValidateTestCaseIDForResults(projectAbbreviation, testCaseId);
            if (error != null)
                return error;

            string environment = row[TestResultFileColumns.environment].ToString().Replace("'", "");

            error = CTMethods.ValidateProjectEnvironmentExists(projectAbbreviation, environment, true);
            if (error != null)
                return error;

            string browserAbbreviation = row[TestResultFileColumns.browserAbbreviation].ToString().Replace("'", "");

            error = CTMethods.ValidateBrowserAbbreviation(browserAbbreviation);
            if (error != null)
                return error;

            string status = row[TestResultFileColumns.status].ToString().Replace("'", "");

            error = CTMethods.ValidateStatus(status);
            if (error != null)
                return error;

            string screenshots = row[TestResultFileColumns.screenshots].ToString();
            string screenshotDescriptions = row[TestResultFileColumns.screenshotDescriptions].ToString();

            if (!String.IsNullOrEmpty(screenshots))
            {
                List<string> splitScreenshots = screenshots.Split(new[] { '|' }).ToList();
                List<string> splitScreenshotDescriptions = screenshotDescriptions.Split(new[] { '|' }).ToList();

                if (splitScreenshotDescriptions.Count != splitScreenshots.Count && !String.IsNullOrEmpty(screenshotDescriptions))
                {
                    return "The number of screenshots must match the number of screenshot descriptions";
                }
            }

            return null;
        }

        protected void gvDeletedTestCases_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gvDeletedTestCases_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            /*  
             * CommandPieces[0] = projectAbbreviation
             * CommandPieces[1] = testCaseID
            */
            if (e.CommandName == "Restore")
            {
                string commandArgument = (e.CommandArgument ?? "").ToString();
                string[] commandPieces = commandArgument.Split(new[] { '|' });

                string projectAbbreviation = commandPieces[0];
                int testCaseID = Convert.ToInt32(commandPieces[1]);

                CTMethods.RestoreTestCase(AspUtilities.RemovePrefixFromUserName(HttpContext.Current.User.Identity.Name), projectAbbreviation, testCaseID);
                gvDeletedTestCases.DataBind();
            }
        }

        protected void ddlProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentProject"] = ((DropDownList)sender).SelectedValue;
        }

        protected void btnInstallAutomatedTestCase_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/FunctionalTesting/InstallAutomatedTestCase.aspx");
        }

    }
} 