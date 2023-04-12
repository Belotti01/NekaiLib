using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.HighPerformance.Buffers;
using Nekai.Common;

namespace Nekai.Blazor.Components;

public class ClassBuilder
{
	// Flags
	private bool _isButton;
	private bool _isTextColorSet;
	private bool _hasDarkBackground;

	private const string _CLASSES_PREFIX = "";
	private StringBuilder _Builder { get; }
	
	public ClassBuilder(string? baseClass = null)
	{
		_Builder = new(baseClass);
	}

	public ClassBuilder WithClass(ReadOnlySpan<char> @class)
	{
		Append(@class);
		return this;
	}

	public ClassBuilder WithBorders(Borders borders, int borderSize, ColorType borderColor = ColorType.Default)
	{
		// Borders="@Borders.All" is assumed if the border size is set, but the Borders property is not.
		// The same applies to the border color - if it's the only value set, assume that all borders should have size 1.
		if(borders == Borders.None)
		{
			if(borderSize == 0) 
			{
				if(borderColor == ColorType.Default)
					return this;
				borderSize = 1;
			}
			borders = Borders.All;
		}

		Append(borders.ToCssClass());
		Append($"borders-{Math.Clamp(borderSize, 1, 5)}");
		Append(borderColor.ToBorderColorCssClass());
		return this;
	}

	public ClassBuilder WithTextNoWrap(bool noWrap = true)
	{
		if(noWrap)
		{
			Append("text-nowrap");
		}
		return this;
	}

	public ClassBuilder WithDisplay(DisplayType type)
	{
		Append(type.ToCssClass());
		return this;
	}

	public ClassBuilder WithFontSize(TextType size)
	{
		Append(size.ToFontSizeCssClass());
		return this;
	}

	public ClassBuilder WithTextType(TextType type)
	{
		Append(type.ToHeaderTypeCssClass());
		return this;
	}

	public ClassBuilder WithMargin(MarginType type, Dimension dimensions = Dimension.All)
	{
		Append(type.ToCssClass(dimensions));
		return this;
	}

	public ClassBuilder WithPadding(PaddingType type, Dimension dimensions = Dimension.All)
	{
		Append(type.ToCssClass(dimensions));
		return this;
	}

	public ClassBuilder WithNoGutter(bool noGutter = true)
	{
		if(noGutter)
		{
			Append("g-0");
		}
		return this;
	}

	public ClassBuilder WithJustifyContent(PositioningType type)
	{
		Append(type.ToCss("justify-content"));
		return this;
	}

	public ClassBuilder WithAlignItems(PositioningType type)
	{
		Append(type.ToCss("align-items"));
		return this;
	}

	public ClassBuilder WithAlignSelf(PositioningType type)
	{
		Append(type.ToCss("align-self"));
		return this;
	}

	public ClassBuilder WithFlexFill(bool flexFill = true)
	{
		if(flexFill)
		{
			Append("flex-fill");
		}
		return this;
	}

	public ClassBuilder WithItemOrder(int? order)
	{
		if(order is not null)
		{
			Append($"order-{order}");
		}
		return this;
	}

	public ClassBuilder WithFlexGrowOrShrink(int? growValue)
	{
		if(growValue is null)
			return this;
		
		if(growValue > 0)
		{
			Append($"flex-grow-{growValue}");
		}else
		{
			Append($"flex-shrink-{-growValue}");
		}
		return this;
	}

	public ClassBuilder WithColSpan(int span)
	{
		if(span > 0)
		{
			Append($"col-{Math.Min(span, 12)}");
		}
		return this;
	}

	public ClassBuilder WithTextColor(ColorType textColor)
	{		
		Append(textColor.ToTextColorCssClass());
		_isTextColorSet = true;

		return this;
	}

	public ClassBuilder WithBackgroundColor(ColorType color)
	{
		Append(color.ToBackgroundCssClass());
		_hasDarkBackground = color.IsDarkBackgroundColor();

		// If no text color is set, and the background color is dark, automatically make the text light-colored.
		// It can still be overridden, so the call order doesn't affect the result.
		if(!_hasDarkBackground || _isTextColorSet)
			return this;

		return WithTextColor(ColorType.Light);
	}

	public ClassBuilder WithResponsiveColSpan(int xs, int sm, int md, int lg, int xl, int xxl)
	{
		Append($"col-xs-{xs} col-sm-{sm} col-md-{md} col-lg-{lg} col-xl-{xl} col-xxl-{xxl}");
		return this;
	}

	public ClassBuilder WithResponsiveColSpan(int[]? values)
	{
		if(values is null)
			return this;

		return values.Length switch
		{
			0 => this,
			1 => WithResponsiveColSpan(values[0], values[0], values[0], values[0], values[0], values[0]),
			2 => WithResponsiveColSpan(values[0], values[0], values[1], values[1], values[1], values[1]),
			3 => WithResponsiveColSpan(values[0], values[0], values[1], values[1], values[2], values[2]),
			4 => WithResponsiveColSpan(values[0], values[1], values[2], values[2], values[3], values[3]),
			5 => WithResponsiveColSpan(values[0], values[1], values[2], values[3], values[4], values[4]),
			_ => WithResponsiveColSpan(values[0], values[1], values[2], values[3], values[4], values[5]),
		};
	}

	public ClassBuilder WithButton(ColorType buttonColor = ColorType.Default)
	{
		if(!_isButton)
		{
			Append("btn");
			_isButton = true;
		}
		_hasDarkBackground = buttonColor.IsDarkButtonColor();
		Append(buttonColor.ToButtonColor());

		return this;
	}

	protected virtual void Append(string? value)
	{
		if(value is null)
			return;
		Append(value.AsSpan());
	}

	protected virtual void Append(ReadOnlySpan<char> value)
	{
		if(value.IsEmpty)
			return;
		
		// Pre-allocate memory to avoid resizing the StringBuilder more than once
		if(_Builder.Length > 0)
		{
			_Builder.EnsureCapacity(_Builder.Length + value.Length + _CLASSES_PREFIX.Length + 1);
			_Builder.Append(' ');
		}
		else
		{
			_Builder.EnsureCapacity(_Builder.Length + value.Length + _CLASSES_PREFIX.Length);
		}
		
		_Builder
			.Append(_CLASSES_PREFIX)
			.Append(value);
	}

	public string Build()
	{
		return _Builder.ToString();
	}
}
