using System;
using System.Xml;
using Ghpr.Core.Common;
using Ghpr.Core.Interfaces;
using NUnit.Engine.Internal;

namespace Ghpr.NUnit.Utils
{
    public static class TestRunHelper
    {
        public static ITestRun GetTestRun(XmlNode finishNode)
        {
            var test = new TestRun();
            try
            {
                var guid = finishNode.SelectSingleNode("properties/property[@name='TestGuid']")?.GetAttribute("value");
                test.Name = finishNode.GetAttribute("name") ?? "";
                test.FullName = finishNode.GetAttribute("fullname") ?? "";
                test.Guid = guid != null ? Guid.Parse(guid) : Guid.Empty;
                test.Result = finishNode.GetAttribute("result") ?? "Unknown";
            }
            catch (Exception ex)
            {
                Core.Utils.Log.Exception(ex, "Exception in GetTestRun");
            }
            return test;
        }
    }
}