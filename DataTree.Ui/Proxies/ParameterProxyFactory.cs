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

using DataBase.Parameters;
using System;

namespace DataTree.Ui.Proxies
{
    internal static class ParameterProxyFactory
    {
        internal static INodeProxy CreateParam(DataParameterBase param)
        {
            if (param is BoolParameter)
                return new ParameterProxy<bool>((BoolParameter) param);

            if (param is FloatParameter)
                return new FloatParameterProxy((FloatParameter) param);

            if (param is ChoiceParameter)
                return new ChoiceParameterProxy((ChoiceParameter) param);

            if (param is IntParameter)
                return new ParameterProxy<int>((IntParameter) param);

            if (param is StringParameter)
                return new ParameterProxy<string>((StringParameter) param);

            if (param is BinaryParameter)
                return new ParameterProxy<byte[]>((BinaryParameter) param);

            throw new ArgumentOutOfRangeException("ParameterProxyFactory.CreateParam: Parameter type not recognized");
        }
    }
}
