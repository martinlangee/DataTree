using System;
using System.Linq;
using DataBase.Container;

namespace DataBase.Parameters
{
    /// <summary>
    /// Represents a parameter with byte array value type
    /// </summary>
    public sealed class BinaryParameter : DataParameter<byte[]>
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="designation"></param>
        /// <param name="defaultValue"></param>
        public BinaryParameter(DataContainer parent, string id, string designation, byte[] defaultValue)
            : base(parent, id, designation, defaultValue) { }

        /// <summary>
        /// Returns true if the two byte arrays are equal
        /// </summary>
        protected override bool IsEqualValue(byte[] value1, byte[] value2) => (value1 != null) && (value2 != null) && value1.SequenceEqual(value2);

        /// <summary>
        /// Returns the base-64 representation of the value
        /// </summary>
        public override string AsString
        {
            get => Value != null ? Convert.ToBase64String(Value) : "";
            set => Value = value.Length > 0 ? Convert.FromBase64String(value) : new byte[0];
        }
    }
}
