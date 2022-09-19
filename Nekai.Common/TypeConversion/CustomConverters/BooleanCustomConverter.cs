namespace Nekai.Common;

internal class BooleanCustomConverter : ICustomConverter {
	private const string _DEFAULT_TRUE_VALUE = "True";
	private const string _DEFAULT_FALSE_VALUE = "False";

	private readonly string[] _trueValues = new[] {
		"TRUE", "YES", "Y", "T", "ENABLE", "ENABLED", "ACTIVE", ""
	};

	public Type Type => typeof(bool);

	public string ConvertToString(object value) {
		return value is bool b && b
			? _DEFAULT_TRUE_VALUE
			: _DEFAULT_FALSE_VALUE;
	}

	object? ICustomConverter.ConvertFromString(string value) {
		if(value.IsNumeric())	// For numbers, only return false if 0
			return decimal.Parse(value) != 0;
		
		return _trueValues.Contains(value.ToUpper());
	}
}
