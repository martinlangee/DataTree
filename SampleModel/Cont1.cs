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

using System.Diagnostics;

using DataBase.Container;
using DataBase.Parameters;

namespace SampleModel
{
    [DebuggerStepThrough]
    public sealed class Cont1: DataContainer
    {
        public Cont1(DataContainer parent)
            : base(parent, "Cont1", "Cont1")
        {
            IntParam = new IntParameter(this, "IntParam", "IntParam", 0);
            StrParam = new StringParameter(this, "StrParam", "StrParam", "abc");
            FloatParam1 = new FloatParameter(this, "FloatParam1", "FloatParam1", 0.5f, "m/s", 5);
            FloatParam2 = new FloatParameter(this, "FP2", "FloatParam2", 5.5f, "m/s", 8);
        }

        public IntParameter IntParam { get; }
        public StringParameter StrParam { get; }
        public FloatParameter FloatParam1 { get; }
        public FloatParameter FloatParam2 { get; }
    }
}
