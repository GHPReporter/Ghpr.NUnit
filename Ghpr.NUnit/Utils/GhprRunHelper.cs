using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Ghpr.Core;
using Ghpr.Core.Enums;
using Ghpr.NUnit.Extensions;
using NUnit;

namespace Ghpr.NUnit.Utils
{
    public class GhprRunHelper
    {
        public static void CreateReportFromFile(string path)
        {
            try
            {
                var reporter = new Reporter(TestingFramework.NUnit);
                var xmlString = File.ReadAllText(path);
                var doc = new XmlDocument();
                doc.LoadXml(xmlString);
                XmlNode node = doc.DocumentElement;
                var testCases = node?.SelectNodes(".//*/test-case")?.Cast<XmlNode>();
                if (testCases == null)
                {
                    return;
                }
                var testRuns = testCases.Select(TestRunHelper.GetTestRun).ToList();
                reporter.GenerateFullReport(testRuns);
            }
            catch (Exception ex)
            {
                var log = new Core.Utils.Log(GhprEventListener.OutputPath);
                log.Exception(ex, "Exception in CreateReportFromFile");
            }
        }
    }
}