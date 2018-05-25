using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataTreeBase
{
    public abstract class DataTreeNode
    {
        protected readonly DataTreeContainer _parent;

        public DataTreeNode(DataTreeContainer parent, string id, string name)
        {
            _parent = parent;
            Id = id;
            Name = name;
        }

        public string Id { get; }

        public string Name { get; }

        public abstract bool IsModified { get; }

        public abstract void SaveToXml(XmlDocument xmlDoc, XmlNode parentXmlNode);

        public abstract void LoadFromXml(XmlNode parentXmlNode);
    }
}
