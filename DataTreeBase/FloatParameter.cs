using System;
using System.Collections.Generic;
using System.Globalization;

namespace DataTreeBase
{
    /// <summary>
    /// Represents a parameter with double value type
    /// </summary>
    public sealed class FloatParameter : DataTreeParameter<double>
    {
        private string _formatStr;
        private int _decimals;
        private readonly char _currentDecimalSep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Parameter identificator</param>
        /// <param name="name">Parameter name</param>
        /// <param name="defaultValue">Float parameter default value</param>
        /// <param name="unit">Parameter unit</param>
        /// <param name="decimals">Floating point decimals</param>
        public FloatParameter(DataTreeContainer parent, string id, string name, double defaultValue, string unit, int decimals)
            : base(parent, id, name, defaultValue)
        {
            Unit = unit;
            Decimals = decimals;
        }

        /// <summary>
        /// Returns true if the two float values are equal comparing only the number of digits specified in the decimals
        /// </summary>
        protected override bool IsEqualValue(double value1, double value2)
        {
            return Math.Round(value1, Decimals).CompareTo(Math.Round(value2, Decimals)) == 0;
        }

        /// <summary>
        /// Returns the attributes the float parameter needs to be saved to xml
        /// </summary>
        protected override List<Tuple<string, string>> GetXmlAttributes()
        {
            var attr = base.GetXmlAttributes();
            attr.Add(new Tuple<string, string>(Helper.Attr.Unit, Unit));

            return attr;
        }

        /// <summary>
        /// Value is assigned from specified value. May be overridden to manipulated value befor assigning it.
        /// </summary>
        protected override void AssignValueAndNotify(double value)
        {
            base.AssignValueAndNotify(Math.Round(value, Decimals));
        }

        /// <summary>
        /// Gets or sets the string representation considering the decimals 
        /// </summary>
        public override string AsString
        {
            get { return Value.ToString(_formatStr, CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.'); }
            set
            {
                double dblVal;
                if (double.TryParse(value, out dblVal))
                    Value = dblVal;
                else
                    throw new ArgumentException($"FloatParameter.SetAsString: cannot convert '{value}' to float.");
            }
        }

        /// <summary>
        /// Returns the parameter unit
        /// </summary>
        public string Unit { get; }

        /// <summary>
        /// Gets or sets the number of decimals
        /// </summary>
        public int Decimals
        {
            get { return _decimals; }
            set
            {
                if (_decimals == value) return;

                _decimals = value;
                _formatStr = "0." + new string('0', _decimals);

                if (Math.Round(Value, Decimals).CompareTo(Value) != 0)
                    AssignValueAndNotify(Value);
            }
        }

        /// <summary>
        /// Gets or sets the value from string representation containing the current culture's decimal seperator
        /// </summary>
        public string AsStringC
        {
            get { return Value.ToString(_formatStr, CultureInfo.CurrentCulture).TrimEnd('0').TrimEnd(_currentDecimalSep); }
            set { AsString = value.Replace(_currentDecimalSep, '.'); }
        }
    }
}