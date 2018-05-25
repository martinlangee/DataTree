using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataTreeBase
{
    public abstract class DataTreeParameterBase: DataTreeNode
    {
        private const string Tagname = "P";

        public DataTreeParameterBase(DataTreeContainer parent, string id, string name, string unit = "")
            : base(parent, id, name)
        {
            Unit = unit;

            _parent?.Params.Add(this);
        }

        public override void SaveToXml(XmlDocument xmlDoc, XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByNameAndId(Tagname, Id);

            var attr =
                    new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>(XmlHelper.Attr.Id, Id),
                        new Tuple<string, string>(XmlHelper.Attr.Name, Name),
                        new Tuple<string, string>(XmlHelper.Attr.Value, AsString),
                        new Tuple<string, string>(XmlHelper.Attr.Unit, Unit),
                    };

            if (xmlNode == null)
                parentXmlNode.AppendChildNode(xmlDoc, Tagname, attr);
            else
                xmlNode.SetAttributes(xmlDoc, attr);
        }

        public override void LoadFromXml(XmlNode parentXmlNode)
        {
            AsString = parentXmlNode.ChildNodeByNameAndId(Tagname, Id)?.AttributeByName(XmlHelper.Attr.Value).Value;
        }

        public abstract string AsString { get; internal set; }

        public string Unit { get; }
    }
}
