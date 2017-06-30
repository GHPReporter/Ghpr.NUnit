using Ghpr.Core;
using Ghpr.Core.Enums;
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
        private static readonly Reporter Reporter;
        public static string OutputPath => Reporter.Settings.OutputPath;

        static GhprEventListener()
        {
            Reporter = new Reporter(TestingFramework.NUnit);
            StaticLog.Initialize(Reporter.Settings.OutputPath);
        }

        public void OnTestEvent(string report)
        {
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
                    Reporter.TestStarted(TestRunHelper.GetTestRun(xmlNode));
                    break;
                }
                case "test-case":
                {
                    Reporter.TestFinished(TestRunHelper.GetTestRun(xmlNode));
                    break;
                }
                default:
                {
                    //Log.Warning($"Unknown XML Node! Node name: '{xmlNode.Name}'");
                    break;
                }
            }
        }
    }
}