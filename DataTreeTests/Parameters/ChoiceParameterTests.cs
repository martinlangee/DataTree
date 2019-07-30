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
using System.Collections.Generic;
using DataBase.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests.Parameters
{
    [TestClass]
    public class ChoiceParameterTests : BaseParamTests<int>
    {
        private bool _passedOnChanged;

        [TestMethod]
        public void InitializationTest()
        {
            var p = CreateParam();
            Assert.IsNotNull(p, "Parameter object could not be created");

            Assert.AreEqual(p.Id, "myChoiceId", "Id not initialized correctly");
            Assert.AreEqual(p.Designation, "myChoiceName", "Name not initialized correctly");
            Assert.AreEqual(p.PathId, "myChoiceId", "PathId not initialized correctly");
            Assert.AreEqual(p.Value, 5, "Value not initialized correctly");
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
            p.ResetModifiedState();
            Assert.AreEqual(p.BufferedValue, p.Value, "BufferedValue not set correctly");

            p.Value = 5;
            p.Restore();
            Assert.AreEqual(p.BufferedValue, 8, "BufferedValue not set correctly");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var p = CreateParam();
            p.OnChanged += ParamOnChanged;

            // setting Value

            _passedOnChanged = false;
            p.Value = 3;
            CheckSingleValue(p, 3, "drei", 0);

            _passedOnChanged = false;
            p.Value = 5;
            CheckSingleValue(p, 5, "fünf", 1);

            _passedOnChanged = false;
            p.Value = 8;
            CheckSingleValue(p, 8, "acht", 2);

            _passedOnChanged = false;
            p.Value = 9;
            CheckSingleValue(p, 9, "neun", 3);

            _passedOnChanged = false;
            p.Value = 1;
            CheckSingleValue(p, 1, "eins", 4);

            // setting ValueIdx

            _passedOnChanged = false;
            p.ValueIdx = 0;
            CheckSingleValue(p, 3, "drei", 0);

            _passedOnChanged = false;
            p.ValueIdx = 1;
            CheckSingleValue(p, 5, "fünf", 1);

            _passedOnChanged = false;
            p.ValueIdx = 2;
            CheckSingleValue(p, 8, "acht", 2);

            _passedOnChanged = false;
            p.ValueIdx = 3;
            CheckSingleValue(p, 9, "neun", 3);

            _passedOnChanged = false;
            p.ValueIdx = 4;
            CheckSingleValue(p, 1, "eins", 4);

            // setting AsString

            _passedOnChanged = false;
            p.AsString = "drei";
            CheckSingleValue(p, 3, "drei", 0);

            _passedOnChanged = false;
            p.AsString = "fünf";
            CheckSingleValue(p, 5, "fünf", 1);

            _passedOnChanged = false;
            p.AsString = "acht";
            CheckSingleValue(p, 8, "acht", 2);

            _passedOnChanged = false;
            p.AsString = "neun";
            CheckSingleValue(p, 9, "neun", 3);

            _passedOnChanged = false;
            p.AsString = "eins";
            CheckSingleValue(p, 1, "eins", 4);
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

        [TestMethod]
        public void CheckChoiceList()
        {
            // TODO: wieder in Betrieb nehmen, wenn Konzept für Undo/Redo von Änderungen an der Choice-List implementiert sind

            //var p = CreateParam();

            //// value remains the same when changing Choices and the old value is still available; choice string can be changing

            //p.Value = 5;
            //p.Choices = new List<Tuple<int, string>>()
            //               {
            //                   new Tuple<int, string>(5, "five"),
            //                   new Tuple<int, string>(9, "nine"),
            //                   new Tuple<int, string>(1, "one"),
            //               };
            //Assert.AreEqual(p.Value, 5, "Value is changed but should not be");

            //p.Choices = new List<Tuple<int, string>>()
            //               {
            //                   new Tuple<int, string>(1, "one"),
            //                   new Tuple<int, string>(2, "two"),
            //                   new Tuple<int, string>(3, "three"),
            //                   new Tuple<int, string>(4, "four"),
            //                   new Tuple<int, string>(5, "five new"),
            //                   new Tuple<int, string>(9, "nine"),
            //               };
            //Assert.AreEqual(p.Value, 5, $"Value is changed  to '{p.Value}' but should be '5'");
            //Assert.AreEqual(p.AsString, "five new", $"AsString is {p.AsString} but should be 'five new'");

            //p.Choices = new List<Tuple<int, string>>()
            //               {
            //                   new Tuple<int, string>(1, "one"),
            //                   new Tuple<int, string>(2, "two"),
            //                   new Tuple<int, string>(3, "three"),
            //                   new Tuple<int, string>(4, "four"),
            //                   new Tuple<int, string>(9, "nine"),
            //               };


            //// null choice list assignment
            //CheckArgumentNullException(p, null);


            //// empty choice list assignment
            //CheckArgumentNullException(p, new List<Tuple<int, string>>());

            //// ambiguous choice values
            //CheckArgumentException(p, new List<Tuple<int, string>>
            //                          {
            //                              new Tuple<int, string>(5, "five"),
            //                              new Tuple<int, string>(9, "nine"),
            //                              new Tuple<int, string>(5, "one"),
            //                          });

            //// ambiguous choice strings
            //CheckArgumentException(p, new List<Tuple<int, string>>
            //                          {
            //                              new Tuple<int, string>(1, "five"),
            //                              new Tuple<int, string>(9, "nine"),
            //                              new Tuple<int, string>(5, "nine"),
            //                          });
        }

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

        private void CheckSingleValue(ChoiceParameter p, int val, string valStr, int idx)
        {
            Assert.AreEqual(p.Value, val, $"Value '{val}' expected but was '{p.Value}'");
            Assert.AreEqual(p.AsString, valStr, $"AsString '{valStr}' expected but was '{p.Value}'");
            Assert.AreEqual(p.ValueIdx, idx, $"ValueIdx '{idx}' expected but was  '{p.ValueIdx}'");
            Assert.IsTrue(_passedOnChanged, "IsModified = false but must be true");
        }

        //private void CheckArgumentException(ChoiceParameter p, List<Tuple<int, string>> choices)
        //{
        //    try
        //    {
        //        p.Choices = choices;
        //        Assert.Fail("ArgumentException expected but not detected");
        //    }
        //    catch (ArgumentException)
        //    {
        //        Console.WriteLine("Expected ArgumentException exception occurred");
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.Fail("Unexpected exception occurred: " + e);
        //    }
        //}

        //private void CheckArgumentNullException(ChoiceParameter p, List<Tuple<int, string>> choices)
        //{
        //    try
        //    {
        //        p.Choices = choices;
        //        Assert.Fail("ArgumentNullException expected but not detected");
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        Console.WriteLine("Expected ArgumentNullException exception occurred");
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.Fail("Unexpected exception occurred: " + e);
        //    }
        //}

        private void ParamOnChanged(DataParameterBase dataParameterBase)
        {
            _passedOnChanged = true;
            Console.WriteLine($"Parameter {dataParameterBase.Designation} value set to: {dataParameterBase.AsString}");
        }
    }
}
