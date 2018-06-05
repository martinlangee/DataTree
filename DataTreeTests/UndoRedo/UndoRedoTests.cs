using System;

using DataTreeBase;
using DataTreeBase.Interfaces;
using DataTreeBase.UndoRedo;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTreeTests.UndoRedo
{
    internal static class Flags
    {
        internal static bool HasSetValue;
        internal static bool IsCanUndoRedoChangedCalled;
    }

    internal interface IUndoRedoableTestNode : IUndoRedoableNode
    {
        UndoRedoStack Stack { get; }
        int Value { get; set; }
    }

    internal sealed class TestDataNode : IUndoRedoableTestNode
    {
        private int _value;

        internal TestDataNode()
        {
            Stack = new UndoRedoStack();
            _value = 0;
        }

        public UndoRedoStack Stack { get; }

        public void Set(object value)
        {
            Value = (int) value;
            Flags.HasSetValue = true;
        }

        public int Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;

                var oldValue = Value;
                _value = value;
                Stack.ValueChanged(this, oldValue, _value);
            }
        }
    }

    [TestClass]
    public class UndoRedoTests
    {
        private IUndoRedoableTestNode _testNode;

        [TestInitialize]
        public void Init()
        {
            _testNode = new TestDataNode();
        }

        [TestMethod]
        public void UndoAfterChangedTest()
        {
            _testNode.Value = 345;

            Assert.IsTrue(_testNode.Stack.CanUndo, "CanUndo should be true but is not");
            Assert.IsFalse(_testNode.Stack.CanRedo, "CanRedo should be false but is not");

            Flags.HasSetValue = false;
            _testNode.Stack.Undo();
            Assert.IsTrue(Flags.HasSetValue, "TestDataNode has not been set but should have been");
            Assert.AreEqual(_testNode.Value, 0, "TestDataNode change has not been undone but should have been");
            Assert.IsFalse(_testNode.Stack.CanUndo, "CanUndo should be false but is not");
            Assert.IsTrue(_testNode.Stack.CanRedo, "CanRedo should be true but is not");
        }

        [TestMethod]
        public void ClearStackTest()
        {
            Assert.AreEqual(_testNode.Stack.Count, 0, "UndoRedoStack has not been cleared");
            Assert.AreEqual(_testNode.Stack.Pointer, -1, "UndoRedoStack has not been cleared");
            Assert.IsFalse(_testNode.Stack.CanUndo, "CanUndo should be false but is not");
            Assert.IsFalse(_testNode.Stack.CanRedo, "CanRedo should be false but is not");

            20.TimesDo(i =>
            {
                _testNode.Value = i + 1;
            });

            Assert.AreEqual(_testNode.Stack.Count, 20, "Stack count not as expected");
            Assert.AreEqual(_testNode.Stack.Pointer, 19, "Stack pointer not as expected");
            Assert.IsTrue(_testNode.Stack.CanUndo, "CanUndo should be true but is not");
            Assert.IsFalse(_testNode.Stack.CanRedo, "CanRedo should be false but is not");

            _testNode.Stack.Clear();

            Assert.AreEqual(_testNode.Stack.Count, 0, "Stack count not as expected");
            Assert.AreEqual(_testNode.Stack.Pointer, -1, "Stack pointer not as expected");
            Assert.IsFalse(_testNode.Stack.CanUndo, "CanUndo should be false but is not");
            Assert.IsFalse(_testNode.Stack.CanRedo, "CanRedo should be false but is not");

            15.TimesDo(i =>
            {
                _testNode.Value = i + 1;
            });

            10.TimesDo(i =>
            {
                _testNode.Stack.Undo();
            });


            3.TimesDo(i =>
            {
                _testNode.Stack.Redo();
            });

            Assert.AreEqual(_testNode.Stack.Count, 15, "Stack count not as expected");
            Assert.AreEqual(_testNode.Stack.Pointer, 7, "Stack pointer not as expected");
            Assert.IsTrue(_testNode.Stack.CanUndo, "CanUndo should be true but is not");
            Assert.IsTrue(_testNode.Stack.CanRedo, "CanRedo should be true but is not");

            _testNode.Stack.Clear();

            Assert.AreEqual(_testNode.Stack.Count, 0, "Stack count not as expected");
            Assert.AreEqual(_testNode.Stack.Pointer, -1, "Stack pointer not as expected");
            Assert.IsFalse(_testNode.Stack.CanUndo, "CanUndo should be false but is not");
            Assert.IsFalse(_testNode.Stack.CanRedo, "CanRedo should be false but is not");
        }

        [TestMethod]
        public void UndoRedoWithManyChangesTest()
        {
            const int changeCount = 20;

            changeCount.TimesDo(i =>
                              {
                                  _testNode.Value = i + 1;
                              });

            Assert.IsTrue(_testNode.Stack.CanUndo, "CanUndo should be true but is not");
            Assert.IsFalse(_testNode.Stack.CanRedo, "CanRedo should be false but is not");

            var z = 0;
            while (_testNode.Stack.CanUndo)
            {
                Flags.HasSetValue = false;
                _testNode.Stack.Undo();
                Assert.IsTrue(Flags.HasSetValue, "TestDataNode has not been set but should have been");
                Assert.AreEqual(_testNode.Value, changeCount - z - 1);
                z++;
            }
            Assert.AreEqual(z, changeCount, "Wrong number of possible undo's");
            Assert.IsTrue(_testNode.Stack.CanRedo, "CanRedo should be true but is not");

            z = 0;
            while (_testNode.Stack.CanRedo)
            {
                Flags.HasSetValue = false;
                _testNode.Stack.Redo();
                Assert.AreEqual(_testNode.Value, z + 1);
                Assert.IsTrue(Flags.HasSetValue, "TestDataNode has not been set but should have been");
                z++;
            }
            Assert.AreEqual(z, changeCount, "Wrong number of possible undo's");
            Assert.IsTrue(_testNode.Stack.CanUndo, "CanUndo should be true but is not");
        }

        [TestMethod]
        public void UndoMustThrowExceptWhenStackPointerAtBeginningTest()
        {
            70.TimesDo(i =>
                    {
                        _testNode.Value = i + 456;
                    });

            while (_testNode.Stack.CanUndo)
            {
                Flags.HasSetValue = false;
                _testNode.Stack.Undo();
            }

            try
            {
                _testNode.Stack.Undo();
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
            13.TimesDo(i =>
                     {
                         _testNode.Value = i + 5678;
                     });

            while (_testNode.Stack.CanUndo)
            {
                Flags.HasSetValue = false;
                _testNode.Stack.Undo();
            }

            while (_testNode.Stack.CanRedo)
            {
                Flags.HasSetValue = false;
                _testNode.Stack.Redo();
            }

            try
            {
                _testNode.Stack.Redo();
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
            15.TimesDo(i =>
                     {
                         _testNode.Value = i + 67890;
                     });

            5.TimesDo(i =>
                    {
                        Flags.HasSetValue = false;
                        _testNode.Stack.Undo();
                    });

            Assert.AreEqual(_testNode.Stack.Count, 15, "Stack count not as expected");
            Assert.AreEqual(_testNode.Stack.Pointer, 9, "Stack pointer not as expected");

            _testNode.Value = 2222;

            Assert.AreEqual(_testNode.Stack.Count, 11, "Stack count not as expected");
            Assert.AreEqual(_testNode.Stack.Pointer, 10, "Stack pointer not as expected");

            Assert.IsTrue(_testNode.Stack.CanUndo, "CanUndo should be true but is not");
            Assert.IsFalse(_testNode.Stack.CanRedo, "CanRedo should be false but is not");

            5.TimesDo(i =>
                    {
                        Flags.HasSetValue = false;
                        _testNode.Stack.Undo();
                    });

            Assert.AreEqual(_testNode.Stack.Count, 11, "Stack count not as expected");
            Assert.AreEqual(_testNode.Stack.Pointer, 5, "Stack pointer not as expected");

            _testNode.Value = 45764576;

            Assert.AreEqual(_testNode.Stack.Count, 7, "Stack count not as expected");
            Assert.AreEqual(_testNode.Stack.Pointer, 6, "Stack pointer not as expected");

            Assert.IsTrue(_testNode.Stack.CanUndo, "CanUndo should be true but is not");
            Assert.IsFalse(_testNode.Stack.CanRedo, "CanRedo should be false but is not");
        }

        [TestMethod]
        public void CanUndoRedoChangedEventTest()
        {
            _testNode.Stack.CanUndoRedoChanged += OnCanUndoRedoChanged;

            Flags.IsCanUndoRedoChangedCalled = false;
            _testNode.Value = 22;
            Assert.IsTrue(Flags.IsCanUndoRedoChangedCalled, "OnCanUndoRedoChanged call missing");

            Flags.IsCanUndoRedoChangedCalled = false;
            _testNode.Value = 333;
            Assert.IsFalse(Flags.IsCanUndoRedoChangedCalled, "OnCanUndoRedoChanged called but should not");

            Flags.IsCanUndoRedoChangedCalled = false;
            _testNode.Value = 4444;
            Assert.IsFalse(Flags.IsCanUndoRedoChangedCalled, "OnCanUndoRedoChanged called but should not");

            Flags.IsCanUndoRedoChangedCalled = false;
            _testNode.Stack.Undo();
            Assert.IsTrue(Flags.IsCanUndoRedoChangedCalled, "OnCanUndoRedoChanged call missing");

            Flags.IsCanUndoRedoChangedCalled = false;
            _testNode.Stack.Undo();
            Assert.IsFalse(Flags.IsCanUndoRedoChangedCalled, "OnCanUndoRedoChanged called but should not");

            Flags.IsCanUndoRedoChangedCalled = false;
            _testNode.Stack.Undo();
            Assert.IsTrue(Flags.IsCanUndoRedoChangedCalled, "OnCanUndoRedoChanged call missing");
        }

        private void OnCanUndoRedoChanged()
        {
            Flags.IsCanUndoRedoChangedCalled = true;
        }
    }
}
