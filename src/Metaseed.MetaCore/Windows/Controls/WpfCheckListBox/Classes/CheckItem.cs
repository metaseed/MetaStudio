using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WpfCheckListBox
{
    public class CheckItem : INotifyPropertyChanged
    {
        #region ctors

        public CheckItem()
        {
            this.IsChecked = false;
        }

        public CheckItem(object KeyValue, string display)
        {
            this.IsChecked = false;
            this.KeyValue = KeyValue;
            this.Display = display;
        }
        #endregion

        #region Property - KeyValue

        public object KeyValue { get; set; }
        #endregion

        #region Property - Index

        public Int32 Index { get; set; }
        #endregion

        #region Property - IsChecked

        private bool isChecked = false;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    RaisePropertyChanged("IsChecked");
                }
            }
        }
        #endregion

        #region Property - Display

        public string Display { get; set; }
        #endregion

        #region INotifyPropertyChanged Members

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

}
