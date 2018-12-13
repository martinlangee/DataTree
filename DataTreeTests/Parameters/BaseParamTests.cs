using System;
using DataBase.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests.Parameters
{
    public class BaseParamTests<T>
    {
        protected void CheckSetErroneousValueAsString(DataParameter<T> p, string testValue)
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

        protected void CheckProhibitedValueChange(DataParameter<T> p, T value, Action<DataParameterBase> onChangedAction)
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