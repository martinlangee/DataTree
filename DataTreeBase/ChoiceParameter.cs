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
        // todo: Value als "Master"-Value behandeln; ValueIdx als nachrangig (wie AsString)
        private int _valueIdx;
        private IList<Tuple<int, string>> _choiceList;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <param name="choiceList"></param>
        public ChoiceParameter(DataTreeContainer parent, string id, string name, int defaultValue, IList<Tuple<int, string>> choiceList)
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

            return attr;
        }

        /// <summary>
        /// Gets or sets the string representation of the value
        /// </summary>
        public override string AsString
        {
            get { return ChoiceList.First(i => i.Item1 == Value).Item2; }
            set
            {
                if (IsChanging) throw new InvalidOperationException("ChoiceParameter.SetAsString: changing value while executing OnChanged is not allowed");

                var newChoice = ChoiceList.FirstOrDefault(i => i.Item2 == value);
                if (newChoice == null) throw new ArgumentOutOfRangeException($"ChoiceParameter.SetAsString: no choice found for string '{value}'");

                Value = newChoice.Item1;
            }
        }

        /// <summary>
        /// Returns the value written to xml file on save
        /// </summary>
        protected override string XmlValue => Value.ToString();

        /// <summary>
        /// Loads the parameter value from the id-matching child xml node of the specified parent xml node using the valueIdx attribute
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        public override void LoadFromXml(XmlNode parentXmlNode)
        {
            int val;
            if (int.TryParse(parentXmlNode.ChildNodeByTagAndId(Helper.ParamTag, Id)?.AttributeByName(Helper.Attr.Value).Value, out val))
                Value = val;
        }

        /// <summary>
        /// Gets or sets the list of choices this parameters defines
        /// </summary>
        public IList<Tuple<int, string>> ChoiceList 
        {
            get { return _choiceList; }
            set
            {
                if ((value == null) || (value.Count < 1)) 
                    throw new ArgumentNullException("ChoiceParameter.SetChoiceList: invalid choice list set");

                _choiceList = value;

                if (_choiceList.All(c => c.Item1 != Value))
                    Value = value[0].Item1;
            }
        }

        /// <summary>
        /// Int typed parameter value
        /// </summary>
        public override int Value
        {
            get { return base.Value; }
            set
            {
                if (ChoiceList.All(c => c.Item1 != value)) throw new ArgumentOutOfRangeException("ChoiceParameter.SetValue: no such item");

                base.Value = value;
            }
        }

        /// <summary>
        /// Index of the selected value from the choice list
        /// </summary>
        public int ValueIdx
        {
            get { return ChoiceList.IndexOf(new Tuple<int, string>(Value, AsString)); }
            set
            {
                if (IsChanging) throw new InvalidOperationException("ChoiceParameter.SetValueIdx: changing value while executing OnChanged is not allowed");

                Value = ChoiceList[value].Item1;
            }
        }
    }
}
