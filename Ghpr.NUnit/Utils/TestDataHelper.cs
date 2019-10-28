using System;
using Ghpr.Core.Utils;
using NUnit.Framework.Internal;

namespace Ghpr.NUnit.Utils
{
    public static class TestDataHelper
    {
        public static void AddTestData(string actual, string expected, string comment)
        {
            var count = 0;
            var dateTimeKey = Paths.GetTestDataDateTimeKey(count);
            var actualKey = Paths.GetTestDataActualKey(count);
            var expectedKey = Paths.GetTestDataExpectedKey(count);
            var commentKey = Paths.GetTestDataCommentKey(count);
            while (TestExecutionContext.CurrentContext.CurrentTest.Properties.Get(dateTimeKey) != null 
                && TestExecutionContext.CurrentContext.CurrentTest.Properties.Get(actualKey) != null 
                && TestExecutionContext.CurrentContext.CurrentTest.Properties.Get(expectedKey) != null 
                && TestExecutionContext.CurrentContext.CurrentTest.Properties.Get(commentKey) != null)
            {
                count++;
                dateTimeKey = Paths.GetTestDataDateTimeKey(count);
                actualKey = Paths.GetTestDataActualKey(count);
                expectedKey = Paths.GetTestDataExpectedKey(count);
                commentKey = Paths.GetTestDataCommentKey(count);
            }

            TestExecutionContext.CurrentContext.CurrentTest.Properties.Add(dateTimeKey, DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff"));
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Add(actualKey, actual);
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Add(expectedKey, expected);
            TestExecutionContext.CurrentContext.CurrentTest.Properties.Add(commentKey, comment);
        }
    }
}