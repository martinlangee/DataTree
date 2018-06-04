using System;

namespace DataTreeBase
{
    /// <summary>
    /// Interface unifying the access to the Undo and Redo command of parameters and dynamic containers
    /// </summary>
    internal interface IUndoRedoNode
    {
        /// <summary>
        /// Triggering this node to load the specified value
        /// </summary>
        void Set(object value);
    }
}