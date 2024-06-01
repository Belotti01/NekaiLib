using System.Configuration;

namespace Nekai.Common.Framework.ToolKit;

public static partial class CurrentEnvironment
{
	public static class Machine
	{
		public static Configuration ReadConfiguration()
			=> ConfigurationManager
				.OpenMachineConfiguration();
	}
}