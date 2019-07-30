using System;
using System.Xml;

namespace DataBase.Container
{
    /// <summary>
    /// Base class for all containers that are elements of a dynamic parent container
    /// </summary>
    public class DataDynContainer: DataContainer
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Identificator</param>
        /// <param name="designation">Container name</param>
        protected DataDynContainer(DataContainer parent, string id, string designation)
            : base(parent, id, designation) { }

        public override string PathId
        {
            get
            {
                var idx = Parent?.Children.IndexOf(this);
                return (Parent != null ? Parent.PathId + $"{XmlHelper.PathDelimiter}" : "") + $"{Id}[{idx}]";
            }
        }

        /// <summary>
        /// Loads the container and it's sub-containers from the specified own xml node
        /// </summary>
        /// <param name="xmlNode">Own xml node</param>
        public override void LoadFromXml(XmlNode xmlNode)
        {
            Params.ForEach(p => p.LoadFromXml(xmlNode));
            Children.ForEach(c => c.LoadFromXml(xmlNode));
        }

        /// <summary>
        /// Returns the xml node as child of the parent node that shows the id of this node.
        /// If not found new node is created and returned.
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        protected override XmlNode GetOwnXmlNode(XmlNode parentXmlNode) => parentXmlNode.AppendChildNode(XmlHelper.ContainerTag);
    }
}
