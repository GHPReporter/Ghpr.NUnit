using System;
using Ghpr.Core.Extensions;
using Ghpr.Core.Interfaces;
using NUnit.Framework;

namespace Ghpr.NUnit.Utils
{
    public class ScreenshotService : IScreenshotService
    {
        public IDataService DataService { get; internal set; }

        public void InitializeDataService(IDataService dataService)
        {
            DataService = dataService;
        }

        public void SaveScreenshot(byte[] screenBytes)
        {
            var guid = TestContext.CurrentContext.Test.Properties.Get("TestGuid")?.ToString();
            var fullName = TestContext.CurrentContext.Test.FullName;
            var testGuid = guid != null ? Guid.Parse(guid) : fullName.ToMd5HashGuid();
            DataService.SaveScreenshot(screenBytes, testGuid, DateTime.Now);
        }
    }
}