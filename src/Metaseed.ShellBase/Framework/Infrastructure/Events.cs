using System;
using Catel.Messaging;
using System.Windows;
using System.Globalization;
using Catel.MVVM.Views;
using Metaseed.MetaShell.Services;
using Metaseed.MetaShell.ViewModels;

namespace Metaseed.MetaShell.Infrastructure{
    public class AppClosingEvent : MessageBase<AppClosingEvent, IDataWindow> { }
    public class CurrentThemeChangedEvent : MessageBase<CurrentThemeChangedEvent, Tuple<AppTheme, AppTheme>>
    {

    }
    public class CurrentCultureChangedEvent : MessageBase<CurrentCultureChangedEvent, Tuple<CultureInfo, CultureInfo>>
    {

    }
    public class ActiveDocumentChangedEvent : MessageBase<ActiveDocumentChangedEvent, Tuple<IDocumentViewModel, IDocumentViewModel>>
    {

    }
    public class DocumentClosedEvent : MessageBase<DocumentClosedEvent, IDocumentViewModel>
    {

    }
    public class ShellShownEvent : MessageBase<ShellShownEvent, ShellViewModel>
    {

    }
}
