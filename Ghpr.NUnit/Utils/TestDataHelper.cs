using System;
using NUnit.Framework.Internal;

namespace Ghpr.NUnit.Utils
{
    public static class TestDataHelper
    {
        public static void AddTestData(string actual, string expected, string comment)
        {
            var count = 0;
            var dateTimeKey = Core.Helpers.TestDataHelper.GetTestDataDateTimeKey(count);
            var actualKey = Core.Helpers.TestDataHelper.GetTestDataActualKey(count);
            var expectedKey = Core.Helpers.TestDataHelper.GetTestDataExpectedKey(count);
            var commentKey = Core.Helpers.TestDataHelper.GetTestDataCommentKey(count);
            while (TestExecutionContext.CurrentContext.CurrentTest.Properties.Get(dateTimeKey) != null 
                && TestExecutionContext.CurrentContext.CurrentTest.Properties.Get(actualKey) != null 
                && TestExecutionContext.CurrentContext.CurrentTest.Properties.Get(expectedKey) != null 
                && TestExecutionContext.CurrentContext.CurrentTest.Properties.Get(commentKey) != null)
            {
                count++;
                dateTimeKey = Core.Helpers.TestDataHelper.GetTestDataDateTimeKey(count);
                actualKey = Core.Helpers.TestDataHelper.GetTestDataActualKey(count);
                expectedKey = Core.Helpers.TestDataHelper.GetTestDataExpectedKey(count);
                commentKey = Core.Helpers.TestDataHelper.GetTestDataCommentKey(count);
            }

            TestExecutionContext.CurrentContext.CurrentTest.Properties.Add(dateTimeKey, DateTime.Now.ToString("yyyyMMdd_HHmmssfff"));
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Add(actualKey, actual);
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Add(expectedKey, expected);
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Add(commentKey, comment);
        }
    }
}