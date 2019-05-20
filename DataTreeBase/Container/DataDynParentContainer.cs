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
    public class DataDynParentContainer<T> : DataContainer, IUndoRedoableNode where T: DataDynContainer
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

            Containers.ForEach(c => _bufferedContainers.Add(c));

            _undoRedo = Root?.UndoRedo;
        }

        /// <summary>
        /// List of sub-containers
        /// </summary>
        public new IReadOnlyList<T> Containers => base.Containers.Cast<T>().ToList();

        /// <summary>
        /// Notifies to the undo/redo stack a change in the dynamic child container list
        /// </summary>
        /// <param name="actionType">Added (also inserted), removed or cleared</param>
        /// <param name="index">Index of the concerning child container</param>
        /// <param name="oldContainers">List containing the one removed container or in case of 'Clear' the whole list</param>
        /// <param name="newContainers">List containing the one added container or (in case of 'Remove' or 'Clear') is empty</param>
        private void DynContainersChanged(UndoAction actionType, int index, List<DataDynContainer> oldContainers, List<DataDynContainer> newContainers)
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
        /// <returns>The new DataContainer</returns>
        public T Add()
        {
            var type = typeof(T);
            var cont = Activator.CreateInstance(type, this) as T;
            DynContainersChanged(actionType: UndoAction.Add,
                                 index: base.Containers.IndexOf(cont),
                                 oldContainers: new List<DataDynContainer>(),
                                 newContainers: new List<DataDynContainer>() { cont });
            return cont;
        }

        /// <summary>
        /// Adds a new data tree container to the list of sub containers executing the specified init action
        /// </summary>
        /// <returns>The new DataContainer</returns>
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
                                 oldContainers: new List<DataDynContainer>(),
                                 newContainers: new List<DataDynContainer>() { container });
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
            RemoveAt(base.Containers.IndexOf(container));
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
                                 oldContainers: new List<DataDynContainer>() { cont },
                                 newContainers: new List<DataDynContainer>());
        }

        /// <summary>
        /// Removes the container at the end of the list and returns it
        /// </summary>
        public T Pop()
        {
            var index = Containers.Count - 1;
            var cont = Containers.Last();
            RemoveAt(index);
            return cont;
        }

        /// <summary>
        /// Removes all sub containers
        /// </summary>
        public void Clear()
        {
            base.Containers.Clear();
            DynContainersChanged(actionType: UndoAction.Clear,
                                 index: -1,
                                 oldContainers: Containers.Cast<DataDynContainer>().ToList(),
                                 newContainers: new List<DataDynContainer>());
        }

        /// <summary>
        /// Loads the container and it's sub-containers dynamically from the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode">The parent node</param>
        public override void LoadFromXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByTagAndId(XmlHelper.ContnTag, Id);
            if (xmlNode == null) return;

            Clear();

            xmlNode.ChildNodes.Cast<XmlNode>().
                Where(ch => ch.Name == XmlHelper.ContnTag).
                ForEach(ch => Add().LoadFromXml(ch));

            Params.ForEach(p => p.LoadFromXml(xmlNode));
        }

        /// <summary>
        /// Clones the sub containers and parameters from the specified container
        /// </summary>
        internal override void CloneFrom(DataContainer aContainer)
        {
            if (aContainer.PathId != PathId || aContainer.GetType() != GetType())
                throw new InvalidOperationException("DataDynParentContainer.CloneFrom: container ids or types not matching");

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
                        undoData.Containers.For((i, c) => Insert(i, (T) c)); // Undo
                    else
                        Clear();                                                 // Redo
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns true if one of it's parameters or the parameters of one of it's sub- (and sub-sub) containers is modified
        /// </summary>
        public override bool IsModified => base.IsModified && 
                                           base.Containers.Count != _bufferedContainers.Count;

        /// <summary>
        /// Sets the values of all parameters and of those of the sub-containers to the buffered value
        /// </summary>
        public override void Restore()
        {
            Clear();
            _bufferedContainers.ForEach(b => base.Containers.Add(b));
        }


        /// <summary>
        /// Resets the modified flag of all parameters and those of the sub-containers to false by setting the buffered value to the current value
        /// </summary>
        public override void ResetModified()
        {
            _bufferedContainers.Clear();
            Containers.ForEach(c => _bufferedContainers.Add(c));
        }

        /// <summary>
        /// Sets the configuration of the dynamic sub containers to the default value
        /// </summary>
        public override void SetToDefault()
        {
            Clear();
            _initDefaultContainers?.Invoke(this);
        }
    }
}
