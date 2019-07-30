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

namespace DataBase
{
    /// <summary>
    /// Container for the xml attributes used for serialization
    /// </summary>
    internal struct ParamAttributes
    {
        internal string Id;
        internal string Name;
        internal string Value;
        internal string ValueStr;
        internal string Unit;
    }

    /// <summary>
    /// Several helper function
    /// </summary>
    internal static class XmlHelper
    {
        // XML tags
        internal const string ContainerTag = "Cnt";
        internal const string ParamTag = "Pm";

        // Path delimiter
        internal const string PathDelimiter = ".";

        /// <summary>
        /// Returns the initialized static Attr property
        /// </summary>
        internal static ParamAttributes Attr = new ParamAttributes
        {
            Id = "id",
            Name = "name",
            Value = "val",
            ValueStr = "valStr",
            Unit = "unit"
        };

        /// <summary>
        /// Returns the xml attribute object of the specified xml node with the specified name,
        /// Returns null if not attribute is not found.
        /// </summary>
        /// <param name="xmlNode">The xml node</param>
        /// <param name="name">The attribute name</param>
        internal static XmlAttribute AttributeByName(this XmlNode xmlNode, string name)
        {
            for (var i = 0; i < (xmlNode.Attributes?.Count ?? 0); i++)
            {
                if (xmlNode.Attributes[i].Name == name)
                {
                    return xmlNode.Attributes[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the child xml node of the specified xml node with the specified name and id attribute.
        /// Returns null if not found.
        /// </summary>
        /// <param name="xmlNode">The parent xml node</param>
        /// <param name="tag">The name attribute</param>
        /// <param name="id">The id attribute</param>
        internal static XmlNode ChildNodeByTagAndId(this XmlNode xmlNode, string tag, string id)
        {
            return xmlNode.ChildNodes
                .Cast<XmlNode>()
                .FirstOrDefault(
                    childNode => childNode.Name == tag &&
                    childNode.AttributeByName(Attr.Id)?.Value == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="attributes"></param>
        internal static void SetAttributes(this XmlNode xmlNode, List<Tuple<string, string>> attributes = null)
        {
            if (xmlNode.OwnerDocument == null)
            {
                throw new NullReferenceException("XmlHelper.SetAttributes: owner document not set");
            }
            xmlNode.Attributes?.RemoveAll();
            attributes?.ForEach(a =>
            {
                var attr = xmlNode.Attributes?.Append(xmlNode.OwnerDocument.CreateAttribute(a.Item1));
                if (attr != null)
                {
                    attr.Value = a.Item2;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        internal static XmlNode AppendChildNode(this XmlNode xmlNode, string nodeName, List<Tuple<string, string>> attributes = null)
        {
            if (xmlNode.OwnerDocument == null)
            {
                throw new NullReferenceException("XmlHelper.AppendChildNode: owner document not set");
            }

            var childNode = xmlNode.AppendChild(xmlNode.OwnerDocument.CreateElement(nodeName));
            childNode?.SetAttributes(attributes);

            return childNode;
        }

        /// <summary>
        /// Performs the specified action in a foreach loop over the specified list
        /// </summary>
        internal static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// Performs the specified action in a foreach loop over the specified list
        /// </summary>
        internal static void For<T>(this IEnumerable<T> list, Action<int, T> action)
        {
            var i = 0;
            foreach (var item in list)
            {
                action(i++, item);
            }
        }

        /// <summary>
        /// Performs the specified action in a foreach loop over the specified list
        /// </summary>
        internal static void TimesDo(this int times, Action<int> action)
        {
            for (var i = 0; i < times; i++)
                action(i);
        }
    }
}