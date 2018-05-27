using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace DataTreeBase
{
    /// <summary>
    /// Container owning other sub-containers and parameters
    /// </summary>
    public class DataTreeContainer: DataTreeNode
    {
        private const string Tagname = "C";

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Container identfication</param>
        /// <param name="name">Container name</param>
        protected DataTreeContainer(DataTreeContainer parent, string id, string name) 
            : base(parent, id, name)
        {
            Containers = new List<DataTreeContainer>();
            Params = new List<DataTreeParameterBase>();

            Parent?.Containers.Add(this);
        }

        /// <summary>
        /// List of sub-containers
        /// </summary>
        public IList<DataTreeContainer> Containers { get; }

        /// <summary>
        /// List of parameters
        /// </summary>
        public IList<DataTreeParameterBase> Params { get; }

        /// <summary>
        /// Retruns true if one of it's parameters or the parameters of one of it's sub- (and sub-sub) containers is modified
        /// </summary>
        public override bool IsModified =>
                Containers.Any(c => c.IsModified) ||
                Params.Any(p => p.IsModified);

        /// <summary>
        /// Saves the container to the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode"></param>
        /// <returns>Xml document if the parent xml node is empty (means new document was created)</returns>
        public XmlDocument SaveToXml(XmlNode parentXmlNode)
        {
            XmlDocument doc = null;
            XmlNode xmlNode;

            if (parentXmlNode == null)
            {
                doc = new XmlDocument();
                doc.LoadXml($"<{Tagname}/>");
                xmlNode = doc.DocumentElement;
            }
            else
            {
                xmlNode = parentXmlNode.ChildNodeByNameAndId(Tagname, Id) ??
                          parentXmlNode.AppendChildNode(Tagname);
            }

            xmlNode.SetAttributes(new List<Tuple<string, string>>
                                  {
                                      new Tuple<string, string>(Helper.Attr.Id, Id),
                                      new Tuple<string, string>(Helper.Attr.Name, Name),
                                  });

            Params.ForEach(p => p.SaveToXml(xmlNode));
            Containers.ForEach(c => c.SaveToXml(xmlNode));

            return doc;
        }

        /// <summary>
        /// Loads the container and it's sub-containers from the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode"></param>
        public void LoadFromXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByNameAndId(Tagname, Id);
            if (xmlNode == null) return;
            
            Params.ForEach(p => p.LoadFromXml(xmlNode));
            Containers.ForEach(c => c.LoadFromXml(xmlNode));
        }

        /// <summary>
        /// Loads the container and it's sub-containers from the specified file
        /// </summary>
        /// <param name="fileName">The file name</param>
        public void LoadFromFile(string fileName)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);
            LoadFromXml(doc.DocumentElement);
            ResetModified();
        }

        /// <summary>
        /// Saves the container and it's sub-containers to the specified file
        /// </summary>
        /// <param name="fileName">The file name</param>
        public void SaveToFile(string fileName)
        {
            SaveToXml(null).Save(fileName);
            ResetModified();
        }

        /// <summary>
        /// Resets the modified flag of all parameters and those of the sub-containers to false by setting the buffered value to the current value
        /// </summary>
        public void ResetModified()
        {
            Params.ForEach(p => p.ResetModified());
            Containers.ForEach(c => c.ResetModified());
        }

        /// <summary>
        /// Sets the values of all parameters and of those of the sub-containers to the buffered value
        /// </summary>
        public void Restore()
        {
            Params.ForEach(p => p.Restore());
            Containers.ForEach(c => c.Restore());
        }
    }
}
