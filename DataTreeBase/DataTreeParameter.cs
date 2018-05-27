using System;
using System.Collections.Generic;

namespace DataTreeBase
{
    /// <summary>
    /// Represents a generic parameter base class
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public abstract class DataTreeParameter<T>: DataTreeParameterBase
    {
        private T _value;
        private T _bufferedValue;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Parameter identificator</param>
        /// <param name="name">Parameter name</param>
        /// <param name="defaultValue">Generic typed parameter default value</param>
        protected DataTreeParameter(DataTreeContainer parent, string id, string name, T defaultValue)
            : base(parent, id, name)
        {
            _value = defaultValue;
            _bufferedValue = defaultValue;
        }

        /// <summary>
        /// Fires the OnChanged event
        /// </summary>
        protected void FireOnChanged()
        {
            OnChanged(this);
        }

        /// <summary>
        /// Generic typed parameter value
        /// </summary>
        public virtual T Value
        {
            get { return _value; }
            set
            {
                if (IsEqualValue(value, _value)) return;

                _value = value;
                FireOnChanged();
            } 
        }

        /// <summary>
        /// Event fired when the parameter value hase changed
        /// Specifies the changed parameter object
        /// </summary>
        public event Action<DataTreeParameterBase> OnChanged = parameter => {};

        /// <summary>
        /// Returns true if the parameter is changed compared to the buffered value
        /// </summary>
        public override bool IsModified => !IsEqualValue(Value, _bufferedValue);

        /// <summary>
        /// Resets the modified flag to false by setting the buffered value to the current value
        /// </summary>
        public override void ResetModified()
        {
            _bufferedValue = Value;
        }

        /// <summary>
        /// Sets the value to the buffered value
        /// </summary>
        public override void Restore()
        {
            Value = _bufferedValue;
        }

        /// <summary>
        /// Returns true if the specified generic typed values are equal
        /// </summary>
        protected virtual bool IsEqualValue(T value1, T value2)
        {
            return Comparer<T>.Default.Compare(value1, value2) == 0;
        }
    }
}
