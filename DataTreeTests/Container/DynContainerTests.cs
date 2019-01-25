﻿using System;
using System.Collections.Generic;
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

            Assert.IsTrue(model.Tc1.Containers.Count == 2,
                $"Number of sub containers of Tc1 must be 2 but are {model.Tc1.Containers.Count}");

            Assert.IsTrue(model.Tc1.Containers[0].IntParam.Value == 5,
                $"Value of IntParam in container 0 must be 5 but is {model.Tc1.Containers[0].IntParam.Value}");
            Assert.IsTrue(model.Tc1.Containers[0].IntParam.BufferedValue == 5,
                $"BufferedValue of IntParam in container 0 must be 5 but is {model.Tc1.Containers[0].IntParam.BufferedValue}");

            Assert.IsTrue(model.Tc1.Containers[1].IntParam.Value == 16683,
                $"Value of IntParam in container 1 must be 16683 but is {model.Tc1.Containers[1].IntParam.Value}");
            Assert.IsTrue(model.Tc1.Containers[1].IntParam.BufferedValue == 16683,
                $"BufferedValue of IntParam in container 1 must be 16683 but is {model.Tc1.Containers[1].IntParam.BufferedValue}");
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
