using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Catel;
using Catel.IoC;
using Catel.Logging;
using Fluent;
using Metaseed.MetaShell.Controls;
using Metaseed.MetaShell.Services;
using System.Linq;

namespace Metaseed.MVVM.Commands
{
    //public enum RibbonUIType { Button }
    /// <summary>
    /// contract of UIData.ExtraData:
    /// </summary>
    public class RibbonRemoteCommandUIBuilder : RemoteCommandUIBuilder
    {
        private ILog _log = LogManager.GetCurrentClassLogger();
        protected readonly IShellService ShellService;
        protected Fluent.Ribbon Ribbon
        {
            get { return ((RibbonService)(ShellService.Ribbon)).Ribbon; }
        }

        public RibbonRemoteCommandUIBuilder()
        {
            ShellService = ServiceLocator.Default.ResolveType<IShellService>();
            ShellService.Ribbon.RibbonTabAdded += Ribbon_RibbonTabAdded;
        }
        void Ribbon_RibbonTabAdded(Fluent.RibbonTabItem ribbonTab)
        {
            if (string.IsNullOrEmpty(ribbonTab.Name)) return;
            CompositeRemoteCommand ribbonRemoteCommandFound = null;
            foreach (var ribbonRemoteCommand in DelayGenerateUiList)
            {
                var uidata = (RibbonUIData)(ribbonRemoteCommand.DeserializedUIData);
                if (!ribbonTab.Name.Equals(uidata.UiPosition.RibbonTab.Name)) continue;
                GenerateUI(ribbonRemoteCommand);
                ribbonRemoteCommandFound = ribbonRemoteCommand;
                break;
            }
            if (ribbonRemoteCommandFound != null) DelayGenerateUiList.Remove(ribbonRemoteCommandFound);
        }

