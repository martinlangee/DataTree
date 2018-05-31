using System;
using System.Collections.Generic;

using DataTreeBase;

namespace SampleModel
{
    public sealed class Cont2 : DataTreeDynParentContainer<DynContainer>
    {
        public Cont2(DataTreeContainer parent)
            : base(parent, "Cont2", "Cont2")
        {
            IntParam3 = new IntParameter(this, "IntParam3", "IntParam3", 0);
            StrParam4 = new StringParameter(this, "StrParam4", "StrParam4", "def");
            BoolParam5 = new BoolParameter(this, "BoolParam5", "BoolParam5", true);
            ChParam6 = new ChoiceParameter(this, "ChParam6", "ChParam6", 2,
                                           new List<Tuple<int, string>>
                                           {
                                               new Tuple<int, string>(0, "rot"),
                                               new Tuple<int, string>(5, "gelb"),
                                               new Tuple<int, string>(10, "grün"),
                                               new Tuple<int, string>(20, "blau"),
                                           });
            BinParam7 = new BinaryParameter(this, "BinParam7", "BinParam7", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        }

        public IntParameter IntParam3 { get; }
        public StringParameter StrParam4 { get; }
        public BoolParameter BoolParam5 { get; }
        public ChoiceParameter ChParam6 { get; }
        public BinaryParameter BinParam7 { get; }

        public IList<DynContainer> DynContainers => Containers;
    }
}
