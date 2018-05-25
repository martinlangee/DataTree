using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace DataTreeBase
{
    public class DataTreeContainer: DataTreeNode
    {
        private const string Tagname = "C";

        protected DataTreeContainer(DataTreeContainer parent, string id, string name) 
            : base(parent, id, name)
        {
            Containers = new List<DataTreeContainer>();
            Params = new List<DataTreeParameterBase>();

            Parent?.Containers.Add(this);
        }

        public IList<DataTreeContainer> Containers { get; }

        public IList<DataTreeParameterBase> Params { get; }

        public override bool IsModified =>
                Containers.Any(c => c.IsModified) ||
                Params.Any(p => p.IsModified);

        public XmlDocument SaveToXml(XmlNode parentXmlNode)
        {
            XmlDocument doc = null;
            XmlNode xmlNode;

            if (parentXmlNode == null)
            {
                doc = new XmlDocument();
                doc.LoadXml($"<{Tagname}/>");
                xmlNode = doc.DocumentElement;
            }
            else
            {
                xmlNode = parentXmlNode.ChildNodeByNameAndId(Tagname, Id) ??
                          parentXmlNode.AppendChildNode(Tagname);
            }

            xmlNode.SetAttributes(new List<Tuple<string, string>>
                                  {
                                      new Tuple<string, string>(XmlHelper.Attr.Id, Id),
                                      new Tuple<string, string>(XmlHelper.Attr.Name, Name),
                                  });

            Params.ForEach(p => p.SaveToXml(xmlNode));
            Containers.ForEach(c => c.SaveToXml(xmlNode));

            return doc;
        }

        public void LoadFromXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByNameAndId(Tagname, Id);
            if (xmlNode == null) return;
            
            Params.ForEach(p => p.LoadFromXml(xmlNode));
            Containers.ForEach(c => c.LoadFromXml(xmlNode));
        }

        public void LoadFromFile(string fileName)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);
            LoadFromXml(doc.DocumentElement);
            ResetModified();
        }

        public void SaveToFile(string fileName)
        {
            SaveToXml(null).Save(fileName);
            ResetModified();
        }

        public void ResetModified()
        {
            Params.ForEach(p => p.ResetModified());
            Containers.ForEach(c => c.ResetModified());
        }

        public void Restore()
        {
            Params.ForEach(p => p.Restore());
            Containers.ForEach(c => c.Restore());
        }
    }
}
