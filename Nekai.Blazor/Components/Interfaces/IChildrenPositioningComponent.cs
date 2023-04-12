using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Nekai.Blazor.Components;

public interface IChildrenPositioningComponent : IComponent
{
	public PaddingType Padding { get; set; }
	public PaddingType XPadding { get; set; }
	public PaddingType YPadding { get; set; }
	public PositioningType ContentAlignment { get; set; }
	public FlexType Flex { get; set; }
}
