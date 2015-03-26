using System;
namespace Metaseed.MetaShell.Services
{
    using Infrastructure;
	public enum ToolPaneLocation
	{
		Left,
		Right,
		Bottom
	}
//    public static class ToolPaneLocationExtension{
//       static public ToolPaneLocation GetToolPaneLocation(this string toolPanelName){
//            if (toolPanelName.Equals(RegionNames.DocumentsRegion))
//            {
//                throw new ArgumentOutOfRangeException(toolPanelName, "Please using a tool panel name, not the document panel name.");
//            }
//            else if (toolPanelName.Equals(RegionNames.ToolboxLeftRegion))
//            {
//                return ToolPaneLocation.Left;
//            }
//            else if (toolPanelName.Equals(RegionNames.ToolboxRightRegion))
//            {
//                return ToolPaneLocation.Right;
//            }
//            else if (toolPanelName.Equals(RegionNames.ToolboxBottomRegion))
//            {
//                return ToolPaneLocation.Bottom;
//            }
//            else
//            {
//                throw new ArgumentOutOfRangeException(toolPanelName, "Please using a valid tool panel name.");
//            }
//        }

//}
}