#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
#endregion

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
        /// <param name="designation">Node name</param>
        protected DataNode(DataContainer parent, string id, string designation = "")
        {
            if (id.Contains(XmlHelper.PathDelimiter))
            {
                throw new ArgumentOutOfRangeException($"DataNode: node id may not contain path delimiter '{XmlHelper.PathDelimiter}'");
            }
            Parent = parent;
            Id = id;
            Designation = string.IsNullOrEmpty(designation) ? id : designation;
        }

        /// <summary>
        /// Returns the node identificator used for serialization
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Returns the full slash-seperated path of ids containing all parent ids
        /// </summary>
        public virtual string PathId => (Parent != null ? Parent.PathId + $"{XmlHelper.PathDelimiter}" : "") + Id;

        /// <summary>
        /// Returns the Node name
        /// </summary>
        public string Designation { get; internal set; }

        /// <summary>
        /// Returns true if the node has been modified since last modified reset
        /// </summary>
        public abstract bool IsModified { get; }

        /// <summary>
        /// Gets or sets the parent container.
        /// Setting the parent container relocates the node and all of it's sub-nodes.
        /// Setting it to null detaches it from the data tree.
        /// </summary>
        internal DataContainer Parent { get; }

        /// <summary>
        /// Resets the modified flag to false by setting the buffered value to the current value
        /// </summary>
        public abstract void ResetModifiedState();

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