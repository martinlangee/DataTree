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
using System.Xml;

namespace DataBase.Container
{
    /// <summary>
    /// Base class for all containers that are elements of a dynamic parent container
    /// </summary>
    public class DataDynContainer: DataContainer
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Identificator</param>
        /// <param name="designation">Container name</param>
        protected DataDynContainer(DataContainer parent, string id, string designation)
            : base(parent, id, designation) { }

        public override string PathId
        {
            get
            {
                var idx = Parent?.Children.IndexOf(this);
                return (Parent != null ? Parent.PathId + $"{XmlHelper.PathDelimiter}" : "") + $"{Id}[{idx}]";
            }
        }

        /// <summary>
        /// Loads the container and it's sub-containers from the specified own xml node
        /// </summary>
        /// <param name="xmlNode">Own xml node</param>
        public override void LoadFromXml(XmlNode xmlNode)
        {
            Params.ForEach(p => p.LoadFromXml(xmlNode));
            Children.ForEach(c => c.LoadFromXml(xmlNode));
        }

        /// <summary>
        /// Returns the xml node as child of the parent node that shows the id of this node.
        /// If not found new node is created and returned.
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        protected override XmlNode GetOwnXmlNode(XmlNode parentXmlNode) => parentXmlNode.AppendChildNode(XmlHelper.ContainerTag);
    }
}
