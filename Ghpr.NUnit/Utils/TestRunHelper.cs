using System;
using System.Linq;
using System.Xml;
using Ghpr.Core.Common;
using Ghpr.Core.Interfaces;
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
                var categories = testNode.SelectNodes("properties/property[@name='Category']")?.Cast<XmlNode>().Select(n => n.GetAttribute("value")).ToArray();
                var r = testNode.GetAttribute("result");
                var l = testNode.GetAttribute("label");

                var test = new TestRun
                {
                    Name = testNode.GetAttribute("name") ?? "",
                    FullName = testNode.GetAttribute("fullname") ?? "",
                    TestInfo = new ItemInfo
                    {
                        Guid = guid != null ? Guid.Parse(guid) : Guid.Empty,
                        Start = testNode.GetAttribute("start-time", now),
                        Finish = testNode.GetAttribute("end-time", now)
                    },
                    TestType = testType,
                    Priority = priority,
                    Categories = categories,
                    Result = r != null ? (l != null ? $"{r}: {l}" : r) : "Unknown",
                    TestDuration = testNode.GetAttribute("duration", 0.0),
                    Output = testNode.SelectSingleNode(".//output")?.InnerText ?? "",
                    TestMessage = testNode.SelectSingleNode(".//message")?.InnerText ?? "",
                    TestStackTrace = testNode.SelectSingleNode(".//stack-trace")?.InnerText ?? ""
                };

                return test;
            }
            catch (Exception ex)
            {
                Core.Utils.Log.Exception(ex, "Exception in GetTestRun");
                return new TestRun();
            }
        }
    }
}