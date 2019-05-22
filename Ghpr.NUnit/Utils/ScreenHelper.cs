using System;
using System.IO;
using NUnit.Framework;

namespace Ghpr.NUnit.Utils
{
    public static class ScreenHelper
    {
        public static void SaveScreenshot(byte[] screenshotBytes, string format = "png")
        {
            var wd = TestContext.CurrentContext.WorkDirectory;
            var filePath = Path.Combine(wd, $"{Guid.NewGuid()}.{format}");
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(screenshotBytes, 0, screenshotBytes.Length);
            }
            TestContext.AddTestAttachment(filePath, "Screenshot added");
        }
    }
}