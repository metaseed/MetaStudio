using System.Windows;
using Metaseed.Common;

namespace ChooseFontTest
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void ChooseFont_Click(object sender, RoutedEventArgs args)
		{
            FontInfo fi = new FontInfo();
            fi.FontFamily = sampleText.FontFamily;
            fi.Typeface = FontInfo.selectBestMatchingTypeface(fi.FontFamily, sampleText.FontStyle,
                sampleText.FontWeight, sampleText.FontStretch);
            fi.FontColor = (sampleText.Foreground as System.Windows.Media.SolidColorBrush).Color;
            fi.FontSize = (double)sampleText.FontSize;
            fi.Decorations = sampleText.TextDecorations;
            ChooseFont dlg = new ChooseFont(fi);
			//ChooseFont dlg = new ChooseFont(sampleText);
			//dlg.IsFontDetailsExpanded = true;
			if (dlg.ShowDialog() == true)
			{
				dlg.ApplyTo(sampleText);
			}
		}
	}
}
