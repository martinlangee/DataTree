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
using System.Diagnostics;
using System.Xml;
using DataBase.Container;

namespace DataBase.Parameters
{
    /// <summary>
    /// Represents an abstract base parameter class
    /// </summary>
    [DebuggerDisplay("{GetType().Name}, {PathId}, {AsString}, {IsModified}")]
    public abstract class DataParameterBase: DataNode
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Parameter identificator</param>
        /// <param name="designation">Parameter name</param>
        protected DataParameterBase(DataContainer parent, string id, string designation)
            : base(parent, id, designation)
        {
            Parent?.Params.Add(this);
        }

        /// <summary>
        /// Returns the attributes needed to be saved to xml
        /// </summary>
        protected virtual List<Tuple<string, string>> GetXmlAttributes()
        {
            return
                    new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>(XmlHelper.Attr.Id, Id),
                        new Tuple<string, string>(XmlHelper.Attr.Name, Designation),
                        new Tuple<string, string>(XmlHelper.Attr.Value, XmlValue),
                    };
        }

        /// <summary>
        /// Creates new xml node, saves the parameter attributes to the new xml node and adds it to the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        public void SaveToXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByTagAndId(XmlHelper.ParamTag, Id);

            var attr = GetXmlAttributes();

            if (xmlNode == null)
                parentXmlNode.AppendChildNode(XmlHelper.ParamTag, attr);
            else
                xmlNode.SetAttributes(attr);
        }

        /// <summary>
        /// Loads the parameter value from the id-matching child xml node of the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        public virtual void LoadFromXml(XmlNode parentXmlNode)
        {
            AsStringInvCult = parentXmlNode.ChildNodeByTagAndId(XmlHelper.ParamTag, Id)?.AttributeByName(XmlHelper.Attr.Value).Value;
        }

        /// <summary>
        /// Clones the parameter state from the specified parameter
        /// </summary>
        internal virtual void CloneFrom(DataParameterBase param)
        {
            if (param.Id != Id || param.GetType() != GetType())
                throw new InvalidOperationException("DataParameterBase.CloneFrom: parameter ids or types not matching");

            AsString = param.AsString;
        }

        /// <summary>
        /// Gets or sets the string representation of the parameter value using the current culture
        /// </summary>
        public abstract string AsString { get; set; }

        /// <summary>
        /// Returns the textual representation of the parameter value using the current culture (normally same as <see cref="AsString"/>)
        /// </summary>
        public virtual string AsText
        {
            get => AsString;
            set => AsString = value;
        }

        /// <summary>
        /// Gets or sets the string representation of the parameter value using the invariant culture (used for serialization)
        /// </summary>
        public virtual string AsStringInvCult
        {
            get => AsString;
            set => AsString = value;
        }

        /// <summary>
        /// Returns the value written to xml file on save
        /// </summary>
        protected virtual string XmlValue => AsStringInvCult;

        /// <summary>
        /// Returns the value type of the parameter
        /// </summary>
        public abstract Type ValueType { get; }
    }
}
