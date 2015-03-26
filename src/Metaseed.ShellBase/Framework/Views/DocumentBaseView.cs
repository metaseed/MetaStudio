using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Regions;
using Catel.IoC;
namespace Metaseed.MetaShell.Views
{
    using Infrastructure;
    using ViewModels;
    using System.ComponentModel;
    using Metaseed.ComponentModel;
    
    public  class DocumentBaseView:LayoutContentView,IDocumentView
    {
        public DocumentBaseView():this(null)
        {

        }
        public DocumentBaseView(DocumentBaseViewModel viewModel)
            : base(viewModel)
        {
            
        }
      
       
        //public IDocumentViewModel DocumentViewModel { get { return this.ViewModel as IDocumentViewModel; } }
    }
}
