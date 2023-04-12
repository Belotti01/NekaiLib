using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nekai.Common;

namespace Nekai.Blazor.Components;

[Flags]
public enum StyleSet
{
	None = 0,
	FlexAndCenterItems = 1,
	FlexAndStretchItems = 2,
	FlexAndSpaceItemsAround = 4,
	CenterSelf = 8,
	CenterSelf2D = 16
}

public static class StyleSetExtensions
{	
	public static ClassBuilder WithStyleSet(this ClassBuilder classBuilder, StyleSet styleSet)
	{
		if(styleSet == StyleSet.None)
			return classBuilder;

		if(styleSet.HasFlag(StyleSet.FlexAndCenterItems))
		{
			classBuilder
				.WithDisplay(DisplayType.Flex)
				.WithAlignItems(PositioningType.Center)
				.WithJustifyContent(PositioningType.Center);
		}

		
		
		switch(styleSet)
		{
			case StyleSet.None:
				return classBuilder;

			case StyleSet.FlexAndCenterItems:
				return classBuilder
					.WithDisplay(DisplayType.Flex)
					.WithAlignItems(PositioningType.Center)
					.WithJustifyContent(PositioningType.Center);
				
			case StyleSet.FlexAndStretchItems:
				return classBuilder
					.WithDisplay(DisplayType.Flex)
					.WithAlignItems(PositioningType.Stretch)
					.WithJustifyContent(PositioningType.Stretch);
				
			case StyleSet.FlexAndSpaceItemsAround:
				return classBuilder
					.WithDisplay(DisplayType.Flex)
					.WithAlignItems(PositioningType.Around)
					.WithJustifyContent(PositioningType.Around);

			case StyleSet.CenterSelf:
				return classBuilder
					.WithAlignSelf(PositioningType.Center)
					.WithMargin(MarginType.Auto, Dimension.X);

			default:
				Exceptor.ThrowIfDebug($"Invalid {nameof(StyleSet)} value: {styleSet}");
				return classBuilder;
		}
	}
	
}

