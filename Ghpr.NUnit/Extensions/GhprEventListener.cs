using Ghpr.Core;
using Ghpr.NUnit.Utils;
using NUnit.Engine;
using NUnit.Engine.Extensibility;
using NUnit.Engine.Internal;

namespace Ghpr.NUnit.Extensions
{
    [Extension(Path = "/NUnit/Engine/TypeExtensions/ITestEventListener")]
    public class GhprEventListener : ITestEventListener
    {
        private readonly Reporter _reporter = new Reporter();
        
        public void OnTestEvent(string report)
        {
            var xmlNode = XmlHelper.CreateXmlNode(report);

            switch (xmlNode.Name)
            {
                case "start-run":
                    _reporter.RunStarted();
                    break;
                    
                case "test-run":
                    _reporter.RunFinished();
                    break;

                case "start-test":
                    _reporter.TestStarted(TestRunHelper.GetTestRun(xmlNode));
                    break;

                case "test-case":
                    _reporter.TestFinished(TestRunHelper.GetTestRun(xmlNode));
                    break;
                    
            }
        }
    }
}