namespace DataTreeBase
{
    /// <summary>
    /// Represents a parameter with string value type
    /// </summary>
    public sealed class StringParameter: DataTreeParameter<string>
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Parameter identificator</param>
        /// <param name="name">Parameter name</param>
        /// <param name="defaultValue">Float parameter default value</param>
        public StringParameter(DataTreeContainer parent, string id, string name, string defaultValue)
            : base(parent, id, name, defaultValue)
        {
        }

        /// <summary>
        /// Gets or sets the string representation of the value
        /// </summary>
        public override string AsString
        {
            get { return Value; }
            set { Value = value; }
        }
    }
}
