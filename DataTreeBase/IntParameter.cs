using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTreeBase
{
    public class IntParameter : DataTreeParameter<int>
    {
        public IntParameter(DataTreeContainer parent, string id, string name, int defaultValue)
            : base(parent, id, name, defaultValue)
        {
        }

        protected override bool IsEqualValue(int value1, int value2)
        {
            return value1 == value2;
        }

        public override string AsString
        {
            get { return Value.ToString(); }
            internal set
            {
                int intVal;
                if (int.TryParse(value, out intVal))
                    Value = intVal;
            }
        }
    }
}
