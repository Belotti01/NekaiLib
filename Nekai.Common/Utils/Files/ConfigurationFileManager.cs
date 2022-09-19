using System.Reflection;
using Nekai.Common.Reflection;

namespace Nekai.Common;

public abstract class ConfigurationFileManagerBase : IDisposable {
	protected string[] _commentsPrefixes = new string[] { "#", "//" };
	protected string Filepath { get; private set; }
	protected (MemberInfo member, ConfigurationAttribute attribute)[] ConfigurationMembers { get; private init; }
	
	public ConfigurationFileManagerBase(string filepath) {
		Filepath = filepath;
		ConfigurationMembers = GetType()
			.GetMembersWithAttribute<ConfigurationAttribute>(true)
			.Select(x => (x, x.GetCustomAttribute<ConfigurationAttribute>()!))
			.ToArray();

		foreach(var member in ConfigurationMembers.Select(x => x.member)) {
			var memberType = member.GetMemberReturnType();
			var isConvertible = memberType.GetInterface(nameof(IConvertible)) is not null;
			if(!isConvertible) {
				throw new InvalidTypeException(memberType, $"Type of Configuration Fields and Properties must inherit {nameof(IConvertible)}.");
			}
		}
	}


	public void Serialize() {
		List<string> lines = new();
		string key, value;

		foreach(var (member, attribute) in ConfigurationMembers) {
			key = attribute.Names.First();
			value = member.GetValue(this)?.ToString() ?? attribute.DefaultValue;
			if(_commentsPrefixes.Any() && !string.IsNullOrWhiteSpace(attribute.Description)) {
				lines.Add($"{_commentsPrefixes.First()} {attribute.Description.Trim()}");
			}
			lines.Add($"{key}: {value}");
		}

		File.WriteAllLines(Filepath, lines);
	}

	public void Deserialize() {
		var data = ToDictionary();
		string? name = null;
		object value;

		foreach(var (member, attribute) in ConfigurationMembers) {
			foreach(string possibleName in attribute.Names) {
				if(data.ContainsKey(possibleName)) {
					name = possibleName;
					break;
				}
			}

			if(name is null)
				continue;   // Value not found

			value = data[name].ConvertTo(member.GetMemberReturnType()) ?? attribute.DefaultValue;

			member.TrySetValue(this, value);
		}
	}

	protected Dictionary<string, string> ToDictionary() {
		Dictionary<string, string> data = new();
		if(!File.Exists(Filepath))
			return data;

		string[] lines = File.ReadAllLines(Filepath);
		string line;

		for(int i = 0; i < lines.Length; i++) {
			line = lines[i].Trim();
			if(_commentsPrefixes.Any(x => line.StartsWith(x)))
				continue;

			if(line.Contains(':')) {
				string[] parts = line.Split(':', 2, StringSplitOptions.TrimEntries);
				data.Add(parts[0], parts[1]);
			}
		}

		return data;
	}

	public override string ToString() {
		string[] lines = _ToLines();
		return string.Join(Environment.NewLine, lines);
	}

	public void Dispose() {
		Serialize();
		GC.SuppressFinalize(this);
	}

	
	private string[] _ToLines() {
		List<string> lines = new();
		string key, value;

		foreach(var (member, attribute) in ConfigurationMembers) {
			key = attribute.Names.First();
			value = member.GetValue(this)?.ToString() ?? attribute.DefaultValue;
			if(_commentsPrefixes.Any() && !string.IsNullOrWhiteSpace(attribute.Description)) {
				lines.Add($"{_commentsPrefixes.First()} {attribute.Description.Trim()}");
			}
			lines.Add($"{key}: {value}");
		}

		return lines.ToArray();
	}
}
