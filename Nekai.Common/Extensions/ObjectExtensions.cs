using DotNext.Runtime;

namespace Nekai.Common;

public static class ObjectExtensions
{
	/// <summary>
	/// Returns the result of <see cref="object.ToString"/> if the object is not <see langword="null"/> and no error occurs; otherwise, returns
	/// <see langword="null"/>.
	/// </summary>
	/// <param name="obj"> The object to convert. </param>
	public static string? ToStringOrNull(this object? obj)
	{
		if(obj is null)
			return null;

		try
		{
			return obj.ToString();
		}
		catch { }

		return null;
	}

	/// <inheritdoc cref="Intrinsics.IsDefault{T}"/>
	public static bool IsDefault<T>(this T value)
		=>  Intrinsics.IsDefault(value);
}