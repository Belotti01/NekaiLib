using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace Nekai.Analyzers
{
	public static class SyntaxHelpers
	{

		public static bool IsInsideConstructor(SyntaxNode node, out ConstructorDeclarationSyntax constructorSyntax)
		{
			constructorSyntax = null;

			while(!(node.Parent is null))
			{
				if(node.Parent is ConstructorDeclarationSyntax)
				{
					constructorSyntax = (ConstructorDeclarationSyntax)node.Parent;
					return true;
				}

				if(node.Parent is MethodDeclarationSyntax)
					return false;   // No need to go further up - we're outside of a constructor.

				node = node.Parent;
			}

			return false;
		}
	}
}
