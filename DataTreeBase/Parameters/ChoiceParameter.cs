using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DataBase.Container;

namespace DataBase.Parameters
{
    /// <summary>
    /// Represents a choice parameter which defines a list of choices (consisting of value/string tuples)
    /// Remark: Tuple used (instead of KeyValuePair) due to performance reasons (ref. https://www.dotnetperls.com/tuple-keyvaluepair).
    /// </summary>
    public sealed class ChoiceParameter : DataParameter<int>
    {
        private List<Tuple<int, string>> _choices;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <param name="choices"></param>
        public ChoiceParameter(DataContainer parent, string id, string name, int defaultValue, List<Tuple<int, string>> choices)
            : base(parent, id, name, defaultValue)
        {
            CheckAndAssignChoices(choices);
        }

        /// <summary>
        /// Checks if both key and value of the choice list are unambiguously
        /// </summary>
        private void CheckAndAssignChoices(List<Tuple<int, string>> choices)
        {
            if ((choices == null) || (choices.Count < 1))
                throw new ArgumentNullException($"{nameof(ChoiceParameter)}: invalid or empty choice list");

            if (choices.GroupBy(ch => ch.Item1).Any(g => g.Count() > 1))
                throw new ArgumentException($"{nameof(ChoiceParameter)}: choice values are not unambiguously");

            // This does not necessarily have to be checked but i prefer to be more strict. 
            // Furthermore it makes the 'AsString' assignment unambiguously.
            if (choices.GroupBy(ch => ch.Item2).Any(g => g.Count() > 1))
                throw new ArgumentException($"{nameof(ChoiceParameter)}: choice descriptions are not unambiguously");

            _choices = choices;
        }

        /// <summary>
        /// Returns the attributes the choice parameter needs to be saved to xml
        /// </summary>
        protected override List<Tuple<string, string>> GetXmlAttributes()
        {
            var attr = base.GetXmlAttributes();
            attr.Add(new Tuple<string, string>(XmlHelper.Attr.ValueStr, AsString));

            return attr;
        }

        /// <summary>
        /// Gets or sets the string representation of the value
        /// </summary>
        public override string AsString
        {
            get { return Choices.First(i => i.Item1 == Value).Item2; }
            set
            {
                if (IsChanging) throw new InvalidOperationException("ChoiceParameter.SetAsString: changing value while executing OnChanged is not allowed");

                var newChoice = Choices.FirstOrDefault(i => i.Item2 == value);
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
            if (int.TryParse(parentXmlNode.ChildNodeByTagAndId(XmlHelper.ParamTag, Id)?.AttributeByName(XmlHelper.Attr.Value).Value, out val))
                Value = val;
        }

        /// <summary>
        /// Gets or sets the list of choices this parameters defines
        /// </summary>
        public IReadOnlyCollection<Tuple<int, string>> Choices 
        {
            get => _choices;
            set
            {
                CheckAndAssignChoices(value.ToList());

                // if old current value is not apparent in the new choices list set value to first item
                if (_choices.All(c => c.Item1 != Value))
                    Value = _choices[0].Item1;
            }
        }

        /// <summary>
        /// Int typed parameter value
        /// </summary>
        public override int Value
        {
            get => base.Value;
            set
            {
                if (_choices.All(c => c.Item1 != value)) throw new ArgumentOutOfRangeException($"{nameof(ChoiceParameter)}.{nameof(Value)}: no such item");

                base.Value = value;
            }
        }

        /// <summary>
        /// Index of the selected value from the choice list
        /// </summary>
        public int ValueIdx
        {
            get => _choices.IndexOf(new Tuple<int, string>(Value, AsString));
            set
            {
                if (IsChanging) throw new InvalidOperationException("ChoiceParameter.SetValueIdx: changing value while executing OnChanged is not allowed");

                Value = _choices[value].Item1;
            }
        }
    }
}
