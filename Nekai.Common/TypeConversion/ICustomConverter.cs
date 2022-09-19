namespace Nekai.Common;

public interface ICustomConverter {
	Type Type { get; }

	string ConvertToString(object value);
	object? ConvertFromString(string value);
}
