using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTreeBase
{
    public sealed class StringParameter: DataTreeParameter<string>
    {
        public StringParameter(DataTreeContainer parent, string id, string name, string defaultValue)
            : base(parent, id, name, defaultValue)
        {
        }

        public override string AsString
        {
            get { return Value; }
            internal set { Value = value; }
        }
    }
}
