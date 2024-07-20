using System.Configuration;

namespace Nekai.Common;

public static partial class CurrentEnvironment
{
	public static class App
	{
		public static Configuration ReadConfiguration(ConfigurationUserLevel level = ConfigurationUserLevel.PerUserRoamingAndLocal)
		{
			return ConfigurationManager
				.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
		}

		/// <inheritdoc cref="GCMemoryInfo.TotalCommittedBytes"/>
		public static long GetUsedHeapMemory()
		{
			return GC.GetGCMemoryInfo().TotalCommittedBytes;
		}

		/// <inheritdoc cref="GC.GetTotalMemory(bool)"/>
		public static long GetFreeHeapMemory()
		{
			return GC.GetTotalMemory(false);
		}

		/// <inheritdoc cref="GCMemoryInfo.HeapSizeBytes"/>
		public static long GetTotalHeapMemory()
		{
			return GC.GetGCMemoryInfo().HeapSizeBytes;
		}

		/// <summary>
		/// Check whether the memory usage of the last GC invocation is above the "High-load" threshold.
		/// </summary>
		public static bool IsMemoryUnderPressure()
		{
			var memoryInfo = GC.GetGCMemoryInfo();
			return memoryInfo.MemoryLoadBytes > memoryInfo.HighMemoryLoadThresholdBytes;
		}
	}
}