        private List<CompositeRemoteCommand> DelayGenerateUiList = new List<CompositeRemoteCommand>();
        override public void GenerateUI(CompositeRemoteCommand command)
        {
            try
            {
                Argument.IsNotNull(() => command.UIData);
                string uiType = command.UIType;
                if (string.IsNullOrEmpty(command.UIType))
                {
                    uiType = typeof(RibbonButtonUIData).Name;
                    _log.Warning("Command Type is not valid, but generate UI for Command:" + command.ID + "using command type RibbonButtonUIData");
                }
                
                if (typeof(RibbonButtonUIData).Name.Equals(uiType))
                {
                    RibbonUIPositionRibbonTab ribbonTabData;
                    RibbonButtonUIData ribbonUiData;
                    command.DeserializedUIData = ribbonUiData = RibbonButtonUIData.Deserialize(command.UIData);
                    ribbonTabData = ribbonUiData.UiPosition.RibbonTab;
                    var groupBox = GetUiPosition(command);
                    if (groupBox == null) return;
                    var button = new Fluent.Button();
                    button.Name = command.ID;
                    if (!string.IsNullOrEmpty(ribbonUiData.ShortCutKeys)) button.SetValue(KeyTip.KeysProperty, ribbonUiData.ShortCutKeys);
                    if (!string.IsNullOrEmpty(ribbonUiData.LocalizedHeader))
                        button.BindToLoc(Fluent.Button.HeaderProperty, ribbonUiData.LocalizedHeader);
                    else
                    {
                        button.Header = command.ID;
                    }
                    if (string.IsNullOrEmpty(ribbonUiData.IconURI))
                    {
                        button.Icon = new BitmapImage(new Uri("pack://application:,,,/Metaseed.ShellBase;component/Resources/Images/No.png",
                               UriKind.RelativeOrAbsolute));
                        button.LargeIcon = new BitmapImage(new Uri("pack://application:,,,/Metaseed.ShellBase;component/Resources/Images/No.png",
                                UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        button.Icon = GetBitmap(ribbonUiData.IconURI);
                        button.LargeIcon = GetBitmap(ribbonUiData.IconURI);
                    }
                    
                    RibbonControlSize size;
                    button.SetValue(Fluent.Button.SizeProperty,
                        RibbonControlSize.TryParse(ribbonUiData.Size, out size) ? size : RibbonControlSize.Large);

                    var toolTip = new Fluent.ScreenTip();
                    toolTip.BindToLoc(ScreenTip.TitleProperty, ribbonUiData.ScreenTip.LocalizedTitle);
                    toolTip.BindToLoc(ScreenTip.TextProperty, ribbonUiData.ScreenTip.LocalizedText);
                    toolTip.Image =
                        string.IsNullOrEmpty(ribbonUiData.ScreenTip.IconURI) ?
                        new BitmapImage(new Uri("pack://application:,,,/Metaseed.ShellBase;component/Resources/Images/No.png", UriKind.RelativeOrAbsolute)) :
                        GetBitmap(ribbonUiData.ScreenTip.IconURI);
                    //toolTip.HelpTopic = "FunctionBlock_CreatNewDoc";
                    toolTip.BindToLoc(ScreenTip.DisableReasonProperty, ribbonUiData.ScreenTip.LocalizedDisableReason);
                    button.ToolTip = toolTip;
                    groupBox.Items.Add(button);
                    var binding = new Binding() { Source = command };
                    button.SetBinding(ButtonBase.CommandProperty, binding);
                }



            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Can not parse the command config data: " + e.Messages());
                return;
            }
        }

        RibbonGroupBox GetUiPosition(CompositeRemoteCommand command)
        {
            RibbonTabItem ribbonTab;
           return GetUiPosition(command, out ribbonTab);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>null: Delay add</returns>
        private RibbonGroupBox GetUiPosition(CompositeRemoteCommand command,out RibbonTabItem ribbonTab)
        {
            Argument.IsNotNull(() => command.DeserializedUIData);
            Argument.IsOfType(() => command.DeserializedUIData, typeof(RibbonUIData));
            var ribbonUiData = (RibbonUIData)(command.DeserializedUIData);
            var ribbonTabData = ribbonUiData.UiPosition.RibbonTab;
            //find ribbon tab
            string tabName = null;
            if (ribbonTabData == null || string.IsNullOrEmpty(ribbonTabData.Name))
                tabName = "RibbonTabHome";
            else
            {
                tabName = ribbonTabData.Name;
            }
            ribbonTab = Ribbon.Tabs.FirstOrDefault(tab => tab.Name.Equals(tabName));
            if (ribbonTab == null)
            {
                if (ribbonTabData != null && ribbonTabData.CreatNewIfCanNotFound)
                {
                    //ToDo: creat new, consider contextual tab?
                    ribbonTab = new RibbonTabItem { Name = ribbonTabData.Name };
                    ribbonTab.BindToLoc(RibbonTabItem.HeaderProperty, ribbonTabData.LocalizedHeader);
                    //Consider ribbon tab group data
                    var ribbonGroupData = ribbonUiData.UiPosition.RibbonTabGroup;
                    if (!string.IsNullOrEmpty(ribbonGroupData.Name))
                    {
                        //find or create
                        var tabGroup =Ribbon.ContextualGroups.FirstOrDefault(ribbonGroup => ribbonGroup.Name.Equals(ribbonGroupData.Name));
                        if (tabGroup == null)
                        {
                            var conv = new BrushConverter();
                            tabGroup = new RibbonContextualTabGroup() { Visibility = Visibility.Visible, Name = ribbonGroupData.Name };

                            if (!string.IsNullOrEmpty(ribbonGroupData.BorderBrush))
                                tabGroup.BorderBrush =
                                    conv.ConvertFromString(ribbonGroupData.BorderBrush) as SolidColorBrush;
                            if (!string.IsNullOrEmpty(ribbonGroupData.BackgroundBrush))
                                tabGroup.Background =
                                    conv.ConvertFromString(ribbonGroupData.BackgroundBrush) as SolidColorBrush;

                            if (string.IsNullOrEmpty(ribbonGroupData.LocalizedHeader))
                            {
                                tabGroup.Header = ribbonGroupData.Name;
                            }
                            else
                            {
                                tabGroup.BindToLoc(RibbonContextualTabGroup.HeaderProperty, ribbonGroupData.LocalizedHeader);
                            }
                            ShellService.Ribbon.AddRibbonContextualTabGroup(tabGroup);
                        }
                        if (ribbonTab.Group == null)
                        {
                            ribbonTab.Group = tabGroup;
                        }
                    }
                    ShellService.Ribbon.AddRibbonTab(ribbonTab);
                }
                else
                {
                    //delay the remote command add.
                    DelayGenerateUiList.Add(command);
                    return null;
                }
            }

            //ribbon group box
            var groupBox = ribbonTab.Groups.FirstOrDefault(g => g.Name.Equals(ribbonUiData.UiPosition.GroupBox.Name));
            if (groupBox == null)
            {
                groupBox = new RibbonGroupBoxContextUI() { Name = ribbonUiData.UiPosition.GroupBox.Name };
                if (string.IsNullOrEmpty(ribbonUiData.UiPosition.GroupBox.LocalizedHeader))
                {
                    groupBox.Header = ribbonUiData.UiPosition.GroupBox.Name;
                }
                else
                {
                    groupBox.BindToLoc(RibbonGroupBox.HeaderProperty, ribbonUiData.UiPosition.GroupBox.LocalizedHeader);
                }

                ribbonTab.Groups.Add(groupBox);
            }
            return groupBox;
        }

        override public void RemoveUI(CompositeRemoteCommand command)
        {
            RibbonTabItem ribbonTab=null;
            var groupBox=GetUiPosition(command,out ribbonTab);
            var ui = groupBox.Items.Cast<FrameworkElement>().FirstOrDefault(item => item.Name.Equals(command.ID));
            groupBox.Items.Remove(ui);
            if (groupBox.Items.Count != 0) return;
            ribbonTab.Groups.Remove(groupBox);
            if (ribbonTab.Groups.Count == 0)
            {
                ShellService.Ribbon.RemoveRibbonTab(ribbonTab);
            }
        }
    }
}
