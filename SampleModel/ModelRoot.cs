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
            Group1 = new Group1(this);
            Group2 = new Group2(this);
        }

        public Group1 Group1 { get; }
        public Group2 Group2 { get; }
    }
}
