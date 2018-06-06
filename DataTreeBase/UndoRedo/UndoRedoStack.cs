using System;
using System.Collections.Generic;
using System.Diagnostics;

using DataTreeBase.Interfaces;

namespace DataTreeBase.UndoRedo
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
        private int _pointer = -1;
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
                _stack[_pointer].Node.Set(value);
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
            CanUndo = (_stack.Count > 0) && (_pointer >= 0);
            CanRedo = (_stack.Count > 0) && (_pointer < (_stack.Count - 1));
        }

        /// <summary>
        /// The last change made to the data tree is reverted
        /// </summary>
        public void Undo()
        {
            if (_pointer < 0)
                throw new InvalidOperationException("DataTreeContainer.Undo: pointer to undo stack already at lower limit");

            Debug.WriteLine("Undo: OldValue=" + _stack[_pointer].OldValue + " Ptr=" + _pointer);
            SafeSetValue(_stack[_pointer].OldValue);
            _pointer--;

            UpdateCanUndoRedo();
        }

        /// <summary>
        /// The last undo action made to the data tree is reverted
        /// </summary>
        public void Redo()
        {
            if (_pointer >= (_stack.Count - 1))
                throw new InvalidOperationException("DataTreeContainer.Undo: pointer to redo stack already at upper limit");

            _pointer++;
            Debug.WriteLine("Redo: NewValue=" + _stack[_pointer].NewValue + " Ptr=" + _pointer);
            SafeSetValue(_stack[_pointer].NewValue);

            UpdateCanUndoRedo();
        }

        /// <summary>
        /// Returns true if the any change to the data tree can be reverted
        /// </summary>
        public bool CanUndo
        {
            get { return _canUndo; }
            private set
            {
                if (CanUndo == value) return; 

                _canUndo = value;
                CanUndoRedoChanged.Invoke();
            }
        }

        /// <summary>
        /// Returns true if any formerly undone change to the data tree can be redone
        /// </summary>
        public bool CanRedo
        {
            get { return _canRedo; }
            private set
            {
                if (CanRedo == value) return;

                _canRedo = value;
                CanUndoRedoChanged.Invoke();
            }
        }

        /// <summary>
        /// Event fired when either the CanUndo or then CanRedo propertry has changed it's state
        /// </summary>
        public event Action CanUndoRedoChanged = () => { };

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
            if (_stack.Count > 0 && _pointer < (_stack.Count - 1))
                _stack.RemoveRange(_pointer + 1, _stack.Count - _pointer - 1);

            var undoItem = new UndoRedoItem()
                           {
                               Node = dataNode,
                               OldValue = oldValue,
                               NewValue = newValue
                           };
            _stack.Add(undoItem);
            _pointer++;

            UpdateCanUndoRedo();

            Debug.WriteLine("ValueChanged: " + undoItem.Node + " ptr=" + _pointer);
        }

        /// <summary>
        /// Clears the undo/redo stack
        /// </summary>
        public void Clear()
        {
            _stack.Clear();
            _pointer = -1;

            UpdateCanUndoRedo();
        }

        /// <summary>
        /// If set the undo/redo stack is not updated
        /// </summary>
        internal bool IsMuted { private get; set; }

        // for testing purposes only
        internal int Count => _stack.Count;
        internal int Pointer => _pointer;
    }
}