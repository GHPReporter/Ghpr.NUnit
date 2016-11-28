using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ghpr.Core;
using Ghpr.Core.Common;
using Ghpr.Core.Interfaces;
using Ghpr.Core.Utils;
using Ghpr.NUnit.Extensions;
using NUnit;
using NUnit.Framework;

namespace Ghpr.NUnit.Utils
{
    public static class TestRunHelper
    {
        private const string ScreenNameTemplate = "ghpr_screenshot_";

        private static string GetScreenKey(int count)
        {
            return $"{ScreenNameTemplate}{count}";
        }

        public static void SaveScreenshot(byte[] screenBytes, string outputPath = "")
        {
            var guid = TestContext.CurrentContext.Test.Properties.Get("TestGuid")?.ToString();
            var fullName = TestContext.CurrentContext.Test.FullName;

            var testGuid = guid != null ? Guid.Parse(guid) : GuidConverter.ToMd5HashGuid(fullName);

            var screenshotName = Taker.SaveScreenshot(
                Path.Combine(outputPath.Equals("") ? GhprEventListener.Settings.OutputPath : outputPath, 
                Reporter.TestsFolderName, testGuid.ToString(), Reporter.ImgFolderName), screenBytes, DateTime.Now);

            var count = 0;
            var screenKey = GetScreenKey(count);
            while (TestContext.CurrentContext.Test.Properties.Get(screenKey) != null)
            {
                count++;
                screenKey = GetScreenKey(count);
            }

            TestContext.CurrentContext.Test.Properties.Add(screenKey, screenshotName);
        }
        
        public static ITestRun GetTestRun(XmlNode testNode)
        {
            try
            {
                var now = DateTime.Now;
                var guid = testNode.SelectSingleNode("properties/property[@name='TestGuid']")?.GetAttribute("value");
                var testType = testNode.SelectSingleNode("properties/property[@name='TestType']")?.GetAttribute("value");
                var priority = testNode.SelectSingleNode("properties/property[@name='Priority']")?.GetAttribute("value");
                var categories = testNode.SelectNodes("properties/property[@name='Category']")?.Cast<XmlNode>()
                    .Select(n => n.GetAttribute("value")).ToArray();

                var screenNames = testNode.SelectNodes($"properties/property[contains(@name,'{ScreenNameTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.GetAttribute("value")).ToArray();

                var screens = screenNames?.Select(screenName => new TestScreenshot(screenName))
                    .Cast<ITestScreenshot>().ToList();

                var r = testNode.GetAttribute("result");
                var l = testNode.GetAttribute("label");
                var fullName = testNode.GetAttribute("fullname");
                var ti = new ItemInfo
                {
                    Guid = guid != null ? Guid.Parse(guid) : GuidConverter.ToMd5HashGuid(fullName),
                    Start = testNode.GetAttribute("start-time", now),
                    Finish = testNode.GetAttribute("end-time", now)
                };
                var test = new TestRun
                {
                    Name = testNode.GetAttribute("name"),
                    FullName = fullName,
                    TestInfo = ti,
                    TestType = testType,
                    Priority = priority,
                    Categories = categories,
                    Result = r != null ? (l != null ? $"{r}: {l}" : r) : "Unknown",
                    TestDuration = testNode.GetAttribute("duration", 0.0),
                    Output = testNode.SelectSingleNode(".//output")?.InnerText ?? "",
                    TestMessage = testNode.SelectSingleNode(".//message")?.InnerText ?? "",
                    TestStackTrace = testNode.SelectSingleNode(".//stack-trace")?.InnerText ?? "",
                    Screenshots = screens ?? new List<ITestScreenshot>()
                };
                return test;
            }
            catch (Exception ex)
            {
                var log = new Core.Utils.Log(GhprEventListener.Settings.OutputPath);
                log.Exception(ex, "Exception in GetTestRun");
                return new TestRun();
            }
        }
    }
}