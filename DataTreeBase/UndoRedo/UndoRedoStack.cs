using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataBase.Interfaces;

namespace DataBase.UndoRedo
{
    /// <summary>
    /// Handling the undo/redo actions and states
    /// </summary>
    public sealed class UndoRedoStack
    {
        /// <summary>
        /// Container for a single undo/redo item
        /// </summary>
        internal sealed class UndoRedoItem
        {
            public IUndoRedoableNode Node { get; set; }
            public object OldValue { get; set; }
            public object NewValue { get; set; }
        }

        private bool _undoRedoing;
        private readonly List<UndoRedoItem> _stack = new List<UndoRedoItem>();
        private bool _canUndo;
        private bool _canRedo;

        public UndoRedoStack()
        {
            UpdateCanUndoRedo();
        }

        /// <summary>
        /// Set the specified value avoiding recursive loop
        /// </summary>
        private void SafeSetValue(object value)
        {
            if (_undoRedoing)
                return;

            _undoRedoing = true;
            try
            {
                _stack[Pointer].Node.Set(value);
            }
            finally
            {
                _undoRedoing = false;
            }
        }

        /// <summary>
        /// Updates the state of the CanUndo and CanRedo properties
        /// </summary>
        private void UpdateCanUndoRedo()
        {
            CanUndo = (_stack.Count > 0) && (Pointer >= 0);
            CanRedo = (_stack.Count > 0) && (Pointer < (_stack.Count - 1));

            UndoRedoListChanged?.Invoke();
        }

        /// <summary>
        /// The last change made to the data tree is reverted
        /// </summary>
        public void Undo(int count = 1)
        {
            if (Pointer < 0)
                throw new InvalidOperationException("DataContainer.Undo: pointer to undo stack already at lower limit");

            count.TimesDo(i =>
            {
                Debug.WriteLine("Undo: OldValue=" + _stack[Pointer].OldValue + " Ptr=" + Pointer);
                SafeSetValue(_stack[Pointer].OldValue);
                Pointer--;

                UpdateCanUndoRedo();
            });
        }

        /// <summary>
        /// The last undo action made to the data tree is reverted
        /// </summary>
        public void Redo(int count = 1)
        {
            if (Pointer >= (_stack.Count - 1))
                throw new InvalidOperationException("DataContainer.Undo: pointer to redo stack already at upper limit");

            count.TimesDo(i =>
            {
                Pointer++;
                Debug.WriteLine("Redo: NewValue=" + _stack[Pointer].NewValue + " Ptr=" + Pointer);
                SafeSetValue(_stack[Pointer].NewValue);

                UpdateCanUndoRedo();
            });
        }

        /// <summary>
        /// Returns true if the any change to the data tree can be reverted
        /// </summary>
        public bool CanUndo
        {
            get => _canUndo;
            private set
            {
                if (CanUndo == value) return; 

                _canUndo = value;
                CanUndoRedoChanged?.Invoke();
            }
        }

        /// <summary>
        /// Returns true if any formerly undone change to the data tree can be redone
        /// </summary>
        public bool CanRedo
        {
            get => _canRedo;
            private set
            {
                if (CanRedo == value) return;

                _canRedo = value;
                CanUndoRedoChanged?.Invoke();
            }
        }

        /// <summary>
        /// Event fired when either the CanUndo or then CanRedo property has changed it's state
        /// </summary>
        public event Action CanUndoRedoChanged;

        /// <summary>
        /// Event fired when either the UndoList and/or then RedoList has changed
        /// </summary>
        public event Action UndoRedoListChanged;

        /// <summary>
        /// Stores the change of the specified paramerter with it's old and new value in the undo/redo stack
        /// </summary>
        /// <param name="dataNode">The data node who's value was changed</param>
        /// <param name="oldValue">The former value of the parameter</param>
        /// <param name="newValue">The new value of the parameter</param>
        internal void ValueChanged(IUndoRedoableNode dataNode, object oldValue, object newValue)
        {
            if (_undoRedoing || IsMuted)
                return;

            // clear "Redo"-Entries when new change has occurred
            if (_stack.Count > 0 && Pointer < (_stack.Count - 1))
                _stack.RemoveRange(Pointer + 1, _stack.Count - Pointer - 1);

            var undoItem = new UndoRedoItem()
                           {
                               Node = dataNode,
                               OldValue = oldValue,
                               NewValue = newValue
                           };
            _stack.Add(undoItem);
            Pointer++;

            UpdateCanUndoRedo();

            Debug.WriteLine("ValueChanged: " + undoItem.Node + " ptr=" + Pointer);
        }

        /// <summary>
        /// Clears the undo/redo stack
        /// </summary>
        public void Clear()
        {
            _stack.Clear();
            Pointer = -1;

            UpdateCanUndoRedo();
        }

        /// <summary>
        /// Returns a list of the changes on the undo stack that can be undone
        /// </summary>
        public IList<string> UndoList
        {
            get
            {
                var result = new List<string>();
                if (Pointer < 0) return result;

                for (var i = Pointer; i >= 0; i--)
                {
                    result.Add($"{_stack[i].Node.Name}: {_stack[i].NewValue} -> {_stack[i].OldValue}");
                }

                return result;
            }
        }

        /// <summary>
        /// Returns a list of the changes on the redo stack that can be undone
        /// </summary>
        public IList<string> RedoList
        {
            get
            {
                var result = new List<string>();

                for (var i = Pointer + 1; i < _stack.Count; i++)
                {
                    result.Add($"{_stack[i].Node.Name}: {_stack[i].OldValue} -> {_stack[i].NewValue}");
                }

                return result;
            }
        }

        /// <summary>
        /// If set the undo/redo stack is not updated
        /// </summary>
        internal bool IsMuted { private get; set; }


        /// <summary>
        /// Returns the current state of the stack pointer
        /// </summary>
        public int Pointer { get; private set; } = -1;

        // for testing purposes only
        internal int Count => _stack.Count;
    }
}