using System;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Nekai.Analyzers
{
	internal static class _OperationResultEnums
	{
		internal static void _VerifyOperationResultDefinition(SymbolAnalysisContext context)
		{
			var symbol = (INamedTypeSymbol)context.Symbol;
			if(!_IsEnum(symbol))
				return;

			if(!_HasOperationResultAttribute(symbol))
			{
				// SomethingOperationResult enum is not decorated with the OperationResultAttribute.
				if(!symbol.Name.EndsWith("OperationResult", StringComparison.OrdinalIgnoreCase))
					return;
				Diagnostic diagnostic = NekaiDiagnostics.OperationResultWithoutAttribute.AsDiagnostic(symbol.Locations[0], symbol.Name);
				context.ReportDiagnostic(diagnostic);
			}

			// <Something>OperationResult enum is decorated with the OperationResultAttribute.
			if(symbol.EnumUnderlyingType.Name != nameof(Int32))
			{
				// OperationResult type should use base type int.
				Diagnostic diagnostic = NekaiDiagnostics.OperationResultBaseType.AsDiagnostic(symbol.Locations[0], symbol.Name, symbol.EnumUnderlyingType.Name);
				context.ReportDiagnostic(diagnostic);
				return;
			}
		}

		private static bool _IsEnum(INamedTypeSymbol symbol)
			=> symbol.TypeKind == TypeKind.Enum;

		private static bool _HasOperationResultAttribute(INamedTypeSymbol symbol)
			=> symbol.TryGetAttribute("OperationResultAttribute", out _);
	}
}
