/*!
 * Crystal Test NetworkingUtilities.cs
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

using System.IO;
using System.Net;
using System.Text;

namespace Utilities
{
    public class NetworkingUtilities
    {
        public static string HttpRequestAndResponse(string url)
        {
            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // execute the request
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // we will read data via the response stream
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
