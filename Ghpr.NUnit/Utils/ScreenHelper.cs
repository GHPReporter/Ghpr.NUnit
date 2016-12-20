using System;
using System.IO;
using Ghpr.Core.Utils;
using Ghpr.NUnit.Extensions;
using NUnit.Framework;

namespace Ghpr.NUnit.Utils
{
    public static class ScreenHelper
    {
        internal const string ScreenNameTemplate = "ghpr_screenshot_";

        private static string GetScreenKey(int count)
        {
            return $"{ScreenNameTemplate}{count}";
        }

        public static void SaveScreenshot(byte[] screenBytes, string outputPath = "")
        {
            var guid = TestContext.CurrentContext.Test.Properties.Get("TestGuid")?.ToString();
            var fullName = TestContext.CurrentContext.Test.FullName;

            var testGuid = guid != null ? Guid.Parse(guid) : GuidConverter.ToMd5HashGuid(fullName);

            var screenshotName = ScreenshotHelper.SaveScreenshot(
                Path.Combine(outputPath.Equals("") ? GhprEventListener.Settings.OutputPath : outputPath,
                Names.TestsFolderName, testGuid.ToString(), Names.ImgFolderName), screenBytes, DateTime.Now);

            var count = 0;
            var screenKey = GetScreenKey(count);
            while (TestContext.CurrentContext.Test.Properties.Get(screenKey) != null)
            {
                count++;
                screenKey = GetScreenKey(count);
            }

            TestContext.CurrentContext.Test.Properties.Add(screenKey, screenshotName);
        }
    }
}