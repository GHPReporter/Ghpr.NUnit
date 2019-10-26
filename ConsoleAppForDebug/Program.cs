using Ghpr.NUnit.Utils;

namespace ConsoleAppForDebug
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[]
                {"C:\\Users\\Evgeniy.Kosjakov\\Downloads\\AutomationTestResults_A_00_10232019_151438.xml"};
            GhprNUnitRunHelper.CreateReportFromFile(args[0]);
        }
    }
}
