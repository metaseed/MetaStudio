
using System.Collections.Generic;

namespace Metaseed.MetaShell.Services
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Interface to resolve missing assemblies by Orchestra.
    /// </summary>
    public interface IMissingAssemblyResolverService
    {
        /// <summary>
        /// Resolves the assembly and referenced assemblies.
        /// </summary>
        /// <returns>The resolved assembly or <c>null</c> if the assembly cannot be resolved.</returns>
        Assembly ResolveAssembly(string assemblyFileName);
    }
}