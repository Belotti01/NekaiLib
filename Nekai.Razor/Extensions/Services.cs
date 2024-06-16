using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nekai.Razor;

public static class Services
{
    public static IServiceCollection AddNekaiRazorComponents(this IServiceCollection services)
    {
        services.AddHttpClient();    // For compatibility with Blazor Server.
        services.AddFluentUIComponents();
        return services;
	}

	public static WebApplication UseNekaiRazor(this WebApplication app)
    {
        return app;
    }
}
