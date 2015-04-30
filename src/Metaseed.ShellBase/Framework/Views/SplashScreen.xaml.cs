
using System;
using System.Xml;
using System.Xml.Linq;
using Catel.Windows;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Metaseed.MetaShell.Views
{
    using ViewModels;
    using Catel.MVVM;
    using Catel.IoC;
    /// <summary>
    /// Interaction logic for SplashScreen.xaml.
    /// </summary>
    public partial class SplashScreen : DataWindow
    {
        private  string SplashScreenLocation =AppEnvironment.AppPath+ @"\Resources\Images\SplashScreen";
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreen"/> class.
        /// </summary>
        public SplashScreen()
            : base(DataWindowMode.Custom)
        {
            var assembly=Assembly.GetEntryAssembly();
            var titleAttributes=assembly.GetCustomAttributes(typeof (AssemblyTitleAttribute), true);
            
            InitializeComponent();
            if (titleAttributes.Length > 0)
            {
                AppName.Text = ((AssemblyTitleAttribute)titleAttributes[0]).Title;
            }
            var copyrightAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            if (copyrightAttributes.Length > 0)
            {
                Copyright.Text = ((AssemblyCopyrightAttribute)copyrightAttributes[0]).Copyright;
            }
            var configPath = AppEnvironment.AppPath + @"\Resources\Images\SplashScreen\SplashScreenInfo.xml";
            if (File.Exists(configPath))
            {
                var info = XElement.Load(configPath);
                var productName = info.Element("ProductName");
                if (productName != null) AppName.Text = productName.Value;
                var copyright = info.Element("Copyright");
                if (copyright != null) Copyright.Text = copyright.Value;
            }

            CustomiseSplashScreen();
        }

        private void CustomiseSplashScreen()
        {
            var directory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            try
            {
                string splashScreenDir = Path.Combine(directory, SplashScreenLocation);
                var pictures= Directory.GetFiles(splashScreenDir);
                int i = new Random(DateTime.Now.Millisecond).Next(pictures.Length);

                if (File.Exists(pictures[i]))
                {
                    if (Path.GetFileName(pictures[i]).StartsWith("L"))
                    {
                        statusPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    }
                    splashScreenImage.Source = new BitmapImage(new Uri(pictures[i], UriKind.Absolute));
                    return;
                }
            }
            catch (Exception)
            {
                // Swallow exception
            }
        }
    }
}
