using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Nekai.Analyzers.Helpers
{
	internal class NekaiDiagnosticCodeGenerator
	{
		private static readonly int _typeKindsCount = Enum.GetValues(typeof(TypeKind)).Length;
		private static readonly int _categoriesCount = Enum.GetValues(typeof(DiagnosticCategory)).Length;

		public const string CODE_PREFIX = "NK";
		private readonly ushort[][] _codes = new ushort[_typeKindsCount + 1][];
		private readonly ImmutableDictionary<DiagnosticCategory, char> _postFixes = new Dictionary<DiagnosticCategory, char>()
		{
			{ DiagnosticCategory.Naming, 'N' },
			{ DiagnosticCategory.Usage, 'U' },
			{ DiagnosticCategory.Design, 'D' },
			{ DiagnosticCategory.Performance, 'P' },
			{ DiagnosticCategory.Reliability, 'R' },
			{ DiagnosticCategory.Security, 'S' },
			{ DiagnosticCategory.Maintainability, 'M' },
			{ DiagnosticCategory.Documentation, 'O' },
			{ DiagnosticCategory.Localization, 'L' },
			{ DiagnosticCategory.Other, 'X' }
		}.ToImmutableDictionary();


		public NekaiDiagnosticCodeGenerator()
		{
			Debug.Assert(_postFixes.Count >= Enum.GetValues(typeof(DiagnosticCategory)).Length, "Cover all available categories.");
			Debug.Assert(_postFixes.Count == _postFixes.Values.Distinct().Count(), "Duplicate postfix found.");

			for(int i = 0; i < _codes.Length; i++)
				_codes[i] = new ushort[_categoriesCount];
		}

		public string Next(TypeKind targetType, DiagnosticCategory category)
		{
			ushort nextNumber = _NextNumber(targetType, category);
			return CODE_PREFIX + nextNumber.ToString("D3") + _postFixes[category];
		}

		public string Next(SyntaxKind targetType, DiagnosticCategory category)
		{
			ushort nextNumber = _NextNumber(targetType, category);
			return CODE_PREFIX + nextNumber.ToString("D3") + _postFixes[category];
		}

		private ushort _NextNumber(TypeKind targetType, DiagnosticCategory category)
		{
			ushort nextNumber = ++_codes[(int)targetType][(int)category];
			if(nextNumber == 100)
				throw new OutOfMemoryException($"Limit of diagnostics for type '{targetType}' of category '{category}' has been hit.");

			nextNumber += (ushort)((uint)targetType * 100);
			return nextNumber;
		}

		private ushort _NextNumber(SyntaxKind targetType, DiagnosticCategory category)
		{
			// Use a single set of codes (the last of the array) for all syntax kinds.
			ushort nextNumber = ++_codes[_typeKindsCount][(int)category];
			if(nextNumber == 100)
				throw new OutOfMemoryException($"Limit of Syntax diagnostics for category '{category}' has been hit.");

			nextNumber += (ushort)((uint)targetType * 100);
			return nextNumber;
		}
	}
}
