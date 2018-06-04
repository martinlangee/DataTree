using System;

namespace DataTreeBase.Interfaces
{
    /// <summary>
    /// Interface unifying the access from undo/redo stack to the parameters and dynamic containers inorder to set the undo- or redo-value
    /// </summary>
    internal interface IUndoRedoNode
    {
        /// <summary>
        /// Set the new value as result of the undo or redo process
        /// </summary>
        void Set(object value);
    }
}