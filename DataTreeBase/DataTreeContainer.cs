using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataTreeBase
{
    public class DataTreeContainer: DataTreeNode
    {
        const string Tagname = "C";

        protected DataTreeContainer(DataTreeContainer parent, string id, string name) 
            : base(parent, id, name)
        {
            Containers = new List<DataTreeContainer>();
            Params = new List<DataTreeParameterBase>();

            _parent?.Containers.Add(this);
        }

        public IList<DataTreeContainer> Containers { get; }

        public IList<DataTreeParameterBase> Params { get; }

        public override bool IsModified =>
                Containers.Any(c => c.IsModified) ||
                Params.Any(p => p.IsModified);

        public override void SaveToXml(XmlDocument xmlDoc, XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByNameAndId(Tagname, Id) ??
                          parentXmlNode.AppendChildNode(xmlDoc, Tagname);

            xmlNode.SetAttributes(xmlDoc,
                                  new List<Tuple<string, string>>
                                  {
                                      new Tuple<string, string>(XmlHelper.Attr.Id, Id),
                                      new Tuple<string, string>(XmlHelper.Attr.Name, Name),
                                  });

            Params.ForEach(p => p.SaveToXml(xmlDoc, xmlNode));
            Containers.ForEach(c => c.SaveToXml(xmlDoc, xmlNode));
        }

        public override void LoadFromXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByNameAndId(Tagname, Id);
            if (xmlNode == null) return;
            
            Params.ForEach(p => p.LoadFromXml(xmlNode));
            Containers.ForEach(c => c.LoadFromXml(xmlNode));
        }
    }
}
