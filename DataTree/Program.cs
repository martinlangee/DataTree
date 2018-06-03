using System;
using System.Diagnostics;

using DataTreeBase;

using SampleModel;

namespace DataTreeHost
{
    class Program
    {
        static Root model = new Root();

        static void Main(string[] args)
        {
            model.LoadFromFile("D://DataTree.xml");

//            model.Cont2.StrParam4.OnChanged += OnChange;

            model.UndoRedo.CanUndoRedoChanged += ModelOnCanUndoRedoChanged;

            model.Cont1.IntParam.Value = model.Cont1.IntParam.Value + 33;
            model.Cont1.StrParam.Value = "xxxx" + model.Cont1.StrParam.Value;
            model.Cont1.FloatParam1.Value = model.Cont1.FloatParam1.Value + 1.3;

            while (model.UndoRedo.CanUndo)
            {
                model.UndoRedo.Undo();
            }

            while (model.UndoRedo.CanRedo)
            {
                model.UndoRedo.Redo();
            }

            model.Cont2.StrParam4.Value = "eeeeee";
            model.Cont2.StrParam4.Value = "eeeeedsdfsdfsf";
            var mod = model.Cont2.StrParam4.IsModified;
            var n = model.Cont2.StrParam4.Name;
            var i = model.Cont2.StrParam4.Id;

            model.Cont2.IntParam3.Value = model.Cont1.IntParam.Value;

            model.Cont2.BoolParam5.Value = false;
            model.Cont2.ChParam6.ValueIdx = 2;
            model.Cont2.BinParam7.Value = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 0 };

            model.Cont2.Add(d =>
            {
                d.BoolParam6.Value = true;
                d.IntParam4.Value = 3333;
                d.StrParam5.Value = "adöfgsödlfkjgölds";
            });
            model.Cont2.Add(d =>
            {
                d.BoolParam6.Value = false;
                d.IntParam4.Value = 77;
                d.StrParam5.Value = "xxxxxxxxxxx";
            });

            model.Cont2.DynContainers[1].StrParam5.Value = "nonononononon";
            var p = model.Cont2.DynContainers[1].PathId;
            //model.Cont2.

            var clone = model.Clone() as Root;
            clone.Cont1.FloatParam1.Value = 99999999.1234;
            clone.SaveToFile("D://DataTreeClone.xml");

            model.SaveToFile("D://DataTree.xml");
        }

        private static void ModelOnCanUndoRedoChanged()
        {
            Console.WriteLine("ModelOnCanUndoRedoChanged:" + model.UndoRedo.CanUndo + "/" + model.UndoRedo.CanRedo);
        }

        static void OnChange(DataTreeParameterBase param)
        {
            Console.WriteLine("Parameter " + param.PathId + " Val=" + param.AsString);
        }
    }
}
