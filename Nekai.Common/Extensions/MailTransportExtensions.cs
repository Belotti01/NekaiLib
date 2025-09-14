using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using Nekai.Common.Data;

namespace Nekai.Common.Extensions;

public static class MailTransportExtensions
{
	
	private static readonly MailboxAddress _defaultSender = new("Nekai", "nekai@noreply.com");

	/// <summary>
	/// Attempt to send mail.
	/// </summary>
	/// <param name="client">The client to send the mail with.</param>
	/// <param name="from">The sender of the email.</param>
	/// <param name="to">The list of receivers of the email.</param>
	/// <param name="subject">The subject of the email.</param>
	/// <param name="body">The body of the email.</param>
	/// <param name="token">The <see cref="CancellationToken"/> for this operation.</param>
	/// <returns>The response from the server, or the error caught.</returns>
	public static async Task<MailOperationResult> TrySendMailAsync(this IMailTransport client, MailboxAddress from, ICollection<MailboxAddress> to, string subject, string body, CancellationToken token = default)
	{
		MimeMessage message = new();
		message.To.AddRange(to);
		message.From.Add(from);
		message.Subject = subject;
		message.Body = new TextPart()
		{
			Text = body
		};

		return await TrySendAsync(client, message, token);
	}

	/// <summary>
	/// Attempt to send mail.
	/// </summary>
	/// <param name="client">The client to send the mail with.</param>
	/// <param name="to">The receiver of the email.</param>
	/// <param name="subject">The subject of the email.</param>
	/// <param name="body">The text body of the email.</param>
	/// <param name="token">THe cancellation token.</param>
	/// <returns>A <see cref="MailOperationResult"/> containing information about the transaction.</returns>
	public static async Task<MailOperationResult> TrySendMailAsync(this IMailTransport client, MailboxAddress to, string subject, string body, CancellationToken token = default)
	{
		return await TrySendMailAsync(client, _defaultSender, [to], subject, body, token);
	}

	/// <summary>
	/// Attempt to send mail.
	/// </summary>
	/// <param name="client">The client to send the mail with.</param>
	/// <param name="to">The receiver of the email.</param>
	/// <param name="subject">The subject of the email.</param>
	/// <param name="body">The text body of the email.</param>
	/// <param name="token">THe cancellation token.</param>
	/// <returns>A <see cref="MailOperationResult"/> containing information about the transaction.</returns>
	public static async Task<MailOperationResult> TrySendMailAsync(this IMailTransport client, ICollection<MailboxAddress> to, string subject, string body, CancellationToken token = default)
	{
		return await TrySendMailAsync(client, _defaultSender, to, subject, body, token);
	}

	/// <summary>
	/// Attempt to send mail.
	/// </summary>
	/// <param name="message">The message to send.</param>
	/// <param name="client">The client to send the mail with.</param>
	/// <param name="token">THe cancellation token.</param>
	/// <returns>A <see cref="MailOperationResult"/> containing information about the transaction.</returns>
	public static async Task<MailOperationResult> TrySendAsNekaiSenderAsync(this IMailTransport client, MimeMessage message, CancellationToken token = default)
	{
		message.From.Add(_defaultSender);
		return await TrySendAsync(client, message, token);
	}

	/// <summary>
	/// Attempt to send mail.
	/// </summary>
	/// <param name="message">The message to send.</param>
	/// <param name="client">The client to send the mail with.</param>
	/// <param name="token">THe cancellation token.</param>
	/// <returns>A <see cref="MailOperationResult"/> containing information about the transaction.</returns>
	public static async Task<MailOperationResult> TrySendAsync(this IMailTransport client, MimeMessage message, CancellationToken token = default)
	{
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