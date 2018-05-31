using System;

namespace DataTreeBase
{
    /// <summary>
    /// Represents a parameter with int value type
    /// </summary>
    public sealed class IntParameter : DataTreeParameter<int>
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public IntParameter(DataTreeContainer parent, string id, string name, int defaultValue)
            : base(parent, id, name, defaultValue)
        {
        }

        /// <summary>
        /// Gets or sets the string representation of the value
        /// </summary>
        public override string AsString
        {
            get { return Value.ToString(); }
            set
            {
                int intVal;
                if (int.TryParse(value, out intVal))
                    Value = intVal;
                else
                    throw new ArgumentException($"IntParameter.SetAsString: cannot convert '{value}' to int.");
            }
        }
    }
}
