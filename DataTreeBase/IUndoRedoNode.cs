using System;

namespace DataTreeBase
{
    /// <summary>
    /// Interface unifying the access to the Undo and Redo command of parameters and containers
    /// </summary>
    internal interface IUndoRedoNode
    {
        void Undo(object oldValue);
        void Redo(object newValue);
    }
}