using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Metaseed;
using Catel;
namespace Metaseed.ComponentModel.Composition
{
    public static class ServiceProvider
    {
        static IServiceProvider currentProvider;
        public static IServiceProvider Current
        {
            get
            {
                Argument.IsNotNull("ServiceProvider.Current", currentProvider);
                return currentProvider;
            }
            set { currentProvider = value; }
        }
    }
}
