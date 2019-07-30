#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;

using DataBase.Container;
using DataBase.Parameters;

namespace SampleModel
{
    [DebuggerStepThrough]
    public sealed class Cont2 : DataDynParentContainer<DynContainer>
    {
        public Cont2(DataContainer parent)
            : base(parent, "Cont2", "Cont2")
        {
            IntParam3 = new IntParameter(this, "IntParam3", "IntParam3", 0);
            StrParam4 = new StringParameter(this, "StrParam4", "StrParam4", "def");
            BoolParam5 = new BoolParameter(this, "BoolParam5", "BoolParam5", true);
            ChParam6 = new ChoiceParameter(this, "ChParam6", "ChParam6", 5,
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

        public IReadOnlyList<DynContainer> DynContainers => Items;
    }
}
