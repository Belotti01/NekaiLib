using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace Nekai.Analyzers
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NekaiAnalyzersCodeFixProvider)), Shared]
	public class NekaiAnalyzersCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds
		{
			get => NekaiDiagnostics
				.ById
				.Where(x => x.Value.HasCodeFix)
				.Select(x => x.Key)
				.ToImmutableArray();
		}

		public sealed override FixAllProvider GetFixAllProvider()
		{
			// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			if(Debugger.IsAttached)
			{
				Debugger.Launch();
			} 

			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			// Register code fixes manually here.
			context.AddCodeFix<EnumDeclarationSyntax>(root, NekaiDiagnostics.OperationResultBaseType, _OperationResultEnumsFixes.SetIntegerBaseType);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableArray<Diagnostic> GetDiagnostics(CodeFixContext context, NekaiDiagnostic ofType)
		{
			return context.Diagnostics
				.Where(x => x.Id == ofType.Code)
				.ToImmutableArray();
		}
	}
}
