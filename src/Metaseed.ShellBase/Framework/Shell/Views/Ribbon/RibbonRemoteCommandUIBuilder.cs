using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catel;
using Catel.IoC;
using Fluent;
using Metaseed.MetaShell.Services;
using Metaseed.MVVM.Commands;
using Metaseed.ShellBase.Views;

namespace Metaseed.ShellBase.Framework.Shell.Views
{
    public enum RibbonUIType { Button, CheckBox}
    /// <summary>
    /// contract of UIData.ExtraData:
    /// </summary>
    internal class RibbonRemoteCommandUIBuilder : IRemoteCommandUIBuilder
    {
        protected IShellService ShellService;
        List<CompositeRemoteCommand> notAddedCommands=new List<CompositeRemoteCommand>();

        private Ribbon Ribbon
        {
            get { return ((RibbonService) (ShellService.Ribbon)).Ribbon; }
        }

        public RibbonRemoteCommandUIBuilder()
        {
            ShellService = ServiceLocator.Default.ResolveType<IShellService>();
            ShellService.Ribbon.RibbonTabAdded += Ribbon_RibbonTabAdded;
        }
        virtual public void GenerateUI(CompositeRemoteCommand command)
        {
            //deserialize extraUIData
            Argument.IsNotNull(() => command.UIData);
            Argument.IsNotNull(()=>command.UIData.ExtraData);
            var ribbonExtraData= RibbonRemoteCommandUI_ExtrUIData.Deserialize(command.UIData.ExtraData);
            //find ribbon tab
            var ribbonTabData = ribbonExtraData.RibbonTab;
            Argument.IsNotNull(() => ribbonTabData.Name);
            var tab = Ribbon.Tabs.FirstOrDefault(ribbonTab=>ribbonTab.Name.Equals(ribbonTabData.Name));
            if (tab == null)
            {
                if (ribbonTabData.CreatNewIfCanNotFound)
                {
                    //ToDo: creat new, consider contextual tab?
                    throw new Exception("ToDo: creat new, consider contextual tab?");
                }
                else
                {
                    //ToDo: add logic to delay the remote command add.
                    throw new Exception("ToDo: add logic to delay the remote command add.");
                }
            }
            else
            {
                
            }
            //ribbon tab group
            var ribbonGroupData = ribbonExtraData.RibbonTabGroup;
            if (!string.IsNullOrEmpty(ribbonGroupData.Name))
            {
                //find or creat
                var tabGroup = Ribbon.ContextualGroups.FirstOrDefault(ribbonGroup =>  ribbonGroup.Name.Equals(ribbonGroupData.Name));
                if (tabGroup == null)
                {
                    var conv = new BrushConverter();
                    tabGroup = new RibbonContextualTabGroup()
                    {
                        BorderBrush =string.IsNullOrEmpty(ribbonGroupData.BorderBrush)? Brushes.Purple: conv.ConvertFromString(ribbonGroupData.BorderBrush) as SolidColorBrush,
                        Background = string.IsNullOrEmpty(ribbonGroupData.BackgroundBrush)? Brushes.Green: conv.ConvertFromString(ribbonGroupData.BackgroundBrush) as SolidColorBrush
                    };
                    if (string.IsNullOrEmpty(ribbonGroupData.HeaderLocalizedKey))
                    {
                        tabGroup.Header = ribbonGroupData.Name;
                    }
                    else
                    {
                        tabGroup.BindToLoc(RibbonContextualTabGroup.HeaderProperty,ribbonGroupData.HeaderLocalizedKey));
                    }
                    ShellService.Ribbon.AddRibbonContextualTabGroup(tabGroup);
                }
            }
            var uiType = (RibbonUIType)Enum.Parse(typeof(RibbonUIType),command.UIData.UIType, true);
            switch (uiType)
            {
                case RibbonUIType.Button:
                {
                    var button = new Fluent.Button();
                    button.SetValue(KeyTip.KeysProperty, "ND");
                    button.BindToLoc(Fluent.Button.HeaderProperty,"☯Metaseed.Modules.FunctionBlock:FunctionBlockResource:CreatNewDoc");
                    button.Icon =new BitmapImage(new Uri(
                            "/☯Metaseed.Modules.FunctionBlock;component/Resources/Images/NewDoc.png", UriKind.Relative));
                    button.LargeIcon =
                        new BitmapImage(new Uri(
                            "/☯Metaseed.Modules.FunctionBlock;component/Resources/Images/NewDoc.png", UriKind.Relative));
                    button.SetValue(Button.SizeProperty, RibbonControlSize.Large);
                    Binding binding = new Binding("DataContext.NewDocCommand");
                    binding.Source = this;
                    button.SetBinding(ButtonBase.CommandProperty, binding);
                    var toolTip = new Fluent.ScreenTip();
                    toolTip.BindToLoc(ScreenTip.TitleProperty,
                        "☯Metaseed.Modules.FunctionBlock:FunctionBlockResource:CreatNewDoc");
                    toolTip.BindToLoc(ScreenTip.TextProperty,
                        "☯Metaseed.Modules.FunctionBlock:FunctionBlockResource:CreatNewDoc_Des");
                    toolTip.Image =
                        new System.Windows.Media.Imaging.BitmapImage(
                            new Uri("/☯Metaseed.Modules.FunctionBlock;component/Resources/Images/NewDoc.png",
                                UriKind.Relative));
                    toolTip.HelpTopic = "FunctionBlock_CreatNewDoc";
                    toolTip.BindToLoc(ScreenTip.DisableReasonProperty,
                        "☯Metaseed.Modules.FunctionBlock:FunctionBlockResource:CreatNewDoc_DisableReason");
                    button.ToolTip = toolTip;
                    this.AddChild(button);

                    break;
                }
                    
            }


        }
        void Ribbon_RibbonTabAdded(Fluent.RibbonTabItem ribbonTab)
        {
            if (ribbonTab.Name.Equals("JobCommon"))
            {
                //ShellService.Ribbon.AddRibbonGroupBox(new GuruRibbonGroupBox(), "JobCommon");
            }
        }
        virtual public void RemoveUI(string commandID)
        {

        }
    }
}
