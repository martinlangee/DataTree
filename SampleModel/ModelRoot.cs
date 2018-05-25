using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataTreeBase;

namespace SampleModel
{
    public sealed class Root: DataTreeRoot
    {
        public static Root Create()
        {
            return new Root();
        }

        private Root()
        {
            Group1 = new Group1(this);
            Group2 = new Group2(this);
        }

        public Group1 Group1 { get; }
        public Group2 Group2 { get; }
    }
}
