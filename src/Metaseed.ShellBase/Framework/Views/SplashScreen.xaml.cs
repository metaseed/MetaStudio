
using System;
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
            InitializeComponent();
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
