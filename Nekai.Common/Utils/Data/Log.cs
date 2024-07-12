using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using DotNext.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Nekai.Common;

[JsonSerializable(typeof(Log))]
public class Log
{
	[JsonPropertyName("@t")]
	public DateTime TimeStamp { get; set; }
	[JsonPropertyName("@mt")]
	public string Message { get; set; } = "";
	[JsonPropertyName("@l")]
	public string TypeString { get; set; } = "";
	[JsonPropertyName("@tr")]
	public string? TR { get; set; }
	[JsonPropertyName("@sp")]
	public string? SP { get; set; }

	public LogLevel Type
	{
		get
		{
			var success = Enum.TryParse(TypeString, out LogLevel result);
			return success 
				? result 
				: LogLevel.None;
		}
	}

}
