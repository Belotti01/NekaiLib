using System.Reflection;

namespace Nekai.Common.Reflection;

public static class ValueRetrievalExtensions
{
	public static Dictionary<FieldInfo, object?> ExtractAllFieldsValues(this object obj)
		=> obj.GetType()
			.GetFields()
			.ToDictionary(x => x, x => x.GetValue(obj));

	public static Dictionary<PropertyInfo, object?> ExtractAllPropertiesValues(this object obj, bool includeWriteOnly = false)
	{
		var properties = obj.GetType()
 			.GetProperties();
		Dictionary<PropertyInfo, object?> values = [];

		foreach(var property in properties)
		{
			if(property.CanRead)
			{
				values.Add(property, property.GetValue(obj));
			}
			else if(includeWriteOnly)
			{
				values.Add(property, null);
			}
		}
		return values;
	}

	public static Dictionary<MemberInfo, object?> ExtractAllMembersValues(this object obj)
	{
		MemberInfo[] members = obj.GetType().GetMembers();
		Dictionary<MemberInfo, object?> values = [];
		foreach(var member in members)
		{
			if(member.TryGetMemberValue(obj, out object? value))
				values.Add(member, value);
		}
		return values;
	}
}