using System;
using System.Globalization;

using DataTreeBase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests
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
            Assert.AreEqual(p.Name, "myFloatName", "Name not initialized correctly");
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
            p.ResetModified();
            Assert.AreEqual(p.DefaultValue, 123, "DefaultValue is changed but may not be");
            Assert.AreEqual(p.BufferedValue, p.Value, "BufferedValue not set correctly");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var p = new FloatParameter(null, "myFloatId", "myFloatName", 55.076, "m/s", 5);

            p.AsString = "123456789,23";

            CheckValueSet(p, "Otto");
            CheckValueSet(p, "true");
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
            Assert.AreEqual(p.AsString, "55.076", "AsString representation not correct");
            Assert.AreEqual(p.AsStringC, "55.076".Replace('.', CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]), "AsStringC representation not correct");

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

        private void ParamOnChanged(DataTreeParameterBase dataTreeParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataTreeParameterBase.Name} value set to: {dataTreeParameterBase.AsString}");
        }
    }
}