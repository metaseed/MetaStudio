
using Metaseed.MetaShell.Services;
using Metaseed.Views;
namespace Metaseed.MetaShell.ViewModels
{
    public interface IToolViewModel : ILayoutContentViewModel, IContextUI
	{
		ToolPaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        double PreferredHeight { get; }
		bool IsVisible { get; set; }
	}
}