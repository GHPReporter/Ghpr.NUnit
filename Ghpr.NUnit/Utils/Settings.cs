using System;
using System.Configuration;
using Ghpr.Core.Interfaces;

namespace Ghpr.NUnit.Utils
{
    public class Settings : IReporterSettings
    {
        public Settings()
        {
            OutputPath = ConfigurationManager.AppSettings["OutputPath"];
            TakeScreenshotAfterFail = bool.Parse(ConfigurationManager.AppSettings["TakeScreenshotAfterFail"]);
            Sprint = ConfigurationManager.AppSettings["Sprint"];
            RunName = ConfigurationManager.AppSettings["RunName"];
            RunGuid = ConfigurationManager.AppSettings["RunGuid"];
            RealTimeGeneration = bool.Parse(ConfigurationManager.AppSettings["RealTimeGeneration"]);
        }
        
        public string OutputPath { get; }
        public bool TakeScreenshotAfterFail { get; }
        public string Sprint { get; }
        public string RunName { get; }
        public string RunGuid { get; }
        public bool RealTimeGeneration { get; }
    }
}