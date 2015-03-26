using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Metaseed.Data.Contracts
{
    using Properties;
    using Data;

    public interface IMetaData : INameDescription
    {
        /// <summary>
        /// the id used to identify the object. 
        /// Note: when serialize and deserialize, make this the same value
        /// </summary>
        IDataID DataID { get; }
        IMetaData GetPortDataByID(IDataID idToSearch);
    }

    public class MetaData : NameDescription_INotifyPropertyChanged, IMetaData
    {
        public static MetaData Empty = new MetaData()
        {
            NameText = Resources.EmptyValueObjectName,
            Description = Resources.EmptyValueObjectDescription
        };
        public static bool IsNullOrEmpty(IMetaData metaData){
            if (metaData==null)
            {
                return true;
            }
            if (metaData==Empty)
            {
                return true;
            }
            if (metaData.DataID.Equals(MetaData.Empty.DataID))
            {
                return true;
            }
            return false;
        }
        public MetaData()
        {
            //LocalizeDictionary.Instance.PropertyChanged += Instance_PropertyChanged;
        }
        //void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName.Equals("Culture"))
        //    {
        //        NameText = Resources.EmptyValueObjectName;
        //        Description = Resources.EmptyValueObjectDescription;
        //    }
        //}
        IDataID _PortDataID = Contracts.DataID.Empty;
        virtual public IDataID DataID { get { return _PortDataID; } set { _PortDataID = value; } }
        public virtual IMetaData GetPortDataByID(IDataID idToSearch)
        {
            if (this.DataID.Equals(idToSearch))
            {
                return this;
            }
            return MetaData.Empty;
        }
    }

    public class MetaDataCollection_NotUsed<T> : ObservableCollection<T>, INotifyPropertyChanged ,IMetaData where T : IMetaData
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
        IDataID _PortDataID = Contracts.DataID.Empty;
        virtual public IDataID DataID { get { return _PortDataID; } set { _PortDataID = value; } }
        public virtual IMetaData GetPortDataByID(IDataID idToSearch)
        {
            if (this.DataID.Equals(idToSearch))
            {
                return this;
            }
            return MetaData.Empty;
        }

        private string _NameText;
        public string NameText
        {
            get { return _NameText; }
            set
            {
                if (value != _NameText)
                {
                    _NameText = value;
                    RaisePropertyChanged("NameText");
                }
            }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }


        
    }


}
