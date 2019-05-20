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
using System.Linq;
using DataBase.Container;

namespace DataBase.Parameters
{
    /// <summary>
    /// Represents a parameter with byte array value type
    /// </summary>
    public sealed class BinaryParameter : DataParameter<byte[]>
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="designation"></param>
        /// <param name="defaultValue"></param>
        public BinaryParameter(DataContainer parent, string id, string designation, byte[] defaultValue)
            : base(parent, id, designation, defaultValue)
        {
        }

        /// <summary>
        /// Returns true if the two byte arrays are equal
        /// </summary>
        protected override bool IsEqualValue(byte[] value1, byte[] value2)
        {
            return (value1 != null) && (value2 != null) && value1.SequenceEqual(value2);
        }

        /// <summary>
        /// Returns the base-64 representation of the value
        /// </summary>
        public override string AsString
        {
            get => Value != null ? Convert.ToBase64String(Value) : "";
            set => Value = value.Length > 0 ? Convert.FromBase64String(value): new byte[0];
        }
    }
}
