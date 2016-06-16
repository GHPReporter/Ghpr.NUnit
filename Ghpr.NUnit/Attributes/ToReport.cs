using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Ghpr.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ToReport : NUnitAttribute, ITestAction
    {
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
