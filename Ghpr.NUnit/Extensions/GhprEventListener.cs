using System;
using System.Collections.Generic;
using Ghpr.Core.Common;
using Ghpr.Core.Enums;
using Ghpr.Core.Factories;
using Ghpr.Core.Interfaces;
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

        static GhprEventListener()
        {
            Reporter = ReporterFactory.Build(TestingFramework.NUnit, new TestDataProvider());
        }

        public void OnTestEvent(string report)
        {
            var eventTime = DateTime.Now;
            var xmlNode = XmlHelper.CreateXmlNode(report);

            switch (xmlNode.Name)
            {
                case "start-run":
                {
                    Reporter.RunStarted();
                    _finishedTestInfoDtos = new List<ItemInfoDto>();
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
                    break;
                }
                case "test-suite":
                {
                    var featureOutputData = TestRunHelper.GetOutputsFromSuite(xmlNode, _finishedTestInfoDtos);
                    foreach (var data in featureOutputData)
                    {
                        Reporter.DataService.UpdateTestOutput(data.Key, data.Value);
                    }
                    break;
                }
                case "test-run":
                {
                    Reporter.RunFinished();
                    Reporter.TearDown();
                    break;
                }
            }
        }
    }
}