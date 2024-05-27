/*!
 * Crystal Test SeleniumSharedMethods.cs
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
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Management;
using System.ServiceProcess;
using System.Threading;
using System.Web.Script.Serialization;
using Utilities;

namespace CTInfrastructure
{
    public static class SeleniumSharedMethods
    {
        /// <summary>
        /// This method accepts a popup alert.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        public static void AcceptAlert(IWebDriver driver)
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// This method dismisses a popup alert
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        public static void DismissAlert(IWebDriver driver)
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Dismiss();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// This method gets you past a certificate warning page in IE9
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        public static void CertificateFix(IWebDriver driver)
        {
            if (((ICustomRemoteDriver)driver).GetBrowserName() == "internet explorer")
            {
                Thread.Sleep(5000);
                driver.Navigate().GoToUrl("javascript:var target = document.getElementById('overridelink'); if (target != null) {overridelink.click();}");
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// This method waits for an object to appear, clicks it, then waits for it to disappear, such as when a button closes a modal or directs you to another page.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object you wish to click. 
        /// Example: "ddlProject" This is used in the result output if the instruction fails.
        /// </param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <param name="initialWaitSeconds"></param>
        /// <param name="subsequentWaitSeconds"></param>
        public static void ClickClosingButtonReliably(IWebDriver driver, string id, By by, int initialWaitSeconds = 15, int subsequentWaitSeconds = 1)
        {
            try
            {
                // Give the element the chance to materialize if it isn't already there
                int attempt = 1;
                while (attempt < 20 && !ThisObjectExists(driver, by))
                {
                    Thread.Sleep(500);
                    attempt++;
                }

                // If it's still not there, throw an exception
                if (!ThisObjectExists(driver, by))
                {
                    throw new Exception("Element, " + id + ", is not displayed?");
                }

                //IWebElement element = driver.FindElement(by);

                // Click it and give the button the chance to disappear
                ClickSimple(driver, id, by, returnIfExceptionThrown: true);
                attempt = 1;
                while (attempt < initialWaitSeconds && ThisObjectExists(driver, by))
                {
                    Thread.Sleep(1000);
                    attempt++;
                }

                // If it doesn't disappear, click it again until it does
                attempt = 1;
                while (attempt < 10 && ThisObjectExists(driver, by))
                {
                    ClickSimple(driver, id, by, returnIfExceptionThrown: true);
                    Thread.Sleep(subsequentWaitSeconds * 1000);
                    attempt++;
                }

                // If it's still hasn't disappeared, throw an exception
                if (attempt == 10 && ThisObjectExists(driver, by))
                {
                    throw new Exception("This button, " + id + ", refused to disappear after clicked. If it wasn't supposed to disappear, this was the wrong method to call for it.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error in ClickClosingButtonReliably: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// This method will attempt to click a button and keep attempting until the maxAttempts # has been reached or the button was clicked.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object you wish to click. 
        /// Example: "ddlProject" This is used in the result output if the instruction fails.
        /// </param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <param name="returnIfExceptionThrown">If true, the test will continue even if an exception is thrown.</param>
        public static void ClickSimple(IWebDriver driver, string id, By by, bool returnIfExceptionThrown = false)
        {
            Exception latestException = null;

            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                try
                {
                    IWebElement webElement = driver.FindElement(by);
                    webElement.Click();

                    // If no exception happened, we're done, so return.
                    return;
                }
                catch (Exception e)
                {
                    if (returnIfExceptionThrown)
                    {
                        return;
                    }

                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("Click of " + id + " failed continuously for all attempts.", latestException);
        }

        /// <summary>
        /// Click an element until another element displays.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="clickId">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <param name="waitForId">The ID of the object to wait for</param>
        /// <param name="waitForBy">THe locator type of the object to wait for</param>
        /// <param name="waitForDisplayedValue">Decide whether to wait for the object to have a displayed value of true</param>
        public static void ClickUntil(IWebDriver driver, string clickId, By clickBy, string waitForId, By waitForBy, bool waitForDisplayedValue)
        {
            ClickUntil(driver, clickId, clickBy, waitForId, waitForBy, waitForDisplayedValue, false);
        }

        /// <summary>
        /// Click an element until another element displays.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="clickId">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <param name="waitForId">The ID of the object to wait for</param>
        /// <param name="waitForBy">THe locator type of the object to wait for</param>
        /// <param name="waitForDisplayedValue">Decide whether to wait for the object to have a displayed value of true</param>
        /// <param name="ignoreAlerts">Automatically passes false for this parameter</param>
        public static void ClickUntil(IWebDriver driver, string clickId, By clickBy, string waitForId, By waitForBy, bool waitForDisplayedValue, bool ignoreAlerts)
        {
            ClickUntil(driver, clickId, clickBy, waitForId, waitForBy, waitForDisplayedValue, ignoreAlerts, false);
        }

        /// <summary>
        /// Click an element until another element displays.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="clickId">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <param name="waitForId">The ID of the object to wait for</param>
        /// <param name="waitForBy">THe locator type of the object to wait for</param>
        /// <param name="waitForDisplayedValue">Decide whether to wait for the object to have a displayed value of true</param>
        /// <param name="ignoreAlerts">Boolean</param>
        /// <param name="acceptAlerts">Boolean</param>
        public static void ClickUntil(IWebDriver driver, string clickId, By clickBy, string waitForId, By waitForBy, bool waitForDisplayedValue, bool ignoreAlerts, bool acceptAlerts)
        {
            Exception latestException = null;

            int maxAttempts = 3;
            int waitMillisecondsBetweenAttempts = 30000;

            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                try
                {
                    ClickSimple(driver, clickId, clickBy, returnIfExceptionThrown: true);

                    Thread.Sleep(500);

                    for (int i = 0; i < 10; i++)
                    {
                        if (ignoreAlerts)
                        {
                            DismissAlert(driver);
                            Thread.Sleep(500);
                        }
                        else if (acceptAlerts)
                        {
                            AcceptAlert(driver);
                            Thread.Sleep(500);
                        }

                        if (ThisObjectExists(driver, waitForBy) == waitForDisplayedValue)
                        {
                            // It's there, we're done!
                            return;
                        }

                        Thread.Sleep(waitMillisecondsBetweenAttempts / 10);
                    }
                }
                catch (Exception e)
                {
                    latestException = e;
                    if (ThisObjectExists(driver, waitForBy) == waitForDisplayedValue)
                    {
                        // It's there, we're done!
                        return;
                    }

                    Thread.Sleep(waitMillisecondsBetweenAttempts);

                    if (ignoreAlerts)
                    {
                        DismissAlert(driver);
                        Thread.Sleep(500);
                    }
                    else if (acceptAlerts)
                    {
                        AcceptAlert(driver);
                        Thread.Sleep(500);
                    }
                }
            }

            throw new Exception("Click of " + clickId + " failed continuously for all attempts.", latestException);
        }


        /// <summary>
        /// Click until one of 2 objects appears on a page
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="clickId">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <param name="waitForId1">The ID of the first object to wait for</param>
        /// <param name="waitForBy1">THe locator type of the first object to wait for</param>
        /// <param name="waitForDisplayedValue1">Boolean</param>
        /// <param name="waitForId2">The ID of the second object to wait for</param>
        /// <param name="waitForBy2">THe locator type of the second object to wait for</param>
        /// <param name="waitForDisplayedValue2"></param>
        public static void ClickUntilEither(IWebDriver driver, string clickId, By clickBy, string waitForId1, By waitForBy1, bool waitForDisplayedValue1, string waitForId2, By waitForBy2, bool waitForDisplayedValue2)
        {
            Exception latestException = null;

            int maxAttempts = 3;
            int waitMillisecondsBetweenAttempts = 5000;

            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                try
                {
                    ClickSimple(driver, clickId, clickBy, returnIfExceptionThrown: true);

                    if (ThisObjectExists(driver, waitForBy1) == waitForDisplayedValue1
                        || ThisObjectExists(driver, waitForBy2) == waitForDisplayedValue2)
                    {
                        // It's there, we're done!
                        return;
                    }

                    Thread.Sleep(waitMillisecondsBetweenAttempts);

                    if (ThisObjectExists(driver, waitForBy1) == waitForDisplayedValue1
                        || ThisObjectExists(driver, waitForBy2) == waitForDisplayedValue2)
                    {
                        // It's there, we're done!
                        return;
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        if (ThisObjectExists(driver, waitForBy1) == waitForDisplayedValue1
                            || ThisObjectExists(driver, waitForBy2) == waitForDisplayedValue2)
                        {
                            // It's there, we're done!
                            return;
                        }
                    }
                    catch (Exception)
                    {
                    }

                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("Click of " + clickId + " failed continuously for all attempts.", latestException);
        }

        /// <summary>
        /// Clicks until an object is selected.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <param name="selectedValue">Boolean</param>
        public static void ClickUntilSelected(IWebDriver driver, string id, By by, bool selectedValue)
        {
            Exception latestException = null;

            int maxAttempts = 3;
            int waitMillisecondsBetweenAttempts = 5000;

            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                try
                {
                    ClickSimple(driver, id, by, returnIfExceptionThrown: true);

                    if (GetSelectedSimple(driver, id, by))
                    {
                        // It's there, we're done!
                        return;
                    }

                    Thread.Sleep(waitMillisecondsBetweenAttempts);

                    if (GetSelectedSimple(driver, id, by))
                    {
                        // It's there, we're done!
                        return;
                    }
                }
                catch (Exception e)
                {
                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception(id + " was not able to be selected. Attempts: " + maxAttempts + " Wait Time Between Attempts: " + waitMillisecondsBetweenAttempts + "milliseconds.", latestException);
        }

        /// <summary>
        /// Maximize the window
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        public static void MaximizeSimple(IWebDriver driver)
        {
            Exception latestException = null;

            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            for (int attempts = 0;attempts < maxAttempts;attempts++)
            {
                try
                {
                    driver.Manage().Window.Maximize();
                    return;
                }
                catch (Exception e)
                {
                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("Unable to maximize screen. Since this is the first step, it may be that the screen is not loading at all.", latestException);
        }

        /// <summary>
        /// Send text to an object such as filling in a textbox.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <param name="valueToSend">The text string to send t the object</param>
        /// <param name="clearFirst">Boolean Decides whether to clear the textbox before entering the text</param>
        /// <param name="clickFirst">Boolean Decides whether to click inside the textbox before entering the text. This sometimes auto-clears the textbox.</param>
        public static void SendKeysSimple(IWebDriver driver, string id, By by, string valueToSend, bool clearFirst, bool clickFirst)
        {
            Exception latestException = null;

            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            for (int attempts = 0;attempts < maxAttempts;attempts++)
            {
                try
                {

                    try
                    {
                        // This is to handle an occasional issue where a popup box for selecting a local file gets stuck open.
                        IAlert alert = driver.SwitchTo().Alert();
                        alert.Dismiss();
                    }
                    catch (Exception)
                    {
                    }

                    IWebElement element = driver.FindElement(by);

                    if (clearFirst)
                    {
                        element.Clear();
                    }

                    if (clickFirst)
                    {
                        element.Click();
                    }

                    element.SendKeys(valueToSend);

                    // If no exception happened, we're done, so return.
                    return;
                }
                catch (Exception e)
                {
                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("Unable to send keys to element " + id, latestException);
        }

        /// <summary>
        /// DO NOT USE THIS FOR "INPUT" HTML ELEMENTS
        /// This returns a string of text from an object that is NOT an input element, such as a label, etc.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <returns>string</returns>
        public static string GetTextSimple(IWebDriver driver, string id, By by)
        {
            Exception latestException = null;

            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            for (int attempts = 0;attempts < maxAttempts;attempts++)
            {
                try
                {
                    IWebElement element = driver.FindElement(by);

                    return element.Text;
                }
                catch (Exception e)
                {
                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("Unable to get text from element " + id, latestException);
        }

        /// <summary>
        /// USE THIS FOR "INPUT" HTML ELEMENTS
        /// This returns a string of text from an object that IS an input element, such as a textbox, etc.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")      
        /// </param>
        /// <returns>string</returns>
        public static string GetValueSimple(IWebDriver driver, string id, By by)
        {
            Exception latestException = null;

            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            for (int attempts = 0;attempts < maxAttempts;attempts++)
            {
                try
                {
                    IWebElement element = driver.FindElement(by);

                    return element.GetAttribute("value");
                }
                catch (Exception e)
                {
                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("Unable to get value from element " + id, latestException);
        }

        /// <summary>
        /// Returns true if the displayed property of an object is true
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <returns>boolean</returns>
        public static bool GetDisplayedSimple(IWebDriver driver, string id, By by)
        {
            Exception latestException = null;

            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            for (int attempts = 0;attempts < maxAttempts;attempts++)
            {
                try
                {
                    IWebElement element = driver.FindElement(by);

                    return element.Displayed;
                }
                catch (Exception e)
                {
                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("Unable to get displayed for element " + id, latestException);
        }

        /// <summary>
        /// Returns true if the selected property of an object is true, such as a checkbox
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <returns>boolean</returns>
        public static bool GetSelectedSimple(IWebDriver driver, string id, By by)
        {
            Exception latestException = null;

            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            for (int attempts = 0;attempts < maxAttempts;attempts++)
            {
                try
                {
                    IWebElement element = driver.FindElement(by);

                    return element.Selected;
                }
                catch (Exception e)
                {
                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("Unable to get displayed for element " + id, latestException);
        }

        /// <summary>
        /// Sometimes an object may respond to a blur event, aka unfocus. This method forces the unfocus event.
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="idToBlur">The ID of the object to click</param>
        public static void RemoveFocus(IWebDriver driver, string idToBlur)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("var target = document.getElementById(\"" + idToBlur + "\"); var evt = document.createEvent('HTMLEvents'); evt.initEvent('blur', false, true); target.dispatchEvent(evt);");
        }

        /// <summary>
        /// Select text within an object such as a dropdown.
        /// Selenium doesn't fire change events for Internet Explorer dropdpowns inside an UpdatePanel
        /// see https://groups.google.com/forum/#!topic/selenium-users/nwWwPHU7Chs
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <param name="textToSelect">string</param>
        public static void SelectByTextSimple(IWebDriver driver, string id, By by, string textToSelect) // , bool explicitlyCallChangeForIE = false
        {
            if (((ICustomRemoteDriver)driver).GetBrowserName() == "firefox"
             || ((ICustomRemoteDriver)driver).GetBrowserName() == "internet explorer")
            {
                textToSelect = textToSelect.Replace("  ", " ").TrimEnd(new[] {' '});
            }
            Exception latestException = null;

            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            for (int attempts = 0;attempts < maxAttempts;attempts++)
            {
                try
                {
                    SelectElement element = new SelectElement(driver.FindElement(by));
                    element.SelectByText(textToSelect);
                    return;
                }
                catch (Exception e)
                {
                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("Unable to select text of element " + id, latestException);
        }

        /// <summary>
        /// Waits an amount of time for the displayed status of an object. Tries 10 times to get displayed value
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <param name="displayedStatusToWaitFor">string</param>
        public static void WaitForDisplayedStatus(IWebDriver driver, string id, By by, string displayedStatusToWaitFor)
        {
            Exception latestException = null;

            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            for (int attempts = 0;attempts < maxAttempts;attempts++)
            {
                try
                {
                    if (driver.FindElement(by).Displayed.ToString()
                        .Equals(displayedStatusToWaitFor, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // If no exception happened, and the displayed value matched, then we're done, so return.
                        return;
                    }

                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
                catch (Exception e)
                {
                    latestException = e;
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                }
            }

            throw new Exception("DisplayedStatus of element " + id + " never became " + displayedStatusToWaitFor, latestException);
        }

        /// <summary>
        /// Scroll the page so that an object is in view
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="elementClientId">The ID of the object that needs to be in view</param>
        /// <param name="alignToTop">Boolean</param>
        public static void ScrollIntoView(IWebDriver driver, string elementClientId, bool alignToTop = false)
        {
            Thread.Sleep(500);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("var target = document.getElementById(\"" + elementClientId + "\"); target.scrollIntoView(" + (alignToTop ? "true" : "false") + ")");
            Thread.Sleep(500);
        }


        /// <summary>
        /// Execute a javascript command until something happens
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="javascriptCommand">Javascript command to execute</param>
        /// <param name="waitForId">The ID of the object to wait for</param>
        /// <param name="waitForBy">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <param name="waitForDisplayedValue">Decide whether to wait for the object to have a displayed value of true</param>
        /// <param name="initialWaitSeconds">Number of seconds to wait for a displayed value</param>
        /// <param name="subsequentWaitSeconds">The number of seconds to wait after the first wait</param>
        public static void ExecuteJavascriptUntil(IWebDriver driver, string javascriptCommand, string waitForId, By waitForBy, bool waitForDisplayedValue, int initialWaitSeconds = 45, int subsequentWaitSeconds = 15)
        {
            ExecuteJavascriptUntil(driver, javascriptCommand, waitForId, waitForBy, waitForDisplayedValue, false, initialWaitSeconds: initialWaitSeconds, subsequentWaitSeconds: subsequentWaitSeconds);
        }

        /// <summary>
        /// Execute a javascript command until something happens
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="javascriptCommand">Javascript command to execute</param>
        /// <param name="id">The ID of the object to wait for</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <param name="waitForDisplayedValue">Decide whether to wait for the object to have a displayed value of true</param>
        /// <param name="ignoreAlerts">Boolean Decides whether to ignore alerts or not</param>
        /// <param name="initialWaitSeconds">Number of seconds to wait for a displayed value</param>
        /// <param name="subsequentWaitSeconds">The number of seconds to wait after the first wait</param>
        public static void ExecuteJavascriptUntil(IWebDriver driver, string javascriptCommand, string id, By by, bool waitForDisplayedValue, bool ignoreAlerts, int initialWaitSeconds = 45, int subsequentWaitSeconds = 15)
        {
            try
            {
                // Run it and give it some time
                ((IJavaScriptExecutor)driver).ExecuteScript(javascriptCommand);

                int secondsWaited = 0;
                while (secondsWaited < initialWaitSeconds && ThisObjectExists(driver, by) != waitForDisplayedValue)
                {
                    Thread.Sleep(1000);
                    secondsWaited++;
                }

                // If it doesn't work, try it again until it does
                int additionalAttempt = 0;
                int maxAdditionalAttempts = 3;
                while (additionalAttempt < maxAdditionalAttempts && ThisObjectExists(driver, by) != waitForDisplayedValue)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript(javascriptCommand);

                    Thread.Sleep(subsequentWaitSeconds * 1000);
                    additionalAttempt++;
                }

                // If it's still hasn't disappeared, throw an exception
                if (additionalAttempt == maxAdditionalAttempts && ThisObjectExists(driver, by) != waitForDisplayedValue)
                {
                    throw new Exception("Execution of javascript command \"" + javascriptCommand + "\" failed continuously for all attempts.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error in ExecuteJavascriptUntil: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Determines whether an object exists or not
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <returns></returns>
        public static bool ThisObjectExists(IWebDriver driver, By by)
        {

            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 0, 0, 200));

                int maxAttempts = 3;
                int waitMillisecondsBetweenAttempts = 200;

                for (int attempts = 0;attempts < maxAttempts;attempts++)
                {
                    try
                    {
                        IWebElement element = driver.FindElement(by);
                        return element.Displayed;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(waitMillisecondsBetweenAttempts);
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, CTTestGridMethods.implicitWaitTime));
                }
                catch (Exception)
                {
                    Console.Write("");
                }
            }
        }

        /// <summary>
        /// Send Keys with or without clearing first
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="id">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <param name="valueToSend">Text string to enter into an input object.</param>
        /// <param name="clearFirst">Boolean</param>
        /// <param name="clickFirst">Boolean</param>
        public static void SendKeysReliably(IWebDriver driver, string id, By by, string valueToSend, bool clearFirst, bool clickFirst)
        {
            int maxAttempts = 10;
            int waitMillisecondsBetweenAttempts = 1000;

            try
            {
                // Give the element the chance to materialize if it isn't already there
                int attempt = 1;
                while (attempt < maxAttempts && !ThisObjectExists(driver, by))
                {
                    Thread.Sleep(waitMillisecondsBetweenAttempts);
                    attempt++;
                }

                // If it's still not there, throw an exception
                if (!ThisObjectExists(driver, by))
                {
                    throw new Exception("The field, " + id + ", is not displayed.");
                }

                SendKeysSimple(driver, id, by, valueToSend, clearFirst, clickFirst);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error in SendKeysReliably: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Navigates to a URL
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="url">The URL to navigate to</param>
        /// <param name="idToLookFor">The unique ID of the object to look for to verify we are on the right page</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <param name="maxSeconds">Maximum seconds to wait for before giving up</param>
        public static void SimpleGoToUrl(IWebDriver driver, String url, String idToLookFor, By by, int maxSeconds = 120)
        {
            // Possible fix for timeouts? Seemed to help in firefox but not chrome... hmm...
            Exception latestException = null;

            int attemptToReachUrlMaxCount = 2;
            for (int attemptToReachUrlCount = 0;attemptToReachUrlCount < attemptToReachUrlMaxCount;attemptToReachUrlCount++)
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 2, 0));

                    driver.Navigate().GoToUrl(url);


                    int elapsedSeconds = 0;
                    while (!SeleniumSharedMethods.ThisObjectExists(driver, by) && elapsedSeconds < maxSeconds)
                    {
                        elapsedSeconds++;
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    latestException = ex;
                }
                finally
                {
                    driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, CTTestGridMethods.implicitWaitTime));
                }

                if (SeleniumSharedMethods.ThisObjectExists(driver, by))
                {
                    return;
                }
            }
            throw new Exception("Was unable to navigate to URL " + url, latestException);
        }

        /// <summary>
        /// Click an object and verify window opened
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="clickName">The ID of the object to click</param>
        /// <param name="clickBy">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <param name="expectedNumberOfWindows">The expected number of windows that should be open after the button click.</param>
        /// <param name="javascriptToRunForInernetExplorer">internet explorer workaround command</param>
        public static void SimpleClickToOpenWindow(IWebDriver driver, 
            String clickName, 
            By clickBy, 
            int expectedNumberOfWindows, 
            String javascriptToRunForInernetExplorer = null)
        {
            int maxClicks = 3;

            for (int clicks = 0; clicks < maxClicks; clicks++)
            {
                if ((javascriptToRunForInernetExplorer != null)
                    && (((ICustomRemoteDriver)driver).GetBrowserName() == "internet explorer"))
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript(javascriptToRunForInernetExplorer);
                }
                else
                {
                    SeleniumSharedMethods.ClickSimple(driver, clickName, clickBy);
                }

                Thread.Sleep(2000);

                if (driver.WindowHandles.Count >= expectedNumberOfWindows)
                {
                    break;
                }

                Thread.Sleep(4000);

                if (driver.WindowHandles.Count >= expectedNumberOfWindows)
                {
                    break;
                }

                Thread.Sleep(8000);

                if (driver.WindowHandles.Count >= expectedNumberOfWindows)
                {
                    break;
                }
            }

            
            if (driver.WindowHandles.Count > expectedNumberOfWindows)
            {
                throw new Exception("More than the expected number of windows were found opened after clicking " + clickName);
            }

            if (driver.WindowHandles.Count < expectedNumberOfWindows)
            {
                throw new Exception("A new window was not opened after clicking " + clickName);
            }
        }

        // Note, this assumes we only ever use up to 2 windows at a time and close misc. windows as we go to avoid having 3.
        // If we ever need 3, we need to develop a more complex solution that tracks each window so you can identify which is the new one.
        /// <summary>
        /// Switch to another open window
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="currentWindow">Current window name</param>
        /// <param name="closeCurrentWindow">Boolean</param>
        /// <returns></returns>
        public static string SimpleSwitchToOtherWindow(IWebDriver driver, string currentWindow, bool closeCurrentWindow = false)
        {
            ITargetLocator locator = driver.SwitchTo();

            string newWindow = null;

            ReadOnlyCollection<string> windowHandles = driver.WindowHandles;
            for (int i = 0; i < windowHandles.Count; i++)
            {
                if (windowHandles[i] != currentWindow)
                {
                    newWindow = windowHandles[i];
                }
            }

            if (newWindow != null)
            {
                if (closeCurrentWindow)
                {
                    locator.Window(currentWindow);
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.onbeforeunload = function(e){};");
                    Thread.Sleep(500);
                    driver.Close();
                    Thread.Sleep(1000);
                }

                locator.Window(newWindow);

                Thread.Sleep(500);

                SeleniumSharedMethods.MaximizeSimple(driver);

                Thread.Sleep(1000);

                return newWindow;
            }
            else
            {
                throw new Exception("No other window was found to switch to!");
            }
        }

        /// <summary>
        /// Get all the options in a dropdown
        /// </summary>
        /// <param name="driver">Just enter 'driver' here without quotes</param>
        /// <param name="name">The ID of the object to click</param>
        /// <param name="by">Locator type. 
        /// Possible locator types: By.Id(""), By.XPath(""), By.CssSelector(""), By.Name(""), By.LinkText(""), By.ClassName(""), By.TagName(""), By.PartialLinkText("")</param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> GetAllOptionsInDropdown(IWebDriver driver, string name, By by)
        {
            try
            {
                IWebElement sel = driver.FindElement(by);
                return sel.FindElements(By.TagName("option"));
            }
            catch (Exception)
            {
                throw new Exception("Unable to get values in dropdown " + name);
            }
        }


        public static bool WaitUntilExists(IWebDriver driver, string idToLookFor, By by, int maxAttempts, int waitMillisecondsBetweenAttempts)
        {
            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                if (ThisObjectExists(driver, by))
                {
                    // If no exception happened, and the object exists, then we're done, so return.
                    return true;
                }
                
                Thread.Sleep(waitMillisecondsBetweenAttempts);
            }
            
            return false;
        }
    }
}
