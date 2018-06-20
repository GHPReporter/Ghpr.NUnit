using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Ghpr.Core.Common;
using Ghpr.Core.Extensions;
using Ghpr.Core.Interfaces;
using Ghpr.Core.Utils;
using NUnit;

namespace Ghpr.NUnit.Utils
{
    public static class TestRunHelper
    {
        public static KeyValuePair<TestRunDto, TestOutputDto> GetTestRunOnStarted(XmlNode testNode, DateTime startDateTime, ILogger logger)
        {
            var testRun = GetTestRun(testNode, logger);
            testRun.TestInfo.Start = startDateTime;
            return new KeyValuePair<TestRunDto, TestOutputDto>(testRun, new TestOutputDto());
        }

        public static KeyValuePair<TestRunDto, TestOutputDto> GetTestRunOnFinished(XmlNode testNode, DateTime finishDateTime, ILogger logger)
        {
            var testRun = GetTestRun(testNode, logger);
            testRun.TestInfo.Finish = finishDateTime;
            var testOutput = GetTestOutput(testNode, finishDateTime, logger);
            return new KeyValuePair<TestRunDto,TestOutputDto>(testRun, testOutput);
        }

        public static KeyValuePair<TestRunDto, TestOutputDto> GetTestAndOutput(XmlNode testNode, ILogger logger)
        {
            var testRun = GetTestRun(testNode, logger);
            var testOutput = GetTestOutput(testNode, testRun.TestInfo.Finish, logger);
            return new KeyValuePair<TestRunDto, TestOutputDto>(testRun, testOutput);
        }

        public static TestOutputDto GetTestOutput(XmlNode testNode, DateTime testFinishDate, ILogger logger)
        {
            var output = new TestOutputDto
            {
                FeatureOutput = "",
                Output = testNode.SelectSingleNode(".//output")?.InnerText ?? "",
                TestOutputInfo = new SimpleItemInfoDto
                {
                    Date = testFinishDate
                }
            };
            return output;
        }

        public static TestRunDto GetTestRun(XmlNode testNode, ILogger logger)
        {
            try
            {
                var now = DateTime.Now;
                var guid = testNode.SelectSingleNode("properties/property[@name='TestGuid']")?.GetAttribute("value");
                var testType = testNode.SelectSingleNode("properties/property[@name='TestType']")?.GetAttribute("value");
                var priority = testNode.SelectSingleNode("properties/property[@name='Priority']")?.GetAttribute("value");
                var description = testNode.SelectSingleNode("properties/property[@name='Description']")?.GetAttribute("value");
                var categories = testNode.SelectNodes("properties/property[@name='Category']")?.Cast<XmlNode>()
                    .Select(n => n.GetAttribute("value")).ToArray();

                var testDataDateTimes = testNode.SelectNodes(
                        $"properties/property[contains(@name,'{Paths.Names.TestDataDateTimeKeyTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.GetAttribute("value")).ToList();
                var testDataActuals = testNode.SelectNodes(
                        $"properties/property[contains(@name,'{Paths.Names.TestDataActualKeyTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.GetAttribute("value")).ToArray();
                var testDataExpecteds = testNode.SelectNodes(
                        $"properties/property[contains(@name,'{Paths.Names.TestDataExpectedKeyTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.GetAttribute("value")).ToArray();
                var testDataComments = testNode.SelectNodes(
                        $"properties/property[contains(@name,'{Paths.Names.TestDataCommentKeyTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.GetAttribute("value")).ToArray();
                var testData = new List<TestDataDto>();
                for (var i = 0; i < testDataDateTimes?.Count; i++)
                {
                    testData.Add(new TestDataDto
                    {
                        TestDataInfo = new SimpleItemInfoDto
                        {
                            Date = DateTime.ParseExact(testDataDateTimes?[i], "yyyyMMdd_HHmmssfff", CultureInfo.InvariantCulture),
                            ItemName = "Test Data"
                        },
                        Actual = testDataActuals?[i],
                        Expected = testDataExpecteds?[i],
                        Comment = testDataComments?[i]
                    });
                }
                var r = testNode.GetAttribute("result");
                var l = testNode.GetAttribute("label");
                var fullName = testNode.GetAttribute("fullname");
                var testGuid = guid != null ? Guid.Parse(guid) : fullName.ToMd5HashGuid();
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
                var ti = new ItemInfoDto
                {
                    Guid = testGuid,
                    Start = testNode.GetAttribute("start-time", now),
                    Finish = testNode.GetAttribute("end-time", now)
                };
                var test = new TestRunDto
                {
                    Name = name,
                    FullName = fullName,
                    Description = description == "null" ? "" : description,
                    TestInfo = ti,
                    TestType = testType,
                    Priority = priority,
                    Categories = categories,
                    Result = r != null ? (l != null ? $"{r}: {l}" : r) : "Unknown",
                    Output = new SimpleItemInfoDto
                    {
                        Date = ti.Finish,
                        ItemName = "Test Output"
                    },
                    TestMessage = testNode.SelectSingleNode(".//message")?.InnerText ?? "",
                    TestStackTrace = testNode.SelectSingleNode(".//stack-trace")?.InnerText ?? "",
                    Screenshots = new List<SimpleItemInfoDto>(),//new List<TestScreenshotDto>(),
                    TestData = testData ?? new List<TestDataDto>()
                };
                return test;
            }
            catch (Exception ex)
            {
                logger.Exception("Exception in GetTestRun", ex);
                return new TestRunDto();
            }
        }
    }
}