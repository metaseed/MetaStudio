using System.Windows;
using System.Windows.Controls;
using Metaseed.MetaShell.Views;

namespace Metaseed.MetaShell.Controls
{
    using ViewModels;
	public class PanesStyleSelector : StyleSelector
	{
		public Style ToolStyle
		{
			get;
			set;
		}

		public Style DocumentStyle
		{
			get;
			set;
		}

		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is ToolBaseViewModel)
				return ToolStyle;

			if (item is DocumentBaseViewModel)
				return DocumentStyle;

			return base.SelectStyle(item, container);
		}
	}
}