using System;

using DataTreeBase.Interfaces;
using DataTreeBase.UndoRedo;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests.UndoRedo
{
    internal static class Flags
    {
        internal static bool HasSetValue;
    }

    internal interface IUndoRedoTestNode : IUndoRedoNode
    {
        object Value { get; set; }
    }

    internal class TestDataNode : IUndoRedoTestNode
    {
        public void Set(object value)
        {
            Value = value;
            Flags.HasSetValue = true;
        }

        public object Value { get; set; }
    }

    [TestClass]
    public class UndoRedoTests
    {
        private IUndoRedoTestNode _testNode;

        [TestInitialize]
        public void Init()
        {
            _testNode = new TestDataNode();
        }

        [TestMethod]
        public void UndoAfterChangedTest()
        {
            var stack = new UndoRedoStack();
            stack.NotifyChangeEvent(_testNode, 123, 345);
            _testNode.Value = 345;

            Assert.IsTrue(stack.CanUndo, "ur.CanUndo should be true but is not");
            Assert.IsFalse(stack.CanRedo, "ur.CanRedo should be false but is not");

            Flags.HasSetValue = false;
            stack.Undo();
            Assert.IsTrue(Flags.HasSetValue, "TestDataNode has not been set but should have been");
            Assert.AreEqual(_testNode.Value, 123, "TestDataNode change has not been undone but should have been");
            Assert.IsFalse(stack.CanUndo, "ur.CanUndo should be false but is not");
            Assert.IsTrue(stack.CanRedo, "ur.CanRedo should be true but is not");
        }

        [TestMethod]
        public void UndoRedoWithManyChangesTest()
        {
            var stack = new UndoRedoStack();

            const int changeCount = 20;

            for (var i = 0; i < changeCount; i++)
            {
                stack.NotifyChangeEvent(_testNode, i, i + 1);
            }

            Assert.IsTrue(stack.CanUndo, "CanUndo should be true but is not");
            Assert.IsFalse(stack.CanRedo, "CanRedo should be false but is not");

            var z = 0;
            while (stack.CanUndo)
            {
                Flags.HasSetValue = false;
                stack.Undo();
                Assert.IsTrue(Flags.HasSetValue, "TestDataNode has not been set but should have been");
                z++;
            }
            Assert.AreEqual(z, changeCount, "Wrong number of undo's possible");
            Assert.IsTrue(stack.CanRedo, "CanRedo should be true but is not");

            z = 0;
            while (stack.CanRedo)
            {
                Flags.HasSetValue = false;
                stack.Redo();
                Assert.IsTrue(Flags.HasSetValue, "TestDataNode has not been set but should have been");
                z++;
            }
            Assert.AreEqual(z, changeCount, "Wrong number of undo's possible");
            Assert.IsTrue(stack.CanUndo, "CanUndo should be true but is not");
        }

        [TestMethod]
        public void UndoMustThrowExceptWhenStackPointerAtBeginningTest()
        {
            var stack = new UndoRedoStack();

            const int changeCount = 5;

            for (var i = 0; i < changeCount; i++)
            {
                stack.NotifyChangeEvent(_testNode, i, 2*i);
            }

            var z = 0;
            while (stack.CanUndo)
            {
                Flags.HasSetValue = false;
                stack.Undo();
                z++;
            }

            try
            {
                stack.Undo();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("InvalidOperationException occured");
                return;
            }

            Assert.Fail("InvalidOperationException expected but did not occur");
        }

        [TestMethod]
        public void RedoMustThrowExceptWhenStackPointerAtEndTest()
        {
            var stack = new UndoRedoStack();

            const int changeCount = 5;

            for (var i = 0; i < changeCount; i++)
            {
                stack.NotifyChangeEvent(_testNode, i, 2 * i);
            }

            var z = 0;
            while (stack.CanUndo)
            {
                Flags.HasSetValue = false;
                stack.Undo();
                z++;
            }

            while (stack.CanRedo)
            {
                Flags.HasSetValue = false;
                stack.Redo();
                z++;
            }

            try
            {
                stack.Redo();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("InvalidOperationException occured");
                return;
            }

            Assert.Fail("InvalidOperationException expected but did not occur");
        }

        [TestMethod]
        public void StackMustBeShortenedWhenNodeHasChangedWithStackPointerNotAtEndTest()
        {
            var stack = new UndoRedoStack();

            const int changeCount = 15;

            for (var i = 0; i < changeCount; i++)
            {
                stack.NotifyChangeEvent(_testNode, i, 2 * i);
            }

            var z = 0;
            while (z++ < 5)
            {
                Flags.HasSetValue = false;
                stack.Undo();
            }

            stack.NotifyChangeEvent(_testNode, 1111, 2222);

            Assert.IsTrue(stack.CanUndo, "CanUndo should be true but is not");
            Assert.IsFalse(stack.CanRedo, "CanRedo should be false but is not");
        }
    }
}
