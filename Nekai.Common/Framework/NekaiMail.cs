using MailKit.Net.Smtp;
using MimeKit;
using Nekai.Common.Data;

namespace Nekai.Common;

/// <summary>
/// Send emails asynchronously.
/// </summary>
public static class NekaiMail
{
	private static readonly MailboxAddress _defaultSender = new("Nekai", "nekai@noreply.com");
	
	/// <summary>
	/// Send an email.
	/// </summary>
	/// <param name="from">The sender of the email.</param>
	/// <param name="to">The list of receivers of the email.</param>
	/// <param name="subject">The subject of the email.</param>
	/// <param name="body">The body of the email.</param>
	/// <param name="token">The <see cref="CancellationToken"/> for this operation.</param>
	/// <returns>The response from the server, or the error caught.</returns>
	public static async Task<MailOperationResult> TrySendMailAsync(MailboxAddress from, ICollection<MailboxAddress> to, string subject, string body, CancellationToken token = default)
	{
		using SmtpClient client = new();
		MimeMessage message = new();
		message.To.AddRange(to);
		message.From.Add(from);
		message.Subject = subject;
		message.Body = new TextPart()
		{
			Text = body
		};

		return await TrySendAsync(message, token);
	}

	public static async Task<MailOperationResult> TrySendMailAsync(MailboxAddress to, string subject, string body, CancellationToken token = default)
	{
		return await TrySendMailAsync(_defaultSender, [to], subject, body, token);
	}

	public static async Task<MailOperationResult> TrySendMailAsync(ICollection<MailboxAddress> to, string subject, string body, CancellationToken token = default)
	{
		return await TrySendMailAsync(_defaultSender, to, subject, body, token);
	}

	public static async Task<MailOperationResult> TrySendAsNekaiSenderAsync(MimeMessage message, CancellationToken token = default)
	{
		message.From.Add(_defaultSender);
		return await TrySendAsync(message, token);
	}

	public static async Task<MailOperationResult> TrySendAsync(MimeMessage message, CancellationToken token = default)
	{
		using SmtpClient client = new();
		try
		{
			string response = await client.SendAsync(message, token);
			return new(response, true);
		}
		catch(Exception ex)
		{
			string targets = string.Join(", ", message.To);
			NekaiLogs.Shared.Error("Couldn't send mail to [{targets}]: {ex}", targets, ex.Message);
			return new(ex.Message, false);
		}
	}
}	
