/*!
 * Crystal Test AutomationTestEngine.cs
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

using CTInfrastructure;
using System;
using System.Threading;
using Utilities;



namespace AutomationTestEngine
{
    class Program
    {
        // Every 5 seconds
        private const int iterationsPerMinute = 12;

        static void Main(string[] args)
        {
            MasterThread();
        }

        public static void MasterThread()
        {
            DateTime now = DateTime.Now;
            Console.WriteLine(now.ToShortDateString() + " " + now.ToLongTimeString() + ": Starting engine");
            Console.WriteLine(now.ToShortDateString() + " " + now.ToLongTimeString() + ": Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            string previousStatus = "";

            int waitMillisecondsBetweenPings = 60000 / iterationsPerMinute;
            int iteration = 0;

            int inProgressTestCount = AutomatedTestBase.GetInProgressTestCount();
            if (inProgressTestCount > 0)
            {
                Console.WriteLine(now.ToShortDateString() + " " + now.ToLongTimeString() + ": There were already " + inProgressTestCount + " test(s) 'In Progress' at the time the test engine was started. Most likely the test engine has just been restarted or the test grid unexpectedly shut down. Now reverting all 'In Progress' tests back to 'Retest' so they can be started again.");
                RevertAllInProgressTestsRetest();
            }

            while (true)
            {
                string result = AutomatedTestBase.CheckForRunnableQueuedTests();

                if (previousStatus != result)
                {
                    previousStatus = result;
                    now = DateTime.Now;
                    Console.Write("\n" + now.ToShortDateString() + " " + now.ToLongTimeString() + ": " + result + ".\r\n");
                }
                else if (iteration % iterationsPerMinute == 0)
                {
                    // once per minute print even if nothing is happening
                    Console.Write("\n" + now.ToShortDateString() + " " + now.ToLongTimeString() + ": There is currently " + inProgressTestCount + " tests running.\r\n");
                }

                Thread.Sleep(waitMillisecondsBetweenPings);
                iteration++;
            }
        }

        public static void RevertAllInProgressTestsRetest()
        {
            // A using statement will make the contained object exist only for the duration of
            // the following curly bracket section. At the end, .Dispose is called AUTOMATICALLY.
            using (System.Data.SqlClient.SqlConnection Conn =
                new System.Data.SqlClient.SqlConnection(Constants.QAAConnectionString))
            {
                Conn.Open();

                using (System.Data.SqlClient.SqlCommand cmd = Conn.CreateCommand())
                {
                    cmd.CommandText = "update [dbo].[TestResults]"
                                    + " set status = '" + Constants.ProcessStatus.RETEST + "'"
                                    + " where status = '" + Constants.ProcessStatus.IN_PROGRESS + "'";

                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
