using System;
using System.Xml;
using Ghpr.Core;
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

        public void BeforeTest(ITest nunitTest)
        {
            if (!Reporter.TestRunStarted)
            {
                Reporter.RunStarted();
            }
            if (!nunitTest.IsSuite)
            {
                var ghprTest = GetGhprTestRun(nunitTest);
                Reporter.TestFinished(ghprTest);
            }
        }

        public void AfterTest(ITest nunitTest)
        {
            if (!nunitTest.IsSuite)
            {
                var ghprTest = GetGhprTestRun(nunitTest);
                Reporter.TestFinished(ghprTest);
            }
        }

        public ActionTargets Targets => ActionTargets.Test;

        private ITestRun GetGhprTestRun(ITest nunitTest)
        {
            var testXml = nunitTest.ToXml(true).OuterXml;
            var xDoc = new XmlDocument();
            xDoc.LoadXml(testXml);
            var testRun = TestRunHelper.GetTestRun(xDoc.DocumentElement);
            return testRun;
        }
    }
}