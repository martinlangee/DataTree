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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DataTree.Ui.Helper
{
    /// <summary>
    /// This is taken from: http://breakingdotnet.blogspot.com/2012/05/data-template-selector-in-xaml.html
    /// </summary>

    [MarkupExtensionReturnType(typeof(DataTemplateSelector))]
    public class TemplateSelectorExtension : MarkupExtension
    {
        #region Ctor

        public TemplateSelectorExtension()
            : base()
        {
            _defaultKeySelector = i =>
            {
                var type = i.GetType().GetProperty(Property).GetValue(i, null).ToString();
                return type;
            };
        }

        public TemplateSelectorExtension(TemplateDictionary templatePresenter)
            : this()
        {
            TemplateDict = templatePresenter;
        }

        #endregion

        #region Properties

        public string Property { get; set; }

        public TemplateDictionary TemplateDict { get; set; }

        public Func<object, object> KeySelector { get; set; }

        private Func<object, string> _defaultKeySelector;

        #endregion

        #region Overriden members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (TemplateDict == null) throw new ArgumentException("TemplateDictionary can not be null");

            if (string.IsNullOrEmpty(Property)) throw new ArgumentException("Property value can not be null or empty");

            return new TemplateProvider(this);
        }

        #endregion

        #region Implementation

        public DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var key = KeySelector == null
                ? _defaultKeySelector.Invoke(item)
                : KeySelector.Invoke(item);

            if (TemplateDict.ContainsKey(key))
                return TemplateDict[key];

            return null;
        }

        class TemplateProvider : DataTemplateSelector
        {
            TemplateSelectorExtension templateSelector;

            public TemplateProvider(TemplateSelectorExtension extension)
                : base()
            {
                templateSelector = extension;
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                return templateSelector.SelectTemplate(item, container);
            }
        }

        #endregion
    }

    public class TemplateDictionary : Dictionary<object, DataTemplate>
    {
        #region Ctor

        public TemplateDictionary()
            : base()
        {
        }

        #endregion
    }
}