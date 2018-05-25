using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataTreeBase;

using SampleModel;

namespace DataTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = Root.Create();

            model.Group2.Param4.OnChanged += OnChange;

            model.Group1.Param1.Value = 33;
            model.Group2.Param4.Value = "slkjsdfg";

            model.Group2.Param3.Value = model.Group1.Param1.Value;
        }

        static void OnChange(DataTreeParameter<string> newVal)
        {
            
        }
    }
}
