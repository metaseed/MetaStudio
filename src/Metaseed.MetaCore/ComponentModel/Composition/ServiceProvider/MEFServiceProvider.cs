using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
namespace Metaseed.ComponentModel.Composition
{
    public class MEFServiceProvider:IServiceProvider
    {
        CompositionContainer _Container;
        public MEFServiceProvider(CompositionContainer container)
        {
            _Container = container;
            ServiceProvider.Current = this;
        }
        public void RegisterInstance<T>(T exportedValue)
        {
            _Container.ComposeExportedValue<T>(exportedValue);
        }

        public void RegisterInstance<T>(T exportedValue, string contractName)
        {
            _Container.ComposeExportedValue<T>(contractName, exportedValue);
        }
    }
}
