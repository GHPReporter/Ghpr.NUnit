using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ghpr.Core.Common;
using Ghpr.Core.Enums;
using Ghpr.Core.Factories;
using Ghpr.Core.Interfaces;
using Ghpr.NUnit.Common;

namespace Ghpr.NUnit.Utils
{
    public class GhprNUnitRunHelper
    {
        public static void CreateReportFromFile(string path)
        {
            IReporter reporter = null;
            try
            {
                reporter = ReporterFactory.Build(TestingFramework.NUnit, new TestDataProvider());
                var testRuns = GetTestRunsListFromFile(path, reporter.Logger);
                foreach (var ghprTestCase in testRuns.Where(t => t.GhprTestScreenshots.Any()))
                {
                    foreach (var screenshot in ghprTestCase.GhprTestScreenshots)
                    {
                        reporter.DataService.SaveScreenshot(screenshot);
                    }
                }
                reporter.GenerateFullReport(testRuns.Select(tr => new KeyValuePair<TestRunDto, TestOutputDto>(tr.GhprTestRun, tr.GhprTestOutput)).ToList());
                reporter.TearDown();
            }
            catch (Exception ex)
            {
                reporter?.Logger.Exception("Exception in CreateReportFromFile", ex);
            }
        }

        public static List<GhprTestCase> GetTestRunsListFromFile(string path, ILogger logger)
        {
            try
            {
                var xmlString = File.ReadAllText(path);
                var doc = new XmlDocument();
                doc.LoadXml(xmlString);
                XmlNode node = doc.DocumentElement;
                var testCases = node?.SelectNodes(".//*/test-case")?.Cast<XmlNode>().ToList();
                var list = testCases?.Select(n => TestRunHelper.GetTestAndOutput(n, logger)).ToList() ?? new List<GhprTestCase>();
                var testSuites = node?.SelectNodes(".//*/test-suite")?.Cast<XmlNode>().ToList() ?? new List<XmlNode>();
                var testInfoDtos = list.Select(d => d.GhprTestRun.TestInfo).ToList();
                foreach (var testSuite in testSuites)
                {
                    var testOutputs = TestRunHelper.GetOutputsFromSuite(testSuite, testInfoDtos);
                    foreach (var output in testOutputs)
                    {
                        var test = list.FirstOrDefault(t => t.GhprTestRun.TestInfo.Guid == output.Key.Guid
                                                            && t.GhprTestRun.TestInfo.Finish == output.Key.Finish) ?? new GhprTestCase();
                        test.GhprTestOutput.Output = output.Value.Output;
                        test.GhprTestOutput.SuiteOutput = output.Value.SuiteOutput;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                logger.Exception("Exception in GetTestRunsListFromFile", ex);
                return null;
            }
        }
    }
}