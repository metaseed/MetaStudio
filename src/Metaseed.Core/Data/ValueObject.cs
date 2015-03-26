using System;
using System.Xml.Linq;

namespace Metaseed.Data
{
    public class ValueObject : NameDescription_INotifyPropertyChanged, IValueObject
    {
        public event EventHandler<EventArgs<object, object>> ValueChanged;
        public event EventHandler<EventArgs<object, object>> ValueWrite;
        override public XElement XML
        {
            get
            {
                var x = base.XML;
                x.Add(new XElement("Unit", Unit));
                return x;
            }
            set
            {
                base.XML = value;
                var sigXml = value;
                Unit = sigXml.Element("Unit").Value;
                
            }
        }
        internal event EventHandler<EventArgs<object, object>> ValueWrite_SyncPair;
        protected object _Value = null;
        virtual public object Value
        {
            get { return _Value; }
            set
            {
                WriteValue_SyncPair(value);
                if (ValueWrite_SyncPair != null)
                {
                    ValueWrite_SyncPair(this, new EventArgs<object, object>(_Value, value));
                }
            }
        }

        internal void WriteValue_SyncPair(object value)
        {
            if (value == null)
            {
                if (_Value == null)
                {
                    if (ValueWrite != null)
                    {
                        ValueWrite(this, new EventArgs<object, object>(_Value, value));
                    }
                    
                    return;
                }
            }
            else
            {
                if (value.Equals(_Value))
                {
                    if (ValueWrite != null)
                    {
                        ValueWrite(this, new EventArgs<object, object>(_Value, value));
                    }
                    
                    return;
                }
            }
            var oldValue = _Value;
            _Value = value;
            OnValueChanged(oldValue, value);
            RaiseValueWriteAndChanged(value, oldValue);
            RaisePropertyChanged("Value");
        }

        public void RaiseValueWriteAndChanged(object value, object oldValue=null)
        {
            if (oldValue==null)
            {
                oldValue = _Value;
            }
            var valuechaged = ValueChanged;
            if (valuechaged != null)
            {
                valuechaged(this, new EventArgs<object, object>(oldValue, value));
            }
            var valueWrite = ValueWrite;
            if (valueWrite != null)
            {
                valueWrite(this, new EventArgs<object, object>(oldValue, value));
            }
            
        }

        
        protected virtual void OnValueChanged(object oldValue, object newValue)
        {

        }
        protected string _Unit = string.Empty;
        virtual public string Unit
        {
            get { return _Unit; }
            set
            {
                if (!_Unit.Equals(value))
                {
                    _Unit = value;
                    RaisePropertyChanged("Unit");
                }
            }
        }


        public override string ToString()
        {
            if (string.IsNullOrEmpty(Description))
            {
                return NameText;
            }
            else
            {
                if (string.IsNullOrEmpty(Unit))
                {
                    return NameText + "-" + Description;
                }
                return NameText + "(" + Unit + ")-" + Description;
            }
        }
    }
    public class ValueObject<T> : ValueObject, IValueObject<T>
    {
        T IValueObject<T>.Value
        {
            get { return (T)Value; }
        }

    }
}
