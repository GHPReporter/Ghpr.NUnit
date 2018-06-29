using Ghpr.Core.Common;

namespace Ghpr.NUnit.Common
{
    public class NUnitTestCase
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public ItemInfoDto GhprTestInfo { get; set; }
        public TestOutputDto GhprTestOutput { get; set; }
    }
}