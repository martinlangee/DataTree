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
    [TestClass]
    public class IntParameterTests: BaseParamTests<int>
    {
        private bool _passedOnChanged;

        [TestMethod]
        public void InitializationTest()
        {
            var p = new IntParameter(null, "myIntId", "myIntName", 55);
            Assert.IsNotNull(p, "Parameter object could not be created");

            Assert.AreEqual(p.Id, "myIntId", "Id not initialized correctly");
            Assert.AreEqual(p.Designation, "myIntName", "Name not initialized correctly");
            Assert.AreEqual(p.PathId, "myIntId", "PathId not initialized correctly");
            Assert.AreEqual(p.Value, 55, "Value not initialized correctly");
            Assert.AreEqual(p.BufferedValue, 55, "BufferedValue not initialized correctly");
            Assert.AreEqual(p.AsString, "55", "AsString not initialized correctly");
        }

        [TestMethod]
        public void ModificationTest()
        {
            var p = new IntParameter(null, "myIntId", "myIntName", 11);
            p.OnChanged += ParamOnChanged;

            _passedOnChanged = false;
            p.Value = 11;
            Assert.IsFalse(_passedOnChanged, "OnChanged called without a cause");
            Assert.IsFalse(p.IsModified, "IsModified = true but must be false");

            _passedOnChanged = false;
            p.Value = 0;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.Value = 55;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = true but must be false");

            p.OnChanged -= ParamOnChanged;
            _passedOnChanged = false;
            p.Value = 345;
            Assert.IsFalse(_passedOnChanged, "OnChanged not allowed but occurred");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");
        }

        [TestMethod]
        public void BufferTest()
        {
            var p = new IntParameter(null, "myIntId", "myIntName", 123);

            p.Value = 555555;
            p.ResetModified();
            Assert.AreEqual(p.BufferedValue, p.Value, "BufferedValue not set correctly");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var p = new IntParameter(null, "myIntId", "myIntName", 1);

            p.AsString = "123456789";

            CheckSetErroneousValueAsString(p, "Otto");
            CheckSetErroneousValueAsString(p, "1,234");
            CheckSetErroneousValueAsString(p, "567.345");
            CheckSetErroneousValueAsString(p, "45645.");
            CheckSetErroneousValueAsString(p, "-4545764.123e12");
        }

        [TestMethod]
        public void ProhibitedValueChangeTest()
        {
            var p = new IntParameter(null, "myIntId", "myIntName", 0);

            CheckProhibitedValueChange(p, 77,
                                       param =>
                                           ((IntParameter) param).Value = 1234
                                      );
        }

        private void ParamOnChanged(DataParameterBase dataParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataParameterBase.Designation} value set to: {dataParameterBase.AsString}");
        }
    }
}
