using System;
using DataBase.Container;

namespace DataBase.Parameters
{
    /// <summary>
    /// Represents a parameter with bool value type
    /// </summary>
    public sealed class BoolParameter : DataParameter<bool>
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public BoolParameter(DataContainer parent, string id, string name, bool defaultValue = false)
            : base(parent, id, name, defaultValue)
        {
        }

        /// <summary>
        /// Returns "true" if the value is true and "false" if the value is false.
        /// Or sets the value accordingly.
        /// </summary>
        public override string AsString
        {
            get => Value.ToString();
            set
            {
                if (bool.TryParse(value, out var boolVal))
                    Value = boolVal;
                else
                    throw new ArgumentException($"BoolParameter.SetAsString: cannot convert '{value}' to bool.");
            }
        }
    }
}
