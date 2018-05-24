using Ghpr.NUnit.Extensions;

namespace Ghpr.NUnit.Utils
{
    public static class ScreenHelper
    {
        public static void SaveScreenshot(byte[] screenshotBytes)
        {
            GhprEventListener.Reporter.SaveScreenshot(screenshotBytes);
        }
    }
}