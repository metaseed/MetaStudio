﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Metaseed.MetaShell.Framework.Shell.Views {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ShellViewResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ShellViewResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Metaseed.MetaShell.Framework.Shell.Views.ShellViewResource", typeof(ShellViewResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attention.
        /// </summary>
        internal static string Attention {
            get {
                return ResourceManager.GetString("Attention", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This Document Has Not Been Saved, Are You Want To Save It And Then Close It?.
        /// </summary>
        internal static string ClosingDocumentDirtyQuestion {
            get {
                return ResourceManager.GetString("ClosingDocumentDirtyQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CLOSE.
        /// </summary>
        internal static string MyNotifyIcon_Close {
            get {
                return ResourceManager.GetString("MyNotifyIcon_Close", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New Data Not Saved.
        /// </summary>
        internal static string NewDataNotSaved {
            get {
                return ResourceManager.GetString("NewDataNotSaved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File Path:.
        /// </summary>
        internal static string StatusBarItem_FilePath {
            get {
                return ResourceManager.GetString("StatusBarItem_FilePath", resourceCulture);
            }
        }
    }
}
