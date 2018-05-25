using System;
using System.Collections.Generic;
using System.Globalization;

namespace DataTreeBase
{
    public class FloatParameter : DataTreeParameter<float>
    {
        private readonly string _formatStr;

        public FloatParameter(DataTreeContainer parent, string id, string name, float defaultValue, string unit, int precision)
            : base(parent, id, name, defaultValue)
        {
            Unit = unit;
            _formatStr = "0." + new string('0', precision);
        }

        protected override bool IsEqualValue(float value1, float value2)
        {
            return value1.ToString(_formatStr) == value2.ToString(_formatStr);
        }

        protected override List<Tuple<string, string>> GetXmlAttributes()
        {
            var attr = base.GetXmlAttributes();
            attr.Add(new Tuple<string, string>(XmlHelper.Attr.Unit, Unit));

            return attr;
        }

        public override string AsString
        {
            get { return Value.ToString(_formatStr, CultureInfo.InvariantCulture); }
            internal set
            {
                float floatVal;
                if (float.TryParse(value, out floatVal))
                    Value = floatVal;
            }
        }

        public string Unit { get; }
    }
}
