using System;

using DataTreeBase.Parameters;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests.Parameters
{
    public class BaseParamTests<T>
    {
        protected void CheckSetErroneousValueAsString(DataTreeParameter<T> p, string testValue)
        {
            try
            {
                p.AsString = testValue;
                Assert.Fail("ArgumentException expected but did not occur");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Expected ArgumentException occurred");
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception occurred: " + e);
            }
        }

        protected void CheckProhibitedValueChange(DataTreeParameter<T> p, T value, Action<DataTreeParameterBase> onChangedAction)
        {
            p.OnChanged += onChangedAction;
            try
            {
                p.Value = value;
                Assert.Fail("InvalidOperationException expected but did not occur");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Expected InvalidOperationException occurred");
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception occurred: " + e);
            }
        }
    }
}