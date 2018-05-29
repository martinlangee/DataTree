using DataTreeBase;

namespace SampleModel
{
    public sealed class Root: DataTreeContainer
    {
        public static Root Create()
        {
            return new Root();
        }

        private Root()
            : base(null, "Root", "Root")
        {
            Cont1 = new Cont1(this);
            Cont2 = new Cont2(this);
        }

        public Cont1 Cont1 { get; }
        public Cont2 Cont2 { get; }
    }
}
