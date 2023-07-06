using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Nekai.Analyzers
{
	public static class NamedTypeSymbolExtensions
	{
		public static bool TryGetAttributes(this INamedTypeSymbol symbol, string attributeName, out AttributeData[] attributes)
		{
			attributes = symbol.GetAttributes()
				.Where(x => x.AttributeClass.Name == attributeName)
				.ToArray();
			return attributes.Length != 0;
		}

		public static bool TryGetAttribute(this INamedTypeSymbol symbol, string attributeName, out AttributeData attribute)
		{
			attribute = symbol.GetAttributes()
				.Where(x => x.AttributeClass.Name == attributeName)
				.FirstOrDefault();
			return !(attribute is null);
		}
	}
}
