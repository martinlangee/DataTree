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
using System.Linq;
using DataBase.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests.Parameters
{
    [TestClass]
    public class BinaryParameterTests: BaseParamTests<byte[]>
    {
        readonly byte[] _defValue = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private bool _passedOnChanged;
        
        [TestMethod]
        public void InitializationTest()
        {
            var p = new BinaryParameter(null, "myBinaryId", "myBinaryName", _defValue);
            Assert.IsNotNull(p, "Parameter object could not be created");

            Assert.AreEqual(p.Id, "myBinaryId", "Id not initialized correctly");
            Assert.AreEqual(p.Designation, "myBinaryName", "Name not initialized correctly");
            Assert.AreEqual(p.PathId, "myBinaryId", "PathId not initialized correctly");
            Assert.IsTrue(p.Value.SequenceEqual(_defValue), "Value not initialized correctly");
            Assert.IsTrue(p.BufferedValue.SequenceEqual(_defValue), "BufferedValue not initialized correctly");
            Assert.AreEqual(p.AsString, Convert.ToBase64String(_defValue), "AsString not initialized correctly");
        }

        [TestMethod]
        public void ModificationTest()
        {
            var p = new BinaryParameter(null, "myBinaryId", "myBinaryName", _defValue);
            p.OnChanged += ParamOnChanged;

            _passedOnChanged = false;
            p.Value = _defValue;
            Assert.IsFalse(_passedOnChanged, "OnChanged called without a cause");
            Assert.IsFalse(p.IsModified, "IsModified = true but must be false");

            _passedOnChanged = false;
            p.Value = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 0 };
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.Value = null;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.Value = new byte[] { 10, 21, 32, 43 };
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            p.OnChanged -= ParamOnChanged;
            _passedOnChanged = false;
            p.Value = new byte[] { 50, 61, 72, 83, 234, 29 };
            Assert.IsFalse(_passedOnChanged, "OnChanged not allowed but occurred");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            p.ResetModified();
            Assert.IsFalse(p.IsModified, "IsModified = false but must be true");
        }

        [TestMethod]
        public void BufferTest()
        {
            var p = new BinaryParameter(null, "myBinaryId", "myBinaryName", _defValue);

            p.Value = new byte[] { 50, 61, 72, 83, 234, 29, 44, 71 };
            p.ResetModified();
            Assert.IsTrue(p.BufferedValue.SequenceEqual(p.Value), "BufferedValue not set correctly");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var p = new BinaryParameter(null, "myBinaryId", "myBinaryName", _defValue);

            p.Value = new byte[] { 10, 21, 32, 43 , 55, 123, 222, 12, 22, 3, 2, 56, 78, 99, 255 };
            Assert.AreEqual(p.AsString, "ChUgKzd73gwWAwI4TmP/", "AsString expected equal but was not");

            p.Value = new byte[0];
            Assert.AreEqual(p.AsString, "", "Empty string expected but is not");

            p.Value = null;
            Assert.AreEqual(p.AsString, "", "Empty string expected but is not");
        }

        [TestMethod]
        public void ProhibitedValueChangeTest()
        {
            var p = new BinaryParameter(null, "myBinaryId", "myBinaryName", _defValue);

            CheckProhibitedValueChange(p, new byte[] { 0, 1 },
                                       param =>
                                           ((BinaryParameter) param).Value = new byte[] { 9, 8, 7, 6 }
                                      );
        }

        private void ParamOnChanged(DataParameterBase dataParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataParameterBase.Designation} value set to: {dataParameterBase.AsString}");
        }
    }
}
