using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
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
    
    public static IServiceCollection AddNekaiDbAuthenticationStateProvider<TUser>(this IServiceCollection services)
    where TUser : NekaiUser
    {
        return services.AddScoped<AuthenticationStateProvider, NekaiDbAuthStateProvider<TUser>>();
    }

    /// <summary>
    /// Injects and configures the authentication schemes and cookies.
    /// </summary>
    public static AuthenticationBuilder AddNekaiAuthentication(this IServiceCollection services)
    {
        var builder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        }
        );
        builder.AddIdentityCookies();

        return builder;
    }
    
    public static IServiceCollection AddNekaiSmtp<TUser>(this IServiceCollection services)
    where TUser : NekaiUser
    {
        return services.AddSingleton<IEmailSender<TUser>, NekaiEmailSender<TUser>>();
    }

	public static WebApplication UseNekaiRazor(this WebApplication app)
    {
        return app;
    }

    public static IdentityBuilder AddNekaiIdentity<TDbContext, TUser>(this IServiceCollection services)
    where TDbContext : NekaiDbContext<TUser>
    where TUser : NekaiUser
    {
        services.AddScoped<IdentityRedirectManager>();
        return services.AddIdentityCore<TUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<TDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
    }
}
