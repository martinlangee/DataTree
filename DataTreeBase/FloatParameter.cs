using System;
using System.Collections.Generic;
using System.Globalization;

namespace DataTreeBase
{
    /// <summary>
    /// Represents a parameter with float value type
    /// </summary>
    public sealed class FloatParameter : DataTreeParameter<float>
    {
        private readonly string _formatStr;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Parameter identificator</param>
        /// <param name="name">Parameter name</param>
        /// <param name="defaultValue">Float parameter default value</param>
        /// <param name="unit">Parameter unit</param>
        /// <param name="precision">Floating point precision</param>
        public FloatParameter(DataTreeContainer parent, string id, string name, float defaultValue, string unit, int precision)
            : base(parent, id, name, defaultValue)
        {
            Unit = unit;
            _formatStr = "0." + new string('0', precision);
        }

        /// <summary>
        /// Returns true if the two float values are equal comparing only the number of digits specified in the precision
        /// </summary>
        protected override bool IsEqualValue(float value1, float value2)
        {
            return value1.ToString(_formatStr).Equals(value2.ToString(_formatStr));
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
        /// Gets or sets the string representation considering the precision 
        /// </summary>
        public override string AsString
        {
            get { return Value.ToString(_formatStr, CultureInfo.InvariantCulture).TrimEnd('0'); }
            set
            {
                float floatVal;
                if (float.TryParse(value, out floatVal))
                    Value = floatVal;
                else
                    throw new ArgumentException($"FloatParameter.SetAsString: cannot convert '{value}' to float.");
            }
        }

        /// <summary>
        /// Returns the parameter unit
        /// </summary>
        public string Unit { get; }
    }
}
