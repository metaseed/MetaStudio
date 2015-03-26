using System;
using System.Windows;
using System.Collections.ObjectModel;
using Fluent;
using Catel;
using System.ComponentModel;
using System.Globalization;
using Catel.Messaging;
using Xceed.Wpf.AvalonDock.Themes;

namespace Metaseed.MetaShell.Services
{
    using ViewModels;
    using Views;

    public interface IShellService
    {
        //event PropertyChangedEventHandler CurrentThemeChanged;
        //event PropertyChangedEventHandler CurrentCultureChanged;
        IMessager Messager{get;}
        CultureInfo CurrentCulture { get; }

        #region Theme
        AppTheme CurrentTheme { get; set; }
        #endregion
        bool Clear();
        #region Document
        ObservableCollection<IDocumentViewModel> Documents { get; }
        ObservableCollection<DocumentUnopenBase> DocumentsUnopen { get; }
        IDocumentViewModel ActiveDocument { get; }
        //event PropertyChangedEventHandler ActiveDocumentChanged;
        void OpenDocument(IDocumentViewModel documentViewModel);
        void ActivateDocument(IDocumentViewModel documentViewModel);
        bool CloseDocument(IDocumentViewModel documentViewModel);
        bool CouldDeleteDocument(IDocumentViewModel documentViewModel);
        void DeleteDocument(ILayoutContentViewModel documentViewModel);
        //event Action<IDocumentViewModel> DocumentClosed;
        #endregion
        #region Tool
        ObservableCollection<IToolViewModel> Tools { get; }        
        void ShowTool(IToolViewModel toolViewModel);
        void HideTool(IToolViewModel toolViewModel);
        #endregion

        IRibbonService Ribbon { get; }
        void CloseApp();
    }
    public enum AppTheme
    {
        Office2013,
        Office2010Silver,
        Office2010Black,
        Office2010Blue
        
    }
}