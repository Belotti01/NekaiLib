using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Nekai.Razor;

public static class AuthenticationExtensions
{
	/// <summary>
	/// Get the currently logged in user's ID as a <see langword="string"/>.
	/// </summary>
	/// <param name="auth"> The claims' source. </param>
	/// <returns> The currently logged in user's ID, or <see langword="null"/> if the user is not authenticated. </returns>
	public static async Task<string?> GetCurrentUserIdAsync(this AuthenticationStateProvider auth)
	{
		var authState = await auth.GetAuthenticationStateAsync();

		var claimsIdentity = (ClaimsIdentity?)authState.User.Identity;
		var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
		var userIdString = claim?.Value;

		return userIdString;
	}

	/// <summary>
	/// Get the currently logged in user's ID as a <typeparamref name="TKey"/>.
	/// </summary>
	/// <typeparam name="TKey"> The parsable type of the ID. </typeparam>
	/// <param name="auth"> The claims' source. </param>
	/// <param name="format"> The format used for parsing. </param>
	/// <returns> The currently logged in user's ID, or <see langword="null"/> if the user is not authenticated. </returns>
	public static async Task<TKey?> GetCurrentUserIdAsync<TKey>(this AuthenticationStateProvider auth, IFormatProvider? format = null)
		where TKey : IParsable<TKey>
	{
		var userIdString = await GetCurrentUserIdAsync(auth);
		if(userIdString is null)
			return default;

		if(!TKey.TryParse(userIdString, format, out TKey? userId))
			return default;

		return userId;
	}
}
