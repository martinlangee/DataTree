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
    public class StringParameterTests: BaseParamTests<string>
    {
        private bool _passedOnChanged;

        [TestMethod]
        public void InitializationTest()
        {
            var p = new StringParameter(null, "myStringId", "myStringName", "default");
            Assert.IsNotNull(p, "Parameter object could not be created");

            Assert.AreEqual(p.Id, "myStringId", "Id not initialized correctly");
            Assert.AreEqual(p.Designation, "myStringName", "Name not initialized correctly");
            Assert.AreEqual(p.PathId, "myStringId", "PathId not initialized correctly");
            Assert.AreEqual(p.Value, "default", "Value not initialized correctly");
            Assert.AreEqual(p.BufferedValue, "default", "BufferedValue not initialized correctly");
            Assert.AreEqual(p.AsString, "default", "AsString not initialized correctly");
        }

        [TestMethod]
        public void ModificationTest()
        {
            var p = new StringParameter(null, "myStringId", "myStringName", "default");
            p.OnChanged += ParamOnChanged;

            _passedOnChanged = false;
            p.Value = "default";
            Assert.IsFalse(_passedOnChanged, "OnChanged called without a cause");
            Assert.IsFalse(p.IsModified, "IsModified = true but must be false");

            _passedOnChanged = false;
            p.Value = "abcdef";
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.Value = "default";
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsFalse(p.IsModified, "IsModified = true but must be false");

            p.OnChanged -= ParamOnChanged;
            _passedOnChanged = false;
            p.Value = "sdflkjgf";
            Assert.IsFalse(_passedOnChanged, "OnChanged not allowed but occurred");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");
        }

        [TestMethod]
        public void BufferTest()
        {
            var p = new StringParameter(null, "myStringId", "myStringName", "default");

            p.Value = "hjdsfkkjsdhfkdsj";
            p.ResetModifiedState();
            Assert.AreEqual(p.BufferedValue, p.Value, "BufferedValue not set correctly");
        }

        [TestMethod]
        public void ProhibitedValueChangeTest()
        {
            var p = new StringParameter(null, "myStringId", "myStringName", "default");

            CheckProhibitedValueChange(p, "hhhhhhhhhhh",
                                       param =>
                                           ((StringParameter) param).Value = "ddddddd"
                                      );
        }

        private void ParamOnChanged(DataParameterBase dataParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataParameterBase.Designation} value set to: {dataParameterBase.AsString}");
        }
    }
}
