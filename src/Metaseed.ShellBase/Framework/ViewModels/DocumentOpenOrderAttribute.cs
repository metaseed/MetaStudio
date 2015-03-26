using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.MetaShell.ViewModels
{
    /// <summary>
    /// could be put on DocumentViewModel or derived class of DocumentViewModel
    /// FunctionBlockDesignerDocumentViewModel =-8
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class DocumentOpenOrderAttribute : Attribute
    {
        readonly int _OrderValue;
        public DocumentOpenOrderAttribute(int orderValue)
        {
            this._OrderValue = orderValue;
        }
        public int Value { get { return _OrderValue; } }
    }
}
