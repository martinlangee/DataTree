using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataTreeBase;

namespace SampleModel
{
    public sealed class Group2: DataTreeContainer
    {
        public Group2(DataTreeContainer parent)
            : base(parent, "Group2", "Group2")
        {
            Param3 = new IntParameter(this, "Param3", "Param3", 0);
            Param4 = new StringParameter(this, "Param4", "Param4", "def");
        }

        public IntParameter Param3 { get; }
        public StringParameter Param4 { get; }
    }
}
