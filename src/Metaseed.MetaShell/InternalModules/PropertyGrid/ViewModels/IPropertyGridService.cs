using Xceed.Wpf.Toolkit.PropertyGrid;
using System;
using Catel.Messaging;
namespace Metaseed.Modules.PropertyGrid.Services
{
    using MetaShell.ViewModels;
    public class PropertyGridServiceAvailableEvent : MessageBase<PropertyGridServiceAvailableEvent, IPropertyGridService>//CompositePresentationEvent<IPropertyGridService>
    { }
	public interface IPropertyGridService : IToolViewModel
	{
        //https://wpftoolkit.codeplex.com/wikipage?title=PropertyGrid&referringTitle=Home
        //http://stackoverflow.com/questions/356464/localization-of-displaynameattribute

        //concerning your browsable properties, you have 2 options.
        //https://wpftoolkit.codeplex.com/discussions/403411
        //https://wpftoolkit.codeplex.com/discussions/461292
        /*
         using Metaseed.ComponentModel;
         using System.ComponentModel;
         using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
        [LocalizedDisplayName("Metaseed.MetaShell:Resources:InstanceTitle")]
        [LocalizedDescription("Metaseed.MetaShell:Resources:InstanceTitleDescription")]
        [LocalizedCategory("Metaseed.MetaShell:Resources:InstanceTitle")]
        [PropertyOrder(1)]
        [Browsable(true)]
         */
        object SelectedObject { get; set; }
        bool AutoGenerateProperties { get; set; }
        void SetPropertyEditor(Type propertyType, Type iTypeEditor);
        void SetPropertyEditor(string propertyName, Type iTypeEditor);
        void AddEditorDefinition(EditorTemplateDefinition editorDefinition);
	}
}