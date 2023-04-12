using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Blazor.Components;

[Flags]
public enum Dimension
{
	None = 0,
	Top = 1,
	End = 2,
	Bottom = 4,
	Start = 8,
	X = Start | End,
	Y = Top | Bottom,
	All = X | Y
}

public static class DimensionExtensions
{
	public static string ToCss(this Dimension dimensions, ReadOnlySpan<char> prefix, ReadOnlySpan<char> postfix)
	{
		if(dimensions == Dimension.None)
			return "";
		
		if(dimensions == Dimension.All)
			return $"{prefix}{postfix}";

		string css = "";
		
		if(dimensions.HasFlag(Dimension.X))
		{
			css = css._Append(prefix, 'x', postfix);
		}else
		{
			if(dimensions.HasFlag(Dimension.Start)) 
			{
				css = css._Append(prefix, 's', postfix);
			}
			else if(dimensions.HasFlag(Dimension.End))
			{
				css = css._Append(prefix, 'e', postfix);
			}
		}

		if(dimensions.HasFlag(Dimension.Y))
		{
			css += $"{prefix}y{postfix}";
		}else
		{
			if(dimensions.HasFlag(Dimension.Top))
			{
				css = css._Append(prefix, 't', postfix);
			}
			else if(dimensions.HasFlag(Dimension.Bottom))
			{
				css = css._Append(prefix, 'b', postfix);
			}
		}

		return css;
	}

	private static string _Append(this string css, ReadOnlySpan<char> prefix, char dimension, ReadOnlySpan<char> postfix)
	{
		if(css.Length == 0)
			return $"{prefix}{dimension}{postfix}";
		return $"{css} {prefix}{dimension}{postfix}";
	}
}
