using Microsoft.AspNetCore.Components.Authorization;

namespace Nekai.Razor;

public class MissingAuthStateProviderException : NullReferenceException
{
	public MissingAuthStateProviderException()
		: base($"The {nameof(AuthenticationStateProvider)} could not be injected.")
	{ }
}