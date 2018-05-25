namespace DataTreeBase
{
    public abstract class DataTreeNode
    {
        protected readonly DataTreeContainer Parent;

        public DataTreeNode(DataTreeContainer parent, string id, string name)
        {
            Parent = parent;
            Id = id;
            Name = name;
        }

        public string Id { get; }

        public string PathId => Parent.Id + @"\" + Id;

        public string Name { get; }

        public abstract bool IsModified { get; }
    }
}
