using System;
using NUnit.Framework;

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
            while (TestContext.CurrentContext.Test.Properties.Get(dateTimeKey) != null 
                && TestContext.CurrentContext.Test.Properties.Get(actualKey) != null 
                && TestContext.CurrentContext.Test.Properties.Get(expectedKey) != null 
                && TestContext.CurrentContext.Test.Properties.Get(commentKey) != null)
            {
                count++;
                dateTimeKey = Core.Helpers.TestDataHelper.GetTestDataDateTimeKey(count);
                actualKey = Core.Helpers.TestDataHelper.GetTestDataActualKey(count);
                expectedKey = Core.Helpers.TestDataHelper.GetTestDataExpectedKey(count);
                commentKey = Core.Helpers.TestDataHelper.GetTestDataCommentKey(count);
            }

            TestContext.CurrentContext.Test.Properties.Add(dateTimeKey, DateTime.Now.ToString("yyyyMMdd_HHmmssfff"));
            TestContext.CurrentContext.Test.Properties.Add(actualKey, actual);
            TestContext.CurrentContext.Test.Properties.Add(expectedKey, expected);
            TestContext.CurrentContext.Test.Properties.Add(commentKey, comment);
        }
    }
}