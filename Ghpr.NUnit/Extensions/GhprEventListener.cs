using Ghpr.Core;
using Ghpr.NUnit.Utils;
using NUnit;
using NUnit.Engine;
using NUnit.Engine.Extensibility;

namespace Ghpr.NUnit.Extensions
{
    [Extension(Description = "Test Reporter Extension", EngineVersion = "3.5")]
    public class GhprEventListener : ITestEventListener
    {
        public GhprEventListener()
        {
            _reporter = new Reporter();
        }
        
        private static Reporter _reporter;
        public static string OutputPath => _reporter.OutputPath;

        public void OnTestEvent(string report)
        {
            var xmlNode = XmlHelper.CreateXmlNode(report);

            switch (xmlNode.Name)
            {
                case "start-run":
                {
                    _reporter.RunStarted();
                    break;
                }
                case "test-run":
                {
                    _reporter.RunFinished();
                    break;
                }
                case "start-test":
                {
                    _reporter.TestStarted(TestRunHelper.GetTestRun(xmlNode));
                    break;
                }
                case "test-case":
                {
                    _reporter.TestFinished(TestRunHelper.GetTestRun(xmlNode));
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