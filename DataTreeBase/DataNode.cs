using System;
using System.Diagnostics;
using DataBase.Container;

namespace DataBase
{
    // ok: dynamische Container-Listen
    // ok: Parameter-Unit-Tests schreiben
    // canceled: Buffer arbeitet mit Stack
    // ok: Undo/Redo
    // todo: Container-Unit-Tests schreiben
    // todo: Strings: UTF8?
    // todo: Unit-Header mit copyright etc.
    // todo: CRC-Check
    // todo: Thread-Sicherheit: auf Container-Ebene, Methode zum Container-Klonen einführen (v)
    // todo: Anbindung an UI über Proxy oder DTO
    // todo: Technik zur DTO-Initialisierung überlegen
    // todo: DB zur Serialisierung; mit Tabellen-Generierung?
    // todo: auf Github veröffentlichen

    /// <summary>
    /// Abstract base node class
    /// </summary>
    [DebuggerDisplay("{GetType().Name}, {PathId}, {IsModified}")]
    public abstract class DataNode
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Node identificator</param>
        /// <param name="name">Node name</param>
        protected DataNode(DataContainer parent, string id, string name)
        {
            if (id.Contains(XmlHelper.PathDelimiter))
                throw new ArgumentOutOfRangeException($"DataNode: node id may not contain path delimiter '{XmlHelper.PathDelimiter}'");

            Parent = parent;
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the parent container.
        /// Setting the parent container relocates the node and all of it's sub-nodes.
        /// Setting it to null detaches it from the data tree.
        /// </summary>
        internal DataContainer Parent { get; }

        /// <summary>
        /// Returns the node identificator used for serialization
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Returns the full slash-seperated path of ids containing all parent ids
        /// </summary>
        public virtual string PathId => (Parent != null ? Parent.PathId + $"{XmlHelper.PathDelimiter}" : "") + Id;

        /// <summary>
        /// Returns the code name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns true if the node has been modified since last modified reset
        /// </summary>
        public abstract bool IsModified { get; }

        /// <summary>
        /// Resets the modified flag to false by setting the buffered value to the current value
        /// </summary>
        public abstract void ResetModified();

        /// <summary>
        /// Sets the value to the buffered value
        /// </summary>
        public abstract void Restore();

        /// <summary>
        /// Sets the value to the default value
        /// </summary>
        public abstract void SetToDefault();
    }
}