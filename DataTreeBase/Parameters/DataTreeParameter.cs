using System;
using System.Collections.Generic;

using DataTreeBase.Container;
using DataTreeBase.Interfaces;
using DataTreeBase.UndoRedo;

namespace DataTreeBase.Parameters
{
    /// <summary>
    /// Represents a generic parameter base class
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public abstract class DataTreeParameter<T>: DataTreeParameterBase, IUndoRedoNode
    {
        private T _value;
        private readonly UndoRedoStack _undoRedo;

        protected bool IsChanging;

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
            BufferedValue = defaultValue;
            _value = defaultValue;
            _undoRedo = Parent?.Root?.UndoRedo;
        }

        /// <summary>
        /// Fires the OnChanged event
        /// </summary>
        protected void FireOnChanged()
        {
            OnChanged(this);
        }

        /// <summary>
        /// Value is assigned from specified value and returned. May be overridden to manipulated value befor assigning it.
        /// </summary>
        protected virtual void AssignValueAndNotify(T value)
        {
            var oldValue = _value;
            _value = value;
            _undoRedo?.ValueChanged(this, oldValue, _value);
            FireOnChanged();
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
                if (IsChanging) throw new InvalidOperationException("DataTreeParameter.SetValue: changing value while executing OnChanged is not allowed");

                IsChanging = true;
                try
                {
                    AssignValueAndNotify(value);
                }
                finally
                {
                    IsChanging = false;
                }
            } 
        }

        /// <summary>
        /// Clones the parameter state from the specified parameter
        /// </summary>
        internal override void CloneFrom(DataTreeParameterBase param)
        {
            base.CloneFrom(param);
            BufferedValue = ((DataTreeParameter<T>) param).BufferedValue;
        }

        /// <summary>
        /// Returns the buffered value set on creation or ResetModified
        /// </summary>
        public T BufferedValue { get; private set; }

        /// <summary>
        /// Event fired when the parameter value hase changed
        /// Specifies the changed parameter object
        /// </summary>
        public event Action<DataTreeParameterBase> OnChanged = parameter => {};

        /// <summary>
        /// Returns true if the parameter is changed compared to the buffered value
        /// </summary>
        public override bool IsModified => !IsEqualValue(Value, BufferedValue);

        /// <summary>
        /// Resets the modified flag to false by setting the buffered value to the current value
        /// </summary>
        public override void ResetModified()
        {
            BufferedValue = Value;
            FireOnChanged();   // fired since 'IsModified' has possibly changed
        }

        /// <summary>
        /// Sets the value to the buffered value
        /// </summary>
        public override void Restore()
        {
            Value = BufferedValue;
        }

        /// <summary>
        /// Returns true if the specified generic typed values are equal
        /// </summary>
        protected virtual bool IsEqualValue(T value1, T value2)
        {
            return Comparer<T>.Default.Compare(value1, value2) == 0;
        }

        /// <summary>
        /// Action fires on change of the parameter value used by the undo/redo controller
        /// </summary>
        internal Action<DataTreeParameterBase, object, object> OnChangedForUndoRedo { get; set; }

        /// <summary>
        /// Set the new value as result of the undo or redo process
        /// </summary>
        public void Set(object value)
        {
            Value = (T) value;
        }
    }
}
