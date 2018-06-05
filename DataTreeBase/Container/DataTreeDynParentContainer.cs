using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using DataTreeBase.Interfaces;
using DataTreeBase.UndoRedo;

namespace DataTreeBase.Container
{
    /// <summary>
    /// Specifying the type if action that has to be undone
    /// </summary>
    internal enum UndoAction
    {
        Add,
        Remove,
        Clear
    }

    /// <summary>
    /// Data container boxing the data needed for undo/redo of dynamic container lists
    /// </summary>
    internal sealed class DynContainerUndoData
    {
        internal UndoAction ActionType;
        internal int Index;
        internal List<DataTreeDynContainer> Containers = new List<DataTreeDynContainer>();
    }

    /// <summary>
    /// Represents a container where sub-containers can be added and removed dynamically
    /// </summary>
    public class DataTreeDynParentContainer<T> : DataTreeContainer, IUndoRedoableNode where T: DataTreeDynContainer
    {
        private readonly UndoRedoStack _undoRedo;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Container identfication</param>
        /// <param name="name">Container name</param>
        protected DataTreeDynParentContainer(DataTreeContainer parent, string id, string name)
            : base(parent, id, name)
        {
            _undoRedo = Root?.UndoRedo;
        }

        /// <summary>
        /// List of sub-containers
        /// </summary>
        protected new IReadOnlyList<T> Containers => base.Containers.Cast<T>().ToList();

        /// <summary>
        /// Notifies to the undo/redo stack a change in the dynamic child container list
        /// </summary>
        /// <param name="actionType">Added (also inserted), removed or cleared</param>
        /// <param name="index">Index of the concerning child container</param>
        /// <param name="oldContainers">List containing the one removed container or in case of 'Clear' the whole list</param>
        /// <param name="newContainers">List containing the one added container or (in case of 'Remove' or 'Clear') is empty</param>
        private void DynContainersChanged(UndoAction actionType, int index, List<DataTreeDynContainer> oldContainers, List<DataTreeDynContainer> newContainers)
        {
            _undoRedo?.ValueChanged(this,
                                         new DynContainerUndoData
                                         {
                                             ActionType = actionType,
                                             Index = index,
                                             Containers = oldContainers
                                         },
                                         new DynContainerUndoData
                                         {
                                             ActionType = actionType,
                                             Index = index,
                                             Containers = newContainers
                                         });
        }

        /// <summary>
        /// Adds a new data tree container to the list of sub containers
        /// </summary>
        /// <returns>The new DataTreeContainer</returns>
        public T Add()
        {
            var type = typeof(T);
            var cont = Activator.CreateInstance(type, this) as T;
            DynContainersChanged(actionType: UndoAction.Add,
                                 index: base.Containers.IndexOf(cont),
                                 oldContainers: new List<DataTreeDynContainer>(),
                                 newContainers: new List<DataTreeDynContainer>() { cont });
            return cont;
        }

        /// <summary>
        /// Adds a new data tree container to the list of sub containers executing the specified init action
        /// </summary>
        /// <returns>The new DataTreeContainer</returns>
        public T Add(Action<T> initAction)
        {
            var cont = Add();
            initAction?.Invoke(cont);
            return cont;
        }

        /// <summary>
        /// Inserts the specified container at the specified index position
        /// </summary>
        private void Insert(int index, T container)
        {
            base.Containers.Insert(index, container);
            DynContainersChanged(actionType: UndoAction.Add,
                                 index: index,
                                 oldContainers: new List<DataTreeDynContainer>(),
                                 newContainers: new List<DataTreeDynContainer>() { container });
        }

        /// <summary>
        /// Inserts a new container at the specified index position
        /// </summary>
        public T Insert(int index)
        {
            var type = typeof(T);
            var cont = Activator.CreateInstance(type, this) as T;
            Insert(index, cont);
            return cont;
        }

        /// <summary>
        /// Inserts a new container at the specified index position executing the specified init action
        /// </summary>
        public T Insert(int index, Action<T> initAction)
        {
            var cont = Insert(index);
            initAction?.Invoke(cont);
            Insert(index, cont);
            return cont;
        }

        /// <summary>
        /// Deletes the specified dynamic sub container from the list
        /// </summary>
        public void Remove(T container)
        {
            base.Containers.RemoveAt(base.Containers.IndexOf(container));
        }

        /// <summary>
        /// Deletes the dynamic sub container at the specified index from the list
        /// </summary>
        public void RemoveAt(int index)
        {
            var cont = Containers[index];
            base.Containers.RemoveAt(index);
            DynContainersChanged(actionType: UndoAction.Remove,
                                 index: index,
                                 oldContainers: new List<DataTreeDynContainer>() { cont },
                                 newContainers: new List<DataTreeDynContainer>());
        }

        /// <summary>
        /// Removes all sub containers
        /// </summary>
        public void Clear()
        {
            base.Containers.Clear();
            DynContainersChanged(actionType: UndoAction.Clear,
                                 index: -1,
                                 oldContainers: Containers.Cast<DataTreeDynContainer>().ToList(),
                                 newContainers: new List<DataTreeDynContainer>());
        }

        /// <summary>
        /// Loads the container and it's sub-containers dynamically from the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode">The parent node</param>
        public override void LoadFromXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByTagAndId(Helper.ContnTag, Id);
            if (xmlNode == null) return;

            Clear();

            xmlNode.ChildNodes.Cast<XmlNode>().
                Where(ch => ch.Name == Helper.ContnTag).
                ForEach(ch => Add().LoadFromXml(ch));

            Params.ForEach(p => p.LoadFromXml(xmlNode));
        }

        /// <summary>
        /// Clones the sub containers and parameters from the specified container
        /// </summary>
        internal override void CloneFrom(DataTreeContainer aContainer)
        {
            if (aContainer.PathId != PathId || aContainer.GetType() != GetType())
                throw new InvalidOperationException("DataTreeDynParentContainer.CloneFrom: container ids or types not matching");

            for (var i = 0; i < aContainer.Containers.Count; i++)
            {
                if (Containers.Count >= i) Add();
                Containers[i].CloneFrom(aContainer.Containers[i]);
            }

            for (var i = 0; i < Params.Count; i++)
            {
                Params[i].CloneFrom(aContainer.Params[i]);
            }
        }

        /// <summary>
        /// Set the new container as result of the undo or redo process
        /// </summary>
        public void Set(object value)
        {
            var undoData = (DynContainerUndoData) value;

            switch (undoData.ActionType)
            {
                // Child container added
                case UndoAction.Add:
                    if (undoData.Containers.Count == 0)
                        RemoveAt(undoData.Index);                               // Undo
                    else
                        Insert(undoData.Index, (T) undoData.Containers[0]);     // Redo
                    break;

                // Child container removed
                case UndoAction.Remove:
                    if (undoData.Containers.Count > 0)
                        Insert(undoData.Index, (T) undoData.Containers[0]);     // Undo
                    else
                        RemoveAt(undoData.Index);                               // Redo
                    break;

                // all child containers removed
                case UndoAction.Clear:
                    if (undoData.Containers.Count > 0)
                        undoData.Containers.ForEach((i, c) => Insert(i, (T) c)); // Undo
                    else
                        Clear();                                                 // Redo
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
