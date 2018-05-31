using System;
using Ghpr.Core.Enums;
using Ghpr.Core.Factories;
using Ghpr.Core.Interfaces;
using Ghpr.Core.Utils;
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
            StaticLog.Initialize(Reporter.ReporterSettings.OutputPath);
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
                    break;
                }
                case "test-run":
                {
                    Reporter.RunFinished();
                    break;
                }
                case "start-test":
                {
                    var testRunDto = TestRunHelper.GetTestRunOnStarted(xmlNode);
                    testRunDto.TestInfo.Start = eventTime;
                    Reporter.TestStarted(TestRunHelper.GetTestRunOnStarted(xmlNode));
                    break;
                }
                case "test-case":
                {
                    var testRunDto = TestRunHelper.GetTestRunOnFinished(xmlNode);
                    testRunDto.TestInfo.Finish = eventTime;
                    Reporter.TestFinished(testRunDto);
                    break;
                }
            }
        }
    }
}