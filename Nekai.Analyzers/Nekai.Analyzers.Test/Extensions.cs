using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;

namespace Nekai.Analyzers.Test;
public static class Extensions
{
	public static DiagnosticResult AsResult(this NekaiDiagnostic diagnostic)
	{
		return new DiagnosticResult(diagnostic.AsRule);
	}

	public static DiagnosticResult WithLocationOf(this DiagnosticResult result, string text, string textInLine, string errorText)
	{
		var lines = text.Split('\n');
		int line = Array.FindIndex(lines, x => x.Contains(textInLine));
		int column = lines[line].IndexOf(errorText);

		if(line == -1)
			throw new ArgumentException($"Text '{textInLine}' not found in text:\n{text}");
		if(column == -1) 
			throw new ArgumentException($"Text '{errorText}' not found in line:\n{lines[line]}");
		return result.WithLocation(line + 1, column + 1);
	}
}
