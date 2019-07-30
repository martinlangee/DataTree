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
using DataBase.Container;
using DataTree.Ui.Proxies;

namespace DataTree.Ui.ViewModels
{
    public class ContainerViewModel: NodeViewModel, INodeProxy
    {
        readonly DataContainer _cont;

        public ContainerViewModel(DataContainer container) : base()
        {
            _cont = container;


            Designation = _cont.Designation;

            foreach (var param in _cont.Params)
            {
                Nodes.Add(ParameterProxyFactory.CreateParam(param));
            }

            foreach (var child in _cont.Children)
            {
                Nodes.Add(new ContainerViewModel(child));
            }
        }

        public override string Value { get => ""; set => throw new InvalidOperationException("Set_Value"); }

        public override bool IsModified => _cont.IsModified;

        public override NodeType NodeType => NodeType.Container;

        public override string ValueType => null;
    }
}
