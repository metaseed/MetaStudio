
using System;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Catel.Windows;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
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
                //product name
                var info = XElement.Load(configPath);
                var productName = info.Element("ProductName");
                if (productName != null ) {
                    if(!productName.Elements().Any())
                        AppName.Text = productName.Value;
                    else
                    {
                        var text = productName.Element("Text");
                        if (text!=null)
                            AppName.Text = text.Value;
                        var fontFile= productName.Element("FontFileDir");
                        
                        var font = productName.Element("FontFamily");
                        
                        if (!String.IsNullOrEmpty(font?.Value))
                        {
                            if (fontFile!=null)
                            {
                                var fontFileUri = new Uri("file:///" + AppEnvironment.AppPath +"/"+ fontFile.Value+"/");
                                AppName.FontFamily = new FontFamily(fontFileUri, "./#" + font.Value);
                            }
                            else
                            {
                                AppName.FontFamily = new FontFamily(font.Value.Trim());
                            }
                        }

                        var size = productName.Element("FontSize");
                        if (!String.IsNullOrEmpty(size?.Value))
                        {
                            var myFontSizeConverter = new FontSizeConverter();
                            var convertFromString = myFontSizeConverter.ConvertFromString(size.Value.Trim());
                            if (convertFromString != null)
                                AppName.FontSize= (Double)convertFromString;
                        }
                        var weight = productName.Element("FontWeight");
                        if (!String.IsNullOrEmpty(weight?.Value))
                        {
                            var convertFromString = new FontWeightConverter().ConvertFromString(weight.Value);
                            if (convertFromString != null)
                                AppName.FontWeight= (FontWeight)convertFromString;
                        }
                        var style = productName.Element("FontStyle");
                        if (!String.IsNullOrEmpty(style?.Value))
                        {
                            var convertFromString = new FontStyleConverter().ConvertFromString(style.Value);
                            if (convertFromString != null)
                                AppName.FontStyle= (FontStyle)convertFromString;
                        }
                        var color = productName.Element("Color");
                        if (!String.IsNullOrEmpty(color?.Value))
                        {
                            var fromString = new BrushConverter().ConvertFromString(color.Value);
                            if (fromString != null)
                            {
                                var convertFromString = (Brush)fromString;
                                AppName.Foreground = convertFromString;
                            }
                        }
                        var outLineColor = productName.Element("OutlineColor");
                        if (!String.IsNullOrEmpty(outLineColor?.Value))
                        {
                            var fromString = ColorConverter.ConvertFromString(outLineColor.Value);
                            if (fromString != null)
                            {
                                var convertFromString = (Color)fromString;
                                AppNameOutline.Color = convertFromString;
                            }
                        }
                        var margin = productName.Element("Margin");
                        if (!String.IsNullOrEmpty(margin?.Value))
                        {
                            var fromString = new ThicknessConverter().ConvertFromString(margin.Value);
                            if (fromString != null)
                            {
                                var convertFromString = (Thickness)fromString;
                                AppName.Margin = convertFromString;
                            }
                        }
                        var horizontalAlignment = productName.Element("HorizontalAlignment");
                        if (!String.IsNullOrEmpty(horizontalAlignment?.Value))
                        {
                            HorizontalAlignment convertFromString;
                            var r = Enum.TryParse(horizontalAlignment.Value, out convertFromString);
                            if (r)
                                AppName.HorizontalAlignment = convertFromString;
                        }
                    }
                }

                //sub title
                var subTitle = info.Element("SubTitle");
                if (subTitle != null)
                {
                    if (!subTitle.Elements().Any())
                        SubTitle.Text = subTitle.Value;
                    else
                    {
                        var text = subTitle.Element("Text");
                        if (text != null)
                            SubTitle.Text = text.Value;
                        else
                        {
                            SubTitle.Height = 0;
                        }
                        var fontFile = subTitle.Element("FontFileDir");

                        var font = subTitle.Element("FontFamily");

                        if (!String.IsNullOrEmpty(font?.Value))
                        {
                            if (fontFile != null)
                            {
                                var fontFileUri =
                                    new Uri("file:///" + AppEnvironment.AppPath + "/" + fontFile.Value + "/");
                                SubTitle.FontFamily = new FontFamily(fontFileUri, "./#" + font.Value);
                            }
                            else
                            {
                                SubTitle.FontFamily = new FontFamily(font.Value.Trim());
                            }
                        }

                        var size = subTitle.Element("FontSize");
                        if (!String.IsNullOrEmpty(size?.Value))
                        {
                            var myFontSizeConverter = new FontSizeConverter();
                            var convertFromString = myFontSizeConverter.ConvertFromString(size.Value.Trim());
                            if (convertFromString != null)
                                SubTitle.FontSize = (Double) convertFromString;
                        }
                        var weight = subTitle.Element("FontWeight");
                        if (!String.IsNullOrEmpty(weight?.Value))
                        {
                            var convertFromString = new FontWeightConverter().ConvertFromString(weight.Value);
                            if (convertFromString != null)
                                SubTitle.FontWeight = (FontWeight) convertFromString;
                        }
                        var style = subTitle.Element("FontStyle");
                        if (!String.IsNullOrEmpty(style?.Value))
                        {
                            var convertFromString = new FontStyleConverter().ConvertFromString(style.Value);
                            if (convertFromString != null)
                                SubTitle.FontStyle = (FontStyle) convertFromString;
                        }
                        var color = subTitle.Element("Color");
                        if (!String.IsNullOrEmpty(color.Value))
                        {
                            var fromString = new BrushConverter().ConvertFromString(color.Value);
                            if (fromString != null)
                            {
                                var convertFromString = (Brush) fromString;
                                SubTitle.Foreground = convertFromString;
                            }
                        }
                        var outLineColor = subTitle.Element("OutlineColor");
                        if (!String.IsNullOrEmpty(outLineColor?.Value))
                        {
                            var fromString = (Color)ColorConverter.ConvertFromString(outLineColor.Value);
                            if (fromString != null)
                            {
                                var convertFromString = fromString;
                                SubTitleOutline.Color = convertFromString;
                            }
                        }
                        var margin = subTitle.Element("Margin");
                        if (!String.IsNullOrEmpty(margin?.Value))
                        {
                            var fromString = new ThicknessConverter().ConvertFromString(margin.Value);
                            if (fromString != null)
                            {
                                var convertFromString = (Thickness)fromString;
                                SubTitle.Margin = convertFromString;
                            }
                        }
                        var horizontalAlignment = subTitle.Element("HorizontalAlignment");
                        if (!String.IsNullOrEmpty(horizontalAlignment?.Value))
                        {
                            HorizontalAlignment convertFromString;
                            var r=  Enum.TryParse(horizontalAlignment.Value,out convertFromString);
                            if (r)
                                SubTitle.HorizontalAlignment = convertFromString;
                        }
                    }
                }
                else
                {
                    SubTitle.Height = 0;
                }

                //copy right
                var copyright = info.Element("Copyright");
                if (!string.IsNullOrEmpty(copyright?.Value))
                {
                    Copyright.Text = copyright.Value;
                }
                else
                {
                    Copyright.Height = 0;
                }
            }

            CustomiseSplashScreen();
        }

        private void CustomiseSplashScreen()
        {
            var directory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            try
            {
                string splashScreenDir = Path.Combine(directory, SplashScreenLocation);
                var pictures= Directory.GetFiles(splashScreenDir, "*.jpg");
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
