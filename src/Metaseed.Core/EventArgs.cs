using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//http://codereview.stackexchange.com/questions/5470/generic-eventargs-to-go-with-generic-eventhandler
namespace Metaseed
{/*
  public event EventHandler<ReadOnlyEventArgs<Tuple<string,double>>> SelectChanged;
  public void SomeFormMethod()
{
   var handlers = SomeTupleEventHandler;
   if(handlers == null)
   {
      //this saves us the trouble of closing the generic.
      var args = SomeTupleEventHandler.CreateReadOnlyArgs(txtInput1.Text, numUpDownInput1.Value);

      handlers(this, args);
   }   
}
  */
    public class ReadOnlyEventArgs<T> : EventArgs
    {
        public T Parameter { get; private set; }

        public ReadOnlyEventArgs(T input)
        {
            Parameter = input;
        }
    }

    public class EventArgs<T> : EventArgs
    {
        public T Parameter { get; set; }

        public EventArgs(T input)
        {
            Parameter = input;
        }
    }
    public class EventArgs<T1,T2> : EventArgs
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public EventArgs(T1 t1,T2 t2)
        {
            Item1 = t1;
            Item2= t2;
        }
    }
    public class EventArgs<T1, T2,T3> : EventArgs
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public EventArgs(T1 t1, T2 t2, T3 t3)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
        }
    }
    public static class EventHandlerExtensions
    {
        public static EventArgs<T> CreateArgs<T>(this EventHandler<EventArgs<T>> handler, T input)
        {
            return new EventArgs<T>(input);
        }

        public static ReadOnlyEventArgs<T> CreateReadOnlyArgs<T>(this EventHandler<ReadOnlyEventArgs<T>> handler, T input)
        {
            return new ReadOnlyEventArgs<T>(input);
        }

        public static ReadOnlyEventArgs<Tuple<T1, T2>> CreateReadOnlyArgs<T1, T2>(this EventHandler<ReadOnlyEventArgs<Tuple<T1, T2>>> handler, T1 input1, T2 input2)
        {
            return new ReadOnlyEventArgs<Tuple<T1, T2>>(Tuple.Create(input1, input2));
        }

        public static ReadOnlyEventArgs<Tuple<T1, T2, T3>> CreateReadOnlyArgs<T1, T2, T3>(this EventHandler<ReadOnlyEventArgs<Tuple<T1, T2, T3>>> handler, T1 input1, T2 input2, T3 input3)
        {
            return new ReadOnlyEventArgs<Tuple<T1, T2, T3>>(Tuple.Create(input1, input2, input3));
        }

        //etc up to 8 input parameters cause that's all the overloads Tuple.Create() has
    }
}
