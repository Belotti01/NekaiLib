using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nekai.Common.Utils.Serializers;

[JsonSerializable(typeof(NekaiLog))]
public partial class NekaiLogsDeserializer : JsonSerializerContext
{
	public static NekaiLog Deserialize(string json)	
	{
		return JsonSerializer.Deserialize<NekaiLog>(json, Default.NekaiLog) ?? new();
	}
}