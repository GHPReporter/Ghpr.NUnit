using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Ghpr.Core;
using Ghpr.Core.Common;
using Ghpr.Core.Enums;
using Ghpr.Core.Interfaces;
using Ghpr.Core.Utils;
using Ghpr.NUnit.Utils;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Ghpr.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GhprTestAttribute : Attribute, ITestAction
    {
        private static readonly Reporter Reporter;
        public static string OutputPath => Reporter.Settings.OutputPath;

        static GhprTestAttribute()
        {
            Reporter = new Reporter(TestingFramework.NUnit);
            StaticLog.Initialize(Reporter.Settings.OutputPath);
        }

        public void BeforeTest(ITest test)
        {
            if (!Reporter.TestRunStarted)
            {
                Reporter.RunStarted();
            }
            if (!test.IsSuite)
            {
                Reporter.TestStarted(GetGhprTestRun(test));
            }
        }

        public void AfterTest(ITest test)
        {
            Reporter.TestFinished(GetGhprTestRun(test));
        }

        public ActionTargets Targets => ActionTargets.Test;

        private ITestRun GetGhprTestRun(ITest nunitTest)
        {
            var testXml = nunitTest.ToXml(true).ToString();
            var xDoc = new XmlDocument();
            xDoc.LoadXml(testXml);
            var testRun = TestRunHelper.GetTestRun(xDoc);
            return testRun;
        }
    }
}