namespace Nekai.Common;

/// <summary>
/// The exit codes for the application. Multiples of 10 are more generic, so only use them if a more specific code
/// is not available.
/// </summary>
/// <remarks>
/// Error ranges:
/// <list type="bullet">
///		<item>001-009: Unknown</item>
///		<item>010-019: Configuration</item>
///		<item>020-029: Files</item>
///		<item>030-039: Network</item>
///		<item>040-049: Databases</item>
///		<item>050-059: Security</item>
///		<item>060-069: System</item>
/// </list>
/// </remarks>
public enum AppExitCode
{
	Success = 0,

	// 1 - 9 | Unknown error
	UnknownError = 1,
	ExternalError = 2,

	// 10 - 19 | Configuration errors
	ConfigurationError = 10,

	// 20 - 29 | File errors
	FileAccessError = 20,
	FileCreationError = 21,
	SerializationError = 22,
	DeserializationError = 23,
	DirectoryAccessError = 24,
	DirectoryCreationError = 25,

	// 30 - 39 | Network errors

	// 40 - 49 | Database errors

	// 50 - 59 | Security errors
	UnhautorizedOperation = 50,

	// 60 - 69 | System errors
	SystemError = 60,
	NotEnoughDiskSpace = 61,
	
	// 70 - 79 | Code errors
	CodeError = 70,
	FixedPathError = 71

}