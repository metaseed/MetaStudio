// <copyright file="FontInfo.cs" company="Oleg V. Polikarpotchkin">
// Copyright © 2009 Oleg V. Polikarpotchkin. All Right Reserved
// </copyright>
// <author>Oleg V. Polikarpotchkin</author>
// <email>ov-p@yandex.ru</email>
// <date>2009-04-28</date>
// <summary>ChooseFontDialog. Font Properties.</summary>
// <revision>$Id$</revision>
//http://ovpwp.wordpress.com/page/3/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Metaseed.Windows.Controls
{
	/// <summary>
	/// Font Properties.
	/// </summary>
	/// <remarks>
	/// This class contains a lot of properties describing the Font.
	/// <para>Serves as the backing Data Model for the dialog UI. The properties are bound to
	/// the dialog controls.</para>
	/// </remarks>
	public class FontInfo : ICloneable, INotifyPropertyChanged, IDataErrorInfo
	{
		/// <summary>
		/// Initializes the <see cref="FontInfo"/> class.
		/// </summary>
		static FontInfo()
		{
			namedBrushes = from pi in typeof(Brushes).GetProperties()
						   select new NamedBrush(pi.Name, pi.GetValue(null, null) as Brush);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FontInfo"/> class.
		/// </summary>
		public FontInfo() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="FontInfo"/> class with the 
		/// properties of the DependencyObject specified.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		public FontInfo(DependencyObject dependencyObject)
		{
			FontFamily = dependencyObject.GetValue(TextElement.FontFamilyProperty) as FontFamily;
			Typeface = selectBestMatchingTypeface(FontFamily
				, (FontStyle)dependencyObject.GetValue(TextElement.FontStyleProperty)
				, (FontWeight)dependencyObject.GetValue(TextElement.FontWeightProperty)
				, (FontStretch)dependencyObject.GetValue(TextElement.FontStretchProperty));
			FontSize = (double)dependencyObject.GetValue(TextElement.FontSizeProperty);
			Decorations = dependencyObject.GetValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
			SolidColorBrush brush = dependencyObject.GetValue(TextElement.ForegroundProperty) as SolidColorBrush;
			if (brush != null)
				FontColor = brush.Color;
			// Typography
			AnnotationAlternates = Typography.GetAnnotationAlternates(dependencyObject);
			Capitals = Typography.GetCapitals(dependencyObject);
			CapitalSpacing = Typography.GetCapitalSpacing(dependencyObject);
			CaseSensitiveForms = Typography.GetCaseSensitiveForms(dependencyObject);
			ContextualAlternates = Typography.GetContextualAlternates(dependencyObject);
			ContextualLigatures = Typography.GetContextualLigatures(dependencyObject);
			ContextualSwashes = Typography.GetContextualSwashes(dependencyObject);
			DiscretionaryLigatures = Typography.GetDiscretionaryLigatures(dependencyObject);
			EastAsianExpertForms = Typography.GetEastAsianExpertForms(dependencyObject);
			EastAsianLanguage = Typography.GetEastAsianLanguage(dependencyObject);
			EastAsianWidths = Typography.GetEastAsianWidths(dependencyObject);
			Fraction = Typography.GetFraction(dependencyObject);
			HistoricalForms = Typography.GetHistoricalForms(dependencyObject);
			HistoricalLigatures = Typography.GetHistoricalLigatures(dependencyObject);
			Kerning = Typography.GetKerning(dependencyObject);
			MathematicalGreek = Typography.GetMathematicalGreek(dependencyObject);
			NumeralAlignment = Typography.GetNumeralAlignment(dependencyObject);
			NumeralStyle = Typography.GetNumeralStyle(dependencyObject);
			SlashedZero = Typography.GetSlashedZero(dependencyObject);
			StandardLigatures = Typography.GetStandardLigatures(dependencyObject);
			StandardSwashes = Typography.GetStandardSwashes(dependencyObject);
			StylisticAlternates = Typography.GetStylisticAlternates(dependencyObject);
			StylisticSet1 = Typography.GetStylisticSet1(dependencyObject);
			StylisticSet2 = Typography.GetStylisticSet2(dependencyObject);
			StylisticSet3 = Typography.GetStylisticSet3(dependencyObject);
			StylisticSet4 = Typography.GetStylisticSet4(dependencyObject);
			StylisticSet5 = Typography.GetStylisticSet5(dependencyObject);
			StylisticSet6 = Typography.GetStylisticSet6(dependencyObject);
			StylisticSet7 = Typography.GetStylisticSet7(dependencyObject);
			StylisticSet8 = Typography.GetStylisticSet8(dependencyObject);
			StylisticSet9 = Typography.GetStylisticSet9(dependencyObject);
			StylisticSet10 = Typography.GetStylisticSet10(dependencyObject);
			StylisticSet11 = Typography.GetStylisticSet11(dependencyObject);
			StylisticSet12 = Typography.GetStylisticSet12(dependencyObject);
			StylisticSet13 = Typography.GetStylisticSet13(dependencyObject);
			StylisticSet14 = Typography.GetStylisticSet14(dependencyObject);
			StylisticSet15 = Typography.GetStylisticSet15(dependencyObject);
			StylisticSet16 = Typography.GetStylisticSet16(dependencyObject);
			StylisticSet17 = Typography.GetStylisticSet17(dependencyObject);
			StylisticSet18 = Typography.GetStylisticSet18(dependencyObject);
			StylisticSet19 = Typography.GetStylisticSet19(dependencyObject);
			StylisticSet20 = Typography.GetStylisticSet20(dependencyObject);
			Variants = Typography.GetVariants(dependencyObject);
		}

		#region ICloneable Implementation
		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return MemberwiseClone();
			//return new FontInfo()
			//{
			//    FontFamily = this.FontFamily,
			//    Typeface = this.Typeface,
			//    FontSize = this.FontSize,
			//    FontColor = this.FontColor,
			//    Strikethrough = this.Strikethrough,
			//    Underline = this.Underline,
			//    Overline = this.Overline,
			//    Baseline = this.Baseline,
			//	  IsFontDetailsExpanded = this.IsFontDetailsExpanded,
			//    // Typography
			//    AnnotationAlternates = this.AnnotationAlternates,
			//    Capitals = this.Capitals,
			//    CapitalSpacing = this.CapitalSpacing,
			//    CaseSensitiveForms = this.CaseSensitiveForms,
			//    ContextualAlternates = this.ContextualAlternates,
			//    ContextualLigatures = this.ContextualLigatures,
			//    ContextualSwashes = this.ContextualSwashes,
			//    DiscretionaryLigatures = this.DiscretionaryLigatures,
			//    EastAsianExpertForms = this.EastAsianExpertForms,
			//    EastAsianLanguage = this.EastAsianLanguage,
			//    EastAsianWidths = this.EastAsianWidths,
			//    Fraction = this.Fraction,
			//    HistoricalForms = this.HistoricalForms,
			//    HistoricalLigatures = this.HistoricalLigatures,
			//    Kerning = this.Kerning,
			//    MathematicalGreek = this.MathematicalGreek,
			//    NumeralAlignment = this.NumeralAlignment,
			//    NumeralStyle = this.NumeralStyle,
			//    SlashedZero = this.SlashedZero,
			//    StandardLigatures = this.StandardLigatures,
			//    StandardSwashes = this.StandardSwashes,
			//    StylisticAlternates = this.StylisticAlternates,
			//    StylisticSet1 = this.StylisticSet1,
			//    StylisticSet2 = this.StylisticSet2,
			//    StylisticSet3 = this.StylisticSet3,
			//    StylisticSet4 = this.StylisticSet4,
			//    StylisticSet5 = this.StylisticSet5,
			//    StylisticSet6 = this.StylisticSet6,
			//    StylisticSet7 = this.StylisticSet7,
			//    StylisticSet8 = this.StylisticSet8,
			//    StylisticSet9 = this.StylisticSet9,
			//    StylisticSet10 = this.StylisticSet10,
			//    StylisticSet11 = this.StylisticSet11,
			//    StylisticSet12 = this.StylisticSet12,
			//    StylisticSet13 = this.StylisticSet13,
			//    StylisticSet14 = this.StylisticSet14,
			//    StylisticSet15 = this.StylisticSet15,
			//    StylisticSet16 = this.StylisticSet16,
			//    StylisticSet17 = this.StylisticSet17,
			//    StylisticSet18 = this.StylisticSet18,
			//    StylisticSet19 = this.StylisticSet19,
			//    StylisticSet20 = this.StylisticSet20,
			//    Variants = this.Variants,
			//};
		}
		#endregion ICloneable Implementation

		/// <summary>
		/// Applies the Font properties to the DependencyObject specified.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		public void ApplyTo(DependencyObject dependencyObject)
		{
			dependencyObject.SetValue(TextElement.FontFamilyProperty, FontFamily);
			dependencyObject.SetValue(TextElement.FontStyleProperty, Typeface.Style);
			dependencyObject.SetValue(TextElement.FontWeightProperty, Typeface.Weight);
			dependencyObject.SetValue(TextElement.FontStretchProperty, Typeface.Stretch);
			dependencyObject.SetValue(TextElement.FontSizeProperty, FontSize);
			dependencyObject.SetValue(Inline.TextDecorationsProperty, Decorations);
			dependencyObject.SetValue(TextElement.ForegroundProperty,
				new SolidColorBrush(FontColor));
			// Typography
			Typography.SetAnnotationAlternates(dependencyObject, AnnotationAlternates);
			Typography.SetCapitals(dependencyObject, Capitals);
			Typography.SetCapitalSpacing(dependencyObject, CapitalSpacing);
			Typography.SetCaseSensitiveForms(dependencyObject, CaseSensitiveForms);
			Typography.SetContextualAlternates(dependencyObject, ContextualAlternates);
			Typography.SetContextualLigatures(dependencyObject, ContextualLigatures);
			Typography.SetContextualSwashes(dependencyObject, ContextualSwashes);
			Typography.SetDiscretionaryLigatures(dependencyObject, DiscretionaryLigatures);
			Typography.SetEastAsianExpertForms(dependencyObject, EastAsianExpertForms);
			Typography.SetEastAsianLanguage(dependencyObject, EastAsianLanguage);
			Typography.SetEastAsianWidths(dependencyObject, EastAsianWidths);
			Typography.SetFraction(dependencyObject, Fraction);
			Typography.SetHistoricalForms(dependencyObject, HistoricalForms);
			Typography.SetHistoricalLigatures(dependencyObject, HistoricalLigatures);
			Typography.SetKerning(dependencyObject, Kerning);
			Typography.SetMathematicalGreek(dependencyObject, MathematicalGreek);
			Typography.SetNumeralAlignment(dependencyObject, NumeralAlignment);
			Typography.SetNumeralStyle(dependencyObject, NumeralStyle);
			Typography.SetSlashedZero(dependencyObject, SlashedZero);
			Typography.SetStandardLigatures(dependencyObject, StandardLigatures);
			Typography.SetStandardSwashes(dependencyObject, StandardSwashes);
			Typography.SetStylisticAlternates(dependencyObject, StylisticAlternates);
			Typography.SetStylisticSet1(dependencyObject, StylisticSet1);
			Typography.SetStylisticSet2(dependencyObject, StylisticSet2);
			Typography.SetStylisticSet3(dependencyObject, StylisticSet3);
			Typography.SetStylisticSet4(dependencyObject, StylisticSet4);
			Typography.SetStylisticSet5(dependencyObject, StylisticSet5);
			Typography.SetStylisticSet6(dependencyObject, StylisticSet6);
			Typography.SetStylisticSet7(dependencyObject, StylisticSet7);
			Typography.SetStylisticSet8(dependencyObject, StylisticSet8);
			Typography.SetStylisticSet9(dependencyObject, StylisticSet9);
			Typography.SetStylisticSet10(dependencyObject, StylisticSet10);
			Typography.SetStylisticSet11(dependencyObject, StylisticSet11);
			Typography.SetStylisticSet12(dependencyObject, StylisticSet12);
			Typography.SetStylisticSet13(dependencyObject, StylisticSet13);
			Typography.SetStylisticSet14(dependencyObject, StylisticSet14);
			Typography.SetStylisticSet15(dependencyObject, StylisticSet15);
			Typography.SetStylisticSet16(dependencyObject, StylisticSet16);
			Typography.SetStylisticSet17(dependencyObject, StylisticSet17);
			Typography.SetStylisticSet18(dependencyObject, StylisticSet18);
			Typography.SetStylisticSet19(dependencyObject, StylisticSet19);
			Typography.SetStylisticSet20(dependencyObject, StylisticSet20);
			Typography.SetVariants(dependencyObject, Variants);
		}

		bool isFontDetailsExpanded = false;
		/// <summary>
		/// Gets or sets a value indicating whether Font Details panel is expanded in the dialog UI.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if Font Details panel is expanded in the dialog UI; otherwise, <c>false</c>.
		/// </value>
		public bool IsFontDetailsExpanded 
		{
			get { return isFontDetailsExpanded; }
			set
			{
				if (isFontDetailsExpanded != value)
				{
					isFontDetailsExpanded = value;
					NotifyPropertyChanged("IsFontDetailsExpanded");
				}
			}
		}

		#region Select Best Matching Typeface
		/// <summary>
		/// Selects the best matching typeface.
		/// </summary>
		/// <param name="family">Font family.</param>
		/// <param name="style">Font style.</param>
		/// <param name="weight">Font weight.</param>
		/// <param name="stretch">Font stretch.</param>
		/// <returns></returns>
		public static Typeface selectBestMatchingTypeface(FontFamily family, FontStyle style
			, FontWeight weight, FontStretch stretch)
		{
			ICollection<Typeface> typefaces = family.GetTypefaces();
			if (typefaces.Count == 0)
				return null;
			IEnumerable<Typeface> matchingTypefaces 
				= from tf in typefaces
				  where tf.Style == style && tf.Weight == weight && tf.Stretch == stretch
				  select tf;
			if (matchingTypefaces.Count() == 0)
				matchingTypefaces = from tf in typefaces
					  where tf.Style == style && tf.Weight == weight
					  select tf;
			if (matchingTypefaces.Count() == 0)
				matchingTypefaces = from tf in typefaces
					  where tf.Style == style
					  select tf;
			if (matchingTypefaces.Count() == 0)
				return typefaces.First();
			return matchingTypefaces.First();
		}

		/// <summary>
		/// Selects the best matching typeface.
		/// </summary>
		/// <param name="family">The family.</param>
		/// <returns></returns>
		Typeface selectBestMatchingTypeface(FontFamily family)
		{
			if (Typeface == null)
			{
				ICollection<Typeface> typefaces = family.GetTypefaces();
				if (typefaces.Count == 0)
					return null;
				return typefaces.First();
			}
			return selectBestMatchingTypeface(family, Typeface.Style, Typeface.Weight, Typeface.Stretch);
		}
		#endregion Select Best Matching Typeface

		#region Font Properties
		#region FontFamilies
		/// <summary>
		/// Gets the System Font Families collection.
		/// </summary>
		/// <value>The System Font Families.</value>
		public static IEnumerable<FontFamily> FontFamilies
		{
			get { return Fonts.SystemFontFamilies; }
		}
		#endregion FontFamilies

		#region FontFamily
		FontFamily fontFamily = new FontFamily("Lucida Console, Tahoma, Microsoft Sans Serif, Times New Roman");
		/// <summary>
		/// Gets or sets the FontFamily property.
		/// </summary>
		public FontFamily FontFamily
		{
			get { return fontFamily; }
			set
			{
				if (fontFamily != value)
				{
					Typeface newTypeface = selectBestMatchingTypeface(value);

					fontFamily = value;
					NotifyPropertyChanged("FontFamily");
					NotifyPropertyChanged("Typefaces");
					NotifyPropertyChanged("NamedTypefaces");
					Typeface = newTypeface;
				}
			}
		}
		#endregion FontFamily

		#region Typefaces
		/// <summary>
		/// Gets the <see cref="FontFamily"/> <see cref="Typeface"/> collection.
		/// </summary>
		/// <value>The typefaces.</value>
		public ICollection<Typeface> Typefaces
		{
			get { return FontFamily.GetTypefaces(); }
		}

		/// <summary>
		/// Gets the named typefaces sequence from the <see cref="Typefaces"/> collection.
		/// </summary>
		/// <value>The named typefaces.</value>
		public IEnumerable<NamedTypeface> NamedTypefaces
		{
			get { return from tf in Typefaces select new NamedTypeface(tf); }
		}
		#endregion Typefaces

		#region Typeface
		Typeface typeface;
		/// <summary>
		/// Gets or sets the typeface.
		/// </summary>
		/// <value>The typeface.</value>
		public Typeface Typeface
		{
			get { return typeface; }
			set
			{
				if (typeface != value)
				{
					typeface = value;
					NotifyPropertyChanged("Typeface");
					NotifyPropertyChanged("TypefaceName");

					descriptiveTextCultures = null;
					NotifyPropertyChanged("DescriptiveTextCultures");
				}
			}
		}

		/// <summary>
		/// Gets the name of the typeface.
		/// </summary>
		/// <value>The name of the typeface.</value>
		public string TypefaceName
		{
			get { return typeface.Name(); }
		}
		#endregion Typeface

		#region GlyphTypeface
		/// <summary>
		/// Retrieves the <see cref="GlyphTypeface"/> that corresponds to the <see cref="Typeface"/>.
		/// </summary>
		/// <value><see cref="GlyphTypeface"/> object that corresponds to this typeface, or 
		/// <c>null</c> if the typeface was constructed from a composite font.</value>
		public GlyphTypeface GlyphTypeface
		{
			get
			{
				GlyphTypeface glyphTypeface = null;
				if (Typeface != null)
					Typeface.TryGetGlyphTypeface(out glyphTypeface);
				return glyphTypeface;
			}
		}
		#endregion GlyphTypeface

		#region GlyphTypeface properties
		#region FontUri
		public Uri GlyphTypefaceFontUri
		{
			get { return GlyphTypeface == null ? null : GlyphTypeface.FontUri; }
		}
		#endregion FontUri

		#region Copyright
		string getGlyphTypefaceCopyright(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.Copyrights.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceCopyright
		{
			get { return getGlyphTypefaceCopyright(DescriptiveTextCulture); }
		}
		#endregion Copyright

		#region Description
		string getGlyphTypefaceDescription(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.Descriptions.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceDescription
		{
			get { return getGlyphTypefaceDescription(DescriptiveTextCulture); }
		}
		#endregion Description

		#region DesignerName
		string getGlyphTypefaceDesignerName(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.DesignerNames.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceDesignerName
		{
			get { return getGlyphTypefaceDesignerName(DescriptiveTextCulture); }
		}
		#endregion DesignerName

		#region DesignerUrl
		string getGlyphTypefaceDesignerUrl(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.DesignerUrls.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceDesignerUrl
		{
			get { return getGlyphTypefaceDesignerUrl(DescriptiveTextCulture); }
		}
		#endregion DesignerUrl

		#region FaceName
		string getGlyphTypefaceFaceName(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.FaceNames.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceFaceName
		{
			get { return getGlyphTypefaceFaceName(DescriptiveTextCulture); }
		}
		#endregion FaceName

		#region FamilyName
		string getGlyphTypefaceFamilyName(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.FamilyNames.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceFamilyName
		{
			get { return getGlyphTypefaceFamilyName(DescriptiveTextCulture); }
		}
		#endregion FamilyName

		#region ManufacturerName
		string getGlyphTypefaceManufacturerName(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.ManufacturerNames.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceManufacturerName
		{
			get { return getGlyphTypefaceManufacturerName(DescriptiveTextCulture); }
		}
		#endregion ManufacturerName

		#region SampleText
		string getGlyphTypefaceSampleText(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.SampleTexts.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceSampleText
		{
			get { return getGlyphTypefaceSampleText(DescriptiveTextCulture); }
		}
		#endregion SampleText

		#region Trademark
		string getGlyphTypefaceTrademark(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.Trademarks.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceTrademark
		{
			get { return getGlyphTypefaceTrademark(DescriptiveTextCulture); }
		}
		#endregion Trademark

		#region VendorUrl
		string getGlyphTypefaceVendorUrl(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.VendorUrls.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceVendorUrl
		{
			get { return getGlyphTypefaceVendorUrl(DescriptiveTextCulture); }
		}
		#endregion VendorUrl

		#region VersionString
		string getGlyphTypefaceVersionString(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.VersionStrings.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceVersionString
		{
			get { return getGlyphTypefaceVersionString(DescriptiveTextCulture); }
		}
		#endregion VersionString

		#region Win32FaceName
		string getGlyphTypefaceWin32FaceName(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.Win32FaceNames.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceWin32FaceName
		{
			get { return getGlyphTypefaceWin32FaceName(DescriptiveTextCulture); }
		}
		#endregion Win32FaceName

		#region Win32FamilyName
		string getGlyphTypefaceWin32FamilyName(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.Win32FamilyNames.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceWin32FamilyName
		{
			get { return getGlyphTypefaceWin32FamilyName(DescriptiveTextCulture); }
		}
		#endregion Win32FamilyName

		#region LicenseDescription
		string getGlyphTypefaceLicenseDescription(CultureInfo culture)
		{
			GlyphTypeface glyphTypeface = GlyphTypeface;
			if (glyphTypeface == null)
				return null;
			string str = null;
			glyphTypeface.LicenseDescriptions.TryGetValue(culture, out str);
			return str;
		}
		public string GlyphTypefaceLicenseDescription
		{
			get { return getGlyphTypefaceLicenseDescription(DescriptiveTextCulture); }
		}
		#endregion LicenseDescription
		#endregion GlyphTypeface properties

		#region DescriptiveTextCulture
		static CultureInfo defaultTextCulture = new CultureInfo("en-US");
		CultureInfo descriptiveTextCulture = defaultTextCulture;
		/// <summary>
		/// Gets or sets the descriptive text culture.
		/// </summary>
		/// <value>The descriptive text culture.</value>
		/// <remarks>
		/// The Culure used to get the Typeface Description.
		/// </remarks>
		public CultureInfo DescriptiveTextCulture
		{
			get { return descriptiveTextCulture; }
			set
			{
				if (descriptiveTextCulture != value)
				{
					descriptiveTextCulture = value;
					NotifyPropertyChanged("DescriptiveTextCulture");

					NotifyPropertyChanged("GlyphTypeface");
					NotifyPropertyChanged("GlyphTypefaceFontUri");
					NotifyPropertyChanged("GlyphTypefaceCopyright");
					NotifyPropertyChanged("GlyphTypefaceDescription");
					NotifyPropertyChanged("GlyphTypefaceDesignerName");
					NotifyPropertyChanged("GlyphTypefaceDesignerUrl");
					NotifyPropertyChanged("GlyphTypefaceFaceName");
					NotifyPropertyChanged("GlyphTypefaceFamilyName");
					NotifyPropertyChanged("GlyphTypefaceManufacturerName");
					NotifyPropertyChanged("GlyphTypefaceSampleText");
					NotifyPropertyChanged("GlyphTypefaceTrademark");
					NotifyPropertyChanged("GlyphTypefaceVendorUrl");
					NotifyPropertyChanged("GlyphTypefaceVersionString");
					NotifyPropertyChanged("GlyphTypefaceWin32FaceName");
					NotifyPropertyChanged("GlyphTypefaceWin32FamilyName");
					NotifyPropertyChanged("GlyphTypefaceLicenseDescription");
				}
			}
		}
		#endregion DescriptiveTextCulture

		#region DescriptiveTextCultures
		IEnumerable<CultureInfo> descriptiveTextCultures;
		/// <summary>
		/// Gets the list of descriptive text cultures.
		/// </summary>
		/// <value>The descriptive text cultures.</value>
		public IEnumerable<CultureInfo> DescriptiveTextCultures
		{
			get
			{
				if (descriptiveTextCultures == null)
				{
					descriptiveTextCultures = from c in CultureInfo.GetCultures(CultureTypes.AllCultures)
											  where getGlyphTypefaceCopyright(c) != null
												  | getGlyphTypefaceDescription(c) != null
												  | getGlyphTypefaceDesignerName(c) != null
												  | getGlyphTypefaceDesignerUrl(c) != null
												  | getGlyphTypefaceFaceName(c) != null
												  | getGlyphTypefaceFamilyName(c) != null
												  | getGlyphTypefaceManufacturerName(c) != null
												  | getGlyphTypefaceSampleText(c) != null
												  | getGlyphTypefaceTrademark(c) != null
												  | getGlyphTypefaceVendorUrl(c) != null
												  | getGlyphTypefaceVersionString(c) != null
												  | getGlyphTypefaceWin32FaceName(c) != null
												  | getGlyphTypefaceWin32FamilyName(c) != null
											  select c;

					if (!descriptiveTextCultures.Contains(DescriptiveTextCulture))
						DescriptiveTextCulture = defaultTextCulture;
					else
					{
						NotifyPropertyChanged("GlyphTypeface");
						NotifyPropertyChanged("GlyphTypefaceFontUri");
						NotifyPropertyChanged("GlyphTypefaceCopyright");
						NotifyPropertyChanged("GlyphTypefaceDescription");
						NotifyPropertyChanged("GlyphTypefaceDesignerName");
						NotifyPropertyChanged("GlyphTypefaceDesignerUrl");
						NotifyPropertyChanged("GlyphTypefaceFaceName");
						NotifyPropertyChanged("GlyphTypefaceFamilyName");
						NotifyPropertyChanged("GlyphTypefaceManufacturerName");
						NotifyPropertyChanged("GlyphTypefaceSampleText");
						NotifyPropertyChanged("GlyphTypefaceTrademark");
						NotifyPropertyChanged("GlyphTypefaceVendorUrl");
						NotifyPropertyChanged("GlyphTypefaceVersionString");
						NotifyPropertyChanged("GlyphTypefaceWin32FaceName");
						NotifyPropertyChanged("GlyphTypefaceWin32FamilyName");
						NotifyPropertyChanged("GlyphTypefaceLicenseDescription");
					}
				}
				return descriptiveTextCultures;
			}
		}
		#endregion DescriptiveTextCultures

		#region FontSize
		double fontSize = 8.0;
		/// <summary>
		/// Gets or sets the FontSize property.
		/// </summary>
		public double FontSize
		{
			get { return fontSize; }
			set
			{
				if (fontSize != value)
				{
					fontSize = value;
					NotifyPropertyChanged("FontSize");
					NotifyPropertyChanged("SampleFontSizes");
				}
			}
		}
		#endregion FontSize

		#region FontSizes
		private static double[] standardFontSizes = new double[] {
            3.0d,    4.0d,   5.0d,   6.0d,   6.5d,
            7.0d,    7.5d,   8.0d,   8.5d,   9.0d,
            9.5d,   10.0d,  10.5d,  11.0d,  11.5d,
            12.0d,  12.5d,  13.0d,  13.5d,  14.0d,
            15.0d,  16.0d,  17.0d,  18.0d,  19.0d,
            20.0d,  22.0d,  24.0d,  26.0d,  28.0d,  30.0d,  32.0d,  34.0d,  36.0d,  38.0d,
            40.0d,  44.0d,  48.0d,  52.0d,  56.0d,  60.0d,  64.0d,  68.0d,  72.0d,  76.0d,
            80.0d,  88.0d,  96.0d, 104.0d, 112.0d, 120.0d, 128.0d, 136.0d, 144.0d, 152.0d,
           160.0d, 176.0d, 192.0d, 208.0d, 224.0d, 240.0d, 256.0d, 272.0d, 288.0d, 304.0d,
           320.0d, 352.0d, 384.0d, 416.0d, 448.0d, 480.0d, 512.0d, 544.0d, 576.0d, 608.0d,
           640.0d
		};
		/// <summary>
		/// Gets the commonly used font sizes.
		/// </summary>
		/// <value>Commonly used font sizes.</value>
		public double[] FontSizes
		{
			get { return standardFontSizes; }
		}
		#endregion FontSizes

		#region SampleFontSizes
		/// <summary>
		/// Gets the sample font sizes collection.
		/// </summary>
		/// <value>The sample font sizes.</value>
		/// <remarks>
		/// The sequence returned containg 10 values arount the current <see cref="FontSize"/>
		/// value.
		/// </remarks>
		public IEnumerable<double> SampleFontSizes
		{
			get 
			{
				int i = 0;
				for (; i < FontSizes.Length; i++)
				{
					if (FontSize <= FontSizes[i])
						break;
				}
				if (i < 5)
					i = 0;
				else if (i + 5 > FontSizes.Length)
					i = FontSizes.Length - 10;
				else
					i -= 5;
				return FontSizes.Skip(i).Take(10);
			}
		}
		#endregion SampleFontSizes

		#region FontColor
		Color fontColor = Colors.Black;
		/// <summary>
		/// Gets or sets the FontColor property.
		/// </summary>
		public Color FontColor
		{
			get { return fontColor; }
			set
			{
				if (fontColor != value)
				{
					fontColor = value;
					NotifyPropertyChanged("FontColor");
				}
			}
		}

		/// <summary>
		/// The Brush decorated with user-readable name to show in UI.
		/// </summary>
		public struct NamedBrush
		{
			public NamedBrush(string name, Brush brush)
				: this()
			{
				Name = name;
				Brush = brush;
			}
			public string Name { get; set; }
			public Brush Brush { get; set; }
		}

		static IEnumerable<NamedBrush> namedBrushes;
		/// <summary>
		/// Gets the named brushes sequence.
		/// </summary>
		/// <value>The named brushes.</value>
		public static IEnumerable<NamedBrush> NamedBrushes
		{
			get { return namedBrushes; }
		}
		#endregion FontColor

		#region Font Decorations
		bool strikethrough;
		public bool Strikethrough
		{
			get { return strikethrough; }
			set
			{
				if (strikethrough != value)
				{
					strikethrough = value;
					NotifyPropertyChanged("Strikethrough");
					NotifyPropertyChanged("Decorations");
				}
			}
		}

		bool overline;
		public bool Overline
		{
			get { return overline; }
			set
			{
				if (overline != value)
				{
					overline = value;
					NotifyPropertyChanged("Overline");
					NotifyPropertyChanged("Decorations");
				}
			}
		}

		bool baseline;
		public bool Baseline
		{
			get { return baseline; }
			set
			{
				if (baseline != value)
				{
					baseline = value;
					NotifyPropertyChanged("Baseline");
					NotifyPropertyChanged("Decorations");
				}
			}
		}

		bool underline;
		public bool Underline
		{
			get { return underline; }
			set
			{
				if (underline != value)
				{
					underline = value;
					NotifyPropertyChanged("Underline");
					NotifyPropertyChanged("Decorations");
				}
			}
		}

		/// <summary>
		/// Gets or sets the Font Decorations collection.
		/// </summary>
		/// <value>The decorations.</value>
		/// <remarks>
		/// Used in the dialog XAML.
		/// </remarks>
		public TextDecorationCollection Decorations
		{
			get
			{
				TextDecorationCollection decorations = new TextDecorationCollection();
				if (Strikethrough)
				{
					decorations.Add(TextDecorations.Strikethrough[0]);
				}
				if (Underline)
				{
					decorations.Add(TextDecorations.Underline[0]);
				}
				if (Overline)
				{
					decorations.Add(TextDecorations.OverLine[0]);
				}
				if (Baseline)
				{
					decorations.Add(TextDecorations.Baseline[0]);
				}
				return decorations;
			}
			set
			{
				foreach (TextDecoration decoration in value)
				{
					if (decoration.Equals(TextDecorations.Strikethrough[0] as TextDecoration))
					{
						Strikethrough = true;
					}
					else if (decoration.Equals(TextDecorations.Underline[0] as TextDecoration))
					{
						Underline = true;
					}
					else if (decoration.Equals(TextDecorations.OverLine[0] as TextDecoration))
					{
						Overline = true;
					}
					else if (decoration.Equals(TextDecorations.Baseline[0] as TextDecoration))
					{
						Baseline = true;
					}
				}
			}
		}
		#endregion Font Decorations

		#region Typography
		int annotationAlternates;
		/// <summary>
		/// Gets or sets the annotation alternates.
		/// </summary>
		/// <value>The index of the alternate annotation form. The default value is 0 (zero).</value>
		/// <remarks>
		/// Annotation forms include glyphs placed in open or solid circles, squares, 
		/// parentheses, diamonds, or rounded boxes.
		/// <para>If the value of AnnotationAlternates is greater than 0 and the selected font 
		/// does not support annotation alternates, the default form of the letter is displayed.</para>
		/// </remarks>
		public int AnnotationAlternates
		{
			get { return annotationAlternates; }
			set
			{
				if (annotationAlternates != value)
				{
					annotationAlternates = value;
					NotifyPropertyChanged("AnnotationAlternates");
				}
			}
		}

		FontCapitals capitals = FontCapitals.Normal;
		/// <summary>
		/// Gets or sets a <see cref="FontCapitals"/> enumerated value that indicates the 
		/// capital form of the selected font.
		/// </summary>
		/// <value>A <see cref="FontCapitals"/> enumerated value. 
		/// The default value is <see cref="FontCapitals.Normal"/>.</value>
		public FontCapitals Capitals
		{
			get { return capitals; }
			set
			{
				if (capitals != value)
				{
					capitals = value;
					NotifyPropertyChanged("Capitals");
				}
			}
		}
		
		bool capitalSpacing;
		/// <summary>
		/// Gets or sets a value that determines whether inter-glyph spacing for 
		/// all-capital text is globally adjusted to improve readability.
		/// </summary>
		/// <value><c>true</c> if spacing is adjusted; otherwise, <c>false</c>.
		/// The default value is <c>false</c>.</value>
		public bool CapitalSpacing
		{
			get { return capitalSpacing; }
			set
			{
				if (capitalSpacing != value)
				{
					capitalSpacing = value;
					NotifyPropertyChanged("CapitalSpacing");
				}
			}
		}

		bool caseSensitiveForms;
		/// <summary>
		/// Gets or sets a value that determines whether glyphs adjust their vertical position 
		/// to better align with uppercase glyphs.
		/// </summary>
		/// <value><c>true</c> if the vertical position is adjusted; otherwise, <c>false</c>.
		/// The default value is <c>false</c>.</value>
		public bool CaseSensitiveForms
		{
			get { return caseSensitiveForms; }
			set
			{
				if (caseSensitiveForms != value)
				{
					caseSensitiveForms = value;
					NotifyPropertyChanged("CaseSensitiveForms");
				}
			}
		}

		bool contextualAlternates;
		public bool ContextualAlternates
		{
			get { return contextualAlternates; }
			set
			{
				if (contextualAlternates != value)
				{
					contextualAlternates = value;
					NotifyPropertyChanged("ContextualAlternates");
				}
			}
		}
			
		bool contextualLigatures;
		public bool ContextualLigatures
		{
			get { return contextualLigatures; }
			set
			{
				if (contextualLigatures != value)
				{
					contextualLigatures = value;
					NotifyPropertyChanged("ContextualLigatures");
				}
			}
		}

		int contextualSwashes;
		/// <summary>
		/// Gets or sets a value that specifies the index of a contextual swashes form..
		/// </summary>
		/// <value>The index of the standard swashes form. The default value is 0 (zero).</value>
		/// <remarks>
		/// Certain combinations of swash glyphs can cause an unattractive appearance, such as 
		/// overlapping descenders on adjacent letters. Using a contextual swash allows you to 
		/// use a substitute swash glyph that produces a better appearance. The following text 
		/// shows the same word before and after a contextual swash is applied.
		/// </remarks>
		public int ContextualSwashes
		{
			get { return contextualSwashes; }
			set
			{
				if (contextualSwashes != value)
				{
					contextualSwashes = value;
					NotifyPropertyChanged("ContextualSwashes");
				}
			}
		}

		bool discretionaryLigatures;
		public bool DiscretionaryLigatures
		{
			get { return discretionaryLigatures; }
			set
			{
				if (discretionaryLigatures != value)
				{
					discretionaryLigatures = value;
					NotifyPropertyChanged("DiscretionaryLigatures");
				}
			}
		}
		
		bool eastAsianExpertForms;
		/// <summary>
		/// Gets or sets a value that determines whether the standard Japanese font forms 
		/// have been replaced with the corresponding preferred typographic forms.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if standard Japanese font forms have been replaced with the corresponding preferred typographic forms; 
		/// 	otherwise, <c>false</c>. The default value is <c>false</c>.
		/// </value>
		public bool EastAsianExpertForms
		{
			get { return eastAsianExpertForms; }
			set
			{
				if (eastAsianExpertForms != value)
				{
					eastAsianExpertForms = value;
					NotifyPropertyChanged("EastAsianExpertForms");
				}
			}
		}

		FontEastAsianLanguage eastAsianLanguage = FontEastAsianLanguage.Normal;
		/// <summary>
		/// Gets or sets a <see cref="FontEastAsianLanguage"/> enumerated value that indicates 
		/// the version of glyphs to be used for a specific writing system or language.
		/// </summary>
		/// <value>A <see cref="FontEastAsianLanguage"/> enumerated value. 
		/// The default value is <see cref="FontEastAsianLanguage.Normal"/>.</value>
		public FontEastAsianLanguage EastAsianLanguage
		{
			get { return eastAsianLanguage; }
			set
			{
				if (eastAsianLanguage != value)
				{
					eastAsianLanguage = value;
					NotifyPropertyChanged("EastAsianLanguage");
				}
			}
		}

		FontEastAsianWidths eastAsianWidths = FontEastAsianWidths.Normal;
		/// <summary>
		/// Gets or sets a <see cref="FontEastAsianWidths"/> enumerated value that indicates the 
		/// proportional width to be used for Latin characters in an East Asian font.
		/// </summary>
		/// <value>A <see cref="FontEastAsianWidths"/> enumerated value. 
		/// The default value is <see cref="FontEastAsianWidths.Normal"/>.</value>
		public FontEastAsianWidths EastAsianWidths
		{
			get { return eastAsianWidths; }
			set
			{
				if (eastAsianWidths != value)
				{
					eastAsianWidths = value;
					NotifyPropertyChanged("EastAsianWidths");
				}
			}
		}

		FontFraction fraction = FontFraction.Normal;
		/// <summary>
		/// Gets or sets a <see cref="T:System.Windows.Documents.FontFraction"/> enumerated value 
		/// that indicates the fraction style.
		/// </summary>
		/// <value>A <see cref="T:System.Windows.Documents.FontFraction"/> enumerated value. 
		/// The default value is <see cref="System.Windows.Documents.FontFraction.Normal"/>.</value>
		public FontFraction Fraction
		{
			get { return fraction; }
			set
			{
				if (fraction != value)
				{
					fraction = value;
					NotifyPropertyChanged("Fraction");
				}
			}
		}

		bool historicalForms;
		public bool HistoricalForms
		{
			get { return historicalForms; }
			set
			{
				if (historicalForms != value)
				{
					historicalForms = value;
					NotifyPropertyChanged("HistoricalForms");
				}
			}
		}

		bool historicalLigatures;
		public bool HistoricalLigatures
		{
			get { return historicalLigatures; }
			set
			{
				if (historicalLigatures != value)
				{
					historicalLigatures = value;
					NotifyPropertyChanged("HistoricalLigatures");
				}
			}
		}

		bool kerning;
		public bool Kerning 
		{
			get { return kerning; }
			set
			{
				if (kerning != value)
				{
					kerning = value;
					NotifyPropertyChanged("Kerning");
				}
			}
		}
		
		bool mathematicalGreek;
		/// <summary>
		/// Gets or sets a value that indicates whether standard typographic font forms of Greek 
		/// glyphs have been replaced with corresponding font forms commonly used in mathematical 
		/// notation.
		/// </summary>
		/// <value><c>true</c> if mathematical Greek forms are enabled; otherwise, <c>false</c>.
		/// The default value is <c>false</c>.</value>
		public bool MathematicalGreek
		{
			get { return mathematicalGreek; }
			set
			{
				if (mathematicalGreek != value)
				{
					mathematicalGreek = value;
					NotifyPropertyChanged("MathematicalGreek");
				}
			}
		}

		FontNumeralAlignment numeralAlignment = FontNumeralAlignment.Normal;
		/// <summary>
		/// Gets or sets a <see cref="FontNumeralAlignment"/> enumerated value that indicates 
		/// the alighnment of widths when using numerals.
		/// </summary>
		/// <value>A <see cref="FontNumeralAlignment"/> enumerated value. 
		/// The default value is <see cref="FontNumeralAlignment.Normal"/>.</value>
		public FontNumeralAlignment NumeralAlignment
		{
			get { return numeralAlignment; }
			set
			{
				if (numeralAlignment != value)
				{
					numeralAlignment = value;
					NotifyPropertyChanged("NumeralAlignment");
				}
			}
		}

		FontNumeralStyle numeralStyle = FontNumeralStyle.Normal;
		/// <summary>
		/// Gets or sets a <see cref="FontNumeralStyle"/> enumerated value that determines 
		/// the set of glyphs that are used to render numeric alternate font forms.
		/// </summary>
		/// <value>A <see cref="FontNumeralStyle"/> enumerated value. 
		/// The default value is <see cref="FontNumeralStyle.Normal"/>.</value>
		public FontNumeralStyle NumeralStyle
		{
			get { return numeralStyle; }
			set
			{
				if (numeralStyle != value)
				{
					numeralStyle = value;
					NotifyPropertyChanged("NumeralStyle");
				}
			}
		}

		bool slashedZero;
		/// <summary>
		/// Gets or sets a value that indicates whether a nominal zero font form should be 
		/// replaced with a slashed zero.
		/// </summary>
		/// <value><c>true</c> if slashed zero forms are enabled; otherwise, <c>false</c>.
		/// The default value is <c>false</c>.</value>
		public bool SlashedZero
		{
			get { return slashedZero; }
			set
			{
				if (slashedZero != value)
				{
					slashedZero = value;
					NotifyPropertyChanged("SlashedZero");
				}
			}
		}

		bool standardLigatures;
		public bool StandardLigatures 
		{
			get { return standardLigatures; }
			set
			{
				if (standardLigatures != value)
				{
					standardLigatures = value;
					NotifyPropertyChanged("StandardLigatures");
				}
			}
		}

		int standardSwashes;
		/// <summary>
		/// Gets or sets a value that specifies the index of a standard swashes form.
		/// </summary>
		/// <value>The index of the standard swashes form. The default value is 0 (zero).</value>
		public int StandardSwashes
		{
			get { return standardSwashes; }
			set
			{
				if (standardSwashes != value)
				{
					standardSwashes = value;
					NotifyPropertyChanged("StandardSwashes");
				}
			}
		}

		int stylisticAlternates;
		/// <summary>
		/// Gets or sets a value that specifies the index of a stylistic alternates form.
		/// </summary>
		/// <value>The index of the stylistic alternates form. The default value is 0 (zero).</value>
		public int StylisticAlternates
		{
			get { return stylisticAlternates; }
			set
			{
				if (stylisticAlternates != value)
				{
					stylisticAlternates = value;
					NotifyPropertyChanged("StylisticAlternates");
				}
			}
		}

		bool stylisticSet1;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet1
		{
			get { return stylisticSet1; }
			set
			{
				if (stylisticSet1 != value)
				{
					stylisticSet1 = value;
					NotifyPropertyChanged("StylisticSet1");
				}
			}
		}

		bool stylisticSet2;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet2
		{
			get { return stylisticSet2; }
			set
			{
				if (stylisticSet2 != value)
				{
					stylisticSet2 = value;
					NotifyPropertyChanged("StylisticSet2");
				}
			}
		}

		bool stylisticSet3;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet3
		{
			get { return stylisticSet3; }
			set
			{
				if (stylisticSet3 != value)
				{
					stylisticSet3 = value;
					NotifyPropertyChanged("StylisticSet3");
				}
			}
		}

		bool stylisticSet4;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet4
		{
			get { return stylisticSet4; }
			set
			{
				if (stylisticSet4 != value)
				{
					stylisticSet4 = value;
					NotifyPropertyChanged("StylisticSet4");
				}
			}
		}

		bool stylisticSet5;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet5
		{
			get { return stylisticSet5; }
			set
			{
				if (stylisticSet5 != value)
				{
					stylisticSet5 = value;
					NotifyPropertyChanged("StylisticSet5");
				}
			}
		}

		bool stylisticSet6;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet6
		{
			get { return stylisticSet6; }
			set
			{
				if (stylisticSet6 != value)
				{
					stylisticSet6 = value;
					NotifyPropertyChanged("StylisticSet6");
				}
			}
		}

		bool stylisticSet7;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet7
		{
			get { return stylisticSet7; }
			set
			{
				if (stylisticSet7 != value)
				{
					stylisticSet7 = value;
					NotifyPropertyChanged("StylisticSet7");
				}
			}
		}

		bool stylisticSet8;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet8
		{
			get { return stylisticSet8; }
			set
			{
				if (stylisticSet8 != value)
				{
					stylisticSet8 = value;
					NotifyPropertyChanged("StylisticSet8");
				}
			}
		}

		bool stylisticSet9;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet9
		{
			get { return stylisticSet9; }
			set
			{
				if (stylisticSet9 != value)
				{
					stylisticSet9 = value;
					NotifyPropertyChanged("StylisticSet9");
				}
			}
		}

		bool stylisticSet10;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet10
		{
			get { return stylisticSet10; }
			set
			{
				if (stylisticSet10 != value)
				{
					stylisticSet10 = value;
					NotifyPropertyChanged("StylisticSet10");
				}
			}
		}

		bool stylisticSet11;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet11
		{
			get { return stylisticSet11; }
			set
			{
				if (stylisticSet11 != value)
				{
					stylisticSet11 = value;
					NotifyPropertyChanged("StylisticSet11");
				}
			}
		}

		bool stylisticSet12;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet12
		{
			get { return stylisticSet12; }
			set
			{
				if (stylisticSet12 != value)
				{
					stylisticSet12 = value;
					NotifyPropertyChanged("StylisticSet12");
				}
			}
		}

		bool stylisticSet13;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet13
		{
			get { return stylisticSet13; }
			set
			{
				if (stylisticSet13 != value)
				{
					stylisticSet13 = value;
					NotifyPropertyChanged("StylisticSet13");
				}
			}
		}

		bool stylisticSet14;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet14
		{
			get { return stylisticSet14; }
			set
			{
				if (stylisticSet14 != value)
				{
					stylisticSet14 = value;
					NotifyPropertyChanged("StylisticSet14");
				}
			}
		}

		bool stylisticSet15;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet15
		{
			get { return stylisticSet15; }
			set
			{
				if (stylisticSet15 != value)
				{
					stylisticSet15 = value;
					NotifyPropertyChanged("StylisticSet15");
				}
			}
		}

		bool stylisticSet16;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet16
		{
			get { return stylisticSet16; }
			set
			{
				if (stylisticSet16 != value)
				{
					stylisticSet16 = value;
					NotifyPropertyChanged("StylisticSet16");
				}
			}
		}

		bool stylisticSet17;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet17
		{
			get { return stylisticSet17; }
			set
			{
				if (stylisticSet17 != value)
				{
					stylisticSet17 = value;
					NotifyPropertyChanged("StylisticSet17");
				}
			}
		}

		bool stylisticSet18;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet18
		{
			get { return stylisticSet18; }
			set
			{
				if (stylisticSet18 != value)
				{
					stylisticSet18 = value;
					NotifyPropertyChanged("StylisticSet18");
				}
			}
		}

		bool stylisticSet19;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet19
		{
			get { return stylisticSet19; }
			set
			{
				if (stylisticSet19 != value)
				{
					stylisticSet19 = value;
					NotifyPropertyChanged("StylisticSet19");
				}
			}
		}

		bool stylisticSet20;
		/// <summary>
		/// Gets or sets a value that indicates whether a stylistic set of a font form is enabled.
		/// </summary>
		/// <value><c>true</c> if if the stylistic set of the font form is enabled;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool StylisticSet20
		{
			get { return stylisticSet20; }
			set
			{
				if (stylisticSet20 != value)
				{
					stylisticSet20 = value;
					NotifyPropertyChanged("StylisticSet20");
				}
			}
		}

		FontVariants variants = FontVariants.Normal;
		/// <summary>
		/// Gets or sets a <see cref="FontVariants"/> enumerated value that indicates a variation 
		/// of the standard typographic form to be used.
		/// </summary>
		/// <value>A <see cref="FontVariants"/> enumerated value. The default value is <see cref="FontVariants.Normal"/>.</value>
		public FontVariants Variants
		{
			get { return variants; }
			set
			{
				if (variants != value)
				{
					variants = value;
					NotifyPropertyChanged("Variants");
				}
			}
		}
		#endregion Typography
		#endregion Font Properties

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;

		void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion INotifyPropertyChanged Members

		#region IDataErrorInfo Members
		string IDataErrorInfo.Error
		{
			get { return null; }
		}

		protected virtual string GetDataErrorInfo(string columnName)
		{
			switch (columnName)
			{
				case "FontSize":
					if (FontSize <= 0)
						return "FontSize must be positive";
					break;
			}
			return null;
		}

		string IDataErrorInfo.this[string columnName]
		{
			get
			{
				return GetDataErrorInfo(columnName);
			}
		}
		#endregion IDataErrorInfo Members
	}

	/// <summary>
	/// Typeface with the Name.
	/// </summary>
	/// <remarks>
	/// The sole purpouse of this class is to provide the Name property to the 
	/// <see cref="Typeface"/> object.
	/// <para>The name is aquired by the Typeface.Name() extension method.</para>
	/// </remarks>
	public class NamedTypeface
	{
		public NamedTypeface(Typeface typeface)
		{
			Typeface = typeface;
		}

		public Typeface Typeface { get; private set; }
		public string Name { get { return Typeface.Name(); } }
	}
}
