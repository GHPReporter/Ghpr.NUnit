using System;
using System.Collections.Generic;
using System.Linq;
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
        internal static IReporter Reporter;
        public static string OutputPath => Reporter.ReporterSettings.OutputPath;
        private List<GhprTestCase> _testCases;
        private List<GhprTestSuite> _testSuites;

        public void OnTestEvent(string report)
        {
            var eventTime = DateTime.Now;
            var xmlNode = XmlHelper.CreateXmlNode(report);

            switch (xmlNode.Name)
            {
                case "start-run":
                {
                    var cmd = Environment.CommandLine;
                    var args = Environment.GetCommandLineArgs();
                    var projName = args.Length >= 2 ? args[1] : cmd;
                    Reporter = ReporterFactory.Build(TestingFramework.NUnit, new TestDataProvider(), projName);
                    Reporter.RunStarted();
                    _testCases = new List<GhprTestCase>();
                    _testSuites = new List<GhprTestSuite>();
                    break;
                }
                case "start-test":
                {
                    var testRun = TestRunHelper.GetTestRunOnStarted(xmlNode, eventTime, Reporter.Logger);
                    Reporter.TestStarted(testRun.GhprTestRun);
                    break;
                }
                case "test-case":
                {
                    var testCase = TestRunHelper.GetTestRunOnFinished(xmlNode, eventTime, Reporter.Logger);
                    testCase.GhprTestOutput.TestOutputInfo.Date = eventTime;
                    foreach (var screenshot in testCase.GhprTestScreenshots)
                    {
                        var testScreenshotInfo = Reporter.DataWriterService.SaveScreenshot(screenshot);
                        testCase.GhprTestRun.Screenshots.Add(testScreenshotInfo);
                    }
                    Reporter.TestFinished(testCase.GhprTestRun, testCase.GhprTestOutput);
                    _testCases.Add(testCase);
                    break;
                }
                case "test-suite":
                {
                    var testSuite = TestRunHelper.GetTestSuite(xmlNode);
                    _testSuites.Add(testSuite);
                    var tests = _testCases.Where(tc => tc.ParentId.Equals(testSuite.Id) && !tc.ParentId.Equals(""))
                        .ToList();
                    var childSuites = _testSuites
                        .Where(ts => ts.ParentId.Equals(testSuite.Id) && !ts.ParentId.Equals("") && ts.Type.Equals("ParameterizedFixture")).ToList();
                    foreach (var suite in childSuites)
                    {
                        tests.AddRange(_testCases.Where(tc => tc.ParentId.Equals(suite.Id) && !tc.ParentId.Equals("")));
                    }
                    foreach (var test in tests)
                    {
                        test.GhprTestOutput.SuiteOutput = testSuite.Output;
                        Reporter.DataWriterService.UpdateTestOutput(test.GhprTestRun.TestInfo, test.GhprTestOutput);
                    }
                    
                    break;
                }
                case "test-run":
                {
                    Reporter.RunFinished();
                    Reporter.CleanUpJob();
                    Reporter.TearDown();
                    break;
                }
            }
        }
    }
}