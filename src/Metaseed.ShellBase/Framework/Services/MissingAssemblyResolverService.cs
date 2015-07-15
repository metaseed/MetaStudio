using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Metaseed.MetaShell.Services
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Catel;
    using Catel.IO;
    using Catel.Logging;
    using Modules;
    /// <summary>
    /// Service to resolve missing assemblies.
    /// </summary>
    public class MissingAssemblyResolverService : IMissingAssemblyResolverService
    {
        public const string ModulesDirectory = @".\Modules";
        public static readonly List<string> MetaStudioModuleAssemblyPrefix = new List<string>() { "☯", "M." };
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        static readonly List<string> _DllFileNamesOfApp;
        static public List<string> DllFileNamesOfApp { get { return _DllFileNamesOfApp; } }

        static readonly List<string> _ModuleFiles = new List<string>();
        static public List<string> ModuleFiles { get { return _ModuleFiles; } }
        static IEnumerable<string> _languageFolders;
        static IEnumerable<string> LanguageFolders
        {
            get
            {
                return _languageFolders ??(_languageFolders = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(info => info.Name));
            }
        }
        static MissingAssemblyResolverService()
        {
            // Load shell files
            _DllFileNamesOfApp = new List<string>(Directory.GetFiles(Metaseed.AppEnvironment.AppPath, "*.dll"));
            // Load module files
            AddAssemblyResolveFolder(Path.Combine(Metaseed.AppEnvironment.AppPath, ModulesDirectory));
        }
        
        public static void AddAssemblyResolveFolder(string folerPath)
        {

            //store all dlls in folerPath and subFolders except language dlls.
            var dllFileNamesOfApp = new List<String>();
            dllFileNamesOfApp.AddRange(Directory.GetFiles(folerPath, "*.dll"));
            var folders =new DirectoryInfo(folerPath).GetDirectories("*", SearchOption.AllDirectories).ToList();
            //http://www.csharp-examples.net/culture-names/
            var folders_ExceptLanguageFolders = from folder in folders
                                                where !LanguageFolders.Contains(folder.Name, StringComparer.InvariantCultureIgnoreCase)
                                                select folder.FullName;
            foreach (var folder in folders_ExceptLanguageFolders)
            {
                var files = Directory.GetFiles(folder, "*.dll", SearchOption.TopDirectoryOnly);
                dllFileNamesOfApp.AddRange(files);
            }
            foreach (var file in dllFileNamesOfApp)
            {
                foreach (var prefix in MetaStudioModuleAssemblyPrefix)
                {
                    if (new FileInfo(file).Name.StartsWith(prefix))
                    {
                        _ModuleFiles.Add(file);
                    }
                }
                
            }
            _DllFileNamesOfApp.AddRange(dllFileNamesOfApp);
        }

        /// <summary>
        /// Resolves the assembly and referenced assemblies.
        /// </summary>
        /// <param name="assemblyFileName">Name of the assembly file.</param>
        /// <returns>The resolved assembly or <c>null</c> if the assembly cannot be resolved.</returns>
        /// <exception cref="ArgumentException">The <paramref name="assemblyFileName"/> is <c>null</c> or whitespace.</exception>
        public Assembly ResolveAssembly(string assemblyName)
        {
            Argument.IsNotNull("assemblyFileName", assemblyName);
            var assemblyFileName = GetAssemblyFileName(assemblyName);
            if (assemblyFileName == null)
            {
                Log.Error("Failed to load assembly '{0}'-could not find the file in all application directories.", assemblyName);
                return null;
            }
            Log.Debug("Resolving assembly '{0}' manually", assemblyFileName);

            var appDomain = AppDomain.CurrentDomain;
            var assemblyDirectory = Catel.IO.Path.GetParentDirectory(assemblyFileName);

            // Load references
            var assemblyForReflectionOnly = Assembly.ReflectionOnlyLoadFrom(assemblyFileName);
            foreach (var referencedAssembly in assemblyForReflectionOnly.GetReferencedAssemblies())
            {
                if (!appDomain.GetAssemblies().Any(a => string.CompareOrdinal(a.GetName().Name, referencedAssembly.Name) == 0))
                {
                    // First, try to load from GAC
                    if (referencedAssembly.GetPublicKeyToken() != null)
                    {
                        try
                        {
                            appDomain.Load(referencedAssembly.FullName);
                            continue;
                        }
                        catch (Exception)
                        {
                            Log.Debug("Failed to load assembly '{0}' from GAC, trying local file", referencedAssembly.FullName);
                        }
                    }
                    // Second, try to load from directory (including referenced assemblies)
                    ResolveAssembly(referencedAssembly.Name);
                }
            }

            // Load assembly itself
            Assembly assembly = null;
            try
            {
                if (appDomain.GetAssemblies().Any(a => string.CompareOrdinal(a.GetName().FullName, assemblyForReflectionOnly.FullName) == 0))
                {
                    Log.Info("Loading assembly '{0}' is not required because it is already loaded", assemblyForReflectionOnly.FullName);
                }
                else
                {
                    assembly = Assembly.LoadFrom(assemblyFileName);
                    Log.Info("Resolved assembly '{0}' manually", assemblyFileName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load assembly '{0}'", assemblyFileName);
            }

            return assembly;
        }

        static public string GetAssemblyFileName(string assemblyName)
        {
            string assemblyFileName = null;
            foreach (var file in DllFileNamesOfApp)
            {
                var fileInfo = new FileInfo(file);
                var fileAssemblyName = fileInfo.Name;
                var extensionStart = fileAssemblyName.LastIndexOf(fileInfo.Extension);
                if (extensionStart > 0)
                {
                    fileAssemblyName = fileAssemblyName.Substring(0, extensionStart);
                }
                if (string.Equals(fileAssemblyName, assemblyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    assemblyFileName = fileInfo.FullName;
                    break;
                }
            }
            return assemblyFileName;
        }
    }
}