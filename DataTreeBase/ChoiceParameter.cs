using System;
using System.Collections.Generic;
using System.Xml;

namespace DataTreeBase
{
    public sealed class ChoiceParameter : DataTreeParameter<int>
    {
        private int _valueIdx;

        public ChoiceParameter(DataTreeContainer parent, string id, string name, int defaultValue, List<Tuple<int, string>> choiceList)
            : base(parent, id, name, defaultValue)
        {
            ChoiceList = choiceList;
        }

        protected override List<Tuple<string, string>> GetXmlAttributes()
        {
            var attr = base.GetXmlAttributes();
            attr.RemoveAt(2);    // "Value"
            attr.Add(new Tuple<string, string>(Helper.Attr.ValueIdx, ValueIdx.ToString()));
            attr.Add(new Tuple<string, string>(Helper.Attr.ValueStr, ChoiceList[ValueIdx].Item2));

            return attr;
        }

        public override string AsString
        {
            get { return ChoiceList[ValueIdx].Item2; }
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
            if (int.TryParse(parentXmlNode.ChildNodeByNameAndId(Tagname, Id)?.AttributeByName(Helper.Attr.ValueIdx).Value, out valIdx))
                ValueIdx = valIdx;
        }

        public List<Tuple<int, string>> ChoiceList { get; set; }

        public override int Value => ChoiceList[ValueIdx].Item1;

        public int ValueIdx
        {
            get { return _valueIdx; }
            set
            {
                if (_valueIdx == value) return;

                _valueIdx = value;
                FireOnChanged();
            }
        }
    }
}
