using System;
using NUnit.Framework;

namespace NUnitTests
{
    public class TestFixtureBase
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Console.WriteLine("OneTimeSetUp Base");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Console.WriteLine("OneTimeTearDown Base");
        }
    }
}