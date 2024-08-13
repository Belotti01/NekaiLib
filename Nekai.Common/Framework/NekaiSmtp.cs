using System.Net;
using System.Net.Mail;

namespace Nekai.Common;

public static class NekaiSmtp
{
	/// <summary>
	/// Create an instance of <see cref="SmtpClient"/> using the stored SMTP configuration.
	/// </summary>
	public static SmtpClient CreateClientInstance()
	{
		var config = NekaiGeneralConfiguration.Singleton.Smtp;
		
		SmtpClient client = new()
		{
			Host = config.Url,
			Port = config.Port,
			EnableSsl = config.UseSsl,
			Credentials = string.IsNullOrWhiteSpace(config.UserName)
				? null
				: new NetworkCredential(config.UserName, config.Password),
			UseDefaultCredentials = config.UseDefaultCredentials
		};

		return client;
	}
	
	/// <summary> Request to configured SMTP server to send an email. </summary>
	/// <param name="message"> The message to send. </param>
	/// <param name="cancellationToken"> The token used to cancel the operation. </param>
	public static async Task<NetworkOperationResult> SendMailAsync(MailMessage message, CancellationToken cancellationToken = default)
	{
		try
		{
			using var client = CreateClientInstance();
			await client.SendMailAsync(message, cancellationToken);
			return NetworkOperationResult.Success;
		}
		catch(InvalidOperationException ex)
		{
			return NetworkOperationResult.BadFormat;
		}
		catch(SmtpException ex)
		{
			_LogSmtpFailure();
			return NetworkOperationResult.ServerError;
		}
		catch(Exception ex)
		{
			return NetworkOperationResult.BadFormat;
		}
	}

	/// <summary> Request to configured SMTP server to send an email. </summary>
	/// <param name="from"> The sender of the email. </param>
	/// <param name="recipients"> The recipients of the email. </param>
	/// <param name="subject"> The subject of the message to send. </param>
	/// <param name="body"> The body of the message to send. </param>
	/// <param name="cancellationToken"> The token used to cancel the operation. </param>
	public static async Task<NetworkOperationResult> SendMailAsync(string from, string recipients, string subject, string body, CancellationToken cancellationToken = default)
	{
		try
		{
			using var client = CreateClientInstance();
			await client.SendMailAsync(from, recipients, subject, body, cancellationToken);
			return NetworkOperationResult.Success;
		}
		catch(InvalidOperationException ex)
		{
			return NetworkOperationResult.BadFormat;
		}
		catch(SmtpException ex)
		{
			_LogSmtpFailure(ex.GetFullMessage());
			return NetworkOperationResult.ServerError;
		}
	}

	private static void _LogSmtpFailure(string? message = null)
	{
		NekaiLogs.Shared.Error("The SMTP server '{server}:{port}' failed to send an email. {message}", NekaiGeneralConfiguration.Singleton.Smtp.Url, NekaiGeneralConfiguration.Singleton.Smtp.Port, message);
	}
}