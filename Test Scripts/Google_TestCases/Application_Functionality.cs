using OpenQA.Selenium;
using CTInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Utilities;

namespace ApplicationName_TestScripts
{
    public class ProjectName_Functionality : SeleniumTestBase
    {
        /// <summary>
        /// Method #1 - Standalone Method
        /// This is the method called when a user clicks the Test button
        /// All it does is call the setup method with standalone set to True
        /// The only thing that needs changed here is the method name of the method being called. (Functionality)
        /// </summary>
        protected override void TestSpecific_RunSeleniumTest()
        {
            Functionality(driver, projectAbbreviation, autoMetaDataTable, autoMetaDataRow.Value, true, environment);

        }


        /// <summary>
        /// Method #2
        /// This is the method called by other test scripts
        /// All it does is call the setup method with standalone set to False
        /// The only thing that needs changed here is the name of this method and the name of the method being called. (Functionality)
        /// </summary>
        public static void Functionality(IWebDriver driver, string projectAbbreviation, string autoMetaDataTable, int autoMetaDataRow)
        {
            Functionality(driver, projectAbbreviation, autoMetaDataTable, autoMetaDataRow, false, null);
        }



        /// <summary>
        /// Method #3
        /// This is the setup script which automatically grabs the test data from the database and passes it to the test script method in a dictionary
        /// The only thing that needs changed here is the name of this method and the name of the method being called. (Functionality)
        /// </summary>
        private static void Functionality(IWebDriver driver, string projectAbbreviation, string autoMetaDataTable, int autoMetaDataRow, bool isStandaloneTest, string environmentForStandaloneTest)
        {

            // Get dictionary of metadata table <fieldNames, Values>
            Dictionary<string, string> dictionary = DatabaseUtilities.GetTableDictionary(autoMetaDataTable, "testId", autoMetaDataRow);
            
                if (isStandaloneTest)
                {
                    string baseUrl = CTTestGridMethods.GetBaseUrlForEnvironment(environmentForStandaloneTest, projectAbbreviation);

                    CTTestGridMethods.SetUpSeleniumDriver(driver, baseUrl);
                    Thread.Sleep(2000);
                }

                Functionality(driver, dictionary);
            }


        /// <summary>
        /// Method #4
        /// This is the test script method.
        /// At this point the browser window is open and ready to test
        /// This is where we code our test steps
        /// </summary>
        public static void Functionality(IWebDriver driver, Dictionary<string, string> dictionary)
        {
            // Parse the dictionary list
            string databaseFieldName1 = dictionary["databaseFieldName1"];
            int databaseFieldName2 = Convert.ToInt32(dictionary["databaseFieldName2"]);
            bool databaseFieldName3 = Convert.ToBoolean(dictionary["databaseFieldName3"]);


            // Define the page elements that will be interacted with
            // The following is just an example. Exchange these with your elements
            // Alternatively, these can be defined in a database table.
            string Textbox = "TextboxID";
            string DropdownList = "DropdownListID";
            string Button = "ButtonID";

            //Example Test Step
            if (SeleniumSharedMethods.ThisObjectExists(driver, By.Id(Textbox)))
            {
                SeleniumSharedMethods.SelectByTextSimple(driver, DropdownList, By.Id(DropdownList), "MySelectionChoice");
                SeleniumSharedMethods.ClickSimple(driver, Button, By.Id(Button));
            }
            else
            {
                throw new Exception("The textbox with ID of " + Textbox + " was not visible on the page."); //This is the custom error message that will show up in the Reason for Status field in the test results
            }
        }
    }
}

