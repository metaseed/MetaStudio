using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catel;

namespace Metaseed.Data
{
    public class DataDirtyManager : IDataDirty
    {
        List<WeakReference> DataObjects = new List<WeakReference>();
        public void AddDataObject(IDataDirty dataObj)
        {
            DataObjects.Add(new WeakReference(dataObj));
            this.SubscribeToWeakGenericEvent<DataDirtyEventArgs>(dataObj, "IsDataDirtyChangedEvent", IsDataDirtyChangedEventHandler);
        }
        void IsDataDirtyChangedEventHandler(object sender, DataDirtyEventArgs args)
        {
            foreach (var dataObj in DataObjects)
            {
                if (dataObj.Target!=null)
                {
                    if ((dataObj.Target as IDataDirty).IsDataDirty)
                    {
                        IsDataDirty = true;
                        return;
                    }

                }
                else
                {
                    DataObjects.Remove(dataObj);
                }
            }
            IsDataDirty = false;
        }

        public event EventHandler<DataDirtyEventArgs> IsDataDirtyChangedEvent;
        bool _IsDirty;
        public bool IsDataDirty
        {
            get { return _IsDirty; }
            private set
            {
                if ( _IsDirty == value)
                {
                    return;
                }
                _IsDirty = value;

                if (IsDataDirtyChangedEvent != null)
                {
                    IsDataDirtyChangedEvent(this, new DataDirtyEventArgs(value));
                }
            }
        }
        public void ClearIsDataDirty()
        {
            foreach (var dataObj in DataObjects)
            {
                if (dataObj.Target != null)
                {
                    if ((dataObj.Target as IDataDirty).IsDataDirty)
                    {
                        IsDataDirty = false;
                    }
                }
                else
                {
                    DataObjects.Remove(dataObj);
                }
            }
        }
    }
}
