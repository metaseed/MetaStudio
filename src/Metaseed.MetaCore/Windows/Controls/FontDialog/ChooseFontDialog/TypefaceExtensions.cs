// <copyright file="TypefaceExtensions.cs" company="Oleg V. Polikarpotchkin">
// Copyright © 2009 Oleg V. Polikarpotchkin. All Right Reserved
// </copyright>
// <author>Oleg V. Polikarpotchkin</author>
// <email>ov-p@yandex.ru</email>
// <date>2009-04-28</date>
// <summary>ChooseFontDialog. Extends the Typeface type.</summary>
// <revision>$Id$</revision>

using System.Collections.Generic;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Media;

namespace Metaseed.Windows.Controls
{
	/// <summary>
	/// Extends the <see cref="Typeface"/> type.
	/// </summary>
	public static class TypefaceExtensions
	{
		/// <summary>
		/// Gets the typeface name.
		/// </summary>
		/// <param name="typeface">The typeface.</param>
		/// <returns></returns>
		/// <remarks>
		/// Gets the typeface name for either the current Culture, en-us culture or the
		/// first FaceName available, in that order.
		/// <para>Note: When new APIs are available to obtain the language-specific face name
		/// from the font this code will need to be replaced with calls to those new APIs.</para>
		/// </remarks>
		public static string Name(this Typeface typeface)
		{
			if (typeface == null)
				return null;
			IDictionary<XmlLanguage, string> faceNames = typeface.FaceNames;
			if (faceNames.Count == 0)
				return null;
			string faceName = null;
			if (!faceNames.TryGetValue(XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag), out faceName)
				&& !faceNames.TryGetValue(XmlLanguage.GetLanguage("en-us"), out faceName))
			{   // The typeface doesn't have a FaceName neither for the CurrentUICulture 
				// nor for the "en-us" culture.
				// Get the first FaceName available.
				foreach (KeyValuePair<XmlLanguage, string> pair in faceNames)
				{
					faceName = pair.Value;
					break;
				}
			}
			return faceName;
		}
	}
}
