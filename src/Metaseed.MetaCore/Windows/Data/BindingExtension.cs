using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Data;
namespace Metaseed.Windows.Data
{
    public static class BindingExtensions
    {
        public static void UpdateTarget(
            this FrameworkElement element,
            DependencyProperty property)
        {
            BindingExpression expression = element.GetBindingExpression(property);
            if (expression != null)
            {
                expression.UpdateTarget();
            }
        }

        public static void UpdateSource(
            this FrameworkElement element,
            DependencyProperty property)
        {
            BindingExpression expression = element.GetBindingExpression(property);
            if (expression != null)
            {
                expression.UpdateSource();
            }
        }
    }
}
