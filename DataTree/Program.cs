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

            model.LoadFromFile("D://DataTree.xml");

            model.Group2.Param4.OnChanged += OnChange;

            model.Group1.IntParam.Value = 33;
            model.Group2.Param4.Value = "slkjsdfg";

            model.Group2.Param3.Value = model.Group1.IntParam.Value;

            model.SaveToFile("D://DataTree.xml");
        }

        static void OnChange(DataTreeParameter<string> newVal)
        {
            
        }
    }
}
