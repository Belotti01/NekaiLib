using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Nekai.Common.Reflection;

namespace Nekai.Common;

public class RequiredValueException
	: Exception
{
	public readonly int? MissingValuesCount;
	public readonly string[]? MissingValueMemberNames;

	public RequiredValueException([ConstantExpected] string memberName, Exception? innerException = null)
		: base($"A value assignment is required for the member \"{memberName}\".", innerException)
	{
		MissingValueMemberNames = new[] { memberName };
		MissingValuesCount = 1;
	}

	public RequiredValueException(int missingAmount, Exception? innerException = null)
		: base($"{missingAmount} members that require a value were not assigned to.", innerException)
	{
		MissingValuesCount = missingAmount;
	}

	public RequiredValueException(string[] missingMembers, Exception? innerException = null)
		: base($"{missingMembers.Length} members that require a value were not assigned to: {string.Join(", ", missingMembers)}.", innerException)
	{
		MissingValueMemberNames = missingMembers;
		MissingValuesCount = missingMembers.Length;
	}

	public static void ThrowIfRequired(MemberInfo member)
	{
		if(member.HasRequiredAttribute())
			throw new RequiredValueException(member.Name);
	}

	[Conditional("DEBUG")]
	public static void DebugThrowIfRequired(MemberInfo member)
		=> ThrowIfRequired(member);
}