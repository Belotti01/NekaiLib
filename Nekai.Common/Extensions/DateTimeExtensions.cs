using System.Globalization;

namespace Nekai.Common.Extensions;

public static class DateTimeExtensions
{
	/// <summary>
	/// Returns the string representation of the <see cref="DateTime"/> in universal sortable format, as defined by ISO 8601.
	/// </summary>
	/// <param name="dt"> The <see cref="DateTime"/> to convert. </param>
	public static string ToUniversalSortableString(this DateTime dt)
	{
		var format = CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern;
		return dt.ToString(format);
	}
	
	/// <summary>
	/// Returns the string representation of the <see cref="TimeSpan"/> in universal sortable format, as defined by ISO 8601.
	/// </summary>
	/// <param name="dt"> The <see cref="TimeSpan"/> to convert. </param>
	public static string ToUniversalSortableString(this TimeSpan dt)
	{
		var format = CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern;
		return dt.ToString(format);
	}
	
	/// <summary>
	/// Returns the string representation of the <see cref="DateTimeOffset"/> in universal sortable format, as defined by ISO 8601.
	/// </summary>
	/// <param name="dt"> The <see cref="DateTimeOffset"/> to convert. </param>
	public static string ToUniversalSortableString(this DateTimeOffset dt)
	{
		var format = CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern;
		return dt.ToString(format);
	}
}