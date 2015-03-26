using System;
using System.Collections.Generic;
using System.Collections;

namespace Metaseed.Data
{

    public interface IValueObject : INameDescription
    {
        string Unit { get; }
        object Value { get; set; }
        event EventHandler<EventArgs<object, object>> ValueChanged;
        event EventHandler<EventArgs<object, object>> ValueWrite;
    }
    public interface IValueObject<out T> : IValueObject
    {
       new T Value { get; }
    }
  
}
