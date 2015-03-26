using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Reflection;
using System.Collections;


namespace WpfCheckListBox
{
    public abstract class CheckComboBoxBase : ComboBox, ICheckCtrlBase
    {
        #region members

        private bool isRadioMode;
        #endregion

        #region ctors

        public CheckComboBoxBase(bool isRadioMode) : base()
        {
            this.isRadioMode = isRadioMode;

            //hook up the DataContextChanged, which will allow the validation to 
            //work when a new bound object is seen
            this.DataContextChanged += OnDataContextChanged;
        }
        #endregion

        #region EventConsumer - DataContextChanged

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyInfo        _propInfo = null;
            BindingExpression   _bindExp  = this.GetBindingExpression(CheckComboBoxBase.CheckedValueProperty);

            if (_bindExp!=null && _bindExp.ParentBinding!=null && _bindExp.ParentBinding.Path!=null)
            {
                if (e.NewValue != null)
                {
                    _propInfo = e.NewValue.GetType().GetProperty(_bindExp.ParentBinding.Path.Path);
                }
                else
                {
                    if (_bindExp.DataItem != null)
                    {
                        _propInfo = _bindExp.DataItem.GetType().GetProperty(_bindExp.ParentBinding.Path.Path);
                    }
                }
                
                if (_propInfo != null)
                {
                    CreateViewModel(_propInfo.PropertyType);
                }
            }
        }
        #endregion

        #region Method - CreateViewModel

        public void CreateViewModel(Type boundType)
        {
            if (boundType==null)
            {
                return;
            }
            if (this.CheckViewModel == null || this.CheckViewModel.BoundType != boundType)
            {
                Type _viewBase = typeof(CheckViewModel<>);
                Type _viewType = _viewBase.MakeGenericType(boundType);

                this.CheckViewModel = (ICheckViewModel)Activator.CreateInstance(_viewType);
                this.CheckViewModel.HostParent  = (ICheckCtrlBase)this;
                this.CheckViewModel.IsRadioMode = this.isRadioMode;

                if (this.CheckListArray != null)
                {
                    List<CheckItem> _itemList = new List<CheckItem>();
                    foreach (CheckItem _checkItem in this.CheckListArray)
                    {
                        _itemList.Add(_checkItem);
                    }
                    this.CheckViewModel.CheckItems = _itemList;
                }
                else
                {
                    this.CheckViewModel.InitDiscovery();
                }
            }

            if (this.CheckViewModel != null && this.ItemsSource == null)
            {
                this.ItemsSource        = this.CheckViewModel.CheckItems;
                this.DisplayMemberPath  = "Display";
                this.SelectedValuePath  = "KeyValue";
                if (this.isRadioMode)
                {
                    //  radio
                    this.IsTextSearchEnabled = true;
                    this.IsEditable          = true;
                    this.IsReadOnly          = false;
                    this.IsSynchronizedWithCurrentItem = false;
                }
                else
                {
                    //  checkboxes
                    this.IsTextSearchEnabled = false;
                    this.IsEditable          = true;
                    this.IsReadOnly          = true;
                    this.IsSynchronizedWithCurrentItem = false;
                }
            }
        }
        #endregion


        #region Property - CheckViewModel

        public ICheckViewModel CheckViewModel { get; set; }

        #endregion

        #region Property - CheckListItems

        public ArrayList CheckListArray { get; set; }
        #endregion

        #region DP - CheckedValue

        //  this property is actually bound
        public static readonly DependencyProperty CheckedValueProperty = DependencyProperty.Register("CheckedValue", typeof(object), typeof(CheckComboBoxBase),
                                                                                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCheckedValueChanged)));
        public object CheckedValue
        {
            get { return (object)GetValue(CheckedValueProperty); }
            set {                SetValue(CheckedValueProperty, value); }
        }

        private static void OnCheckedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
            {
                return;
            }

            CheckComboBoxBase _ctrl = (CheckComboBoxBase)d;
            object           _value = (object)e.NewValue;

            if (e.NewValue != e.OldValue)
            {
                if (_ctrl.CheckViewModel == null && _value != null)
                {
                    _ctrl.CreateViewModel(_value.GetType());
                }

                if (_ctrl.CheckViewModel != null)
                {
                    _ctrl.CheckViewModel.ApplyValue(_value);
                }
            }
        }

        #endregion

        #region Method - SetDisplay

        public void SetDisplay(string displayText)
        {
            this.Text = displayText;
        }

        #endregion
    }
}
