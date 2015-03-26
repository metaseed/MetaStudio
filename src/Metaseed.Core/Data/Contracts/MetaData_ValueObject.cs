using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Metaseed.Data.Contracts
{
    using Metaseed.Data;
    using Properties;
    public class MetaData_ValueObject : ValueObject, IMetaData_ValueObject
    {
        public static MetaData_ValueObject Empty;
        static MetaData_ValueObject()
        {
            Empty = new MetaData_ValueObject()
        {
            NameText = Resources.EmptyValueObjectName,
            Description = Resources.EmptyValueObjectDescription,

        };
            Empty._DataID = Contracts.DataID.Empty;//same as MetaData.Empty.DataID
        }
        public MetaData_ValueObject()
        {
            //LocalizeDictionary.Instance.PropertyChanged += Instance_PropertyChanged;
        }
        override public XElement XML
        {
            get
            {
                var x = base.XML;
                x.Add(new XElement("DataID", DataID.XML));
                return x;
            }
            set
            {
                var sigXml = value;
                var x = sigXml.Element("DataID").Elements().First();
                DataID.XML = x;
                IsDataIDFixed = true;
                base.XML = value;
            }
        }
        /// <summary>
        /// called to encode some feature bits into the DataID field.
        /// note: you should also add some random bits, in order for more than one same feature objects with different DataID
        /// </summary>
        public event Action<string> SetDataID;
        /// <summary>
        /// called to encode some feature bits into the DataID field.
        /// note: you should also add some random bits, in order for more than one same feature objects with different DataID
        /// </summary>
        protected virtual void OnSetDataID(string propertyName)
        {
            if (!IsDataIDFixed)
            {
                if (SetDataID != null)
                {
                    SetDataID(propertyName);
                }
            }
        }
        protected bool IsDataIDFixed;

        //void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName.Equals("Culture"))
        //    {
        //        NameText = Resources.EmptyValueObjectName;
        //        Description = Resources.EmptyValueObjectDescription;
        //    }
        //}
        protected DataID _DataID = new DataID() { ID = Guid.NewGuid() };
        virtual public IDataID DataID
        {
            get { return _DataID; }
            set
            {
                var id = value as DataID;
                if (id!=null)
                {
                    _DataID = id;
                }
                else
                {
                    _DataID = new DataID(id.ID);
                }
                
            }
        }
        virtual public IEnumerable<IMetaData_ValueObject> Children { get { return null; } }
        public bool IsNullOrEmpty()
        {
            return MetaData.IsNullOrEmpty(this);
        }
        virtual public IMetaData GetPortDataByID(IDataID idToSearch)
        {
            if (idToSearch.Equals(MetaData.Empty.DataID))
            {
                return MetaData_ValueObject.Empty;
            }
            if (this.DataID.Equals(idToSearch))
            {
                return this;
            }
            if (this.Children == null)
            {
                return MetaData_ValueObject.Empty;
            }
            foreach (IMetaData_ValueObject value in this.Children)
            {
                if (!value.GetPortDataByID(idToSearch).Equals(MetaData_ValueObject.Empty))
                {
                    return value;
                }
            }
            return MetaData_ValueObject.Empty;
        }
    }
    public class MetaData_ValueObject<T> : MetaData_ValueObject, IValueObject<T>
    {
        T IValueObject<T>.Value
        {
            get { return (T)Value; }
        }
    }
}
