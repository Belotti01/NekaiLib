using MailKit.Net.Smtp;
using MimeKit;
using Nekai.Common.Data;

namespace Nekai.Common;

/// <summary>
/// Send emails asynchronously.
/// </summary>
public static class NekaiMail
{
	/// <summary>
	/// Send an email.
	/// </summary>
	/// <param name="from">The sender of the email.</param>
	/// <param name="to">The list of receivers of the email.</param>
	/// <param name="subject">The subject of the email.</param>
	/// <param name="body">The body of the email.</param>
	/// <param name="token">The <see cref="CancellationToken"/> for this operation.</param>
	/// <returns>The response from the server, or the error caught.</returns>
	public static async Task<MailOperationResult> SendMailAsync(MailboxAddress from, ICollection<MailboxAddress> to, string subject, string body, CancellationToken token = default)
	{
		using SmtpClient client = new();
		MimeMessage message = new();
		message.To.AddRange(to);
		message.From.Add(from);
		message.Subject = subject;
		message.Body = new TextPart(body);

		try
		{
			string response = await client.SendAsync(message, token);
			return new(response, true);
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Error("Couldn't send mail to {to}: {ex}", to, ex.Message);
			return new(ex.Message, false);
		}
	}

	public static async Task<MailOperationResult> SendMailAsync(MailboxAddress to, string subject, string body, CancellationToken token = default)
	{
		var from = new MailboxAddress("Nekai", "nekai@noreply.com");
		return await SendMailAsync(from, [to], subject, body, token);
	}

	public static async Task<MailOperationResult> SendMailAsync(ICollection<MailboxAddress> to, string subject, string body, CancellationToken token = default)
	{
		var from = new MailboxAddress("Nekai", "nekai@noreply.com");
		return await SendMailAsync(from, to, subject, body, token);
	}

}	
