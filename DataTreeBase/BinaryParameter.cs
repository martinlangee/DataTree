using System;
using System.Linq;

namespace DataTreeBase
{
    public sealed class BinaryParameter : DataTreeParameter<byte[]>
    {
        public BinaryParameter(DataTreeContainer parent, string id, string name, byte[] defaultValue)
            : base(parent, id, name, defaultValue)
        {
        }

        protected override bool IsEqualValue(byte[] value1, byte[] value2)
        {
            return (value1 != null) && (value2 != null) && value1.SequenceEqual(value2);
        }

        public override string AsString
        {
            get { return Convert.ToBase64String(Value); }
            internal set { Value = Convert.FromBase64String(value); }
        }
    }
}
