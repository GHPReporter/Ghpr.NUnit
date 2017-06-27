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

namespace Ghpr.NUnit.Utils
{
    public static class TestRunHelper
    {

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
                var screenNames = testNode.SelectNodes(
                        $"properties/property[contains(@name,'{ScreenHelper.ScreenKeyTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.GetAttribute("value")).ToArray();
                var screens = screenNames?.Select(screenName => new TestScreenshot(screenName))
                    .Cast<ITestScreenshot>().ToList();
                var r = testNode.GetAttribute("result");
                var l = testNode.GetAttribute("label");
                var fullName = testNode.GetAttribute("fullname");
                var name = testNode.GetAttribute("name");
                if (fullName.Contains(name))
                {
                    var ns = fullName.Substring(0, fullName.LastIndexOf(name, StringComparison.Ordinal) - 1);
                    if (ns.Contains("(") && ns.Contains(")"))
                    {
                        var i1 = ns.IndexOf("(", StringComparison.Ordinal);
                        var i2 = ns.IndexOf(")", StringComparison.Ordinal);
                        ns = ns.Substring(0, i1) + ns.Substring(i2 + 1);
                        fullName = ns + "." + name;
                    }
                }

                var ti = new ItemInfo
                {
                    Guid = guid != null ? Guid.Parse(guid) : GuidConverter.ToMd5HashGuid(fullName),
                    Start = testNode.GetAttribute("start-time", now),
                    Finish = testNode.GetAttribute("end-time", now)
                };
                var test = new TestRun
                {
                    Name = name,
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
                var log = new Core.Utils.Log(GhprEventListener.OutputPath);
                log.Exception(ex, "Exception in GetTestRun");
                return new TestRun();
            }
        }
    }
}