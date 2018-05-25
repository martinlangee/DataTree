using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using DataTreeBase;

namespace SampleModel
{
    public sealed class Group1: DataTreeContainer
    {
        public Group1(DataTreeContainer parent)
            : base(parent, "Group1", "Group1")
        {
            Param1 = new IntParameter(this, "Param1", "Param1", 0);
            Param2 = new StringParameter(this, "Param2", "Param2", "abc");
        }

        public IntParameter Param1 { get; }
        public StringParameter Param2 { get; }
    }
}
