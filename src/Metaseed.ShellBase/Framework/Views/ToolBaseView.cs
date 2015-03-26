using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Regions;
namespace Metaseed.MetaShell.Views
{
    using ViewModels;
    using Infrastructure;
    using Services;
    public  class ToolBaseView:LayoutContentView,IToolView
    {
        public ToolBaseView()
            : this(null)
        {

        }
        public ToolBaseView(ToolBaseViewModel viewModel)
            : base(viewModel)
        {
           
        }
        public IToolViewModel ToolViewModel { get {return this.ViewModel as  IToolViewModel; } }
    }
}
