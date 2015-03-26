using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.ComponentModel.Composition
{
    public interface IServiceProvider
    {
        void RegisterInstance<T>(T exportedValue);
        void RegisterInstance<T>( T exportedValue,string contractName);
    }
}
