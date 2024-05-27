/*!
 * Crystal Test Example_Login.cs
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
using CTInfrastructure;
using System;
using System.Collections.Generic;
using System.Threading;
using Utilities;
using OpenQA.Selenium.Remote;

namespace Example_TestScripts
{
    public class Example_Login : SeleniumTestBase
    {
        /// <summary>
        /// This is the method called when a user clicks the Test button
        /// All it does is call the setup method with standalone set to True
        /// </summary>
        protected override void TestSpecific_RunSeleniumTest()
        {
            Login(driver, projectAbbreviation, autoMetaDataTable, autoMetaDataRow.Value, true, environment);
        }


        /// <summary>
        /// This is the method called by other test scripts
        /// All it does is call the setup method with standalone set to False
        /// </summary>
        public static void Login(IWebDriver driver, string projectAbbreviation, string autoMetaDataTable, int autoMetaDataRow)
        {
            Login(driver, projectAbbreviation, autoMetaDataTable, autoMetaDataRow, false, null);
        }


        /// <summary>
        ///This is the setup script which automatically grabs the test data from the database and passes it to the test script method
        /// </summary>
        private static void Login(IWebDriver driver, string projectAbbreviation, string autoMetaDataTable, int autoMetaDataRow, bool isStandaloneTest, string environmentForStandaloneTest)
        {

            // Get dictionary of metadata table <fieldNames, Values>
            Dictionary<string, string> dictionary = DatabaseUtilities.GetTableDictionary(autoMetaDataTable, "testId", autoMetaDataRow);

            if (isStandaloneTest)
            {
                string baseUrl = CTTestGridMethods.GetBaseUrlForEnvironment(environmentForStandaloneTest, projectAbbreviation);

                CTTestGridMethods.SetUpSeleniumDriver(driver, baseUrl);
                Thread.Sleep(2000);
            }

            Login(driver, dictionary);
        }

        /// <summary>
        ///This is the test script method.
        ///At this point the browser window is open and ready to test
        ///This is where we code our test steps
        /// </summary>
        public static void Login(IWebDriver driver, Dictionary<string, string> dictionary)
        {
            //Parse the dictionary list
            string email = dictionary["Email"];
            string password = dictionary["Password"];

            //Define the page elements that will be used
            string emailField = "Email";
            string passwordField = "Passwd";
            string signInButton = "signIn";
            string logOutButton = "gb_71";
            string userIcon = "/html/body/div/div/div/div/div/div[2]/div[5]/div/a";
            string editAccountListButton = "edit-account-list";
            string accountChooser = "accountchooser-title";
            string selectUser = "choose-account-0";
            string objectToLookForIfSignedIn = "nav-security";

            
            //Check to see if password field exists 
            if (SeleniumSharedMethods.ThisObjectExists(driver, By.Id(passwordField)))
            {

                //Check to see if email field exists                
                if (SeleniumSharedMethods.ThisObjectExists(driver, By.Id(emailField)))
                {
                    //If we are here, both the email and password fields are visible.
                    //Fill them in
                    try
                    {
                        SeleniumSharedMethods.SendKeysReliably(driver, emailField, By.Id(emailField), email, true, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to fill in Email field", ex);
                    }


                    try
                    {
                        SeleniumSharedMethods.SendKeysReliably(driver, passwordField, By.Id(passwordField), password, true, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to fill in Password field", ex);
                    }

                    //Click the sign in button
                    try
                    {
                        SeleniumSharedMethods.ClickSimple(driver, signInButton, By.Id(signInButton));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("There was an issue clicking the log in button after populating the email and password fields.", ex);
                    }

                }
                //If the password field displays but the email field does not, this means the user's account is
                //already cached. Just fill in the password field.
                else
                {
                    try
                    {
                        SeleniumSharedMethods.SendKeysReliably(driver, passwordField, By.Id(passwordField), password, true, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to fill in Password field", ex);
                    }

                    try
                    {
                        SeleniumSharedMethods.ClickSimple(driver, signInButton, By.Id(signInButton));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("There was an issue clicking the log in button after filling in only the password field. The username was pre-populated. Requires manual research.", ex);
                    }

                }
            }
            //If neither the email or password field exists, 
            //The page failed load
            //The user is already logged in
            //Or the Account Chooser page is displayed.
            else
            {
                //If the user is not already logged in
                if (!SeleniumSharedMethods.ThisObjectExists(driver, By.Id(objectToLookForIfSignedIn)))
                {
                    //If the account chooser is displayed
                    if (SeleniumSharedMethods.ThisObjectExists(driver, By.Id(accountChooser)))
                    {
                        //Attempt to remove user from list
                        SeleniumSharedMethods.ClickSimple(driver, editAccountListButton, By.Id(editAccountListButton));
                        SeleniumSharedMethods.ClickSimple(driver, selectUser, By.Id(selectUser));
                        SeleniumSharedMethods.ClickSimple(driver, editAccountListButton, By.Id(editAccountListButton));

                        //Try logging in again.
                        //Both the email and password fields should be visible.
                        try
                        {
                            SeleniumSharedMethods.SendKeysReliably(driver, emailField, By.Id(emailField), email, true, true);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Unable to fill in Email field", ex);
                        }


                        try
                        {
                            SeleniumSharedMethods.SendKeysReliably(driver, passwordField, By.Id(passwordField), password, true, true);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Unable to fill in Password field", ex);
                        }

                        //Click the sign in button
                        try
                        {
                            SeleniumSharedMethods.ClickSimple(driver, signInButton, By.Id(signInButton));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("There was an issue clicking the log in button after populating the email and password fields.", ex);
                        }
                    }
                    else
                    {
                        throw new Exception("The login page failed to load. The user was not logged in, nor was the Account Chooser or Cached Login page visible. This issue requires manual research.");
                    }
                }
                else
                {
                    //Click the User Icon
                    SeleniumSharedMethods.ClickUntil(driver, "User Icon", By.XPath(userIcon), logOutButton, By.Id(logOutButton), true);
                    
                    
                    //Click the Logout button
                    SeleniumSharedMethods.ClickSimple(driver, logOutButton, By.Id(logOutButton));

                    //Give it time to logout and for the login form to appear.
                    Thread.Sleep(5000);

                    //User should be logged out now with the 
                    try
                    {
                        SeleniumSharedMethods.SendKeysReliably(driver, passwordField, By.Id(passwordField), password, true, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to fill in Password field after logging out the user", ex);
                    }

                    try
                    {
                        SeleniumSharedMethods.ClickSimple(driver, signInButton, By.Id(signInButton));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("There was an issue clicking the log in button after filling in only the password field. The username was pre-populated. Requires manual research.", ex);
                    }
                }
            }
        }
    }
}
