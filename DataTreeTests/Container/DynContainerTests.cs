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
using System.Diagnostics;
using DataBase.Container;
using DataBase.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests.Container
{
    [TestClass]
    public sealed class DynContainerTests
    {
        private DynTestRoot _model = new DynTestRoot();

        [TestMethod]
        public void CreationTests()
        {
            var model = new DynTestRoot();

            Assert.IsTrue(model.Tc1.Children.Count == 2,
                $"Number of sub containers of Tc1 must be 2 but are {model.Tc1.Items.Count}");

            Assert.IsTrue(model.Tc1.Items[0].IntParam.Value == 5,
                $"Value of IntParam in container 0 must be 5 but is {model.Tc1.Items[0].IntParam.Value}");
            Assert.IsTrue(model.Tc1.Items[0].IntParam.BufferedValue == 5,
                $"BufferedValue of IntParam in container 0 must be 5 but is {model.Tc1.Items[0].IntParam.BufferedValue}");

            Assert.IsTrue(model.Tc1.Items[1].IntParam.Value == 16683,
                $"Value of IntParam in container 1 must be 16683 but is {model.Tc1.Items[1].IntParam.Value}");
            Assert.IsTrue(model.Tc1.Items[1].IntParam.BufferedValue == 16683,
                $"BufferedValue of IntParam in container 1 must be 16683 but is {model.Tc1.Items[1].IntParam.BufferedValue}");
        }

        [TestMethod]
        public void AddRemoveTests()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void SetToDefaultTests()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ModifiedTests()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ListChangedTests()
        {
            throw new NotImplementedException();
        }
    }

    [DebuggerStepThrough]
    public sealed class DynTestRoot : DataContainer
    {
        public DynTestRoot()
            : base(null, "TR", "TestRoot")
        {
            Tc1 = new DynTestCont1(this, "Tc1", "TestCont1",
                cont =>
                {
                    cont.Add(cont3 => cont3.IntParam.Init(5));
                    cont.Add(cont3 => cont3.IntParam.Init(16683));
                });
        }

        public DynTestCont1 Tc1 { get; }
    }

    [DebuggerStepThrough]
    public sealed class DynTestCont1 : DataDynParentContainer<DynTestCont3>
    {
        public DynTestCont1(DataContainer parent, string id, string name, Action<DataDynParentContainer<DynTestCont3>> initContainers)
            : base(parent, id, name, initContainers)
        {
        }
    }

    [DebuggerStepThrough]
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class DynTestCont3 : DataDynContainer
    {
        public DynTestCont3(DataContainer parent)
            : base(parent, "DTc3", "DynTestCont3")
        {
            IntParam = new IntParameter(this, "IP", "IntParam", 1);
        }

        public IntParameter IntParam { get; }
    }
}
