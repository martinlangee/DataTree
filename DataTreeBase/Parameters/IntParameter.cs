using System;
using DataBase.Container;

namespace DataBase.Parameters
{
    /// <summary>
    /// Represents a parameter with int value type
    /// </summary>
    public sealed class IntParameter : DataParameter<int>
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="designation"></param>
        /// <param name="defaultValue"></param>
        public IntParameter(DataContainer parent, string id, string designation, int defaultValue)
            : base(parent, id, designation, defaultValue)
        {
        }

        /// <summary>
        /// Gets or sets the string representation of the value
        /// </summary>
        public override string AsString
        {
            get => Value.ToString();
            set
            {
                if (int.TryParse(value, out var intVal))
                    Value = intVal;
                else
                    throw new ArgumentException($"IntParameter.SetAsString: cannot convert '{value}' to int.");
            }
        }
    }
}
