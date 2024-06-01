using System.Reflection;
using Nekai.Common.Reflection;

namespace Nekai.Common;

public static partial class NekaiData
{
	/// <summary>
	/// Provides methods to help with non-automated testing.
	/// </summary>
	public static class ManualTesting
	{
		/// <summary>
		/// Retrieve all data contained by an object and format it to be displayable.
		/// </summary>
		/// <param name="obj">The object to extract the data from.</param>
		/// <returns>A <see langword="string"/> containing a list, split between fields and properties, with the name
		/// and values (converted to string) of the members.</returns>
		public static string ExtractObjectData(object? obj)
		{
			if(obj is null)
				return "null";
			string lineFormat = "\t{0} {1}: {2}";
			var fields = obj.ExtractAllFieldsValues().ToSortedDictionary(Comparer<FieldInfo>.Create((x, y) => x.Name.CompareTo(y.Name)));
			var properties = obj.ExtractAllPropertiesValues().ToSortedDictionary(Comparer<PropertyInfo>.Create((x, y) => x.Name.CompareTo(y.Name)));

			var sb = new StringBuilder();
			if(fields.Count > 0)
			{
				string? valueString;
				sb.AppendLine("Fields:");
				foreach(var field in fields)
				{
					valueString = field.Value?.ToString()?.ReplaceLineEndings($"{Environment.NewLine}\t");
					_ = sb.Append(string.Format(lineFormat, field.Key.GetMemberReturnType().Name, field.Key.Name, valueString));
				}
			}

			if(properties.Count > 0)
			{
				sb.AppendLine("Properties:");
				foreach(var property in properties)
				{
					_ = sb.Append(string.Format(lineFormat, property.Key.GetMemberReturnType().Name, property.Key.Name, property.Value));
				}
			}

			if(sb.Length > 0)
			{
				_ = sb.Remove(sb.Length - 1, 1);
			}

			return sb.ToString();
		}
	}
}