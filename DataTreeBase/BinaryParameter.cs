using System;
using System.Linq;

namespace DataTreeBase
{
    /// <summary>
    /// Represents a parameter with byte array value type
    /// </summary>
    public sealed class BinaryParameter : DataTreeParameter<byte[]>
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public BinaryParameter(DataTreeContainer parent, string id, string name, byte[] defaultValue)
            : base(parent, id, name, defaultValue)
        {
        }

        /// <summary>
        /// Returns true if the two byte arrays are equal
        /// </summary>
        protected override bool IsEqualValue(byte[] value1, byte[] value2)
        {
            return (value1 != null) && (value2 != null) && value1.SequenceEqual(value2);
        }

        /// <summary>
        /// Returns the base-64 representation of the value
        /// </summary>
        public override string AsString
        {
            get { return Convert.ToBase64String(Value); }
            set { Value = Convert.FromBase64String(value); }
        }
    }
}
