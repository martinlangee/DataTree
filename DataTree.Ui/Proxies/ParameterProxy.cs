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

using DataBase.Parameters;
using System;
using System.Diagnostics;

namespace DataTree.Ui.Proxies
{
    /// <summary>
    /// Implements a common proxy class used to bind a <see cref="DataParameterBase"/> to a UI element.
    /// Bidirectional change notification is ensured.
    /// </summary>
    /// <typeparam name="T">The value type of the attached parameter</typeparam>
    [DebuggerDisplay("{Param.PathId}, {Value}")]
    public class ParameterProxy<T> : NodeViewModel, IDisposable
    {
        protected DataParameter<T> Param;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="param">The <see cref="DataParameterBase"/> this proxy is connected to</param>
        public ParameterProxy(DataParameter<T> param): base()
        {
            Param = param;
            Param.OnChanged += OnSourceChanged;

            Designation = param.Designation;

            FirePropertyChanged();
        }

        /// <summary>
        /// The value of the attached parameter
        /// </summary>
        public override string Value
        {
            get => Param.AsText;
            set
            {
                if (value.Equals(Param.AsText)) return;

                Param.AsText = value;
                FirePropertiesChanged(nameof(IsModified));
            }
        }

        protected virtual void OnSourceChanged(DataParameterBase param)
        {
            FirePropertiesChanged(nameof(Value), nameof(IsModified));
        }

        /// <summary>
        /// Gets the Modified status of the attached parameter
        /// </summary>
        public override bool IsModified => Param.IsModified;

        /// <summary>
        /// Returns the type of the node
        /// </summary>
        public override NodeType NodeType => NodeType.Parameter;

        public override string ValueType => Param.ValueType.Name;

        /// <summary>
        /// Disconnection all events
        /// </summary>
        public void Dispose()
        {
            Param.OnChanged -= OnSourceChanged;
            Param = null;
        }
    }
}
