using DataTreeBase;
using DataTreeBase.Container;
using DataTreeBase.Parameters;

namespace SampleModel
{
    public sealed class DynContainer: DataTreeDynContainer
    {
        public DynContainer(DataTreeContainer parent)
            : base(parent, "DynCont", "DynContainer")
        {
            IntParam4 = new IntParameter(this, "IntParam4", "IntParam4", 0);
            StrParam5 = new StringParameter(this, "StrParam5", "StrParam5", "def");
            BoolParam6 = new BoolParameter(this, "BoolParam6", "BoolParam6", true);
        }

        public IntParameter IntParam4 { get; }
        public StringParameter StrParam5 { get; }
        public BoolParameter BoolParam6 { get; }
    }
}
