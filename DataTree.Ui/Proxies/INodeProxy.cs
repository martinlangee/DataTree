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
using System.Collections.ObjectModel;

namespace DataTree.Ui.Proxies
{
    public enum NodeType
    {
        Parameter,
        Container
    }

    public interface INodeProxy
    {
        /// <summary>
        /// Gets the parameter designation
        /// </summary>
        string Designation { get; }

        /// <summary>
        /// The value of the attached parameter
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Gets the Modified status of the attached parameter
        /// </summary>
        bool IsModified { get; }

        /// <summary>
        /// Returns the list of child nodes
        /// </summary>
        ObservableCollection<INodeProxy> Nodes { get; }

        /// <summary>
        /// Returns the type of the node
        /// </summary>
        NodeType NodeType { get; }

        string ValueType { get; }

    }
}