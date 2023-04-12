using Microsoft.Fast.Components.FluentUI;
using Nekai.Common;

namespace Nekai.Blazor;

public static class WebApplicationExtensions
{
	/// <summary>
	/// Register the services required by the Nekai.Blazor components.
	/// </summary>
	/// <remarks>
	/// Services registered by this method:
	/// <list type="bullet">
	///		<item>FluentUI</item>
	/// </list>
	/// </remarks>
	public static void AddNekaiBlazor(this IServiceCollection services)
    {
		// Reminder: Update the documentation when modifying this extension method to avoid duplicate
		// service registrations.
        services.AddFluentUIComponents();
    }
}
