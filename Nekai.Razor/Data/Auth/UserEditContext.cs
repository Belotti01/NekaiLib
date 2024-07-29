using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Nekai.Razor;

public class UserEditContext
{
	[StringLength(16, MinimumLength = 4, ErrorMessage = "The Username must be between 4 and 16 characters long.")]
	public string Username { get; set; } = "";
	[StringLength(100, MinimumLength = 8, ErrorMessage = "The Username must be between 8 and 100 characters long.")]
	public string Password { get; set; } = "";
	[RegexStringValidator(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
	public string Email { get; set; } = "";
}