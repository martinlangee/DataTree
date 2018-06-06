using System.Collections.Generic;
using System.Diagnostics;

using DataTreeBase.Container;
using DataTreeBase.Parameters;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests.Container
{
    [TestClass]
    public sealed class DynContainerTests
    {
        [TestMethod]
        public void CreationTests()
        {
        }

        [TestMethod]
        public void AddRemoveTests()
        {
        }

        [TestMethod]
        public void ModifiedTests()
        {
        }

        [TestMethod]
        public void ListChangedTests()
        {
        }
    }

    [DebuggerStepThrough]
    public sealed class DynTestRoot : DataTreeContainer
    {
        public DynTestRoot()
            : base(null, "TR", "TestRoot")
        {
            Tc1 = new DynTestCont1(this, "Tc1", "TestCont1");
        }

        public DynTestCont1 Tc1 { get; }
    }

    [DebuggerStepThrough]
    public sealed class DynTestCont1 : DataTreeDynParentContainer<DynTestCont3>
    {
        public DynTestCont1(DataTreeContainer parent, string id, string name)
            : base(parent, id, name)
        {
            Add(cont3 => cont3.FloatParam.Value = 5);
            Add(cont3 => cont3.FloatParam.Value = 6);
        }

        public IReadOnlyList<DynTestCont3> DynConts => Containers;
    }

    [DebuggerStepThrough]
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class DynTestCont3 : DataTreeDynContainer
    {
        public DynTestCont3(DataTreeContainer parent)
            : base(parent, "DTc3", "DynTestCont3")
        {
            FloatParam = new FloatParameter(this, "BP", "BoolParam", 1, "km", 6);
        }

        public FloatParameter FloatParam { get; }
    }
}
