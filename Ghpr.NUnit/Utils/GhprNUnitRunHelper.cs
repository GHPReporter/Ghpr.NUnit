using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ghpr.Core.Common;
using Ghpr.Core.Enums;
using Ghpr.Core.Factories;
using Ghpr.Core.Interfaces;

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
                reporter.GenerateFullReport(testRuns);
                reporter.TearDown();
            }
            catch (Exception ex)
            {
                reporter?.Logger.Exception("Exception in CreateReportFromFile", ex);
            }
        }

        public static List<KeyValuePair<TestRunDto, TestOutputDto>> GetTestRunsListFromFile(string path, ILogger logger)
        {
            try
            {
                var xmlString = File.ReadAllText(path);
                var doc = new XmlDocument();
                doc.LoadXml(xmlString);
                XmlNode node = doc.DocumentElement;
                var testCases = node?.SelectNodes(".//*/test-case")?.Cast<XmlNode>().ToList();
                var list = testCases?.Select(n => TestRunHelper.GetTestAndOutput(n, logger)).ToList() ?? new List<KeyValuePair<TestRunDto, TestOutputDto>>();
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