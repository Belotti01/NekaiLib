using System.Reflection;

namespace Nekai.Interactivity;

public class MissingAttributeException : Exception
{
	public Type AttributeType { get; protected set; }
	public Type? ThrowingType { get; protected set; }
	public MemberInfo? ThrowingMember { get; protected set; }

	public MissingAttributeException(Type attributeType, Type throwingType)
		: base($"Type \"{throwingType.Name}\" requires an attribute of type [{attributeType.Name}].")
	{
		AttributeType = attributeType;
		ThrowingType = throwingType;
	}

	public MissingAttributeException(Type attributeType, MemberInfo throwingMember)
		: base($"Member \"{throwingMember.Name}\" of Type \"{throwingMember.DeclaringType?.Name ?? "<Unknown>"}\" requires an attribute of type [{attributeType.Name}].")
	{
		AttributeType = attributeType;
		ThrowingMember = throwingMember;
	}
}