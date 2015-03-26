// <copyright file="ChooseFont.xaml.cs" company="Oleg V. Polikarpotchkin">
// Copyright © 2009 Oleg V. Polikarpotchkin. All Right Reserved
// </copyright>
// <author>Oleg V. Polikarpotchkin</author>
// <email>ov-p@yandex.ru</email>
// <date>2009-04-28</date>
// <summary>ChooseFontDialog. Interaction logic for ChooseFont.xaml.</summary>
// <revision>$Id$</revision>

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Metaseed.Windows.Controls
{
	/// <summary>
	/// Interaction logic for ChooseFont.xaml.
	/// </summary>
	public partial class ChooseFont : Window
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChooseFont"/> class.
		/// </summary>
		public ChooseFont(FontInfo fontInfo)
		{
			InitializeComponent();

			FontInfo = fontInfo.Clone() as FontInfo;
			FontInfo.PropertyChanged += FontInfo_PropertyChanged;
			DataContext = FontInfo;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChooseFont"/> class.
		/// </summary>
		public ChooseFont(DependencyObject dependencyObject)
		{
			InitializeComponent();

			FontInfo = new FontInfo(dependencyObject);
			FontInfo.PropertyChanged += FontInfo_PropertyChanged;
			DataContext = FontInfo;
		}

		/// <summary>
		/// Gets the <see cref="FontInfo"/> object.
		/// </summary>
		/// <value>The <see cref="FontInfo"/> object.</value>
		public FontInfo FontInfo { get; private set; }

		/// <summary>
		/// Handles the PropertyChanged event of the FontInfo object.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
		void FontInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Button_OK.IsEnabled = true;
		}

		/// <summary>
		/// Applies the Font properties to the object specified.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		public void ApplyTo(DependencyObject dependencyObject)
		{
			FontInfo.ApplyTo(dependencyObject);
		}

		/// <summary>
		/// Gets or sets a value indicating whether Font Details panel is expanded.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if Font Details panel is expanded; otherwise, <c>false</c>.
		/// </value>
		public bool IsFontDetailsExpanded
		{
			get { return FontInfo.IsFontDetailsExpanded; }
			set { FontInfo.IsFontDetailsExpanded = value; }
		}

		#region Autocomplete handlers
		/// <summary>
		/// Autocomplete for the FontFamily TextBox-ListBox pair. 
		/// Handles the TextChanged event of the txtFontFamily control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
		private void txtFontFamily_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!txtFontFamily.IsFocused)
			{
				lbxFontFamily.ScrollIntoView(FontInfo.FontFamily);
				return;
			}

			int caretIndex = txtFontFamily.CaretIndex;
			string prefix = txtFontFamily.Text.Substring(0, caretIndex);
			if (prefix.Length == 0)
				return;

			IEnumerable<FontFamily> families = from f in FontInfo.FontFamilies
											   where f.Source.StartsWith(prefix)
											   select f;
			if (families.Count() == 0)
				return;

			FontFamily family = families.First();
			txtFontFamily.Text = family.ToString();
			txtFontFamily.Select(caretIndex, txtFontFamily.Text.Length - caretIndex);

			lbxFontFamily.SelectedItem = family;
			lbxFontFamily.ScrollIntoView(family);
		}

		/// <summary>
		/// Autocomplete for the Typeface TextBox-ListBox pair. 
		/// Handles the Changed event of the txtTypeface control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
		private void txtTypeface_Changed(object sender, TextChangedEventArgs e)
		{
			if (!txtTypeface.IsFocused)
			{
				lbxTypeface.ScrollIntoView(FontInfo.Typeface);
				return;
			}

			int caretIndex = txtTypeface.CaretIndex;
			string prefix = txtTypeface.Text.Substring(0, caretIndex);
			if (prefix.Length == 0)
				return;

			IEnumerable<Typeface> typefaces = from tf in FontInfo.Typefaces
											  where tf.Name().StartsWith(prefix)
											  select tf;
			if (typefaces.Count() == 0)
				return;

			Typeface typeface = typefaces.First();
			txtTypeface.Text = typeface.Name();
			txtTypeface.Select(caretIndex, txtTypeface.Text.Length - caretIndex);

			lbxTypeface.SelectedItem = typeface;
			lbxTypeface.ScrollIntoView(typeface);
		}

		/// <summary>
		/// Autocomplete for the FontSize TextBox-ListBox pair. 
		/// Handles the Changed event of the txtFontSize control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>
		/// Deselects lbxFontSize item if there is no matching value in the FontInfo.FontSizes. 
		/// Scrolls nearest lbxFontSize item into view.
		/// </remarks>
		private void txtFontSize_Changed(object sender, TextChangedEventArgs e)
		{
			if (txtFontSize.IsFocused)
			{
				int i = 0;
				for (; i < FontInfo.FontSizes.Length; i++)
				{
					if (FontInfo.FontSize <= FontInfo.FontSizes[i])
						break;
				}
				if (i < FontInfo.FontSizes.Length && FontInfo.FontSize == FontInfo.FontSizes[i])
				{
					lbxFontSize.SelectedIndex = i;
					lbxFontSize.ScrollIntoView(lbxFontSize.SelectedItem);
				}
				else
					lbxFontSize.ScrollIntoView(FontInfo.FontSizes[
						i == FontInfo.FontSizes.Length ? i - 1 : i]);
			}
		}

		/// <summary>
		/// Autocomplete for the Color TextBox-ListBox pair.
		/// Handles the Changed event of the txtColor control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
		private void txtColor_Changed(object sender, TextChangedEventArgs e)
		{
			if (!txtColor.IsFocused)
			{
				// TODO lbxColor.SelectedItem should be changed to the item from FontInfo
				lbxColor.ScrollIntoView(lbxColor.SelectedItem);
				return;
			}

			int caretIndex = txtColor.CaretIndex;
			string prefix = txtColor.Text.Substring(0, caretIndex);
			if (prefix.Length == 0)
				return;

			IEnumerable<FontInfo.NamedBrush> brushes = from b in FontInfo.NamedBrushes
											  where b.Name.StartsWith(prefix)
											  select b;
			if (brushes.Count() == 0)
				return;

			FontInfo.NamedBrush brush = brushes.First();
			txtColor.Text = brush.Name;
			txtColor.Select(caretIndex, txtColor.Text.Length - caretIndex);

			lbxColor.SelectedItem = brush;
			lbxColor.ScrollIntoView(brush);
		}

		/// <summary>
		/// Autocomplete for the DescriptionLanguage TextBox-ListBox pair.
		/// Handles the TextChanged event of the txtDescriptionLanguage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
		private void txtDescriptionLanguage_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!txtDescriptionLanguage.IsFocused)
			{
				lbxDescriptionLanguage.ScrollIntoView(FontInfo.DescriptiveTextCulture);
				return;
			}

			int caretIndex = txtDescriptionLanguage.CaretIndex;
			string prefix = txtDescriptionLanguage.Text.Substring(0, caretIndex);
			if (prefix.Length == 0)
				return;

			IEnumerable<CultureInfo> cultures = from c in FontInfo.DescriptiveTextCultures
											   where c.DisplayName.StartsWith(prefix)
											   select c;
			if (cultures.Count() == 0)
				return;

			CultureInfo culture = cultures.First();
			txtDescriptionLanguage.Text = culture.DisplayName;
			txtDescriptionLanguage.Select(caretIndex, txtDescriptionLanguage.Text.Length - caretIndex);

			lbxDescriptionLanguage.SelectedItem = culture;
			lbxDescriptionLanguage.ScrollIntoView(culture);
		}
		#endregion Autocomplete handlers

		/// <summary>
		/// Handles the Click event of the Button_OK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
