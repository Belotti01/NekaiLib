using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using DotNext.Diagnostics;

namespace Nekai.Common;

[JsonSerializable(typeof(Log))]
public class Log
{
	[JsonPropertyName("@t")]
	public Timestamp TimeStamp { get; set; }
	[JsonPropertyName("@mt")]
	public string Message { get; set; } = "";
	[JsonPropertyName("@l")]
	public string TypeString { get; set; } = "";

	public LogKind Type => Enum.Parse<LogKind>(TypeString);

}
