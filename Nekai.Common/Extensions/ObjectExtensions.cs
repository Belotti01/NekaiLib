namespace Nekai.Common;

public static class ObjectExtensions
{
	/// <summary>
	/// Returns the result of <see cref="object.ToString"/> if the object is not null and no error occurs; otherwise, returns
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
}
