using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataTreeBase
{
    public class ChoiceParameter : DataTreeParameter<int>
    {
        // todo: ValueIdx-Value-Tupel einführen

        public ChoiceParameter(DataTreeContainer parent, string id, string name, int defaultValue)
            : base(parent, id, name, defaultValue)
        {
        }

        protected override bool IsEqualValue(int value1, int value2)
        {
            return value1 == value2;
        }

        public override string AsString
        {
            get { return Value.ToString(); }
            internal set
            {
                int valIdx;
                if (int.TryParse(value, out valIdx))
                    ValueIdx = valIdx;
            }
        }

        public override void LoadFromXml(XmlNode parentXmlNode)
        {
            int valIdx;
            if (int.TryParse(parentXmlNode.ChildNodeByNameAndId(Tagname, Id)?.AttributeByName(XmlHelper.Attr.ValueIdx).Value, out valIdx))
                ValueIdx = valIdx;
        }

        public int ValueIdx { get; set; }
    }
}
