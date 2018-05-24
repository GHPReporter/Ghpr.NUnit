﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Ghpr.NUnit.Utils;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NUnitTests
{
    [TestFixture]
    public class Tests
    {
        [TearDown]
        public void TakeScreenIfFailed()
        {
            var res = TestContext.CurrentContext.Result.Outcome;
            if (res.Equals(ResultState.Failure) || res.Equals(ResultState.Error))
            {
                ScreenHelper.SaveScreenshot(TakeScreen());
            }
        }

        [Test(Description = "This is example of taking screenshots inside test")]
        [Category("Screenshots")]
        public void TestMethodFailed()
        {
            Console.WriteLine("Taking screen...");
            var bytes = TakeScreen();
            //all you need to do is to pass byte[] to ScreenHelper:
            ScreenHelper.SaveScreenshot(bytes);
            Console.WriteLine("Done.");
            Assert.Fail("Noooooo..... Test is failed.");
        }

        [Test, Property("TestGuid", "11111111-1111-1111-1111-111111111111"),
         Property("Priority", "High"),
         Property("TestType", "Smoke")]
        [Category("Cat1")]
        [Category("Failed")]
        [Description("This is test description")]
        public void SimpleFailedTest()
        {
            Console.WriteLine("This is test output, we are logging some stuff!");
            Console.WriteLine($"Comparing '{TestData.Actual}' and '{TestData.Expected}'");
            TestDataHelper.AddTestData(TestData.Actual, TestData.Expected, "Let's compare to XML strings!");
            Console.WriteLine($"Comparing '{TestData.Actual}' and '{TestData.Expected}'");
            TestDataHelper.AddTestData(TestData.Actual, TestData.Expected, "Let's compare for the second time!");
            Assert.AreEqual(1, 2);
        }

        public static byte[] TakeScreen()
        {
            var b = Screen.PrimaryScreen.Bounds;
            var ic = new ImageConverter();
            using (var btm = new Bitmap(b.Width, b.Height))
            using (var g = Graphics.FromImage(btm))
            {
                g.CopyFromScreen(b.X, b.Y, 0, 0, btm.Size, CopyPixelOperation.SourceCopy);
                return (byte[])ic.ConvertTo(btm, typeof(byte[]));
            }
        }
    }
}
