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
using System.Linq;
using System.Xml;
using DataBase.Parameters;
using DataBase.UndoRedo;

namespace DataBase.Container
{
    /// <summary>
    /// Container owning other sub-containers and parameters
    /// </summary>
    public class DataContainer: DataNode
    {
        private bool _isRoot;

        /// <summary>
        /// Contains the top most container (traversing upwards the first one that has a null parent container)
        /// </summary>
        internal DataContainer Root;

        public event Action OnDataLoaded = () => { };

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Container identification</param>
        /// <param name="designation">Container name</param>
        /// <param name="undoRedoActivated">True if undo/redo mechanism should be activated</param>
        protected DataContainer(DataContainer parent, string id, string designation = "", bool undoRedoActivated = true) 
            : base(parent, id, designation)
        {
            Children = new List<DataContainer>();
            Params = new List<DataParameterBase>();

            Parent?.Children.Add(this);

            DetermineRoot();

            UndoRedo = undoRedoActivated
                ? (_isRoot ? new UndoRedoStack() : Root.UndoRedo)
                : null;
        }

        /// <summary>
        /// List of sub-containers
        /// </summary>
        public IList<DataContainer> Children { get; }

        /// <summary>
        /// List of parameters
        /// </summary>
        public IList<DataParameterBase> Params { get; }

        /// <summary>
        /// Returns the list of child nodes regardless if container or parameter
        /// </summary>
        public IList<DataNode> Nodes => Children.Concat(Params.Cast<DataNode>()).ToList();

        /// <summary>
        /// Returns the undo/redo stack object
        /// </summary>
        public UndoRedoStack UndoRedo { get; }

        /// <summary>
        /// Returns a deep clone of this container
        /// </summary>
        public DataContainer Clone()
        {
            var clone = Activator.CreateInstance(GetType()) as DataContainer; 
            clone?.CloneFrom(this);
            return clone;
        }

        /// <summary>
        /// Returns true if one of it's parameters or the parameters of one of it's sub- (and sub-sub) containers is modified
        /// </summary>
        public override bool IsModified =>
            Children.Any(c => c.IsModified) ||
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
                doc.LoadXml($"<{XmlHelper.ContainerTag}/>");
                xmlNode = doc.DocumentElement;
            }
            else
            {
                xmlNode = GetOwnXmlNode(parentXmlNode) ??
                    parentXmlNode.AppendChildNode(XmlHelper.ContainerTag);
            }

            xmlNode.SetAttributes(new List<Tuple<string, string>>
            {
                new Tuple<string, string>(XmlHelper.Attr.Id, Id),
                new Tuple<string, string>(XmlHelper.Attr.Name, Designation)
            });

            Params.ForEach(p => p.SaveToXml(xmlNode));
            Children.ForEach(c => c.SaveToXml(xmlNode));

            return doc;
        }

        /// <summary>
        /// Clones the sub containers and parameters from the specified container
        /// </summary>
        internal virtual void CloneFrom(DataContainer aContainer)
        {
            if (aContainer.PathId != PathId || aContainer.GetType() != GetType())
            {
                throw new InvalidOperationException("DataContainer.CloneFrom: container ids or types not matching");
            }
            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].CloneFrom(aContainer.Children[i]);
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
            var xmlNode = parentXmlNode.ChildNodeByTagAndId(XmlHelper.ContainerTag, Id);
            if (xmlNode == null)
            {
                return;
            }
            Params.ForEach(p => p.LoadFromXml(xmlNode));
            Children.ForEach(c => c.LoadFromXml(xmlNode));
        }

        /// <summary>
        /// Loads the container and it's sub-containers from the specified file
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <param name="shallResetModifiedState">True if the modified flag should be reset after loading</param>
        public void LoadFromFile(string fileName, bool shallResetModifiedState = true)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);
            UndoRedo.IsMuted = true;

            try
            {
                Children.ForEach(c => c.LoadFromXml(doc.DocumentElement));
            }
            finally
            {
                UndoRedo.IsMuted = false;
            }

            if (shallResetModifiedState)
            {
                ResetModifiedState();
            }

            OnDataLoaded();
        }

        /// <summary>
        /// Saves the container and it's sub-containers to the specified file
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <param name="resetModified">True if the modified flag should be reset after saving</param>
        public void SaveToFile(string fileName, bool resetModified = true)
        {
            SaveToXml(null).Save(fileName);
            if (resetModified)
            {
                ResetModifiedState();
            }
        }

        /// <summary>
        /// Resets the modified flag of all parameters and those of the sub-containers to false by setting the buffered value to the current value
        /// </summary>
        public override void ResetModifiedState()
        {
            Params.ForEach(p => p.ResetModifiedState());
            Children.ForEach(c => c.ResetModifiedState());
        }

        /// <summary>
        /// Sets the values of all parameters and of those of the sub-containers to the buffered value
        /// </summary>
        public override void Restore()
        {
            Params.ForEach(p => p.Restore());
            Children.ForEach(c => c.Restore());
        }

        /// <summary>
        /// Sets the value to the default value
        /// </summary>
        public override void SetToDefault()
        {
            Params.ForEach(p => p.SetToDefault());
            Children.ForEach(c => c.SetToDefault());
        }

        /// <summary>
        /// Returns the xml node as child of the parent node that shows the id of this node.
        /// If not found new node is created and returned.
        /// </summary>
        /// <param name="parentXmlNode">The parent xml node</param>
        /// <returns></returns>
        protected virtual XmlNode GetOwnXmlNode(XmlNode parentXmlNode) => parentXmlNode.ChildNodeByTagAndId(XmlHelper.ContainerTag, Id);

        /// <summary>
        /// Determines the root container (traversing upwards the first one that has a null parent container)
        /// </summary>
        private void DetermineRoot()
        {
            var container = this;
            _isRoot = container.Parent == null;
            while (true)
            {
                if (container.Parent == null)
                {
                    Root = container;
                    return;
                }
                container = container.Parent;
            }
        }
    }
}
