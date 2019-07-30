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
    public abstract class DataParameterBase : DataNode
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
            return new List<Tuple<string, string>>
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
            {
                parentXmlNode.AppendChildNode(XmlHelper.ParamTag, attr);
            }
            else
            {
                xmlNode.SetAttributes(attr);
            }
        }

        /// <summary>
        /// Loads the parameter value from the id-matching child xml node of the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        public virtual void LoadFromXml(XmlNode parentXmlNode) =>
            AsStringInvCult = parentXmlNode.ChildNodeByTagAndId(XmlHelper.ParamTag, Id)?.AttributeByName(XmlHelper.Attr.Value).Value;

        /// <summary>
        /// Clones the parameter state from the specified parameter
        /// </summary>
        internal virtual void CloneFrom(DataParameterBase param)
        {
            if (param.Id != Id || param.GetType() != GetType())
            {
                throw new InvalidOperationException("DataParameterBase.CloneFrom: parameter ids or types not matching");
            }
            AsString = param.AsString;
        }

        /// <summary>
        /// Gets or sets the string representation of the parameter value using the current culture
        /// </summary>
        public abstract string AsString { get; set; }

        /// <summary>
        /// Gets or sets the string representation of the parameter value using the invariant culture
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
    }
}
