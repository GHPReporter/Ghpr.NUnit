using System;
using Ghpr.Core;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Ghpr.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ToReport : NUnitAttribute, ITestAction
    {
        private Reporter _reporter = new Reporter();

        public void BeforeTest(ITest test)
        {
            throw new NotImplementedException();
        }

        public void AfterTest(ITest test)
        {
            throw new NotImplementedException();
        }

        public ActionTargets Targets => ActionTargets.Test;
    }
}
