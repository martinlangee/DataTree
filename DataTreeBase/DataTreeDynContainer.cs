namespace DataTreeBase
{
    /// <summary>
    /// Base class for all containers that are elements of a dynamic parent container
    /// </summary>
    public class DataTreeDynContainer: DataTreeContainer
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Identificator</param>
        /// <param name="name">Container name</param>
        protected DataTreeDynContainer(DataTreeContainer parent, string id, string name)
            : base(parent, id, name)
        {
        }

        /// <summary>
        /// Returns the node index used for serialization
        /// </summary>
        public override int Idx => Parent.Containers.IndexOf(this);
    }
}
