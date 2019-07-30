using System;
using DataBase.Container;

namespace DataBase.Parameters
{
    /// <summary>
    /// Represents a parameter with string value type
    /// </summary>
    public sealed class StringParameter: DataParameter<string>
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Parameter identificator</param>
        /// <param name="designation">Parameter name</param>
        /// <param name="defaultValue">Float parameter default value</param>
        public StringParameter(DataContainer parent, string id, string designation, string defaultValue)
            : base(parent, id, designation, defaultValue) { }

        /// <summary>
        /// Gets or sets the string representation of the value
        /// </summary>
        public override string AsString
        {
            get => Value;
            set => Value = value;
        }
    }
}
