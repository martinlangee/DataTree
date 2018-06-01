using System;
using System.Collections.Generic;

using DataTreeBase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests
{
    [TestClass]
    public class ChoiceParameterTests : BaseParamTests<int>
    {
        private bool _passedOnChanged;

        private ChoiceParameter CreateParam()
        {
            return new ChoiceParameter(null, "myChoiceId", "myChoiceName", 5,
                                       new List<Tuple<int, string>>()
                                       {
                                           new Tuple<int, string>(3, "drei"),
                                           new Tuple<int, string>(5, "fünf"),
                                           new Tuple<int, string>(8, "acht"),
                                           new Tuple<int, string>(9, "neun"),
                                           new Tuple<int, string>(1, "eins"),
                                       });
        }

        [TestMethod]
        public void InitializationTest()
        {
            var p = CreateParam();
            Assert.IsNotNull(p, "Parameter object could not be created");

            Assert.AreEqual(p.Id, "myChoiceId", "Id not initialized correctly");
            Assert.AreEqual(p.Name, "myChoiceName", "Name not initialized correctly");
            Assert.AreEqual(p.PathId, "myChoiceId", "PathId not initialized correctly");
            Assert.AreEqual(p.Value, 5, "Value not initialized correctly");
            Assert.AreEqual(p.DefaultValue, 5, "DefaultValue not initialized correctly");
            Assert.AreEqual(p.BufferedValue, 5, "BufferedValue not initialized correctly");
            Assert.AreEqual(p.AsString, "fünf", "AsString not initialized correctly");
        }

        [TestMethod]
        public void ModificationTest()
        {
            var p = CreateParam();
            p.OnChanged += ParamOnChanged;

            _passedOnChanged = false;
            p.Value = 5;
            Assert.IsFalse(_passedOnChanged, "OnChanged called without a cause");
            Assert.IsFalse(p.IsModified, "IsModified = true but must be false");

            _passedOnChanged = false;
            p.Value = 3;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.Value = 8;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = true but must be false");

            _passedOnChanged = false;
            p.Value = 9;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = true but must be false");

            _passedOnChanged = false;
            p.Value = 1;
            Assert.IsTrue(_passedOnChanged, "OnChanged call missing");
            Assert.IsTrue(p.IsModified, "IsModified = true but must be false");

            p.OnChanged -= ParamOnChanged;
            _passedOnChanged = false;
            p.Value = 5;
            Assert.IsFalse(_passedOnChanged, "OnChanged called without a cause");
            Assert.IsFalse(p.IsModified, "IsModified = true but must be false");
        }

        [TestMethod]
        public void BufferTest()
        {
            var p = CreateParam(); 

            p.Value = 8;
            p.ResetModified();
            Assert.AreEqual(p.DefaultValue, 5, "DefaultValue is changed but may not be");
            Assert.AreEqual(p.BufferedValue, p.Value, "BufferedValue not set correctly");

            p.Value = 5;
            p.Restore();
            Assert.AreEqual(p.DefaultValue, 5, "DefaultValue is changed but may not be");
            Assert.AreEqual(p.BufferedValue, 8, "BufferedValue not set correctly");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var p = CreateParam();

            p.Value = 3;
            Assert.AreEqual(p.Value, 3, $"Value '3' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "drei", $"AsString 'drei' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 0, $"ValueIdx '0' expected but was  '{p.ValueIdx}'");

            p.Value = 5;
            Assert.AreEqual(p.Value, 5, $"Value '5' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "fünf", $"AsString 'fünf' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 1, $"ValueIdx '1' expected but was  '{p.ValueIdx}'");

            p.Value = 8;
            Assert.AreEqual(p.Value, 8, $"Value '8' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "acht", $"AsString 'acht' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 2, $"ValueIdx '2' expected but was  '{p.ValueIdx}'");

            p.Value = 9;
            Assert.AreEqual(p.Value, 9, $"Value '9' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "neun", $"AsString 'neun' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 3, $"ValueIdx '3' expected but was  '{p.ValueIdx}'");

            p.Value = 1;
            Assert.AreEqual(p.Value, 1, $"Value '1' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "eins", $"AsString 'eins' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 4, $"ValueIdx '4' expected but was  '{p.ValueIdx}'");

            p.OnChanged += ParamOnChanged;

            _passedOnChanged = false;
            p.ValueIdx = 0;
            Assert.AreEqual(p.Value, 3, $"Value '3' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "drei", $"AsString 'drei' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 0, $"ValueIdx '0' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.ValueIdx = 1;
            Assert.AreEqual(p.Value, 5, $"Value '5' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "fünf", $"AsString 'fünf' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 1, $"ValueIdx '1' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.ValueIdx = 2;
            Assert.AreEqual(p.Value, 8, $"Value '8' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "acht", $"AsString 'acht' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 2, $"ValueIdx '2' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.ValueIdx = 3;
            Assert.AreEqual(p.Value, 9, $"Value '9' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "neun", $"AsString 'neun' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 3, $"ValueIdx '3' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.ValueIdx = 4;
            Assert.AreEqual(p.Value, 1, $"Value '1' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "eins", $"AsString 'eins' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 4, $"ValueIdx '4' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.AsString = "drei";
            Assert.AreEqual(p.Value, 3, $"Value '3' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "drei", $"AsString 'drei' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 0, $"ValueIdx '0' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.AsString = "fünf";
            Assert.AreEqual(p.Value, 5, $"Value '5' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "fünf", $"AsString 'fünf' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 1, $"ValueIdx '1' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.AsString = "acht";
            Assert.AreEqual(p.Value, 8, $"Value '8' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "acht", $"AsString 'acht' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 2, $"ValueIdx '2' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.AsString = "neun";
            Assert.AreEqual(p.Value, 9, $"Value '9' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "neun", $"AsString 'neun' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 3, $"ValueIdx '3' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");

            _passedOnChanged = false;
            p.AsString = "eins";
            Assert.AreEqual(p.Value, 1, $"Value '1' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, "eins", $"AsString 'eins' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, 4, $"ValueIdx '4' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");
        }

        [TestMethod]
        public void SetErroneousValueTest()
        {
            var p = CreateParam();

            CheckSetErroneousValueAsString(p, "Otto");
            CheckSetErroneousValueAsString(p, "0");
            CheckSetErroneousValueAsString(p, "1,234");
            CheckSetErroneousValueAsString(p, "567.345");
            CheckSetErroneousValueAsString(p, "45645.");
            CheckSetErroneousValueAsString(p, "-4545764.123e12");
        }

        [TestMethod]
        public void ProhibitedValueChangeTest()
        {
            var p = CreateParam(); 

            CheckProhibitedValueChange(p, 3,
                                       param =>
                                           ((ChoiceParameter)param).Value = 9
                                      );
        }

        private void ParamOnChanged(DataTreeParameterBase dataTreeParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataTreeParameterBase.Name} value set to: {dataTreeParameterBase.AsString}");
        }
    }
}
