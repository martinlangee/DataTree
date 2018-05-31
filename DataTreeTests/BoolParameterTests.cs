using System;
using System.Linq;

using DataTreeBase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests
{
    [TestClass]
    public class BoolParameterTests: BaseParamTests<bool>
    {
        private bool _passedOnChanged;

        [TestMethod]
        public void InitializationTest()
        {
            var p = new BoolParameter(null, "myBoolId", "myBoolName", true);
            Assert.IsNotNull(p, "Parameter object could not be created");

            Assert.AreEqual(p.Id, "myBoolId", "Id not initialized correctly");
            Assert.AreEqual(p.Name, "myBoolName", "Name not initialized correctly");
            Assert.AreEqual(p.PathId, "myBoolId", "PathId not initialized correctly");
            Assert.AreEqual(p.Value, true, "Value not initialized correctly");
            Assert.AreEqual(p.DefaultValue, true, "DefaultValue not initialized correctly");
            Assert.AreEqual(p.BufferedValue, true, "BufferedValue not initialized correctly");
            Assert.AreEqual(p.AsString, "True", "AsString not initialized correctly");
        }

        [TestMethod]
        public void ModificationTest()
        {
            var p = new BoolParameter(null, "myBoolId", "myBoolName");
            p.OnChanged += ParamOnChanged;

            _passedOnChanged = false;
            p.Value = false;
            Assert.IsFalse(_passedOnChanged, "OnChanged called without a cause");
            Assert.IsFalse(p.IsModified, "IsModified = true but must be false");

            _passedOnChanged = false;
            p.Value = true;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.Value = false;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsFalse(p.IsModified, "IsModified = true but must be false");

            p.OnChanged -= ParamOnChanged;
            _passedOnChanged = false;
            p.Value = true;
            Assert.IsFalse(_passedOnChanged, "OnChanged not allowed but occurred");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var p = new BoolParameter(null, "myBoolId", "myBoolName");

            CheckValueSet(p, "Otto");
        }

        [TestMethod]
        public void BufferTest()
        {
            var p = new BoolParameter(null, "myBoolId", "myBoolName");

            p.Value = !p.Value;
            p.ResetModified();
            Assert.AreEqual(p.DefaultValue, false, "DefaultValue is changed but may not be");
            Assert.AreEqual(p.BufferedValue, p.Value, "BufferedValue not set correctly");
        }

        [TestMethod]
        public void ProhibitedValueChangeTest()
        {
            var p = new BoolParameter(null, "myBoolId", "myBoolName");

            CheckProhibitedValueChange(p, true,
                                       param =>
                                           ((BoolParameter) param).Value = false
                                      );
        }

        private void ParamOnChanged(DataTreeParameterBase dataTreeParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataTreeParameterBase.Name} value set to: {dataTreeParameterBase.AsString}");
        }
    }
}
