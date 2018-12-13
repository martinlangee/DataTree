using System;

namespace DataBase.Interfaces
{
    /// <summary>
    /// Interface unifying the access from undo/redo stack to the parameters and dynamic containers inorder to set the undo- or redo-value
    /// </summary>
    internal interface IUndoRedoableNode
    {
        /// <summary>
        /// Returns the code name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Set the new value as result of the undo or redo process
        /// </summary>
        void Set(object value);
    }
}