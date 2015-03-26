using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catel.IoC;
namespace Metaseed.MetaShell.ViewModels
{
    using Metaseed.Views;
    using Services;
    public class ContextUserInterface 
    {
        List<IContextUI> contextUIs = new List<IContextUI>();
        class ContextualToolViewModelBuilder : IContextUI
        {
            public ContextualToolViewModelBuilder(Guid id, Type toolType)
            {
                ID = id;
                _toolType = toolType;
            }

            public Guid ID { get; private set; }
            Type _toolType;

            public IToolViewModel GetTool()
            {
                return Activator.CreateInstance(_toolType) as IToolViewModel;
            }
            public void Initialize()
            {
                throw new NotImplementedException();
            }

            public bool HasInitialized
            {
                get { throw new NotImplementedException(); }
            }

            public void Show(object objectWithContext)
            {
                throw new NotImplementedException();
            }

            public void Hide(object objectWithContext)
            {
                throw new NotImplementedException();
            }
        }
        public T GetContextualTool<T>(Guid toolID)
        {
            return (T)GetContextualTool(toolID);
        }
        public IToolViewModel GetContextualTool(Guid toolID)
        {
            for (int i = 0; i < contextUIs.Count; i++)
            {
                var contextualUI = contextUIs[i];
                if (contextualUI is ContextualToolViewModelBuilder)
                {
                    var contextualToolViewModelBuilder=contextualUI as ContextualToolViewModelBuilder;
                    if (contextualToolViewModelBuilder.ID.Equals(toolID))
                    {
                        var shellService = this.GetDependencyResolver().Resolve<Metaseed.MetaShell.Services.IShellService>();
                        var tool = ((ShellService)shellService).GetTool(contextualToolViewModelBuilder.ID);
                        if (tool == null)
                        {
                            tool = contextualToolViewModelBuilder.GetTool();
                        }
                        contextualUI = contextUIs[i] = tool;
                        return tool;
                    }
                }
                else if (contextualUI is IToolViewModel)
                {
                   var iToolViewModel= contextualUI as IToolViewModel;
                   if (iToolViewModel.ID.Equals(toolID))
                    {
                        return iToolViewModel;
                    }
                }
               
            }
            return null;
        }
        public void AddContextualTool(Guid toolID,Type toolType)
        {
            contextUIs.Add(new ContextualToolViewModelBuilder(toolID, toolType));
        }
        public void AddContextualUI(IContextUI contextualUI)
        {
            System.Diagnostics.Debug.Assert(!(contextualUI is IToolViewModel), "using AddContextualTool methord to add contextual tool");
            contextUIs.Add(contextualUI);

        }
        public void Clear()
        {
            contextUIs.Clear();
        }
        public void Hide(object obj)
        {
            foreach (var contextualUI in contextUIs)
            {
                if(!(contextualUI is ContextualToolViewModelBuilder))
                {
                    contextualUI.Hide(obj);
                }
            }
        }
        public void Show(object obj)
        {
            for (int i = 0; i < contextUIs.Count; i++)
            {
                var contextualUI = contextUIs[i];
                if (contextualUI is ContextualToolViewModelBuilder)
                {
                    var shellService = this.GetDependencyResolver().Resolve<Metaseed.MetaShell.Services.IShellService>();
                    var tool = ((ShellService)shellService).GetTool((contextualUI as ContextualToolViewModelBuilder).ID) ??(contextualUI as ContextualToolViewModelBuilder).GetTool();
                    contextualUI = contextUIs[i] = tool;
                }
                if (!contextualUI.HasInitialized)
                {
                    contextualUI.Initialize();
                }
                contextualUI.Show(obj);
            }

        }
    }
}
