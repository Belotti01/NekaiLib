using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Diagnostics;

// Using a different namespace to keep the autocomplete cleaner when Reflection is not necessary,
// since most methods here are appliable to any or most Types.
namespace Nekai.Common.Reflection;

public static class ReflectionExtensions
{
	public static void AssignField<TClass, TField>(this TClass obj, string memberName, TField value)
	{
		var objType = typeof(TClass);
		var field = typeof(TClass).GetField(memberName);

		if(field is null)
			throw new MissingImplementationException(objType.Name, memberName, ImplementationType.Field);

		if(!field.FieldType.IsAssignableFrom(typeof(TField)))
			throw new InvalidCastException($"Value of type \"{typeof(TField).Name}\" can't be assigned to Member \"{objType.Name}.{memberName}\" of type \"{field.FieldType.Name}\".");

		field.SetValue(obj, value);
	}

	public static void AssignProperty<TClass, TProperty>(this TClass obj, string propertyName, TProperty value)
	{
		var objType = typeof(TClass);
		var field = objType.GetProperty(propertyName);

		if(field is null)
			throw new MissingImplementationException(objType.Name, propertyName, ImplementationType.Property);

		if(!field.PropertyType.IsAssignableFrom(typeof(TProperty)))
			throw new InvalidCastException($"Value of type \"{typeof(TProperty).Name}\" can't be assigned to Property \"{objType.Name}.{propertyName}\" of type \"{field.PropertyType.Name}\".");

		field.SetValue(obj, value);
	}

	public static void RunMethod<TClass>(this TClass obj, string methodName, params object[] parameters)
	{
		var objType = typeof(TClass);
		var method = objType.GetMethod(methodName);

		if(method is null)
			throw new MissingImplementationException(objType.Name, methodName, ImplementationType.Method);

		method.Invoke(obj, parameters);
	}

	public static TResult? RunMethod<TClass, TResult>(this TClass obj, string methodName, params object[] parameters)
	{
		var objType = typeof(TClass);
		var method = objType.GetMethod(methodName);

		if(method is null)
			throw new MissingImplementationException(objType.Name, methodName, ImplementationType.Method);

		return (TResult?)method.Invoke(obj, parameters);
	}

	public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo member, bool inherit) where TAttribute : Attribute
	{
		return member
			.GetCustomAttributes(typeof(TAttribute), inherit)
			.Cast<TAttribute>();
	}

	public static IEnumerable<MemberInfo> GetMembersWithAttribute<TAttribute>(this Type type, bool inherit = true) where TAttribute : Attribute
	{
		return type
			.GetMembers()
			.Where(x => x.GetCustomAttribute<TAttribute>(inherit) is not null);
	}

	public static IEnumerable<MemberInfo> GetMembersWithAttribute<TAttribute>(this Type type, BindingFlags bindingAttr, bool inherit = true) where TAttribute : Attribute
	{
		return type
			.GetMembers(bindingAttr)
			.Where(x => x.GetCustomAttribute<TAttribute>(inherit) is not null);
	}

	// Not named "GetValue" to disambiguate from the .NET FieldInfo and PropertyInfo methods with the same name
	public static object? GetMemberValue(this MemberInfo member, object source)
	{
		return member.MemberType switch
		{
			MemberTypes.Field => (member as FieldInfo)?.GetValue(source),
			// For properties, first make sure that the property has a getter method, and return null otherwise
			MemberTypes.Property => (member as PropertyInfo)?.GetGetMethod(true)?.Invoke(source, null),
			_ => null
		};
	}

	public static bool TryGetMemberValue(this MemberInfo member, object source, out object? value)
	{
		// Do not simply use the GetValue method, since null can mean both that the member value can't be retrieved,
		// or that the value of the member is null
		value = null;
		// Pattern-matching to the correct member type might be redundant, but it's safer than simply casting the member
		switch(member.MemberType)
		{
			case MemberTypes.Field:
				if(member is not FieldInfo field)
					return false;
				value = field.GetValue(source);
				return true;

			case MemberTypes.Property:
				if(member is not PropertyInfo property || !property.CanRead)
					return false;
				value = property.GetValue(source, null);
				return true;

			default:
				return false;
		}
	}

	public static bool TrySetValue(this MemberInfo member, object obj, object? value)
	{
		switch(member.MemberType)
		{
			case MemberTypes.Field:
				var field = (FieldInfo)member;
				field.SetValue(obj, value);
				break;

			case MemberTypes.Property:
				var property = ((PropertyInfo)member);
				if(property.SetMethod is null)
					return false;
				property.SetValue(obj, value);
				break;

			default:
				Debug.WriteLine("Member type not supported for assignment using reflection ({0} \"{1}\" of {2}).", member.MemberType, member.Name, member.DeclaringType?.Name ?? "<Unknown Source>");
				return false;
		}
		return true;
	}

	public static void SetValue(this MemberInfo member, object obj, object? value)
	{
		switch(member.MemberType)
		{
			case MemberTypes.Field:
				var field = (FieldInfo)member;
				field.SetValue(obj, value);
				break;

			case MemberTypes.Property:
				var property = ((PropertyInfo)member);
				property.SetValue(obj, value);
				break;

			default:
				throw new NotSupportedException($"Member type not supported for assignment using reflection ({member.MemberType} \"{member.Name}\" of {member.DeclaringType?.Name ?? "<Unknown Source>"}).");
		}
	}

	public static Type GetMemberReturnType(this MemberInfo member)
	{
		return member.MemberType switch
		{
			MemberTypes.Field => ((FieldInfo)member).FieldType,
			MemberTypes.Property => ((PropertyInfo)member).PropertyType,
			MemberTypes.Method => ((MethodInfo)member).ReturnType,
			MemberTypes.Constructor => ((ConstructorInfo)member).DeclaringType,
			MemberTypes.Event => ((EventInfo)member).EventHandlerType,
			_ => throw new NotSupportedException($"Member type not supported for return type retrieval using reflection ({member.MemberType} \"{member.Name}\" of {member.DeclaringType?.Name ?? "<Unknown Source>"}).")
		} ?? typeof(void);
	}

	public static void CopyMembersInto<T>(this T source, T target)
	{
		if(source is null)
			ThrowHelper.ThrowArgumentNullException(nameof(source));
		if(target is null)
			ThrowHelper.ThrowArgumentNullException(nameof(target));
		
		var members = typeof(T).GetMembers();

		foreach(var member in members)
		{
			if(!member.TryGetMemberValue(source, out var value))
				continue;	// Inaccessible member.

			member.TrySetValue(target, value);
		}
	}
}