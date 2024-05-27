using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTWebsite
{
    public static class TestCaseFileColumns
    {
        public const int projectAbbreviation = 0;
        public const int testCaseId = 1;
        public const int groupTestAbbreviation = 2;
        public const int release = 3;
        public const int sprint = 4;
        public const int testCaseDescription = 5;
        public const int active = 6;
        public const int testCaseOutdated = 7;
        public const int testScriptOutdated = 8;
        public const int testCaseSteps = 9;
        public const int expectedResults = 10;
        public const int screenshots = 11;
        public const int screenshotDescriptions = 12;
        public const int testCaseNotes = 13;
        public const int isThisAnUpdate = 14;
        public const int testCategory = 15;        
        public const int autoTestClass = 16;
        public const int autoMetaDataTable = 17;
        public const int autoMetaDataRow = 18;
        public const int automated = 19;
        public const int reasonForNotAutomated = 20;        
    }

    public static class TestResultFileColumns
    {
        public const int projectAbbreviation = 0;
        public const int testCaseId = 1;
        public const int environment = 2;
        public const int browserAbbreviation = 3;
        public const int status = 4;
        public const int reasonForStatus = 5;
        public const int reasonForStatusDetailed = 6;
        public const int stepsToReproduce = 7;
        public const int defectTicketNumber = 8;
        public const int screenshots = 9;
        public const int screenshotDescriptions = 10;
    }
}