#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using DataBase.Container;

namespace DataBase.Parameters
{
    /// <summary>
    /// Represents a parameter with double value type
    /// </summary>
    public sealed class FloatParameter : DataParameter<double>
    {
        private string _formatStr;
        private int _decimals;
        private readonly char _currentDecimalSep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
        private readonly char _invariantDecimalSep = CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator[0];

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Parameter identificator</param>
        /// <param name="designation">Parameter name</param>
        /// <param name="defaultValue">Float parameter default value</param>
        /// <param name="unit">Parameter unit</param>
        /// <param name="decimals">Floating point decimals</param>
        public FloatParameter(DataContainer parent, string id, string designation, double defaultValue, string unit, int decimals)
            : base(parent, id, designation, defaultValue)
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
            attr.Add(new Tuple<string, string>(XmlHelper.Attr.Unit, Unit));

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
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        private string ToString(CultureInfo culture)
        {
            string decimalSeperator = culture.NumberFormat.NumberDecimalSeparator;
            string valueStr = Value.ToString(_formatStr, culture);

            return valueStr.Contains(decimalSeperator) ? 
                valueStr.TrimEnd('0').TrimEnd(decimalSeperator[0]) : 
                valueStr;
        }

        /// <summary>
        /// Gets or sets the string representation considering the decimals using invariant culture
        /// </summary>
        public override string AsString
        {
            get => ToString(CultureInfo.CurrentCulture);
            set
            {
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out double dblVal))
                    Value = dblVal;
                else
                    throw new ArgumentException($"'{value}' is not a correct decimal value.");
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
            get => _decimals;
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
        public override string AsStringInvCult
        {
            get => ToString(CultureInfo.InvariantCulture);
            set => AsString = value.Replace(_invariantDecimalSep, _currentDecimalSep);
        }
    }
}