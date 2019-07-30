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
using DataBase.Container;
using DataBase.Interfaces;
using DataBase.UndoRedo;

namespace DataBase.Parameters
{
    /// <summary>
    /// Represents a generic parameter base class
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public abstract class DataParameter<T>: DataParameterBase, IUndoRedoableNode
    {
        private T _value;
        private T _defaultValue;
        private readonly UndoRedoStack _undoRedo;

        protected bool IsChanging;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Parameter identificator</param>
        /// <param name="designation">Parameter name</param>
        /// <param name="defaultValue">Generic typed parameter default value</param>
        protected DataParameter(DataContainer parent, string id, string designation, T defaultValue)
            : base(parent, id, designation)
        {
            _defaultValue = defaultValue;
            BufferedValue = _defaultValue;
            _value = _defaultValue;
            _undoRedo = Parent?.Root?.UndoRedo;
        }

        /// <summary>
        /// Generic typed parameter value
        /// </summary>
        public virtual T Value
        {
            get => _value;
            set
            {
                if (IsEqualValue(value, _value))
                {
                    return;
                }
                if (IsChanging)
                {
                    throw new InvalidOperationException("DataParameter.SetValue: changing value while executing OnChanged is not allowed");
                }

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
        /// Returns the buffered value set on creation or ResetModified
        /// </summary>
        public T BufferedValue { get; private set; }

        /// <summary>
        /// Returns true if the parameter is changed compared to the buffered value
        /// </summary>
        public override bool IsModified => !IsEqualValue(Value, BufferedValue);

        /// <summary>
        /// Value, BufferedValue and DefaultValue are set to the passed value
        /// </summary>
        /// <param name="value"></param>
        public void Init(T value)
        {
            _value = value;
            BufferedValue = value;
            _defaultValue = value;
        }

        /// <summary>
        /// Event fired when the parameter value hase changed
        /// Specifies the changed parameter object
        /// </summary>
        public event Action<DataParameterBase> OnChanged = parameter => {};

        /// <summary>
        /// Resets the modified flag to false by setting the buffered value to the current value
        /// </summary>
        public override void ResetModifiedState()
        {
            BufferedValue = Value;
            FireOnChanged();   // fired since 'IsModified' has possibly changed
        }

        /// <summary>
        /// Sets the value to the buffered value
        /// </summary>
        public override void Restore() => Value = BufferedValue;

        /// <summary>
        /// Sets the value to the default value
        /// </summary>
        public override void SetToDefault() => Value = BufferedValue = _defaultValue;

        /// <summary>
        /// Set the new value as result of the undo or redo process
        /// </summary>
        public void Set(object value) => AsString = (string)value;

        /// <summary>
        /// Clones the parameter state from the specified parameter
        /// </summary>
        internal override void CloneFrom(DataParameterBase param)
        {
            base.CloneFrom(param);
            BufferedValue = ((DataParameter<T>)param).BufferedValue;
        }

        /// <summary>
        /// Action fires on change of the parameter value used by the undo/redo controller
        /// </summary>
        internal Action<DataParameterBase, object, object> OnChangedForUndoRedo { get; set; }

        /// <summary>
        /// Fires the OnChanged event
        /// </summary>
        protected void FireOnChanged() => OnChanged(this);

        /// <summary>
        /// Value is assigned from specified value and returned. May be overridden to manipulated value befor assigning it.
        /// </summary>
        protected virtual void AssignValueAndNotify(T value)
        {
            string oldValue = AsString;
            _value = value;
            _undoRedo?.ValueChanged(this, oldValue, AsString);
            FireOnChanged();
        }

        /// <summary>
        /// Returns true if the specified generic typed values are equal
        /// </summary>
        protected virtual bool IsEqualValue(T value1, T value2) => Comparer<T>.Default.Compare(value1, value2) == 0;
    }
}
