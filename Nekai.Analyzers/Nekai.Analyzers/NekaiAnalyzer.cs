using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Nekai.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NekaiAnalyzer : DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get => NekaiDiagnostics.Rules;
		}

#if DEBUG
		static NekaiAnalyzer()
		{
			try
			{
				//Debugger.Launch();
			}
			catch { }
		}
#endif

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();

			// TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
			// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information

			context.RegisterSymbolAction(_OperationResultEnums._VerifyOperationResultDefinition, SymbolKind.NamedType);
			context.RegisterSyntaxNodeAction(_Constructors._DontThrowInConstructors, SyntaxKind.ThrowKeyword);
		}
	}
}
