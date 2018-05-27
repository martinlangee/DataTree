using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace DataTreeBase
{
    /// <summary>
    /// Represents a choice parameter which defines a list of choices (consisting of value/string tuples)
    /// </summary>
    public sealed class ChoiceParameter : DataTreeParameter<int>
    {
        private int _valueIdx;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <param name="choiceList"></param>
        public ChoiceParameter(DataTreeContainer parent, string id, string name, int defaultValue, List<Tuple<int, string>> choiceList)
            : base(parent, id, name, defaultValue)
        {
            ChoiceList = choiceList;
            CheckChoiceList();
        }

        /// <summary>
        /// Checks if both key and value of the choice list are unique
        /// </summary>
        private void CheckChoiceList()
        {
            if (ChoiceList.GroupBy(ch => ch.Item1).Any(g => g.Count() > 1))
                throw new InvalidDataException("ChoiceParameter: choice values are not unique");

            if (ChoiceList.GroupBy(ch => ch.Item2).Any(g => g.Count() > 1))
                throw new InvalidDataException("ChoiceParameter: choice descriptions are not unique");
        }

        /// <summary>
        /// Returns the attributes the choice parameter needs to be saved to xml
        /// </summary>
        protected override List<Tuple<string, string>> GetXmlAttributes()
        {
            var attr = base.GetXmlAttributes();
            attr.Add(new Tuple<string, string>(Helper.Attr.ValueStr, AsString));
            attr.Add(new Tuple<string, string>(Helper.Attr.ValueIdx, ValueIdx.ToString()));

            return attr;
        }

        /// <summary>
        /// Gets or sets the string representation of the value
        /// </summary>
        public override string AsString
        {
            get { return ChoiceList[ValueIdx].Item2; }
            set
            {
                for (var idx = 0; idx < ChoiceList.Count; idx++)
                {
                    if (ChoiceList[idx].Item2 == value)
                    {
                        ValueIdx = idx;
                        return;
                    }
                }
                throw new ArgumentOutOfRangeException("ChoiceParameter.AsString: no choice for choice descriptions found");
            }
        }

        /// <summary>
        /// Returns the value written to xml file on save
        /// </summary>
        public override string XmlValue => Value.ToString();

        /// <summary>
        /// Loads the parameter value from the id-matching child xml node of the specified parent xml node using the valueIdx attribute
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        public override void LoadFromXml(XmlNode parentXmlNode)
        {
            int valIdx;
            if (int.TryParse(parentXmlNode.ChildNodeByNameAndId(Tagname, Id)?.AttributeByName(Helper.Attr.ValueIdx).Value, out valIdx))
                ValueIdx = valIdx;
        }

        /// <summary>
        /// Gets or sets the list of choices this parameters defines
        /// todo: check if current ValueIdx is still valid
        /// </summary>
        public List<Tuple<int, string>> ChoiceList { get; set; }

        /// <summary>
        /// Int typed parameter value
        /// </summary>
        public override int Value
        {
            get { return ChoiceList[ValueIdx].Item1; }
            set
            {
                for (var idx = 0; idx < ChoiceList.Count; idx++)
                {
                    if (ChoiceList[idx].Item1 == value)
                    {
                        ValueIdx = idx;
                        return;
                    }
                }
                throw new ArgumentOutOfRangeException("ChoiceParameter.Value: no choice for choice value found");
            }
        }

        /// <summary>
        /// Main value property
        /// </summary>
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
