using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Nekai.Blazor.Components;

public interface ISelfPositioningComponent : IComponent
{
	public PositioningType Alignment { get; set; }
	public int? FlexSize { get; set; }
	public int? ItemOrder { get; set; }
	public bool FlexFill { get; set; }

	public MarginType Margin { get; set; }
	public MarginType XMargin { get; set; }
	public MarginType YMargin { get; set; }
}
