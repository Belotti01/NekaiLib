using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security;
using System.Text.Json.Serialization;

namespace Nekai.Common;

public class NekaiSmtpConfiguration
{
	[Url]
	public string Url { get; set; } = "localhost";
	[Range(IPEndPoint.MinPort, IPEndPoint.MaxPort)]
	public int Port { get; set; } = 587;	// Defaults to the SSMTP port.
	public bool UseSsl { get; set; }
	public bool UseDefaultCredentials { get; set; }
	public string? UserName { get; set; }
	public SecureString? Password { get; set; }
}