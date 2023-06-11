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

		public override void Initialize(AnalysisContext context)
		{
			if(Debugger.IsAttached)
			{
				Debugger.Launch();
			}

			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			// TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
			// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information

			context.RegisterSymbolAction(_OperationResultEnums._VerifyOperationResultDefinition, SymbolKind.NamedType);
		}
	}
}
