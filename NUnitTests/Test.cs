using NUnit.Framework;

namespace NUnitTests
{
    public class Test : TestBase
    {
        [Test(Description = "This is a failed test")]
        public void TestWhichIsFailing()
        {
            Assert.Fail("Failed to see how the screenshot is taken");
        }
    }
}