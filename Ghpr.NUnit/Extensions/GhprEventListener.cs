using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Ghpr.Core.Common;
using Ghpr.Core.Enums;
using Ghpr.Core.Factories;
using Ghpr.Core.Interfaces;
using Ghpr.NUnit.Common;
using Ghpr.NUnit.Utils;
using NUnit;
using NUnit.Engine;
using NUnit.Engine.Extensibility;

namespace Ghpr.NUnit.Extensions
{
    [Extension(Description = "Ghpr NUnit Extension")]
    public class GhprEventListener : ITestEventListener
    {
        internal static readonly IReporter Reporter;
        public static string OutputPath => Reporter.ReporterSettings.OutputPath;
        private List<ItemInfoDto> _finishedTestInfoDtos;
        private List<NUnitTestCase> _testCases;
        private List<NUnitTestSuite> _testSuites;

        static GhprEventListener()
        {
            Reporter = ReporterFactory.Build(TestingFramework.NUnit, new TestDataProvider());
        }

        public void OnTestEvent(string report)
        {
            var eventTime = DateTime.Now;
            var xmlNode = XmlHelper.CreateXmlNode(report);

            Reporter.Logger.Warn(report);
            switch (xmlNode.Name)
            {
                case "start-run":
                {
                    Reporter.RunStarted();
                    _finishedTestInfoDtos = new List<ItemInfoDto>();
                    _testCases = new List<NUnitTestCase>();
                    _testSuites = new List<NUnitTestSuite>();
                    break;
                }
                case "start-test":
                {
                    var testRun = TestRunHelper.GetTestRunOnStarted(xmlNode, eventTime, Reporter.Logger);
                    Reporter.TestStarted(testRun.Key);
                    break;
                }
                case "test-case":
                {
                    var testRun = TestRunHelper.GetTestRunOnFinished(xmlNode, eventTime, Reporter.Logger);
                    Reporter.TestFinished(testRun.Key, testRun.Value);
                    _finishedTestInfoDtos.Add(testRun.Key.TestInfo);
                    _testCases.Add(new NUnitTestCase
                    {
                        GhprTestInfo = testRun.Key.TestInfo,
                        GhprTestOutput = testRun.Value,
                        Id = "",
                        ParentId = ""
                    });
                    break;
                }
                case "test-suite":
                {
                    //var featureOutputData = TestRunHelper.GetOutputsFromSuite(xmlNode, _finishedTestInfoDtos);
                    //foreach (var data in featureOutputData)
                    //{
                    //    Reporter.DataService.UpdateTestOutput(data.Key, data.Value);
                    //}
                    break;
                }
                case "test-run":
                {
                    var testSuites = xmlNode.SelectNodes(".//*/test-suite")?.Cast<XmlNode>().ToList() ?? new List<XmlNode>();
                    foreach (var testSuite in testSuites)
                    {
                        var testOutputs = TestRunHelper.GetOutputsFromSuite(testSuite, _finishedTestInfoDtos);
                        foreach (var data in testOutputs)
                        {
                            Reporter.DataService.UpdateTestOutput(data.Key, data.Value);    
                        }
                    }
                    Reporter.RunFinished();
                    Reporter.TearDown();
                    break;
                }
            }
        }
    }
}