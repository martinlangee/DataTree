using System;
using System.Linq;

using DataTreeBase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests
{
    [TestClass]
    public class BinaryParameterTests
    {
        readonly byte[] _defValue = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private bool _passedOnChanged;
        

        [TestMethod]
        public void InitializationTest()
        {
            var p = new BinaryParameter(null, "myBinaryId", "myBinaryName", _defValue);
            Assert.IsNotNull(p, "Parameter object could not be created");

            Assert.AreEqual(p.Id, "myBinaryId", "Id not initialized correctly");
            Assert.AreEqual(p.Name, "myBinaryName", "Name not initialized correctly");
            Assert.AreEqual(p.PathId, "myBinaryId", "PathId not initialized correctly");
            Assert.IsTrue(p.Value.SequenceEqual(_defValue), "Value not initialized correctly");
            Assert.AreEqual(p.AsString, Convert.ToBase64String(_defValue), "AsString not initialized correctly");
        }

        [TestMethod]
        public void ModificationTest()
        {
            var p = new BinaryParameter(null, "myBinaryId", "myBinaryName", _defValue);
            p.OnChanged += ParamOnChanged1;

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

            p.OnChanged -= ParamOnChanged1;
            _passedOnChanged = false;
            p.Value = new byte[] { 50, 61, 72, 83, 234, 29 };
            Assert.IsFalse(_passedOnChanged, "OnChanged not allowed but occurred");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            p.ResetModified();
            Assert.IsFalse(p.IsModified, "IsModified = false but must be true");
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
            p.OnChanged += ParamOnChanged2;
            try
            {
                p.Value = new byte[] { 0, 1 };
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
            ((BinaryParameter) dataTreeParameterBase).Value = new byte[] { 9, 8, 7, 6 };
        }
    }
}
