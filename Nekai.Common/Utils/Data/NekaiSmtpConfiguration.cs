using System.Security;
using System.Text.Json.Serialization;

namespace Nekai.Common;

[JsonSerializable(typeof(NekaiSmtpConfiguration))]
public class NekaiSmtpConfiguration : JsonSerializableObject<NekaiSmtpConfiguration>
{
	public string Url { get; set; } = "localhost";
	public int Port { get; set; } = 587;
	public bool UseTls { get; set; } = true;
	public bool UseSsl { get; set; }
	public string UserName { get; set; }
	public SecureString Password { get; set; }
	public bool UseDefaultCredentials { get; set; }
}