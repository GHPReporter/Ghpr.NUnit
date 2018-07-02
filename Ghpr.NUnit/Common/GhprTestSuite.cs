using System;

namespace Ghpr.NUnit.Common
{
    public class GhprTestSuite
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Output { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            var outputPart = Output.Equals("") ? "No Output" : $"Output = {Output}";
            return $"GhprTestSuite: Id = {Id}, ParentID = {ParentId}, Type = {Type}, {Environment.NewLine}{outputPart}";
        }
    }
}