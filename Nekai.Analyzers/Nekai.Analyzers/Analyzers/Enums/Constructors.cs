using System;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nekai.Analyzers
{
	internal static class _Constructors
	{
		internal static void _DontThrowInConstructors(SyntaxNodeAnalysisContext context)
		{
			if(!(context.Node is ThrowStatementSyntax))
				return;
			
			if(SyntaxHelpers.IsInsideConstructor(context.Node, out var constructorSyntax))
			{
				Diagnostic diagnostic = NekaiDiagnostics.DontThrowInConstructors.AsDiagnostic(constructorSyntax.GetLocation(), constructorSyntax.GetText().ToString());
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}
