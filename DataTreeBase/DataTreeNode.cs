namespace DataTreeBase
{
    // ok: dynamische Container-Listen
	// todo: Import/Export
	// todo: CRC-Check
    // todo: Unit-Tests schreiben
    // todo: Anbindung an UI über Proxy oder DTO
    // todo: Technik zur DTO-Initialisierung überlegen
    // todo: Thread-Sicherheit
    // todo: Rückwärtsgang
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
        public DataTreeNode(DataTreeContainer parent, string id, string name)
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
        public DataTreeContainer Parent { get; }

        /// <summary>
        /// Returns the node identificator used for serialization
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Returns the full slash-seperated path of ids containing all parent ids
        /// </summary>
        public virtual string PathId => (Parent != null ? Parent.PathId + @"\": "") + Id;

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