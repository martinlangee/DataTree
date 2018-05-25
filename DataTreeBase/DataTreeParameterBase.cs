using System;
using System.Collections.Generic;
using System.Xml;

namespace DataTreeBase
{
    public abstract class DataTreeParameterBase: DataTreeNode
    {
        protected const string Tagname = "P";

        public DataTreeParameterBase(DataTreeContainer parent, string id, string name)
            : base(parent, id, name)
        {
            Parent?.Params.Add(this);
        }

        protected virtual List<Tuple<string, string>> GetXmlAttributes()
        {
            return
                    new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>(XmlHelper.Attr.Id, Id),
                        new Tuple<string, string>(XmlHelper.Attr.Name, Name),
                        new Tuple<string, string>(XmlHelper.Attr.Value, AsString),
                    };
        }

        public void SaveToXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByNameAndId(Tagname, Id);

            var attr = GetXmlAttributes();

            if (xmlNode == null)
                parentXmlNode.AppendChildNode(Tagname, attr);
            else
                xmlNode.SetAttributes(attr);
        }

        public virtual void LoadFromXml(XmlNode parentXmlNode)
        {
            AsString = parentXmlNode.ChildNodeByNameAndId(Tagname, Id)?.AttributeByName(XmlHelper.Attr.Value).Value;
        }

        public abstract string AsString { get; internal set; }

        public abstract void ResetModified();

        public abstract void Restore();

    }
}
