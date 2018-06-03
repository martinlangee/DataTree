using System;
using System.Collections.Generic;
using System.Xml;

namespace DataTreeBase
{
    /// <summary>
    /// Represents an abstract base parameter class
    /// </summary>
    public abstract class DataTreeParameterBase: DataTreeNode
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Parameter identificator</param>
        /// <param name="name">Parameter name</param>
        protected DataTreeParameterBase(DataTreeContainer parent, string id, string name)
            : base(parent, id, name)
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
                        new Tuple<string, string>(Helper.Attr.Id, Id),
                        new Tuple<string, string>(Helper.Attr.Name, Name),
                        new Tuple<string, string>(Helper.Attr.Value, XmlValue),
                    };
        }

        /// <summary>
        /// Creates new xml node, saves the parameter attributes to the new xml node and adds it to the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        public void SaveToXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByTagAndId(Helper.ParamTag, Id);

            var attr = GetXmlAttributes();

            if (xmlNode == null)
                parentXmlNode.AppendChildNode(Helper.ParamTag, attr);
            else
                xmlNode.SetAttributes(attr);
        }

        /// <summary>
        /// Loads the parameter value from the id-matching child xml node of the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        public virtual void LoadFromXml(XmlNode parentXmlNode)
        {
            AsString = parentXmlNode.ChildNodeByTagAndId(Helper.ParamTag, Id)?.AttributeByName(Helper.Attr.Value).Value;
        }

        /// <summary>
        /// Clones the parameter state from the specified parameter
        /// </summary>
        internal virtual void CloneFrom(DataTreeParameterBase param)
        {
            if (param.Id != Id || param.GetType() != GetType())
                throw new InvalidOperationException("DataTreeParameterBase.CloneFrom: parameter ids or types not matching");

            AsString = param.AsString;
        }

        /// <summary>
        /// Gets or sets the string representation of the parameter value
        /// </summary>
        public abstract string AsString { get; set; }

        /// <summary>
        /// Returns the value written to xml file on save
        /// </summary>
        protected virtual string XmlValue => AsString;

        /// <summary>
        /// Resets the modified flag to false by setting the buffered value to the current value
        /// </summary>
        public abstract void ResetModified();

        /// <summary>
        /// Sets the value to the buffered value
        /// </summary>
        public abstract void Restore();
    }
}
