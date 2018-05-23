using Ghpr.Core.Interfaces;

namespace Ghpr.NUnit.Utils
{
    public static class ScreenHelper
    {
        private static IReporter _reporter;

        public static void Init(IReporter reporter)
        {
            _reporter = reporter;
        }
        
        public static void SaveScreenshot(byte[] screenshotBytes)
        {
            _reporter.SaveScreenshot(screenshotBytes);
        }
    }
}