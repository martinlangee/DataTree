using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace DataTreeBase
{
    internal struct Attributes
    {
        internal string Id;
        internal string Name;
        internal string Value;
        internal string Unit;
    }

    internal static class XmlHelper
    {
        internal static Attributes Attr = new Attributes
                                          {
                                              Id = "id",
                                              Name = "name",
                                              Value = "val",
                                              Unit = "unit"
                                          };

        internal static XmlAttribute AttributeByName(this XmlNode xmlNode, string name)
        {
            for (var z = 0; z < xmlNode.Attributes?.Count; z++)
            {
                if (xmlNode.Attributes[z].Name == name)
                    return xmlNode.Attributes[z];
            }
            return null;
        }

        internal static XmlNode ChildNodeByNameAndId(this XmlNode xmlNode, string name, string id)
        {
            return xmlNode.ChildNodes.Cast<XmlNode>().
                           FirstOrDefault(childNode => (childNode.Name == name) && (childNode.AttributeByName(Attr.Id)?.Value == id));
        }

        internal static void SetAttributes(this XmlNode xmlNode, XmlDocument xmlDoc, List<Tuple<string, string>> attributes = null)
        {
            xmlNode.Attributes?.RemoveAll();

            attributes?.ForEach(a =>
            {
                var attr = xmlNode.Attributes?.Append(xmlDoc.CreateAttribute(a.Item1));
                if (attr != null)
                    attr.Value = a.Item2;
            });
        }

        internal static XmlNode AppendChildNode(this XmlNode xmlNode, XmlDocument xmlDoc, string nodeName, List<Tuple<string, string>> attributes = null)
        {
            var childNode = xmlNode.AppendChild(xmlDoc.CreateElement(nodeName));
            childNode?.SetAttributes(xmlDoc, attributes);

            return childNode;
        }

        internal static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
    }
}