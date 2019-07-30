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
using System.Globalization;
using DataBase.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests.Parameters
{
    [TestClass]
    public class FloatParameterTests: BaseParamTests<double>
    {
        private bool _passedOnChanged;

        [TestMethod]
        public void InitializationTest()
        {
            var p = new FloatParameter(null, "myFloatId", "myFloatName", 55.0, "m/s", 5);
            Assert.IsNotNull(p, "Parameter object could not be created");

            Assert.AreEqual(p.Id, "myFloatId", "Id not initialized correctly");
            Assert.AreEqual(p.Designation, "myFloatName", "Name not initialized correctly");
            Assert.AreEqual(p.PathId, "myFloatId", "PathId not initialized correctly");
            Assert.AreEqual(p.Value, 55, "Value not initialized correctly");
            Assert.AreEqual(p.AsString, "55", "AsString not initialized correctly");
            Assert.AreEqual(p.Unit, "m/s", "Unit not initialized correctly");
            Assert.AreEqual(p.Decimals, 5, "Decimals not initialized correctly");
        }

        [TestMethod]
        public void ModificationTest()
        {
            var p = new FloatParameter(null, "myFloatId", "myFloatName", 11.123, "m/s", 5);
            p.OnChanged += ParamOnChanged;

            _passedOnChanged = false;
            p.Value = 11.123;
            Assert.IsFalse(_passedOnChanged, "OnChanged called without a cause");
            Assert.IsFalse(p.IsModified, "IsModified = true but must be false");

            _passedOnChanged = false;
            p.Value = 0;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.Value = 55.7689;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");

            p.Value = 123455.12345;
            _passedOnChanged = false;
            p.Value = 123455.123451234;  // more decimals then precision
            Assert.IsFalse(_passedOnChanged, "OnChanged called without a cause");

            p.OnChanged -= ParamOnChanged;
            _passedOnChanged = false;
            p.Value = 345.1;
            Assert.IsFalse(_passedOnChanged, "OnChanged not allowed but occurred");
        }

        [TestMethod]
        public void BufferTest()
        {
            var p = new FloatParameter(null, "myFloatId", "myFloatName", 123.0, "m/s", 5);

            p.Value = 555555.66;
            p.ResetModifiedState();
            Assert.AreEqual(p.BufferedValue, p.Value, "BufferedValue not set correctly");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var p = new FloatParameter(null, "myFloatId", "myFloatName", 55.076, "m/s", 5);

            p.AsString = $"123456789{CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator}23";

            CheckSetErroneousValueAsString(p, "Otto");
            CheckSetErroneousValueAsString(p, "true");
        }

        [TestMethod]
        public void ProhibitedValueChangeTest()
        {
            var p = new FloatParameter(null, "myFloatId", "myFloatName", 55.076, "m/s", 5);

            CheckProhibitedValueChange(p, 77,
                                       param =>
                                           ((FloatParameter) param).Value = 1234.34
                                      );
        }

        [TestMethod]
        public void FloatFormatTest()
        {
            var p = new FloatParameter(null, "myFloatId", "myFloatName", 55.076, "m/s", 5);
            Assert.AreEqual(p.AsStringInvCult, "55.076", "AsString representation not correct");
            Assert.AreEqual(p.AsString, "55.076".Replace('.', CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]), "AsStringInvCult representation not correct");

            p.Value = 1.23456789;
            Assert.AreEqual(p.Value, 1.23457, "Value not correctly assigned or rounded");

            p.OnChanged += ParamOnChanged;

            _passedOnChanged = false;
            p.Decimals = 2;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.AreEqual(p.Value, 1.23, "Value not correctly assigned or rounded");

            _passedOnChanged = false;
            p.Decimals = 8;
            Assert.IsFalse(_passedOnChanged, "OnChanged called but should have not");
            Assert.AreEqual(p.Value, 1.23, "Value changed by rounding");
        }

        private void ParamOnChanged(DataParameterBase dataParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataParameterBase.Designation} value set to: {dataParameterBase.AsString}");
        }
    }
}