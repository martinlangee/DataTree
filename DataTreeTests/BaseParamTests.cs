using System;

using DataTreeBase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests
{
    public class BaseParamTests<T>
    {
        protected void CheckValueSet(DataTreeParameter<T> p, string testValue)
        {
            try
            {
                p.AsString = testValue;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Expected ArgumentException occurred");
                return;
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception occurred: " + e);
            }
            Assert.Fail("ArgumentException expected but did not occur");
        }

        protected void CheckProhibitedValueChange(DataTreeParameter<T> p, T value, Action<DataTreeParameterBase> onChangedAction)
        {
            p.OnChanged += onChangedAction;
            try
            {
                p.Value = value;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Expected InvalidOperationException occurred");
                return;
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception occurred: " + e);
            }
            Assert.Fail("InvalidOperationException expected but did not occur");
        }
    }
}