using System;
using System.Linq;

using DataTreeBase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests
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
            Assert.AreEqual(p.Name, "myStringName", "Name not initialized correctly");
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
            p.ResetModified();
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

        private void ParamOnChanged(DataTreeParameterBase dataTreeParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataTreeParameterBase.Name} value set to: {dataTreeParameterBase.AsString}");
        }
    }
}
