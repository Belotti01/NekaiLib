using System.Text.Json.Serialization;
using Nekai.Common;

namespace Nekai.Blazor;

public struct NekaiStyle : INekaiStyle
{
	public string? Class { get; set; }
	public string? Style { get; set; }

	
}
