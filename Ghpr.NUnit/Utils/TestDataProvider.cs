using System;
using Ghpr.Core.Extensions;
using Ghpr.Core.Interfaces;
using NUnit.Framework;

namespace Ghpr.NUnit.Utils
{
    public class TestDataProvider : ITestDataProvider
    {
        public Guid GetCurrentTestRunGuid()
        {
            var guid = TestContext.CurrentContext.Test.Properties.Get("TestGuid")?.ToString();
            var testGuid = guid != null ? Guid.Parse(guid) : GetCurrentTestRunFullName().ToMd5HashGuid();
            return testGuid;
        }

        public string GetCurrentTestRunFullName()
        {
            var fullName = TestContext.CurrentContext.Test.FullName;
            return fullName;
        }
    }
}