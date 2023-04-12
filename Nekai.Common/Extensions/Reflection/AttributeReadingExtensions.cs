using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nekai.Common.Reflection;

public static class AttributeReadingExtensions
{
	#region Single instance of generic attribute

	public static bool TryGetAttribute<TAttribute>(this MemberInfo member, [NotNullWhen(true)] out TAttribute? attribute)
		where TAttribute : Attribute
	=> member.TryGetAttribute(false, out attribute);

	public static bool TryGetAttribute<TAttribute>(this MemberInfo member, bool inherit, [NotNullWhen(true)] out TAttribute? attribute)
		where TAttribute : Attribute
	{
		attribute = member.GetCustomAttribute<TAttribute>();
		return attribute is not null;
	}

	public static bool TryGetAttribute(this MemberInfo member, Type attributeType, [NotNullWhen(true)] out object? attribute)
		=> member.TryGetAttribute(attributeType, false, out attribute);

	public static bool TryGetAttribute(this MemberInfo member, Type attributeType, bool inherit, [NotNullWhen(true)] out object? attribute)
	{
		attribute = member.GetCustomAttribute(attributeType, inherit);
		return attribute is not null;
	}

	#endregion Single instance of generic attribute

	#region Multiple instances of generic attribute

	public static bool TryGetAttributes<TAttribute>(this MemberInfo member, [NotNullWhen(true)] out IEnumerable<TAttribute>? attributes)
		where TAttribute : Attribute
	=> member.TryGetAttributes(false, out attributes);

	public static bool TryGetAttributes<TAttribute>(this MemberInfo member, bool inherit, [NotNullWhen(true)] out IEnumerable<TAttribute>? attributes)
		where TAttribute : Attribute
	{
		attributes = member.GetCustomAttributes<TAttribute>();
		return attributes is not null;
	}

	public static bool TryGetAttributes(this MemberInfo member, Type attributeType, [NotNullWhen(true)] out IEnumerable<object>? attributes)
		=> member.TryGetAttributes(attributeType, false, out attributes);

	public static bool TryGetAttributes(this MemberInfo member, Type attributeType, bool inherit, [NotNullWhen(true)] out IEnumerable<object>? attributes)
	{
		attributes = member.GetCustomAttributes(attributeType, inherit);
		return attributes is not null;
	}

	#endregion Multiple instances of generic attribute

	#region Values of specific attributes

	public static bool IsRequiredMember(this MemberInfo member)
		=> member.GetCustomAttribute<RequiredAttribute>() is not null;

	public static string? GetDisplayName(this MemberInfo member)
		=> member.GetCustomAttribute<DisplayAttribute>()?.Name;

	public static string? GetDescription(this MemberInfo member)
		=> member.GetCustomAttribute<DisplayAttribute>()?.Description;

	#endregion Values of specific attributes
}