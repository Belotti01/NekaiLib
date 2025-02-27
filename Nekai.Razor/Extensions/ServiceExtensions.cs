using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using Nekai.Common;

namespace Nekai.Razor;

public static class ServiceExtensions
{

	public static IServiceCollection AddNekaiServices(this IServiceCollection services)
	{
		return services.AddScoped<SettingsManager>();
	}
	
}
