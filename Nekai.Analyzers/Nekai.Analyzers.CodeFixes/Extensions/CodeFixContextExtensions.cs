using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nekai.Analyzers
{
	public static class CodeFixContextExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableArray<Diagnostic> GetDiagnostics(this ref CodeFixContext context, NekaiDiagnostic ofType)
			=> context.Diagnostics
				.Where(x => x.Id == ofType.Code)
				.ToImmutableArray();

		public static void AddCodeFix<T>(this ref CodeFixContext context, SyntaxNode root, NekaiDiagnostic diagnosticType, Func<Document, T, CancellationToken, Task<Document>> codeFix)
			where T : SyntaxNode
		{
			var diagnostics = context.GetDiagnostics(diagnosticType);

			Document doc = context.Document;
			foreach(var diagnostic in diagnostics)
			{
				T nodeDeclaration = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<T>();
				CodeAction action = CodeAction.Create(diagnosticType.CodeFixDescription.ToString(), token => codeFix(doc, nodeDeclaration, token));
				context.RegisterCodeFix(action, diagnostics);
			}
		}
	}
}
