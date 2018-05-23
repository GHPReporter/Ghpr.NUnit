using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ghpr.Core.Common;
using Ghpr.Core.Enums;
using Ghpr.Core.Factories;
using Ghpr.NUnit.Extensions;

namespace Ghpr.NUnit.Utils
{
    public class GhprNUnitRunHelper
    {
        public static void CreateReportFromFile(string path)
        {
            try
            {
                var reporter = ReporterFactory.Build(TestingFramework.NUnit, new TestDataProvider());
                var testRuns = GetTestRunsListFromFile(path);
                reporter.GenerateFullReport(testRuns);
            }
            catch (Exception ex)
            {
                var log = new Core.Utils.Log(GhprEventListener.OutputPath);
                log.Exception(ex, "Exception in CreateReportFromFile");
            }
        }

        public static List<TestRunDto> GetTestRunsListFromFile(string path)
        {
            try
            {
                var xmlString = File.ReadAllText(path);
                var doc = new XmlDocument();
                doc.LoadXml(xmlString);
                XmlNode node = doc.DocumentElement;
                var testCases = node?.SelectNodes(".//*/test-case")?.Cast<XmlNode>().ToList();
                var list = testCases?.Select(TestRunHelper.GetTestRun).ToList() ?? new List<TestRunDto>();
                return list;
            }
            catch (Exception ex)
            {
                var log = new Core.Utils.Log(GhprEventListener.OutputPath);
                log.Exception(ex, "Exception in GetTestRunsListFromFile");
                return null;
            }
        }
    }
}