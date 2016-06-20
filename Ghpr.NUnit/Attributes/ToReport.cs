using System;
using Ghpr.Core;
using Ghpr.Core.Common;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Ghpr.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ToReport : NUnitAttribute, ITestAction
    {
        private readonly string _testGuidString;

        public ToReport(string testGuidString = "")
        {
            _testGuidString = testGuidString;
        }

        public void BeforeTest(ITest test)
        {
            var testRun = new TestRun(_testGuidString);
            Reporter.TestStarted(testRun);
        }

        public void AfterTest(ITest test)
        {
            var context = TestContext.CurrentContext;
            var testRun = new TestRun
            {
                Name = test.Name,
                FullName = test.FullName,
                TestStackTrace = context.Result.StackTrace ?? "",
                TestMessage = context.Result.Message ?? "",
                Result = context.Result.Outcome?.ToString() ?? "Unknown"
            };
            Reporter.TestFinished(testRun);
        }

        public ActionTargets Targets => ActionTargets.Test;
    }
}
