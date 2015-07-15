
using Catel.Memento;
using System.Collections.Generic;
using System;
namespace Metaseed.MetaShell.ViewModels
{
    using Metaseed.Views;
	public interface IDocumentViewModel : ILayoutContentViewModel
	{
        IMementoService MementoService { get; }
        //void OnActiveDocumentChanged(IDocumentViewModel oldValue, IDocumentViewModel newValue);
        string InstanceTitle { get; set; }
        bool IsDeletable { get; }
        bool KeepAliveWhenClose { get; set; }
        bool IsAliveClosed { get; }
        void Show();
	}
}