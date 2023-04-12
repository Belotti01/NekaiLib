using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Nekai.Blazor.Components.Interfaces;

namespace Nekai.Blazor.Components;

public interface IButtonComponent : IClickableComponent
{
	public TargetType Target { get; set; }
	public ButtonActionType Type { get; set; }
}
