using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Metaseed.Data.MetaValueSync
{
    using Metaseed.Data.Contracts;
    public class MetaValueSyncPair : IDisposable,INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        /// <summary>
        /// The PropertyChanged event is used by consuming code
        /// (like WPF's binding infrastructure) to detect when
        /// a value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise the PropertyChanged event for the 
        /// specified property.
        /// </summary>
        /// <param name="propertyName">
        /// A string representing the name of 
        /// the property that changed.</param>
        /// <remarks>
        /// Only raise the event if the value of the property 
        /// has changed from its previous value</remarks>
        protected void RaisePropertyChanged(string propertyName)
        {
            // Validate the property name in debug builds
            VerifyProperty(propertyName);

            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Verifies whether the current class provides a property with a given
        /// name. This method is only invoked in debug builds, and results in
        /// a runtime exception if the <see cref="OnPropertyChanged"/> method
        /// is being invoked with an invalid property name. This may happen if
        /// a property's name was changed but not the parameter of the property's
        /// invocation of <see cref="OnPropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        private void VerifyProperty(string propertyName)
        {
            Type type = this.GetType();

            // Look for a *public* property with the specified name
            System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
            if (pi == null)
            {
                // There is no matching property - notify the developer
                string msg = "OnPropertyChanged was invoked with invalid " +
                                "property name {0}. {0} is not a public " +
                                "property of {1}.";
                msg = String.Format(msg, propertyName, type.FullName);
                System.Diagnostics.Debug.Fail(msg);
            }
        }

        #endregion

        public MetaValueSyncPair()
        {

        }
        public MetaValueSyncPair(IMetaData_ValueObject v1,IMetaData_ValueObject v2)
        {
            ValueObject1 = v1 as MetaData_ValueObject;
            ValueObject2 = v2 as MetaData_ValueObject;
        }
        MetaData_ValueObject _ValueObject1;
        public MetaData_ValueObject ValueObject1
        {
            get
            {
                return _ValueObject1;
            }
            set
            {
                if (ValueObject2 != null)
                {
                    //ValueObject2.ValueChanged -= ValueObject2_ValueChanged;
                    //ValueObject2.ValueChanged += ValueObject2_ValueChanged;
                    ValueObject2.ValueWrite_SyncPair -= ValueObject2_ValueWrite_SyncPair;
                    ValueObject2.ValueWrite_SyncPair += ValueObject2_ValueWrite_SyncPair;
                }
                if (_ValueObject1 != null)
                {
                   // _ValueObject1.ValueChanged -= ValueObject1_ValueChanged;
                    ValueObject1.ValueWrite_SyncPair -= ValueObject1_ValueWrite_SyncPair;
                }
                _ValueObject1 = value;
                if (_ValueObject1 != null)
                {
                    //_ValueObject1.ValueChanged -= ValueObject1_ValueChanged;
                    //_ValueObject1.ValueChanged += ValueObject1_ValueChanged;
                    ValueObject1.ValueWrite_SyncPair -= ValueObject1_ValueWrite_SyncPair;
                    ValueObject1.ValueWrite_SyncPair += ValueObject1_ValueWrite_SyncPair;
                }
                RaisePropertyChanged("ValueObject1");
            }
        }


        MetaData_ValueObject _ValueObject2;
        public MetaData_ValueObject ValueObject2
        {
            get
            {
                return _ValueObject2;
            }
            set
            {
                if (ValueObject1 != null)
                {
                    //ValueObject1.ValueChanged -= ValueObject1_ValueChanged;
                    //ValueObject1.ValueChanged += ValueObject1_ValueChanged;
                    ValueObject1.ValueWrite_SyncPair -= ValueObject1_ValueWrite_SyncPair;
                    ValueObject1.ValueWrite_SyncPair += ValueObject1_ValueWrite_SyncPair;
                }
                if (_ValueObject2 != null)
                {
                   // _ValueObject2.ValueChanged -= ValueObject2_ValueChanged;
                    ValueObject2.ValueWrite_SyncPair -= ValueObject2_ValueWrite_SyncPair;
                }
                _ValueObject2 = value;
                if (_ValueObject2 != null)
                {
                    //_ValueObject2.ValueChanged -= ValueObject2_ValueChanged;
                    //_ValueObject2.ValueChanged += ValueObject2_ValueChanged;
                    ValueObject2.ValueWrite_SyncPair -= ValueObject2_ValueWrite_SyncPair;
                    ValueObject2.ValueWrite_SyncPair += ValueObject2_ValueWrite_SyncPair;
                }
                RaisePropertyChanged("ValueObject2");
            }
        }
        void ValueObject2_ValueWrite_SyncPair(object sender, EventArgs<object, object> e)
        {
            if (ValueObject1 != null)
            {
                //ValueObject1.RaiseValueWrite_SyncPair(e.Item1, e.Item2);
                if (ValueObject1 != null)
                {
                    ValueObject1.WriteValue_SyncPair(ValueObject2.Value);
                }
            }
        }
        void ValueObject1_ValueWrite_SyncPair(object sender, EventArgs<object, object> e)
        {
            //if (ValueObject2 != null)
            //{
            //    ValueObject2.RaiseValueWrite_SyncPair(e.Item1, e.Item2);
            //}
            if (ValueObject2 != null)
            {
                ValueObject2.WriteValue_SyncPair( ValueObject1.Value);
            }
        }
        //void ValueObject1_ValueChanged(object sender, EventArgs<object, object> e)
        //{
        //    if (ValueObject2 != null)
        //    {
        //        ValueObject2.Value = ValueObject1.Value;
        //    }
        //}
        //void ValueObject2_ValueChanged(object sender, EventArgs<object, object> e)
        //{
        //    if (ValueObject1 != null)
        //    {
        //        ValueObject1.Value = ValueObject2.Value;
        //    }
        //}
        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_ValueObject1 != null)
                    {
                        _ValueObject1.ValueWrite_SyncPair -= ValueObject1_ValueWrite_SyncPair;
                        //_ValueObject1.ValueChanged -= ValueObject1_ValueChanged;
                        _ValueObject1 = null;
                    }
                    if (_ValueObject2 != null)
                    {
                        _ValueObject2.ValueWrite_SyncPair -= ValueObject2_ValueWrite_SyncPair;
                        //_ValueObject2.ValueChanged -= ValueObject2_ValueChanged;
                        _ValueObject2 = null;
                    }
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~MetaValueSyncPair()
        {
            Dispose(false);
        }
    }
    public class MetaValueSynchronizer
    {
        ObservableCollection<MetaValueSyncPair> _SyncPairs = new ObservableCollection<MetaValueSyncPair>();
        public ObservableCollection<MetaValueSyncPair> SyncPairs
        {
            get
            {
                return _SyncPairs;
            }
        }
        /// <summary>
        /// IEnumerable<IPortOut<IMetaData>>
        /// </summary>
        public IEnumerable<Object> Objects { get; set; }//source
        public void InsertSyncPairs(int index,MetaValueSyncPair syncPair)
        {
            SyncPairs.Insert(index,syncPair);
        }
        public void AddSyncPairs(MetaValueSyncPair syncPair)
        {
            SyncPairs.Add(syncPair);
        }
        public void RemoveSyncPairs(MetaValueSyncPair syncPair)
        {
            SyncPairs.Remove(syncPair);
            syncPair.Dispose();
        }
    }
}
