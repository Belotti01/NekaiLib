using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Fast.Components.FluentUI;

namespace Nekai.Blazor.Components;

public interface IStylizableComponent : IComponent
{
	public Borders Borders { get; set; }
}
