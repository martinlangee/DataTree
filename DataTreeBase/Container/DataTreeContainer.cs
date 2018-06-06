using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using DataTreeBase.Parameters;
using DataTreeBase.UndoRedo;

namespace DataTreeBase.Container
{
    /// <summary>
    /// Container owning other sub-containers and parameters
    /// </summary>
    public class DataTreeContainer: DataTreeBaseNode
    {
        private bool _isRoot = false;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Container identfication</param>
        /// <param name="name">Container name</param>
        /// <param name="undoRedoActivated">True if undo/redo mechanism should be activated</param>
        protected DataTreeContainer(DataTreeContainer parent, string id, string name, bool undoRedoActivated = true) 
            : base(parent, id, name)
        {
            Containers = new List<DataTreeContainer>();
            Params = new List<DataTreeParameterBase>();

            Parent?.Containers.Add(this);

            DetermineRoot();

            UndoRedo = undoRedoActivated
                           ? (_isRoot ? new UndoRedoStack() : Root.UndoRedo)
                           : null;
        }

        /// <summary>
        /// Contains the top most container (traversing upwards the first one that has a null parent container)
        /// </summary>
        internal DataTreeContainer Root;

        /// <summary>
        /// Determines the root container (traversing upwards the first one that has a null parent container)
        /// </summary>
        private void DetermineRoot()
        {
            var cont = this;
            while (true)
            {
                if (cont?.Parent == null)
                {
                    Root = cont;
                    _isRoot = true;
                    return;
                }
                cont = cont?.Parent;
            }
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
        /// Returns the undo/redo stack object
        /// </summary>
        public UndoRedoStack UndoRedo { get; }

        /// <summary>
        /// Returns a deep clone of this container
        /// </summary>
        public DataTreeContainer Clone()
        {
            var clone = Activator.CreateInstance(GetType()) as DataTreeContainer; 
            clone?.CloneFrom(this);
            return clone;
        }

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
                doc.LoadXml($"<{Helper.ContnTag}/>");
                xmlNode = doc.DocumentElement;
            }
            else
            {
                xmlNode = GetOwnXmlNode(parentXmlNode) ??
                          parentXmlNode.AppendChildNode(Helper.ContnTag);
            }

            xmlNode.SetAttributes(new List<Tuple<string, string>>
                                  {
                                      new Tuple<string, string>(Helper.Attr.Id, Id),
                                      new Tuple<string, string>(Helper.Attr.Name, Name)
                                  });

            Params.ForEach(p => p.SaveToXml(xmlNode));
            Containers.ForEach(c => c.SaveToXml(xmlNode));

            return doc;
        }

        /// <summary>
        /// Returns the xml node as child of the parent node that shows the id of this node.
        /// If not found new node is created and returned.
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        /// <returns></returns>
        protected virtual XmlNode GetOwnXmlNode(XmlNode parentXmlNode)
        {
            return parentXmlNode.ChildNodeByTagAndId(Helper.ContnTag, Id);
        }

        /// <summary>
        /// Clones the sub containers and parameters from the specified container
        /// </summary>
        internal virtual void CloneFrom(DataTreeContainer aContainer)
        {
            if (aContainer.PathId != PathId || aContainer.GetType() != GetType())
                throw new InvalidOperationException("DataTreeContainer.CloneFrom: container ids or types not matching");

            for (var i = 0; i < Containers.Count; i++)
            {
                Containers[i].CloneFrom(aContainer.Containers[i]);
            }

            for (var i = 0; i < Params.Count; i++)
            {
                Params[i].CloneFrom(aContainer.Params[i]);
            }
        }

        /// <summary>
        /// Loads the container and it's sub-containers from the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode"></param>
        public virtual void LoadFromXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByTagAndId(Helper.ContnTag, Id);
            if (xmlNode == null)
                return;

            Params.ForEach(p => p.LoadFromXml(xmlNode));
            Containers.ForEach(c => c.LoadFromXml(xmlNode));
        }

        /// <summary>
        /// Loads the container and it's sub-containers from the specified file
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <param name="resetModified">True if the modified flag should be reset after loading</param>
        public void LoadFromFile(string fileName, bool resetModified = true)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);
            UndoRedo.IsMuted = true;
            try
            {
                Containers.ForEach(c => c.LoadFromXml(doc.DocumentElement));
            }
            finally
            {
                UndoRedo.IsMuted = false;
            }
            if (resetModified) ResetModified();
        }

        /// <summary>
        /// Saves the container and it's sub-containers to the specified file
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <param name="resetModified">True if the modified flag should be reset after saving</param>
        public void SaveToFile(string fileName, bool resetModified = true)
        {
            SaveToXml(null).Save(fileName);
            if (resetModified) ResetModified();
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
