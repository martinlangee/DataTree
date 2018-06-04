using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace DataTreeBase
{
    internal enum UndoAction
    {
        Add,
        Remove,
        Clear
    }

    internal class DynContainerUndoData
    {
        internal UndoAction Action;
        internal int Index;
        internal List<DataTreeDynContainer> Containers = new List<DataTreeDynContainer>();
    }

    /// <summary>
    /// Represents a container where sub-containers can be added and removed dynamically
    /// </summary>
    public class DataTreeDynParentContainer<T> : DataTreeContainer, IUndoRedoNode where T: DataTreeDynContainer
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
            _undoRedo = Root.UndoRedo;
        }

        /// <summary>
        /// List of sub-containers
        /// </summary>
        protected new IList<T> Containers => base.Containers.Cast<T>().ToList();

        /// <summary>
        /// Adds a new data tree container to the list of sub containers
        /// </summary>
        /// <returns>The new DataTreeContainer</returns>
        public T Add()
        {
            var type = typeof(T);
            var cont = Activator.CreateInstance(type, this) as T;
            _undoRedo.NotifyChangeEvent(this,
                                       new DynContainerUndoData
                                       {
                                           Action = UndoAction.Add,
                                           Index = Containers.IndexOf(cont),
                                           Containers = new List<DataTreeDynContainer>()
                                       },
                                       new DynContainerUndoData
                                       {
                                           Action = UndoAction.Add,
                                           Index = Containers.IndexOf(cont),
                                           Containers = new List<DataTreeDynContainer>() { cont }
                                       });
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
            Containers.Insert(index, container);
            _undoRedo.NotifyChangeEvent(this,
                           new DynContainerUndoData
                           {
                               Action = UndoAction.Add,
                               Index = index,
                               Containers = new List<DataTreeDynContainer>()
                           },
                           new DynContainerUndoData
                           {
                               Action = UndoAction.Add,
                               Index = index,
                               Containers = new List<DataTreeDynContainer>() { container }
                           });
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
            Containers.RemoveAt(Containers.IndexOf(container));
        }

        /// <summary>
        /// Deletes the dynamic sub container at the specified index from the list
        /// </summary>
        public void RemoveAt(int index)
        {
            var cont = Containers[index];
            Containers.RemoveAt(index);
            _undoRedo.NotifyChangeEvent(this,
                           new DynContainerUndoData
                           {
                               Action = UndoAction.Add,
                               Index = index,
                               Containers = new List<DataTreeDynContainer>() { cont }
                           },
                           new DynContainerUndoData
                           {
                               Action = UndoAction.Add,
                               Index = index,
                               Containers = new List<DataTreeDynContainer>()
                           });
        }

        /// <summary>
        /// Removes all sub containers
        /// </summary>
        public void Clear()
        {
            Containers.Clear();
            _undoRedo.NotifyChangeEvent(this,
                           new DynContainerUndoData
                           {
                               Action = UndoAction.Clear,
                               Index = -1,
                               Containers = Containers.Cast<DataTreeDynContainer>().ToList()
                           },
                           new DynContainerUndoData
                           {
                               Action = UndoAction.Add,
                               Index = -1,
                               Containers = new List<DataTreeDynContainer>()
                           });
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
        /// Triggering this node to load the specified value
        /// </summary>
        public void Set(object value)
        {
            var undoData = (DynContainerUndoData) value;

            switch (undoData.Action)
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
