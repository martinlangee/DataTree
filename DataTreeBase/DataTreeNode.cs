namespace DataTreeBase
{
    // ok: dynamische Container-Listen
    // ok: Parameter-Unit-Tests schreiben
    // canceled: Buffer arbeitet mit Stack
    // canceled: Thread-Sicherheit: auf Container-Ebene: Methode zum Container-Klonen einführen (v)
    // todo: Undo/Redo
    // todo: Container-Unit-Tests schreiben
    // todo: Strings: UTF8?
    // todo: Import/Export von einzelnen Containern 
    // todo: CRC-Check
    // todo: Anbindung an UI über Proxy oder DTO
    // todo: Technik zur DTO-Initialisierung überlegen
    // todo: DB zur Serialisierung; mit Tabellen-Generierung?

    /// <summary>
    /// Abstract base node class
    /// </summary>
    public abstract class DataTreeNode
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Node identificator</param>
        /// <param name="name">Node name</param>
        protected DataTreeNode(DataTreeContainer parent, string id, string name)
        {
            Parent = parent;
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the parent container.
        /// Setting the parent container relocates the node and all of it's sub-nodes.
        /// Setting it to null detaches it from the data tree.
        /// </summary>
        protected DataTreeContainer Parent { get; }

        /// <summary>
        /// Returns the node identificator used for serialization
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Returns the full slash-seperated path of ids containing all parent ids
        /// </summary>
        public virtual string PathId => (Parent != null ? Parent.PathId + $"{Helper.PathDelimiter}" : "") + Id;

        /// <summary>
        /// Returns the code name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns true if the node has been modified since last modified reset
        /// </summary>
        public abstract bool IsModified { get; }
    }
}