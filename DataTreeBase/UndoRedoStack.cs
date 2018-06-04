using System;
using System.Collections.Generic;

namespace DataTreeBase
{
    public sealed class UndoRedoStack
    {
        /// <summary>
        /// Container for a single undo/redo item
        /// </summary>
        internal sealed class UndoRedoItem
        {
            public IUndoRedoNode Node { get; set; }
            public object OldValue { get; set; }
            public object NewValue { get; set; }
        }

        private bool _undoRedoing;
        private int _pointer = -1;
        private readonly List<UndoRedoItem> _stack = new List<UndoRedoItem>();

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
        /// The last change made to the data tree is reverted
        /// </summary>
        public void Undo()
        {
            if (_pointer < 0)
                throw new InvalidOperationException("DataTreeContainer.Undo: pointer to undo stack already at lower limit");

            SafeSetValue(_stack[_pointer].OldValue);
            _pointer--;

            if ((_pointer == 0) || (_pointer < (_stack.Count - 1)))
                CanUndoRedoChanged.Invoke();

        }

        /// <summary>
        /// The last undo action made to the data tree is reverted
        /// </summary>
        public void Redo()
        {
            if (_pointer >= (_stack.Count - 1))
                throw new InvalidOperationException("DataTreeContainer.Undo: pointer to redo stack already at upper limit");

            _pointer++;
            SafeSetValue(_stack[_pointer].NewValue);

            if ((_pointer > 0) || (_pointer) == (_stack.Count - 1))
                CanUndoRedoChanged.Invoke();
        }

        /// <summary>
        /// Returns true if the any change to the data tree can be reverted
        /// </summary>
        public bool CanUndo => (_stack.Count > 0) && (_pointer >= 0);

        /// <summary>
        /// Returns true if any formerly undone change to the data tree can be redone
        /// </summary>
        public bool CanRedo => (_stack.Count > 0) && (_pointer < (_stack.Count - 1));

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
        internal void NotifyChangeEvent(IUndoRedoNode dataNode, object oldValue, object newValue)
        {
            if (_undoRedoing || IsMuted)
                return;

            // clear "Redo"-Entries when new change has occurred
            if (_stack.Count > 0 && _pointer < (_stack.Count - 1))
                _stack.RemoveRange(_pointer + 1, _stack.Count - _pointer - 1);

            _stack.Add(new UndoRedoItem()
                       {
                           Node = dataNode,
                           OldValue = oldValue,
                           NewValue = newValue
                       });
            _pointer++;
        }

        /// <summary>
        /// Clears the undo/redo stack
        /// </summary>
        public void Clear()
        {
            _stack.Clear();
            _pointer = -1;
        }

        /// <summary>
        /// If set the undo/redo stack is not updated
        /// </summary>
        internal bool IsMuted { private get; set; }
    }
}