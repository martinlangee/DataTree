using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTreeBase
{
    public class StringParameter: DataTreeParameter<string>
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

        protected override bool IsEqualValue(string value1, string value2)
        {
            return string.Equals(value1, value2);
        }
    }
}
