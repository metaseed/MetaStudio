using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Metaseed
{
    public static partial class AppEnvironment
    {
        static string _AppPath;
        public static readonly bool IsDesign = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
        static public string AppPath
        {
            get
            {
                if (_AppPath == null)
                {
                    if (IsDesign)
                    {
                        _AppPath = (
                            from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            where assembly.CodeBase.EndsWith("Metaseed.MetaCore.dll")
                            select System.IO.Path.GetDirectoryName(assembly.CodeBase.Replace("file:///", ""))
                            ).FirstOrDefault();
                    }
                    else
                    {
                        //_AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                        //_AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(AppEnvironment)).Location);
                        _AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    }
                }
                //_AppPath = Environment.GetEnvironmentVariable("MetaStudioPath");
                return _AppPath;
            }
        }
        private static bool _bypassDevEnvCheck;
        private static bool? _isInDesignMode;
        /// <summary>
        /// Gets whether the software is currently in design mode.
        /// <para />
        /// </summary>
        /// <returns><c>true</c> if the software is in design mode, <c>false</c> otherwise.</returns>
        public static bool GetIsInDesignMode()
        {
            var isInDesignMode = new bool?((bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue);
            if (!isInDesignMode.Value)
            {
                isInDesignMode = new bool?(DesignerProperties.GetIsInDesignMode(new DependencyObject()));
            }
            if (!BypassDevEnvCheck && !isInDesignMode.Value && ProcessHelper.IsProcessHostedByTool)
            {
                isInDesignMode = new bool?(true);
            }
            return isInDesignMode.Value;
        }
        /// <summary>
        /// Gets or sets a value indicating whether the "devenv.exe" check should be bypassed. By default, the <see cref="P:Catel.CatelEnvironment.IsInDesignMode" />
        /// also checks whether the current process is "devenv.exe".
        /// <para />
        /// This behavior is not very useful when using Catel in Visual Studio extensions, so it is possible to bypass that specific check.
        /// </summary>
        /// <value><c>true</c> if the check should be bypassed; otherwise, <c>false</c>.</value>
        public static bool BypassDevEnvCheck
        {
            get
            {
                return _bypassDevEnvCheck;
            }
            set
            {
                _bypassDevEnvCheck = value;
                _isInDesignMode = null;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the environment is currently in design mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if the environment is in design mode; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    _isInDesignMode = new bool?(GetIsInDesignMode());
                }
                return _isInDesignMode.Value;
            }
        }
    }
}
