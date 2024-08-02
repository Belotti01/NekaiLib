using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Nekai.Razor;

internal sealed class IdentityUserAccessor(UserManager<IdentityUser<Guid>> userManager, IdentityRedirectManager redirectManager)
{
	public async Task<IdentityUser<Guid>> GetRequiredUserAsync(HttpContext context)
	{
		var user = await userManager.GetUserAsync(context.User);

		if(user is null)
		{
			redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
		}

		return user;
	}
}