using System.Diagnostics;

using DataTreeBase;

namespace SampleModel
{
    [DebuggerStepThrough]
    public sealed class Cont1: DataTreeContainer
    {
        public Cont1(DataTreeContainer parent)
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
