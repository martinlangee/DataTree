using System;

using DataTreeBase;

using SampleModel;

namespace DataTreeHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = Root.Create();

            model.LoadFromFile("D://DataTree.xml");

            model.Group2.StrParam4.OnChanged += OnChange;

            model.Group1.IntParam.Value = 33;
            model.Group2.StrParam4.Value = "slkjsdfg";

            model.Group2.IntParam3.Value = model.Group1.IntParam.Value;

            model.Group2.BoolParam5.Value = false;
            model.Group2.ChParam6.ValueIdx = 2;
            model.Group2.BinParam7.Value = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 0 };

            model.SaveToFile("D://DataTree.xml");
        }

        static void OnChange(DataTreeParameter<string> newVal)
        {
            
        }
    }
}
