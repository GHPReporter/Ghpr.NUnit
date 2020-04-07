using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Ghpr.Core.Common;
using Ghpr.Core.Extensions;
using Ghpr.Core.Interfaces;
using Ghpr.Core.Providers;
using Ghpr.Core.Utils;
using Ghpr.NUnit.Common;
using Ghpr.NUnit.Extensions;
using NUnit;

namespace Ghpr.NUnit.Utils
{
    public static class TestRunHelper
    {
        public static GhprTestCase GetTestRunOnStarted(XmlNode testNode, DateTime startDateTime, ILogger logger)
        {
            var testCase = GetTestRun(testNode, logger);
            testCase.GhprTestRun.TestInfo.Start = startDateTime;
            return testCase;
        }

        public static GhprTestCase GetTestRunOnFinished(XmlNode testNode, DateTime finishDateTime, ILogger logger)
        {
            var testCase = GetTestRun(testNode, logger);
            testCase.GhprTestRun.TestInfo.Finish = finishDateTime;
            return testCase;
        }

        public static GhprTestCase GetTestAndOutput(XmlNode testNode, ILogger logger)
        {
            var testRun = GetTestRun(testNode, logger);
            return testRun;
        }

        public static TestOutputDto GetTestOutput(XmlNode testNode, DateTime testFinishDate, ILogger logger)
        {
            var output = new TestOutputDto
            {
                SuiteOutput = "",
                Output = testNode.SelectSingleNode("./output")?.InnerText ?? "",
                TestOutputInfo = new SimpleItemInfoDto
                {
                    Date = testFinishDate,
                    ItemName = NamesProvider.GetTestOutputFileName(testFinishDate)
                }
            };
            return output;
        }

        public static GhprTestSuite GetTestSuite(XmlNode suiteNode)
        {
            var suiteOutput = suiteNode.SelectSingleNode("./output")?.InnerText ?? "";
            var id = suiteNode.GetAttribute("id") ?? "";
            var parentId = suiteNode.GetAttribute("parentId") ?? "";
            var type = suiteNode.GetAttribute("type") ?? "";
            var ts = new GhprTestSuite
            {
                Id = id,
                ParentId = parentId,
                Output = suiteOutput,
                Type = type
            };
            return ts;
        }

        public static List<KeyValuePair<ItemInfoDto, TestOutputDto>> GetOutputsFromSuite(XmlNode suiteNode, List<ItemInfoDto> finishedTestInfoDtos)
        {
            var suiteOutput = suiteNode.SelectSingleNode("./output")?.InnerText ?? "";
            var res = new List<KeyValuePair<ItemInfoDto, TestOutputDto>>();
            var testNodes = suiteNode.SelectNodes("./test-case")?.Cast<XmlNode>().ToList() ?? new List<XmlNode>();
            if (!testNodes.Any() || suiteOutput.Equals(""))
            {
                return res;
            }
            foreach (var testNode in testNodes)
            {
                var testGuid = GetTestGuid(testNode);
                var testInfoDto = finishedTestInfoDtos.FirstOrDefault(i => i.Guid.Equals(testGuid));
                var testOutputDto = new TestOutputDto
                {
                    TestOutputInfo = new SimpleItemInfoDto(),
                    SuiteOutput = suiteOutput,
                    Output = testNode.SelectSingleNode("./output")?.InnerText ?? ""
                };
                var data = new KeyValuePair<ItemInfoDto, TestOutputDto> (testInfoDto, testOutputDto);
                res.Add(data);
            }
            return res;
        }

        private static Guid GetTestGuid(XmlNode testNode)
        {
            var guid = testNode.SelectSingleNode("properties/property[@name='TestGuid']")?.Val();
            return guid != null ? Guid.Parse(guid) : testNode.GetAttribute("fullname").ToMd5HashGuid();
        }

        public static GhprTestCase GetTestRun(XmlNode testNode, ILogger logger)
        {
            try
            {
                var now = DateTime.UtcNow;
                var testType = testNode.SelectSingleNode("properties/property[@name='TestType']")?.Val();
                var priority = testNode.SelectSingleNode("properties/property[@name='Priority']")?.Val();
                var description = testNode.SelectSingleNode("properties/property[@name='Description']")?.Val();
                var categories = testNode.SelectNodes("properties/property[@name='Category']")?.Cast<XmlNode>()
                    .Select(n => n.Val()).ToArray();

                var testDataDateTimes = testNode.SelectNodes(
                        $"properties/property[contains(@name,'{Paths.Names.TestDataDateTimeKeyTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.Val()).ToList();
                var testDataActuals = testNode.SelectNodes(
                        $"properties/property[contains(@name,'{Paths.Names.TestDataActualKeyTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.Val()).ToArray();
                var testDataExpecteds = testNode.SelectNodes(
                        $"properties/property[contains(@name,'{Paths.Names.TestDataExpectedKeyTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.Val()).ToArray();
                var testDataComments = testNode.SelectNodes(
                        $"properties/property[contains(@name,'{Paths.Names.TestDataCommentKeyTemplate}')]")?
                    .Cast<XmlNode>()
                    .Select(n => n.Val()).ToArray();
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
                var testGuid = GetTestGuid(testNode);
                var name = testNode.GetAttribute("name");
                var duration = double.Parse(testNode.GetAttribute("duration") ?? "0.0", CultureInfo.InvariantCulture);
                var id = testNode.GetAttribute("id") ?? "";
                var parentId = testNode.GetAttribute("parentId") ?? "";
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
                    Duration = duration,
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
                    TestMessage = testNode.SelectSingleNode("./failure/message")?.InnerText ?? "",
                    TestStackTrace = testNode.SelectSingleNode("./failure/stack-trace")?.InnerText ?? "",
                    Screenshots = new List<SimpleItemInfoDto>(),
                    TestData = testData ?? new List<TestDataDto>()
                };

                var imageAttachments = testNode.SelectNodes(".//attachments/attachment/filePath")?
                    .Cast<XmlNode>().Select(n => n.InnerText).Where(t => t.EndsWithImgExtension()).ToList();
                
                var testScreenshots = new List<TestScreenshotDto>();
                foreach (var imageAttachment in imageAttachments.Where(File.Exists))
                {
                    var ext = Path.GetExtension(imageAttachment);
                    var fileInfo = new FileInfo(imageAttachment);
                    var bytes = File.ReadAllBytes(imageAttachment);
                    var base64 = Convert.ToBase64String(bytes);
                    var screenInfo = new SimpleItemInfoDto
                    {
                        Date = fileInfo.CreationTimeUtc,
                        ItemName = ""
                    };
                    var testScreenshotDto = new TestScreenshotDto
                    {
                        Format = ext.Replace(".", ""),
                        TestGuid = testGuid,
                        TestScreenshotInfo = screenInfo,
                        Base64Data = base64
                    };
                    testScreenshots.Add(testScreenshotDto);
                    test.Screenshots.Add(testScreenshotDto.TestScreenshotInfo);
                }
                
                var ghprTestCase = new GhprTestCase
                {
                    Id = id,
                    ParentId = parentId,
                    GhprTestRun = test,
                    GhprTestOutput = GetTestOutput(testNode, test.TestInfo.Finish, logger),
                    GhprTestScreenshots = testScreenshots
                };
                
                return ghprTestCase;
            }
            catch (Exception ex)
            {
                logger.Exception($"Exception in GetTestRun: {ex.Message}{Environment.NewLine}{ex.StackTrace}", ex);
                return new GhprTestCase();
            }
        }
    }
}