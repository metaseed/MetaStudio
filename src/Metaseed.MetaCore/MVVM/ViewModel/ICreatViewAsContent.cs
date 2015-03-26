using System;
using System.Windows;
using System.Windows.Controls;

namespace Metaseed.MVVM.ViewModel
{
    public interface ICreatViewAsContent
    {
        /// <summary>
        /// paramenters is view and viewmodel
        /// </summary>
        void SetViewAsContent(ContentControl contentControl, FrameworkElement view, ICreatViewAsContent viewModel);
    }
}
