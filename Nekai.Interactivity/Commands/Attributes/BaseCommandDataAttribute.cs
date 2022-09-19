using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Interactivity;

public abstract class BaseCommandDataAttribute : Attribute {
	/// <summary>
	/// The primary name associated with this element.
	/// </summary>
	public string Name { get; protected set; }
	/// <summary>
	/// Alternative names that are associated with this element.
	/// </summary>
	public string[] Aliases { get; protected set; }
	/// <summary>
	/// Displayable description of this element.
	/// </summary>
	public string? Description { get; set; }

	public BaseCommandDataAttribute(string name, params string[] aliases) {
		Name = name;
		Aliases = aliases;
	}

	public string[] GetAllNames() {
		return Aliases.Prepend(Name).ToArray();
	}
}
