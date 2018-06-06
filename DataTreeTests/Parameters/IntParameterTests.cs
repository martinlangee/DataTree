using System;

using DataTreeBase.Parameters;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests.Parameters
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
            Assert.AreEqual(p.Name, "myIntName", "Name not initialized correctly");
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

        private void ParamOnChanged(DataTreeParameterBase dataTreeParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataTreeParameterBase.Name} value set to: {dataTreeParameterBase.AsString}");
        }
    }
}
