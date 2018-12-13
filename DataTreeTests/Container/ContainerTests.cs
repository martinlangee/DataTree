using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataBase;
using DataBase.Container;
using DataBase.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests.Container
{
    [TestClass]
    public sealed class ContainerTests
    {
        [TestMethod]
        public void ContainerCreationTest()
        {
            var root = new TestRoot();

            Assert.IsNotNull(root.Tc1, "TestCont1 is null but should not be");
            Assert.IsNotNull(root.Tc2, "TestCont2 is null but should not be");
            Assert.IsNotNull(root.Tc1.Tc3, "TestCont3 is null but should not be");

            Assert.AreEqual(root.Containers[0], root.Tc1, "TestCont1 was not inserted into container list");
            Assert.AreEqual(root.Containers[1], root.Tc2, "TestCont2 was not inserted into container list");
            Assert.AreEqual(root.Containers.Count, 2, "Number of containers should be 2 but was " + root.Containers.Count);

            Assert.AreEqual(root.Tc1.Containers[0], root.Tc1.Tc3, "TestCont3 was not inserted into container list");
            Assert.AreEqual(root.Tc1.Containers.Count, 1, "Number of containers should be 1 but was " + root.Tc1.Containers.Count);

            Assert.AreEqual(root.Tc1.Name, "TestCont1", "Name of TestCont1 should be 'TestCont1' but was " + root.Tc1.Name);
            Assert.AreEqual(root.Tc2.Name, "TestCont2", "Name of TestCont2 should be 'TestCont2' but was " + root.Tc2.Name);
            Assert.AreEqual(root.Tc1.Tc3.Name, "TestCont3", "Name of TestCont3 should be 'TestCont3' but was " + root.Tc1.Tc3.Name);

            Assert.AreEqual(root.Tc1.Id, "Tc1", "Id of TestCont1 should be 'Tc1' but was " + root.Tc1.Id);
            Assert.AreEqual(root.Tc2.Id, "Tc2", "Id of TestCont2 should be 'Tc2' but was " + root.Tc2.Id);
            Assert.AreEqual(root.Tc1.Tc3.Id, "Tc3", "Id of TestCont3 should be 'Tc3' but was " + root.Tc1.Tc3.Id);

            Assert.AreEqual(root.Tc1.Parent, root, "Parent of TestCont1 not correct");
            Assert.AreEqual(root.Tc2.Parent, root, "Parent of TestCont2 not correct");
            Assert.AreEqual(root.Tc1.Tc3.Parent, root.Tc1, "Parent of TestCont3 not correct");

            Assert.AreEqual(root.Tc1.Root, root, "Root of TC1 is not set correctly");
            Assert.AreEqual(root.Tc2.Root, root, "Root of TC2 is not set correctly");
            Assert.AreEqual(root.Tc1.Tc3.Root, root, "Root of TC3 is not set correctly");

            Assert.AreEqual(root.Params.Count, 0, "Number of Root params should be 0 but are" + root.Params.Count);
            Assert.AreEqual(root.Tc1.Params.Count, 5, "Number of TC1 params should be 5 but are" + root.Tc1.Params.Count);
            Assert.AreEqual(root.Tc2.Params.Count, 5, "Number of TC2 params should be 5 but are" + root.Tc2.Params.Count);
            Assert.AreEqual(root.Tc1.Tc3.Params.Count, 1, "Number of TC3 params should be 1 but are" + root.Tc1.Tc3.Params.Count);
        }

        [TestMethod]
        public void PathIdTest()
        {
            var root = new TestRoot();

            Assert.AreEqual(root.PathId, "TR", $"PathId '{root.PathId}' is wrong");
            Assert.AreEqual(root.Tc1.PathId, "TR.Tc1", $"PathId '{root.Tc1.PathId}' is wrong");
            Assert.AreEqual(root.Tc1.BinParam.PathId, "TR.Tc1.BP", $"PathId '{root.Tc1.BinParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc1.ChParam.PathId, "TR.Tc1.CP", $"PathId '{root.Tc1.ChParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc1.FloatParam.PathId, "TR.Tc1.FP", $"PathId '{root.Tc1.FloatParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc1.IntParam.PathId, "TR.Tc1.IP", $"PathId '{root.Tc1.IntParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc1.StrParam.PathId, "TR.Tc1.SP", $"PathId '{root.Tc1.StrParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc1.Tc3.PathId, "TR.Tc1.Tc3", $"PathId '{root.Tc1.Tc3.PathId}' is wrong");
            Assert.AreEqual(root.Tc1.Tc3.BoolParam.PathId, "TR.Tc1.Tc3.BP", $"PathId '{root.Tc1.Tc3.BoolParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc2.PathId, "TR.Tc2", $"PathId '{root.Tc2.PathId}' is wrong");
            Assert.AreEqual(root.Tc2.BinParam.PathId, "TR.Tc2.BP", $"PathId '{root.Tc2.BinParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc2.ChParam.PathId, "TR.Tc2.CP", $"PathId '{root.Tc2.ChParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc2.FloatParam.PathId, "TR.Tc2.FP", $"PathId '{root.Tc2.FloatParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc2.IntParam.PathId, "TR.Tc2.IP", $"PathId '{root.Tc2.IntParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc2.StrParam.PathId, "TR.Tc2.SP", $"PathId '{root.Tc2.StrParam.PathId}' is wrong");
            Assert.AreEqual(root.Tc2.Tc3.PathId, "TR.Tc2.Tc3", $"PathId '{root.Tc2.Tc3.PathId}' is wrong");
            Assert.AreEqual(root.Tc2.Tc3.BoolParam.PathId, "TR.Tc2.Tc3.BP", $"PathId '{root.Tc2.Tc3.BoolParam.PathId}' is wrong");

            var allParams = new List<DataNode>();
            AddNodes(allParams, root);
            var pathIds = allParams.Select(p => p.PathId).ToList();
            Assert.AreEqual(pathIds.Distinct().Count(), pathIds.Count, "PathIds over all parameters contain duplicates but should not");
        }

        private static void AddNodes(List<DataNode> plist, DataContainer cont)
        {
            cont.Nodes.ForEach(n =>
                               {
                                   plist.Add(n);
                                   if (n is DataContainer)
                                        AddNodes(plist, n as DataContainer);
                               });
        }

        [TestMethod]
        public void ModifiedTest()
        {
            var root = new TestRoot();

            var oldValue = root.Tc1.FloatParam.Value;
            root.Tc1.FloatParam.Value = oldValue + 123;

            Assert.IsTrue(root.IsModified, "Root is not modified but should be");
            Assert.IsTrue(root.Tc1.IsModified, "TC1 is not modified but should be");
            Assert.IsFalse(root.Tc2.IsModified, "TC2 is modified but should not be");
            Assert.IsFalse(root.Tc1.Tc3.IsModified, "TC3 is modified but should not be");

            root.Tc1.FloatParam.Value = oldValue;

            Assert.IsFalse(root.IsModified, "Root is modified but should not be");
            Assert.IsFalse(root.Tc1.IsModified, "TC1 is modified but should not be");
            Assert.IsFalse(root.Tc2.IsModified, "TC2 is modified but should not be");
            Assert.IsFalse(root.Tc1.Tc3.IsModified, "TC3 is modified but should not be");

            root.Tc1.Tc3.BoolParam.Value = !root.Tc1.Tc3.BoolParam.Value;

            Assert.IsTrue(root.IsModified, "Root is not modified but should be");
            Assert.IsTrue(root.Tc1.IsModified, "TC1 is not modified but should be");
            Assert.IsFalse(root.Tc2.IsModified, "TC2 is modified but should not be");
            Assert.IsTrue(root.Tc1.Tc3.IsModified, "TC3 is not modified but should be");

            root.Tc1.FloatParam.Value = oldValue + 123;
            root.Restore();

            Assert.AreEqual(root.Tc1.FloatParam.Value, oldValue, "FloatParam should have value 0.5 but was " + root.Tc1.FloatParam.Value);
            Assert.AreEqual(root.Tc1.Tc3.BoolParam.Value, true, "BoolParam should have value true but was " + root.Tc1.Tc3.BoolParam.Value);
            Assert.IsFalse(root.IsModified, "Root is modified but should not be");
            Assert.IsFalse(root.Tc1.IsModified, "TC1 is modified but should not be");
            Assert.IsFalse(root.Tc2.IsModified, "TC2 is modified but should not be");
            Assert.IsFalse(root.Tc1.Tc3.IsModified, "TC3 is modified but should not be");

            root.Tc1.FloatParam.Value = oldValue + 123;
            root.Tc1.Tc3.BoolParam.Value = !root.Tc1.Tc3.BoolParam.Value;
            root.ResetModified();

            Assert.AreEqual(root.Tc1.FloatParam.Value, oldValue + 123, "FloatParam should have value 123.5 but was " + root.Tc1.FloatParam.Value);
            Assert.AreEqual(root.Tc1.Tc3.BoolParam.Value, false, "BoolParam should have value false but was " + root.Tc1.Tc3.BoolParam.Value);
            Assert.IsFalse(root.IsModified, "Root is modified but should not be");
            Assert.IsFalse(root.Tc1.IsModified, "TC1 is modified but should not be");
            Assert.IsFalse(root.Tc2.IsModified, "TC2 is modified but should not be");
            Assert.IsFalse(root.Tc1.Tc3.IsModified, "TC3 is modified but should not be");
        }

        [TestMethod]
        public void CloneTest()
        {
            var root = new TestRoot();

            var clone = root.Clone() as TestRoot;

            Assert.AreEqual(clone?.Containers.Count, 2, "Number of containers not as expected");
            Assert.AreEqual(clone?.Tc1.Containers.Count, 1, "Number of containers not as expected");

            root.Tc1.Tc3.BoolParam.Value = !root.Tc1.Tc3.BoolParam.Value;

            Assert.AreNotEqual(root.Tc1.Tc3.BoolParam.Value, clone?.Tc1.Tc3.BoolParam.Value, "Parameter value in original and clone are equal but should not be");

            clone.Tc2.FloatParam.Value = clone.Tc2.FloatParam.Value + 456;

            Assert.AreNotEqual(root.Tc2.FloatParam.Value, clone.Tc2.FloatParam.Value, "Parameter value in original and clone are equal but should not be");
        }

        // todo: XML Load/Save
        [TestMethod]
        public void LoadSaveTest()
        {
            var r1 = new TestRoot();

            r1.Tc1.FloatParam.Value = 678.123;
            r1.Tc1.IntParam.Value = 789;
            r1.Tc1.StrParam.Value = "asdfghjkl";
            r1.Tc1.BinParam.Value = new byte[] { 12, 34, 56, 78, 90, 23, 45, 76, 23 };
            r1.Tc1.ChParam.Value = 3;
            r1.Tc1.Tc3.BoolParam.Value = false;
            r1.Tc2.FloatParam.Value = 98.21;
            r1.Tc2.IntParam.Value = 14539;
            r1.Tc2.StrParam.Value = "wertwel";

            string tempFilePath = Path.Combine(Path.GetTempPath(), "ModelRoot.xml");

            r1.SaveToFile(tempFilePath);

            var r2 = new TestRoot();
            r2.LoadFromFile(tempFilePath);

            Assert.AreEqual(r2.Tc1.FloatParam.Value, 678.123);
            Assert.AreEqual(r2.Tc1.IntParam.Value, 789);
            Assert.AreEqual(r2.Tc1.StrParam.Value, "asdfghjkl");
            Assert.IsTrue(r2.Tc1.BinParam.Value.SequenceEqual(new byte[] { 12, 34, 56, 78, 90, 23, 45, 76, 23 }));
            Assert.AreEqual(r2.Tc1.ChParam.Value, 3);
            Assert.AreEqual(r2.Tc1.Tc3.BoolParam.Value, false);
            Assert.AreEqual(r2.Tc2.FloatParam.Value, 98.21);
            Assert.AreEqual(r2.Tc2.IntParam.Value, 14539);
            Assert.AreEqual(r2.Tc2.StrParam.Value, "wertwel");
        }
    }

    [DebuggerStepThrough]
    public sealed class TestRoot : DataContainer
    {
        public TestRoot()
            : base(null, "TR", "TestRoot")
        {
            Tc1 = new TestCont1(this, "Tc1", "TestCont1");
            Tc2 = new TestCont1(this, "Tc2", "TestCont2");
        }

        public TestCont1 Tc1 { get; }
        public TestCont1 Tc2 { get; }
    }

    [DebuggerStepThrough]
    public sealed class TestCont1 : DataContainer
    {
        public TestCont1(DataContainer parent, string id, string name)
            : base(parent, id, name)
        {
            Tc3 = new TestCont3(this);

            IntParam = new IntParameter(this, "IP", "IntParam", 0);
            StrParam = new StringParameter(this, "SP", "StrParam", "abc");
            FloatParam = new FloatParameter(this, "FP", "FloatParam", 0.5f, "m/s", 5);
            BinParam = new BinaryParameter(this, "BP", "BinParam", new byte[] { 1, 2, 3, 4, 5 });
            ChParam = new ChoiceParameter(this, "CP", "ChParam", 0, new List<Tuple<int, string>>()
                                                                    {
                                                                        new Tuple<int, string>(0, "Ch1"),
                                                                        new Tuple<int, string>(3, "Ch2"),
                                                                        new Tuple<int, string>(5, "Ch3"),
                                                                        new Tuple<int, string>(15, "Ch4"),
                                                                    });
        }

        public TestCont3 Tc3 { get; }

        public IntParameter IntParam { get; }
        public StringParameter StrParam { get; }
        public FloatParameter FloatParam { get; }
        public BinaryParameter BinParam { get; }
        public ChoiceParameter ChParam { get; }
    }

    [DebuggerStepThrough]
    public sealed class TestCont3 : DataContainer
    {
        public TestCont3(DataContainer parent)
            : base(parent, "Tc3", "TestCont3")
        {
            BoolParam = new BoolParameter(this, "BP", "BoolParam", true);
        }

        public BoolParameter BoolParam { get; }
    }
}
