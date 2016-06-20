using Ghpr.Core;
using NUnit.Engine;
using NUnit.Engine.Extensibility;
using NUnit.Engine.Internal;

namespace Ghpr.NUnit.Extensions
{
    [Extension]
    public class GhprEventListener : ITestEventListener
    {
        public void OnTestEvent(string report)
        {
            var xmlNode = XmlHelper.CreateXmlNode(report);

            switch (xmlNode.Name)
            {
                case "start-run":
                    Reporter.RunStarted();
                    break;
                    
                case "test-run":
                    Reporter.RunFinished();
                    break;
            }
        }
    }
}