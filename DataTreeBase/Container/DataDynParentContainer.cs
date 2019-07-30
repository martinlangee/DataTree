using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DataBase.Interfaces;
using DataBase.UndoRedo;

namespace DataBase.Container
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
        internal List<DataDynContainer> Containers = new List<DataDynContainer>();
    }

    /// <summary>
    /// Represents a container where sub-containers can be added and removed dynamically
    /// </summary>
    public class DataDynParentContainer<T> : DataContainer, IUndoRedoableNode, IEnumerable<T> where T : DataDynContainer
    {
        private readonly UndoRedoStack _undoRedo;
        private readonly Action<DataDynParentContainer<T>> _initDefaultContainers;
        private readonly List<T> _bufferedContainers = new List<T>();

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Container identfication</param>
        /// <param name="designation">Container name</param>
        /// <param name="initDefaultContainers">Action that initializes the default configuration of this container's dynamic sub containers</param>
        protected DataDynParentContainer(DataContainer parent, string id, string designation, Action<DataDynParentContainer<T>> initDefaultContainers = null)
            : base(parent, id, designation)
        {
            _initDefaultContainers = initDefaultContainers;
            _initDefaultContainers?.Invoke(this);

            Items.ForEach(c => _bufferedContainers.Add(c));

            _undoRedo = Root?.UndoRedo;
        }

        public T this[int index] => Items[index];

        /// <summary>
        /// List of sub-containers
        /// </summary>
        public IReadOnlyList<T> Items => Children.Cast<T>().ToList();

        /// <summary>
        /// Notifies to the undo/redo stack a change in the dynamic child container list
        /// </summary>
        /// <param name="actionType">Added (also inserted), removed or cleared</param>
        /// <param name="index">Index of the concerning child container</param>
        /// <param name="oldContainers">List containing the one removed container or in case of 'Clear' the whole list</param>
        /// <param name="newContainers">List containing the one added container or (in case of 'Remove' or 'Clear') is empty</param>
        private void DynContainersChanged(UndoAction actionType, int index, List<DataDynContainer> oldContainers, List<DataDynContainer> newContainers)
        {
            _undoRedo?.ValueChanged(
                this,
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
        /// <returns>The new DataContainer</returns>
        public T Add()
        {
            var item = Activator.CreateInstance(typeof(T), this) as T;
            DynContainersChanged(
                actionType: UndoAction.Add,
                index: Children.IndexOf(item),
                oldContainers: new List<DataDynContainer>(),
                newContainers: new List<DataDynContainer>() { item });
            return item;
        }

        /// <summary>
        /// Adds a new data tree container to the list of sub containers executing the specified init action
        /// </summary>
        /// <returns>The new DataContainer</returns>
        public T Add(Action<T> initAction)
        {
            var item = Add();
            initAction?.Invoke(item);
            return item;
        }

        /// <summary>
        /// Inserts the specified container at the specified index position
        /// </summary>
        private void Insert(int index, T container)
        {
            Children.Insert(index, container);
            DynContainersChanged(
                actionType: UndoAction.Add,
                index: index,
                oldContainers: new List<DataDynContainer>(),
                newContainers: new List<DataDynContainer>() { container });
        }

        /// <summary>
        /// Inserts a new container at the specified index position
        /// </summary>
        /// <returns>The new container, created by this call.</returns>
        public T Insert(int index)
        {
            var item = Activator.CreateInstance(typeof(T), this) as T;
            Insert(index, item);
            return item;
        }

        /// <summary>
        /// Inserts a new container at the specified index position executing the specified init action
        /// </summary>
        public T Insert(int index, Action<T> initAction)
        {
            var item = Insert(index);
            initAction?.Invoke(item);
            Insert(index, item);
            return item;
        }

        /// <summary>
        /// Deletes the specified dynamic sub container from the list
        /// </summary>
        public void Remove(T container) => RemoveAt(base.Children.IndexOf(container));

        /// <summary>
        /// Deletes the dynamic sub container at the specified index from the list
        /// </summary>
        public void RemoveAt(int index)
        {
            var item = Items[index];
            Children.RemoveAt(index);
            DynContainersChanged(
                actionType: UndoAction.Remove,
                index: index,
                oldContainers: new List<DataDynContainer>() { item },
                newContainers: new List<DataDynContainer>());
        }

        /// <summary>
        /// Removes the container at the end of the list and returns it
        /// </summary>
        public T Pop()
        {
            var index = Items.Count - 1;
            var item = Items.Last();
            RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Removes all sub containers
        /// </summary>
        public void Clear()
        {
            Children.Clear();
            DynContainersChanged(
                actionType: UndoAction.Clear,
                index: -1,
                oldContainers: Items.Cast<DataDynContainer>().ToList(),
                newContainers: new List<DataDynContainer>());
        }

        /// <summary>
        /// Loads the container and it's sub-containers dynamically from the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode">The parent node</param>
        public override void LoadFromXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByTagAndId(XmlHelper.ContainerTag, Id);
            if (xmlNode == null)
            {
                return;
            }

            Clear();

            xmlNode.ChildNodes
                .Cast<XmlNode>()
                .Where(ch => ch.Name == XmlHelper.ContainerTag)
                .ForEach(ch => Add().LoadFromXml(ch));

            Params.ForEach(p => p.LoadFromXml(xmlNode));
        }

        /// <summary>
        /// Clones the sub containers and parameters from the specified container
        /// </summary>
        internal override void CloneFrom(DataContainer aContainer)
        {
            if (aContainer.PathId != PathId || aContainer.GetType() != GetType())
            {
                throw new InvalidOperationException("DataDynParentContainer.CloneFrom: container ids or types not matching");
            }
            for (var i = 0; i < aContainer.Children.Count; i++)
            {
                if (Items.Count >= i)
                {
                    Add();
                }
                Items[i].CloneFrom(aContainer.Children[i]);
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
            var undoData = (DynContainerUndoData)value;
            switch (undoData.ActionType)
            {
                // Child container added
                case UndoAction.Add:
                    if (undoData.Containers.Count == 0)
                    {
                        RemoveAt(undoData.Index);                               // Undo
                    }
                    else
                    {
                        Insert(undoData.Index, (T)undoData.Containers[0]);     // Redo
                    }
                    break;

                // Child container removed
                case UndoAction.Remove:
                    if (undoData.Containers.Count > 0)
                    {
                        Insert(undoData.Index, (T)undoData.Containers[0]);     // Undo
                    }
                    else
                    {
                        RemoveAt(undoData.Index);                               // Redo
                    }
                    break;

                // all child containers removed
                case UndoAction.Clear:
                    if (undoData.Containers.Count > 0)
                    {
                        undoData.Containers.ForEach((i, c) => Insert(i, (T)c)); // Undo
                    }
                    else
                    {
                        Clear();                                                 // Redo
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns true if one of it's parameters or the parameters of one of it's sub- (and sub-sub) containers is modified
        /// </summary>
        public override bool IsModified =>
            base.IsModified ||
            Children.Count != _bufferedContainers.Count;

        /// <summary>
        /// Sets the values of all parameters and of those of the sub-containers to the buffered value
        /// </summary>
        public override void Restore()
        {
            Clear();
            _bufferedContainers.ForEach(b => base.Children.Add(b));
        }


        /// <summary>
        /// Resets the modified flag of all parameters and those of the sub-containers to false by setting the buffered value to the current value
        /// </summary>
        public override void ResetModifiedState()
        {
            _bufferedContainers.Clear();
            Items.ForEach(c => _bufferedContainers.Add(c));
            base.ResetModifiedState();
        }

        /// <summary>
        /// Sets the configuration of the dynamic sub containers to the default value
        /// </summary>
        public override void SetToDefault()
        {
            Clear();
            _initDefaultContainers?.Invoke(this);
        }

        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
