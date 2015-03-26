using System;
using System.Windows;
using System.ComponentModel;

namespace Metaseed.Data
{
    public class SignalObjectViewModel:INotifyPropertyChanged
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
        public SignalObjectViewModel()
        {
        }
        private SignalObject _Signal;
        public SignalObject Signal
        {
            get { return _Signal; }
            set
            {
                if (value != _Signal)
                {
                    if (value != null)
                    {
                        value.SetDataID += value_SetDataID;

                    }
                    if (_Signal != null)
                    {
                        _Signal.SetDataID -= value_SetDataID;
                    }
                    _Signal = value;
                    if (_Signal != null)
                    {
                        if (_Signal.IsUsingLowHighLimit)
                        {
                            _Min = (Signal.GetRealRawValue(Signal.RawValueLowLimit)) * Signal.Factor + Signal.Offset;
                            RaisePropertyChanged("Min");
                            _Max = (Signal.GetRealRawValue(Signal.RawValueHighLimit)) * Signal.Factor + Signal.Offset;
                            RaisePropertyChanged("Max");
                        }
                        _DefauleValue = (Signal.GetRealRawValue(Signal.RawDefaultValue)) * Signal.Factor + Signal.Offset;
                        RaisePropertyChanged("DefaultValue");
                    }
                    RaisePropertyChanged("Signal");
                }
            }
        }

        void value_SetDataID(string propertyName)
        {
            if (Signal.IsUsingLowHighLimit)
            {
                updateLowLimtValue();
                udpateHighLimtValue();
            }
            udpateDefaultValue();
        }



        bool updateLowLimtValue()
        {
            if (Signal.IsUsingLowHighLimit)
            {
                try
                {
                    bool isOutRange = false;
                    Signal.RawValueLowLimit = Signal.GetInRangeRawValue(Min,out isOutRange,false); 
                }
                catch (Exception)
                {
                    MessageBox.Show("Please Input A Valid Value For Min Value");
                    return false;
                }

            }
            return true;
        }

        bool udpateHighLimtValue()
        {
            if (Signal.IsUsingLowHighLimit)
            {
                try
                {
                    bool isOutRange = false;
                    Signal.RawValueHighLimit = Signal.GetInRangeRawValue(Max,out isOutRange,false);
                }
                catch (Exception)
                {
                    MessageBox.Show("Please Input A Valid Value For Max Value");
                    return false;
                }

            }
            return true;
        }
        bool udpateDefaultValue()
        {
            try
            {
                bool isOutRange = false;
                Signal.RawDefaultValue = Signal.GetInRangeRawValue(DefaultValue,out isOutRange,true); 
            }
            catch (Exception)
            {
                MessageBox.Show("Please Input A Valid Value For DefaultValue");
                return false;
            }
            return true;
        }
        private double _Max;
        public double Max
        {
            get { return _Max; }
            set
            {
                if (value != _Max)
                {
                    var r = udpateHighLimtValue();
                    if (r)
                    {
                        _Max = value;
                        RaisePropertyChanged("Max");
                    }

                }
            }
        }

        
        private double _Min;
        public double Min
        {
            get { return _Min; }
            set
            {
                if (value != _Min)
                {
                    var r = updateLowLimtValue();
                    if (r)
                    {
                        _Min = value;
                        RaisePropertyChanged("Min");
                    }
                }
            }
        }

        private double _DefauleValue;
        public double DefaultValue
        {
            get { return _DefauleValue; }
            set
            {
                if (value != _DefauleValue)
                {
                    var r = udpateDefaultValue();
                    if (r)
                    {
                        _DefauleValue = value;
                        RaisePropertyChanged("DefaultValue");
                    }

                }
            }
        }
    }
}
