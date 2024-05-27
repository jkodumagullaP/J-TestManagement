/*!
 * Crystal Test Constants.cs
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
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Net.Mail;


namespace Utilities
{
    public static class Constants
    {
        // These get set in ThreeColumn.Master
        public static string LocalRootPath = "";
        public static string NetworkRootPath = "";

        public static String QAAConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["QAAConnectionString"].ConnectionString;
            }
        }

        public static String QAANetworkDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["QAANetworkDomain"];
            }
        }

        public static String QAANetworkServiceUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["QAANetworkServiceUserName"];
            }
        }

        public static String QAANetworkServiceUserPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["QAANetworkServiceUserPassword"];
            }
        }

        public static String FROMEMAILADDRESS
        {
            get
            {
                return ConfigurationManager.AppSettings["NoReplyEmailAddress"];
            }
        }

        public static String PICKUPDIRECTORYLOCATION
        {
            get
            {
                return ConfigurationManager.AppSettings["pickupDirectoryLocation"];
            }
        }

        public static class TestType
        {
            public const string AUTOMATION = "Automated";
            public const string MANUAL = "Manual";
        }

        public static class Browsers
        {
            public const string CHROME = "CHROME";
            public const string FF = "FF";
            public const string IE8 = "IE8";
            public const string IE9 = "IE9";
            public const string IE10 = "IE10";
            public const string IE11 = "IE11";
            public const string MACSAF = "MACSAF";
            public const string WINSAF = "WINSAF";
        }

        public static string GetSeleniumCompatibleBrowserName(string browserAbbreviation)
        {
            switch (browserAbbreviation)
            {
                case "FF":
                    return "firefox";
                case "IE8":
                case "IE9":
                case "IE10":
                case "IE11":
                    return "internet explorer";
                case "CHROME":
                    return "chrome";
                default:
                    return "ERROR";
            }
        }

        public static string GetSeleniumCompatibleBrowserVersion(string browserAbbreviation)
        {
            switch (browserAbbreviation)
            {
                case "FF":
                    return "";
                case "IE8":
                    return "8";
                case "IE9":
                    return "9";
                case "IE10":
                    return "10";
                case "IE11":
                    return "11";
                case "CHROME":
                    return "";
                default:
                    return "ERROR";
            }
        }

        public static class ProcessStatus
        {
            public const string FAIL = "Fail";
            public const string IN_QUEUE = "In Queue";
            public const string IN_PROGRESS = "In Progress";
            public const string NOT_STARTED = "Not Started";
            public const string PASS = "Pass";
            public const string TEST_CASE_NEEDS_UPDATED = "Test Case Needs Updated";
            public const string RETEST = "Retest";
        }
    }
}
