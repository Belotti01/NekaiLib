using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Nekai.Analyzers.Helpers
{
	internal class NekaiDiagnosticCodeGenerator
	{
		public const string CODE_PREFIX = "NK";
		private readonly ushort[][] _codes = new ushort[Enum.GetValues(typeof(TypeKind)).Length][];
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
			Debug.Assert(_postFixes.Count == Enum.GetValues(typeof(DiagnosticCategory)).Length, "Cover all available categories.");
			Debug.Assert(_postFixes.Values.Count() == _postFixes.Values.Distinct().Count(), "Duplicate postfix found.");
		}

		public string Next(TypeKind targetType, DiagnosticCategory category)
		{
			ushort nextNumber = _NextNumber(targetType, category);
			return CODE_PREFIX + nextNumber.ToString("D3") + _postFixes[category];
		}

		private ushort _NextNumber(TypeKind targetType, DiagnosticCategory category)
		{
			if(_codes[(int)targetType] is null)
			{
				_codes[(int)targetType] = new ushort[Enum.GetValues(typeof(DiagnosticCategory)).Length];
			}

			ushort nextNumber = ++_codes[(int)targetType][(int)category];
			if(nextNumber == 100)
				throw new OutOfMemoryException($"Limit of diagnostics for type '{targetType}' of category '{category}' has been hit.");

			nextNumber += (ushort)((uint)targetType * 100);
			return nextNumber;
		}
	}
}
