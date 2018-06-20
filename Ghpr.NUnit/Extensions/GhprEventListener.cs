using System;
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
                    break;
                }
                case "test-run":
                {
                    Reporter.RunFinished();
                    Reporter.TearDown();
                    break;
                }
                default:
                {

                    break;
                }
            }
        }
    }
}