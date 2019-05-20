#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
#endregion

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