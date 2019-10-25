using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NUnitTests
{
    [TestFixture]
    public class TestBase
    {
        [TearDown]
        public void TakeScreenIfFailed()
        {
            var res = TestContext.CurrentContext.Result.Outcome;
            if (res.Equals(ResultState.Failure) || res.Equals(ResultState.Error))
            {
                //ScreenHelper.SaveScreenshot(TakeScreen());
                var bytes = TakeScreen();
                var fullPath = SaveScreen(bytes, Guid.NewGuid() + ".png");
                TestContext.AddTestAttachment(fullPath);
            }
        }

        public static byte[] TakeScreen()
        {
            return Convert.FromBase64String(@"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");
        }

        public static string SaveScreen(byte[] screen, string fileName)
        {
            var folder = @"C:\Screenshots";
            Directory.CreateDirectory(folder);
            var fullPath = Path.Combine(folder, fileName);
            File.WriteAllBytes(fullPath, screen);
            return fullPath;
        }
    }
}