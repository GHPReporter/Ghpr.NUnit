using Ghpr.NUnit.Utils;

namespace ConsoleAppForDebug
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GhprNUnitRunHelper.CreateReportFromFile(args[0]);
        }
    }
}
