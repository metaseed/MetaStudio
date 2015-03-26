using System;
using System.Xml.Linq;

namespace Metaseed.Data.Contracts
{
    public interface IDataID
    {
        Guid ID { get; }
        Guid ID_Ext { get; }
        bool IsID_ExtUsed { get; }
        XElement XML { get; set; }
    }

    [Serializable]
    public class DataID : IDataID
    {
        public static readonly DataID Empty = new DataID();
        Guid _id = Guid.Empty;
        public DataID()
        {

        }
        public DataID(string str)
        {
            _id = new Guid(str);
        }
        public DataID(Guid id)
        {
            _id = id;
        }
        virtual public Guid ID
        {
            get { return _id; }
            set { _id = value; }
        }
        virtual public Guid ID_Ext
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        virtual public bool IsID_ExtUsed { get { return false; } }
        virtual public XElement XML
        {
            get
            {
                var x=new XElement(this.GetType().Name,
                    new XElement("ID", _id.ToString())
                    );
                return x;
            }
            set
            {
                ID = new Guid(value.Element("ID").Value);
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var dataId = obj as IDataID;
            if (dataId == null)
            {
                return false;
            }
            if (IsID_ExtUsed != dataId.IsID_ExtUsed)
            {
                if (_id.Equals(Guid.Empty) && dataId.Equals(Guid.Empty))
                {
                    if (IsID_ExtUsed)
                    {
                        if (ID_Ext.Equals(Guid.Empty))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (dataId.ID_Ext.Equals(Guid.Empty))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            if (!IsID_ExtUsed)
            {
                return (_id.Equals(dataId.ID));
            }
            else
            {
                return (_id.Equals(dataId.ID)) && (ID_Ext.Equals(dataId.ID_Ext));
            }
        }
        public override int GetHashCode()
        {
            if (!IsID_ExtUsed)
            {
                return _id.GetHashCode();
            }
            return _id.GetHashCode() ^ ID_Ext.GetHashCode();
        }

        public override string ToString()
        {
            return ID.ToString();
        }
    }

    [Serializable]
    public class DataID_Ext : DataID
    {
        public static DataID_Ext Empty = new DataID_Ext();
        Guid _ID_Ext = Guid.Empty;//emyty id
        override public Guid ID_Ext
        {
            get { return _ID_Ext; }
            set { _ID_Ext = value; }
        }
        public DataID_Ext()
        {

        }
        public DataID_Ext(string id)
        {
            string[] stra = id.Split(new char[] { '+' });
            ID = new Guid(stra[0]);
            _ID_Ext = new Guid(stra[1]);
        }
        override public XElement XML
        {
            get
            {
                var x = base.XML;
                x.Add("ID_Ext", ID_Ext.ToString());
                return x;
            }
            set
            {
                var x = value;
                ID_Ext = new Guid(x.Element("ID_Ext").Value);
                base.XML = value;
            }
        }
        override public bool IsID_ExtUsed { get { return true; } }
        public override string ToString()
        {
            return ID.ToString() + "+" + ID_Ext.ToString();
        }
    }
}
