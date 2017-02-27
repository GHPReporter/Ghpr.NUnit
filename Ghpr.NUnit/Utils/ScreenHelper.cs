using System;
using System.IO;
using Ghpr.Core.Helpers;
using Ghpr.Core.Utils;
using Ghpr.NUnit.Extensions;
using NUnit.Framework;

namespace Ghpr.NUnit.Utils
{
    public static class ScreenHelper
    {
        internal const string ScreenKeyTemplate = "ghpr_screenshot_";

        private static string GetScreenKey(int count)
        {
            return $"{ScreenKeyTemplate}{count}";
        }

        public static void SaveScreenshot(byte[] screenBytes)
        {
            var guid = TestContext.CurrentContext.Test.Properties.Get("TestGuid")?.ToString();
            var fullName = TestContext.CurrentContext.Test.FullName;
            var testGuid = guid != null ? Guid.Parse(guid) : GuidConverter.ToMd5HashGuid(fullName);
            var fullPath = Path.Combine(GhprEventListener.OutputPath, Paths.Folders.Tests, testGuid.ToString(), Paths.Folders.Img);
            var screenshotName = ScreenshotHelper.SaveScreenshot(fullPath, screenBytes, DateTime.Now);
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