using System.Reflection;

namespace Nekai.Common.Reflection;

public static class ReflectionExtensions {
	public static void AssignField<TClass, TField>(this TClass obj, string memberName, TField value) {
		var objType = typeof(TClass);
		var field = typeof(TClass).GetField(memberName);

		if(field is null)
			throw new MissingImplementationException(objType.Name, memberName, ImplementationType.Field);

		if(!field.FieldType.IsAssignableFrom(typeof(TField)))
			throw new InvalidCastException($"Value of type \"{typeof(TField).Name}\" can't be assigned to Member \"{objType.Name}.{memberName}\" of type \"{field.FieldType.Name}\".");

		field.SetValue(obj, value);
	}

	public static void AssignProperty<TClass, TProperty>(this TClass obj, string propertyName, TProperty value) {
		var objType = typeof(TClass);
		var field = objType.GetProperty(propertyName);

		if(field is null)
			throw new MissingImplementationException(objType.Name, propertyName, ImplementationType.Property);

		if(!field.PropertyType.IsAssignableFrom(typeof(TProperty)))
			throw new InvalidCastException($"Value of type \"{typeof(TProperty).Name}\" can't be assigned to Property \"{objType.Name}.{propertyName}\" of type \"{field.PropertyType.Name}\".");

		field.SetValue(obj, value);
	}

	public static void RunMethod<TClass>(this TClass obj, string methodName, params object[] parameters) {
		var objType = typeof(TClass);
		var method = objType.GetMethod(methodName);

		if(method is null)
			throw new MissingImplementationException(objType.Name, methodName, ImplementationType.Method);

		method.Invoke(obj, parameters);
	}

	public static TResult? RunMethod<TClass, TResult>(this TClass obj, string methodName, params object[] parameters) {
		var objType = typeof(TClass);
		var method = objType.GetMethod(methodName);

		if(method is null)
			throw new MissingImplementationException(objType.Name, methodName, ImplementationType.Method);

		return (TResult?)method.Invoke(obj, parameters);
	}

	public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo member, bool inherit) where TAttribute : Attribute {
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

	public static object? GetValue(this MemberInfo member, object obj) {
		return member.MemberType switch {
			MemberTypes.Field => ((FieldInfo)member).GetValue(obj),
			MemberTypes.Property => ((PropertyInfo)member).GetValue(obj),
			_ => null
		};
	}

	public static bool TrySetValue(this MemberInfo member, object obj, object? value) 
	{
		switch(member.MemberType) {
			case MemberTypes.Field:
				((FieldInfo)member).SetValue(obj, value);
				break;
			case MemberTypes.Property:
				((PropertyInfo)member).SetValue(obj, value);
				break;
			default:
				return false;
		}
		return true;
	}

	public static void SetValue(this MemberInfo member, object obj, object? value)
	{
		switch(member.MemberType)
		{
			case MemberTypes.Field:
				((FieldInfo)member).SetValue(obj, value);
				break;
			case MemberTypes.Property:
				((PropertyInfo)member).SetValue(obj, value);
				break;
		}
	}

	public static Type GetMemberReturnType(this MemberInfo member) {
		return member.MemberType switch {
			MemberTypes.Field => ((FieldInfo)member).FieldType,
			MemberTypes.Property => ((PropertyInfo)member).PropertyType,
			MemberTypes.Method => ((MethodInfo)member).ReturnType,
			MemberTypes.Constructor => ((ConstructorInfo)member).DeclaringType,
			MemberTypes.Event => ((EventInfo)member).EventHandlerType,
			_ => null
		} ?? typeof(void);
	}
}
