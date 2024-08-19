using System.Net.Mail;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nekai.Common;
using Serilog;

namespace Nekai.Razor;

public class NekaiEmailSender<TUser> : IEmailSender<TUser>
where TUser : NekaiUser
{
	[Inject]
	public ILogger Logger { get; set; }
	[Inject]
	public NekaiDbContext<TUser> Db { get; set; }
	
	public async Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
	{
		MailMessage mail = new()
		{
			From = new MailAddress("Nekai.Services@gmail.com"),
			To = { email },
			Subject = "Account Confirmation",
			Body = "Here is the confirmation link for your account: " + confirmationLink
		};
		
		try
		{
			var result = await NekaiSmtp.SendMailAsync(mail);
			if(result == NetworkOperationResult.Success)
			{
				Logger.Information("Confirmation link sent for user {user}: {link}", user.Email ?? user.UserName ?? "User", confirmationLink);
				return;
			}
		}catch { }

		Logger.Error("Confirmation email could not be sent to user {user}: {link} - using automatic confirmation.", user.Email ?? user.UserName ?? "User", confirmationLink);
		user.EmailConfirmed = true;
		Db.Update(user);
		await Db.SaveChangesAsync();
	}

	public async Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
	{
		MailMessage mail = new()
		{
			From = new MailAddress("Nekai.Services@gmail.com"),
			To = { email },
			Subject = "Password Reset",
			Body = "Here is the password reset link for your account: " + resetLink
		};

		try
		{
			var result = await NekaiSmtp.SendMailAsync(mail);
			if(result == NetworkOperationResult.Success)
			{
				Logger.Information("Password reset email sent for user {user}: {link}", user.Email ?? user.UserName ?? "User", resetLink);
				return;
			}
		}catch { }

		Logger.Error("Password reset email could not be sent to user {user}: {link}", user.Email ?? user.UserName ?? "User", resetLink);
	}

	public async Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
	{
		MailMessage mail = new()
		{
			From = new MailAddress("Nekai.Services@gmail.com"),
			To = { email },
			Subject = "Password Reset",
			Body = "Here is the password reset code for your account: " + resetCode
		};

		try
		{
			var result = await NekaiSmtp.SendMailAsync(mail);
			if(result == NetworkOperationResult.Success)
			{
				Logger.Information("Password reset code email sent for user {user}: {link}", user.Email ?? user.UserName ?? "User", resetCode);
				return;
			}
		}catch { }
		Logger.Error("Password reset code email could not be sent to user {user}: {link}", user.Email ?? user.UserName ?? "User", resetCode);
	}
}