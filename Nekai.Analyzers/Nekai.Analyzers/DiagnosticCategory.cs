using System;
using System.Collections.Generic;
using System.Text;

namespace Nekai.Analyzers
{
	public enum DiagnosticCategory
	{
		Naming,
		Usage,
		Design,
		Performance,
		Reliability,
		Security,
		Maintainability,
		Documentation,
		Localization,
		Other
	}

	public static class DiagnosticCategoryExtensions
	{
		public static string ToDiagnosticClass(this DiagnosticCategory category)
		{
			switch(category)
			{
				case DiagnosticCategory.Naming:
					return "Naming";
				case DiagnosticCategory.Usage:
					return "Usage";
				case DiagnosticCategory.Design:
					return "Design";
				case DiagnosticCategory.Performance:
					return "Performance";
				case DiagnosticCategory.Reliability:
					return "Reliability";
				case DiagnosticCategory.Security:
					return "Security";
				case DiagnosticCategory.Maintainability:
					return "Maintainability";
				case DiagnosticCategory.Documentation:
					return "Documentation";
				case DiagnosticCategory.Localization:
					return "Localization";
				case DiagnosticCategory.Other:
					return "Other";
				default:
					throw new ArgumentOutOfRangeException(nameof(category), category, null);
			};
		}
	}	
}
