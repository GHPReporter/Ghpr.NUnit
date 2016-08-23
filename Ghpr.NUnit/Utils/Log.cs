using System;
using System.IO;

namespace Ghpr.NUnit.Utils
{
    public static class Log
    {
        private const string LogFile = @"Ghpr.NUnit.Log.txt";
        private const string Output = @"C:\_GhprReport";

        private static void WriteToFile(string msg, string fileName)
        {
            Directory.CreateDirectory(Output);
            using (var sw = File.AppendText(Path.Combine(Output, fileName)))
            {
                try
                {
                    var logLine = $"{DateTime.Now:G}: {msg}";
                    sw.WriteLine(logLine);
                }
                finally
                {
                    sw.Close();
                }
            }
        }

        public static void Write(string msg)
        {
            Directory.CreateDirectory(Output);
            using (var sw = File.AppendText(Path.Combine(Output, LogFile)))
            {
                try
                {
                    var logLine = $"{DateTime.Now:G}: {msg}";
                    sw.WriteLine(logLine);
                }
                catch (Exception ex)
                {
                    Exception(ex);
                }
                finally
                {
                    sw.Close();
                }
            }
        }

        public static void Exception(Exception exception, string exceptionMessage = "")
        {
            var msg = (exceptionMessage.Equals("") ? "Exception!" : exceptionMessage) + Environment.NewLine
                + " Message: " + Environment.NewLine + exception.Message + Environment.NewLine +
                "StackTrace: " + Environment.NewLine + exception.StackTrace;
            var inner = exception.InnerException;
            while (inner != null)
            {
                msg = msg + Environment.NewLine + " Inner Exception: " + Environment.NewLine +
                    inner.Message + Environment.NewLine +
                "StackTrace: " + Environment.NewLine + inner.StackTrace;
                inner = inner.InnerException;
            }
            WriteToFile(msg, "Exception_" + DateTime.Now.ToString("ddMMyyHHmmssfff") + ".txt");
        }
        
        public static void Warning(string warningMessage)
        {
            WriteToFile(warningMessage, "Warning_" + DateTime.Now.ToString("ddMMyyHHmmssfff") + ".txt");
        }
    }
}
