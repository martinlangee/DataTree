using System;
using System.Linq;

using DataTreeBase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests
{
    [TestClass]
    public class BoolParameterTests
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
            Assert.AreEqual(p.AsString, "True", "AsString not initialized correctly");
        }

        [TestMethod]
        public void ModificationTest()
        {
            var p = new BoolParameter(null, "myBoolId", "myBoolName");
            p.OnChanged += ParamOnChanged1;

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

            p.OnChanged -= ParamOnChanged1;
            _passedOnChanged = false;
            p.Value = true;
            Assert.IsFalse(_passedOnChanged, "OnChanged not allowed but occurred");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var p = new BoolParameter(null, "myBoolId", "myBoolName");
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
            var p = new BoolParameter(null, "myBoolId", "myBoolName");
            p.OnChanged += ParamOnChanged2;
            try
            {
                p.Value = true;
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
            ((BoolParameter) dataTreeParameterBase).Value = false;
        }
    }
}
