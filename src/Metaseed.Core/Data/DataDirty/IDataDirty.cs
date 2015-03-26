using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.Data
{
    public class DataDirtyEventArgs:EventArgs
    {
        public DataDirtyEventArgs(bool isDirty)
        {
            IsDataDirty = isDirty;
        }
        public bool IsDataDirty;
    }
    public interface IDataDirty
    {
        event EventHandler<DataDirtyEventArgs> IsDataDirtyChangedEvent;
        bool IsDataDirty { get; }
    }
    public class DataDirtyObject:IDataDirty
    {
        public event EventHandler<DataDirtyEventArgs> IsDataDirtyChangedEvent;
        bool _IsDirty;
        public bool IsDataDirty
        {
            get { return _IsDirty; }
            set
            {
                if (_IsDirty == value)
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
    }
}
