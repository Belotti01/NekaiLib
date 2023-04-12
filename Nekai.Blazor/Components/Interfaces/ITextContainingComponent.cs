using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Nekai.Blazor.Components;

public interface ITextContainingComponent : IComponent
{
	public string? TextTooltip { get; set; }
	public bool NoWrap { get; set; }
	public TextType TextType { get; set; }
}
