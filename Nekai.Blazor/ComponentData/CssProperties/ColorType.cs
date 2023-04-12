using System.Collections.Immutable;
using System.Diagnostics;

namespace Nekai.Blazor.Components;

public enum ColorType
{
	Default,
	Primary,
	Secondary,
	Success,
	Danger,
	Warning,
	Info,
	Light,
	Dark,
	Body,
	White,
	/// <summary> Only affects text. </summary>
	Muted,
	/// <summary> Only affects background. </summary>
	Transparent
}

public static class ColorTypeExtensions
{
	public static bool IsDarkBackgroundColor(this ColorType color)
	{
		return color 
			is ColorType.Primary 
			or ColorType.Secondary 
			or ColorType.Success
			or ColorType.Danger
			or ColorType.Dark;
	}

	public static bool IsDarkButtonColor(this ColorType color)
	{
		return color
			is ColorType.Primary
			or ColorType.Secondary
			or ColorType.Success
			or ColorType.Danger
			or ColorType.Info
			or ColorType.Dark;
	}

	public static ReadOnlySpan<char> ToBackgroundCssClass(this ColorType color)
	{
		return color switch
		{
			ColorType.Default => default,
			ColorType.Primary => "bg-primary",
			ColorType.Secondary => "bg-secondary",
			ColorType.Success => "bg-success",
			ColorType.Danger => "bg-danger",
			ColorType.Warning => "bg-warning",
			ColorType.Info => "bg-info",
			ColorType.Light => "bg-light",
			ColorType.Dark => "bg-dark",
			ColorType.Body => "bg-body",
			ColorType.White => "bg-white",
			ColorType.Transparent => "bg-transparent",
			ColorType.Muted => default,
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(color), color, null)
				: default
		};
	}

	public static ReadOnlySpan<char> ToTextColorCssClass(this ColorType color)
	{
		return color switch
		{
			ColorType.Default => default,
			ColorType.Primary => "text-primary",
			ColorType.Secondary => "text-secondary",
			ColorType.Success => "text-success",
			ColorType.Danger => "text-danger",
			ColorType.Warning => "text-warning",
			ColorType.Info => "text-info",
			ColorType.Light => "text-light",
			ColorType.Dark => "text-dark",
			ColorType.Body => "text-body",
			ColorType.White => "text-white",
			ColorType.Transparent => default,
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(color), color, null)
				: default
		};
	}

	public static ReadOnlySpan<char> ToBorderColorCssClass(this ColorType color)
	{
		return color switch
		{
			ColorType.Default => default,
			ColorType.Primary => "border-primary",
			ColorType.Secondary => "border-secondary",
			ColorType.Success => "border-success",
			ColorType.Danger => "border-danger",
			ColorType.Warning => "border-warning",
			ColorType.Info => "border-info",
			ColorType.Light => "border-light",
			ColorType.Dark => "border-dark",
			ColorType.Body => "border-body",
			ColorType.White => "border-white",
			ColorType.Transparent => default,
			ColorType.Muted => default,
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(color), color, null)
				: null
		};
	}

	public static ReadOnlySpan<char> ToButtonColor(this ColorType color)
	{
		// Skips "btn-link" class
		return color switch
		{
			ColorType.Default => default,
			ColorType.Primary => "btn-primary",
			ColorType.Secondary => "btn-secondary",
			ColorType.Success => "btn-success",
			ColorType.Danger => "btn-danger",
			ColorType.Warning => "btn-warning",
			ColorType.Info => "btn-info",
			ColorType.Light => "btn-light",
			ColorType.Dark => "btn-dark",
			ColorType.Body => "btn-body",
			ColorType.White => "btn-white",
			ColorType.Transparent => "btn-transparent",
			ColorType.Muted => default,
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(color), color, null)
				: null
		};
	}

	// NOTE: Using a specific method (see ToTextColor(ColorType)) results in better memory efficiency due to constants optimization.
	public static string? ToCssClass(this ColorType color, ReadOnlySpan<char> prefix)
	{
		return color switch
		{
			ColorType.Default => default,
			ColorType.Primary => $"{prefix}primary",
			ColorType.Secondary => $"{prefix}secondary",
			ColorType.Success => $"{prefix}success",
			ColorType.Danger => $"{prefix}danger",
			ColorType.Warning => $"{prefix}warning",
			ColorType.Info => $"{prefix}info",
			ColorType.Light => $"{prefix}light",
			ColorType.Dark => $"{prefix}dark",
			ColorType.Body => $"{prefix}body",
			ColorType.White => $"{prefix}white",
			ColorType.Transparent => $"{prefix}transparent",
			ColorType.Muted => $"{prefix}muted",
			_ => Debugger.IsAttached
				? throw new ArgumentOutOfRangeException(nameof(color), color, null)
				: null
		};
	}
}
