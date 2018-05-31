using System;
using System.Linq;

using DataTreeBase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests
{
    [TestClass]
    public class IntParameterTests
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
            Assert.AreEqual(p.AsString, "55", "AsString not initialized correctly");
        }

        [TestMethod]
        public void ModificationTest()
        {
            var p = new IntParameter(null, "myIntId", "myIntName", 11);
            p.OnChanged += ParamOnChanged1;

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

            p.OnChanged -= ParamOnChanged1;
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
            Assert.AreEqual(p.DefaultValue, 123, "DefaultValue is changed but may not be");
            Assert.AreEqual(p.BufferedValue, p.Value, "BufferedValue not set correctly");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var p = new IntParameter(null, "myIntId", "myIntName", 1);
            try
            {
                p.AsString = "Otto";
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

        [TestMethod]
        public void ProhibitedValueChangeTest()
        {
            var p = new IntParameter(null, "myIntId", "myIntName", 0);
            p.OnChanged += ParamOnChanged2;
            try
            {
                p.Value = 77;
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

        private void ParamOnChanged1(DataTreeParameterBase dataTreeParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataTreeParameterBase.Name} value set to: {dataTreeParameterBase.AsString}");
        }

        private void ParamOnChanged2(DataTreeParameterBase dataTreeParameterBase)
        {
            ((IntParameter) dataTreeParameterBase).Value = 1234;
        }
    }
}
