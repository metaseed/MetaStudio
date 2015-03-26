using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Input;
using System.Windows.Data;

namespace Metaseed.Windows.Interactivity
{
    //usage: textBox..... a control could access key
    //xmlns:ia="clr-namespace:Metaseed.Windows.Interactivity;assembly=Metaseed.MetaCore"  
    //<TextBox Name="itemNameTextBox" Text="{Binding Path=ItemName}" c:EnterKeyToUpdateInput.UpdatePropertySourceWhenEnterPressed="TextBox.Text"/>

    public static class EnterKeyToUpdateInput
    {
        public static readonly DependencyProperty UpdatePropertySourceWhenEnterPressed = DependencyProperty.RegisterAttached(
            "UpdatePropertySourceWhenEnterPressed", typeof(DependencyProperty), typeof(EnterKeyToUpdateInput), new PropertyMetadata(null, OnUpdatePropertySourceWhenEnterPressedPropertyChanged));

        public static void SetUpdatePropertySourceWhenEnterPressed(DependencyObject dp, DependencyProperty value)
        {
            dp.SetValue(UpdatePropertySourceWhenEnterPressed, value);
        }

        public static DependencyProperty GetUpdatePropertySourceWhenEnterPressed(DependencyObject dp)
        {
            return (DependencyProperty)dp.GetValue(UpdatePropertySourceWhenEnterPressed);
        }

        private static void OnUpdatePropertySourceWhenEnterPressedPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = dp as UIElement;
            if (element == null)
            {
                return;
            }
            if (e.OldValue != null)
            {
                element.PreviewKeyDown -= HandlePreviewKeyDown;
            }
            if (e.NewValue != null)
            {
                element.PreviewKeyDown += new KeyEventHandler(HandlePreviewKeyDown);
            }
        }

        static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoUpdateSource(e.Source);
            }
        }

        static void DoUpdateSource(object source)
        {
            DependencyProperty property = GetUpdatePropertySourceWhenEnterPressed(source as DependencyObject);
            if (property == null)
            {
                return;
            }
            UIElement elt = source as UIElement;
            if (elt == null)
            {
                return;
            }
            BindingExpression binding = BindingOperations.GetBindingExpression(elt, property);
            if (binding != null)
            {
                binding.UpdateSource();
            }
        }
    }
}
