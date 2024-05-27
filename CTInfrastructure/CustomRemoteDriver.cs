/*!
 * Crystal Test CustomRemoteDriver.cs
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
using System;
using System.Collections.Generic;

namespace CTInfrastructure
{
    // See the parent class definition here: http://code.google.com/p/selenium/source/browse/trunk/remote/client/src/csharp/webdriver-remote-client/IWebDriver.cs?spec=svn10911&r=10911
    public class CustomRemoteDriver : RemoteWebDriver, ICustomRemoteDriver
    {

        public CustomRemoteDriver(Uri uri, DesiredCapabilities capabilities)
            : base(uri, capabilities)
        {

        }

        public CustomRemoteDriver(DesiredCapabilities capabilities)
            : base(capabilities)
        {

        }

        public SessionId GetSessionId()
        {
            return base.SessionId;
        }

        public ICommandExecutor GetCommandExecutor()
        {
            return base.CommandExecutor;
        }

        public Response PublicExecute(string driverCommandToExecute, Dictionary<string, object> parameters)
        {
            return Execute(driverCommandToExecute, parameters);
        }

        public String GetBrowserName()
        {
            return Capabilities.BrowserName;
        }

    }

}
