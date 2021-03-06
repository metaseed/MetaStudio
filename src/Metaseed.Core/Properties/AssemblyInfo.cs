﻿using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using System.Windows.Markup;

[assembly: AssemblyTitle("Metaseed.Core")]
[assembly: AssemblyDescription("Metaseed.Core")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Metaseed")]
[assembly: AssemblyProduct("Metaseed.Core")]
[assembly: AssemblyCopyright("Copyright © Metaseed 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleToAttribute("Metaseed.MetaCore")]
[assembly: InternalsVisibleToAttribute("Metaseed.ShellBase")]
[assembly: InternalsVisibleToAttribute("Metaseed.MetaStudioTest")]
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly:ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                             //(used if a resource is not found in the page, 
                             // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                      //(used if a resource is not found in the page, 
                                      // app, or any theme specific resource dictionaries)
)]
[assembly: XmlnsDefinition("http://www.metaseed.com", "Metaseed.Windows.Controls")]
[assembly: XmlnsDefinition("http://www.metaseed.com", "Metaseed.Windows.Data.Converters")]
[assembly: XmlnsPrefix("http://www.metaseed.com", "metaseed")]


