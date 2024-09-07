using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Nekai.Razor;

public class NekaiDbAuthStateProvider<TUser> : ServerAuthenticationStateProvider
where TUser : NekaiUser
{
	[Inject]
	public NekaiDbContext<TUser> Db { get; set; }
	
	public const string LOGGED_IN_AUTH_TYPE = "LoggedIn";
	public const string ANONYMOUS_IN_AUTH_TYPE = "Anonymous";

	public AuthenticationState CurrentUser { get; protected set; }


	public NekaiDbAuthStateProvider()
	{
		CurrentUser = GetAnonymousAuthenticationState();
	}
	
	
	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		var authState = await base.GetAuthenticationStateAsync();
		var idClaim = authState.User.FindFirstValue(ClaimTypes.Sid);
		
		if(idClaim is null || authState.User.Identity is null)
			return CurrentUser = GetAnonymousAuthenticationState();
		
		return CurrentUser = await GetUserAuthenticationStateAsync(idClaim);
	}

	protected AuthenticationState GetAnonymousAuthenticationState()
	{
		var claims = GetAnonymousClaims();
		return new(claims);
	}

	protected async Task<AuthenticationState> GetUserAuthenticationStateAsync(string userId)
	{
		if(!Guid.TryParse(userId, out var guid) || guid == Guid.Empty)
			return GetAnonymousAuthenticationState();

		var user = await Db.Users.FindAsync(guid);
		if(user is null)	// User not found.
			return GetAnonymousAuthenticationState();

		var claims = GetUserClaims(user);
		return new(claims);
	}
	
	protected ClaimsPrincipal GetUserClaims(IdentityUser<Guid> user)
	{
		var identity = new ClaimsIdentity(new[]
		{
			new Claim(ClaimTypes.Sid, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? "User"),
			new Claim(ClaimTypes.Role, "User"),
			new Claim(ClaimTypes.Email, user.Email ?? "")
		}, LOGGED_IN_AUTH_TYPE);
		return new ClaimsPrincipal(identity);
	}
	
	protected ClaimsPrincipal GetAnonymousClaims()
	{
		var identity = new ClaimsIdentity(new[]
		{
			new Claim(ClaimTypes.Sid, Guid.Empty.ToString()),
			new Claim(ClaimTypes.Name, ANONYMOUS_IN_AUTH_TYPE),
			new Claim(ClaimTypes.Role, ANONYMOUS_IN_AUTH_TYPE),
			new Claim(ClaimTypes.Email, "")
		}, ANONYMOUS_IN_AUTH_TYPE);
		return new ClaimsPrincipal(identity);
	}
}