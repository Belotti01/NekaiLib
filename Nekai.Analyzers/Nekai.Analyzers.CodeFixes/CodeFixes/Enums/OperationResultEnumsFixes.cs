using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace Nekai.Analyzers
{
	public static class _OperationResultEnumsFixes
	{
		public static async Task<Document> SetIntegerBaseType(Document document, EnumDeclarationSyntax syntax, CancellationToken cancellationToken)
		{
			SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

			var baseList = syntax.BaseList;

			var list = SyntaxFactory.SeparatedList<BaseTypeSyntax>();
			var newBaseList = baseList.WithTypes(list);
			var newSyntax = syntax.WithBaseList(newBaseList);
			var newRoot = root.ReplaceNode(syntax, newSyntax);

			return document.WithSyntaxRoot(newRoot);
		}
	}
}
