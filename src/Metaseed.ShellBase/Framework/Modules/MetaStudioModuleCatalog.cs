using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism;

namespace Metaseed.Modules
{
    using Metaseed.MetaShell.Services;
    public class MetaStudioModuleCatalog : ModuleCatalog
    {
        private class InnerModuleInfoLoader : MarshalByRefObject
        {
            internal ModuleInfo[] GetModuleInfos(string path)
            {
                ResolveEventHandler value = (object sender, ResolveEventArgs args) => this.OnReflectionOnlyResolve(args);
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += value;
                var assembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().First((Assembly asm) => asm.FullName == typeof(IModule).Assembly.FullName);
                 var type = assembly.GetType(typeof(IModule).FullName);
                var notAllreadyLoadedModuleInfos = InnerModuleInfoLoader.GetNotAllreadyLoadedModuleInfos(path, type);
                var result = notAllreadyLoadedModuleInfos.ToArray<ModuleInfo>();
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= value;
                return result;
            }
            private static IEnumerable<ModuleInfo> GetNotAllreadyLoadedModuleInfos(string path, Type moduleType)
            {
                try
                {
                    Assembly.ReflectionOnlyLoadFrom(path);
                }
                catch (BadImageFormatException)
                {
                }
                var r = from t in Assembly.ReflectionOnlyLoadFrom(path).GetExportedTypes().Where(new Func<Type, bool>(moduleType.IsAssignableFrom))
                        where t != moduleType
                        where !t.IsAbstract
                        select t into type
                        select InnerModuleInfoLoader.CreateModuleInfo(type);
                return r;
            }
            private Assembly OnReflectionOnlyResolve(ResolveEventArgs args)
            {
                var assembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().FirstOrDefault((Assembly asm) => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));
                if (assembly != null)
                {
                    return assembly;
                }
                var assemblyName = new AssemblyName(args.Name);
                string text = MissingAssemblyResolverService.GetAssemblyFileName(assemblyName.Name);
                if (File.Exists(text))
                {
                    return Assembly.ReflectionOnlyLoadFrom(text);
                }
                return Assembly.ReflectionOnlyLoad(args.Name);
            }
            internal void LoadAssemblies(IEnumerable<string> assemblies)
            {
                foreach (var current in assemblies)
                {
                    try
                    {
                        Assembly.ReflectionOnlyLoadFrom(current);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
            }
            private static ModuleInfo CreateModuleInfo(Type type)
            {
                var name = type.Name;
                var flag = false;
                var customAttributeData = CustomAttributeData.GetCustomAttributes(type).FirstOrDefault((CustomAttributeData cad) => cad.Constructor.DeclaringType != null && cad.Constructor.DeclaringType.FullName == typeof(ModuleAttribute).FullName);
                if (customAttributeData != null && customAttributeData.NamedArguments!=null)
                {
                    foreach (var current in customAttributeData.NamedArguments)
                    {
                        var name2 = current.MemberInfo.Name;
                        if (!string.IsNullOrEmpty(name2)) continue;
                        if (name2 != "ModuleName")
                        {
                            if (name2 != "OnDemand")
                            {
                                if (name2 == "StartupLoaded")
                                {
                                    flag = !(bool)current.TypedValue.Value;
                                }
                            }
                            else
                            {
                                flag = (bool)current.TypedValue.Value;
                            }
                        }
                        else
                        {
                            name = (string)current.TypedValue.Value;
                        }
                    }
                }
                var enumerable =
                    from cad in CustomAttributeData.GetCustomAttributes(type)
                    where cad.Constructor.DeclaringType != null && cad.Constructor.DeclaringType.FullName == typeof(ModuleDependencyAttribute).FullName
                    select cad;
                var list = enumerable.Select(current2 => (string) current2.ConstructorArguments[0].Value).ToList();
                var moduleInfo = new ModuleInfo(name, type.AssemblyQualifiedName)
                {
                    InitializationMode = flag ? InitializationMode.OnDemand : InitializationMode.WhenAvailable,
                    Ref = type.Assembly.CodeBase
                };
                moduleInfo.DependsOn.AddRange(list);
                return moduleInfo;
            }
        }
        /// <summary>
        /// Directory containing modules to search for.
        /// </summary>
        public string ModulePath
        {
            get;
            set;
        }

        /// <summary>
        /// Drives the main logic of building the child domain and searching for the assemblies.
        /// </summary>
        protected override void InnerLoad()
        {
            if (string.IsNullOrEmpty(this.ModulePath))
            {
                throw new InvalidOperationException("ModulePathCannotBeNullOrEmpty");
            }
            if (!Directory.Exists(this.ModulePath))
            {
                throw new InvalidOperationException(string.Format("Directory: {0} NotFound", new object[]
				{
					this.ModulePath
				}));
            }
            var appDomain = this.BuildChildDomain(AppDomain.CurrentDomain);
            InnerModuleInfoLoader innerModuleInfoLoader = null;
            try
            {
                var list = new List<string>();
                var collection = from Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 where !(assembly is AssemblyBuilder) && assembly.GetType().FullName != "System.Reflection.Emit.InternalAssemblyBuilder" && !string.IsNullOrEmpty(assembly.Location)
                                 select assembly.Location;
                list.AddRange(collection);
                Type typeFromHandle = typeof(InnerModuleInfoLoader);

                innerModuleInfoLoader = (InnerModuleInfoLoader)appDomain.CreateInstanceFrom(typeFromHandle.Assembly.Location, typeFromHandle.FullName).Unwrap();
                innerModuleInfoLoader.LoadAssemblies(list);

            }
            catch (Exception e)
            {
                AppDomain.Unload(appDomain);
                throw e;
            }

            var modulePaths = MissingAssemblyResolverService.ModuleFiles;
            try
            {
                foreach (var modulePath in modulePaths)
                {
                    base.Items.AddRange(innerModuleInfoLoader.GetModuleInfos(modulePath));
                }
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
        }

        protected virtual AppDomain BuildChildDomain(AppDomain parentDomain)
        {
            if (parentDomain == null)
            {
                throw new ArgumentNullException("parentDomain");
            }
            var securityInfo = new Evidence(parentDomain.Evidence);
            var setupInformation = parentDomain.SetupInformation;
            return AppDomain.CreateDomain("DiscoveryRegion", securityInfo, setupInformation);
        }

    }
}
