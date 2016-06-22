using System;
using System.Xml;
using Ghpr.Core.Common;
using Ghpr.Core.Interfaces;
using NUnit.Engine.Internal;

namespace Ghpr.NUnit.Utils
{
    public static class TestRunHelper
    {
        public static ITestRun GetTestRun(XmlNode testNode)
        {
            try
            {
                var guid = testNode.SelectSingleNode("properties/property[@name='TestGuid']")?.GetAttribute("value");
                var r = testNode.GetAttribute("result");
                var l = testNode.GetAttribute("label");

                var test = new TestRun
                {
                    Name = testNode.GetAttribute("name") ?? "",
                    FullName = testNode.GetAttribute("fullname") ?? "",
                    TestGuid = guid != null ? Guid.Parse(guid) : Guid.Empty,
                    Result = r != null ? (l != null ? $"{r}: {l}" : r) : "Unknown",
                    DateTimeStart = testNode.GetAttribute("start-time", default(DateTime)),
                    DateTimeFinish = testNode.GetAttribute("end-time", default(DateTime)),
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