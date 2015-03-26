using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WpfCheckListBox
{
    public class CheckViewModel<T> : ICheckViewModel
    {
        #region members

        private Type        typeT       = typeof(T);
        private Type        typeV       = typeof(T);
        private bool[]      boolFlags;
        private UInt64[]    intValues;
        private int         itemCount;
        private bool        isNullable  = false;

        #endregion

        #region ctors

        public CheckViewModel()
        {
        }
        #endregion

        #region Method - InitDiscovery

        public void InitDiscovery()
        {
            #region Enum

            if (typeT.IsEnum)
            {
                this.typeV = Enum.GetUnderlyingType(typeT);
                this.IsRadioMode = ((FlagsAttribute[])typeT.GetCustomAttributes(typeof(FlagsAttribute), true)).Length == 0;

                //  directly fill the CheckItems
                List<CheckItem> _itemList = new List<CheckItem>();

                Array _enumValues = Enum.GetValues(typeT);

                if (_enumValues != null && _enumValues.Length > 0)
                {
                    T _enumItem = (T)_enumValues.GetValue(0);

                    for (int i = 0; i < _enumValues.Length; i++)
                    {
                        _enumItem = (T)_enumValues.GetValue(i);
                        if (!this.IsRadioMode)
                        {
                            //  check here for None, leave out
                            if (i == 0 && Convert.ToUInt64(_enumItem) == 0)
                            {
                                continue;
                            }
                        }
                        _itemList.Add(new CheckItem((object)_enumItem, _enumItem.ToString()));
                    }
                }
                this.CheckItems = _itemList;
            }
            #endregion

            #region bool[]

            if (typeof(bool[]).IsAssignableFrom(typeT))
            {

            }
            #endregion
        }
        #endregion

        #region Property - CheckItems

        private List<CheckItem> checkItems = null;

        public List<CheckItem> CheckItems
        {
            get
            {
                return this.checkItems;
            }
            set
            {
                //  first clean up existing
                if (this.itemCount > 0 && this.checkItems != null && this.checkItems.Count > 0)
                {
                    for (int i = 0; i < this.itemCount; i++)
                    {
                        this.CheckItems[i].PropertyChanged -= Item_PropertyChanged;
                    }
                    this.checkItems.Clear();
                    this.checkItems = null;
                    this.itemCount  = 0;
                    this.boolFlags  = null;
                    this.intValues  = null;
                }
                this.checkItems = value;

                this.itemCount = this.CheckItems.Count;
                this.boolFlags = new bool[this.itemCount];
                this.intValues = new UInt64[this.itemCount];

                for (int i = 0; i < this.itemCount; i++)
                {
                    this.boolFlags[i] = false;
                    this.intValues[i] = Convert.ToUInt64(this.CheckItems[i].KeyValue);

                    this.CheckItems[i].Index = i;
                    this.CheckItems[i].PropertyChanged += Item_PropertyChanged;
                }
            }
        }
        #endregion

        #region Property - BoundType

        public Type BoundType
        {
            get { return typeT; }
        }
        #endregion

        #region Property - HostParent

        public ICheckCtrlBase HostParent { get; set; }

        #endregion

        #region Property - IsRadioMode
        private bool isRadioMode = false;

        public bool IsRadioMode
        {
            get { return this.isRadioMode; }
            set { if (!this.isRadioMode) this.isRadioMode = value; }
        }
        #endregion

        #region EventConsumer - PropertyChange

        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.CheckItems == null)
            {
                throw new Exception();
            }

            CheckItem _item = (sender as CheckItem);
            Int32 _index = _item.Index;

            if (this.itemCount == 0 || this.boolFlags[_index] == _item.IsChecked)
            {
                return;
            }

            int i = 0;
            int count = this.CheckItems.Count;

            if (this.IsRadioMode)
            {
                //  manually deselect
                for (i = 0; i < count; i++)
                {
                    this.boolFlags[i] = false;
                }
            }

            this.boolFlags[_index] = _item.IsChecked;

            if (typeT.IsEnum || typeT.IsValueType)
            {
                UInt64 _intValue = 0;

                if (this.IsRadioMode)
                {
                    _intValue = this.intValues[_index];
                }
                else
                {
                    for (i = 0; i < count; i++)
                    {
                        if (this.boolFlags[i])
                        {
                            _intValue = _intValue | this.intValues[i];
                        }
                    }
                }
                if (this.HostParent != null)
                {
                    this.HostParent.CheckedValue = (object)UnboxIntValue(_intValue);
                }
            }

            if (typeof(bool[]).IsAssignableFrom(typeT))
            {
                if (this.HostParent != null)
                {
                    // important: clone, otherwise same ref (and no update)
                    this.HostParent.CheckedValue = this.boolFlags.Clone();
                }
            }
            this.SetDisplay();
        }
        #endregion

        #region Method - SetDisplay

        private void SetDisplay()
        {
            int i = 0;
            int count = this.CheckItems.Count;

            if (this.HostParent != null)
            {
                StringBuilder _display = new StringBuilder();

                for (i = 0; i < count; i++)
                {
                    if (this.boolFlags[i])
                    {
                        _display.Append(this.CheckItems[i].Display+" ");
                    }
                }
                this.HostParent.SetDisplay(_display.ToString());
            }
        }

        #endregion

        #region Method - ApplyValue

        public void ApplyValue(object newValue)
        {
            int i = 0;
            int count = this.CheckItems.Count;

            if (newValue==null)
            {
                for (i = 0; i < count; i++)
                {
                    this.boolFlags[i] = false;
                }
            }
            else
            {
                if (typeT.IsEnum || typeT.IsValueType)
                {
                    UInt64 _newValue = Convert.ToUInt64(newValue);

                    if (this.IsRadioMode)
                    {
                        for (i = 0; i < count; i++)
                        {
                            this.boolFlags[i] = (this.intValues[i] == _newValue);
                        }
                    }
                    else
                    {
                        for (i = 0; i < count; i++)
                        {
                            this.boolFlags[i] = (_newValue & this.intValues[i]) != 0;
                        }
                    }
                }

                if (typeof(bool[]).IsAssignableFrom(typeT))
                {
                    bool[] _newValue = newValue as bool[];
                    int _count = _newValue.Length;

                    for (i = 0; i < count; i++)
                    {
                        this.boolFlags[i] = _newValue[i];
                    }
                }
            }

            for (i = 0; i < count; i++)
            {
                if (this.CheckItems[i].IsChecked != this.boolFlags[i])
                {
                    this.CheckItems[i].IsChecked = this.boolFlags[i];
                }
            }
            this.SetDisplay();
        }
        #endregion

        #region Method - BoxEnumValue

        private object BoxEnumValue(T enumValue)
        {
            UInt64 _result = Convert.ToUInt64(enumValue);
            return (object)Convert.ChangeType(_result, typeV);
        }
        #endregion

        #region Method - UnboxIntValue

        private T UnboxIntValue(UInt64 intValue)
        {
            object _result = (object)Convert.ChangeType(intValue, typeV);
            return (T)_result;
        }
        #endregion
    }

}
