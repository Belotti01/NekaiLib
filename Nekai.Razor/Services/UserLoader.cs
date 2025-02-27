using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Nekai.Razor;

public class UserLoader<TUser>(AuthenticationStateProvider auth, UserManager<TUser> userManager)
where TUser : class
{
	private AuthenticationState? _authState;
	
	public async Task<AuthenticationState> GetAuthStateAsync()
	{
		// Lazy-loaded.
		_authState ??= await auth.GetAuthenticationStateAsync();
		return _authState;
	}
	
	public async Task<TUser?> GetCurrentUserAsync()
	{
		var authState = await GetAuthStateAsync();
		var user = await userManager.GetUserAsync(authState.User);

		return user;
	}
	
	public async Task<Guid?> GetCurrentUserIdAsync()
	{
		var authState = await GetAuthStateAsync();
		var userIdString = userManager.GetUserId(authState.User);

		if(userIdString is null || !Guid.TryParse(userIdString, out var userId))
			return null;

		return userId;
	}
}