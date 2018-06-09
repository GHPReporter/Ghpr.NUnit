using System;
using NUnit.Framework;

namespace NUnitTests
{
    [TestFixture]
    public class CoolTests : TestFixtureBase
    {
        [SetUp]
        public void TestSetUp()
        {
            Console.WriteLine("SetUp Base");
        }

        [TearDown]
        public void TestTearDown()
        {
            Console.WriteLine("TearDown Base");
        }

        [Test]
        public void Test1()
        {
            Console.WriteLine("Test1");
            Assert.AreEqual(1, 1);
        }

        [Test]
        public void Test2()
        {
            Console.WriteLine("Test2");
            Assert.AreEqual(1, 2);
        }

        [TestCase(1, 2)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        public void Test3(int a, int b)
        {
            Console.WriteLine($"Test3: Comparing {a} and {b}...");
            Assert.AreEqual(a, b);
        }
    }
}