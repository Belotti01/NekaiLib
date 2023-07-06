using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nekai.Analyzers.Helpers;

namespace Nekai.Analyzers
{

	/// <summary>
	/// 
	/// </summary>
	public static class NekaiDiagnostics
	{
		public static ImmutableDictionary<string, NekaiDiagnostic> ById { get; }
		public static ImmutableArray<DiagnosticDescriptor> Rules 
			=> ById.Values.Select(x => x.AsRule).ToImmutableArray();

		static NekaiDiagnostics()
		{
			ById = typeof(NekaiDiagnostics)
				.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
				.Where(x => x.PropertyType == typeof(NekaiDiagnostic))
				.Select(d => (NekaiDiagnostic)d.GetValue(null))
				.ToImmutableDictionary(x => x.Code);
		}

		public static NekaiDiagnostic OperationResultWithoutAttribute { get; } = new NekaiDiagnostic(TypeKind.Enum, DiagnosticCategory.Design, DiagnosticSeverity.Warning, nameof(OperationResultWithoutAttribute));
		public static NekaiDiagnostic OperationResultBaseType { get; } = new NekaiDiagnostic(TypeKind.Enum, DiagnosticCategory.Design, DiagnosticSeverity.Error, nameof(OperationResultBaseType));
	}

	public class NekaiDiagnostic
	{	
		/// <summary> Dinamically generates the <see cref="Code"/> for each instance, so that there are no duplicate codes. </summary>
		private static readonly NekaiDiagnosticCodeGenerator _codeGenerator = new NekaiDiagnosticCodeGenerator();
		private string _ResourcePrefix { get; }

		public string Code { get; }
		public LocalizableString Title { get; }
		public LocalizableString Description { get; }
		public LocalizableString MessageFormat { get; }
		public LocalizableString CodeFixDescription { get; private set; }

		public TypeKind TargetType { get; }
		public DiagnosticSeverity Severity { get; }
		public DiagnosticCategory Category { get; }

		public bool HasCodeFix { get; private set; }
		public bool IsEnableByDefault { get; private set; } = true;
		public string[] Tags { get; private set; } = Array.Empty<string>();
		public DiagnosticDescriptor AsRule { get; }


		internal NekaiDiagnostic(TypeKind forType, DiagnosticCategory category, DiagnosticSeverity severity, string resourcePrefix)
		{
			// The same resource can be used multiple times, so no need to check for duplicate 'resourcePrefix'es.
			Code = _codeGenerator.Next(forType, category);
			
			TargetType = forType;
			Severity = severity;
			Category = category;

			_ResourcePrefix = resourcePrefix;
			Title = new LocalizableResourceString(resourcePrefix + "Title", Resources.ResourceManager, typeof(Resources));
			Description = new LocalizableResourceString(resourcePrefix + "Description", Resources.ResourceManager, typeof(Resources));
			MessageFormat = new LocalizableResourceString(resourcePrefix + "MessageFormat", Resources.ResourceManager, typeof(Resources));

			// Set to null if no resource is present.
			if(string.IsNullOrEmpty(Description.ToString()))
			{
				Description = null;
			}

			// Test for missing resources.
			Debug.Assert(!string.IsNullOrWhiteSpace(Title.ToString()), $"Missing required resource '{resourcePrefix}Title'.");
			Debug.Assert(!string.IsNullOrWhiteSpace(MessageFormat.ToString()), $"Missing required resource '{resourcePrefix}MessageFormat'.");
			
			AsRule = new DiagnosticDescriptor(
				Code,
				Title,
				MessageFormat,
				Category.ToDiagnosticClass(),
				Severity,
				IsEnableByDefault,
				description: Description,
				customTags: Tags
			);
		}

		public NekaiDiagnostic AsDisabledByDefault()
		{
			IsEnableByDefault = false;
			return this;
		}

		public NekaiDiagnostic WithTags(params string[] tags)
		{
			Tags = tags;
			return this;
		}

		public NekaiDiagnostic WithCodeFix()
		{
			HasCodeFix = true;
			CodeFixDescription = new LocalizableResourceString(_ResourcePrefix + "CodeFix", Resources.ResourceManager, typeof(Resources));

			if(string.IsNullOrEmpty(CodeFixDescription.ToString()))
			{
				// It's not required, so continue even if the resource is missing.
				CodeFixDescription = null;
			}
			return this;
		}

		public Diagnostic AsDiagnostic(Location targetLocation, params string[] messageArgs)
		{
			_DebugThrowIfMissingMessageArguments(messageArgs.Length);
			return Diagnostic.Create(AsRule, targetLocation, messageArgs);
		}

		[Conditional("DEBUG")]
		private void _DebugThrowIfMissingMessageArguments(int argsCount)
		{
			int maxArg = -1;	// -1 == no arguments required
			string format = MessageFormat.ToString().Replace("{{", "");
			for(int i = 0; i < format.Length; i++)
			{
				if(format[i] != '{')
					continue;

				int startIndex = i + 1;
				int endIndex = format.IndexOf('}', startIndex);
				if(endIndex <= 0)
					break;

				if(!int.TryParse(format.Substring(startIndex, endIndex - startIndex), out int newArg))
					continue;

				if(newArg > maxArg)
					maxArg = newArg;
			}

			if(maxArg + 1 > argsCount)
				throw new ArgumentException($"Not enough arguments were given for the formatted message of the diagnostic '{Code}' (resource '{_ResourcePrefix}MessageFormat').", "messageArgs");
		}
	}
}
