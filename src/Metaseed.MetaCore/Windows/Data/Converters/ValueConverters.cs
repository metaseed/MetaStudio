// <copyright file="ValueConverters.cs" company="Oleg V. Polikarpotchkin">
// Copyright © 2009 Oleg V. Polikarpotchkin. All Right Reserved
// </copyright>
// <author>Oleg V. Polikarpotchkin</author>
// <email>ov-p@yandex.ru</email>
// <date>2009-04-28</date>
// <summary>ChooseFontDialog. Misc Value Converters.</summary>
// <revision>$Id$</revision>

using System;
using System.Windows.Data;
using System.Windows.Media;
using Metaseed.Windows.Controls;

namespace Metaseed.Windows.Data.Converters
{
	/// <summary>
	/// Converts the <see cref="Color"/> to the <see cref="FontInfo.NamedBrush"/> value.
	/// </summary>
	[ValueConversion(typeof(Color), typeof(FontInfo.NamedBrush))]
	class ColorToNamedBrushConverter : IValueConverter
	{
		#region IValueConverter Members
		/// <summary>
		/// Converts the specified Color to the NamedBrush.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="parameter">The parameter.</param>
		/// <param name="culture">The culture.</param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null && value.GetType() != typeof(Color))
				return value;
			Color color = (Color)value;
			foreach (FontInfo.NamedBrush item in FontInfo.NamedBrushes)
			{
				if ((item.Brush as SolidColorBrush).Color == color)
					return item;
			}
			return null;
		}

		/// <summary>
		/// Converts the specified NamedBrush to the Color.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="parameter">The parameter.</param>
		/// <param name="culture">The culture.</param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null && value.GetType() != typeof(FontInfo.NamedBrush))
				return value;
			return (((FontInfo.NamedBrush)value).Brush as SolidColorBrush).Color;
		}

		#endregion IValueConverter Members
	}

	/// <summary>
	/// Converts the <see cref="Typeface"/> value to the Typeface Name.
	/// </summary>
	[ValueConversion(typeof(Typeface), typeof(String))]
	class TypefaceToStringConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || value.GetType() != typeof(Typeface))
				return value;

			Typeface typeface = value as Typeface;
			string faceName = typeface.Name();
			if (faceName == null)
				return value;
			return faceName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion IValueConverter Members
	}

	/// <summary>
	/// Inverts the <c>bool</c> value.
	/// </summary>
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InvertBoolConverter : IValueConverter
	{
		#region IValueConverter Members
		/// <summary>
		/// Inverts the <c>bool</c> <paramref name="value"/> value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>Inverted <paramref name="value"/> value.</returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || value.GetType() != typeof(bool))
				return value;

			return !((bool)value);
		}

		/// <summary>
		/// Inverts the <c>bool</c> <paramref name="value"/> value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>Inverted <paramref name="value"/> value.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || value.GetType() != typeof(bool))
				return value;

			return !((bool)value);
		}

		#endregion IValueConverter Members
	}

	/// <summary>
	/// Equality of the <c>int</c> and parameter values.
	/// </summary>
	/// <remarks>
	/// Compares the input value to the value of the parameter and returns <c>true</c> 
	/// if they are equal. Both values are treated as objects of <c>Int32</c> type.
	/// </remarks>
	[ValueConversion(typeof(int), typeof(bool))]
	public class IntValueToParamEqualityConverter : IValueConverter
	{
		#region IValueConverter Members
		/// <summary>
		/// Converts the integer to bool basing on the value passed in the <paramref name="parameter"/>.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// <c>true</c> if the value of the <paramref name="value"/> is equal to the value of
		/// the <paramref name="parameter"/>; otherwise <c>false</c>.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || parameter == null)
				return value;
			try
			{
				int param = (int)System.Convert.ChangeType(parameter, typeof(int));
				int val = (int)System.Convert.ChangeType(value, typeof(int));
				return val == param;
			}
			catch { }
			return value;
		}

		/// <summary>
		/// Converts the bool value to the integer value basing on the value passed in the <paramref name="parameter"/>.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// If <paramref name="value"/> is <c>true</c> returns the <paramref name="parameter"/> value;
		/// otherwise retirns the <paramref name="value"/>.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || value.GetType() != typeof(bool) || parameter == null)
				return value;
			try
			{
				int param = (int)System.Convert.ChangeType(parameter, typeof(int));
				return (bool)value ? param : value;
			}
			catch { }
			return value;
		}

		#endregion IValueConverter Members
	}

	/// <summary>
	/// Equality of the object and parameter values.
	/// </summary>
	/// <remarks>
	/// Compares the object value to the value of the parameter and returns <c>true</c> 
	/// if they are equal.
	/// </remarks>
	[ValueConversion(typeof(Object), typeof(bool))]
	public class ValueToParamEqualityConverter : IValueConverter
	{
		#region IValueConverter Members
		/// <summary>
		/// Compares the object value to the value of the <paramref name="parameter"/> and
		/// returns the boolean result of the comparison.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// <c>true</c> if the value of the <paramref name="value"/> is equal to the value of
		/// the <paramref name="parameter"/>; otherwise <c>false</c>.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || parameter == null)
				return value;

			return value.Equals(parameter);
		}

		/// <summary>
		/// If <paramref name="value"/> is <c>true</c> returns the <paramref name="parameter"/> value;
		/// otherwise returns the <paramref name="value"/>
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// If <paramref name="value"/> is <c>true</c> returns the <paramref name="parameter"/> value;
		/// otherwise returns the <paramref name="value"/>.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || value.GetType() != typeof(bool) || parameter == null)
				return value;

			return (bool)value ? parameter : value;
		}

		#endregion IValueConverter Members
	}
}
